using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 数据集合相关帮助类（数组，List）
/// </summary>
public static class M_ListHelper  {

    /// <summary>
    /// List的添加
    /// </summary>
    public static List<T> Add_M<T>(this List<T> list,T item) where T:Object
    {
        if (item == null)
        {
            return list;
        }
        if (list.Contains(item))
        {
            return list;
        }
        else
        {
            list.Add(item);
            return list;
        }
    }

    /// <summary>
    /// List的移除
    /// </summary>
    public static List<T> Remove_M<T>(this List<T> list, T item) where T : Object
    {
        if (item == null)
        {
            return list;
        }
        if (list.Contains(item))
        {
            list.Remove(item);
            return list;
        }
        else
        {
            return list;
        }
    }
}
