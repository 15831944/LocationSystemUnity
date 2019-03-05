using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Base.Common
{
    public static class Log
    {
        public static void ShowMessage()
        {

        }

        public static int Mode = 0;

        public static void Error(string tag,Exception ex, object msg, StackTrace st0=null)
        {
            if (Mode == 0)
            {
                WriteLine(string.Format("error:{0}|{1}|{2}", tag, msg, ex));
            }
            else
            {
                LogArg arg = new LogArg(LogType.Error, tag, msg, st0);
                arg.Exception = ex.ToString();
                WriteLine(arg);
            }
        }

        public static void Error(string tag, Exception ex, StackTrace st0 = null)
        {
            if (Mode == 0)
            {
                WriteLine(string.Format("error:{0}|{1}", tag, ex));
            }
            else
            {
                LogArg arg = new LogArg(LogType.Error, tag, "", st0);
                arg.Exception = ex.ToString();
                WriteLine(arg);
            } 
        }
        public static void Error(string tag, string msg = null, StackTrace st0 = null)
        {
            if (Mode == 0)
            {
                WriteLine(string.Format("error:{0}|{1}", tag, msg));
            }
            else
            {
                LogArg arg = new LogArg(LogType.Error, tag, msg, st0);
                WriteLine(arg);
            }  
        }

        public static int MaxLength = 200;

        public static void Info(string tag, object msg)
        {
            if (Mode == 0)
            {
                WriteLine(string.Format("info:{0}|{1}", tag, msg));
            }
            else
            {
                LogArg arg = new LogArg(LogType.Info, tag, msg);
                WriteLine(arg);
            }   
        }

        public static void Debug(string tag, object msg=null)
        {
            if (Mode == 0)
            {
                WriteLine(string.Format("debug:{0}|{1}", tag, msg));
            }
            else
            {
                LogArg arg = new LogArg(LogType.Debug, tag, msg);
                WriteLine(arg);
            }   
        }

        public static void Alarm(string tag, object msg = null)
        {
            if (Mode == 0)
            {
                WriteLine(string.Format("debug:{0}|{1}", tag, msg));
            }
            else
            {
                LogArg arg = new LogArg(LogType.Alarm, tag, msg);
                WriteLine(arg);
            }
        }

        public static void Info(string msg)
        {
            if (Mode == 0)
            {
                WriteLine(msg);
            }
            else
            {
                LogArg arg = new LogArg(LogType.None, "",msg);
                WriteLine(arg);
            }
        }

        private static void WriteLine(string line)
        {
            if (MaxLength > 0 && line.Length > MaxLength)
            {
                line = line.Substring(0, MaxLength)+"。。。。。";
            }
#if !WINDOWS_UWP
            Trace.WriteLine(line);
#endif
            if (LogChanged != null)
            {
                LogChanged(line);
            }
        }

        public static void WriteLine(LogArg arg)
        {
            string line = arg.ToString();
            if (MaxLength > 0 && line.Length > MaxLength)
            {
                line = line.Substring(0, MaxLength) + "。。。。。";
            }

#if !WINDOWS_UWP
            Trace.WriteLine(line);
#endif
            if (LogChanged != null)
            {
                LogChanged(line);
            }

            if (LogChangedEx != null)
            {
                LogChangedEx(arg);
            }
        }

        /// <summary>
        /// 写了日志的事件，这个很可能是在子线程中调用的，注意不能直接写在控件上。
        /// </summary>
        public static event Action<string> LogChanged;

        /// <summary>
        /// 写了日志的事件，这个很可能是在子线程中调用的，注意不能直接写在控件上。
        /// </summary>
        public static event Action<LogArg> LogChangedEx;
    }
}
