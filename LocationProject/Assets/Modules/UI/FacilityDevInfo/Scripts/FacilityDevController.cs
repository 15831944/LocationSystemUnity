using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Location.WCFServiceReferences.LocationServices;
using System;
using RTEditor;
using Mogoson.CameraExtension;

public class FacilityDevController : DevNode
{
    /// <summary>
    /// 生产信息漂浮UI
    /// </summary>
    private DeviceFollowUI followUI;
	// Use this for initialization
	public override void Start ()
	{
        if (transform.GetComponent <Collider>()!=null)
        {
            DoubleClickEventTrigger_u3d trigger = DoubleClickEventTrigger_u3d.Get(gameObject);
            trigger.onClick += OnClick;
            trigger.onDoubleClick += OnDoubleClick;
        }      
    }
    public void OnClick()
    {        
        Debug.Log("FacilityDevController.OnClick");
        if (PersonSubsystemManage.Instance.IsOnEditArea) return;
        if (Info == null)
        {
            Debug.LogError("FacilityDevController.OnClick Info == null");
            //return;//2019_05_16_cww:不还回
        }
        if (!DevSubsystemManage.IsRoamState)
        {
            HighlightOn();

            //FollowTargetManage.Instance.CreateDevFollowUI(gameObject, ParentDepNode, this);
        }
        ModifyDevInfo();
        ShowFollowUI();
        if (Info != null)
        {
            Debug.Log("Click  ID: " + Info.Id + " DevID: " + Info.DevID);
        }
       
    }
    public void OnDoubleClick()
    {
        DepNode dep = transform.GetComponent<DepNode>();
        if (dep) return;//既是建筑，又是设备，先响应建筑的双击
        Debug.Log("FacilityDevController.OnDoubleClick");
        if (DevSubsystemManage.IsRoamState) return;
        if (Info == null)
        {
            Debug.LogError("FacilityDevController.OnDoubleClick Info == null");
            //return;//2019_05_14_cww:不还回
        }
         FocusDev();
    }
    /// <summary>
    /// 显示生产信息漂浮UI
    /// </summary>
    public void ShowFollowUI()
    {
        if (followUI == null)
        {
            CreateFollowUI();
        }

        bool isEditMode = DevSubsystemManage.Instance != null && DevSubsystemManage.Instance.DevEditorToggle.isOn ? true : false;
        if (followUI!=null&& !DevSubsystemManage.IsRoamState&&!isEditMode)
        {
            followUI.ShowUI();
        }
    }

    /// <summary>
    /// 聚焦/取消聚焦
    /// </summary>
    private void FocusDev()
    {
        if (!IsFocus) FocusOn();
        else FocusOff();
    }

    private bool isInfoFinded = false;

    /// <summary>
    /// 创建漂浮UI
    /// </summary>
    public void CreateFollowUI()
    {
        if (Info == null)
        {
            if (isInfoFinded == false)
            {
                Info = CommunicationObject.Instance.GetDevByGameObjecName(this.name);
                Log.Info("FacilityDevController.CreateFollowUI", string.Format("FindInfo:{0},{1}", this.name, Info));
                isInfoFinded = true;
            }
            
        }
        if (ParentDepNode == null)
        {
            Log.Error("FacilityDevController.CreateFollowUI", "ParentDepNode == null");
        }

        FollowTargetManage.Instance.CreateDevFollowUI(gameObject, ParentDepNode, this, obj => { followUI = obj; });
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

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (RoomFactory.Instance.StaticDevList.Contains(this))
        {
            RoomFactory.Instance.StaticDevList.Remove(this);
        }
    }

    float radius = 0;

    /// <summary>
    /// 获取相机聚焦物体的信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected override AlignTarget GetTargetInfo(GameObject obj)
    {
        if (radius == 0)
        {
            var bounds = ColliderHelper.CaculateBounds(gameObject.transform, false);//不用碰撞体，计算包围盒就行了
            radius = ColliderHelper.GetRadius(bounds.size);
        }

        camDistance = radius * 1.5f;
        disRange = new Range(radius * 0.75f, radius * 3f);
        angleFocus = new Vector2(60, 60);

        AlignTarget alignTargetTemp = new AlignTarget(obj.transform, angleFocus,
                               camDistance, angleRange, disRange);
        return alignTargetTemp;
    }
}
