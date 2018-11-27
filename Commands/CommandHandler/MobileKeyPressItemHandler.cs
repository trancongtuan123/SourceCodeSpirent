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
    class MobileKeyPressItemHandler : RanorexItemHandler
    {

        private RepoGenBaseFolder repo;

        public MobileKeyPressItemHandler(RepoGenBaseFolder repo)
        {
            this.repo = repo;
        }

        public override RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments)
        {
            RanorexStepExecutionResponse stepResponse = new RanorexStepExecutionResponse();
            try
            {
                RepoItemInfo repoItemInfo = null;

                string target = (string)arguments.GetValueOrDefault<string, object>("target");
                string key = (string)arguments.GetValueOrDefault<string, object>("key");
                int duration = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("duration"));
                int delay = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("delay"));
                int timeout = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("timeout"));
                bool retrieveImage = (bool)arguments.GetValueOrDefault<string, object>("retrieveImage");

                if (timeout == default(int))
                {
                    timeout = 10000;
                }

                if (duration == default(int))
                {
                    duration = 0;
                }

                if (target != default(string))
                {
                    repoItemInfo = CreateItemInfo(repo, target, timeout);
                    Ranorex.Unknown adapter = CreateAdapter(repoItemInfo);
                    Report.Log(ReportLevel.Info, "Keyboard", "Key press '" + key + "' with focus on UI Element.", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                    adapter.PressKeys(key, duration);
                    if (retrieveImage)
                    {
                        stepResponse.image64 = GetImage64(adapter);
                    }        
                }
                
                Delay.Milliseconds(200);

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
