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
    class MouseWheelItemHandler : RanorexItemHandler
    {

        public MouseWheelItemHandler()
        {

        }

        public override RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments)
        {
            RanorexStepExecutionResponse stepResponse = new RanorexStepExecutionResponse();
            try
            {
                string orientation = (string)arguments.GetValueOrDefault<string, object>("orientation");
                int delta = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("delta"));
                int delay = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("delay"));

                Report.Log(ReportLevel.Info, "Mouse", "Mouse Scroll "+orientation+" by "+delta+" units.", new RecordItemIndex(Agent.EXECUTION_INDEX));

                if (orientation == "Vertical")
                {
                    Mouse.ScrollWheel(delta);

                }
                else if (orientation == "Horizontal")
                {
                    Mouse.ScrollHorizontalWheel(delta);
                }
                else
                {
                    throw new Exception("Invalid orientation!");
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
