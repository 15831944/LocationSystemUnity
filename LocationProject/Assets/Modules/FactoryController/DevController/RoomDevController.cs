using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDevController : DevNode
{
	// Use this for initialization
	public override void Start ()
	{
	    base.Start();
        DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        trigger.onClick = OnClick;
        trigger.onDoubleClick = OnDoubleClick;
    }
	
    private void OnClick()
    {
        if (DevSubsystemManage.IsRoamState|| ObjectAddListManage.IsEditMode) return;
        HighlightOn();
        //Debug.Log("Click:" + Info.DevName);
    }
    private void OnDoubleClick()
    {
        if (DevSubsystemManage.IsRoamState) return;
        //Debug.Log("DoubleClick:"+Info.DevName);
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
}
