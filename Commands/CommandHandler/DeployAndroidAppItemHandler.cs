using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ranorex;
using Ranorex.Controls;
using Ranorex.Core;
using Ranorex.Core.Recorder;
using Ranorex.Core.FastXml;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;
using Ranorex.Core.Remoting;
using Ranorex.Core.Remoting.RecordItems;
using Ranorex.Core.Reporting;
using Ranorex.Plugin.Mobile.Device;
using System.Windows.Forms;

namespace RxAgent.Commands.CommandHandler
{
    class DeployAndroidAppItemHandler : RanorexItemHandler
    {

        public DeployAndroidAppItemHandler()
        {

        }

        public override RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments)
        {
            RanorexStepExecutionResponse stepResponse = new RanorexStepExecutionResponse();
            try
            {

                string command = (string)arguments["command"];
                string application = (string)arguments["application"];

                if (command == default(string))
                {
                    throw new Exception("No device selected!");
                }
                if (application == default(string))
                {
                    throw new Exception("No application to deploy!");
                }
                Host.Local.InstrumentAndDeployAndroidApp(command, application, true, Host.DeploymentModes.Auto, 600000, new Ranorex.Core.Remoting.RecordItems.InstrumentApkOptions() { EnableWebTesting = true, TreeSimplification = true, FullImageComparison = false, RIdClass = "", KeyStore = "", KeyAlias = "", JdkBinPath = "", KeyStorePass = "", KeyPass = "" });

                Delay.Milliseconds(400);
                stepResponse.success = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                stepResponse.message = "Ranorex Exception: " + e.Message;
                stepResponse.success = false; 
            }
        
            return stepResponse;
        }
    }
}
