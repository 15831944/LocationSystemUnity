using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Base.Common.Extensions
{
    public static class XmlNodeExtension
    {
        public static string GetValue(this XmlNode node, string name)
        {
            if (node.Attributes != null && node.Attributes[name] != null)
            //if (node.Attributes[name] != null)
            {
                return node.Attributes[name].Value;
            }
            return "";
        }

        public static void SetValue(this XmlNode node, object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                XmlAttributeAttribute attribute = AttributeHelper.Get<XmlAttributeAttribute>(propertyInfo);
                string propertyName = propertyInfo.Name;
                if(attribute!=null&&!string.IsNullOrEmpty(attribute.AttributeName))
                {
                    propertyName = attribute.AttributeName;
                }
                string attributeValue = node.GetValue(propertyName);

                try
                {
                    if (attributeValue != "")
                    {
                        propertyInfo.SetValue(obj, attributeValue, null);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("XmlNodeExtension.SetValue", ex);
                }
            }
        }

        public static int GetIntValue(this XmlNode node, string name)
        {
            string value = node.GetValue(name);
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                try
                {
                    return Int32.Parse(value);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}
