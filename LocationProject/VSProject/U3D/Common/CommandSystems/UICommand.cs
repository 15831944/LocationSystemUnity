using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Common
{
    public class UICommand
    {
        private Action action;
        private object arg;
        private string name;
        private object sender;
        private string targetName;
        public UICommand()
        {

        }
        public UICommand(object sender, string name, object arg)
        {
            this.Sender = sender;
            this.Name = name;
            this.Arg = arg;
        }

        public UICommand(object sender, Action action, string targetName, string name)
        {
            this.sender = sender;
            this.action = action;
            this.TargetName = targetName;
            this.name = name;
        }

        public bool DoAction()
        {
            if (action != null)
            {
                action();
                return true;
            }
            return false;
        }

        public object Arg
        {
            get
            {
                return arg;
            }

            set
            {
                arg = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public object Sender
        {
            get
            {
                return sender;
            }

            set
            {
                sender = value;
            }
        }

        public string TargetName
        {
            get
            {
                return targetName;
            }

            set
            {
                targetName = value;
            }
        }
    }
}
