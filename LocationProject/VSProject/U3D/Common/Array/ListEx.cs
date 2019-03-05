using System.Collections.Generic;

namespace Base.Common
{
    /// <summary>
    /// List扩展
    /// 1.不会重复添加相同元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListEx<T>:List<T>
    {
        public new void Add(T item)
        {
            if (!Contains(item))
            {
                base.Add(item);
            }
        }
    }
}
