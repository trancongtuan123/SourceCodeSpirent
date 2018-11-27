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


namespace RxAgent.Commands.CommandHandler
{
    class RunApplicationItemHandler : RanorexItemHandler
    {

        public RunApplicationItemHandler()
        {

        }

        public override RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments)
        {
            RanorexStepExecutionResponse stepResponse = new RanorexStepExecutionResponse();
            try
            {

                string command = (string)arguments["command"];
                string args = (string)arguments["arguments"];
                bool maximized = (bool)arguments["maximized"];

                if (command == default(string))
                {
                    throw new Exception("No file name defined to run!");
                }
                Host.Local.RunApplication(command, args, "", maximized);
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
