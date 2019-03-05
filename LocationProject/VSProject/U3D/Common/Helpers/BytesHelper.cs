using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Common
{
    public static class BytesHelper
    {
        /// <summary>
        /// 查找byte b所在byte[]的位置
        /// </summary>
        /// <param name="bytMsg">主byte[]</param>
        /// <param name="b">查找目标byte</param>
        /// <returns>b所在的POS</returns>
        public static int Indexofbyte(byte[] bytMsg, byte b,int start=0)
        {
            int length = bytMsg.Length;
            for (int i = start; i < length; i++)
            {
                if (bytMsg[i] == b)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 尝试多线程查找，在多线程初始化的情况下会有问题
        /// </summary>
        /// <param name="bytMsg"></param>
        /// <param name="b"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int IndexofbyteEx(byte[] bytMsg, byte b, int start = 0)
        {
            int length = bytMsg.Length;
            if (length < 10000)
            {
                return Indexofbyte(bytMsg, b, start);
            }

            List<int> indexs=new List<int>();
            
            int index = -1;
            int threadCount = 5;
            int interval = length/ threadCount;
            List<Action> actions = new List<Action>();
            for (int k = 0; k < threadCount; k++)
            {
                if (k == 0)
                {
                    Action action1 = () =>
                    {
                        for (int i = start; i <= interval; i += 1) //从前往后
                        {
                            if (bytMsg[i] == b)
                            {
                                index = i;
                                break;
                            }
                        }
                    };
                    actions.Add(action1);
                }
                else
                {
                    var k1 = k;
                    int max = interval * (k1 + 1);
                    if (max > length - 1 || k== threadCount-1)
                    {
                        max = length - 1;
                    }
                    Action action2 = () =>
                    {
                        if (length < 2) return;
                        
                        for (int j = interval + 1; j<= max; j++) //从后往前
                        {
                            if (index != -1)
                            {
                                break;
                            }
                            if (bytMsg[j] == b)
                            {
                                indexs.Add(j);
                            }
                        }
                    };
                    actions.Add(action2);
                }
            }
            ThreadHelper.RunMulti(()=>{}, actions.ToArray());

            if (index == -1 && indexs.Count>0)
            {
                indexs.Sort();
                index = indexs[0];
            }

            //int index3 = -1;
            //for (int k = start; k < length; k++)
            //{
            //    if (bytMsg[k] == b)
            //    {
            //        index3 = k;
            //        break;
            //    }
            //}
            //if (index != index3)
            //{

            //}

            return index;
        }

        public const string OutputNullString = "{ null }";
        static char[] HexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        public const string CRLF = "\r\n";
        public static string byte2hex(byte[] bs, int pos, int length)
        {
            if (bs == null || length < 0 || pos < 0 || pos >= bs.Length)
            {
                return OutputNullString;
            }

            int endPos = (pos + length) > bs.Length ? bs.Length : (pos + length);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < (endPos - pos); i++)
            {
                uint n = (uint)bs[i + pos];
                if (i != 0)
                {
                    if ((i % 40) == 0)
                        sb.Append(CRLF);
                    else if ((i % 8) == 0)
                        sb.Append(".");
                    else if ((i % 4) == 0)
                        sb.Append(" ");
                }
                sb.Append(HexChars[(n % 256) / 16]);
                sb.Append(HexChars[n % 16]);
            }

            return sb.ToString();
        }
    }
}
