using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using LitJson;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmHub : Hub
{
    /// <summary>
    /// 设备告警
    /// </summary>
    public event Action<List<DeviceAlarm>> OnDeviceAlarmRecieved;
    /// <summary>
    /// 定位告警
    /// </summary>
    public event Action<List<LocationAlarm>> OnLocationAlarmRecieved;
    public AlarmHub()
        : base("alarmHub")
    {
        // Setup server-called functions     
        base.On("GetDeviceAlarms", GetDeviceAlarms);
        base.On("GetLocationAlarms", GetLocationAlarms);
    }
    /// <summary>
    /// 设备告警回调
    /// </summary>
    /// <param name="hub"></param>
    /// <param name="methodCall"></param>
    private void GetDeviceAlarms(Hub hub, MethodCallMessage methodCall)
    {       
        string arg0 = JsonMapper.ToJson(methodCall.Arguments[0]);
        List<DeviceAlarm> alarm = JsonMapper.ToObject<List<DeviceAlarm>>(arg0);
        //Debug.Log("OnAlarmRecieved:"+methodCall.Arguments.Length);
        if (OnDeviceAlarmRecieved != null) OnDeviceAlarmRecieved(alarm);      
    }
    /// <summary>
    /// 定位告警回调
    /// </summary>
    /// <param name="hub"></param>
    /// <param name="methodCall"></param>
    private void GetLocationAlarms(Hub hub, MethodCallMessage methodCall)
    {
        string arg0 = JsonMapper.ToJson(methodCall.Arguments[0]);
        List<LocationAlarm> alarm = JsonMapper.ToObject<List<LocationAlarm>>(arg0);
        //Debug.Log("OnAlarmRecieved:"+methodCall.Arguments.Length);
        if (OnLocationAlarmRecieved != null) OnLocationAlarmRecieved(alarm);
    }
}
