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
using System.Windows.Forms;

namespace RxAgent.Commands.CommandHandler
{
    class OpenBrowserItemHandler : RanorexItemHandler
    {

        public OpenBrowserItemHandler()
        {

        }

        public override RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments)
        {
            RanorexStepExecutionResponse stepResponse = new RanorexStepExecutionResponse();
            try
            {
                string url = (string)arguments["command"];
                string browser = (string)arguments["browserType"];
                bool maximized = (bool)arguments["maximized"];

                Host.Local.OpenBrowser(url, browser, "", false, maximized);
                Delay.Milliseconds(2000);
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
