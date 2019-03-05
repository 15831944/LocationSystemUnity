using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Location.WCFServiceReferences.LocationServices;
using System;
using RTEditor;

public class FacilityDevController : DevNode
{
	// Use this for initialization
	public override void Start ()
	{
        if (transform.GetComponent <Collider>()!=null)
        {
            DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
            trigger.onClick = OnClick;
            trigger.onDoubleClick = OnDoubleClick;
        }      
    }
    public void OnClick()
    {
        if (!DevSubsystemManage.IsRoamState) HighlightOn();
        ModifyDevInfo();       
        Debug.Log("Click  ID: " + Info.Id + " DevID: " + Info.DevID);
    }
    public void OnDoubleClick()
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
    /// 创建漂浮UI
    /// </summary>
    public void CreateFollowUI()
    {
        FollowTargetManage.Instance.CreateDevFollowUI(gameObject, ParentDepNode, this);
    }
    /// <summary>
    /// 修改设备信息
    /// </summary>
    private void ModifyDevInfo()
    {
        DevSubsystemManage manager = DevSubsystemManage.Instance;
        if(manager&&manager.DevEditorToggle.isOn)
        {
            ClearSelection();
            DeviceEditUIManager.Instacne.Show(this);
        }
    }

    /// <summary>
    /// 清除设备选中
    /// </summary>
    private void ClearSelection()
    {
        EditorObjectSelection selection = EditorObjectSelection.Instance;
        if (selection)
        {
            selection.ClearSelection(false);
        }
    }
    /// <summary>
    /// 鼠标是否Hover
    /// </summary>
    /// <param name="isEnter"></param>
    public void SetMouseState(bool isEnter)
    {
        if(isEnter)
        {
            if (!DevSubsystemManage.IsRoamState) return;
            HighlightOn();
            DevSubsystemManage.Instance.SetFocusDevInfo(this, true);
            if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.ShowDevInfo(Info);
        }
        else
        {
            if (!DevSubsystemManage.IsRoamState) return;
            HighLightOff();
            DevSubsystemManage.Instance.SetFocusDevInfo(this, false);
            if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.Close();
        }
    }

    //void OnMouseEnter()
    //{
    //    if (!DevSubsystemManage.IsRoamState) return;
    //    HighlightOn();
    //    DevSubsystemManage.Instance.SetFocusDevInfo(this, true);
    //    if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.ShowDevInfo(Info);
    //}
    //void OnMouseExit()
    //{
    //    if (!DevSubsystemManage.IsRoamState) return;
    //    HighLightOff();
    //    DevSubsystemManage.Instance.SetFocusDevInfo(this, false);
    //    if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.Close();
    //}
}
