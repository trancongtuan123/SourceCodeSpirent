using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Windows;
using System.Runtime;
using System.Runtime.CompilerServices;

using JsonExSerializer;

using RxAgent.Commands;

namespace RxAgent
{
    class AgentServer
    {
        private static byte[] handshakeBytes = { 4, 8, 15, 16, 23, 42 };
        private static int AGENT_TIMEOUT_MS = 1000 * 25;

        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private bool standalone;
        private int port;

        private NetworkStream stream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        private System.Timers.Timer timeoutTimer;
        private int pingTimeout;

        private readonly object writeLock = new object();
        private readonly object pingLock = new object();

        public delegate void NewCommand(Command command);

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event NewCommand OnCommand;

        private bool clientConnected = false;
        private bool isStarted = false;

        public AgentServer(bool standalone, int port)
        {
            this.standalone = standalone;
            this.port = port;
        }

        public void Open()
        {
            Thread startThread = new Thread(NetLoop);
            startThread.SetApartmentState(ApartmentState.STA);
            startThread.Start();
        }

        public void Start()
        {
            isStarted = true;
        }

        public void Ping()
        {
            lock (pingLock)
            {
                pingTimeout = 0;
            }
        }

        public void SendMessage(object obj)
        {
            Serializer serializer = new Serializer(obj.GetType());
            lock (writeLock)
            {
                if (streamWriter == null)
                {
                    return;
                }
                try
                {
                    serializer.Serialize(obj, streamWriter);
                    streamWriter.Flush();
                }
                catch (Exception e)
                {

                }
            }
        }

        private void StartTimeoutTimer(int timeout)
        {
            if (timeoutTimer != null)
            {
                timeoutTimer.Stop();
                timeoutTimer.Dispose();
            }
            timeoutTimer = new System.Timers.Timer();
            timeoutTimer.Elapsed += (source, e) =>
            {
                lock (pingLock)
                {
                    pingTimeout += 2000;
                    if (pingTimeout >= timeout)
                    {
                        DropClient();
                    }
                }
            };
            timeoutTimer.AutoReset = true;
        }

        public void DropClient()
        {
            if (tcpClient != null)
            {
                clientConnected = false;

                tcpClient.GetStream().Close();
                tcpClient.Close();
                tcpClient = null;
            }

            timeoutTimer.Stop();
            timeoutTimer.Dispose();

            Disconnected(this, new EventArgs());
        }

        private void NetLoop()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();

                if (!standalone)
                {
                    StartTimeoutTimer(AGENT_TIMEOUT_MS);
                }

                Console.WriteLine("SUCCESS: Server started");

                while (true)
                {
                    TcpClient socket = tcpListener.AcceptTcpClient();

                    if (tcpClient == null)
                    {

                        stream = socket.GetStream();

                        streamReader = new StreamReader(stream);
                        streamWriter = new StreamWriter(stream);

                        stream.Write(handshakeBytes, 0, handshakeBytes.Length);
                        streamWriter.Flush();

                        Thread receiverThread = new Thread(ReceiverDelegate);
                        receiverThread.IsBackground = true;
                        receiverThread.Start();

                        if (standalone)
                        {
                            StartTimeoutTimer(AGENT_TIMEOUT_MS);
                        }

                        tcpClient = socket;
                    }
                    else
                    {
                        tcpClient.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("FAIL: " + e.Message);
                Application.Exit();
            }
        }

        public void ReceiverDelegate()
        {
            clientConnected = true;
            Serializer serializer = new Serializer(typeof(Command));
            while (clientConnected)
            {

                try
                {
                    Command command = (Command)serializer.Deserialize(streamReader);

                    while (!isStarted)
                    {
                        Thread.Sleep(1000);
                    }

                    if (command != null)
                    {
                        OnCommand(command);
                    }
                    else
                    {
                        clientConnected = false;
                    }
                }
                catch (Exception e)
                {
                    clientConnected = false;
                }
            }

            if (!standalone)
            {
                ExitApplication();
            }

        }

        private void ExitApplication()
        {
            if (tcpClient != null)
            {
                tcpClient.GetStream().Close();
                tcpClient.Close();
                tcpListener.Stop();
            }
            Environment.Exit(0);
        }
    }
}
