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
    class CloseApplicationItemHandler : RanorexItemHandler
    {

        private RepoGenBaseFolder repo;

        public CloseApplicationItemHandler(RepoGenBaseFolder repo)
        {
            this.repo = repo;
        }

        public override RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments)
        {
            RanorexStepExecutionResponse stepResponse = new RanorexStepExecutionResponse();
            try
            {
                RepoItemInfo repoItemInfo = null;
                Ranorex.Unknown adapter = null;

                string target = (string)arguments["target"];
                string method = (string)arguments["method"];
                int gracePeriod = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("gracePeriod"));
                int timeout = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("timeout"));

                if (timeout == default(int))
                {
                    timeout = 10000;
                }

                if (target != default(string))
                {
                    repoItemInfo = CreateItemInfo(repo, target, timeout);
                    adapter = CreateAdapter(repoItemInfo);
                }
                else
                {
                    throw new Exception("No target defined!");
                }

                if (method == "CloseWindow")
                {
                    Host.Local.CloseApplication(adapter, new Duration(gracePeriod));
                }
                else if (method == "KillProcess")
                {
                    Host.Local.KillApplication(adapter);
                }
                
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
