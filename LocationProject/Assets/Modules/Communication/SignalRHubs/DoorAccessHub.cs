using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using TModel.Location.Data;
using UnityEngine;

public class DoorAccessHub : Hub {
    /// <summary>
    /// 门禁状态推送
    /// </summary>
    public event Action<List<DoorAccessState>> OnDoorAccessStateRecieved;
    public DoorAccessHub(): base("doorAccessHub")
    {
        // Setup server-called functions     
        base.On("GetDoorAccessInfo", GetDoorAccessInfo);
    }
    /// <summary>
    /// 获取门禁推送信息
    /// </summary>
    /// <param name="hub"></param>
    /// <param name="methodCall"></param>
    private void GetDoorAccessInfo(Hub hub, MethodCallMessage methodCall)
    {
        string arg0 = JsonMapper.ToJson(methodCall.Arguments[0]);
        //Debug.LogError("GetDoorAccessInfo....");
        List<DoorAccessState> states = JsonMapper.ToObject<List<DoorAccessState>>(arg0);
        //Debug.Log("Count:"+alarm[0].Dev.DevID);
        if (OnDoorAccessStateRecieved != null) OnDoorAccessStateRecieved(states);
    }

}
