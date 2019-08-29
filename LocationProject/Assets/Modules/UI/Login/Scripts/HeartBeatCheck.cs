using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatCheck : MonoBehaviour {

    private CommunicationObject service;
    private string connectInfo = "connect";
    private string errorMsg = "与服务端连接断开...";
    private bool IsConnect=true;
	// Use this for initialization
	void Start () {
        StartHeartBeatCheck();

    }

    public void StartHeartBeatCheck()
    {
        if(!IsInvoking("HeartCheck"))
        {
            InvokeRepeating("HeartCheck", 0, 1);
        }
    }
	/// <summary>
    /// 进行心跳检测
    /// </summary>
    private void HeartCheck()
    {
        if (service == null) service = CommunicationObject.Instance;
        if (service == null) return;
        service.HeartBeat(connectInfo,OnConnect,OnDisConnect);
    }
    /// <summary>
    /// 连接成功
    /// </summary>
    /// <param name="info"></param>
    private void OnConnect(string info)
    {
        CommunicationObject.IsConnect = true;
        if(!IsConnect)
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
            if (GlobalTipsManage.Instance) GlobalTipsManage.Instance.Show(errorMsg,true);
            SceneEvents.OnConnectStateChange(SceneEvents.ServerConnectState.disConnect);
        }
    }
}
