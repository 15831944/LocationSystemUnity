using Assets.M_Plugins.Helpers.Utils;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevAlarmManage : MonoBehaviour {
    //public static DevAlarmManage Instance;

    /// <summary>
    /// 是否显示告警（演示情况下，可能屏蔽告警）
    /// </summary>
    public static bool IsShowAlarm;
    /// <summary>
    /// 告警信息列表
    /// </summary>
    private List<DeviceAlarm> AlarmInfoList = new List<DeviceAlarm>();
    /// <summary>
    /// 当前告警设备
    /// </summary>
    private List<DevAlarmInfo> AlarmDevList = new List<DevAlarmInfo>();
    /// <summary>
    /// 告警中的设备Id
    /// </summary>
    private List<int> AlarmDevsId = new List<int>();
    // Use this for initialization
    void Start()
    {
        IsShowAlarm = true;
        //Instance = this;
        CommunicationCallbackClient.Instance.alarmHub.OnDeviceAlarmRecieved += OnDeviceAlarmRecieved;
        SceneEvents.OnDepCreateComplete += OnRoomCreateComplete;
        SceneEvents.DepNodeChanged += OnDepChanged;
    }
    #region 设备告警
    private void OnDepChanged(DepNode oldDep, DepNode currentDep)
    {
        if ((oldDep is RoomController && oldDep.ParentNode == currentDep) || (currentDep is RoomController && currentDep.ParentNode == oldDep)) return;
        HighOffLastDep();
    }
    /// <summary>
    /// 区域创建完成（设备加载完成后）
    /// </summary>
    /// <param name="dep"></param>
    private void OnRoomCreateComplete(DepNode dep)
    {
        ShowDevAlarmInfo(dep);
    }
    private void OnDeviceAlarmRecieved(List<DeviceAlarm> devList)
    {
        foreach (var dev in devList)
        {
            if (dev.Level!=Abutment_DevAlarmLevel.无)
            {
                PushAlarmInfo(dev);
            }
            else
            {
                RemoveAlarmInfo(dev);
            }          
        }
    }
    /// <summary>
    /// 显示设备告警
    /// </summary>
    public void ShowDevAlarm()
    {
        IsShowAlarm = true;
        if (AlarmDevList != null && AlarmDevList.Count != 0)
        {
            foreach (var dev in AlarmDevList)
            {
                if (dev == null || dev.gameObject == null) continue;
                dev.AlarmOn();
            }
        }
    }
    /// <summary>
    /// 关闭设备告警
    /// </summary>
    public void HideDevAlarm()
    {
        IsShowAlarm = false;
        if (AlarmDevList != null && AlarmDevList.Count != 0)
        {
            foreach (var dev in AlarmDevList)
            {
                if (dev == null || dev.gameObject == null) continue;
                dev.AlarmOff(false);
            }
        }
    }
    /// <summary>
    /// 服务端推送告警信息
    /// </summary>
    private void PushAlarmInfo(DeviceAlarm alarmInfo)
    {
        if (AlarmDevsId.Contains(alarmInfo.DevId))
        {
            Debug.Log("Alarm is already exist.");
            return;
        }
        AlarmDevsId.Add(alarmInfo.DevId);
        AlarmInfoList.Add(alarmInfo);
        if (FactoryDepManager.currentDep == null) return;
        string parentId = alarmInfo.Dev.ParentId.ToString();
        if (IsDepDev(FactoryDepManager.currentDep, parentId))
        {
            DevNode dev = RoomFactory.Instance.GetCreateDevById(alarmInfo.Dev.DevID, int.Parse(parentId));
            if (dev == null)
            {
                Debug.LogError("Dev not find:" + alarmInfo.DevId);
                return;
            }
            AlarmDev(dev, alarmInfo);
        }
    }
    /// <summary>
    /// 告警恢复
    /// </summary>
    /// <param name="alarmInfo"></param>
    private void RemoveAlarmInfo(DeviceAlarm alarmInfo)
    {
        if (AlarmDevsId.Contains(alarmInfo.DevId)) AlarmDevsId.Remove(alarmInfo.DevId);
        else
        {
            Debug.Log("CancelAlarm Failed,Dev is null.DevId:" + alarmInfo.DevId);
            return;
        }
        DeviceAlarm alarmInfoTemp = AlarmInfoList.Find(dev => dev.DevId == alarmInfo.DevId);
        if (alarmInfoTemp == null) return;
        AlarmInfoList.Remove(alarmInfoTemp);
        //恢复正在告警的设备
        if (FactoryDepManager.currentDep == null) return;
        if (IsDepDev(FactoryDepManager.currentDep, alarmInfo.Dev.ParentId.ToString()))
        {
            try
            {
                DevAlarmInfo dev = AlarmDevList.Find(i => i.AlarmInfo.DevId == alarmInfo.DevId);
                if (dev == null) return;
                dev.AlarmOff(true);
                AlarmDevList.Remove(dev);
                DestroyImmediate(dev);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }


    /// <summary>
    /// 显示区域下，设备告警信息
    /// </summary>
    /// <param name="parentId"></param>
    private void ShowDevAlarmInfo(DepNode dep)
    {
        //HighOffLastDep();
        string pId = dep.NodeID.ToString();
        if (AlarmInfoList == null || AlarmInfoList.Count == 0) return;
        List<DeviceAlarm> alarmInfos = GetDepAlarmInfo(dep);
        if (alarmInfos != null && alarmInfos.Count != 0)
        {
            List<DevNode> devs = RoomFactory.Instance.GetDepDevs(dep);
            foreach (var alarmDev in alarmInfos)
            {
                DevNode dev = devs.Find(i => i.Info.DevID == alarmDev.Dev.DevID);
                AlarmDev(dev, alarmDev);
            }
        }
    }
    /// <summary>
    /// 获取区域下，设备告警信息
    /// </summary>
    /// <param name="dep"></param>
    /// <returns></returns>
    private List<DeviceAlarm> GetDepAlarmInfo(DepNode dep)
    {
        List<DeviceAlarm> alarmInfos = new List<DeviceAlarm>();
        if (dep as FloorController)
        {
            alarmInfos.AddRange(AlarmInfoList.FindAll(i => (int)i.Dev.ParentId == dep.NodeID));
            foreach (var room in dep.ChildNodes)
            {
                alarmInfos.AddRange(AlarmInfoList.FindAll(i => (int)i.Dev.ParentId == room.NodeID));
            }
        }
        else
        {
            alarmInfos = AlarmInfoList.FindAll(i => (int)i.Dev.ParentId == dep.NodeID);
        }
        return alarmInfos;
    }
    /// <summary>
    /// 取消上一个区域的告警
    /// </summary>
    private void HighOffLastDep()
    {
        if (AlarmDevList != null && AlarmDevList.Count != 0)
        {
            foreach (var dev in AlarmDevList)
            {
                if (dev == null || dev.gameObject == null) continue;
                dev.AlarmOff(false);
            }
            AlarmDevList.Clear();
        }
    }

    /// <summary>
    /// 显示设备告警
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="alarmInfo"></param>
    private void AlarmDev(DevNode dev, DeviceAlarm alarmInfo)
    {
        if (dev == null || dev.gameObject == null) return;
        DevAlarmInfo info = dev.gameObject.GetComponent<DevAlarmInfo>();
        if(info == null)
        {
            info = dev.gameObject.AddMissingComponent<DevAlarmInfo>();
            info.InitAlarmInfo(alarmInfo, dev);
            if (FollowTargetManage.Instance)
            {
                string typeCode = dev.Info.TypeCode.ToString();
                if (TypeCodeHelper.IsBorderAlarmDev(typeCode))
                {
                    FollowTargetManage.Instance.CreateBorderDevFollowUI(dev.gameObject, dev.ParentDepNode, alarmInfo);
                }else if(TypeCodeHelper.IsAlarmDev(typeCode))
                {
                    FollowTargetManage.Instance.CreateFireDevFollowUI(dev.gameObject, dev.ParentDepNode, alarmInfo);
                }
            }
        }
        if(!AlarmDevList.Contains(info))AlarmDevList.Add(info);
        info.AlarmOn();
    }
    /// <summary>
    /// 是否当前区域设备
    /// </summary>
    /// <param name="currentDep"></param>
    /// <param name="ParentId"></param>
    /// <returns></returns>
    private bool IsDepDev(DepNode currentDep, string ParentId)
    {
        if (currentDep as FloorController)
        {
            string floorId = currentDep.NodeID.ToString();
            if (floorId == ParentId) return true;
            else if (currentDep.ChildNodes != null)
            {
                foreach (var room in currentDep.ChildNodes)
                {
                    if (room.NodeID.ToString() == ParentId) return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (currentDep.NodeID.ToString() == ParentId) return true;
            else return false;
        }
    }
    #endregion
}
