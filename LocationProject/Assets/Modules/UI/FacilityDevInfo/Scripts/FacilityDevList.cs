using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生成系统
/// </summary>
public class FacilitySystemList
{
    public List<FacilitySystem> DevList;   
}
/// <summary>
/// 子系统
/// </summary>
public class FacilitySystem
{
    /// <summary>
    /// 设备名称
    /// </summary>
    public string DevName;
    /// <summary>
    /// 设备标识码
    /// </summary>
    public string DevKKS;
    /// <summary>
    /// 状态
    /// </summary>
    public string Status;
    /// <summary>
    /// 值
    /// </summary>
    public string Value;
    /// <summary>
    /// 子系统
    /// </summary>
    public List<FacilitySystem> SubDevs;
}
