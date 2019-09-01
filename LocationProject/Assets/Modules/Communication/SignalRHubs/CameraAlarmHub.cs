using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using LitJson;
using Location.WCFServiceReferences.LocationServices;
using UnityEngine;

public class CameraAlarmHub : Hub
{
    /// <summary>
    /// 设备告警
    /// </summary>
    public event Action<List<CameraAlarmInfo>> OnCameraAlarmsRecieved;

    public CameraAlarmHub()
        : base("cameraAlarmHub")
    {
        // Setup server-called functions     
        base.On("GetCameraAlarms", GetCameraAlarms);
    }

    /// <summary>
    /// 设备告警回调
    /// </summary>
    /// <param name="hub"></param>
    /// <param name="methodCall"></param>
    private void GetCameraAlarms(Hub hub, MethodCallMessage methodCall)
    {
        var arg = methodCall.Arguments[0];
        string json = JsonMapper.ToJson(arg);
        Debug.Log("CameraAlarmHub.GetCameraAlarms");
        Debug.Log(json);
        List<CameraAlarmInfo> alarms = JsonMapper.ToObject<List<CameraAlarmInfo>>(json);
        ////Debug.Log("OnAlarmRecieved:"+methodCall.Arguments.Length);
        if (OnCameraAlarmsRecieved != null) OnCameraAlarmsRecieved(alarms);
    }
}
