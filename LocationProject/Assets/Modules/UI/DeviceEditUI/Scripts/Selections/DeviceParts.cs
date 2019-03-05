using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceParts : MonoBehaviour {
    public GameObject Window;

    public DeviceEdit SingleEditPart;//单设备编辑界面
    public DeviceSelections MultiDevEditPart;//多设备编辑界面
    public GameObject NoDevicePart;//没有选中设备界面
	// Use this for initialization
	void Start () {
		
	}
	/// <summary>
    /// 显示设备编辑界面
    /// </summary>
    public void SetDeviceInfo(DevNode dev)
    {
        Window.SetActive(true);
        SingleEditPart.SetDeviceInfo(dev);
        MultiDevEditPart.Close();
        NoDevicePart.gameObject.SetActive(false);
    }
    /// <summary>
    /// 没有选中任何设备界面
    /// </summary>
    public void SetEmptyInfo()
    {
        Window.SetActive(true);
        SingleEditPart.Close();
        MultiDevEditPart.Close();
        NoDevicePart.gameObject.SetActive(true);
    }
    /// <summary>
    /// 显示设备编辑界面
    /// </summary>
    /// <param name="devs"></param>
    public void SetDeviceInfo(List<DevNode>devs)
    {
        Window.SetActive(true);
        SingleEditPart.Close();
        MultiDevEditPart.SetDevInfo(devs);
        NoDevicePart.gameObject.SetActive(false);
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Close()
    {
        Window.SetActive(false);
        MultiDevEditPart.Close();
        SingleEditPart.Close();
        NoDevicePart.SetActive(false);
    }
}
