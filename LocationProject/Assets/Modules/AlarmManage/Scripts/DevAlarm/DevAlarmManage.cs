using Assets.M_Plugins.Helpers.Utils;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevAlarmManage : MonoBehaviour {
    //public static DevAlarmManage Instance;
    public static DevAlarmManage Instance;
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
    ///// <summary>
    ///// 告警中的设备Id
    ///// </summary>
    //private List<int> AlarmDevsId = new List<int>();
    private string AlarmDevUIName = "AlarmDevUI";  //告警设备(边界、消防等)

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        IsShowAlarm = true;
        //Instance = this;
        CommunicationCallbackClient.Instance.alarmHub.OnDeviceAlarmRecieved += OnDeviceAlarmRecieved;
        SceneEvents.OnDepCreateComplete += OnRoomCreateComplete;
        SceneEvents.DepNodeChanged += OnDepChanged;
    }

    /// <summary>
    /// 区域是否有消防告警
    /// </summary>
    /// <param name="depId"></param>
    /// <returns></returns>
    public bool IsDepFireAlarm(int depId)
    {
        DeviceAlarm alarmT = AlarmInfoList.Find(dev=>dev!=null&&dev.AreaId==depId&&TypeCodeHelper.IsFireFightDevType(dev.DevTypeCode.ToString()));
        return alarmT == null ? false : true;
    }

    #region 设备告警
    private void OnDepChanged(DepNode oldDep, DepNode currentDep)
    {
        if ((oldDep is RoomController && oldDep.ParentNode == currentDep) || (currentDep is RoomController && currentDep.ParentNode == oldDep)) return;
        HighOffLastDep(oldDep);
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
        //Debug.LogError("RecieveAlarmDevs,Count:"+devList.Count);
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
        var logTag = "DevAlarmManage.PushAlarmInfo";
        Log.Info(logTag);
        //if (AlarmDevsId.Contains(alarmInfo.DevId))
        //{
        //    Debug.Log("Alarm is already exist.");
        //    return;
        //}
        //AlarmDevsId.Add(alarmInfo.DevId);
        if (AlarmInfoList.Contains(alarmInfo))
        {
            Debug.Log("Alarm is already exist.");
            return;
        }
        AlarmInfoList.Add(alarmInfo);

        string parentId = alarmInfo.AreaId.ToString();
        bool isDepDev = IsDepDev(FactoryDepManager.currentDep, parentId);
        bool isFireFightDevType = TypeCodeHelper.IsFireFightDevType(alarmInfo.DevTypeCode.ToString());
        Log.Info(logTag,string.Format("isDepDev:{0},isFireFightDevType:{1}",isDepDev, isFireFightDevType));
        if (isDepDev)
        {
            DevNode dev = RoomFactory.Instance.GetCreateDevById(alarmInfo.DevId.ToString(), int.Parse(parentId));
            if (dev == null && !isFireFightDevType)
            {
                Debug.LogError("Dev not find:" + alarmInfo.DevId);
                return;
            }
            AlarmDev(dev, alarmInfo);
        }
        else
        {
            if (isFireFightDevType)
            {
                int areaId = (int)alarmInfo.AreaId;
                if (!FireAreas.Contains(areaId))
                {
                    FireAreas.Add(areaId);
                }
            }
        }
    }
    /// <summary>
    /// 告警恢复
    /// </summary>
    /// <param name="alarmInfo"></param>
    private void RemoveAlarmInfo(DeviceAlarm alarmInfo)
    {
        //if (AlarmDevsId.Contains(alarmInfo.DevId)) AlarmDevsId.Remove(alarmInfo.DevId);
        //else
        //{
        //    Debug.Log("CancelAlarm Failed,Dev is null.DevId:" + alarmInfo.DevId);
        //    return;
        //}
        DeviceAlarm alarmInfoTemp = AlarmInfoList.Find(dev => dev.DevId == alarmInfo.DevId);
        if (alarmInfoTemp == null) return;
        AlarmInfoList.Remove(alarmInfoTemp);
        //恢复正在告警的设备
        if (IsDepDev(FactoryDepManager.currentDep, alarmInfo.AreaId.ToString()))
        {
            try
            {
                if(TypeCodeHelper.IsFireFightDevType(alarmInfo.DevTypeCode.ToString()))
                {
                    DepNode area = RoomFactory.Instance.GetDepNodeById((int)alarmInfo.AreaId);
                    if (area != null)
                        AlarmMonitorRange(false,alarmInfo,area);
                }
                else
                {
                    DevAlarmInfo dev = AlarmDevList.Find(i => i.AlarmInfo.DevId == alarmInfo.DevId);
                    if (dev == null) return;
                    dev.AlarmOff(true);
                    AlarmDevList.Remove(dev);
                    DestroyImmediate(dev);
                }                               
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
        ShowDepFireAlarm(dep);
        string pId = dep.NodeID.ToString();
        if (AlarmInfoList == null || AlarmInfoList.Count == 0) return;
        List<DeviceAlarm> alarmInfos = GetDepAlarmInfo(dep);
        if (alarmInfos != null && alarmInfos.Count != 0)
        {
            List<DevNode> devs = RoomFactory.Instance.GetDepDevs(dep);
            foreach (var alarmDev in alarmInfos)
            {
                DevNode dev = devs.Find(i => i.Info.Id == alarmDev.DevId);
                AlarmDev(dev, alarmDev);
            }
        }
    }
    /// <summary>
    /// 取消上一个区域的告警
    /// </summary>
    private void HighOffLastDep(DepNode dep)
    {
        HideDepFireAlarms(dep);
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
    /// 获取区域下，设备告警信息
    /// </summary>
    /// <param name="dep"></param>
    /// <returns></returns>
    private List<DeviceAlarm> GetDepAlarmInfo(DepNode dep)
    {
        try
        {
            if (dep == null) return null;
            List<DeviceAlarm> alarmInfos = new List<DeviceAlarm>();
            if (dep as FloorController)
            {
                alarmInfos.AddRange(AlarmInfoList.FindAll(i => i.AreaId == dep.NodeID));
                foreach (var room in dep.ChildNodes)
                {
                    if (room == null) continue;
                    alarmInfos.AddRange(AlarmInfoList.FindAll(i => i.AreaId == room.NodeID));
                }
            }
            else
            {
                alarmInfos = AlarmInfoList.FindAll(i => i.AreaId == dep.NodeID);
            }
            return alarmInfos;
        }catch(Exception e)
        {
            Debug.LogError("Error:DevAlarmManage.GetDepAlarmInfo,Exception:"+e.ToString());
            return null;
        }
    }
   

    /// <summary>
    /// 显示设备告警
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="alarmInfo"></param>
    private void AlarmDev(DevNode dev, DeviceAlarm alarmInfo)
    {
        var logTag = "DevAlarmManage.AlarmDev";
        Log.Info(logTag);
        if (TypeCodeHelper.IsFireFightDevType(alarmInfo.DevTypeCode.ToString()))
        {
            DepNode area = RoomFactory.Instance.GetDepNodeById((int)alarmInfo.AreaId);
            if(area!=null)
            {
                AlarmMonitorRange(true, alarmInfo, area);                
            }            
        }
        else
        {
            if (dev == null || dev.gameObject == null) return;
            DevAlarmInfo info = dev.gameObject.GetComponent<DevAlarmInfo>();
            if (info == null)
            {
                info = dev.gameObject.AddMissingComponent<DevAlarmInfo>();
            }
            if (!AlarmDevList.Contains(info)) AlarmDevList.Add(info);

            //Debug.LogError("增加告警信息："+alarmInfo.Message);
            info.InitAlarmInfo(alarmInfo, dev);//设置告警内容
            info.AlarmOn();//高亮显示

            if (FollowTargetManage.Instance)//告警跟随UI信息
            {
                FollowTargetManage.Instance.SetAlarmFollowUI(dev, alarmInfo);
            }
        }        
    }
    /// <summary>
    /// 是否当前区域设备
    /// </summary>
    /// <param name="currentDep"></param>
    /// <param name="ParentId"></param>
    /// <returns></returns>
    private bool IsDepDev(DepNode currentDep, string ParentId)
    {
        if (currentDep == null) return false;
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
    /// <summary>
    /// 消防告警区域
    /// </summary>
    private List<int> FireAreas = new List<int>();
    /// <summary>
    /// 区域告警/消警
    /// </summary>
    private void AlarmMonitorRange(bool isAlarm,DeviceAlarm alarmInfo,DepNode parentNode)
    {
        if (parentNode == null) return;
        if (!TypeCodeHelper.IsFireFightDevType(alarmInfo.DevTypeCode.ToString())) return;       
        if (isAlarm)
        {
            if ((parentNode is RoomController || parentNode is FloorController) && parentNode.monitorRangeObject != null)
            {
                if(!FireAreas.Contains(parentNode.NodeID))
                {
                    FireAreas.Add(parentNode.NodeID);
                    FollowTargetManage.Instance.CreateFireDevFollowUI(parentNode.monitorRangeObject.gameObject, parentNode, alarmInfo);
                }                
                parentNode.monitorRangeObject.AlarmOn();
            }
        }
        else
        {
            if ((parentNode is RoomController || parentNode is FloorController))
            {
                parentNode.monitorRangeObject.AlarmOff();
                RemoveAlarmDevFollowUI(parentNode);
                if (FireAreas.Contains(parentNode.NodeID)) FireAreas.Remove(parentNode.NodeID);
                #region TestPart
                //List<DevNode> roomDevs = RoomFactory.Instance.GetDepDevs(parentNode);
                //if (roomDevs == null || roomDevs.Count == 0) return;
                //bool isOtherDevAlarm = false;
                //for (int i = 0; i < roomDevs.Count; i++)
                //{
                //    if (roomDevs[i].Info == null) continue;
                //    if (roomDevs[i].isAlarm && roomDevs[i].Info.Id != dev.Id)
                //    {
                //        isOtherDevAlarm = true;
                //        break;
                //    }
                //}
                ////区域下没有告警设备，取消告警
                //if (!isOtherDevAlarm && parentNode.monitorRangeObject != null)
                //{
                //    parentNode.monitorRangeObject.AlarmOff();
                //    RemoveAlarmDevFollowUI(parentNode);
                //    if (!FireAreas.Contains(parentNode.NodeID)) FireAreas.Remove(parentNode.NodeID);
                //}
                #endregion
            }
        }
    }
    /// <summary>
    /// 移除消防告警
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="parentNode"></param>
    public void RemoveAlarmDevFollowUI(DepNode parentNode)
    {
        if (parentNode == null) return;
        if ((parentNode is RoomController || parentNode is FloorController) && parentNode.monitorRangeObject != null)
        {
            FollowTargetManage.Instance.RemoveAlarmDevFloowUI(parentNode,parentNode.monitorRangeObject.gameObject);
        }
    }
    /// <summary>
    /// 显示区域下消防告警
    /// </summary>
    /// <param name="currentNode"></param>
    public void ShowDepFireAlarm(DepNode currentNode)
    {
        if (FireAreas==null||FireAreas.Count == 0) return;
        AlarmDepState(currentNode, true);
        if (currentNode as FloorController)
        {
            foreach (var area in currentNode.ChildNodes)
            {
                AlarmDepState(area, true);
            }
        }
    }
    /// <summary>
    /// 关闭区域下消防告警
    /// </summary>
    /// <param name="oldDep"></param>
    public void HideDepFireAlarms(DepNode oldDep)
    {
        if (FireAreas == null || FireAreas.Count == 0) return;
        AlarmDepState(oldDep,false);
        if (oldDep as FloorController)
        {
            foreach (var area in oldDep.ChildNodes)
            {
                AlarmDepState(area,false);
            }
        }       
    }
    private void AlarmDepState(DepNode dep,bool isAlarm)
    {
        if (dep == null) return;
        if (FireAreas.Contains(dep.NodeID))
        {
            if (dep.monitorRangeObject != null)
            {
                if (!isAlarm)
                {
                    dep.monitorRangeObject.AlarmOff();
                    HideObjectFollowUI(AlarmDevUIName,dep);
                }
                else
                {
                    dep.monitorRangeObject.AlarmOn();
                    DeviceAlarm alarmInfo = AlarmInfoList.Find(i => (int)i.AreaId == dep.NodeID);
                    var obj = FollowTargetManage.Instance.CreateFireDevFollowUI(dep.monitorRangeObject.gameObject, dep, alarmInfo);
                    if (obj != null) obj.gameObject.SetActive(true);
                }
            }
        }
    }
    /// <summary>
    /// 关闭漂浮U
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="dep"></param>
    private void HideObjectFollowUI(string groupName,DepNode dep)
    {
        if (dep.monitorRangeObject == null) return;
        string groupNameTemp = string.Format("{0}{1}", groupName, dep.NodeID);
        Transform targetTagObj = dep.monitorRangeObject.gameObject.transform.Find("TitleTag");
        if (targetTagObj)
        {
            UGUIFollowTarget obj = UGUIFollowManage.Instance.GetUIbyTarget(groupNameTemp, targetTagObj.gameObject);
            if(obj) obj.gameObject.SetActive(false);
        }
    }
    #endregion
}
