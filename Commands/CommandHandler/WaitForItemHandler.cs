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
    class WaitForItemHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public WaitForItemHandler(RepoGenBaseFolder repo)
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
                string waitAction = (string)arguments.GetValueOrDefault<string, object>("waitAction");
                int waitTimeout = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("waitTimeout"));
                int timeout = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("timeout"));

                if (waitAction == default(string))
                {
                    throw new Exception("No wait action defined!");
                }

                if (target != default(string))
                {
                    repoItemInfo = CreateItemInfo(repo, target, timeout);
                }

                if (repoItemInfo != null)
                {
                    switch (waitAction)
                    {
                        case "Exists":
                            repoItemInfo.WaitForExists(waitTimeout);
                            break;
                        case "NotExists":
                            repoItemInfo.WaitForNotExists(waitTimeout);
                            break;
                    }
                }
                else
                {
                    throw new Exception("No target defined!");
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

        private System.Windows.Forms.MouseButtons GetButton(string value)
        {
            switch (value)
            {
                case "Left":
                    return System.Windows.Forms.MouseButtons.Left;
                case "Middle":
                    return System.Windows.Forms.MouseButtons.Middle;
                case "None":
                    return System.Windows.Forms.MouseButtons.None;
                case "Right":
                    return System.Windows.Forms.MouseButtons.Right;
                case "XButton1":
                    return System.Windows.Forms.MouseButtons.XButton1;
                case "XButton2":
                    return System.Windows.Forms.MouseButtons.XButton2;
            }
            return System.Windows.Forms.MouseButtons.None;
        }
    }


}
