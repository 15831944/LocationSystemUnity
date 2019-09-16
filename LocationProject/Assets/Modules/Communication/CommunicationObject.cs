using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.ServiceModel.Channels;
using Unity.Modules.Context;

public class CommunicationObject : MonoBehaviour, IDataClient
{
    private static CommunicationObject _instance;

    public static CommunicationObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CommunicationObject>();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public static string currentIp = "";

    /// <summary>
    /// 服务器IP地址
    /// </summary>
    public string ip = "127.0.0.1";//localhost
    /// <summary>
    /// 服务器端口号
    /// </summary>
    public string port = "8733";

    public string userName = "Admin";

    public string password = "Admin@123456";

    public string session = "";

    public string authority = "";//Guest Guest@123 => guest；Admin Admin@123456 => admin
    /// <summary>
    /// 是否连接服务端
    /// </summary>
    public static bool IsConnect = true;
    /// <summary>
    /// 访客模式
    /// </summary>
    /// <returns></returns>
    public bool IsGuest()
    {
        return authority == "guest";
    }

    /// <summary>
    /// 访客模式
    /// </summary>
    /// <returns></returns>
    public bool IsAdmin()
    {
        return authority == "admin";
    }

    public RefreshSetting RefreshSetting = new RefreshSetting();

    public CommunicationMode Mode = CommunicationMode.Thread;


    private WebApiClient clientWebApi;
    private WCFClientSync clientSync;
    private WCFClientAsync clientAsync;
    private WCFClientThread clientThread;

    private LocationServiceClient serviceClient;

    public LocationServiceClient GetServiceClient()
    {
        if (clientSync == null)
        {
            clientSync = new WCFClientSync(ip, port);
        }
        var client = clientSync.GetServiceClient();

#if UNITY_EDITOR
        //if (loginInfo == null)
        //{
        //    Login(ip, port, userName, password, null);
        //}
#endif

        return client;
    }

    public ICommunicationClient GetClient()
    {
        ICommunicationClient client = null;
        //LogInfo("GetClientEx", "Mode=" + Mode);
        if (Mode == CommunicationMode.Sync)//同步
        {
            if (clientSync == null)
            {
                clientSync = new WCFClientSync(ip, port);
            }
            client = clientSync;
        }
        else if (Mode == CommunicationMode.Thread)//多线程异步
        {
            if (clientThread == null)
            {
                clientThread = new WCFClientThread(ip, port);
            }
            client = clientThread;
        }
        else if (Mode == CommunicationMode.Async)
        {
            if (clientAsync == null)
            {
                clientAsync = new WCFClientAsync(ip, port);
            }
            client = clientAsync;
        }
        else if (Mode == CommunicationMode.WebApi)
        {
            if (clientWebApi == null)
            {
                clientWebApi = gameObject.AddComponent<WebApiClient>();
                clientWebApi.host = ip;
                clientWebApi.port = port;
            }
            client = clientWebApi;
        }
        else
        {
            Log.Error("GetClient Error!!! Mode==" + Mode);
        }
        return client;
    }

    public WebApiClient GetWebApiClient()
    {
        if (clientWebApi == null)
        {
            clientWebApi = gameObject.AddComponent<WebApiClient>();
            clientWebApi.host = ip;
            clientWebApi.port = port;
        }
        return clientWebApi;
    }

    public void HeartBeat(string info, Action<string> callback, Action<string> errorCallback)
    {
        WebApiClient client = GetWebApiClient();
        client.HeartBeat(info, callback, errorCallback);
    }

 

    /// <summary>
    /// 设置单例
    /// </summary>
    private void SetInstance()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance != this)
            {
                Debug.LogError("CommunicateObject is exist...");
                gameObject.RemoveComponent<CommunicationObject>();
            }
        }
    }
    private void Awake()
    {
        AppContext.DataClient = this;

        //Instance = this;
        SetInstance();
        if (currentIp != "")
        {
            ip = currentIp;
        }
        if (AssetBundleHelper.HttpUrl == "")
        {
            AssetBundleHelper.HttpUrl = "http://" + ip + ":8000//StreamingAssets";
            var path = AssetBundleHelper.GetAssetPath();
            //Debug.LogError("path:" + path);
        }
        serviceClient = GetServiceClient();

        //if (SystemSettingHelper.communicationSetting != null)
        //{
        //    if(SystemSettingHelper.communicationSetting.Mode!= CommunicationMode.Editor)
        //    {
        //        Mode = SystemSettingHelper.communicationSetting.Mode;
        //    }
        //}



#if !UNITY_EDITOR
        if (SystemSettingHelper.communicationSetting != null)
        {
            if (SystemSettingHelper.communicationSetting.Mode == CommunicationMode.Editor)
            {
                Mode = CommunicationMode.Thread;
            }
            else
            {
                Mode = SystemSettingHelper.communicationSetting.Mode;
            }

        }
#else
        if (Mode == CommunicationMode.Editor)
        {
            Mode = CommunicationMode.Thread;
        }
#endif

    }



    [ContextMenu("Start")]
    public void Start()
    {
        //#if !UNITY_EDITOR
        //isAsync = SystemSettingHelper.systemSetting.IsAsync;
        //#endif
        SetRefreshSetting();
    }

    void OnDisable()
    {
        Stop();
    }
    private void OnDestroy()
    {
        Stop();
    }

    private void Stop()
    {
        if (serviceClient != null)
        {
            serviceClient.Close();
        }
    }
    #region 登录相关
    /// <summary>
    /// 登录回调
    /// </summary>
    private Action<object, LoginCompletedEventArgs> LoginCompleted;

    private LoginInfo loginInfo = null;


    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="ipT"></param>
    /// <param name="portT"></param>
    /// <param name="loginInfo"></param>
    /// <param name="LoginCompletedT"></param>

    public void Login(LoginInfo info, Action<LoginInfo> callback, Action<string> errorCallback)
    {
        WebApiClient client = GetWebApiClient();
        //client.Login(info, callback, errorCallback);

        string url = client.GetBaseUrl() + "users/LoginPost";
        Debug.Log(url);
        StartCoroutine(client.PostObject(url, info, callback, errorCallback));
    }

    public void Login(string ipT, string portT, string user, string pass, Action<LoginInfo> loginCompletedT)
    {
        Action<LoginInfo> LoginCompletedT = loginCompletedT;
        loginInfo = new LoginInfo();
        loginInfo.UserName = user;
        loginInfo.Password = pass;

        AssetBundleHelper.HttpUrl = "http://" + ipT + ":8000//StreamingAssets";

        var path = AssetBundleHelper.GetAssetPath();
        Debug.LogError("path:" + path);

        currentIp = ipT;//有两个CommunicationObject
        ip = ipT;
        port = portT;
        userName = user;
        password = pass;

        serviceClient = clientSync.Login(ipT, portT, user, pass, (sender, e) =>
         {
             if (e.Error == null)
             {
                 loginInfo = e.Result;
                 if (loginInfo != null)
                 {
                     session = loginInfo.Session;
                     authority = loginInfo.Authority;
                     Debug.LogError("Authority:" + authority);
                 }
             }
             else
             {
                 Debug.LogError("Login Error:" + e.Error);
             }

             if (LoginCompletedT != null)
             {
                 LoginCompletedT(loginInfo);
             }
         });

        if (DianChangLogin.Instance != null)
        {
            if (serviceClient == null)
            {

                DianChangLogin.Instance.LoginFail();
            }
            else
            {
                DianChangLogin.Instance.LoginProcess();
            }
        }
        else
        {
            //Debug.LogError("DianChangLogin.Instance == null");
        }
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <param name="loginInfo"></param>
    public void LoginOut(LoginInfo loginInfo)
    {
        clientSync.LoginOut(loginInfo);
    }
    public void LoginOut(LoginInfo info, Action<LoginInfo> callback, Action<string> errorCallback)
    {
        WebApiClient client = GetWebApiClient();
        client.LoginOut(info,callback,errorCallback);
    }

    /// <summary>
    /// 获取版本号
    /// </summary>
    /// <returns></returns>
    public VersionInfo GetVersionInfo()
    {
        return clientSync.GetVersionInfo();
    }
    #endregion


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

    public bool showLog = false;

    public void LogInfo(object tag, object info)
    {
        if (showLog)
            Log.Info(tag, info);
    }

    //public string RootName = "四会热电厂";

    public int RootIndex = 0;//-1:全部;0:四会热电厂;1:初灵大楼;2:上海宝信

    //用回调函数的方式异步获取拓扑树
    public void GetTopoTree(Action<PhysicalTopology> callback)
    {
        LogInfo("GetTopoTree", "Mode=" + Mode);
        var client = GetClient();
        client.GetTopoTree((root) =>
        {
            PhysicalTopology parkNode = root;
            if (root != null)
            {
                if (RootIndex >= 0 && root.Children != null && root.Children.Length > RootIndex)
                { //统一在通讯处过滤，树控件那里就不用管了，todo:后续改成传个参数个服务端，在服务端过滤
                    parkNode = root.Children[RootIndex];
                }
            }
            else
            {
                Log.Error("CommunicationObject.GetTopoTree.root is null...");
            }

            if (parkNode != null)
            {
                SetMoniterRangePosition(parkNode);
            }

            if (callback != null)
            {
                callback(parkNode);
            }
        });
    }

    public void SetMoniterRangePosition(PhysicalTopology parent)
    {
        if (parent == null) return;

        if (parent.Children != null)
            foreach (PhysicalTopology child in parent.Children)
            {
                if (parent.Type == AreaTypes.楼层)
                {
                    if (child.Type == AreaTypes.范围) //宝信项目偏移修改
                    {
                        child.Transfrom.X -= parent.InitBound.MinX;
                        child.Transfrom.Z -= parent.InitBound.MinY;
                    }
                }
                else
                {
                    SetMoniterRangePosition(child);
                }
            }

    }

    public void GetTags(Action<Tag[]> callback)
    {
        //LogInfo("GetTags", "Mode=" + Mode);
        var client = GetClient();
        client.GetTags(callback);
    }


    public void GetDepartmentTree(Action<Department> callback)
    {
        LogInfo("GetDepartmentTree", "Mode=" + Mode);
        var client = GetClient();
        client.GetDepartmentTree(callback);
    }

    public void GetPersonTree(Action<AreaNode> callback)
    {
        LogInfo("GetPersonTree", "Mode=" + Mode);
        var client = GetClient();
        client.GetPersonTree((root) =>
        {
            if (RootIndex >= 0 && root.Children != null && root.Children.Length > RootIndex)//统一在通讯处过滤，树控件那里就不用管了，todo:后续改成传个参数个服务端，在服务端过滤
            {
                root = root.Children[RootIndex];
            }
            if (callback != null)
            {
                callback(root);
            }
        });
    }

    public void GetAreaStatistics(int id, Action<AreaStatistics> callback)
    {
        LogInfo("GetAreaStatisticsAsync", "Mode=" + Mode);
        if (id == 0)
        {
            Debug.LogWarning("GetAreaStatistics id == 0");
        }
        var client = GetClient();
        client.GetAreaStatistics(id, callback);
    }

    private void LogError(string tag, string msg)
    {
        string txt = Log.Error(tag, msg);
        DebugLog(txt);
    }

    /// <summary>
    /// 获取区域坐标信息
    /// </summary>
    public void GetPointsByPid(int areaId, Action<AreaPoints[]> callback)
    {
        LogInfo("GetAreaBoundsByPidAsync", "Mode=" + Mode);
        var client = GetClient();
        client.GetPointsByPid(areaId, callback);
    }


    /// <summary>
    /// 获取人员列表
    /// </summary>
    public Personnel[] GetPersonnels()
    {
        //Department topoRoot = GetDepTree();
        //return GetPersonnels(topoRoot);

        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            Debug.Log("CommunicationObject->GetPersonnels...");
            return serviceClient.GetPersonList(false);
        }
    }
    /// <summary>
    /// 删除人员
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool DeletePerson(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeletePerson(id);
        }
    }
    /// <summary>
    /// 得到部门信息列表
    /// </summary>
    /// <returns></returns>
    public IList<Department> GetDepartmentList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            IList<Department> results = serviceClient.GetDepartmentList();
            return results;
        }
    }
    public IList<Tag> GetTags()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            IList<Tag> results = serviceClient.GetTags();
            return results;
        }
    }
    /// <summary>
    /// 删除标签
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool DeleteTag(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeleteTag(id);
        }
    }
    /// <summary>
    /// 得到人员
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Personnel GetPerson(int id)
    {

        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            Personnel per = serviceClient.GetPerson(id);
            return per;
        }
    }
    /// <summary>
    /// 编辑人员信息
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool EditPerson(Personnel p)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {

            return serviceClient.EditPerson(p);
        }
    }
    /// <summary>
    /// 添加人员信息
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public int AddPerson(Personnel p)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return 0;
        lock (serviceClient)//1
        {

            return serviceClient.AddPerson(p);
        }
    }
    /// <summary>
    /// 得到岗位名称
    /// </summary>
    /// <returns></returns>
    public List<Post> GetJobsList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            Post[] arr = serviceClient.GetPostList();
            List<Post> list = new List<Post>();
            if (arr != null)
            {
                list.AddRange(arr);
            }
            return list;
        }
    }
    /// <summary>
    /// 编辑岗位信息
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool EditPost(Post p)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.EditPost(p);
        }
    }
    /// <summary>
    /// 编辑部门信息
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool EditDepartment(Department p)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.EditDepartment(p);
        }
    }
    /// <summary>
    /// 添加部门信息
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public int AddDepartment(Department p)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return 0;
        lock (serviceClient)//1
        {
            return serviceClient.AddDepartment(p);
        }
    }
    /// <summary>
    /// 添加岗位
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public int AddPost(Post p)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return 0;
        lock (serviceClient)//1
        {
            return serviceClient.AddPost(p);
        }
    }
    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool DeleteDepartment(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeleteDepartment(id);
        }
    }
    /// <summary>
    /// 删除岗位
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool DeletePost(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeletePost(id);
        }
    }

    /// <summary>
    /// 编辑人员角色定位
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool EditTag(Tag tag)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.EditTag(tag);
        }
    }
    public bool EditCardRole(CardRole p)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.EditCardRole(p);
        }
    }
    public int AddCardRole(CardRole p)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return 0;
        lock (serviceClient)//1
        {
            return serviceClient.AddCardRole(p);
        }
    }
    public bool DeleteCardRole(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeleteCardRole(id);
        }
    }
    public bool SetCardRoleAccessAreas(int roleId, List<int> areaIds)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            int[] list = areaIds.ToArray();
            return serviceClient.SetCardRoleAccessAreas(roleId, list);
        }
    }
    /// <summary>
    /// 添加人员角色定位
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public int AddTag(Tag tag)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return 0;
        lock (serviceClient)//1
        {

            return serviceClient.AddTag(tag);
        }
    }
    /// <summary>
    /// 获取人员角色
    /// </summary>
    /// <returns></returns>
    public List<CardRole> GetCardRoleList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            CardRole[] arr = serviceClient.GetCardRoleList();
            List<CardRole> list = new List<CardRole>();
            if (arr != null)
            {
                list.AddRange(arr);
            }
            return list;
        }
    }
    public List<int> GetCardRoleAccessAreas(int role)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            int[] arr = serviceClient.GetCardRoleAccessAreas(role);
            List<int> list = new List<int>();
            if (arr != null)
            {
                list.AddRange(arr);
            }
            return list;
        }
    }
    /// <summary>
    /// 得到人员的详细信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Department GetDepartment(int id)
    {
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            Department depart = serviceClient.GetDepartment(id);
            return depart;
        }
    }
    public static List<Personnel> GetPersonnels(Department topoRoot)
    {
        List<Personnel> personnelsT = new List<Personnel>();
        if (topoRoot == null) return personnelsT;
        if (topoRoot.Children == null) return personnelsT;
        foreach (Department child in topoRoot.Children)
        {
            if (child.LeafNodes != null)
            {
                foreach (Personnel p in child.LeafNodes)
                {
                    p.Parent = child;
                }
                personnelsT.AddRange(child.LeafNodes);
            }
            personnelsT.AddRange(GetPersonnels(child));
        }
        return personnelsT;
    }

    /// <summary>
    /// 获取定位卡位置信息
    /// </summary>
    /// <returns></returns>
    public List<TagPosition> GetRealPositons()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            TagPosition[] arr = serviceClient.GetRealPositons();
            List<TagPosition> list = new List<TagPosition>();
            if (arr != null)
            {
                list.AddRange(arr);
            }
            return list;
        }
    }


    /// <summary>
    /// 获取取定位卡历史位置信息
    /// </summary>
    /// <returns></returns>
    public List<Position> GetHistoryPositons()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            Position[] arr = serviceClient.GetHistoryPositons();
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            Position[] arr = serviceClient.GetHistoryPositonsByTime(tagcode, start, end);
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            Position[] arr = serviceClient.GetHistoryPositonsByPersonnelID(personnelID, start, end);
            List<Position> list = new List<Position>();
            if (arr != null)
            {
                Log.Info("CommunicationObject,GetHistoryPositonsByPersonnelID", string.Format("{0},{1},{2},count={3}", personnelID, start, end, list.Count));
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            Position[] arr = serviceClient.GetHistoryPositonsByPidAndTopoNodeIds(personnelID, topoNodeIdsT.ToArray(), start, end);
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
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
            serviceClient = GetServiceClient();
            if (serviceClient == null) return false;
            lock (serviceClient)//1//不用线程使用同一个client发送消息会相互干扰
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
    }

    /// <summary>
    /// 3D保存历史数据
    /// </summary>
    public bool AddU3DPosition(List<U3DPosition> pList)
    {
        try
        {
            serviceClient = GetServiceClient();
            if (serviceClient == null) return false;
            lock (serviceClient)//1//不用线程使用同一个client发送消息会相互干扰
            {
                //Debug.Log(string.Format("---------》》》》》》》{0}---------", position.Tag));
                //Debug.Log(string.Format("[AddU3DPosition] Tag:{0}", position.Tag));
                serviceClient.AddU3DPositions(pList.ToArray());
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            U3DPosition[] arr = serviceClient.GetHistoryU3DPositonsByTime(tagcode, start, end);
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            ObjectAddList_Type[] TypeList = serviceClient.GetObjectAddList();
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            bool value = serviceClient.DeleteDevInfo(dev);
            Debug.Log("DeleteDevInfo result:" + value);
        }
    }
    /// <summary>
    /// 保存设备信息
    /// </summary>
    public void AddDevInfo(ref DevInfo dev)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            //DevInfo value = client.AddDevInfo(dev);
            dev = serviceClient.AddDevInfo(dev);
        }
    }
    /// <summary>
    /// 添加设备
    /// </summary>
    /// <param name="devInfos"></param>
    public List<DevInfo> AddDevInfo(List<DevInfo> devInfos)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            DevInfo[] devs = devInfos.ToArray();
            devInfos = serviceClient.AddDevInfoByList(devs).ToList();
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            bool value = serviceClient.ModifyDevInfo(Info);
            Debug.Log("ModifyDevInfo result:" + value);
        }
    }
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="PosInfo"></param>
    public void ModifyDevPos(DevPos PosInfo)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            bool value = serviceClient.ModifyPosInfo(PosInfo);
            //Debug.Log("ModifyDevPos result:" + value);
        }
    }
    /// <summary>
    /// 修改设备位置信息
    /// </summary>
    /// <param name="posList"></param>
    public void ModifyDevPosByList(List<DevPos> posList)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            bool value = serviceClient.ModifyPosByList(posList.ToArray());
            Debug.Log("ModifyDevPos result:" + value);
        }
    }
    /// <summary>
    /// 添加设备位置信息
    /// </summary>
    /// <param name="devPos"></param>
    public void AddDevPosInfo(DevPos devPos)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            bool value = serviceClient.AddDevPosInfo(devPos);
            Debug.Log("AddDevPos result:" + value);
        }
    }
    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    /// <returns></returns>
    public List<DevInfo> GetAllDevInfos()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            DevInfo[] infoList = serviceClient.GetAllDevInfos();
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            DevInfo[] infoList = serviceClient.GetDevInfos(typeList);
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            DevInfo devInfo = serviceClient.GetDevByGUID(devId);//通过设备Id,获取设备(字符串Id,GUID那部分)
            return devInfo;
        }
    }

    /// <summary>
    /// 通过设备Id,获取设备信息
    /// </summary>
    /// <param name="devId"></param>
    public DevInfo GetDevByGameObjecName(string objName)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            DevInfo devInfo = serviceClient.GetDevByGameName(objName);//通过设备Id,获取设备(字符串Id,GUID那部分)
            return devInfo;
        }
    }

    /// <summary>
    /// 通过设备Id,获取设备信息
    /// </summary>
    /// <param name="devId"></param>
    public DevInfo GetDevByid(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            DevInfo devInfo = serviceClient.GetDevById(id);//通过设备Id,获取设备(数字Id,主键)
            return devInfo;
        }
    }

    /// <summary>
    /// 获取所有设备信息
    /// </summary>
    /// <returns></returns>
    public List<DevInfo> FindDevInfos(string key)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            DevInfo[] infoList = serviceClient.FindDevInfos(key);
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            DevPos[] infoList = serviceClient.GetDevPositions();
            if (infoList == null) return null;
            return infoList.ToList();
        }
    }
    public List<DevInfo> GetDevInfoByParent(List<int> idList)
    {
        try
        {
            serviceClient = GetServiceClient();
            if (serviceClient == null) return null; lock (serviceClient)//1
            {
                DevInfo[] infoList = serviceClient.GetDevInfoByParent(idList.ToArray());
                if (infoList == null)
                {
                    return new List<DevInfo>();
                }
                return infoList.ToList();
            }
        }
        catch (Exception e)
        {
            Log.Error("CommunicationObject.GetDevInfoByParent:" + e.ToString());
            return null;
        }
    }

    /// <summary>
    /// 获取园区下的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetParkMonitorRange()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            IList<PhysicalTopology> results = serviceClient.GetParkMonitorRange();
            return results;
        }
    }

    /// <summary>
    /// 获取楼层下的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetFloorMonitorRange()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            IList<PhysicalTopology> results = serviceClient.GetFloorMonitorRange();
            return results;
        }
    }

    /// <summary>
    /// 根据PhysicalTopology的Id获取楼层以下级别的监控范围
    /// </summary>
    /// <returns></returns>
    public IList<PhysicalTopology> GetFloorMonitorRangeById(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            IList<PhysicalTopology> results = serviceClient.GetFloorMonitorRangeById(id);
            return results;
        }
    }

    /// <summary>
    /// 根据节点添加监控范围
    /// </summary>
    public bool EditMonitorRange(PhysicalTopology pt)
    {

        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.EditMonitorRange(pt);
        }
    }

    /// <summary>
    /// 根据节点添加区域范围
    /// </summary>
    public PhysicalTopology AddMonitorRange(PhysicalTopology pt)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.AddMonitorRange(pt);
        }
    }

    /// <summary>
    /// 根据节点删除监控范围
    /// </summary>
    public bool DeleteMonitorRange(PhysicalTopology pt)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeleteMonitorRange(pt);
        }
    }

    /// <summary>
    /// 获取配置信息
    /// </summary>
    /// <returns></returns>
    public ConfigArg GetConfigArgByKey(string key)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            return serviceClient.GetConfigArgByKey(key);
        }
    }

    /// <summary>
    /// 设置配置信息
    /// </summary>
    /// <returns></returns>
    public bool GetConfigArgByKey(ConfigArg config)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.EditConfigArg(config);
        }
    }

    /// <summary>
    /// 获取坐标系转换配置信息
    /// </summary>
    /// <returns></returns>
    public TransferOfAxesConfig GetTransferOfAxesConfig()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            return serviceClient.GetTransferOfAxesConfig();
        }
    }

    /// <summary>
    /// 设置坐标系转换配置信息
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public bool SetTransferOfAxesConfig(TransferOfAxesConfig config)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.SetTransferOfAxesConfig(config);
        }
    }

    /// <summary>
    /// 人员搜索（CaiCai）
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<Personnel> FindPersonnelData(string key)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            Personnel[] infoList = serviceClient.FindPersonList(key);
            if (infoList == null) return new List<Personnel>();
            return infoList.ToList();
        }
    }

    /// <summary>
    /// 人员历史轨迹基准
    /// </summary>
    /// <param name="nFlag"></param>
    /// <param name="strName"></param>
    /// <param name="strName2"></param>
    /// <returns></returns>
    public List<PositionList> GetHistoryPositonStatistics(int nFlag, string strName, string strName2, string strName3)
    {
        Log.Info("GetHistoryPositonStatistics :", string.Format("flag:{0},name1:{1},name2:{2}", nFlag, strName, strName2));
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            PositionList[] infoList = serviceClient.GetHistoryPositonStatistics(nFlag, strName, strName2, strName3);
            if (infoList == null) return new List<PositionList>();
            return infoList.ToList();
        }
    }

    /// <summary>
    /// 人员历史轨迹基准
    /// </summary>
    /// <param name="nFlag"></param>
    /// <param name="strName"></param>
    /// <param name="strName2"></param>
    /// <returns></returns>
    public void GetHistoryPositonStatistics(int nFlag, string strName, string strName2, string strName3,Action<List<PositionList>> callback)
    {
        Log.Info("GetHistoryPositonStatistics Async:", string.Format("flag:{0},name1:{1},name2:{2}",nFlag,strName,strName2));
        serviceClient = GetServiceClient();
        ThreadManager.Run(() =>
        {
            DateTime start = DateTime.Now;
            List<PositionList> result = null;
            if (serviceClient != null)
            {
                lock (serviceClient)//1
                {
                    var list = serviceClient.GetHistoryPositonStatistics(nFlag, strName, strName2,strName3);
                    if (list != null)
                        result = list.ToList();
                    else
                    {
                        Log.Error("GetHistoryPositonStatistics", "list==null");
                    }
                }
            }
            Log.Info("GetHistoryPositonStatistics:",(DateTime.Now - start).TotalMilliseconds + "ms");
            return result;
        }, callback, "GetHistoryPositonStatistics");
    }

    public void GetHistoryPositonData(int nFlag, string strName, string strName2, string strName3, Action<List<Pos>> callback)
    {
        Log.Info("GetHistoryPositonData Async:", string.Format("flag:{0},name1:{1},name2:{2}", nFlag, strName, strName2));
        serviceClient = GetServiceClient();
        ThreadManager.Run(() =>
        {
            DateTime start = DateTime.Now;
            List<Pos> result = null;
            if (serviceClient != null)
            {
                lock (serviceClient)//1
                {
                    var infoList = serviceClient.GetHistoryPositonData(nFlag, strName, strName2, strName3);
                    if (infoList != null)
                        result = infoList.ToList();
                }
            }
            Log.Info("GetHistoryPositonData:", (DateTime.Now - start).TotalMilliseconds + "ms");
            return result;
        }, callback, "GetHistoryPositonData");
    }

    public void GetSwitchAreas(Action<PhysicalTopology[]> callback)
    {
        Log.Info("GetSwitchAreas Async:");
        serviceClient = GetServiceClient();
        ThreadManager.Run(() =>
        {
            DateTime start = DateTime.Now;
            PhysicalTopology[] result = null;
            if (serviceClient != null)
            {
                lock (serviceClient)//1
                {
                    result=serviceClient.GetSwitchAreas();
                }
            }
            Log.Info("GetSwitchAreas:", (DateTime.Now - start).TotalMilliseconds + "ms");
            return result;
        }, callback, "GetSwitchAreas");
    }


    public Post[] GetPostList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            return serviceClient.GetPostList();
        }
    }
    /// <summary>
    /// 获取单个信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CameraAlarmInfo GetCameraAlarm(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetCameraAlarm(id);
        }
    }
    /// <summary>
    /// 宝信摄像头告警数据
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public List<CameraAlarmInfo> GetCameraAlarms(string ip, bool merge)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            CameraAlarmInfo[] list= serviceClient.GetCameraAlarms(ip, merge);
            return list.ToList();
        }
    }
    public List<CameraAlarmInfo> GetAllCameraAlarms(bool merge)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            CameraAlarmInfo[] list = serviceClient.GetAllCameraAlarms(merge);
            return list==null?null:list.ToList();
        }
    }
    public LocationAlarm[] GetLocationAlarms(AlarmSearchArg arg)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            var lst = serviceClient.GetLocationAlarms(arg);
            return ToNotNullList(lst);
        }
    }
    /// <summary>
    /// 设备告警统计
    /// </summary>
    /// <returns></returns>
  
    public AlarmStatistics GetDevAlarmStatistics(SearchArg arg)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            AlarmStatistics devAlarm = serviceClient.GetDevAlarmStatistics(arg);
            return devAlarm;
        }
    }
    /// <summary>
    /// 人员告警统计
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public AlarmStatistics GetLocationAlarmStatistics(SearchArg arg)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            AlarmStatistics perAlarm = serviceClient.GetLocationAlarmStatistics(arg);
            return perAlarm;
        }
    }
    public DeviceAlarmInformation GetDeviceAlarms(AlarmSearchArg arg)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            DeviceAlarmInformation devAlarm= serviceClient.GetDeviceAlarms(arg);
            return devAlarm;
        }
    }

    /// <summary>
    /// 附近人员
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public NearbyPerson[] GetNearbyPerson_Currency(int Id, float distance)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            var lst = serviceClient.GetNearbyPerson_Currency(Id, distance);
            return ToNotNullList(lst);
        }
    }

    private T[] ToNotNullList<T>(T[] array)//unity的wcf不能传空的数组，但是null又容易出错，这里转换一下
    {
        //if (array == null)
        //{
        //    array = new T[0];//但是发现空数组传出去也可能有问题
        //}
        return array;
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            var lst = serviceClient.GetNearbyDev_Currency(id, distance, nFlag);
            return ToNotNullList(lst);
        }
    }
    /// <summary>
    /// 人员经过门禁
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EntranceGuardActionInfo[] GetEntranceActionInfoByPerson24Hours(int id)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {

            var lst = serviceClient.GetEntranceActionInfoByPerson24Hours(id);
            return ToNotNullList(lst);
        }
    }
    /// <summary>
    /// 增加门禁设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool AddDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.AddDoorAccessByList(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 添加门禁信息
    /// </summary>
    /// <param name="doorAccess"></param>
    /// <returns></returns>
    public Dev_DoorAccess AddDoorAccess(Dev_DoorAccess doorAccess)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.AddDoorAccess(doorAccess);
        }
    }
    /// <summary>
    /// 删除门禁设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool DeleteDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeleteDoorAccess(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 修改门禁信息
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool ModifyDoorAccess(List<Dev_DoorAccess> doorAccessList)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.ModifyDoorAccess(doorAccessList.ToArray());
        }
    }
    /// <summary>
    /// 通过区域ID，获取区域下门禁信息
    /// </summary>
    /// <param name="pidList"></param>
    /// <returns></returns>
    public List<Dev_DoorAccess> GetDoorAccessInfoByParent(List<int> pidList)
    {
        try
        {
            serviceClient = GetServiceClient();
            if (serviceClient == null) return null; lock (serviceClient)//1
            {
                List<Dev_DoorAccess> doorList;
                Dev_DoorAccess[] doors = serviceClient.GetDoorAccessInfoByParent(pidList.ToArray());
                if (doors == null) doorList = new List<Dev_DoorAccess>();
                else doorList = doors.ToList();
                return doorList;
            }
        }
        catch (Exception e)
        {
            Log.Error("CommunicationObject.GetDoorAccessInfoByParent:" + e.ToString());
            return null;
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.AddCameraInfoByList(cameraInfoList.ToArray());
        }
    }
    /// <summary>
    /// 增加摄像头设备
    /// </summary>
    /// <param name="cameraInfo"></param>
    /// <returns></returns>
    public Dev_CameraInfo AddCameraInfo(Dev_CameraInfo cameraInfo)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.AddCameraInfo(cameraInfo);
        }
    }
    /// <summary>
    /// 删除摄像头设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public bool DeleteCameraInfo(List<Dev_CameraInfo> cameraInfoList)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeleteCameraInfo(cameraInfoList.ToArray());
        }
    }
    /// <summary>
    /// 修改摄像头设备
    /// </summary>
    /// <param name="doorAccessList"></param>
    /// <returns></returns>
    public Dev_CameraInfo ModifyCameraInfo(Dev_CameraInfo cameraInfo)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null ;
        lock (serviceClient)//1
        {
            return serviceClient.ModifyCameraInfo(cameraInfo);
        }
    }
    /// <summary>
    /// 通过区域ID，获取区域下摄像头设备
    /// </summary>
    /// <param name="pidList"></param>
    /// <returns></returns>
    public List<Dev_CameraInfo> GetCameraInfoByParent(List<int> pidList)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            List<Dev_CameraInfo> cameraInfoList;
            Dev_CameraInfo[] cameraInfos = serviceClient.GetCameraInfoByParent(pidList.ToArray());
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null; lock (serviceClient)//1
        {
            List<Dev_CameraInfo> cameraInfoList;
            Dev_CameraInfo[] cameraInfos = serviceClient.GetAllCameraInfo();
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            Dev_CameraInfo cameraInfo = serviceClient.GetCameraInfoByDevInfo(dev);
            return cameraInfo;
        }
    }
 
    /// <summary>
    /// 通过设备Ip，获取对应摄像头信息
    /// </summary>
    /// <param name="dev"></param>
    /// <returns></returns>
    public Dev_CameraInfo GetCameraInfoByIp(string devIp)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            Dev_CameraInfo cameraInfo = serviceClient.GetCameraInfoByIp(devIp);
            return cameraInfo;
        }
    }
    /// <summary>
    /// 通过rtsp地址，获取对应摄像头信息
    /// </summary>
    /// <param name="dev"></param>
    /// <returns></returns>
    public int? GetCameraDevIdByURL(string rtstUrl)
    {
        Dev_CameraInfo info = GetCameraInfoByURL(rtstUrl);
        if (info != null)
        {
            return info.DevInfoId;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 通过rtsp地址，获取摄像头数据
    /// </summary>
    /// <param name="rtstUrl"></param>
    /// <param name="getDevInfo">是否增加设备信息</param>
    /// <returns></returns>
    public Dev_CameraInfo GetCameraInfoByURL(string rtstUrl,bool getDevInfo=false)
    {
        //"rtsp://admin:admin@ 192.168.1.27:554/ch1/main/h264",
        if (string.IsNullOrEmpty(rtstUrl)) return null;
        string[] ips = rtstUrl.Split('@');
        if (ips == null || ips.Length < 2) return null;
        //有两种情况：1. ：后接端口号   2.  /后接端口号
        string[] ipTemp;
        if (ips[1].Contains(":"))
        {
            ipTemp = ips[1].Split(':');
            if (ipTemp == null || ipTemp.Length < 2) return null;
        }
        else
        {
            ipTemp = ips[1].Split('/');
            if (ipTemp == null || ipTemp.Length < 2) return null;
        }
        string ipFinal = ipTemp[0];
        if (string.IsNullOrEmpty(ipFinal)) return null;
        Dev_CameraInfo info = GetCameraInfoByIp(ipFinal);
        if(info!=null)
        {
            info.DevInfo = GetDevByid(info.DevInfoId);
        }
        return info;
    }
    #endregion
    #region 基站编辑
    /// <summary>
    /// 获取所有基站信息
    /// </summary>
    /// <returns></returns>
    public List<Archor> GetArchors()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {

            List<Archor> archors = serviceClient.GetArchors().ToList();
            return archors;
        }
    }

    public Archor GetArchor(string archorId)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {

            Archor archor = serviceClient.GetArchor(archorId);
            return archor;
        }
    }

    public Archor GetArchorByDevId(int devId)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {

            Archor archor = serviceClient.GetArchorByDevId(devId);
            return archor;
        }
    }
    public bool AddArchor(Archor archor)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.AddArchor(archor);
        }
    }
    public bool EditArchor(Archor archor, int parentId)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            bool value = serviceClient.EditArchor(archor, parentId);
            return value;
        }
    }
    public bool DeleteArchor(int devId)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            return serviceClient.DeleteArchor(devId);
        }
    }
    public bool EditBusAnchor(Archor busArchor, int parentId)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            bool value = serviceClient.EditArchor(busArchor, parentId);
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetOperationTicketList().ToList();
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetWorkTicketList().ToList();
        }
    }

    /// <summary>
    /// 获取巡检设备列表
    /// </summary>
    public List<MobileInspectionDev> GetMobileInspectionDevList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetMobileInspectionDevList().ToList();
        };
    }

    /// <summary>
    /// 获取巡检轨迹列表
    /// </summary>
    /// <returns></returns>
    public List<MobileInspection> GetMobileInspectionList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetMobileInspectionList().ToList();
        };
    }

    public InspectionTrack GetInspectionTrackById(InspectionTrack track)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            return serviceClient.GetInspectionTrackById(track);
        }
    }
    /// <summary>
    /// 获取巡检轨迹列表(新)
    /// </summary>
    /// <returns></returns>
    public List<InspectionTrack> GetInspectionTrackList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            DateTime dt = DateTime.Now;
            DateTime dt2 = DateTime.Now;
			return serviceClient.Getinspectionlist(dt, dt2, false).ToList();
        };
    }

    /// <summary>
    /// 获取人员巡检轨迹列表
    /// </summary>
    /// <returns></returns>
    public List<PersonnelMobileInspection> GetPersonnelMobileInspectionList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            PersonnelMobileInspection[] ps = serviceClient.GetPersonnelMobileInspectionList();
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetOperationTicketHistoryList().ToList();
        };
    }

    /// <summary>
    /// 获取工作票历史记录
    /// </summary>
    /// <returns></returns>
    public List<WorkTicketHistory> GetWorkTicketHistoryList()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            return serviceClient.GetWorkTicketHistoryList().ToList();
        };
    }

    public List<InspectionTrackHistory> Getinspectionhistorylist(DateTime dtBeginTime, DateTime dtEndTime, bool bFlag)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            InspectionTrackHistory[] ps = serviceClient.Getinspectionhistorylist(dtBeginTime, dtEndTime, bFlag);
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            PersonnelMobileInspectionHistory[] ps = serviceClient.GetPersonnelMobileInspectionHistoryList();
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return false;
        lock (serviceClient)//1
        {
            bool value = serviceClient.EditPictureInfo(picture);
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
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            Picture value = serviceClient.GetPictureInfo(pictureName);
            return value;
        }
    }
    #endregion
    #region 设备KKS监控

    /// <summary>
    /// 获取设备监控信息
    /// </summary>
    /// <param name="kksCode"></param>
    /// <returns></returns>
    public Dev_Monitor GetDevMonitor(string kksCode)
    {
        Dev_Monitor monitorInfo = GetMonitorInfoByKKS(kksCode, false);
        return monitorInfo;
    }

    /// <summary>
    /// 获取设备监控信息
    /// </summary>
    /// <param name="kksCode"></param>
    /// <returns></returns>
    public void GetDevMonitor(string kksCode, Action<Dev_Monitor> callback)
    {
        GetMonitorInfoByKKS(kksCode, false,callback);
    }


    /// <summary>
    /// 通过KKS获取监控数据
    /// </summary>
    /// <param name="kksCode"></param>
    /// <param name="displayAllNode">是否显示所有节点（包括没数据的节点）</param>
    /// <returns></returns>
    public Dev_Monitor GetMonitorInfoByKKS(string kksCode, bool displayAllNode)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)//1
        {
            Dev_Monitor value = serviceClient.GetDevMonitorInfoByKKS(kksCode, displayAllNode);
            return value;
        }
    }

    /// <summary>
    /// 通过KKS获取监控数据
    /// </summary>
    /// <param name="kksCode"></param>
    /// <param name="displayAllNode">是否显示所有节点（包括没数据的节点）</param>
    /// <returns></returns>
    public void GetMonitorInfoByKKS(string kksCode, bool displayAllNode,Action<Dev_Monitor> callback)
    {
        Dev_Monitor result = null;
        serviceClient = GetServiceClient();
        if (serviceClient != null)
        {
            lock (serviceClient)//1
            {
                result = serviceClient.GetDevMonitorInfoByKKS(kksCode, displayAllNode);
            }
        }
        if (callback != null)
        {
            callback(result);
        }
    }

    #endregion

    public void GetUnitySetting()
    {
        Debug.Log("->GetUnitySetting");
        serviceClient = GetServiceClient();
        if (serviceClient == null) return;
        lock (serviceClient)//1
        {
            var setting = serviceClient.GetUnitySetting();//从服务端获取配置信息
            if (setting.IsUseService)//用服务端的配置覆盖本地客户端的配置，默认情况下是false,特殊情况，可以通过服务端统一修改客户端配置参数。
            {
                var refreshSetting = setting.RefreshSetting;
                SetRefreshSetting(refreshSetting);
            }
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
        RefreshSetting.TagPos = refreshSetting.TagPos;
        RefreshSetting.PersonTree = refreshSetting.PersonTree;
        RefreshSetting.AreaStatistics = refreshSetting.AreaStatistics;
        RefreshSetting.NearCamera = refreshSetting.NearCamera;
        RefreshSetting.ScreenShot = refreshSetting.ScreenShot;
        RefreshSetting.DepartmentTree = refreshSetting.DepartmentTree;
    }

    private void SetRefreshSetting(Location.WCFServiceReferences.LocationServices.RefreshSetting refreshSetting)
    {
        if (refreshSetting == null)
        {
            Debug.LogError("refreshSetting==null"); return;
        }
        RefreshSetting.TagPos = refreshSetting.TagPos;
        RefreshSetting.PersonTree = refreshSetting.PersonTree;
        RefreshSetting.AreaStatistics = refreshSetting.AreaStatistics;
        RefreshSetting.NearCamera = refreshSetting.NearCamera;
        RefreshSetting.ScreenShot = refreshSetting.ScreenShot;
        RefreshSetting.DepartmentTree = refreshSetting.DepartmentTree;
    }

    public void DebugLog(string msg)
    {
        clientSync.DebugLog(msg);
    }

    public void GetTag(int id, Action<Tag> callback)
    {
        var client = GetClient();
        client.GetTag(id, callback);
    }
    /// <summary>
    /// 厂区首页图片名称
    /// </summary>
    /// <returns></returns>
    public List<string> HomePagePicture()
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            string[] picture = serviceClient.GetHomePageNameList();
            if (picture == null)
            {
                return new List<string>();
            }
            return picture.ToList();
        }
    }
    /// <summary>
    /// 厂区首页图片信息
    /// </summary>
    /// <param name="strPictureName"></param>
    /// <returns></returns>
    public byte[] HomePagePictureInfo(string strPictureName)
    {
        serviceClient = GetServiceClient();
        if (serviceClient == null) return null;
        lock (serviceClient)
        {
            return serviceClient.GetHomePageByName(strPictureName);
        }

    }
}

//public enum CommunicationMode
//{
//    None, Sync, Async, Thread, WebApi, Editor
//}
