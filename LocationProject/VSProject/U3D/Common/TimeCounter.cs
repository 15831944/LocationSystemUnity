using System;

namespace Base.Common
{
    /// <summary>
    /// 计时器，计算函数运行时间
    /// </summary>
    public class TimeCounter
    {
        /// <summary>
        /// 开始运行
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimeSpan Run(Action action)
        {
            DateTime time = DateTime.Now;
            if (action != null)
            {
                action();
            }
            TimeSpan span = DateTime.Now - time;
            return span;
        }
    }
}
