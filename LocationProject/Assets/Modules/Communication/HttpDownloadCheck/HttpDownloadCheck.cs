using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpDownloadCheck : MonoBehaviour {
    public static HttpDownloadCheck Instance;
    public bool EnableCheck;
    // Use this for initialization
    void Awake () {
        Instance = this;
        
    }
	void Start()
    {
        Application.logMessageReceived += Handler;
    }

    void Handler(string logString, string stackTrace, LogType type)
    {
        if (!EnableCheck) return;//默认不启用提示
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            string info = logString.ToLower();
            if(info.Contains("unauthorizedaccessexception"))
            {
                ShowMsg("下载失败：权限不足\n请右键以管理员身份运行程序，或将程序安装在D盘(非系统盘)");
            }else if(info.Contains("socketexception"))
            {
                ShowMsg("下载失败：连接超时\n可能原因：1.下载地址错误 2.网线松动，网络异常");
            }
        }
    }

    public void ShowMsg(string txt)
    {
        if(UGUIMessageBox.Instance)
        {
            UGUIMessageBox.Instance.ShowMessage(txt);
        }
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= Handler;
    }
}
