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
    class MouseItemHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public MouseItemHandler(RepoGenBaseFolder repo)
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
                string button = (string)arguments.GetValueOrDefault<string, object>("button");
                string loc = (string)arguments.GetValueOrDefault<string, object>("location");
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
                if (button == default(string))
                {
                    button = "Unknown";
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
                        case "Click":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse " + button + " Click item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.Click(GetButton(button), loc, duration);
                            break;
                        case "DoubleClick":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse " + button + " Double Click item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.DoubleClick(GetButton(button), loc, duration);
                            break;
                        case "Move":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse move to item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.MoveTo(loc, duration);
                            break;
                        case "Up":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse " + button + " Up item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.MoveTo(loc, duration);
                            Mouse.ButtonUp(GetButton(button));
                            break;
                        case "Down":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse " + button + " Down item at " + loc + ".", repoItemInfo, new RecordItemIndex(Agent.EXECUTION_INDEX));
                            adapter.MoveTo(loc, duration);
                            Mouse.ButtonDown(GetButton(button));
                            break;
                    }
                    if (retrieveImage)
                    {
                        stepResponse.image64 = GetImage64(adapter);
                    }
                }
                else
                {
                    string[] locSplit = loc.Split(';');
                    switch (action)
                    {
                        case "Click":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse " + button + " Click at {X=" + locSplit[0] + ",Y=" + locSplit[1] + "}.", new RecordItemIndex(Agent.EXECUTION_INDEX));
                            Mouse.MoveTo(Int32.Parse(locSplit[0]), Int32.Parse(locSplit[1]), duration);
                            Mouse.Click(GetButton(button));
                            break;
                        case "DoubleClick":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse " + button + " Double Click at {X=" + locSplit[0] + ",Y=" + locSplit[1] + "}.", new RecordItemIndex(Agent.EXECUTION_INDEX));
                            Mouse.MoveTo(Int32.Parse(locSplit[0]), Int32.Parse(locSplit[1]), duration);
                            Mouse.DoubleClick(GetButton(button));
                            break;
                        case "Move":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse Move at {X=" + locSplit[0] + ",Y=" + locSplit[1] + "}.", new RecordItemIndex(Agent.EXECUTION_INDEX));
                            Mouse.MoveTo(Int32.Parse(locSplit[0]), Int32.Parse(locSplit[1]), duration);
                            break;
                        case "Up":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse Up at {X=" + locSplit[0] + ",Y=" + locSplit[1] + "}.", new RecordItemIndex(Agent.EXECUTION_INDEX));
                            Mouse.MoveTo(Int32.Parse(locSplit[0]), Int32.Parse(locSplit[1]), duration);
                            Mouse.ButtonUp(GetButton(button));
                            break;
                        case "Down":
                            Report.Log(ReportLevel.Info, "Mouse", "Mouse Down at {X=" + locSplit[0] + ",Y=" + locSplit[1] + "}.", new RecordItemIndex(Agent.EXECUTION_INDEX));
                            Mouse.MoveTo(Int32.Parse(locSplit[0]), Int32.Parse(locSplit[1]), duration);
                            Mouse.ButtonDown(GetButton(button));
                            break;
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
