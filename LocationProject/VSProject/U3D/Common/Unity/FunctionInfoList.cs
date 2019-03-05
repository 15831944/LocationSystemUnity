using System.Collections.Generic;
using System.Xml.Serialization;

namespace Base.PipeCommunicate
{
    /// <summary>
    /// 函数列表
    /// </summary>
    [XmlType("FuncList")]
    public class FunctionInfoList:List<FunctionInfo>
    {
        /// <summary>
        ///  构造函数
        /// </summary>
        public FunctionInfoList()
        {
            
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="text"></param>
        public FunctionInfoList(string text)
        {
            Text = text;
            string[] parts = text.Split(';');
            foreach (string part in parts)
            {
                if(part.Trim()=="")continue;
                FunctionInfo e = new FunctionInfo(part);
                this.Add(e);
            }
        }
    }
}
