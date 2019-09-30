using Location.WCFServiceReferences.LocationServices;
using UnityEngine;
using UnityEngine.UI;

public class BorderDevFollowUI : DeviceAlarmFollowUI {
    //   /// <summary>
    //   /// 页面Toggle
    //   /// </summary>
    //   public Toggle devToggle;

    //   /// <summary>
    //   /// 信息窗体
    //   /// </summary>
    //   public GameObject InfoWindow;
    //   /// <summary>
    //   /// 告警内容
    //   /// </summary>
    //   public Text alarmInfoText;
    //// Use this for initialization
    //void Start () {
    //       devToggle.onValueChanged.AddListener(OnToggleValueChanged);
    //   }

    ///// <summary>
    ///// 初始化信息
    ///// </summary>
    ///// <param name="alarmInfo"></param>
    //public void InitInfo(DeviceAlarm alarmInfo)
    //{
    //    if(string.IsNullOrEmpty(alarmInfo.Message))
    //    {
    //        alarmInfoText.text = "设备告警！";
    //    }
    //    else
    //    {
    //        alarmInfoText.text = alarmInfo.Message;
    //    }      
    //}

    protected override string GetText()
    {
        string txt = base.GetText();
        if (string.IsNullOrEmpty(txt))
        {
            txt = "设备告警！";
        }
        return txt;
    }

    //private void OnToggleValueChanged(bool isOn)
    //{
    //    if(isOn)
    //    {
    //        InfoWindow.SetActive(true);
    //    }
    //    else
    //    {
    //        InfoWindow.SetActive(false);
    //    }
    //}
}
