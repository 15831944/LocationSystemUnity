using System;
using System.Reflection;

namespace Base.Common
{
    public static class AttributeHelper
    {
        public static T Get<T>(MemberInfo methodInfo)
            where T:Attribute
        {
#if !WINDOWS_UWP
    return (T) Attribute.GetCustomAttribute(methodInfo, typeof (T));
#else
            return default(T);
#endif
        }

#if WINDOWS_UWP
        public static T Get<T>(Type type)
            where T : Attribute
        {
            return default(T);
        }
#endif

        public static T[] Gets<T>(MemberInfo methodInfo)
            where T : Attribute
        {
#if !WINDOWS_UWP
    Attribute[] attributes= Attribute.GetCustomAttributes(methodInfo, typeof(T));
            T[] results=new T[attributes.Length];
            for (int i = 0; i < attributes.Length; i++)
            {
                results[i] = attributes[i] as T;
            }
            return results;
#else
            return null;
#endif
        }
    }
}
