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
    class KeySequenceHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public KeySequenceHandler(RepoGenBaseFolder repo)
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
                string sequence = (string)arguments.GetValueOrDefault<string, object>("keySequence");
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
                    Report.Log(ReportLevel.Info, "Keyboard", "Key '" + sequence + "' Press with focus on UI Element.", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));

                    adapter.PressKeys(sequence, duration);
                    if (retrieveImage)
                    {
                        stepResponse.image64 = GetImage64(adapter);
                    }
                }
                else
                {
                    Report.Log(ReportLevel.Info, "Keyboard", "Key '" + sequence + "' Press.", new RecordItemIndex(Agent.EXECUTION_INDEX));
                    Keyboard.Press(sequence, duration);
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
