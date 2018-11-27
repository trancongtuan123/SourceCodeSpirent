using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RxAgent
{
    class AgentResponse
    {
        public enum MessageType { CAPTURE_STEP_REPONSE, EXECUTION_STEP_RESPONSE, TERMINATION_RESPONSE }

        public MessageType messageType;
        public RanorexStepExecutionResponse stepExecutionResponse;
        public RanorexStepCaptureReponse stepCaptureReponse;

    }
}
