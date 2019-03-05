using System;
using System.Diagnostics;
using System.Reflection;
namespace Base.Common
{
    [Serializable]
    public class LogArg
    {
        public LogType Type { get; set; }
        public string Tag { get; set; }
        public object Msg { get; set; }

        public string Exception { get; set; }

        public DateTime Time { get; set; }

        public static string GetCurrentStackTrace()
        {
            StackTrace st = new StackTrace();
            return GetStackTraceModelName(st);
        }

        /// <summary>
        /// @Author:      HTL
        /// @Email:       Huangyuan413026@163.com
        /// @DateTime:    2015-06-03 19:54:49
        /// @Description: 获取当前堆栈的上级调用方法列表,直到最终调用者,只会返回调用的各方法,而不会返回具体的出错行数，可参考：微软真是个十足的混蛋啊！让我们跟踪Exception到行把！（不明真相群众请入） 
        /// </summary>
        /// <returns></returns>
        static string GetStackTraceModelName(System.Diagnostics.StackTrace st)
        {
            if (st == null) return "";
#if WINDOWS_UWP
                return null;
#else
            //当前堆栈信息
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
            string _filterdName = "ResponseWrite,ResponseWriteError,";
            string _fullName = string.Empty, _methodName = string.Empty;
            for (int i = 1; i < sfs.Length; ++i)
            {
                StackFrame frame = sfs[i];
                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == frame.GetILOffset()) break;
                MethodBase method = frame.GetMethod();
                if (method.ReflectedType == typeof(Log)) continue;
                if (method.ReflectedType == typeof(LogArg)) continue;
                _methodName = method.Name;//方法名称
                                          //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0
                                          //sfs[i].GetMethod().ta
                if (_filterdName.Contains(_methodName)) continue;
                //string txt = frame.GetFileLineNumber()+":"+method.ReflectedType.Name + "." + _methodName;
                string txt = method.ReflectedType.Name + "." + method.ToString();
                _fullName = txt + "()->\n" + _fullName;
            }
            st = null;
            sfs = null;
            _filterdName = _methodName = null;
            return _fullName.TrimEnd('-', '>');
#endif
        }

        //public string StackTrace { get; set; }

        System.Diagnostics.StackTrace st;

        public string Stack { get; set; }

        public string Stack0 { get; set; }

        public string GetStackTrace()
        {
            if (!string.IsNullOrEmpty(Stack)) return Stack;
                return GetStackTraceModelName(st);
        }

        public string GetStackTrace0()
        {
            if (!string.IsNullOrEmpty(Stack0)) return Stack0;
            return GetStackTraceModelName(st);
        }

        public void SetStackTrace()
        {
            Stack = GetStackTraceModelName(st);
            Stack0 = GetStackTraceModelName(st0);
        }

        public LogArg(LogType type, string tag, object msg, StackTrace st0 = null)
        {
            Time = DateTime.Now;
            Type = type;
            Tag = tag;
            if (string.IsNullOrEmpty(Tag))
            {
                Tag = "Tag";
            }
            Msg = msg;
            //StackTrace = GetStackTraceModelName();
            st = new StackTrace();
            this.st0 = st0;
        }

        StackTrace st0;

        public LogArg()
        {
            
        }

        public override string ToString()
        {
            if (Exception != null)
            {
                return string.Format("[{0}][{1}][{2}][{3}][{4}]", Time.ToString("HH:mm:ss.fff"), Type, Tag, Msg, Exception);
            }
            else
            {
                if (string.IsNullOrEmpty(Msg + ""))
                {
                    return string.Format("[{0}][{1}][{2}]", Time.ToString("HH:mm:ss.fff"), Type, Tag);
                }
                else
                {
                    return string.Format("[{0}][{1}][{2}][{3}]", Time.ToString("HH:mm:ss.fff"), Type, Tag, Msg);
                }

            }
        }
    }
}
