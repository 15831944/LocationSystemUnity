using System;
using System.Collections.Generic;

namespace Base.Common
{
#if !WINDOWS_UWP
    [Serializable]
#endif
    public class KeyValueList:List<KeyValue>
    {
        public string GetValue(string key)
        {
            foreach (KeyValue item in this)
            {
                if (item.Key == key)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public KeyValueList()
        {
            
        }
    }
}
