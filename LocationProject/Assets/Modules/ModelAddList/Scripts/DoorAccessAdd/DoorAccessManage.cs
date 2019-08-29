using System.Collections;
using System.Collections.Generic;
using TModel.Location.Data;
using UnityEngine;

public class DoorAccessManage : MonoBehaviour {
    /// <summary>
    /// 是否显示门禁状态
    /// </summary>
    public static bool IsShowDoorState;
    /// <summary>
    /// 所有门禁的状态
    /// </summary>
    public List<DoorAccessState> DoorStateList = new List<DoorAccessState>();
    /// <summary>
    /// 当前区域的门
    /// </summary>
    public List<DoorAccessItem> DepDoorList = new List<DoorAccessItem>();
    // Use this for initialization
    void Start () {
        CommunicationCallbackClient.Instance.doorAccessHub.OnDoorAccessStateRecieved += OnDoorStateRecieved;
        SceneEvents.OnDepCreateComplete += OnRoomCreateComplete;
    }
	
    /// <summary>
    /// 区域创建完成（设备加载完成后）
    /// </summary>
    /// <param name="dep"></param>
    private void OnRoomCreateComplete(DepNode dep)
    {
        ShowDepDoorState(dep);
    }
    /// <summary>
    /// 获取门禁推送信息
    /// </summary>
    /// <param name="doorStates"></param>
    private void OnDoorStateRecieved(List<DoorAccessState>doorStates)
    {
        foreach(var item in doorStates)
        {
            PushDoorStateInfo(item);
        }
    }
    /// <summary>
    /// 更换当前区域下，门的开关状态
    /// </summary>
    /// <param name="state"></param>
    private void PushDoorStateInfo(DoorAccessState state)
    {
        DoorAccessState door = DoorStateList.Find(i=>i.DoorId==state.DoorId&&i.Dev.Id==state.Dev.Id);
        if (door != null) door.Abutment_CardState = state.Abutment_CardState;
        else DoorStateList.Add(state);
        if (FactoryDepManager.currentDep == null) return;
        string parentId = state.Dev.ParentId.ToString();
        if (IsDepDev(FactoryDepManager.currentDep, parentId))
        {
            DevNode dev = RoomFactory.Instance.GetCreateDevById(state.Dev.DevID, int.Parse(parentId));
            if (dev != null)
            {
                DoorAccessDevController doorAccess = dev as DoorAccessDevController;
                if (doorAccess)
                {
                    DoorAccessItem doorItem = doorAccess.DoorItem;
                    if(doorItem==null)
                    {
                        Log.Error("DoorAccessManage.PushDoorStateInfo.DoorAcessItem is null:"+doorAccess.Info==null? "devinfo id is null":doorAccess.Info.Id.ToString());
                        return;
                    }
                    doorItem.ChangeDoorState(state);
                    if(!DepDoorList.Contains(doorItem))DepDoorList.Add(doorItem);
                }
            }           
        }
    }
    /// <summary>
    /// 显示当前区域门禁状态
    /// </summary>
    /// <param name="currentDep"></param>
    private void ShowDepDoorState(DepNode currentDep)
    {
        CloseLastDep(currentDep);
        ShowDepDoors(currentDep);
    }
    /// <summary>
    /// 显示当前区域门的状态
    /// </summary>
    /// <param name="currentDep"></param>
    private void ShowDepDoors(DepNode currentDep)
    {
        List<DoorAccessState> doorStates = DoorStateList.FindAll(dev=>dev.Dev.ParentId==currentDep.NodeID);
        if(doorStates!=null&&doorStates.Count!=0)
        {
            foreach(var door in doorStates)
            {
                DevNode dev = RoomFactory.Instance.GetCreateDevById(door.Dev.DevID, (int)door.Dev.ParentId);
                DoorAccessDevController devController = dev as DoorAccessDevController;
                if(devController != null)
                {
                    DoorAccessItem doorItem = devController.DoorItem;
                    doorItem.ChangeDoorState(door);
                    if (!DepDoorList.Contains(doorItem)) DepDoorList.Add(doorItem);
                }
            }
            
        }
    }
    /// <summary>
    /// 关闭上一个区域的门
    /// </summary>
    private void CloseLastDep(DepNode currentDep)
    {
        if(DepDoorList!=null&&DepDoorList.Count!=0)
        {
            if (DepDoorList[0].ParentID == currentDep.NodeID) return;
            foreach(var item in DepDoorList)
            {
                item.CloseDoor(true);
            }
            DepDoorList.Clear();
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
}
