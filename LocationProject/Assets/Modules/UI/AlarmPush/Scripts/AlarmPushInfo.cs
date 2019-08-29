using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmPushInfo
{
    /// <summary>
    /// 告警类型
    /// </summary>
    public AlarmPushInfoType AlarmType;
    /// <summary>
    /// 设备告警信息
    /// </summary>
    public DeviceAlarm devAlarmInfo;
    /// <summary>
    /// 人员定位告警信息
    /// </summary>
    public LocationAlarm locationAlarmInfo;
    /// <summary>
    /// 摄像头告警信息
    /// </summary>
    public CameraAlarmInfo CameraAlarmInfor;
    /// <summary>
    /// 设备信息
    /// </summary>
    /// <param name="alarm"></param>
    public void SetAlarmInfo(DeviceAlarm alarm)
    {
        AlarmType = AlarmPushInfoType.devAlarm;
        devAlarmInfo = alarm;
    }
    /// <summary>
    /// 人员定位
    /// </summary>
    /// <param name="alarm"></param>
    public void SetAlarmInfo(LocationAlarm alarm)
    {
        AlarmType = AlarmPushInfoType.locationAlarm;
        locationAlarmInfo = alarm;
    }
    public void SetAlarmInfo(CameraAlarmInfo alaem)
    {
        AlarmType = AlarmPushInfoType.CameraAlarmInfo;
        CameraAlarmInfor = alaem;
    }
}
/// <summary>
/// 告警类型
/// </summary>
public enum AlarmPushInfoType
{
    devAlarm,
    locationAlarm,
    CameraAlarmInfo
}