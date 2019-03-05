using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VideoMonitoringManage : MonoBehaviour {
    public static VideoMonitoringManage Instance;
    /// <summary>
    /// 监控窗体
    /// </summary>
    public GameObject window;

    public Button CloseButton;
	// Use this for initialization
	void Start () {
        Instance = this;
	    CloseButton.onClick.AddListener(Close);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 显示界面
    /// </summary>
    public void Show()
    {
        window.SetActive(true);
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Close()
    {
        window.SetActive(false);
    }
    
}
