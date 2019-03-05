using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BorderDevFollowUI : MonoBehaviour {
    /// <summary>
    /// 页面Toggle
    /// </summary>
    public Toggle devToggle;

    /// <summary>
    /// 信息窗体
    /// </summary>
    public GameObject InfoWindow;
    /// <summary>
    /// 告警内容
    /// </summary>
    public Text alarmInfoText;
	// Use this for initialization
	void Start () {
        devToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }
	
    /// <summary>
    /// 初始化信息
    /// </summary>
    /// <param name="alarmInfo"></param>
    public void InitInfo(DeviceAlarm alarmInfo)
    {
        if(string.IsNullOrEmpty(alarmInfo.Message))
        {
            alarmInfoText.text = "边界告警 : 检测到非法越界";
        }
        else
        {
            alarmInfoText.text = alarmInfo.Message;
        }      
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if(isOn)
        {
            InfoWindow.SetActive(true);
        }
        else
        {
            InfoWindow.SetActive(false);
        }
    }
}
