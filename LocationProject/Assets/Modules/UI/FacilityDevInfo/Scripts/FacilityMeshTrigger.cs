using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class FacilityMeshTrigger : MonoBehaviour {
    /// <summary>
    /// 静态设备
    /// </summary>
    private FacilityDevController Dev;
    // Use this for initialization
    void Start () {
        Dev = transform.GetComponentInParent<FacilityDevController>();
        DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
        trigger.onClick = OnClick;
        trigger.onDoubleClick = OnDoubleClick;
    }
	
    private void OnClick()
    {
        if (Dev == null) return;
        Dev.OnClick();
    }
    private void OnDoubleClick()
    {
        if (Dev == null) return;
        Dev.OnDoubleClick();
    }

    /// <summary>
    /// 鼠标是否Hover
    /// </summary>
    /// <param name="isEnter"></param>
    public void SetMouseState(bool isEnter)
    {
        if(isEnter)
        {
            if (Dev == null || !DevSubsystemManage.IsRoamState) return;
            Dev.HighlightOn();
            DevSubsystemManage.Instance.SetFocusDevInfo(Dev, true);
            if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.ShowDevInfo(Dev.Info);
        }
        else
        {
            if (!DevSubsystemManage.IsRoamState) return;
            Dev.HighLightOff();
            DevSubsystemManage.Instance.SetFocusDevInfo(Dev, false);
            if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.Close();
        }
    }
    //void OnMouseEnter()
    //{
    //    if (Dev==null||!DevSubsystemManage.IsRoamState) return;
    //    Dev.HighlightOn();
    //    DevSubsystemManage.Instance.SetFocusDevInfo(Dev, true);
    //    if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.ShowDevInfo(Dev.Info);
    //}
    //void OnMouseExit()
    //{
    //    if (!DevSubsystemManage.IsRoamState) return;
    //    Dev.HighLightOff();
    //    DevSubsystemManage.Instance.SetFocusDevInfo(Dev, false);
    //    if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.Close();
    //}
}
