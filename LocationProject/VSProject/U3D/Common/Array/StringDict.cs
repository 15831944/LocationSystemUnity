using System;
using System.Collections.Generic;

namespace Base.Common
{
    public class StringDict<T> : Dictionary<string, T> where T : class
    {
        public new T this[string key]
        {
            get
            {
                try
                {
                    return base[key];
                }
                catch (Exception ex)
                {
                    Log.Error("StringDict", ex);
                    return null;
                }
            }
            set { base[key] = value; }
        }
    }
}
