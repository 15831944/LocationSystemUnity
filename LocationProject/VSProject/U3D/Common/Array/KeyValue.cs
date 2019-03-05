using System;
using System.Xml.Serialization;

namespace Base.Common
{
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class KeyValue
    {
        [XmlAttribute]
        public string Key{ get; set; }
        [XmlAttribute]
        public string Value{ get; set; }

        public KeyValue()
        {
            
        }

        public KeyValue(string key, object value)
        {
            Key = key;
            //if (value is Dictionary<string, string>)
            //{
            //    SubValues = new List<KeyValue>();
            //    foreach (KeyValuePair<string, string> pair in value as Dictionary<string,string>)
            //    {
            //        SubValues.Add(new KeyValue(pair.Key, pair.Value));
            //    }
            //}
            //else
            {
                Value = value + "";
            }
            
        }

        //public List<KeyValue> SubValues { get; set; }

        public override string ToString()
        {
            return Key + " : " + Value;
        }
    }
}
