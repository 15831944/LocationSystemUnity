using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Base.PipeCommunicate
{
    /// <summary>
    /// 2d和3d通信用的数据结构
    /// </summary>
    [XmlType("Func")]
    public class FunctionInfo
    {
        private List<string> _args;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FunctionInfo()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="args"></param>
        public FunctionInfo(string fun, params object[] args):this()
        {
            Name = fun;
            foreach (object o in args)
            {
                Arg.Add(o + string.Empty);
            }
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string this[int id]
        {
            get { return Arg[id]; }
        }

        //public string Text { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<string> Arg
        {
            get
            {
                if (_args == null)
                {
                    _args=new List<string>();
                }
                return _args;
            }
            set { _args = value; }
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name)) return "";
            var argsTxt = GetArgsTxt();
            return string.Format("{0}({1})", Name, argsTxt);
        }

        private string GetArgsTxt()
        {
            string argsTxt = "";
            if (Arg != null)
                foreach (string arg in Arg)
                {
                    if (arg == null) continue;
                    argsTxt += arg + ",";
                }
            if (argsTxt.Length > 0)
                argsTxt = argsTxt.Substring(0, argsTxt.Length - 1);
            return argsTxt;
        }

        /// <summary>
        /// 解析Xml，解析结果为多个FunctionInfo的xml
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static List<string> Parse(string xml)
        {
            List<string> result = new List<string>();
            Parse(result, xml);
            return result;
        }

        const string startString = "<?xml";

        private static void Parse(List<string> list, string xml)
        {
            if (string.IsNullOrEmpty(xml)) return;
            xml = xml.Trim();
            int id = xml.LastIndexOf(startString);
            if (id == -1) return;
            if (id == 0)
            {
                list.Add(xml); //这里跨线程了，不能直接执行
            }
            else
            {
                string before = xml.Substring(0, id);
                Parse(list, before);
                string after = xml.Substring(id);
                list.Add(after);
            }
        }
    }
}
