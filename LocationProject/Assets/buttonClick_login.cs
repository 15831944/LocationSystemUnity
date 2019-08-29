using Location.WCFServiceReferences.LocationServices;
using UnityEngine;
using UnityEngine.UI;

public class buttonClick_login : MonoBehaviour {

    private CommunicationObject service;
    private string connectInfo = "connect";
    private string errorMsg = "与服务端连接断开...";
    private bool IsConnect = true;
    WebApiClient web;
    // Use this for initialization
    void Start () {
        web = gameObject.AddComponent<WebApiClient>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

 

    public InputField loginName;
    public InputField password;
    public buttonClick_login btn_login;

    public void LoginTest()
    {
        Debug.Log("登录");
        string name = loginName.text;
        string pw = password.text;
        LoginInfo info = new LoginInfo();
        info.UserName = name;
        info.Password = pw;
        Debug.Log(name + "," + pw);
      
        Debug.Log("创建API");
         web.Login(info,OnConnect,OnDisConnect);
        // web.LoginOut(info, OnConnect, OnDisConnect);

    }


    /// <summary>
    /// 连接成功
    /// </summary>
    /// <param name="info"></param>
    private void OnConnect(LoginInfo info)
    {
        CommunicationObject.IsConnect = true;
        if (!IsConnect)
        {
            IsConnect = true;
            if (GlobalTipsManage.Instance) GlobalTipsManage.Instance.Close();
            SceneEvents.OnConnectStateChange(SceneEvents.ServerConnectState.reConnect);
        }
    }
    /// <summary>
    /// 连接失败
    /// </summary>
    /// <param name="errorInfo"></param>
    private void OnDisConnect(string errorInfo)
    {
        CommunicationObject.IsConnect = false;
        if (IsConnect)
        {
            IsConnect = false;
            if (GlobalTipsManage.Instance) GlobalTipsManage.Instance.Show(errorMsg, true);
            SceneEvents.OnConnectStateChange(SceneEvents.ServerConnectState.disConnect);
        }
    }

}
