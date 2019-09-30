using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DeviceAlarm的告警信息展示(边界，后来又有门禁）
/// </summary>
public class DeviceAlarmFollowUI : MonoBehaviour {
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
        InitToggle();
    }

    protected void InitToggle()
    {
        if (devToggle == null)
        {
            Log.Error("DeviceAlarmFollowUI.InitToggle", "devToggle == null");
        }
        else
        {
            devToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    protected List<DeviceAlarm> alarmInfos = new List<DeviceAlarm>();

    /// <summary>
    /// 初始化信息
    /// </summary>
    /// <param name="alarmInfo"></param>
    public void InitInfo(DeviceAlarm alarmInfo)
    {
        AddAlarmInfo(alarmInfo);
        SetText();
    }
    /// <summary>
    /// 添加告警信息
    /// </summary>
    /// <param name="alarmInfo"></param>
    private void AddAlarmInfo(DeviceAlarm alarmInfo)
    {
        //去除内容重复的告警
        DeviceAlarm alarm = alarmInfos.Find(i=>i!=null&&alarmInfo!=null&&i.Message==alarmInfo.Message);
        if (alarm != null) return;
        else alarmInfos.Add(alarmInfo);
    }

    protected virtual string GetText()
    {
        string txt = "";
        foreach (var item in alarmInfos)
        {
            txt += string.Format("{0}:{1}\n", item.Title, item.Message);
        }
        txt = txt.Trim();
        return txt;
    }

    private void SetText()
    {
        alarmInfoText.text = GetText();
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if(isOn)
        {
            SetText();
            InfoWindow.SetActive(true);
        }
        else
        {
            InfoWindow.SetActive(false);
        }
    }
}
