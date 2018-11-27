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
    class UnknownItemHandler : RanorexItemHandler
    {

        public UnknownItemHandler()
        {

        }

        public override RanorexStepExecutionResponse Execute(Dictionary<string, object> arguments)
        {
            RanorexStepExecutionResponse stepResponse = new RanorexStepExecutionResponse();
            stepResponse.success = false;
            stepResponse.message = "Ranorex Exception: " + "Unknown action found! Cannot execute";
            return stepResponse;
        }
    }
}
