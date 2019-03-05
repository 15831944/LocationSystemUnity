using BestHTTP.SignalR.Hubs;
using BestHTTP.SignalR.Messages;
using LitJson;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectionTrackHub : Hub
{
    /// <summary>
    /// 移动巡检回调
    /// </summary>
    public event Action<List<InspectionTrack>> OnInspectionTrackRecieved;
    public InspectionTrackHub(): base("inspectionTrackHub")
    {
        // Setup server-called functions     
        base.On("GetInspectionTrack", GetInspectionTrack);
    }

    /// <summary>
    /// 移动巡检
    /// </summary>
    /// <param name="hub"></param>
    /// <param name="methodCall"></param>
    private void GetInspectionTrack(Hub hub, MethodCallMessage methodCall)
    {
        string arg0 = JsonMapper.ToJson(methodCall.Arguments[0]);
        List<InspectionTrack> inspection = JsonMapper.ToObject<List<InspectionTrack>>(arg0);
        if (OnInspectionTrackRecieved != null) OnInspectionTrackRecieved(inspection);
    }
}
