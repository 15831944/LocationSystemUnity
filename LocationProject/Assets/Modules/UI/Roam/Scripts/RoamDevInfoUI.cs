using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoamDevInfoUI : MonoBehaviour {

    public static RoamDevInfoUI Instance;

    public GameObject Window;
    /// <summary>
    /// 设备标题
    /// </summary>
    public Text DevNameText;
    /// <summary>
    /// 温度列表
    /// </summary>
    public List<DeviceTemperatureItem> ItemList;

    public RoamRaycastCheck RoamDevRaycastCheck;//漫游射线检测
	// Use this for initialization
	void Start () {
        Instance = this;
    }
    /// <summary>
    /// 是否打开设备射线检测（以前鼠标固定中间，直接用OnMouseEnter）
    /// </summary>
    /// <param name="isOn"></param>
    public void SetDevInfoCheckState(bool isOn)
    {
        if(RoamDevRaycastCheck!=null) RoamDevRaycastCheck.gameObject.SetActive(isOn);
        if(!isOn)
        {
            RoamDevRaycastCheck.Clear();
        }
    }

    /// <summary>
    /// 显示设备信息
    /// </summary>
    /// <param name="devInfo"></param>
    public void ShowDevInfo(DevInfo devInfo)
    {
        Window.SetActive(true);
        if (devInfo == null||string.IsNullOrEmpty(devInfo.Name)) DevNameText.text = "设备";
        else DevNameText.text = devInfo.Name;
        for(int i=0;i<ItemList.Count;i++)
        {
            float randomValue = Random.Range(25.8f,40);
            string subDevName = string.Format("子部件{0}温度",i+1);
            ItemList[i].InitItem(randomValue,50, subDevName);
        }
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Close()
    {
        Window.SetActive(false);
    }
}
