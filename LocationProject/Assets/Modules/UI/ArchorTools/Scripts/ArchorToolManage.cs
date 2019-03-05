using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchorToolManage : MonoBehaviour {
    public static ArchorToolManage Instance;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button closeButton;
    public GameObject window;
    /// <summary>
    /// 定位设备位置修改
    /// </summary>
    public ArchorLocation ArchroSetPart;
    /// <summary>
    /// 基准点修改
    /// </summary>
    public ArchorSignSet SignSetPart;
	// Use this for initialization
	void Start () {
        Instance = this;
        closeButton.onClick.AddListener(Close);
	}
	
    public void Show(DevNode dev)
    {
        //ArchroSetPart.Open(dev);
        //window.SetActive(true);
        
    }
    public void Close()
    {
        //if (!window.activeInHierarchy) return;
        //ArchroSetPart.Close();
        //window.SetActive(false);
    }
}
