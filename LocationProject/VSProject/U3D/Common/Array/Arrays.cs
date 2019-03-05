using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Common
{
    /// <summary>
    /// 数组帮助类
    /// </summary>
    public static class Arrays
    {
        /// <summary>
        /// 将数组转换为字符串，默认用" "隔开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">数组</param>
        /// <param name="interval">分隔符</param>
        /// <returns></returns>
        public static string ToString<T>(IEnumerable<T> list,string interval=" ")
        {
            string txt = "";
            foreach (T item in list)
            {
                txt += item + interval;
            }
            if(txt.Length> interval.Length)
                txt = txt.Substring(0, txt.Length - interval.Length);
            return txt;
        }
    }
}
