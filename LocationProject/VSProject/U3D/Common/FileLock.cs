using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Base.Common
{
    /// <summary>
    /// 文件锁
    /// </summary>
    public class FileLock
    {
        private static ReaderWriterLockSlim slim;

        /// <summary>
        /// 获取文件锁
        /// </summary>
        /// <returns></returns>
       public static ReaderWriterLockSlim GetFileLock()
       {
           if (slim == null)
           {
               slim = new ReaderWriterLockSlim();
           }

           return slim;
       }
    }
}
