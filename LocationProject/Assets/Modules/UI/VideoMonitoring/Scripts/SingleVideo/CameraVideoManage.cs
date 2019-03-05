using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraVideoManage : MonoBehaviour
{
    public static CameraVideoManage Instance;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button CloseButton;
    /// <summary>
    /// 界面窗体
    /// </summary>
    public GameObject Window;
	// Use this for initialization
	void Start ()
	{
	    Instance = this;
	    CloseButton.onClick.AddListener(Close);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowVideo(string kksCode)
    {
        //Todo:根据KKS码，去对接并显示视频数据
        //if (string.IsNullOrEmpty(kksCode)) return;
        Debug.Log("ShowVideo...");
        Show();
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Close()
    {
        Window.SetActive(false);
    }
    /// <summary>
    /// 显示界面
    /// </summary>
    public void Show()
    {
        Window.SetActive(true);
    }
}
