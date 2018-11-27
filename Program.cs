using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using RxAgent.Commands;
using Ranorex;
using Ranorex.Controls;
using Ranorex.Core;
using Ranorex.Core.Recorder;
using Ranorex.Core.FastXml;
using Ranorex.Core.Repository;
using JsonExSerializer;


namespace RxAgent
{
    static class Program
    {
        public static MainForm form;

        static string dllPath = "";

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);

            string assembliesDir = dllPath;
            string name = assemblyName.Name;
            if (name.Contains("resources"))
            {
                return null;
            }
            string path = assembliesDir + name + ".dll";
            return Assembly.LoadFrom(path);
        }

        [STAThread]
        static void Main(string[] argv) {
            argv = new string[3];
            argv[0] = "58403";
            argv[1] = "C:\\Program Files (x86)\\Ranorex 8.1\\Bin\\";
            argv[2] = "";
            if (argv.Length < 2)
                return;

            bool standalone = false;
            if (argv.Length == 3)
            {
                if (argv[2] == "standalone")
                {
                    standalone = true;
                }
            }

            dllPath = argv[1];

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form = new MainForm(Int32.Parse(argv[0]), standalone));
        }
     
    }
}
