using System.Collections.Generic;

namespace Base.Common.Extensions
{
    public static class DictExtension
    {
        public static T2 GetValue<T1, T2>(this Dictionary<T1, T2> dic, T1 key)
        {
            if (key == null) return default(T2); ;
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                return default(T2);
            }
        }

        public static void AddValue<T1, T2>(this Dictionary<T1, T2> dic, T1 key, T2 value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }
    }
}
