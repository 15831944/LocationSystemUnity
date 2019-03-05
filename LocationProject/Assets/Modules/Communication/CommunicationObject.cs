using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using UnityEngine;
using System.Linq;
using System.Threading;

public class CommunicationObject : MonoBehaviour
{
    public static CommunicationObject Instance;

    private LocationServiceClient client;
    /// <summary>
    /// 是否开启异步通信
    /// </summary>
    public bool isAsync;
    /// <summary>
    /// 服务器IP地址
    /// </summary>
    public string ip = "192.168.1.1";//localhost

    public static string currentIp = "";
    /// <summary>
    /// 服务器端口号
    /// </summary>
    public string port = "8733";
    /// <summary>
    /// 连接服务器的线程
    /// </summary>
    private Thread connectionServerThread;
    /// <summary>
    /// 是否连接服务器成功
    /// </summary>
    public bool isConnectSucceed;


    /// <summary>
    /// 位置信息刷新间隔
    /// </summary>
    public float TagPosRefreshInterval = 0.35f;

    /// <summary>
    /// 人员树节点刷新间隔
    /// </summary>
    public int PersonTreeRefreshInterval = 5;

    /// <summary>
    /// 区域统计信息刷新间隔
    /// </summary>
    public int AreaStatisticsRefreshInterval = 5;

    /// <summary>
    /// 附加摄像头刷新间隔
    /// </summary>
    public int NearCameraRefreshInterval = 8;

    /// <summary>
    /// 截图保存刷新间隔
    /// </summary>
    public int ScreenShotRefreshInterval = 3;

    /// <summary>
    /// 部门树节点刷新间隔
    /// </summary>
    public int DepartmentTreeRefreshInterval = 60;

    private void Awake()
    {
        Instance = this;

        SetRefreshSetting();

        if (currentIp != "")
        {
            ip = currentIp;
        }
    }



    [ContextMenu("Start")]
    public void Start()
    {

        //Hello();
        //StartConnectionServerThread();

        //#if !UNITY_EDITOR
        //        ip = SystemSettingHelper.communicationSetting.Ip1;
        //        port = SystemSettingHelper.communicationSetting.Port1;
        //#endif
        //GetTopoTree();
    }

    void OnDisable()
    {
        EndConnectionServerThread();
        if (connectionServerClient != null)
        {
            connectionServerClient.Close();
            connectionServerClient.Abort();
        }
        if (client != null)
        {
            client.Close();
            client.Abort();
        }
    }
    private void OnDestroy()
    {
        EndConnectionServerThread();
        if (connectionServerClient != null)
        {
            connectionServerClient.Close();
            connectionServerClient.Abort();
        }
        if (client != null)
        {
            client.Close();
            client.Abort();
        }
    }

    #region 心跳包

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
        //while (true)
        //{
        //    client = GetClient();
        //    client.HelloCompleted -= ConnectionServerCallBack;
        //    client.HelloCompleted += ConnectionServerCallBack;
        //    if(client==null)return null; lock (client)//1
        //    {
        //        client.HelloAsync("1");//心跳包
        //    }
        //    Thread.Sleep(1000);
        //}
        while (true)
        {
            Debug.Log("ConnectionServerTesting!");
            //if (connectionServerClient == null)
            //{
            if (connectionServerClient != null)
            {
                //        if (connectionServerClient.State == CommunicationState.Opened)
                //        {
                //            connectionServerClient.Close();
                connectionServerClient.Abort();
                //        }
            }

            connectionServerClient = CreateServiceClient();
            //}
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
            //lock (connectionServerClient)
            //{
            //    if (connectionServerClient != null)
            //    {
            //        connectionServerClient.Close();
            //    }
            //    connectionServerClient = CreateServiceClient();
            //}

            breakingTimes += 1;
            Debug.LogFormat("连接服务失败！");
            isConnectSucceed = false;
        }
    }
    #endregion

    public LocationServiceClient CreateServiceClient()
    {
        string hostName = ip;
        string portNum = port;
        System.ServiceModel.Channels.Binding wsBinding = new BasicHttpBinding();
        string url =
            string.Format("http://{0}:{1}/LocationService/",
                hostName, portNum);
        Log.Info("Create Service Client:" + url);

        LocationServiceClient serviceClient = null;
        try
        {
            EndpointAddress endpointAddress = new EndpointAddress(url);
            serviceClient = new LocationServiceClient(wsBinding, endpointAddress);
        }
        catch
        {
            Debug.LogError("CreateServiceClient报错了！");
            return null;
        }

        return serviceClient;
    }

    public LocationServiceClient GetClient()
    {
        if (SystemSettingHelper.systemSetting.IsCloseCommunication)
        {
            Debug.LogError("systemSetting.IsCloseCommunication:" + SystemSettingHelper.systemSetting.IsCloseCommunication);
            return null;
        }

        return GetClientOP();
    }

    private LocationServiceClient GetClientOP()
    {
        if (client == null)
        {
            if (client != null)
            {
                //if (client.State == CommunicationState.Opened)
                //{
                //    client.Close();
                //}
                client.Close();
            }
            client = CreateServiceClient();
        }
        return client;
    }

    //private LocationServiceClient client;

    public void Hello(string msg)
    {
        Debug.Log("->Hello");
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            string hello = client.Hello(msg);
            Debug.Log("Hello:" + hello);
        }
    }

    #region 登录相关
    /// <summary>
    /// 登录回调
    /// </summary>
    public Action<object, LoginCompletedEventArgs> LoginCompleted;

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="ipT"></param>
    /// <param name="portT"></param>
    /// <param name="loginInfo"></param>
    /// <param name="LoginCompletedT"></param>
    public void Login(string ipT, string portT, LoginInfo loginInfo, Action<object, LoginCompletedEventArgs> LoginCompletedT)
    {
        EndConnectionServerThread();
        ip = ipT;
        currentIp = ip;//有两个CommunicationObject
        port = portT;
        LoginCompleted = null;
        LoginCompleted = LoginCompletedT;
        //Debug.Log("->GetTags");
        if (client != null)
        {
            client.Abort();
        }
        client = CreateServiceClient();
        if (client == null)
        {
            DianChangLogin.Instance.LoginFail();
            return;
        }
        else
        {
            DianChangLogin.Instance.LoginProcess();
        }
        if (client == null) return;
        lock (client)//1
        {
            client.LoginCompleted -= LoginCompleted_CallBack;
            client.LoginCompleted += LoginCompleted_CallBack;//返回数据回调
            client.LoginAsync(loginInfo);//WCF的异步调用
        }
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

        DebugLog("LoginCompleted_CallBack isAsync:" + isAsync);
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <param name="loginInfo"></param>
    public void LoginOut(LoginInfo loginInfo)
    {
        EndConnectionServerThread();
        //Debug.Log("->GetTags");
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            client.LogoutAsync(loginInfo);
        }
    }
    /// <summary>
    /// 获取版本号
    /// </summary>
    /// <returns></returns>
    public VersionInfo GetVersionInfo()
    {
        //client = GetClient();
        client = GetClientOP();
        
        if (client == null) return null;
        lock (client)//1
        {
            VersionInfo info = client.GetVersionInfo();
            return info;
        }
    }
    #endregion
    PhysicalTopology topoRoot = null;
    private PhysicalTopology GetTopoTree()
    {
        Debug.Log("CommunicationObject->GetTopoTree...");
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            int view = 0; //0:基本数据; 1:设备信息; 2:人员信息; 3:设备信息 + 人员信息
            if (topoRoot == null)//第二次进来就不从数据库获取了
            { topoRoot = client.GetPhysicalTopologyTree(view); }
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

    //用回调函数的方式异步获取拓扑树
    public void GetTopoTreeAsync(Action<PhysicalTopology> callback)
    {
        if (!isAsync)
        {
            ThreadManager.Run(() =>
            {
                return GetTopoTree();
            }, callback, "GetTopoTreeAsync");
        }
        else
        {
            client = GetClient();
            if (client == null) return;
            lock (client)//1
            {
                int view = 0; //0:基本数据; 1:设备信息; 2:人员信息; 3:设备信息 + 人员信息
                if (topoRoot == null)//第二次进来就不从数据库获取了
                {
                    client.BeginGetPhysicalTopologyTree(view, (ar) =>
                     {
                         topoRoot = null;
                         try
                         {
                             LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                             topoRoot = client.EndGetPhysicalTopologyTree(ar);
                         }
                         catch (Exception ex)
                         {
                             LogError("CommunicationObject", ex.ToString());
                         }

                         Loom.DispatchToMainThread(() =>
                         {
                             if (callback != null)
                             {
                                 callback(topoRoot);
                             }
                         });
                         if (topoRoot == null)
                         {
                             LogError("GetTopoTree", "topoRoot == null");
                         }
                         else
                         {
                             Log.Info("GetTopoTree success 1!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                         }
                     }, client);
                }
                else
                {
                    Log.Info("GetTopoTree success 2!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
            }
        }
    }

    [ContextMenu("TTTTT")]
    public void TTTTT()
    {
        int view = 0; //0:基本数据; 1:设备信息; 2:人员信息; 3:设备信息 + 人员信息
        Debug.Log("TTTTT0");
        PhysicalTopology topoRoot = client.GetPhysicalTopologyTree(view);
        Debug.Log("TTTTT1");
    }

    private string ShowTopo(PhysicalTopology dep, int layer)
    {
        string whitespace = GetWhiteSpace(layer);
        if (dep == null) return "";
        string txt = whitespace + layer + ":" + dep.Name + "\n";
        if (dep.Children != null)
        {
            //txt+=whitespace + "length:" + dep.Children.Length+"\n";
            foreach (PhysicalTopology child in dep.Children)
            {
                txt += ShowTopo(child, layer + 1);
            }
        }
        else
        {
            //txt += whitespace + "children==null\n";
        }
        return txt;
    }

    // Update is called once per frame
    void Update()
    {
        if (WaitFormManage.Instance != null)
        {
            //先关闭断线重连判断功能20181220
            //if (breakingTimes > 3 && isConnectSucceed == false)//重连服务器失败超过3次,
            //{
            //    WaitFormManage.Instance.ShowConnectSeverWaitPanel();
            //}
            //else
            //{
            //    WaitFormManage.Instance.HideConnectSeverWaitPanel();
            //}
        }
    }

    //public void GetUser()
    //{
    //    Debug.Log("->GetUser");
    //    client = GetClient();
    //    if(client==null)return null; lock (client)//1
    //    {
    //        User user = client.GetUser();
    //        Debug.Log("Id:" + user.Id);
    //        Debug.Log("Name:" + user.Name);
    //    }
    //}

    //public void GetUsers()
    //{
    //    Debug.Log("->GetUsers");
    //    client = GetClient();
    //    if(client==null)return null; lock (client)//1
    //    {

    //        User[] list = client.GetUsers();
    //        for (int i = 0; i < list.Length; i++)
    //        {
    //            Debug.Log("i:" + i);
    //            Debug.Log("Id:" + list[i].Id);
    //            Debug.Log("Name:" + list[i].Name);
    //        }
    //    }
    //}

    /// <summary>
    /// 获取定位卡相信息
    /// </summary>
    /// <returns></returns>
    private Tag[] GetTags()
    {
        //Debug.Log("->GetTags");
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            Tag[] list = client.GetTags();
            //for (int i = 0; i < list.Length; i++)
            //{
            //    Debug.Log("i:" + i);
            //    Debug.Log("Id:" + list[i].Id);
            //    Debug.Log("Name:" + list[i].Name);
            //}
            return list;
        }
    }

    public Department GetDepartmentTree()
    {
        //if (isConnectSucceed == false) return null;
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            Debug.Log("CommunicationObject->GetDepTree...");
            Department dep = client.GetDepartmentTree();
            //string txt = ShowDep(dep, 0);
            //Debug.Log(txt);
            return dep;
        }

    }

    public AreaNode GetPersonTree()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            Debug.Log("CommunicationObject->GetPersonTree...");
            int view = 2;//0:基本数据;1:基本设备信息;2:基本人员信息;3:基本设备信息+基本人员信息;4:只显示设备的节点;5:只显示人员的节点;6:只显示人员或设备的节点
            AreaNode root = null;
            root = client.GetPhysicalTopologyTreeNode(view);
            return root;
        }
    }

    List<string> funcList = new List<string>();
    List<object> callbackList = new List<object>();
    List<object> funcArg = new List<object>();
    private bool isBusy = false;

    public void GetData(string name, object callback, object arg = null)
    {
        Debug.LogError("GetData:name=" + name);
        if (isBusy)//基本上一直是busy
        {
            Debug.LogError("2");
            Debug.LogError("Add:name=" + name);
            //lock (funcList)
            {
                funcList.Add(name);
                callbackList.Add(callback);
                funcArg.Add(arg);
            }
            DoNextGetData();
        }
        else
        {
            Debug.LogError("1");
            isBusy = true;
            DoGetData(name, callback, arg);
        }


        //todo:似乎不同的消息确实会相互影响，可能需要一个队列，在客户端这边控制消息异步顺序发送执行。
    }

    private void DoGetData(string name, object callback, object arg = null)
    {
        Debug.LogError("DoGetData:name=" + name);
        if (name == "GetPersonTreeAsync")
        {
            DoGetPersonTreeAsync((data) =>
            {
                ((Action<AreaNode>)callback)(data);
                DoNextGetData();
            });
        }
        if (name == "GetDepartmentTreeAsync")
        {
            DoGetDepartmentTreeAsync((data) =>
            {
                ((Action<Department>)callback)(data);
                DoNextGetData();
            });
        }
        if (name == "DoGetTagsAsync")
        {
            DoGetTagsAsync((data) =>
            {
                ((Action<Tag[]>)callback)(data);
                DoNextGetData();
            });
        }
        if (name == "DoGetAreaStatisticsAsync")
        {
            int id = (int)arg;
            DoGetAreaStatisticsAsync(id, (data) =>
             {
                 ((Action<AreaStatistics>)callback)(data);
                 DoNextGetData();
             });
        }
    }

    private void DoNextGetData()
    {
        //lock (funcList)
        {
            if (funcList.Count > 0)
            {
                string name2 = funcList[0]; funcList.RemoveAt(0);
                object callback2 = callbackList[0]; callbackList.RemoveAt(0);
                object arg2 = funcArg[0]; funcArg.RemoveAt(0);
                //Debug.LogError("DoNextGetData:name=" + name);
                DoGetData(name2, callback2, arg2);
            }
        }
    }

    public void GetTagsAsync(Action<Tag[]> callback)
    {
        //GetData("GetTagsAsync", callback);
        //DoGetTagsAsync(callback);

        //tagsPos.Clear();
        if (!isAsync)
        {
            ThreadManager.Run(GetTags, callback, "GetTags");
        }
        else
        {
            DoGetTagsAsync(callback);
        }
    }

    public void DoGetTagsAsync(Action<Tag[]> callback)
    {
        Log.Info("GetTagsAsync Start >>>>>>>>>>");
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            client.BeginGetTags((ar) =>
            {
                Tag[] result = null;
                try
                {
                    //Debug.LogError("GetTagsAsync ar.IsCompleted:" + ar.IsCompleted);
                    LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                    result = client.EndGetTags(ar);
                }
                catch (Exception ex)
                {
                    LogError("CommunicationObject", ex.ToString());
                    Debug.LogError("GetTagsAsync报错！：" + ar.IsCompleted);
                }

                Loom.DispatchToMainThread(() =>
                {
                    if (callback != null)
                    {
                        callback(result);
                    }
                });
                if (result == null) LogError("GetTagsAsync", "result == null");
                Log.Info("GetTagsAsync End <<<<<<<<");
            }, client);
        }
    }

    public void GetDepartmentTreeAsync(Action<Department> callback)
    {
        //GetData("GetDepartmentTreeAsync", callback);
        //DoGetDepartmentTreeAsync(callback);
        if (!isAsync)
        {
            //var topoRoot = GetDepartmentTree();
            ////AfterGetDepTree(topoRoot, callback);
            //callback(topoRoot);

            ThreadManager.Run(GetDepartmentTree, callback, "GetDepartmentTree");
        }
        else
        {
            DoGetDepartmentTreeAsync(callback);
        }
    }

    public void DoGetDepartmentTreeAsync(Action<Department> callback)
    {
        Log.Info("GetDepartmentTreeAsync Start >>>>>>>>>>");
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            client.BeginGetDepartmentTree((ar) =>
            {
                Department result = null;
                try
                {
                    LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                    result = client.EndGetDepartmentTree(ar);
                }
                catch (Exception ex)
                {
                    LogError("CommunicationObject", ex.ToString());
                }
                Loom.DispatchToMainThread(() =>
                {
                    if (callback != null)
                    {
                        callback(result);
                    }
                });
                if (result == null) LogError("GetDepartmentTreeAsync", "result == null");
                Log.Info("GetDepartmentTreeAsync End <<<<<<<<");
            }, client);
        }
    }


    public void GetPersonTreeAsync(Action<AreaNode> callback)
    {
        //GetData("GetPersonTreeAsync", callback);
        DoGetPersonTreeAsync(callback);
    }

    public void DoGetPersonTreeAsync(Action<AreaNode> callback)
    {
        Log.Info("GetPersonTreeAsync Start >>>>>>>>>>");
        if (!isAsync)
        {
            ThreadManager.Run(GetPersonTree, callback, "GetPersonTreeAsync");
        }
        else
        {
            client = GetClient();
            if (client == null) return;
            lock (client)//1
            {
                //Debug.LogError("BeginGetPersonTreeAsync........");
                int view = 2; //0:基本数据; 1:设备信息; 2:人员信息; 3:设备信息 + 人员信息
                client.BeginGetPhysicalTopologyTreeNode(view, (ar) =>
                {
                    AreaNode result = null;
                    try
                    {
                        LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                        //Debug.LogError("EndGetPersonTreeAsync........");
                        result = client.EndGetPhysicalTopologyTreeNode(ar);
                    }
                    catch (Exception ex)
                    {
                        LogError("CommunicationObject", ex.ToString());
                    }
                    Loom.DispatchToMainThread(() =>
                    {
                        if (callback != null)
                        {
                            callback(result);
                        }
                    });
                    if (result == null) LogError("GetPersonTreeAsync", "result == null");
                    Log.Info("GetPersonTreeAsync End <<<<<<<<");
                }, client);
            }
        }
    }

    public void GetAreaStatisticsAsync(int Id, Action<AreaStatistics> callback)
    {
        //GetData("GetAreaStatisticsAsync",callback, Id);
        //DoGetAreaStatisticsAsync(Id, callback);

        //Log.Error("FVIntroduce.TryGetAreaInfo");
        if (!isAsync)
        {
            ThreadManager.Run(() =>
            {
                return GetAreaStatistics(Id);
            }, callback, "GetAreaStatisticsAsync");
        }
        else
        {
            DoGetAreaStatisticsAsync(Id, callback);
        }
    }

    public void DoGetAreaStatisticsAsync(int Id, Action<AreaStatistics> callback)
    {
        Log.Info("GetAreaStatisticsAsync Start >>>>>>>>>>");
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            //Debug.LogError("BeginGetAreaStatistics........");
            client.BeginGetAreaStatistics(Id, (ar) =>
            {
                AreaStatistics result = null;
                try
                {
                    LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                    //Debug.LogError("EndGetAreaStatistics........");
                    result = client.EndGetAreaStatistics(ar);
                }
                catch (Exception ex)
                {
                    LogError("CommunicationObject", ex.ToString());
                }
                Loom.DispatchToMainThread(() =>
                {
                    if (callback != null)
                    {
                        callback(result);
                    }
                });
                if (result == null) LogError("GetAreaStatisticsAsync", "result == null");
                Log.Info("GetAreaStatisticsAsync End <<<<<<<<<<");
            }, client);
        }
    }

    private void LogError(string tag, string msg)
    {
        string txt = Log.Error(tag, msg);
        DebugLog(txt);
    }

    public void GetRealPositonsAsync(Action<List<TagPosition>> callback)
    {
        Log.Info("GetRealPositonsAsync Start <<<<<<<<<<");
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            client.BeginGetRealPositons((ar) =>
            {
                List<TagPosition> result = new List<TagPosition>();
                try
                {
                    //Debug.LogError("GetRealPositonsAsync ar.IsCompleted:" + ar.IsCompleted);
                    LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                    var result2 = client.EndGetRealPositons(ar);
                    result.AddRange(result2);
                }
                catch (Exception ex)
                {
                    LogError("CommunicationObject", ex.ToString());
                    Debug.LogError("GetRealPositonsAsync报错！：" + ar.IsCompleted); ;
                }

                Loom.DispatchToMainThread(() =>
                {
                    if (callback != null)
                    {
                        callback(result.ToList());
                    }
                });
                if (result == null) LogError("GetRealPositonsAsync", "result == null");
                Log.Info("GetRealPositonsAsync End <<<<<<<<<<");
            }, client);
        }
    }

    /// <summary>
    /// 获取人员列表
    /// </summary>
    public void GetAreaBoundsByPidAsync(int areaId,Action<AreaPoints[]> callback)
    {
        string pid = areaId + "";
        client = GetClient();
        if (!isAsync)
        {
            ThreadManager.Run(() =>
            {
                return client.GetPointsByPid(areaId + "");
            }, callback, "GetAreaStatisticsAsync");
        }
        else
        {
            Log.Info("GetAreaBoundsByPidAsync Start <<<<<<<<<<");
            client.BeginGetPointsByPid(pid, (ar) =>
            {
                AreaPoints[] result = null;
                try
                {
                    LocationServiceClient client = ar.AsyncState as LocationServiceClient;
                    //Debug.LogError("EndGetAreaStatistics........");
                    result = client.EndGetPointsByPid(ar);
                }
                catch (Exception ex)
                {
                    LogError("CommunicationObject", ex.ToString());
                }
                Loom.DispatchToMainThread(() =>
                {
                    if (callback != null)
                    {
                        callback(result);
                    }
                });
                if (result == null) LogError("GetAreaStatisticsAsync", "result == null");
                Log.Info("GetAreaStatisticsAsync End <<<<<<<<<<");
            }, client);
        }
    }

    /// <summary>
    /// 获取人员列表
    /// </summary>
    public Personnel[] GetPersonnels()
    {
        //Department topoRoot = GetDepTree();
        //return GetPersonnels(topoRoot);

        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            Debug.Log("CommunicationObject->GetPersonnels...");
            return client.GetPersonList();
        }
    }

    /// <summary>
    /// 获取人员列表
    /// </summary>
    public static List<Personnel> GetPersonnels(Department topoRoot)
    {
        List<Personnel> personnelsT = new List<Personnel>();
        if (topoRoot == null) return personnelsT;
        if (topoRoot.Children == null) return personnelsT;
        foreach (Department child in topoRoot.Children)
        {
            if (child.LeafNodes != null)
            {
                personnelsT.AddRange(child.LeafNodes);
            }
            personnelsT.AddRange(GetPersonnels(child));
        }
        return personnelsT;
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

    private string ShowDep(Department dep, int layer)
    {
        string whitespace = GetWhiteSpace(layer);
        string txt = whitespace + layer + ":" + dep.Name + "\n";
        if (dep.Children != null)
        {
            //txt+=whitespace + "length:" + dep.Children.Length+"\n";
            foreach (Department child in dep.Children)
            {
                txt += ShowDep(child, layer + 1);
            }
        }
        else
        {
            //txt += whitespace + "children==null\n";
        }
        return txt;
    }
    /// <summary>
    /// 获取定位卡位置信息
    /// </summary>
    /// <returns></returns>
    public List<TagPosition> GetRealPositons()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            //Log.Info("GetTagsPosition Start");
            TagPosition[] arr = client.GetRealPositons();
            List<TagPosition> list = new List<TagPosition>();
            if (arr != null)
            {
                list.AddRange(arr);
            }
            //Log.Info("GetTagsPosition End");
            return list;
        }
    }


    /// <summary>
    /// 获取取定位卡历史位置信息
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositons()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            Position[] arr = client.GetHistoryPositons();
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                list.AddRange(arr);
            }
            return list;
        }
    }

    /// <summary>
    /// 获取取定位卡历史位置信息,根据时间
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositonsByTime(string tagcode, DateTime start, DateTime end)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            Position[] arr = client.GetHistoryPositonsByTime(tagcode, start, end);
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                list.AddRange(arr);
            }

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            foreach (Position p in list)
            {
                DateTime time = startTime.AddMilliseconds(p.Time);
                //print(time.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            }
            return list;
        }
    }

    /// <summary>
    /// 获取取定位卡历史位置信息,根据时间和人员Id
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositonsByPersonnelID(int personnelID, DateTime start, DateTime end)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            Position[] arr = client.GetHistoryPositonsByPersonnelID(personnelID, start, end);
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                list.AddRange(arr);
            }

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            foreach (Position p in list)
            {
                DateTime time = startTime.AddMilliseconds(p.Time);
                //print(time.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            }
            return list;
        }
    }

    /// <summary>
    /// 获取取定位卡历史位置信息,根据时间和和TopoNodeId建筑id列表(人员所在的区域)
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositonsByPidAndTopoNodeIds(int personnelID, List<int> topoNodeIdsT, DateTime start, DateTime end)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            Position[] arr = client.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT.ToArray(), start, end);
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                list.AddRange(arr);
            }

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            foreach (Position p in list)
            {
                DateTime time = startTime.AddMilliseconds(p.Time);
                //print(time.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            }
            return list;
        }
    }

    /// <summary>
    /// 获取大数据量测试
    /// </summary>
    public void GetStrsTest()
    {
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            DateTime datetimeStart = DateTime.Now;

            //string s = client.GetStrs(100000);
            DateTime datetimeEnd = DateTime.Now;
            float t = (float)(datetimeEnd - datetimeStart).TotalSeconds;
            Debug.LogError("GetStrsTest:" + t);
        }
    }

    ///// <summary>
    ///// 编辑区域
    ///// </summary>
    //public void EditArea(Area area)
    //{
    //    client = GetClient();
    //    if(client==null)return null; lock (client)//1
    //    {
    //        client.EditArea(area);
    //    }
    //}

    /// <summary>
    /// 3D保存历史数据
    /// </summary>
    public bool AddU3DPosition(U3DPosition position)
    {
        try
        {
            client = GetClient();
            if (client == null) return false;
            lock (client)//1//不用线程使用同一个client发送消息会相互干扰
            {
                //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
                Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
                List<U3DPosition> pList = new List<U3DPosition>();
                pList.Add(position);
                //client.AddU3DPosition(position);
                AddU3DPosition(pList);
                //Debug.Log(string.Format("---------《《《《《《《《{0}---------", position.Tag));
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
            return false;
        }

        //try
        //{
        //    //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
        //    Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
        //    LocationServiceClient c = CreateServiceClient();
        //    c.AddU3DPosition(position);
        //    //Debug.Log(string.Format("---------《《《《《《《《{0}---------", position.Tag));
        //    return true;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.ToString());
        //    return false;
        //}
    }

    /// <summary>
    /// 3D保存历史数据
    /// </summary>
    public bool AddU3DPosition(List<U3DPosition> pList)
    {
        try
        {
            client = GetClient();
            if (client == null) return false;
            lock (client)//1//不用线程使用同一个client发送消息会相互干扰
            {
                //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
                //Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
                client.AddU3DPositions(pList.ToArray());
                //Debug.Log(string.Format("---------《《《《《《《《{0}---------", position.Tag));
            }
            return true;
        }
        catch (Exception ex)
        {
            Thread.CurrentThread.Abort();
            Debug.LogError(ex.ToString());
            return false;
        }

        //try
        //{
        //    //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
        //    Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
        //    LocationServiceClient c = CreateServiceClient();
        //    c.AddU3DPosition(position);
        //    //Debug.Log(string.Format("---------《《《《《《《《{0}---------", position.Tag));
        //    return true;
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex.ToString());
        //    return false;
        //}
    }

    /// <summary>
    ///  获取标签3D历史位置
    /// </summary>
    /// <param name="tagcode"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public List<U3DPosition> GetHistoryU3DPositonsByTime(string tagcode, DateTime start, DateTime end)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            U3DPosition[] arr = client.GetHistoryU3DPositonsByTime(tagcode, start, end);
            List<U3DPosition> list = new List<U3DPosition>();
            if (arr != null)
            {
                list.AddRange(arr);
            }

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            foreach (U3DPosition p in list)
            {
                DateTime time = startTime.AddMilliseconds(p.Time);
                print(time.ToString("yyyy/MM/dd HH:mm:ss:ffff"));
            }
            return list;
        }
    }
    /// <summary>
    /// 获取模型添加列表
    /// </summary>
    public List<ObjectAddList_Type> GetModelAddList()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            ObjectAddList_Type[] TypeList = client.GetObjectAddList();
            if (TypeList == null)
            {
                LogError("GetModelAddList", "TypeList == null");
                return null;
            }
            return TypeList.ToList();
        }
    }
    /// <summary>
    /// 删除设备信息
    /// </summary>
    /// <param name="dev"></param>
    public void DeleteDevInfo(DevInfo dev)
    {
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            bool value = client.DeleteDevInfo(dev);
            Debug.Log("DeleteDevInfo result:" + value);
        }
    }
    /// <summary>
    /// 保存设备信息
    /// </summary>
    public void AddDevInfo(ref DevInfo dev)
    {
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            //DevInfo value = client.AddDevInfo(dev);
            dev = client.AddDevInfo(dev);
        }
    }
    /// <summary>
    /// 添加设备
    /// </summary>
    /// <param name="devInfos"></param>
    public List<DevInfo> AddDevInfo(List<DevInfo> devInfos)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            DevInfo[] devs = devInfos.ToArray();
            devInfos = client.AddDevInfoByList(devs).ToList();
            Debug.Log("AddDevInfoByList result:" + devInfos.Count);
            return devInfos;
        }
    }
    /// <summary>
    /// 修改设备信息
    /// </summary>
    /// <param name="Info"></param>
    public void ModifyDevInfo(DevInfo Info)
    {
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            bool value = client.ModifyDevInfo(Info);
            Debug.Log("ModifyDevInfo result:" + value);
        }
    }
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="PosInfo"></param>
    public void ModifyDevPos(DevPos PosInfo)
    {
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            bool value = client.ModifyPosInfo(PosInfo);
            //Debug.Log("ModifyDevPos result:" + value);
        }
    }
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="posList"></param>
    public void ModifyDevPosByList(List<DevPos> posList)
    {
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            bool value = client.ModifyPosByList(posList.ToArray());
            Debug.Log("ModifyDevPos result:" + value);
        }
    }
    /// <summary>
    /// 添加设备位置信息
    /// </summary>
    /// <param name="devPos"></param>
    public void AddDevPosInfo(DevPos devPos)
    {
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            bool value = client.AddDevPosInfo(devPos);
            Debug.Log("AddDevPos result:" + value);
        }
    }
    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    /// <returns></returns>
    public List<DevInfo> GetAllDevInfos()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            DevInfo[] infoList = client.GetAllDevInfos();
            if (infoList == null) return new List<DevInfo>();
            return infoList.ToList();
        }
    }
    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    /// <returns></returns>
    public List<DevInfo> GetDevInfos(int[] typeList)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            DevInfo[] infoList = client.GetDevInfos(typeList);
            if (infoList == null) return new List<DevInfo>();
            return infoList.ToList();
        }
    }
    /// <summary>
    /// 通过设备Id,获取设备信息
    /// </summary>
    /// <param name="devId"></param>
    public DevInfo GetDevByDevId(string devId)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            DevInfo devInfo = client.GetDevByID(devId);
            return devInfo;
        }
    }

    /// <summary>
    /// 通过设备Id,获取设备信息
    /// </summary>
    /// <param name="devId"></param>
    public DevInfo GetDevByid(int id)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            DevInfo devInfo = client.GetDevByiId(id);
            return devInfo;
        }
    }

    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    /// <returns></returns>
    public List<DevInfo> FindDevInfos(string key)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            DevInfo[] infoList = client.FindDevInfos(key);
            if (infoList == null) return new List<DevInfo>();
            return infoList.ToList();
        }
    }
    /// <summary>
    /// 获取所有设备,位置信息
    /// </summary>
    /// <returns></returns>
    public List<DevPos> GetAllPosInfo()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            DevPos[] infoList = client.GetDevPositions();
            if (infoList == null) return null;
            return infoList.ToList();
        }
    }
    public List<DevInfo> GetDevInfoByParent(List<int> idList)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            DevInfo[] infoList = client.GetDevInfoByParent(idList.ToArray());
            if (infoList == null)
            {
                return new List<DevInfo>();
            }
            return infoList.ToList();
        }
    }

    /// <summary>
    /// 获取园区下的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetParkMonitorRange()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            IList<PhysicalTopology> results = client.GetParkMonitorRange();
            return results;
        }
    }

    /// <summary>
    /// 获取楼层下的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetFloorMonitorRange()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            IList<PhysicalTopology> results = client.GetFloorMonitorRange();
            return results;
        }
    }

    /// <summary>
    /// 根据PhysicalTopology的Id获取楼层以下级别的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetFloorMonitorRangeById(int id)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            IList<PhysicalTopology> results = client.GetFloorMonitorRangeById(id);
            return results;
        }
    }

    /// <summary>
    /// 根据节点添加监控范围
    /// </summary>
    public bool EditMonitorRange(PhysicalTopology pt)
    {

        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.EditMonitorRange(pt);
        }
    }

    /// <summary>
    /// 根据节点添加子监控范围
    /// </summary>
    public bool AddMonitorRange(PhysicalTopology pt)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.AddMonitorRange(pt);
        }
    }

    /// <summary>
    /// 获取配置信息
    /// </summary>
    /// <returns></returns>
    public ConfigArg GetConfigArgByKey(string key)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            return client.GetConfigArgByKey(key);
        }
    }

    /// <summary>
    /// 设置配置信息
    /// </summary>
    /// <returns></returns>
    public bool GetConfigArgByKey(ConfigArg config)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.EditConfigArg(config);
        }
    }

    /// <summary>
    /// 获取坐标系转换配置信息
    /// </summary>
    /// <returns></returns>
    public TransferOfAxesConfig GetTransferOfAxesConfig()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            return client.GetTransferOfAxesConfig();
        }
    }

    /// <summary>
    /// 设置坐标系转换配置信息
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public bool SetTransferOfAxesConfig(TransferOfAxesConfig config)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.SetTransferOfAxesConfig(config);
        }
    }
    /// <summary>
    /// 人员搜索（CaiCai）
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<Personnel> FindPersonnelData(string key)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            Personnel[] infoList = client.FindPersonList(key);
            if (infoList == null) return new List<Personnel>();
            return infoList.ToList();
        }
    }
    public Post[] GetPostList()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            return client.GetPostList();
        }
    }

    public LocationAlarm[] GetLocationAlarms(AlarmSearchArg arg)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            return client.GetLocationAlarms(arg);
        }
    }

    public DeviceAlarm[] GetDeviceAlarms(AlarmSearchArg arg)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            return client.GetDeviceAlarms(arg);
        }
    }
    /// <summary>
    /// 园区信息统计
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public AreaStatistics GetAreaStatistics(int Id)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            return client.GetAreaStatistics(Id);
        }

    }

    /// <summary>
    /// 附近人员
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public NearbyPerson[] GetNearbyPerson_Currency(int Id, float distance)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            return client.GetNearbyPerson_Currency(Id, distance);
        }
    }
    /// <summary>
    /// 人员找附近摄像头
    /// </summary>
    /// <param name="id"></param>
    /// <param name="distance"></param>
    /// <param name="nFlag"></param>
    /// <returns></returns>
    public NearbyDev[] GetNearbyDev_Currency(int id, float distance, int nFlag)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            return client.GetNearbyDev_Currency(id, distance, nFlag);
        }
    }
    /// <summary>
    /// 人员经过门禁
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EntranceGuardActionInfo[] GetEntranceActionInfoByPerson24Hours(int id)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {

            return client.GetEntranceActionInfoByPerson24Hours(id);
        }
    }
    /// <summary>
    /// 增加门禁设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool AddDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.AddDoorAccessByList(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 添加门禁信息
    /// </summary>
    /// <param name="doorAccess"></param>
    /// <returns></returns>
    public bool AddDoorAccess(Dev_DoorAccess doorAccess)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.AddDoorAccess(doorAccess);
        }
    }
    /// <summary>
    /// 删除门禁设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool DeleteDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.DeleteDoorAccess(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 修改门禁信息
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool ModifyDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.ModifyDoorAccess(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 通过区域ID，获取区域下门禁信息
    /// </summary>
    /// <param name="pidList"></param>
    /// <returns></returns>
    public List<Dev_DoorAccess> GetDoorAccessInfoByParent(List<int> pidList)
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            List<Dev_DoorAccess> doorList;
            Dev_DoorAccess[] doors = client.GetDoorAccessInfoByParent(pidList.ToArray());
            if (doors == null) doorList = new List<Dev_DoorAccess>();
            else doorList = doors.ToList();
            return doorList;
        }
    }
    #region 摄像头信息
    /// <summary>
    /// 增加摄像头设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool AddCameraInfo(List<Dev_CameraInfo> cameraInfoList)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.AddCameraInfoByList(cameraInfoList.ToArray());
        }
    }
    /// <summary>
    /// 增加摄像头设备
    /// </summary>
    /// <param name="cameraInfo"></param>
    /// <returns></returns>
    public Dev_CameraInfo AddCameraInfo(Dev_CameraInfo cameraInfo)
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            return client.AddCameraInfo(cameraInfo);
        }
    }
    /// <summary>
    /// 删除摄像头设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool DeleteCameraInfo(List<Dev_CameraInfo> cameraInfoList)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.DeleteCameraInfo(cameraInfoList.ToArray());
        }
    }
    /// <summary>
    /// 修改摄像头设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool ModifyCameraInfo(Dev_CameraInfo cameraInfo)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.ModifyCameraInfo(cameraInfo);
        }
    }
    /// <summary>
    /// 通过区域ID，获取区域下摄像头设备
    /// </summary>
    /// <param name="pidList"></param>
    /// <returns></returns>
    public List<Dev_CameraInfo> GetCameraInfoByParent(List<int> pidList)
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            List<Dev_CameraInfo> cameraInfoList;
            Dev_CameraInfo[] cameraInfos = client.GetCameraInfoByParent(pidList.ToArray());
            if (cameraInfos == null) cameraInfoList = new List<Dev_CameraInfo>();
            else cameraInfoList = cameraInfos.ToList();
            return cameraInfoList;
        }
    }
    /// <summary>
    /// 获取所有的摄像头信息
    /// </summary>
    /// <returns></returns>
    public List<Dev_CameraInfo> GetAllCameraInfo()
    {
        client = GetClient();
        if (client == null) return null; lock (client)//1
        {
            List<Dev_CameraInfo> cameraInfoList;
            Dev_CameraInfo[] cameraInfos = client.GetAllCameraInfo();
            if (cameraInfos == null) cameraInfoList = new List<Dev_CameraInfo>();
            else cameraInfoList = cameraInfos.ToList();
            return cameraInfoList;
        }
    }
    /// <summary>
    /// 通过设备信息，获取对应摄像头信息
    /// </summary>
    /// <param name="dev"></param>
    /// <returns></returns>
    public Dev_CameraInfo GetCameraInfoByDevInfo(DevInfo dev)
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            Dev_CameraInfo cameraInfo = client.GetCameraInfoByDevInfo(dev);
            return cameraInfo;
        }
    }
    #endregion
    #region 基站编辑
    /// <summary>
    /// 获取所有基站信息
    /// </summary>
    /// <returns></returns>
    public List<Archor> GetArchors()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {

            List<Archor> archors = client.GetArchors().ToList();
            return archors;
        }
    }

    public Archor GetArchor(string archorId)
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {

            Archor archor = client.GetArchor(archorId);
            return archor;
        }
    }

    public Archor GetArchorByDevId(int devId)
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {

            Archor archor = client.GetArchorByDevId(devId);
            return archor;
        }
    }
    public bool AddArchor(Archor archor)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.AddArchor(archor);
        }
    }
    public bool EditArchor(Archor archor, int parentId)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            bool value = client.EditArchor(archor, parentId);
            return value;
        }
    }
    public bool DeleteArchor(int devId)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            return client.DeleteArchor(devId);
        }
    }
    public bool EditBusAnchor(Archor busArchor, int parentId)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            bool value = client.EditArchor(busArchor, parentId);
            return value;
        }
    }
    #endregion
    #region 两票移动巡检

    /// <summary>
    ///  获取操作票列表
    /// </summary>
    /// <returns></returns>

    public List<OperationTicket> GetOperationTicketList()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            return client.GetOperationTicketList().ToList();
        }
        //var operationTickets = db.OperationTickets.ToList();
        //return operationTickets.ToWcfModelList();
    }

    /// <summary>
    /// 获取工作票列表
    /// </summary>
    /// <returns></returns>
    public List<WorkTicket> GetWorkTicketList()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            return client.GetWorkTicketList().ToList();
        }
    }

    /// <summary>
    /// 获取巡检设备列表
    /// </summary>
    public List<MobileInspectionDev> GetMobileInspectionDevList()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            return client.GetMobileInspectionDevList().ToList();
        };
    }

    /// <summary>
    /// 获取巡检轨迹列表
    /// </summary>
    /// <returns></returns>
    public List<MobileInspection> GetMobileInspectionList()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            return client.GetMobileInspectionList().ToList();
        };
    }

    /// <summary>
    /// 获取人员巡检轨迹列表
    /// </summary>
    /// <returns></returns>
    public List<PersonnelMobileInspection> GetPersonnelMobileInspectionList()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            PersonnelMobileInspection[] ps = client.GetPersonnelMobileInspectionList();
            if (ps == null)
            {
                List<PersonnelMobileInspection> pList = new List<PersonnelMobileInspection>();
                return pList;
            }
            else
            {
                return ps.ToList();
            }
        };
    }

    /// <summary>
    /// 获取操作票历史记录
    /// </summary>
    /// <returns></returns>
    public List<OperationTicketHistory> GetOperationTicketHistoryList()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            return client.GetOperationTicketHistoryList().ToList();
        };
    }

    /// <summary>
    /// 获取工作票历史记录
    /// </summary>
    /// <returns></returns>
    public List<WorkTicketHistory> GetWorkTicketHistoryList()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            return client.GetWorkTicketHistoryList().ToList();
        };
    }

    public List<InspectionTrackHistory> Getinspectionhistorylist(DateTime dtBeginTime, DateTime dtEndTime, bool bFlag)
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            InspectionTrackHistory[] ps = client.Getinspectionhistorylist(dtBeginTime, dtEndTime, bFlag);
            if (ps == null)
            {
                List<InspectionTrackHistory> pList = new List<InspectionTrackHistory>();
                return pList;
            }
            else
            {
                return ps.ToList();
            }
        }
    }

    /// <summary>
    /// 获取人员巡检轨迹历史记录
    /// </summary>
    /// <returns></returns>
    public List<PersonnelMobileInspectionHistory> GetPersonnelMobileInspectionHistoryList()
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            PersonnelMobileInspectionHistory[] ps = client.GetPersonnelMobileInspectionHistoryList();
            if (ps == null)
            {
                List<PersonnelMobileInspectionHistory> pList = new List<PersonnelMobileInspectionHistory>();
                return pList;
            }
            else
            {
                return ps.ToList();
            }
            //return client.GetPersonnelMobileInspectionHistoryList().ToList();
        };
    }

    #endregion
    #region 顶视图截图

    /// <summary>
    /// 编辑图片
    /// </summary>
    /// <param name="picture"></param>
    /// <returns></returns>
    public bool EditPictureInfo(Picture picture)
    {
        client = GetClient();
        if (client == null) return false;
        lock (client)//1
        {
            bool value = client.EditPictureInfo(picture);
            return value;
        }
    }
    /// <summary>
    /// 获取图片信息
    /// </summary>
    /// <param name="pictureName"></param>
    /// <returns></returns>
    public Picture GetPictureInfo(string pictureName)
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            Picture value = client.GetPictureInfo(pictureName);
            return value;
        }
    }
    #endregion
    #region 设备KKS监控

    public Dev_Monitor GetMonitorInfoByKKS(string kksCode, bool isContainChild)
    {
        client = GetClient();
        if (client == null) return null;
        lock (client)//1
        {
            Dev_Monitor value = client.GetDevMonitorInfoByKKS(kksCode, isContainChild);
            return value;
        }
    }

    #endregion

    public void GetUnitySetting()
    {
        Debug.Log("->GetUnitySetting");
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            var setting = client.GetUnitySetting();//从服务端获取配置信息
            var refreshSetting = setting.RefreshSetting;
            SetRefreshSetting(refreshSetting);
        }
    }

    private void SetRefreshSetting()
    {
#if UNITY_EDITOR
        RefreshSetting refreshSetting = SystemSettingHelper.refreshSetting;
        if (refreshSetting == null)
        {
            Debug.LogError("refreshSetting==null"); return;
        }
        SetRefreshSetting(refreshSetting);
#endif
    }

    private void SetRefreshSetting(RefreshSetting refreshSetting)
    {
        if (refreshSetting == null)
        {
            Debug.LogError("refreshSetting==null"); return;
        }
        TagPosRefreshInterval = refreshSetting.TagPos;
        PersonTreeRefreshInterval = refreshSetting.PersonTree;
        AreaStatisticsRefreshInterval = refreshSetting.AreaStatistics;
        NearCameraRefreshInterval = refreshSetting.NearCamera;
        ScreenShotRefreshInterval = refreshSetting.ScreenShot;
        DepartmentTreeRefreshInterval = refreshSetting.DepartmentTree;
    }

    private void SetRefreshSetting(Location.WCFServiceReferences.LocationServices.RefreshSetting refreshSetting)
    {
        if (refreshSetting == null)
        {
            Debug.LogError("refreshSetting==null"); return;
        }
        TagPosRefreshInterval = refreshSetting.TagPos;
        PersonTreeRefreshInterval = refreshSetting.PersonTree;
        AreaStatisticsRefreshInterval = refreshSetting.AreaStatistics;
        NearCameraRefreshInterval = refreshSetting.NearCamera;
        ScreenShotRefreshInterval = refreshSetting.ScreenShot;
        DepartmentTreeRefreshInterval = refreshSetting.DepartmentTree;
    }

    public void DebugLog(string msg)
    {
        Debug.Log("->DebugLog");
        client = GetClient();
        if (client == null) return;
        lock (client)//1
        {
            client.DebugMessage(msg);
        }
    }
}
