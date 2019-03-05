using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Common.UICommands
{
    public class CommandSystem
    {
        public static CommandSystem Instance = new CommandSystem();
        private CommandSystem()
        {

        }

        private UICommandList  _cmds = new UICommandList ();

        public void Add(object sender,string name,object arg)
        {
            UICommand cmd = new UICommand(sender, name, arg);
            _cmds.Add(cmd);
        }

        public void Add(object sender, Action action, string name="", string targetName="MainForm")
        {
            UICommand cmd = new UICommand(sender, action, targetName,name);
            _cmds.Add(cmd);
        }

        public UICommand Get(int i=0,bool isRemove=true)
        {
            if (_cmds.Count > i)
            {
                UICommand cmd = _cmds[i];
                if (isRemove)
                {
                    _cmds.RemoveAt(i);
                }
                return cmd;
            }
            return null;
        }
    }
}
