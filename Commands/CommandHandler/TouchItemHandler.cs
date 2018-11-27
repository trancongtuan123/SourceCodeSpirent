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
    class TouchItemHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public TouchItemHandler(RepoGenBaseFolder repo)
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
                string action = (string)arguments.GetValueOrDefault<string, object>("itest-action");
                string loc = (string)arguments.GetValueOrDefault<string, object>("elementlocation");
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
                if (loc == default(string))
                {
                    loc = "0;0";
                }

                if (target != default(string))
                {
                    repoItemInfo = CreateItemInfo(repo, target, timeout);
                    Ranorex.Unknown adapter = CreateAdapter(repoItemInfo);
                    switch (action)
                    {
                        case "Touch":
                            Report.Log(ReportLevel.Info, "Touch", "Touch item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.Touch(loc, duration);
                            break;
                        case "DoubleTap":
                            Report.Log(ReportLevel.Info, "Touch", "DoubleTap item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.DoubleTap(loc);
                            break;
                        case "LongTouch":
                            Report.Log(ReportLevel.Info, "Touch", "LongTouch item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.LongTouch(loc, duration);
                            break;
                        case "TouchStart":
                            Report.Log(ReportLevel.Info, "Touch", "TouchStart item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.TouchStart(loc);
                            break;
                        case "TouchMove":
                            Report.Log(ReportLevel.Info, "Touch", "TouchMove item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.TouchMove(loc, duration);
                            break;
                        case "TouchEnd":
                            Report.Log(ReportLevel.Info, "Touch", "TouchEnd item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.TouchEnd(loc, duration);
                            break;
                    }
                    if (retrieveImage)
                    {
                        stepResponse.image64 = GetImage64(adapter);
                    }
                }
                else
                {
                    

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
