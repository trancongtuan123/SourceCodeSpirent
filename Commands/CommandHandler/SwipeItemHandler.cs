using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ranorex;
using Ranorex.Controls;
using Ranorex.Core;
using Ranorex.Core.Recorder.Touch;
using Ranorex.Core.Recorder;
using Ranorex.Core.FastXml;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;


namespace RxAgent.Commands.CommandHandler
{
    class SwipeItemHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public SwipeItemHandler(RepoGenBaseFolder repo)
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
                string action = (string)arguments.GetValueOrDefault<string, object>("action");
                string startLocation = (string)arguments.GetValueOrDefault<string, object>("startlocation");
                int distance = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("distance"));
                double direction = Convert.ToDouble(arguments.GetValueOrDefault<string, object>("direction"));
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
                if (startLocation == default(string))
                {
                    startLocation = "0;0";
                }

                if (target != default(string))
                {
                    repoItemInfo = CreateItemInfo(repo, target, timeout);
                    Ranorex.Unknown adapter = CreateAdapter(repoItemInfo);
                   
                    Report.Log(ReportLevel.Info, "Touch Gestures", "Swipe gesture with direction '" + direction + "' starting from '" + startLocation + "' with distance '" + distance + "' with swipe duration '" + duration + "'", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                    adapter.Swipe(startLocation, direction, distance, duration, 0);

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
