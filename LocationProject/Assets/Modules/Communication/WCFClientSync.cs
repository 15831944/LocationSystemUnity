using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using UnityEngine;

public class WCFClientSync : ICommunicationClient
{

    protected LocationServiceClient serviceClient;

    public string ip;
    public string port;

    public WCFClientSync(string ip,string port)
    {
        this.ip = ip;
        this.port = port;
    }

    public LocationServiceClient CreateServiceClient()
    {
        string hostName = ip;
        string portNum = port;
        Binding wsBinding = new BasicHttpBinding();
        //Binding wsBinding = new WSHttpBinding();
        string url =
            string.Format("http://{0}:{1}/LocationService/",
                hostName, portNum);
        Log.Info("Create Service Client:" + url);

        LocationServiceClient client = null;
        try
        {
            EndpointAddress endpointAddress = new EndpointAddress(url);
            client = new LocationServiceClient(wsBinding, endpointAddress);
        }
        catch
        {
            Debug.LogError("CreateServiceClient报错了！");
            return null;
        }

        return client;
    }

    public virtual LocationServiceClient GetServiceClient()
    {
        if (SystemSettingHelper.debugSetting != null && SystemSettingHelper.debugSetting.IsCloseCommunication)
        {
            Debug.LogError("systemSetting.IsCloseCommunication:" + SystemSettingHelper.debugSetting.IsCloseCommunication);
            return null;
        }
        return GetClientOP();
    }

    protected LocationServiceClient GetClientOP()
    {
        if (serviceClient == null)
        {
            if (serviceClient != null)
            {
                //if (client.State == CommunicationState.Opened)
                //{
                //    client.Close();
                //}
                serviceClient.Close();
            }
            serviceClient = CreateServiceClient();
        }
        return serviceClient;
    }

    public void Close()
    {
        EndConnectionServerThread();
        if (connectionServerClient != null)
        {
            connectionServerClient.Close();
            connectionServerClient.Abort();
        }
        if (serviceClient != null)
        {
            serviceClient.Close();
            serviceClient.Abort();
        }
    }
    public void Hello(string msg)
    {
        Debug.Log("->Hello");
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            string hello = serviceClient.Hello(msg);
            Debug.Log("Hello:" + hello);
        }
    }    
    #region 心跳包
    /// <summary>
    /// 连接服务器的线程
    /// </summary>
    private Thread connectionServerThread;   
    /// <summary>
    /// 是否连接服务器成功
    /// </summary>
    public bool isConnectSucceed;

    private int breakingTimes;//断线次数
    private LocationServiceClient connectionServerClient;//连接服务器的客户端

    /// <summary>
    /// 连接服务器心跳包
    /// </summary>
    private void StartConnectionServerThread()
    {
        EndConnectionServerThread();
        connectionServerThread = new Thread(ConnectionServerTesting);
        connectionServerThread.Start();
    }

    /// <summary>
    /// 关闭心跳包
    /// </summary>
    private void EndConnectionServerThread()
    {
        breakingTimes = 0;
        if (connectionServerThread != null)
        {
            connectionServerThread.Abort();
            connectionServerThread = null;
        }
    }

    /// <summary>
    /// 连接服务器检测
    /// </summary>
    private void ConnectionServerTesting()
    {
        while (true)
        {
            Debug.Log("ConnectionServerTesting!");
            if (connectionServerClient != null)
            {
                connectionServerClient.Abort();
            }
            connectionServerClient = CreateServiceClient();
            connectionServerClient.HelloCompleted -= ConnectionServerCallBack;
            connectionServerClient.HelloCompleted += ConnectionServerCallBack;
            connectionServerClient.HelloAsync("1");//心跳包
            Thread.Sleep(1000);
        }
    }

    private void ConnectionServerCallBack(object sender, HelloCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            breakingTimes = 0;
            string result = e.Result;
            Debug.LogFormat("{0}", result);
            isConnectSucceed = true;
        }
        else
        {
            breakingTimes += 1;
            Debug.LogFormat("连接服务失败！");
            isConnectSucceed = false;
        }
    }
    #endregion

    #region 登陆
    /// <summary>
    /// 登录回调
    /// </summary>
    private Action<object, LoginCompletedEventArgs> LoginCompleted;

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="ipT"></param>
    /// <param name="portT"></param>
    /// <param name="loginInfo"></param>
    /// <param name="LoginCompletedT"></param>
    public LocationServiceClient Login(string ipT, string portT, string user,string pass, Action<object, LoginCompletedEventArgs> LoginCompletedT)
    {
        LoginInfo loginInfo = new LoginInfo();
        loginInfo.UserName = user;
        loginInfo.Password = pass;

        ip = ipT;
        port = portT;
        LoginCompleted = null;
        LoginCompleted = LoginCompletedT;
        if (serviceClient != null)
        {
            serviceClient.Abort();
        }
        EndConnectionServerThread();
        serviceClient = CreateServiceClient();
        if (serviceClient != null)
        {
            lock (serviceClient)//1
            {
                serviceClient.LoginCompleted -= LoginCompleted_CallBack;
                serviceClient.LoginCompleted += LoginCompleted_CallBack;//返回数据回调
                serviceClient.LoginAsync(loginInfo);//WCF的异步调用
            }
        }
        return serviceClient;
    }

    private void LoginCompleted_CallBack(object sender, LoginCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            isConnectSucceed = true;
            //StartConnectionServerThread();//发送心跳包
        }
        else
        {
            isConnectSucceed = false;
        }
        if (LoginCompleted != null)
        {
            LoginCompleted(sender, e);
        }
        //DebugLog("LoginCompleted_CallBack isAsync:" + isAsync);
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <param name="loginInfo"></param>
    public void LoginOut(LoginInfo loginInfo)
    {
        EndConnectionServerThread();
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            serviceClient.LogoutAsync(loginInfo);
        }
    }
    #endregion

    /// <summary>
    /// 获取版本号
    /// </summary>
    /// <returns></returns>
    public VersionInfo GetVersionInfo()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            VersionInfo info = serviceClient.GetVersionInfo();
            return info;
        }
    }

    public virtual void DebugLog(string msg)
    {
        Debug.Log("->DebugLog");
        serviceClient = GetServiceClient();
        lock (serviceClient)//1
        {
            serviceClient.DebugMessage(msg);
        }
    }

    protected virtual void LogError(string tag, string msg)
    {
        string txt = Log.Error(tag, msg);
        DebugLog(txt);
    }

    private string GetWhiteSpace(int count)
    {
        string space = "";
        for (int i = 0; i < count; i++)
        {
            space += "  ";
        }
        return space;
    }

    protected string GetTopoText(PhysicalTopology dep, int layer)
    {
        string whitespace = GetWhiteSpace(layer);
        if (dep == null) return "";
        string txt = whitespace + layer + ":" + dep.Name + "\n";
        if (dep.Children != null)
        {
            //txt+=whitespace + "length:" + dep.Children.Length+"\n";
            foreach (PhysicalTopology child in dep.Children)
            {
                txt += GetTopoText(child, layer + 1);
            }
        }
        else
        {
            //txt += whitespace + "children==null\n";
        }
        return txt;
    }

    protected PhysicalTopology topoRoot = null;
    protected PhysicalTopology GetTopoTreeSync()
    {
        Debug.Log("CommunicationObject->GetTopoTree...");
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            int view = 0; //0:基本数据; 1:设备信息; 2:人员信息; 3:设备信息 + 人员信息
            if (topoRoot == null)//第二次进来就不从数据库获取了
            {
                topoRoot = serviceClient.GetPhysicalTopologyTree(view);
            }
            else
            {
                Log.Info("GetTopoTree success 2!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
            if (topoRoot == null)
            {
                LogError("GetTopoTree", "topoRoot == null");
            }
            else
            {
                Log.Info("GetTopoTree success 1!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
            //string txt = ShowTopo(topoRoot, 0);
            //Debug.Log(txt);
            return topoRoot;
        }
    }
    public virtual void GetTopoTree(Action<PhysicalTopology> callback)
    {
        var tags = GetTopoTreeSync();
        if (callback != null)
        {
            callback(tags);
        }
    }

    /// <summary>
    /// 获取定位卡相信息
    /// </summary>
    /// <returns></returns>
    protected Tag[] GetTagsSync()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetTags();
        }
    }

    public virtual void GetTags(Action<Tag[]> callback)
    {
        var tags = GetTagsSync();
        if (callback != null)
        {
            callback(tags);
        }
    }

    /// <summary>
    /// 获取定位卡相信息
    /// </summary>
    /// <returns></returns>
    protected Tag GetTagSync(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetTag(id);
        }
    }

    public virtual void GetTag(int id, Action<Tag> callback)
    {
        var tag = GetTagSync(id);
        if (callback != null)
        {
            callback(tag);
        }
    }


    protected Department GetDepartmentTreeSync()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            Department dep = serviceClient.GetDepartmentTree();
            return dep;
        }
    }

    public virtual void GetDepartmentTree(Action<Department> callback)
    {
        var result = GetDepartmentTreeSync();
        if (callback != null)
        {
            callback(result);
        }
    }

    public virtual void GetPersonTree(Action<AreaNode> callback)
    {
        var result = GetPersonTreeSync();
        if (callback != null)
        {
            callback(result);
        }
    }

    protected AreaNode GetPersonTreeSync()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            Debug.Log("CommunicationObject->GetPersonTree...");
            int view = 2;//0:基本数据;1:基本设备信息;2:基本人员信息;3:基本设备信息+基本人员信息;4:只显示设备的节点;5:只显示人员的节点;6:只显示人员或设备的节点
            AreaNode root = null;
            root = serviceClient.GetPhysicalTopologyTreeNode(view);
            return root;
        }
    }


    public virtual void GetAreaStatistics(int id, Action<AreaStatistics> callback)
    {
        var result = GetAreaStatisticsSync(id);
        if (callback != null)
        {
            callback(result);
        }
    }

    /// <summary>
    /// 园区信息统计
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    protected AreaStatistics GetAreaStatisticsSync(int Id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            return serviceClient.GetAreaStatistics(Id);
        }
    }

    public virtual void GetPointsByPid(int areaId, Action<AreaPoints[]> callback)
    {
        var result = GetPointsByPidSync(areaId);
        if (callback != null)
        {
            callback(result);
        }
    }

    protected AreaPoints[] GetPointsByPidSync(int areaId)
    {
        serviceClient = GetServiceClient();
        lock (serviceClient)
        {
            return serviceClient.GetPointsByPid(areaId);
        }
    }
}
