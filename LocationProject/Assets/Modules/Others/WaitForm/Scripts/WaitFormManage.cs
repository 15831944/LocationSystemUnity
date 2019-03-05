using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitFormManage : MonoBehaviour {
    public static WaitFormManage Instance;
    /// <summary>
    ///断线重连服务器界面
    /// </summary>
    public GameObject connectSeverWaitPanel;

	// Use this for initialization
	void Start () {
        Instance = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 显示断线重连服务器界面
    /// </summary>
    public void ShowConnectSeverWaitPanel()
    {
        connectSeverWaitPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏断线重连服务器界面
    /// </summary>
    public void HideConnectSeverWaitPanel()
    {
        connectSeverWaitPanel.SetActive(false);
    }
}
