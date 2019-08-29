using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WCFClientAsync : WCFClientSync
{
    public WCFClientAsync(string ip, string port):base(ip,port)
    {
        
    }

    public override LocationServiceClient GetServiceClient()
    {
        return CreateServiceClient();//异步处理 全部重新创建
    }

    private void DoGetTopoTreeAsync(Action<PhysicalTopology> callback)
    {
        var clet = CreateServiceClient();
        {
            int view = 0; //0:基本数据; 1:设备信息; 2:人员信息; 3:设备信息 + 人员信息
            if (topoRoot == null)//第二次进来就不从数据库获取了
            {
                clet.BeginGetPhysicalTopologyTree(view, (ar) =>
                {
                    topoRoot = null;
                    try
                    {
                        LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                        topoRoot = client.EndGetPhysicalTopologyTree(ar);//异步却不是多线程，服务端关闭时这里还是会卡住
                        client.Close();//异步方式用完Close
                    }
                    catch (Exception ex)
                    {
                        LogError("CommunicationObject", ex.ToString());
                    }

                    DoCallBack(callback, topoRoot);
                    if (topoRoot == null)
                    {
                        LogError("GetTopoTree", "topoRoot == null");
                    }
                    else
                    {
                        Log.Info("GetTopoTree success 1!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    }
                }, clet);
                //clet.Close();
            }
            else
            {
                Log.Info("GetTopoTree success 2!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
        }
    }

    private void DoCallBack<T>(Action<T> callback,T data)
    {
        Loom.DispatchToMainThread(() =>
        {
            if (callback != null)
            {
                callback(data);
            }
        });
        //if (callback != null)
        //{
        //    callback(data);
        //}
    }

    public override void GetTopoTree(Action<PhysicalTopology> callback)
    {
        DoGetTopoTreeAsync(callback);
    }

    private void DoGetTagsAsync(Action<Tag[]> callback)
    {
        Log.Info("GetTagsAsync Start >>>>>>>>>>");
        var clet = GetServiceClient();
        clet.BeginGetTags((ar) =>
        {
            Tag[] result = null;
            try
            {
                LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                result = client.EndGetTags(ar);
                client.Close();//异步方式用完Close
            }
            catch (Exception ex)
            {
                LogError("CommunicationObject", ex.ToString());
                Debug.LogError("GetTagsAsync报错！：" + ar.IsCompleted);
            }

            DoCallBack(callback, result);
            if (result == null) LogError("GetTagsAsync", "result == null");
            Log.Info("GetTagsAsync End <<<<<<<<");
        }, clet);
        //clet.Close();
    }


    public override void GetTags(Action<Tag[]> callback)
    {
        DoGetTagsAsync(callback);
    }

    public override void GetTag(int id, Action<Tag> callback)
    {
        Log.Info("GetTag Start >>>>>>>>>>");
        var clet = GetServiceClient();
        clet.BeginGetTag(id, (ar) =>
             {
                 Tag result = null;
                 try
                 {
                     var client = ar.AsyncState as LocationServiceClient;
                     result = client.EndGetTag(ar);
                     client.Close();//异步方式用完Close
                 }
                 catch (Exception ex)
                 {
                     LogError("CommunicationObject", ex.ToString());
                     Debug.LogError("GetTag报错！：" + ar.IsCompleted);
                 }
                 DoCallBack(callback, result);
                 if (result == null) LogError("GetTag", "result == null");
                 Log.Info("GetTag End <<<<<<<<");
             }, clet);
        //clet.Close();
    }

    public override void GetPersonTree(Action<AreaNode> callback)
    {
        DoGetPersonTreeAsync(callback);
    }

    private void DoGetPersonTreeAsync(Action<AreaNode> callback)
    {
        Log.Info("GetPersonTreeAsync Start >>>>>>>>>>");
        var clet = GetServiceClient();
        //Debug.LogError("BeginGetPersonTreeAsync........");
        int view = 2; //0:基本数据; 1:设备信息; 2:人员信息; 3:设备信息 + 人员信息
        clet.BeginGetPhysicalTopologyTreeNode(view, (ar) =>
        {
            AreaNode result = null;
            try
            {
                LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                    //Debug.LogError("EndGetPersonTreeAsync........");
                    result = client.EndGetPhysicalTopologyTreeNode(ar);
                client.Close();//异步方式用完Close
            }
            catch (Exception ex)
            {
                LogError("CommunicationObject", ex.ToString());
            }
            DoCallBack(callback, result);
            if (result == null) LogError("GetPersonTreeAsync", "result == null");
            Log.Info("GetPersonTreeAsync End <<<<<<<<");
        }, clet);
        //clet.Close();
    }

    public void DoGetDepartmentTreeAsync(Action<Department> callback)
    {
        Log.Info("GetDepartmentTreeAsync Start >>>>>>>>>>");
        var clet = CreateServiceClient();
        clet.BeginGetDepartmentTree((ar) =>
        {
            Department result = null;
            try
            {
                var client = ar.AsyncState as LocationServiceClient;
                result = client.EndGetDepartmentTree(ar);
                client.Close();//异步方式用完Close
            }
            catch (Exception ex)
            {
                LogError("CommunicationObject", ex.ToString());
            }
            DoCallBack(callback, result);
            if (result == null) LogError("GetDepartmentTreeAsync", "result == null");
            Log.Info("GetDepartmentTreeAsync End <<<<<<<<");
        }, clet);
        //clet.Close();
    }

    public override void GetDepartmentTree(Action<Department> callback)
    {
        DoGetDepartmentTreeAsync(callback);
    }

    public void DoGetAreaStatisticsAsync(int Id, Action<AreaStatistics> callback)
    {
        Log.Info("GetAreaStatisticsAsync Start >>>>>>>>>>");
        var clet = GetServiceClient();
        //Debug.LogError("BeginGetAreaStatistics........");
        clet.BeginGetAreaStatistics(Id, (ar) =>
        {
            AreaStatistics result = null;
            try
            {
                LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                    //Debug.LogError("EndGetAreaStatistics........");
                    result = client.EndGetAreaStatistics(ar);
                client.Close();//异步方式用完Close
            }
            catch (Exception ex)
            {
                LogError("CommunicationObject", ex.ToString());
            }
            DoCallBack(callback, result);
            if (result == null) LogError("GetAreaStatisticsAsync", "result == null");
            Log.Info("GetAreaStatisticsAsync End <<<<<<<<<<");
        }, clet);
        //clet.Close();
    }

    public override void GetAreaStatistics(int id, Action<AreaStatistics> callback)
    {
        DoGetAreaStatisticsAsync(id, callback);
    }

    public void DoGetRealPositonsAsync(Action<List<TagPosition>> callback)
    {
        Log.Info("GetRealPositonsAsync Start <<<<<<<<<<");
        var clet = GetServiceClient();
        clet.BeginGetRealPositons((ar) =>
            {
                List<TagPosition> result = new List<TagPosition>();
                try
                {
                    var client = ar.AsyncState as LocationServiceClient;
                    var result2 = client.EndGetRealPositons(ar);
                    result.AddRange(result2);
                    client.Close();//异步方式用完Close
                }
                catch (Exception ex)
                {
                    LogError("CommunicationObject", ex.ToString());
                    Debug.LogError("GetRealPositonsAsync报错！：" + ar.IsCompleted); ;
                }

                DoCallBack(callback, result);
                if (result == null) LogError("GetRealPositonsAsync", "result == null");
                Log.Info("GetRealPositonsAsync End <<<<<<<<<<");
            }, clet);
        //clet.Close();
    }

    public override void GetPointsByPid(int areaId, Action<AreaPoints[]> callback)
    {
        DoGetPointsByPidAsync(areaId, callback);
    }

    private void DoGetPointsByPidAsync(int pid, Action<AreaPoints[]> callback)
    {
        Log.Info("GetAreaBoundsByPidAsync Start <<<<<<<<<<");
        var clet = CreateServiceClient();
        clet.BeginGetPointsByPid(pid, (ar) =>
        {
            AreaPoints[] result = null;
            try
            {
                var client = ar.AsyncState as LocationServiceClient;
                result = client.EndGetPointsByPid(ar);
                client.Close();//异步方式用完Close
            }
            catch (Exception ex)
            {
                LogError("CommunicationObject", ex.ToString());
            }
            DoCallBack(callback, result);
            if (result == null) LogError("GetAreaStatisticsAsync", "result == null");
            Log.Info("GetAreaStatisticsAsync End <<<<<<<<<<");
        }, clet);
        //clet.Close();
    }

    public override void DebugLog(string msg)
    {
        Debug.Log("->DebugLog");
        var clet = GetServiceClient();
        clet.DebugMessage(msg);
        //clet.Close();
    }

    protected override void LogError(string tag, string msg)
    {
        //string txt = Log.Error(tag, msg);
        //DebugLog(txt);
    }

}
