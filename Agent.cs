using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Windows;
using System.Runtime;
using System.Runtime.CompilerServices;
using Ranorex;
using Ranorex.Controls;
using Ranorex.Core;
using Ranorex.Core.Recorder;
using Ranorex.Core.FastXml;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;
using Ranorex.Core.Reporting;
using Ranorex.Core.Remoting;
using Ranorex.Core.Remoting.RecordItems;
using Ranorex.Libs.Xml;
using Ranorex.Plugin.Mobile.Utils;
using Ranorex.Plugin.Mobile.Discovery;
using Ranorex.Plugin.Mobile.RxIntegration;
using Ranorex.Plugin.Mobile.Controls;
using RxAgent.Commands;
using RxAgent.Util;
using RxAgent.Commands.CommandHandler;
using JsonExSerializer;
using System.Diagnostics;
using Ranorex.Core.Data;

namespace RxAgent
{
    class Agent
    {
        public static int EXECUTION_INDEX = 0;

        private testRepository repo = testRepository.Instance;
        private Recorder recorder;
        private RecordTable recordTable;
        public TrackerCheckBox tracker = null;
        public ItemRecorderToolWindow toolWindow = null;
        private AttributeForm attributeForm;
        private ItemRecorder itemRecorder;
        private string sessionType;
        private Dictionary<string, object> startupArguments;
        private Ranorex.Core.Remoting.IRemoteEndpoint remoteEndpoint;
        private bool isTerminating = false;
        private bool isStandalone;

        private bool reportSetupDone = false;
        private bool isInteractive = false;
        private bool showExecutionUI = true;
        private string mobileDevice = null;
        private Int32 closeAp = 0;
        private Int32 procesID;
        private string command;
     
        private AgentServer agentServer;

        public Agent(int port, bool isStandalone)
        {
            this.isStandalone = isStandalone;

            agentServer = new AgentServer(isStandalone, port);
            agentServer.OnCommand += new AgentServer.NewCommand(agentServer_OnCommand);
            agentServer.Connected += new EventHandler(agentServer_Connected);
            agentServer.Disconnected += new EventHandler(agentServer_Disconnected);
        }

        void agentServer_Disconnected(object sender, EventArgs e)
        {
            if (isInteractive)
            {
                recorder.Stop();
                toolWindow.Invoke((MethodInvoker)delegate
                {
                    toolWindow.Hide();
                      
                });
                tracker.Stop();
            }
            else
            {
                ProgressForm.Hide();
            }
        }

        void agentServer_Connected(object sender, EventArgs e)
        {
            isTerminating = false;
            ActivityStack.Clear();
            ActivityStack.Update();
            EXECUTION_INDEX = 0;
            recorder.Stop();
            recorder.SetNormalMode();
            tracker.Stop();
        }

        void agentServer_OnCommand(Command command)
        {
            RanorexStepExecutionResponse stepResponse = null;
            if (command.command == "invoke")
            {
                if (!reportSetupDone)
                {
                    TestReport.Setup(Report.CurrentReportLevel, null, true, TestReport.EnableTracingScreenshots, 0, null);
                    reportSetupDone = true;
                }

                if (showExecutionUI)
                {
                    ProgressForm.Show();
                }
                else
                {
                    ProgressForm.Hide();
                }
                
                Dictionary<string, object> arguments = command.arguments;
                Hashtable props = (Hashtable)arguments["props"];

                if (sessionType == "Mobile")
                {
                    string target = (string)command.arguments["target"];
                    if (target.StartsWith("/mobileapp/")) {
                        target.Replace("/mobileapp", "/mobileapp[@devicename'" + mobileDevice + "']");
                    }
                }

                foreach (DictionaryEntry entry in props)
                {
                    arguments.Add((string)entry.Key, entry.Value);
                }
                stepResponse = new RanorexHandlerFactory().GetHandler((string)command.arguments["itest-action"], repo).Execute(arguments);
                EXECUTION_INDEX++;
            }
            else if (command.command == "ping")
            {
                agentServer.Ping();
            }
            else if (command.command == "open")
            {
                try
                {
                    if ((bool)command.arguments["capture"] == true)
                    {
                        isInteractive = true;
                    }

                    OpenSession(command);

                    stepResponse = new RanorexStepExecutionResponse();
                    stepResponse.success = true;
                }
                catch (Exception e)
                {
                    stepResponse = new RanorexStepExecutionResponse();
                    stepResponse.success = false;
                    stepResponse.message = "Failed to open Ranorex session: " + e.Message;

                    agentServer.DropClient();
                }

            }
            else  if (command.command == "close")
            {
            	closeSession();
                agentServer.DropClient();
                
            }
            else if (command.command == "startcapture")
            {

            }
            else if (command.command == "stopcapture")
            {
                Thread startThread = new Thread(StopRecording);
                startThread.SetApartmentState(ApartmentState.STA);
                startThread.Start();
            }

            if (stepResponse != null)
            {
                AgentResponse agentResponse = new AgentResponse();
                agentResponse.messageType = AgentResponse.MessageType.EXECUTION_STEP_RESPONSE;
                agentResponse.stepExecutionResponse = stepResponse;
                agentServer.SendMessage(agentResponse);
            }
        }

        private void InitRanorex()
        {
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.0;

            itemRecorder = new ItemRecorder();
            itemRecorder.CreateControl();

            recordTable = new RecordTable("recordTableName", new List<RecordItem>());
            recorder = new Recorder(itemRecorder, recordTable, ElementEngine.Instance.RootElement);
            recorder.ValidationHandler = new ValidationRequestHandler(this.ValidationRequestCallback);
            recorder.RecordingStopped += new EventHandler<RecordingStoppedEventArgs>(recorder_RecordingStopped);

            recorder.ItemRecorded += new EventHandler<ItemRecordedEventArgs>(this.Recorder_ItemRecorded);

            CreateControlPanel();

            tracker = new TrackerCheckBox();
            tracker.EnableMouseWheelParentCounterChange = false;
            tracker.EnableHighlighterAutoUpdate = false;
            tracker.CreateControl();
            tracker.EnableInstantTracking = false;
            tracker.EnableKeyboardShortcuts = false;
        }

        void recorder_RecordingStopped(object sender, RecordingStoppedEventArgs e)
        {

        }

        public IEnumerable<System.Windows.Forms.Control> GetAll(System.Windows.Forms.Control control, Type type)
        {
            var controls = control.Controls.Cast<System.Windows.Forms.Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        private void CreateControlPanel()
        {   
            this.toolWindow = new ItemRecorderToolWindow(recorder);
            this.toolWindow.CreateControl();

            IEnumerable<System.Windows.Forms.Control> controls = GetAll(this.toolWindow, typeof(System.Windows.Forms.CheckBox));
            foreach (System.Windows.Forms.Control c in controls)
            {
                if (c.Text == "Validate")
                {
                    c.Text = "Get Attribute";
                }
                else if (c.Text == "Remote only")
                {
                    c.Hide();
                }
                else if (c.Text == "Image based")
                {
                    c.Hide();
                }
                else if (c.Text == "Enable Hotkeys")
                {
                    c.Hide();
                }
            }
            Program.form.Invoke((MethodInvoker)delegate
            {
                Rectangle bounds = Screen.FromControl(Program.form).Bounds;
                this.toolWindow.Location = new Point(bounds.Right - this.toolWindow.Width - 5, bounds.Bottom - this.toolWindow.Height - 5);

                this.toolWindow.PauseClicked += new EventHandler(this.ToolPauseClicked);
                this.toolWindow.StopClicked += new EventHandler(this.ToolStopClicked);
                this.toolWindow.ValidateChecked += new EventHandler(this.ToolValidationChecked);
                this.toolWindow.ValidateUnchecked += new EventHandler(this.ToolValidationUnchecked);
                this.toolWindow.MouseEnter += new EventHandler(this.ToolMouseEnter);
                this.toolWindow.MouseLeave += new EventHandler(this.ToolMouseLeave);
                this.toolWindow.Show();
                this.toolWindow.Hide();
            });
        }

        private void StartTracker()
        {
            if (!this.recorder.DesktopRecordingDisabled)
            {
                this.tracker.CurrentRoot = ElementEngine.Instance.RootElement;
                this.tracker.IgnoredForms.Clear();
                this.tracker.IgnoredForms.Add(this.toolWindow);
                
                this.tracker.Start();
            }
        }

        private IList<ValidationRecordItem> ValidationRequestCallback(ElementInfo info)
        {
           
            this.tracker.Stop();

            if (info != null && info.Element != null)
            {
                RecorderValidationForm.TargetMode targetMode = RecorderValidationForm.TargetMode.Host;
                if (sessionType == "Mobile")
                {
                    targetMode = RecorderValidationForm.TargetMode.Remote;
                }
                ShowValidationDialog(info, targetMode, true);   
            }
            return new List<ValidationRecordItem>();

        }

        private void ShowValidationDialog(ElementInfo elementInfo, RecorderValidationForm.TargetMode mode, bool showImageTab)
        {
            Program.form.Invoke((MethodInvoker)delegate
            {
                RecorderValidationForm validationForm = new RecorderValidationForm(recordTable, elementInfo, this.recorder.CaptureImages, showImageTab, mode);
                validationForm.CreateControl();
                this.recorder.AddFilteredWindows(new IntPtr[]
				    {
					    validationForm.Handle
				    });

                if (validationForm.
                    ShowDialog() == DialogResult.OK)
                {
                    IList<ValidationRecordItem> result = validationForm.ResultRecordItems;
                    ProcessValidateItems(elementInfo, result);
                }

                this.recorder.RemoveFilteredWindows(new IntPtr[]
   			        {
					    validationForm.Handle
				    });
                validationForm.Dispose();

                ContinueRecording();
            });             

        }

        public void Start()
        {
            InitRanorex();

            attributeForm = new AttributeForm();
            attributeForm.CreateControl();
            attributeForm.VisibleChanged += new EventHandler(attributeForm_VisibleChanged);
            attributeForm.AttributeSelected += new AttributeForm.SelectAttribute(attributeForm_onAttributeSelected);
            attributeForm.Hide();

            agentServer.Start();
        }

        public void Open()
        {
            agentServer.Open();
        }

        private void SendTerminate(string reason)
        {
            AgentResponse agentResponse = new AgentResponse();
            agentResponse.stepExecutionResponse = new RanorexStepExecutionResponse();
            agentResponse.messageType = AgentResponse.MessageType.TERMINATION_RESPONSE;
            
            agentResponse.stepExecutionResponse.message = reason;
            agentServer.SendMessage(agentResponse);
        }
        private void closeSession(){		
     
     	if(sessionType == "Desktop"){
        		
        		 IList<Ranorex.WindowsApp>findElement = Host.Local.FindChildren<Ranorex.WindowsApp>();
		           string filename = Path.GetFileNameWithoutExtension(command);
		           string pathName = filename.Substring(0,1).ToUpper();
		           string fullName = pathName + filename.Substring(1);
		           try{
		           var getElement = Host.Local.FindSingle("/winapp[@title='" + fullName + "'or @title~'" + fullName + "']");
		           var processID = getElement.GetAttributeValue("ProcessID");
		           closeAp = Convert.ToInt32(processID);
		           } catch (ElementNotFoundException e ){
		           	
		           }
		           
		           try{
        		Host.Local.CloseApplication(closeAp);
		           }catch (Exception e){
		           	
		           }
		           try{
        		Host.Local.CloseApplication(procesID);
		           } catch(Exception e ){
		           	
		           }
     	  	}
        }
        private void OpenSession(Command openCommand)
        {
            this.sessionType = (string)openCommand.arguments["sessionType"];
            this.startupArguments = openCommand.arguments;
            this.showExecutionUI = (bool) openCommand.arguments["showExecutionUI"];
            command = (string) openCommand.arguments["command"];
            if (sessionType == "Web")
            {
                Host.Local.OpenBrowser(command, (string) startupArguments["browserType"]);
                recordTable.RecordingMode = RecordTable.RecorderMode.Web;
            }
            else if (sessionType == "Desktop")
            {	
          
		          procesID = Host.Local.RunApplication(command,(string) startupArguments["applicationArguments"]);
		           recordTable.RecordingMode = RecordTable.RecorderMode.Desktop;
		                	
            }
            else if (sessionType == "Mobile")
            {
                mobileDevice = command;
                string mobileApp = (string) startupArguments["mobileAppPlatform"];
                remoteEndpoint = GetEndpoint(GetRemoteEndpointInformation(command));
                try
                {
                    remoteEndpoint.StartApplication(new AppInformation(mobileApp, string.Empty, -1, string.Empty));
                    remoteEndpoint.EnableEventSynchronization(true);
                }
                catch (Exception e)
                {
                    if (e.HelpLink == "http://www.ranorex.com/support/user-guide-20/ios-testing.html#DebugImage")
                    {
                        remoteEndpoint.Reconnect();
                        remoteEndpoint.EnableEventSynchronization(true);
                    }
                    else
                    {
                        throw;
                    }
                }

                new EndpointRecorderConnector(recorder, remoteEndpoint, false);
                remoteEndpoint.StatusChangedEvent += new EventHandler<EventArgs>(remoteEndpoint_StatusChangedEvent);
                recordTable.RecordingMode = RecordTable.RecorderMode.Mobile;
            }
            else
            {
                throw new Exception("Unknown session type: " + sessionType);
            }

            Delay.Milliseconds(1000);

            if ((bool)openCommand.arguments["capture"])
            {

                StartRecording();
            }
            else
            {
                if ((bool)openCommand.arguments["showExecutionUI"])
                {
                    ShowProgressForm();
                }
                
            }
        }

        void remoteEndpoint_StatusChangedEvent(object sender, EventArgs e)
        {
            if (isTerminating)
            {
                return;
            }
            if (sender is Ranorex.Core.Remoting.IRemoteEndpoint) 
            {
                Ranorex.Core.Remoting.IRemoteEndpoint endpoint = (Ranorex.Core.Remoting.IRemoteEndpoint)sender;
                if (endpoint.Status == ChannelState.Error)
                {
                    isTerminating = true;
                    SendTerminate(endpoint.StatusMessage);
                    agentServer.DropClient();
                }
                else if (endpoint.Status == ChannelState.DeviceConnected)
                {
                    //isTerminating = true;
                    //SendTerminate(endpoint.StatusMessage);
                    //agentServer.DropClient();
                }
            }
        }

        void attributeForm_onAttributeSelected(string name, string value)
        {
            AgentResponse agentResponse = new AgentResponse();
            agentResponse.messageType = AgentResponse.MessageType.CAPTURE_STEP_REPONSE;

            RanorexStepCaptureReponse captureResponse = new RanorexStepCaptureReponse();

            agentResponse.stepCaptureReponse = captureResponse;
            captureResponse.action = "Get Attribute";

            ElementInfo elementInfo = attributeForm.currentElement;

            captureResponse.target = elementInfo.Path.ToResolvedString();

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("name", name);

            captureResponse.properties = properties;

            RanorexStepExecutionResponse executionResponse = new RanorexStepExecutionResponse();
            executionResponse.success = true;
            executionResponse.message = value;

            agentResponse.stepExecutionResponse = executionResponse;

            agentServer.SendMessage(agentResponse);
        }

        void attributeForm_VisibleChanged(object sender, EventArgs e)
        {
            recorder.SetNormalMode();
            recorder.Continue();
            toolWindow.SetMode(Recorder.RecordingMode.Normal);
        }

        private void ShowProgressForm()
        {
            ProgressForm.Show();
        }

        private void HideProgressForm()
        {
            ProgressForm.Hide();
        }

        public void StartRecording()
        {
            Program.form.Invoke((MethodInvoker)delegate
            {
                toolWindow.Show();
                recorder.Start();
            });
        }

        public void StopRecording()
        {
            recorder.Stop();
            this.toolWindow.Hide();
            FlushRecording();
        }

        private KeyboardSequenceRecordItem lastSequenceItem;
        private int prevRecordCount = 0;
        private bool recordingFlushed = false;

        private void Recorder_ItemRecorded(object sender, ItemRecordedEventArgs e)
        {
            int count = recordTable.RecordedItems.Count;

            if (count > 0)
            {
                recordingFlushed = false;
            }
            if (prevRecordCount != count && count > 1)
            {
            	recordingFlushed = true;
                ProcessRecordItem(recordTable.RecordedItems[count - 1]);
                prevRecordCount = count;
            }
        }

        private void FlushRecording()
        {
            if (recordingFlushed)
            {
                return;
            }
            int count = recordTable.RecordedItems.Count;
            if (count > 0)
            {
            	recordingFlushed = true;
                ProcessRecordItem(recordTable.RecordedItems[count - 1]);
            }
            prevRecordCount = count;
        }

        private void ProcessRecordItem(RecordItem recordItem)
        {
        	if(recordingFlushed){
            AgentResponse agentResponse = new AgentResponse();
            agentResponse.messageType = AgentResponse.MessageType.CAPTURE_STEP_REPONSE;

            RanorexStepCaptureReponse captureResponse = new RanorexStepCaptureReponse();
            agentResponse.stepCaptureReponse = captureResponse;

            Dictionary<string, object> properties = new Dictionary<string, object>();
            XmlNode parentNode = new XmlNode("recorditem");
            XmlNode recordNode = recordItem.CreateXmlNode(parentNode, false);

            properties.Add("attributes", recordNode.Attributes);

            captureResponse.action = recordItem.DisplayName;

            if (recordItem is ElementRecordItem)
            {
                RepositoryEntry repoEntry = ((ElementRecordItem)recordItem).GetBoundRepositoryEntry();

                if (repoEntry != null)
                {
                    string absolutePath = repoEntry.AbsolutePath.ToResolvedString();
                    if (absolutePath.StartsWith("/mobileapp"))
                    {
                        absolutePath = Regex.Replace(absolutePath, "/mobileapp\\[@devicename='[^\\/]*'\\]", "/mobileapp");
                    }
                    captureResponse.target = absolutePath;
                }
            }

            if (recordItem is MouseRecordItem)
            {
                MouseRecordItem mouseItem = (MouseRecordItem)recordItem;
                captureResponse.action = mouseItem.Action.ToString();
                properties.Add("button", mouseItem.Button.ToString());
                properties.Add("location", mouseItem.ElementLocation.FixedLocation.X + ";" + mouseItem.ElementLocation.FixedLocation.Y);
                properties.Add("duration", 200);
                captureResponse.properties = properties;
                Console.WriteLine(mouseItem.Action.ToString());
                Console.WriteLine((String) properties["location"]);
                Console.WriteLine((String) captureResponse.target);
                Debug.Print((String) mouseItem.Action.ToString());
                Console.Write((String) mouseItem.Action.ToString());
                agentServer.SendMessage(agentResponse);
            }
            else if (recordItem is KeyboardSequenceRecordItem)
            {
                KeyboardSequenceRecordItem sequenceItem = (KeyboardSequenceRecordItem)recordItem;
                properties.Add("sequence", sequenceItem.KeySequence);
                properties.Add("pressTime", 0);
                properties.Add("duration", 200);
                captureResponse.properties = properties;
                agentServer.SendMessage(agentResponse);
            }
            else if (recordItem is KeyboardRecordItem)
            {
                KeyboardRecordItem keyboardItem = (KeyboardRecordItem)recordItem;
                properties.Add("data", keyboardItem.KeyData.ToString());
                properties.Add("scanCode", keyboardItem.ScanCode);
                properties.Add("setModifiers", keyboardItem.SetModifiers);
                properties.Add("pressTime", 0);
                properties.Add("duration", 200);
                captureResponse.properties = properties;
                agentServer.SendMessage(agentResponse);
            }
            else if (recordItem is MouseWheelRecordItem)
            {
                MouseWheelRecordItem wheelItem = (MouseWheelRecordItem)recordItem;
                string orientation = wheelItem.Orientation.ToString();
                properties.Add("orientation", orientation);
                properties.Add("delta", wheelItem.Delta);
                properties.Add("duration", 200);
                captureResponse.properties = properties;
                agentServer.SendMessage(agentResponse);
            }
            else if (recordItem is SetValueRecordItem)
            {
                SetValueRecordItem setValueItem = (SetValueRecordItem)recordItem;
                properties.Add("name", setValueItem.AttributeName);
                properties.Add("value", setValueItem.AttributeValue);
                captureResponse.properties = properties;
                agentServer.SendMessage(agentResponse);
            }
            else if (recordItem is ElementRecordItem)
            {
                ElementRecordItem elementItem = (ElementRecordItem)recordItem;
                captureResponse.properties = properties;
                agentServer.SendMessage(agentResponse);
            }   
        	}
        }

        private void ProcessValidateItems(ElementInfo elementInfo, IList<ValidationRecordItem> recordedItems)
        {

            FlushRecording();

            AgentResponse agentResponse = new AgentResponse();
            agentResponse.messageType = AgentResponse.MessageType.CAPTURE_STEP_REPONSE;

            RanorexStepCaptureReponse captureResponse = new RanorexStepCaptureReponse();
            RanorexStepExecutionResponse executionResponse = new RanorexStepExecutionResponse();

            agentResponse.stepCaptureReponse = captureResponse;

            Dictionary<string, object> properties = new Dictionary<string, object>();

            Dictionary<string, string> attributes = new Dictionary<string, string>();
            Dictionary<string, object> structuredData = new Dictionary<string,object>();
            foreach (ValidationRecordItem item in recordedItems)
            {
                if (item.Action == ValidationAction.NotExists || item.Action == ValidationAction.Exists)
                {
                    continue;
                }

                if (elementInfo == null)
                {
                    elementInfo = item.Info;
                }
                if (item.Action == ValidationAction.CompareImage || item.Action == ValidationAction.ContainsImage)
                {
                    ImageSearchProperties searchProps = item.ImageBasedLocation;
                    Rectangle imgRect = searchProps.ImageSelectionRectangle;
                    CompressedImage image = elementInfo.LiveElementOrSnapshot.CaptureCompressedImage();
                    properties.Add("image", image.ToBase64String());
                    properties.Add("imageAction", item.Action.ToString());
                    properties.Add("clipX", imgRect.X);
                    properties.Add("clipY", imgRect.Y);
                    properties.Add("clipWidth", imgRect.Width);
                    properties.Add("clipHeight", imgRect.Height);

                    structuredData.Add("imageValidated", true);
                }
                else
                {
                    executionResponse.message += item.MatchName + "=" + item.MatchValue + "\n";
                    attributes.Add(item.MatchName, item.MatchValue);
                }
            }

            properties.Add("attributes", attributes);
            structuredData.Add("attributes", attributes);
            captureResponse.action = "Get Attribute";

            string absolutePath = elementInfo.Path.ToResolvedString();
            if (absolutePath.StartsWith("/mobileapp"))
            {
                absolutePath = Regex.Replace(absolutePath, "/mobileapp\\[@devicename='[^\\/]*'\\]", "/mobileapp");
            }
            captureResponse.target = absolutePath;
    
            captureResponse.properties = properties;

            executionResponse.structuredData = structuredData;
            executionResponse.success = true;

            agentResponse.stepExecutionResponse = executionResponse;
            

            agentServer.SendMessage(agentResponse);
        }

        private void ContinueRecording()
        {
            recorder.SetNormalMode();
            recorder.Continue();
            toolWindow.SetMode(Recorder.RecordingMode.Normal);
        }
  
        private void ToolPauseClicked(object sender, EventArgs e)
        {
            if (recorder.RecMode != Recorder.RecordingMode.Pause)
            {
                recorder.Pause();
                toolWindow.SetMode(Recorder.RecordingMode.Pause);
                return;
            }
            else
            {
                recorder.Continue();
                toolWindow.SetMode(Recorder.RecordingMode.Normal);
            }
           
        }

        private void ToolStopClicked(object sender, EventArgs e)
        {
            StopRecording();
        }

        private void ToolValidationChecked(object sender, EventArgs e)
        {
            recorder.SetValidateMode();
            if (sessionType == "Mobile")
            {
                ShowValidationDialog(null, RecorderValidationForm.TargetMode.Remote, false);  
            }
            else
            {
                tracker.HighlighterColor = Color.Violet;
                tracker.HighlighterThinBorder = false;
                tracker.StopAfterClick = true;
                StartTracker();
            }
            
        }

        private void ToolValidationUnchecked(object sender, EventArgs e)
        {
            ContinueRecording();
        }

        private void ToolMouseEnter(object sender, EventArgs e)
        {

        }

        private void ToolMouseLeave(object sender, EventArgs e)
        {

        }

        private Ranorex.Core.Remoting.IRemoteEndpoint GetEndpoint(IRemoteEndpointInformation information)
        {
            Ranorex.Core.Remoting.IRemoteEndpoint result;
            try
            {
                if (information != null)
                {
                    Ranorex.Core.Remoting.IRemoteEndpoint remoteEndpointFromInfo = RemoteServiceLocator.Service.GetRemoteEndpointFromInfo(information);
                    result = remoteEndpointFromInfo;
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private IRemoteEndpointInformation GetRemoteEndpointInformation(String displayName)
        {
            try
            {
            	Ranorex.Core.Remoting.IRemoteEndpoint remoteEndpoint = RemoteServiceLocator.Service.GetByDisplayName(displayName);
				if(remoteEndpoint != null){
					return remoteEndpoint.RemoteInfo;
				}
                /*XmlNode storedDevices = MobilePluginConfig.StoredDevices;
                string innerText = storedDevices.InnerText;
                List<EndpointInformation> list = StringXmlSerializer.Deserialize<List<EndpointInformation>>(innerText);
                foreach (EndpointInformation endpointInfo in list)
                {
                    if (endpointInfo.DisplayName == displayName)
                    {
                        return endpointInfo;
                    }
                }*/

            }
            catch (Exception exc)
            {

            }

            return null;
        }
    }
}
