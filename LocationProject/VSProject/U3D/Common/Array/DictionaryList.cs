using System;
using System.Collections.Generic;
using System.Text;

namespace DataObjects
{
    /// <summary>
    /// 带Dictionary的List
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class DictionaryList<T1,T2>:List<T2>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DictionaryList():base()
        {
            
        }

        /// <summary>
        /// 索引
        /// </summary>
        protected Dictionary<T1, T2> Index { get; set; }

        private void InitIndex()
        {
            if (Index == null)
            {
                Index = new Dictionary<T1, T2>();
                foreach (T2 item in this)
                {
                    AddIndex(item);
                }
            }
        }

        /// <summary>
        /// 添加索引
        /// </summary>
        /// <param name="item"></param>
        protected abstract void AddIndex(T2 item);

        public virtual T2 Get(T1 key)
        {
            if (key == null) return default(T2);
            InitIndex();
            if (Index.ContainsKey(key))
                return Index[key];
            return default(T2);
        }

        public new void Clear()
        {
            base.Clear();
            if (Index!=null)
            {
                Index.Clear();
            }
        }

        public List<TOut> ConvertTo<TOut>() where TOut:class 
        {
            List<TOut> result=new List<TOut>();
            foreach (T2 item in this)
            {
                result.Add(item as TOut);
            }
            return result;
        } 
    }

    public static class ListHelper
    {
        public static List<TOut> ConvertTo<TIn,TOut>(List<TIn> list) where TOut : class
        {
            List<TOut> result = new List<TOut>();
            foreach (TIn item in list)
            {
                result.Add(item as TOut);
            }
            return result;
        }
    }
}
