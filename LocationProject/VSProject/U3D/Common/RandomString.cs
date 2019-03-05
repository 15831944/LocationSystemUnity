using System;

namespace Base.Common
{
    /// <summary>
    /// 生成随机字符串
    /// </summary>
    public static class RandomString
    {
        private static Random random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// 生成随机名称（只有字母），一定程度随机字符，可能为空的，也可能重名
        /// </summary>
        /// <param name="pre"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string GetName(string pre, int min, int max)
        {
            string name = pre + GetName(random.Next(min, max));
            name = name.Substring(0, random.Next(0, name.Length));//一定程度随机字符，可能为空的，也可能重名
            return name;
        }

        /// <summary>
        /// 生成随机名称（只有字母）
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetName(int length = 5)
        {
            int a = (int)'a';
            string txt = "";
            for (int i = 0; i < length; i++)
            {
                int n = random.Next(24);
                char c = (char)(a + n);
                txt += c;
            }
            return txt;
        }

        /// <summary>
        /// 获取一个名称和一个数字
        /// </summary>
        /// <param name="nameLength">名称的长度，默认1</param>
        /// <param name="numMaxRandom">数字的随机最大值，默认9</param>
        /// <returns></returns>
        public static string GetNameAndNumber(int nameLength=1,int numMaxRandom=9)
        {
            return GetName(nameLength) + random.Next(numMaxRandom);
        }

        /// <summary>
        /// 生成随机Ip
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            string txt = "";
            txt += random.Next(1, 255) + ".";
            txt += random.Next(1, 255) + ".";
            txt += random.Next(1, 255) + ".";
            txt += random.Next(1, 255);
            return txt;
        }
    }
}
