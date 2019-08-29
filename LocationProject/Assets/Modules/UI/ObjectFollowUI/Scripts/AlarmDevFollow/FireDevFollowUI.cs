using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireDevFollowUI : DeviceAlarmFollowUI
{
    ///// <summary>
    ///// 初始化信息
    ///// </summary>
    ///// <param name="alarmInfo"></param>
    //public void InitInfo(DeviceAlarm alarmInfo)
    //{
    //    if (string.IsNullOrEmpty(alarmInfo.Message))
    //    {
    //        alarmInfoText.text = "消防告警 : 消防装置被触发";
    //    }
    //    else
    //    {
    //        alarmInfoText.text = string.Format("消防告警 : {0} ",alarmInfo.Message);
    //    }
    //}

    protected override string GetText()
    {
        string txt = base.GetText();
        if (string.IsNullOrEmpty(txt))
        {
            txt= "消防告警 : 消防装置被触发";
        }
        return txt;
    }
}
