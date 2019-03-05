using UnityEngine;
using System.Collections;

/// <summary>
/// 数据类型转换帮助类
/// </summary>
public class DataTypeConversionHelper
{
    /// <summary>
    /// 字符串转化为int类型，如果转换出异常返回0；
    /// </summary>
    public static int StringToint(string str)
    {
        int i = 0;
        try
        {
            i = int.Parse(str);
        }
        catch
        {
            i = 0;
        }
        return i;
    }
    /// <summary>
    /// 字符串转化为Float类型，如果转换出异常返回0；
    /// </summary>
    public static float StringToFloat(string str)
    {
        float f = 0;
        try
        {
            f = float.Parse(str);
        }
        catch
        {
            f = 0;
        }
        return f;
    }

    /// <summary>
    /// 字符串转化为Float类型，如果转换出异常返回-1；
    /// </summary>
    public static float StringToFloatNone(string str)
    {
        float f = -1;
        try
        {
            f = float.Parse(str);
        }
        catch
        {
            f = -1;
        }
        return f;
    }
}
