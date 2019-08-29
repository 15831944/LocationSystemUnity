using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCFClientThread : WCFClientSync
{
    public WCFClientThread(string ip, string port) : base(ip, port)
    {

    }

    public override void GetTopoTree(Action<PhysicalTopology> callback)
    {
        ThreadManager.Run(GetTopoTreeSync, callback, "GetTopoTree");
    }

    public override void GetTags(Action<Tag[]> callback)
    {
        ThreadManager.Run(GetTagsSync, callback, "GetTags");
    }

    public override void GetTag(int id, Action<Tag> callback)
    {
        ThreadManager.Run(()=>
        {
            return GetTagSync(id);
        }, callback, "GetTag");
    }

    public override void GetPersonTree(Action<AreaNode> callback)
    {
        ThreadManager.Run(GetPersonTreeSync, callback, "GetPersonTreeAsync");
    }

    public override void GetDepartmentTree(Action<Department> callback)
    {
        ThreadManager.Run(GetDepartmentTreeSync, callback, "GetDepartmentTreeAsync");
    }

    public override void GetAreaStatistics(int id, Action<AreaStatistics> callback)
    {
        ThreadManager.Run(() => { return GetAreaStatisticsSync(id); }, callback, "GetAreaStatistics");
    }

    public override void GetPointsByPid(int areaId, Action<AreaPoints[]> callback)
    {
        ThreadManager.Run(() => { return GetPointsByPidSync(areaId); }, callback, "GetAreaBoundsByPidAsync");
    }
}