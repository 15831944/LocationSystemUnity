using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 登录管理脚本
/// </summary>
public class LoginManage : MonoBehaviour {
    public static LoginManage Instance;
    /// <summary>
    /// 当前用户登录相关信息
    /// </summary>
    public LoginInfo info;
    /// <summary>
    /// 登录成功后需要显示的物体
    /// </summary>
    public List<GameObject> LoginShowObjs;
    /// <summary>
    /// 是否登录成功
    /// </summary>
    public bool isLoginSucceed;
    /// <summary>
    /// 是否进行登录成功后初始化
    /// </summary>
    private bool isAfterLoginInit;

    private Action AfterLoginSuccessfullyAction;

    public CommunicationObject communicationObject;

    private string IP;
    private string Port;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        if (communicationObject == null)
        {
            communicationObject = GetComponent<CommunicationObject>();
        }
        if (communicationObject == null)
        {
            communicationObject = CommunicationObject.Instance;
        }
        Log.LogChanged += Log_LogChanged;

    }

    private void Log_LogChanged(string obj)
    {
        Debug.Log(obj);
    }

    // Update is called once per frame
    void Update () {

        AfterLoginInit();

    }

    /// <summary>
    /// 登录测试
    /// </summary>
    [ContextMenu("LoginTest")]
    public void LoginTest()
    {
        // Login(communicationObject.ip, communicationObject.port, "Admin", "Admin@123456");
        string username = "Admin";
        string password = "Admin@123456";
        LoginInfo info = new LoginInfo();
        info.UserName = username;
        info.Password = password;
        Login(info, callback, errorCallback);
    }

    private void errorCallback(string obj)
    {
        Debug.Log(obj.ToString());
    }

    private void callback(LoginInfo obj)
    {
        if (obj != null)
        {
            info = obj;
            isLoginSucceed = info.Result;
            if (info.Result)
            {
                Debug.LogFormat("登录成功！   用户:{0}，密码:{1},权限：{1}", info.UserName, info.Password,info.Authority);
            }
            else
            {
                Debug.LogFormat("登录失败！  用户:{0},密码:{1}", info.UserName,info.Password);
            }

        }
        else
        {
            isLoginSucceed = false;
            Debug.LogFormat("登陆连接失败！用户:{0}",info.UserName);
        }
        isAfterLoginInit = true;
    }

    /// <summary>
    /// 登录
    /// </summary>
    public void Login(string ipT,string portT, string user,string pass)
    {
        if (communicationObject == null)
        {
            Log.Error("Login", communicationObject == null);
            return;
        }
        info = new LoginInfo();
        info.UserName = user;
        info.Password = pass;
        IP = ipT;
        Port = portT;
        Debug.Log("Login ip:"+ ipT);
        communicationObject.Login(ipT, portT, user, pass, (result)=>{
            if (result != null)
            {
                info = result;
                isLoginSucceed = info.Result;
                if (info.Result)
                {
                    Debug.LogFormat("登录成功！  ip:{0}  用户:{1}，密码:{2},权限：{3}", ipT, portT, info.UserName, info.Authority);
                }
                else
                {
                    Debug.LogFormat("登录失败！  ip:{0}  用户:{1},密码:{2}", ipT, portT, info.UserName);
                }
                        
            }
            else
            {
                isLoginSucceed = false;
                Debug.LogFormat("登陆连接失败！  ip:{0}:{1}  用户:{2}", ipT, portT, info.UserName);
                //AfterLoginFailed();
            }
            isAfterLoginInit = true;
        });

    }
    /// <summary>
    /// 调用Api登录
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="passWord"></param>
    public void Login(string user, string pass)
    {
        Debug.Log("api登录");
        LoginInfo info = new LoginInfo();
        info.UserName = user;
        info.Password = pass;
        Login(info, callback, errorCallback);
    }
    /// <summary>
    /// webApi登录
    /// </summary>
    public void Login(LoginInfo info, Action<LoginInfo> callback, Action<string> errorCallback)
    {
        if (communicationObject == null)
        {
            Log.Error("Login", communicationObject == null);
            return;
        }
        communicationObject.Login(info, (result) =>
        {
            if (result != null)
            {
                info = result;
                isLoginSucceed = info.Result;
                if (info.Result)
                {
                    Debug.LogFormat("登录成功！   用户:{0}，密码:{1},权限：{2}",info.UserName,info.Password, info.Authority);
                }
                else
                {
                    Debug.LogFormat("登录失败！   用户:{0},密码:{1}",info.UserName,info.Password);
                }

            }
            else
            {
                isLoginSucceed = false;
                Debug.LogFormat("登陆连接失败！  用户:{0}",info.UserName);
                //AfterLoginFailed();
            }
            isAfterLoginInit = true;
        }, (error) =>
         {
             Debug.Log(error);
         });


    }

    private FileDownLoad downLoad = null;

    /// <summary>
    /// 登录之后初始化
    /// </summary>
    public void AfterLoginInit()
    {
        
        if (isAfterLoginInit)
        {
            if (isLoginSucceed)
            {
                SaveLoginInfo(IP, Port, info);
                CommunicationObject.Instance.GetUnitySetting();

                //OpenSignalR(IP, Port);
                //Invoke("AfterLoginSuccessfully", 2f);//延迟两秒可以使（正在登录...）动画更流畅一点。
                CheckVersion((value,isVersionLower, info) =>
                {
                    if (value)
                    {
                        LoginSuccess();
                    }
                    else
                    {
                        DianChangLogin.Instance.CloseLoginProcess();
                        if (info != null)
                        {
                            string msg = GetVersionMsg(isVersionLower,info.Version, SystemSettingHelper.versionSetting.VersionNumber);
                            string sureBtnName =isVersionLower? "下载安装":"继续登录";
                            string cancelBtnName = isVersionLower ? "直接登录" : "取消登录";
                            UGUIMessageBox.Show(msg, sureBtnName, cancelBtnName, () =>
                             {
                                 if(isVersionLower)
                                 {
                                     CommunicationObject.Instance.DebugLog("更新版本");
                                     if(downLoad==null)
                                     {
                                         downLoad = FileDownLoad.Instance;
                                         downLoad.ShowProgress += DownLoad_ShowProgress;
                                     } 
                                     if (downLoad)
                                     {
                                         Debug.Log("download:"+ info.LocationURL);
                                         CommunicationObject.Instance.DebugLog("download:" + info.LocationURL);
                                         downLoad.Download(info.LocationURL);
                                     }
                                     else
                                     {
                                         Debug.LogError("downLoad == null");
                                         CommunicationObject.Instance.DebugLog("downLoad == null");
                                     }
                                 }
                                 else
                                 {
                                     DianChangLogin.Instance.LoginProcess();
                                     LoginSuccess();
                                 }
                               
                             }, () =>
                             {
                                 if(isVersionLower)
                                 {
                                     DianChangLogin.Instance.LoginProcess();
                                     LoginSuccess();
                                 }                               
                             },()=> 
                             {
                                 Debug.Log("Cancel update...");
                             });
                        }
                        else
                        {
                            UGUIMessageBox.Show("版本号获取失败！");
                        }
                    }
                });
            }
            else
            {
                AfterLoginFailed();
            }
            isAfterLoginInit = false;
        }
    }

    private void DownLoad_ShowProgress(bool isActive, float value, string msg)
    {
        DownloadProgressBar progress = DownloadProgressBar.Instance;
        if (progress)
        {
            if (isActive) progress.Show(value, msg);
            else progress.Hide();
        }
    }

    /// <summary>
    /// 保存登录信息到配置文件
    /// </summary>
    public void SaveLoginInfo(string ipT, string portT, LoginInfo loginInfo)
    {
        SystemSettingHelper.communicationSetting.Ip1 = ipT;
        SystemSettingHelper.communicationSetting.Port1 = portT;
        SystemSettingHelper.SaveSystemSetting();
    }

    /// <summary>
    /// 获取版本比较信息
    /// </summary>
    /// <param name="isVersionLower"></param>
    /// <param name="sysVersion"></param>
    /// <param name="clientVersion"></param>
    /// <returns></returns>
    private string GetVersionMsg(bool isVersionLower,string sysVersion,string clientVersion)
    {
        string value = "";
        if(isVersionLower)
        {
            value = string.Format("检测到新版本:{0} 当前版本:{1} 是否下载并安装?", sysVersion, clientVersion);
        }
        else
        {
            value = string.Format("服务器版本过低，请升级服务器!\n服务器版本号：{0} 客户端版本号：{1}",sysVersion,clientVersion);
        }
        return value;
    }

    /// <summary>
    /// 登录成功
    /// </summary>
    private void LoginSuccess()
    {
        //OpenSignalR(IP, Port);
        Invoke("AfterLoginSuccessfully", 2f);//延迟两秒可以使（正在登录...）动画更流畅一点。
    }
    /// <summary>
    /// 检查版本号 
    /// </summary>
    /// <param name="onComplete"></param>
    private void CheckVersion(Action<bool,bool, VersionInfo> onComplete)
    {
        VersionInfo info = null;
        ThreadManager.Run(()=> 
        {
             info = communicationObject.GetVersionInfo();
        },()=> 
        {
            string systemVersion = "";
            if (SystemSettingHelper.versionSetting != null&&!string.IsNullOrEmpty(SystemSettingHelper.versionSetting.VersionNumber))
            {
                systemVersion = SystemSettingHelper.versionSetting.VersionNumber;
            }
            else
            {
                SystemSettingHelper.GetSystemSetting();
                if (SystemSettingHelper.versionSetting != null) systemVersion = SystemSettingHelper.versionSetting.VersionNumber;
                else Debug.LogError("SystemSettingHelper.GetSystemSetting() failed...");
            }
            if (info!=null&&info.Version.ToLower() == systemVersion)
            {
                if (onComplete != null) onComplete(true,true,info);//版本一致
            }
            else
            {
                bool isLower = IsVersionLower(info.Version,systemVersion);
                if (onComplete != null) onComplete(false,isLower,info);//版本号不一致
            }
        },"Check Version");       
    }
    /// <summary>
    /// 客户端版本是否过低（低/高）
    /// </summary>
    /// <returns></returns>
    private bool IsVersionLower(string systemVersion,string clientVersion)
    {
        if (systemVersion == null)
        {
            Debug.LogError("IsVersionLower systemVersion == null");return false;
        }
        if (clientVersion == null)
        {
            Debug.LogError("IsVersionLower clientVersion == null"); return false;
        }
        systemVersion = systemVersion.Trim();
        clientVersion = clientVersion.Trim();

        string[] sysGroup = systemVersion.Split('.');
        string[] clientGroup = clientVersion.Split('.');

        if(sysGroup.Length== clientGroup.Length)
        {
            for(int i=0;i< sysGroup.Length;i++)
            {
                int? sys = TryParseInt(sysGroup[i]);
                int? client = TryParseInt(clientGroup[i]);
                if (sys == null || client == null||sys==client) continue;
                return sys > client ? true : false;
            }
        }
        Debug.LogError("Split version error...");
        return false;
    }
    public int? TryParseInt(string item)
    {
        try
        {
            int value = int.Parse(item);
            return value;
        }catch(Exception e)
        {
            return null;
        }
    }
    /// <summary>
    /// 获取字符串中的数字
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>数字</returns>
    public static int GetNumberInt(string str)
    {
        int result = 0;
        try
        {
            if (str != null && str != string.Empty)
            {
                str = Regex.Replace(str, @"[^0-9]+", "");
                result = int.Parse(str);
            }
        }
        catch(Exception e)
        {
            result = 0;
        }       
        return result;
    }
    /// <summary>
    /// 连接SignalR服务端
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    private void OpenSignalR(string ip,string port)
    {
        CommunicationCallbackClient clientCallback = CommunicationCallbackClient.Instance;
        if(clientCallback)
        {
            clientCallback.Login(ip,port);
        }
    }
    /// <summary>
    /// 登录成功之后
    /// </summary>
    public void AfterLoginSuccessfully()
    {
        Debug.LogFormat("登录成功后初始化......");
        if (LoginShowObjs != null)
        {
            foreach (GameObject o in LoginShowObjs)
            {
                o.SetActive(true);
            }
        }

        if (AfterLoginSuccessfullyAction != null)
        {
            Debug.LogFormat("AfterLoginSuccessfullyAction");
            AfterLoginSuccessfullyAction();
        }
        DianChangLogin.Instance.CloseLogin();
    }

    /// <summary>
    /// 登录失败之后
    /// </summary>
    public void AfterLoginFailed()
    {
        Debug.LogFormat("登录失败后......");
        DianChangLogin.Instance.LoginFail();
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    [ContextMenu("Logout")]
    public void Logout()
    {
        communicationObject.LoginOut(info);
        isLoginSucceed = false;
        Debug.Log("退出登录！");
    }
    /// <summary>
    /// webApi退出登录
    /// </summary>
    /// <param name="info"></param>
    /// <param name="callback"></param>
    /// <param name="errorCallback"></param>
    public void Logout(LoginInfo info, Action<LoginInfo> callback, Action<string> errorCallback)
    {
        communicationObject.LoginOut(info,callback,errorCallback);
    }


    /// <summary>
    /// 添加登录成功之后绑定事件
    /// </summary>
    public void AddAfterLoginSuccessfullyAction(Action actionT)
    {
        AfterLoginSuccessfullyAction += actionT;
    }

    /// <summary>
    /// 移除登录成功之后绑定事件
    /// </summary>
    public void RemoveAfterLoginSuccessfullyAction(Action actionT)
    {
        AfterLoginSuccessfullyAction -= actionT;
    }
}
