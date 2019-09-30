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
        SceneEvents.FullViewStateChange += OnFullViewChange;
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
        if (isFullView)
        {
            //Debug.LogError("收到设备告警：Count:"+alarm.Count);
            mainPageDeviceAlarms.AddRange(alarm);
        }
        else
        {
            if (OnDeviceAlarmRecieved != null) OnDeviceAlarmRecieved(alarm);
        }
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
        if(isFullView)
        {
            mainPageLocationAlarms.AddRange(alarm);
        }
        else
        {
            if (OnLocationAlarmRecieved != null) OnLocationAlarmRecieved(alarm);
        }
        
    }

    private List<LocationAlarm> mainPageLocationAlarms = new List<LocationAlarm>();
    private List<DeviceAlarm> mainPageDeviceAlarms = new List<DeviceAlarm>();

    private bool isFullView=true;
    private void OnFullViewChange(bool isFullViewT)
    {
        isFullView = isFullViewT;
        if(!isFullView)
        {
            PushLocationAlarm();
            PushDeviceAlarm();
        }
    }


    private void PushLocationAlarm()
    {
        if (mainPageLocationAlarms==null||mainPageLocationAlarms.Count == 0) return;
        if (OnLocationAlarmRecieved != null) OnLocationAlarmRecieved(mainPageLocationAlarms);
        mainPageLocationAlarms.Clear();
    }

    public void PushDeviceAlarm()
    {
        if (mainPageDeviceAlarms == null || mainPageDeviceAlarms.Count == 0) return;
        if (OnDeviceAlarmRecieved != null) OnDeviceAlarmRecieved(mainPageDeviceAlarms);
        //Debug.LogError("Push设备告警：Count:" + mainPageDeviceAlarms.Count);
        mainPageDeviceAlarms.Clear();
    }
}
