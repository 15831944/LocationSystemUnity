using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceSearchTween : MonoBehaviour {
    public static DeviceSearchTween instance;
    /// <summary>
    /// 恢复原来大小的按钮
    /// </summary>
    public Button RecoverBut;
    /// <summary>
    /// 关闭小窗口按钮
    /// </summary>
    public Button CloseBUt;
    /// <summary>
    /// 小窗口界面
    /// </summary>
    public GameObject MinWindow;

    void Start () {
        instance = this;
        RecoverBut.onClick.AddListener(RecoverBut_Click);
        CloseBUt.onClick.AddListener(CloceBut_Click);

    }
    /// <summary>
    /// 恢复设备搜索界面
    /// </summary>
	public void RecoverBut_Click()
    {
        DeviceDataPaging.Instance.ShowdevSearchWindow();
    }
    /// <summary>
    /// 关闭设备搜索界面和小窗口
    /// </summary>
    public void CloceBut_Click()
    {
        DeviceDataPaging.Instance.ClosedevSearchWindow();
        
    }
    /// <summary>
    /// 是否打开小窗口
    /// </summary>
    /// <param name="ison"></param>
    public void ShowMinWindow(bool ison)
    {
        if (ison)
        {
            MinWindow.SetActive(true);
        }
        else
        {
            MinWindow.SetActive(false);
           
        }
    }

    void Update () {
		
	}
}
