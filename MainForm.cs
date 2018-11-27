using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

using Ranorex.Controls;

namespace RxAgent
{
    public partial class MainForm : Form
    {
        private int port;
        private bool standalone;
        private Agent agent;

        public MainForm(int port, bool standalone)
        {
            this.port = port;
            this.standalone = standalone;
            InitializeComponent();
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;

            agent = new Agent(port, standalone);
            agent.Open();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Hide();
            agent.Start();
        }

        private void FormLoad(object sender, EventArgs e)
        {

        }
    
    }
}
