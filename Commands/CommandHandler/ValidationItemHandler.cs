using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Ranorex;
using Ranorex.Controls;
using Ranorex.Core;
using Ranorex.Core.Recorder;
using Ranorex.Core.FastXml;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;


namespace RxAgent.Commands.CommandHandler
{
    class ValidationItemHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public ValidationItemHandler(RepoGenBaseFolder repo)
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
                string function = (string)arguments.GetValueOrDefault<string, object>("function");
                string name = (string)arguments.GetValueOrDefault<string, object>("name");
                string value = (string)arguments.GetValueOrDefault<string, object>("value");

                if (function == default(string))
                {
                    throw new Exception("No validate function defined");
                }

                if (target != default(string))
                {
                    repoItemInfo = new RepoItemInfo(repo, "CommandItem", target, 10000, null, "3529d992-a9f6-4df9-927a-3e7e1e08ca8b");
                    adapter = repoItemInfo.CreateAdapter<Ranorex.Unknown>(true);
                }

                if (adapter != null)
                {
                    switch (function)
                    {
                        case "AttributeEqual":
                            if (name == default(string))
                            {
                                throw new Exception("No name to validate defined");
                            }
                            if (value == default(string))
                            {
                                throw new Exception("No value to validate defined");
                            }
                            Validate.Attribute(adapter, name, value);
                            break;
                        case "Exists":
                            Validate.Exists(adapter);
                            break;
                        case "NotExists":
                            Validate.NotExists(adapter);
                            break;
                        case "AttributeRegEx":
                            if (name == default(string))
                            {
                                throw new Exception("No name to validate defined");
                            }
                            if (value == default(string))
                            {
                                throw new Exception("No value to validate defined");
                            }
                            Validate.Attribute(adapter, name, new Regex(value));
                            break;
                        case "AttributeContains":
                            if (name == default(string))
                            {
                                throw new Exception("No name to validate defined");
                            }
                            if (value == default(string))
                            {
                                throw new Exception("No value to validate defined");
                            }
                            Validate.Attribute(adapter, name, new Regex(Regex.Escape(value)));
                            break;
                        case "AttributeNotContains":
                            if (name == default(string))
                            {
                                throw new Exception("No name to validate defined");
                            }
                            if (value == default(string))
                            {
                                throw new Exception("No value to validate defined");
                            }
                            Validate.Attribute(adapter, name, new Regex("^((?!(" + Regex.Escape(value) + "))(.|\n))*$"));
                            
                            break;
                        case "ContainsImage":

                            break;
                        case "CompareImage":

                            break;

                    }
                }
                else
                {
                    
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
