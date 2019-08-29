using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAccessDevController : DevNode {
    /// <summary>
    /// 门禁对应的门
    /// </summary>
    public DoorAccessItem DoorItem;
    /// <summary>
    /// 门禁信息，通讯用
    /// </summary>
    public Dev_DoorAccess DoorAccessInfo;
    /// <summary>
    /// 门是否打开
    /// </summary>
    private bool IsDoorOpen;
    // Use this for initialization
    public override void Start()
    {
        if(Info!=null)
        {
            base.Start();
            DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
            trigger.onClick = OnClick;
            trigger.onDoubleClick = OnDoubleClick;
        }
        else
        {
            Debug.LogError("DoorAccessDevController.DevInfo is null->TransformName:"+transform.name);
        }
        
    }
    private void OnClick()
    {
        if (DevSubsystemManage.IsRoamState||DoorItem==null) return;
        if (!IsDoorOpen)
        {
            IsDoorOpen = true;
            DoorItem.OpenDoor();
        }
        else
        {
            IsDoorOpen = false;
            DoorItem.CloseDoor(false);
        }
    }
    private void OnDoubleClick()
    {
        if (DevSubsystemManage.IsRoamState) return;
        FocusDev();
    }
    /// <summary>
    /// 聚焦/取消聚焦
    /// </summary>
    private void FocusDev()
    {
        if (!IsFocus) FocusOn();
        else FocusOff();
    }
    /// <summary>
    /// 开门
    /// </summary>
    public void OpenDoor()
    {
        DoorItem.OpenDoor();
    }
    /// <summary>
    /// 关门
    /// </summary>
    public void CloseDoor()
    {
        DoorItem.CloseDoor(false);
    }
}
