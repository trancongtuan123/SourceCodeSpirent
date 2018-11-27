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
    class KeyShortcutHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public KeyShortcutHandler(RepoGenBaseFolder repo)
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
                string keyData = (string)arguments.GetValueOrDefault<string, object>("data");
                int duration = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("duration"));
                int delay = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("delay"));
                int timeout = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("timeout"));
                int scanCode = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("scanCode"));
                int pressTime = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("pressTime"));
                bool retrieveImage = (bool)arguments.GetValueOrDefault<string, object>("retrieveImage");
                bool setModifiers = (bool)arguments.GetValueOrDefault<string, object>("setModifiers");

                string[] keys = keyData.Trim().Split(',');
                System.Windows.Forms.Keys key = (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), keys[0]);

                for (int i = 1; i < keys.Length; i++)
                {
                    key |= (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), keys[i]);
                }

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
                    Report.Log(ReportLevel.Info, "Keyboard", "Key Press", new RecordItemIndex(Agent.EXECUTION_INDEX));

                    Keyboard.PrepareFocus(adapter);
                    Keyboard.Press(key, scanCode, pressTime, 1, setModifiers);

                    if (retrieveImage)
                    {
                        stepResponse.image64 = GetImage64(adapter);
                    }
                }
                else
                {
                    Report.Log(ReportLevel.Info, "Keyboard", "Key Press", new RecordItemIndex(Agent.EXECUTION_INDEX));
                    Keyboard.Press(key, scanCode, pressTime, 1, setModifiers);
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
