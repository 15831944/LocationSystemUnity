using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Base.Common
{
    public static class EventHelper
    {
        /// <summary>
        /// 清除事件上的委托函数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        public static bool RemoveEvent(this object obj, string name)
        {
            if (obj == null) return false;
            EventInfo info = obj.GetType().GetEvent(name);
            if (info == null) return false;
            Delegate[] invokeList = GetObjectEventList(obj, name);
            foreach (Delegate del in invokeList)
            {
                info.RemoveEventHandler(obj, del);
            }
            return true;
        }

        /// <summary>
        /// 清除静态事件上的委托函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public static bool RemoveEvent(this Type type, string name)
        {
            EventInfo info = type.GetEvent(name);
            if (info == null) return false;
            Delegate[] invokeList = GetObjectEventList(type, name);
            foreach (Delegate del in invokeList)
            {
                info.RemoveEventHandler(null, del);
            }
            return true;
        }


        ///  <summary>     
        /// 获取对象事件
        ///  </summary>     
        ///  <param name="obj">对象 </param>     
        ///  <param name="eventName">事件名 </param>     
        ///  <returns>委托列 </returns>     
        public static Delegate[] GetObjectEventList(this object obj, string eventName)
        {
            FieldInfo field = obj.GetType().GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                return null;
            }
            object fieldValue = field.GetValue(obj);
            if (fieldValue is Delegate)
            {
                Delegate objectDelegate = (Delegate)fieldValue;
                return objectDelegate.GetInvocationList();
            }
            return null;
        }

        /// <summary>
        /// 获取静态事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static Delegate[] GetObjectEventList(this Type type, string eventName)
        {
            FieldInfo field = type.GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                return null;
            }

            object fieldValue = field.GetValue(null);
            if (fieldValue is Delegate)
            {
                Delegate objectDelegate = (Delegate)fieldValue;
                return objectDelegate.GetInvocationList();
            }
            return null;
        }
    }
}
