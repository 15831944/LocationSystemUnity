using System;
using System.Reflection;

namespace Base.Common
{
    public static class CloneHelper
    {
        public static void Clone(object obj, object objNew)
        {
            Type type = obj.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo property in propertyInfos)
            {
                if (property.CanRead && property.CanWrite)
                {
                    object value = property.GetValue(obj, null);
                    property.SetValue(objNew, value, null);
                }
            }
        }
    }
}
