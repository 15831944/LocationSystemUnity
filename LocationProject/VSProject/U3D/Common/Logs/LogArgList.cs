using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Common
{
    public class LogArgList : List<LogArg>
    {
        public LogArgList Filter(bool error, bool alarm, bool debug, bool info, bool none, string keyword)
        {
            LogArgList list = new LogArgList();
            foreach (var item in this)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (!item.ToString().ToLower().Contains(keyword.ToLower()))
                    {
                        continue;
                    }
                }
                if (item.Type == LogType.Error && error)
                {
                    list.Add(item);
                }
                else if (item.Type == LogType.Alarm && alarm)
                {
                    list.Add(item);
                }
                else if (item.Type == LogType.Debug && debug)
                {
                    list.Add(item);
                }
                else if (item.Type == LogType.Info && info)
                {
                    list.Add(item);
                }
                else if (item.Type == LogType.None && none)
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }
}
