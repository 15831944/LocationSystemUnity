using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Common
{
    /// <summary>
    /// 3d函数调用信息
    /// </summary>
    public class InvokeMethodInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object[] Args { get; set; }
        /// <summary>
        /// 函数运行前的Sleep时间
        /// </summary>
        public int SleepTimeBefore { get; set; }
        /// <summary>
        /// 函数运行后的Sleep时间
        /// </summary>
        public int SleepTimeAfter { get; set; }
        /// <summary>
        /// 必须等待机房创建好后才调用的函数
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="roomId"></param>
        /// <param name="time"></param>
        /// <param name="time2"></param>
        public InvokeMethodInfo(string name, object[] args,string roomId,int time=5,int time2=0)
        {
            Name = name;
            Args = args;
            RoomId = roomId;
            SleepTimeAfter = time;
            SleepTimeBefore = time2;
        }
        /// <summary>
        /// 获取参数字符串
        /// </summary>
        /// <returns></returns>
        public string GetArgsText()
        {
            if (Args == null) return "";
            string txt = "";
            foreach (object arg in Args)
            {
                txt += arg + " ";
            }
            return txt.Trim();
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + ":" + GetArgsText();
        }

        /// <summary>
        /// 是否是同一指令内容
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSameMethodContent(InvokeMethodInfo other)
        {
            if (this == other) return false;//相同指令不算，这里计算的是相同内容的不同指令
            if (Name != other.Name) return false;
            return this.ToString() == other.ToString();
        }
    }
}
