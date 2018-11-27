using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
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
    class GetAttributeItemHandler : RanorexItemHandler
    {
        private RepoGenBaseFolder repo;

        public GetAttributeItemHandler(RepoGenBaseFolder repo)
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
                int timeout = Convert.ToInt32(arguments.GetValueOrDefault<string, object>("timeout"));
                bool retrieveImage = (bool)arguments.GetValueOrDefault<string, object>("retrieveImage");
                System.Collections.IDictionary imageProps = (System.Collections.IDictionary)arguments["imageValidationProperties"];
                System.Collections.IList attributes = (System.Collections.IList)arguments["attributes"];


                if (timeout == default(int))
                {
                    timeout = 10000;
                }

                if (target != default(string))
                {

                    repoItemInfo = CreateItemInfo(repo, target, timeout);
                    adapter = CreateAdapter(repoItemInfo);

                    Dictionary<string, object> structuredData = new Dictionary<string, object>();
                    Dictionary<string, object> receivedAttributes = new Dictionary<string, object>();
                    foreach (System.Collections.IDictionary attribute in attributes)
                    {
                        string name = (string) attribute["header"];
                        string value = adapter.Element.GetAttributeValueText(name);
                        stepResponse.message += name + "=" + value + "\n";
                        receivedAttributes.Add(name, value);
                    }
                    structuredData.Add("attributes", receivedAttributes);
                    if (imageProps != null)
                    {
                        string validationMode = (string) imageProps["validationMode"];
                        if (validationMode != "NONE")
                        {
                            string image = (string)imageProps["imageData"];
                            int rectX = Convert.ToInt32(imageProps["clipX"]);
                            int rectY = Convert.ToInt32(imageProps["clipY"]);
                            int rectWidth = Convert.ToInt32(imageProps["clipWidth"]);
                            int rectHeight = Convert.ToInt32(imageProps["clipHeight"]);
                            try
                            {

                                CompressedImage compressedImage = new CompressedImage(image);
                                using (Bitmap bitmap = Imaging.Crop(compressedImage.Image, new Rectangle(rectX, rectY, rectWidth, rectHeight)))
                                {
                                    compressedImage = new CompressedImage(bitmap);
                                }
                                if (validationMode == "CONTAINS_IMAGE")
                                {
                                    Validate.ContainsImage(repoItemInfo, compressedImage, Imaging.FindOptions.Default);
                                }
                                else if (validationMode == "COMPARE_IMAGE")
                                {
                                    Validate.ContainsImage(repoItemInfo, compressedImage, Imaging.FindOptions.Default);
                                }
                                structuredData.Add("imageValidated", "true");
                                stepResponse.message += "#imageValidated=true\n";
                            }
                            catch (ValidationException e)
                            {
                                structuredData.Add("imageValidated", "false");
                                stepResponse.message += "#imageValidated=false\n";
                            }
                        }
                    }

                    stepResponse.structuredData = structuredData;
                    if (retrieveImage)
                    {
                        stepResponse.image64 = GetImage64(adapter);
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
       
    }


}
