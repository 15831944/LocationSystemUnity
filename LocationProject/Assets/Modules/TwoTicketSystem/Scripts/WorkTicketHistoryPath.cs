using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

/// <summary>
/// 工作票人员历史
/// </summary>
public class WorkTicketHistoryPath : HistoryPath
{

    /// <summary>
    /// 人员信息
    /// </summary>
    protected Personnel personnel;

    // Use this for initialization
    protected override void Start()
    {
        StartInit();

        foreach (PositionInfoList splinePointsT in PosInfoGroup)
        {
            CreateHistoryPath(splinePointsT.GetVector3List(), splinePointsT.Count);
        }

        isCreatePathComplete = true;
    }

    protected override void Update()    {
        base.Update();
        //RefleshDrawLine();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    ///// <summary>
    ///// 初始化
    ///// </summary>
    //public void Init(Personnel personnelT, Color colorT, List<Vector3> splinePointsT, List<DateTime> timelistT, int segmentsT, bool pathLoopT)
    //{
    //    personnel = personnelT;
    //    segments = segmentsT;
    //    splinePoints = splinePointsT;
    //    timelist = timelistT;
    //    color = colorT;
    //    pathLoop = pathLoopT;
    //}

    public void Init(PathInfo pathInfo, bool pathLoopT)
    {
        personnel = pathInfo.personnelT;
        segments = pathInfo.posList.Count;
        //splinePoints = splinePointsT;
        //timelist = timelistT;
        PosInfoList = pathInfo.posList;
        color = pathInfo.color;
        pathLoop = pathLoopT;
    }

    protected override void StartInit()
    {
        lines = new List<VectorLine>();
        dottedlines = new List<VectorLine>();        CreatePathParent();        //LocationHistoryManager.Instance.AddHistoryPath(this as LocationHistoryPath);
        //transform.SetParent(pathParent);        if (PosCount <= 1) return;

        GroupingLine();
    }


    public void CreatePathParent()
    {
        GameObject historyPathParent = GameObject.Find("WorkTicketHistoryPathParent");
        if (historyPathParent == null)
        {
            historyPathParent = new GameObject("WorkTicketHistoryPathParent");

        }
        if (pathParent == null)
        {
            pathParent = new GameObject("pathParent"+ personnel.Name).transform;
            pathParent.SetParent(historyPathParent.transform);
        }

        transform.SetParent(pathParent);
    }
}
