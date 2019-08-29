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
    public event Action<InspectionTrackList> OnInspectionTrackRecieved;

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
        try
        {
            //Debug.LogError("GetInspectionTrack");
            var arg0 = methodCall.Arguments[0];
            //Debug.LogError(arg0);
            string json = JsonMapper.ToJson(arg0);
            //Debug.LogError(json);
            InspectionTrackList inspection = JsonMapper.ToObject<InspectionTrackList>(json);
            
            //List<InspectionTrack> inspections = JsonMapper.ToObject<List<InspectionTrack>>(json);
            //Debug.LogError(inspection == null);
            //if (inspection != null)
            //{
            //    Debug.LogError(inspection.Count);
            //}

            if (OnInspectionTrackRecieved != null) OnInspectionTrackRecieved(inspection);
        }
        catch (Exception ex)
        {
            //Debug.LogError("GetInspectionTrack:" + ex);
        }

    }
}
