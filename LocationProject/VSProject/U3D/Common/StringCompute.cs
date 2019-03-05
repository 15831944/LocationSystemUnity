using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Common
{
    /// <summary>
    /// 本类适用于比较2个字符的相似度
    ///         // 方式一
    /// StringCompute stringcompute1 = new StringCompute();
    /// stringcompute1.SpeedyCompute("对比字符一", "对比字符二");  // 计算相似度， 不记录比较时间
    /// decimal rate = stringcompute1.ComputeResult.Rate;     // 相似度百分之几，完全匹配相似度为1
    /// // 方式二
    /// StringCompute stringcompute2 = new StringCompute();
    /// stringcompute2.Compute();                 // 计算相似度， 记录比较时间
    /// string usetime = stringcompute2.ComputeResult.UseTime;   // 对比使用时间
    /// </summary>
    public class StringCompute
    {
        /// <summary>
        /// 从数组中找出和关键字相似的字符串
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="list">数组</param>
        /// <returns></returns>
        public static string GetSimilarString(string key, IEnumerable<string> list)
        {
            DateTime start = DateTime.Now;
            if (list == null) return key;
            List<CompareResult > result = new List<CompareResult >();
            foreach (string item in list)
            {
                StringCompute compute = new StringCompute();
                compute.Compute(item, key);
                result.Add(compute.ComputeResult);
            }
            result.Sort();
            if (result.Count == 0) return key;
            TimeSpan time = DateTime.Now - start;
            return result[0].Text1;
        }

        #region 私有变量
        /// <summary>
        /// 字符串1
        /// </summary>
        private char[] _ArrChar1;
        /// <summary>
        /// 字符串2
        /// </summary>
        private char[] _ArrChar2;
        /// <summary>
        /// 统计结果
        /// </summary>
        private CompareResult  _result;
        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime _BeginTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime _EndTime;
        /// <summary>
        /// 计算次数
        /// </summary>
        private int _ComputeTimes;
        /// <summary>
        /// 算法矩阵
        /// </summary>
        private int[,] _Matrix;
        /// <summary>
        /// 矩阵列数
        /// </summary>
        private int _Column;
        /// <summary>
        /// 矩阵行数
        /// </summary>
        private int _Row;
        #endregion
        #region 属性

        /// <summary>
        /// 相似度计算结果
        /// </summary>
        public CompareResult  ComputeResult
        {
            get { return _result; }
        }
        #endregion
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        public StringCompute(string str1, string str2)
        {
            this.StringComputeInit(str1, str2);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StringCompute()
        {
        }


        #endregion
        #region 算法实现
        /// <summary>
        /// 初始化算法基本信息
        /// </summary>
        /// <param name="str1">字符串1</param>
        /// <param name="str2">字符串2</param>
        private void StringComputeInit(string str1, string str2)
        {
            _ArrChar1 = str1.ToCharArray();
            _ArrChar2 = str2.ToCharArray();
            _result = new CompareResult ();
            _result.Text1 = str1;
            _result.Text2 = str1;
            _ComputeTimes = 0;
            _Row = _ArrChar1.Length + 1;
            _Column = _ArrChar2.Length + 1;
            _Matrix = new int[_Row, _Column];
        }
        /// <summary>
        /// 计算相似度
        /// </summary>
        public void Compute()
        {
            //开始时间
            _BeginTime = DateTime.Now;
            //初始化矩阵的第一行和第一列
            this.InitMatrix();
            int intCost = 0;
            for (int i = 1; i < _Row; i++)
            {
                for (int j = 1; j < _Column; j++)
                {
                    if (_ArrChar1[i - 1] == _ArrChar2[j - 1])
                    {
                        intCost = 0;
                    }
                    else
                    {
                        intCost = 1;
                    }
                    //关键步骤，计算当前位置值为左边+1、上面+1、左上角+intCost中的最小值 
                    //循环遍历到最后_Matrix[_Row - 1, _Column - 1]即为两个字符串的距离
                    _Matrix[i, j] = this.Minimum(_Matrix[i - 1, j] + 1, _Matrix[i, j - 1] + 1, _Matrix[i - 1, j - 1] + intCost);
                    _ComputeTimes++;
                }
            }
            //结束时间
            _EndTime = DateTime.Now;
            //相似率 移动次数小于最长的字符串长度的20%算同一题
            int intLength = _Row > _Column ? _Row : _Column;

            _result.Rate = (1 - (decimal)_Matrix[_Row - 1, _Column - 1] / intLength);
            _result.UseTime = (_EndTime - _BeginTime).ToString();
            _result.ComputeTimes = _ComputeTimes.ToString();
            _result.Difference = _Matrix[_Row - 1, _Column - 1];
        }


        /// <summary>
        /// 计算相似度（不记录比较时间）
        /// </summary>
        public void SpeedyCompute()
        {
            //开始时间
            //_BeginTime = DateTime.Now;
            //初始化矩阵的第一行和第一列
            this.InitMatrix();
            int intCost = 0;
            for (int i = 1; i < _Row; i++)
            {
                for (int j = 1; j < _Column; j++)
                {
                    if (_ArrChar1[i - 1] == _ArrChar2[j - 1])
                    {
                        intCost = 0;
                    }
                    else
                    {
                        intCost = 1;
                    }
                    //关键步骤，计算当前位置值为左边+1、上面+1、左上角+intCost中的最小值 
                    //循环遍历到最后_Matrix[_Row - 1, _Column - 1]即为两个字符串的距离
                    _Matrix[i, j] = this.Minimum(_Matrix[i - 1, j] + 1, _Matrix[i, j - 1] + 1, _Matrix[i - 1, j - 1] + intCost);
                    _ComputeTimes++;
                }
            }
            //结束时间
            //_EndTime = DateTime.Now;
            //相似率 移动次数小于最长的字符串长度的20%算同一题
            int intLength = _Row > _Column ? _Row : _Column;

            _result.Rate = (1 - (decimal)_Matrix[_Row - 1, _Column - 1] / intLength);
            // _Result.UseTime = (_EndTime - _BeginTime).ToString();
            _result.ComputeTimes = _ComputeTimes.ToString();
            _result.Difference = _Matrix[_Row - 1, _Column - 1];
        }
        /// <summary>
        /// 计算相似度
        /// </summary>
        /// <param name="str1">字符串1</param>
        /// <param name="str2">字符串2</param>
        public void Compute(string str1, string str2)
        {
            this.StringComputeInit(str1, str2);
            this.Compute();
        }

        /// <summary>
        /// 计算相似度
        /// </summary>
        /// <param name="str1">字符串1</param>
        /// <param name="str2">字符串2</param>
        public void SpeedyCompute(string str1, string str2)
        {
            this.StringComputeInit(str1, str2);
            this.SpeedyCompute();
        }
        /// <summary>
        /// 初始化矩阵的第一行和第一列
        /// </summary>
        private void InitMatrix()
        {
            for (int i = 0; i < _Column; i++)
            {
                _Matrix[0, i] = i;
            }
            for (int i = 0; i < _Row; i++)
            {
                _Matrix[i, 0] = i;
            }
        }
        /// <summary>
        /// 取三个数中的最小值
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <returns></returns>
        private int Minimum(int first, int second, int third)
        {
            int intMin = first;
            if (second < intMin)
            {
                intMin = second;
            }
            if (third < intMin)
            {
                intMin = third;
            }
            return intMin;
        }
        #endregion
    }
    /// <summary>
    /// 相似度计算结果
    /// </summary>
    public struct CompareResult :IComparable<CompareResult >
    {
        /// <summary>
        /// 相似度
        /// </summary>
        public decimal Rate;
        /// <summary>
        /// 对比次数
        /// </summary>
        public string ComputeTimes;
        /// <summary>
        /// 使用时间
        /// </summary>
        public string UseTime;
        /// <summary>
        /// 差异
        /// </summary>
        public int Difference;

        /// <summary>
        /// 字符串1
        /// </summary>
        public string Text1;

        /// <summary>
        /// 字符串2
        /// </summary>
        public string Text2;

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="r2"></param>
        /// <returns></returns>
        public int CompareTo(CompareResult  r2)
        {
            return r2.Rate.CompareTo(this.Rate);
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Rate+""+ Difference;
        }
    }
}
