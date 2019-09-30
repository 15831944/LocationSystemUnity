using Assets.M_Plugins.Helpers.Utils;
using HighlightingSystem;
using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 设备告警信息脚本
/// </summary>
public class DevAlarmInfo : MonoBehaviour {

    /// <summary>
    /// 告警信息
    /// </summary>
    public DeviceAlarm AlarmInfo;
    /// <summary>
    /// 告警设备
    /// </summary>
    public DevNode currentDev;
    /// <summary>
    /// 闪烁频率
    /// </summary>
    private float frequency = 2f;
    // Use this for initialization
    void Start () {
		
	}
    /// <summary>
    /// 初始化告警信息
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="alarmContent"></param>
    public void InitAlarmInfo(DeviceAlarm alarmContent,DevNode dev)
    {
        currentDev = dev;
        currentDev.isAlarm = true;
        AlarmInfo = alarmContent;
        dev.AddAlarm(alarmContent);
        SetNormalFollowUIState(dev,false);
    }
   
    /// <summary>
    /// 开始告警
    /// </summary>
	public void AlarmOn()
    {
        if (!DevAlarmManage.IsShowAlarm) return;
        if(currentDev is BorderDevController)
        {
            BorderDevController dev = currentDev as BorderDevController;
            dev.AlarmOn();
        }
        else
        {
            Highlighter h = gameObject.AddMissingComponent<Highlighter>();
            Color colorStart = Color.red;
            Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0);
            h.FlashingOn(colorStart, colorEnd, frequency);
            AlarmMonitorRange(true);
        }        
    }
    /// <summary>
    /// 结束告警
    /// </summary>
    public void AlarmOff(bool isRemoveAlarm)
    {
        if(isRemoveAlarm&&currentDev!=null)
        {
            SetNormalFollowUIState(currentDev,true);
        }
        if(isRemoveAlarm&&currentDev!=null)
        {
            currentDev.isAlarm = false;
            if (FollowTargetManage.Instance) FollowTargetManage.Instance.RemoveAlarmDevFollowUI(currentDev);
        }
        if (currentDev is BorderDevController)
        {
            BorderDevController dev = currentDev as BorderDevController;
            dev.AlarmOff();
        }
        else
        {
            AlarmMonitorRange(false);
            Highlighter h = gameObject.AddMissingComponent<Highlighter>();
            h.FlashingOff();
        }        
    }

    private void SetNormalFollowUIState(DevNode dev,bool isShow)
    {
        if(dev==null||dev.FollowUI==null)return;
        UGUIFollowTarget follow = dev.FollowUI.GetComponent<UGUIFollowTarget>();
        if(follow)
        {
            follow.enabled = isShow;
            dev.FollowUI.SetActive(isShow);
        }
    }
    /// <summary>
    /// 区域告警/消警
    /// </summary>
    private void AlarmMonitorRange(bool isAlarm)
    {
        if (currentDev == null || currentDev.ParentDepNode == null) return;
        if (!TypeCodeHelper.IsAlarmDev(currentDev.Info.TypeCode.ToString())) return;
        DepNode dep = currentDev.ParentDepNode;
        if (isAlarm)
        {
            if ((dep is RoomController||dep is FloorController) && dep.monitorRangeObject != null)
            {
                dep.monitorRangeObject.AlarmOn();
            }
        }
        else
        {
            if((dep is RoomController || dep is FloorController))
            {
                List<DevNode>roomDevs = RoomFactory.Instance.GetDepDevs(dep);
                if (roomDevs == null||roomDevs.Count==0) return;
                bool isOtherDevAlarm = false;
                for(int i=0;i<roomDevs.Count;i++)
                {
                    if(roomDevs[i].isAlarm&&roomDevs[i]!=currentDev)
                    {
                        isOtherDevAlarm = true;
                        break;
                    }
                }
                //区域下没有告警设备，取消告警
                if(!isOtherDevAlarm&&dep.monitorRangeObject!=null)
                {
                    dep.monitorRangeObject.AlarmOff();
                }
            }            
        }
    }
}
