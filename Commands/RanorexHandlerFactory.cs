using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RxAgent.Commands.CommandHandler;

using Ranorex.Core.Repository;

namespace RxAgent.Commands
{
    class RanorexHandlerFactory
    {
        public RanorexHandlerFactory()
        {

        }

        public RanorexItemHandler GetHandler(string action, RepoGenBaseFolder repo)
        {
            switch (action.ToLower())
            {
                case "click":
                case "doubleclick":
                case "up":
                case "down":
                case "move":
                    return new MouseItemHandler(repo);
                case "touch":
                case "longtouch":
                case "doubletap":
                case "touchstart":
                case "touchmove":
                case "touchend":
                    return new TouchItemHandler(repo);
                case "mobile key press":
                    return new MobileKeyPressItemHandler(repo);
                case "deploy ios app":
                    return new DeployIosAppItemHandler();
                case "deploy android app":
                    return new DeployAndroidAppItemHandler();
                case "swipe":
                    return new SwipeItemHandler(repo);
                case "key sequence":
                    return new KeySequenceHandler(repo);
                case "key shortcut":
                    return new KeyShortcutHandler(repo);
                case "open web browser":
                    return new OpenBrowserItemHandler();
                case "mouse wheel":
                    return new MouseWheelItemHandler();
                case "wait for":
                    return new WaitForItemHandler(repo);
                case "close application":
                    return new CloseApplicationItemHandler(repo);
                case "run application":
                    return new RunApplicationItemHandler();
                case "run mobile app":
                    return new RunMobileAppItemHandler();
                case "set value":
                    return new SetValueItemHandler(repo);
                case "get attribute":
                    return new GetAttributeItemHandler(repo);
                   case "close":
                    return new CloseApplicationItemHandler(repo);
                   
            }
            return new UnknownItemHandler();
        }

    }
}
