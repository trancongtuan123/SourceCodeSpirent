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
    class RunMobileAppItemHandler : RanorexItemHandler
    {

        public RunMobileAppItemHandler()
        {

        }

        public override RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments)
        {
            RanorexStepExecutionResponse stepResponse = new RanorexStepExecutionResponse();
            try
            {

                string command = (string)arguments["command"];
                string mobileApp = (string)arguments["application"];

                if (command == default(string))
                {
                    throw new Exception("No device selected!");
                }
                if (mobileApp == default(string))
                {
                    throw new Exception("No application to run selected!");
                }
                Host.Local.RunMobileApp(command, mobileApp);
                
                Delay.Milliseconds(400);
                stepResponse.success = true;
            }
            catch (Exception e)
            {
                stepResponse.message = "Ranorex Exception: " + e.Message;
                stepResponse.success = false; 
            }
        
            return stepResponse;
        }
    }
}
