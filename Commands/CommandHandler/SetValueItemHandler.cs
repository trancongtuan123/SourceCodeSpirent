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
    class SetValueItemHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public SetValueItemHandler(RepoGenBaseFolder repo)
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

                string target = (string)arguments.GetValueOrDefault<string, object>("target");
                string name = (string)arguments.GetValueOrDefault<string, object>("name");
                string value = (string)arguments.GetValueOrDefault<string, object>("value");

                int timeout = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("timeout"));
                bool retrieveImage = (bool)arguments.GetValueOrDefault<string, object>("retrieveImage");

                if (timeout == default(int))
                {
                    timeout = 10000;
                }

                if (target != default(string))
                {
                    repoItemInfo = CreateItemInfo(repo, target, timeout);
                    adapter = CreateAdapter(repoItemInfo);

                    adapter.Element.SetAttributeValue(name, value);
                    if (retrieveImage)
                    {
                        stepResponse.image64 = GetImage64(adapter);
                    }
                    stepResponse.message = "";
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
       
    }


}
