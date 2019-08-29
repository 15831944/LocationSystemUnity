using Mogoson.CameraExtension;
using RTEditor;
using System.Collections;
using System.Collections.Generic;
using Unity.Modules.Context;
using UnityEngine;
using UnityEngine.UI;

public class DevSubsystemManage : MonoBehaviour {
    public static DevSubsystemManage Instance;
    /// <summary>
    /// 漫游
    /// </summary>
    public Toggle RoamToggle;
   
    /// <summary>
    /// 查询
    /// </summary>
    public Toggle QueryToggle;
   
    /// <summary>
    /// 告警
    /// </summary>
    public Toggle DevAlarmToggle;
    
    /// <summary>
    ///设备编辑
    /// </summary>
    public Toggle DevEditorToggle;

    /// <summary>
    /// 设施管理的子系统
    /// </summary>
    public GameObject DevSubsystem;

    /// <summary>
    /// 漫游时，进入的建筑
    /// </summary>
    public List<BuildingController> triggerBuildings = new List<BuildingController>();

    /// <summary>
    /// 鼠标聚焦的设备
    /// </summary>
    public List<DevNode> FocusDevList = new List<DevNode>();

    /// <summary>
    /// 是否漫游状态
    /// </summary>
    public static bool IsRoamState { get
        {
            return GlobalState.IsRoamState;
        }
        set
        {
            GlobalState.IsRoamState = value;
        }
    }
    /// <summary>
    /// 进入漫游时，改变的Collider
    /// </summary>
    public List<Collider> ColliderChangeList = new List<Collider>();
    void Start()
    {
        Instance = this;
        InitToggleMethod();
        ToggleAuthoritySet();
        SceneEvents.FullViewStateChange += OnFullViewChange;
    }
    
    private void OnFullViewChange(bool isFullView)
    {
        if(isFullView)
        {

        }
        else
        {
            ToggleAuthoritySet();
        }
    }
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            DevEditorToggle.gameObject.SetActive(false);
        }
        else
        {
            DevEditorToggle.gameObject.SetActive(true);
        }
    }
    #region 功能管理
    /// <summary>
    /// 退出子系统
    /// </summary>
    public void ExitDevSubSystem()
    {
        if (RoamToggle.isOn)
        {
            RoamToggle.isOn = false;
            //OnRoamToggleChange(false);
        }else if (QueryToggle.isOn)
        {
            QueryToggle.isOn = false;
            //OnQueryToggleChange(false);
        }else if (DevAlarmToggle.isOn)
        {
            DevAlarmToggle.isOn = false;
            //OnAlarmToggleChange(false);
        }
        else if(DevEditorToggle.isOn)
        {
            DevEditorToggle.isOn = false;
            //OnDevEditToggleChange(false);
        }
    }
    /// <summary>
    /// UI绑定方法
    /// </summary>
    private void InitToggleMethod()
    {
        RoamToggle.onValueChanged.AddListener(OnRoamToggleChange);
        QueryToggle.onValueChanged.AddListener(OnQueryToggleChange);
        DevAlarmToggle.onValueChanged.AddListener(OnAlarmToggleChange);
        DevEditorToggle.onValueChanged.AddListener(OnDevEditToggleChange);
    }
    /// <summary>
    /// 漫游模式
    /// </summary>
    /// <param name="isOn"></param>
    private void OnRoamToggleChange(bool isOn)
    {
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn);
        IsRoamState = isOn;//设置漫游标志位
        CameraSceneManager.Instance.alignCamera.SetMouseInputState(!isOn);
        RoamManage roamManager = RoamManage.Instance;
        roamManager.ShowRoamWindow(isOn);
        FPSMode.Instance.NoFPSUI.SetActive(!isOn);
        AlarmPushManage.Instance.CloseAlarmPushWindow(!isOn);
       // AlarmPushManage.Instance.IsShow.isOn = !isOn ;
        ChangeDefaultAlign(isOn);
        RoomFactory.Instance.FocusNode(FactoryDepManager.Instance,()=> 
        {           
            ChangeImage(isOn, RoamToggle);
            Debug.Log("OnRoamToggleChange:" + isOn);
            FPSMode.Instance.SetColliderState(isOn);
            FPSMode.Instance.SetBorder(isOn);
            if (roamManager)
            {
               roamManager.EntranceRoamShowBox(isOn);
                roamManager.isStart = true;
               EntranceManage.instance.ShowWindow(isOn);
            }            
            OnRoamStateChange(isOn);
            if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.SetDevInfoCheckState(isOn);
            if (isOn)
            {
                
            }
            else
            {
               
                roamManager.ExitRoam();
                if (BuildingTopColliderManage.Instance) BuildingTopColliderManage.Instance.Clear();
                SmallMapController.Instance.ShowMapByDepNode(FactoryDepManager.Instance);
            }
        });            
    }
    /// <summary>
    /// 漫游时，更改摄像机参数
    /// </summary>
    /// <param name="isOn"></param>
    private void ChangeDefaultAlign(bool isOn)
    {
        if (RoomFactory.Instance && RoomFactory.Instance.FactoryType == FactoryTypeEnum.BaoXin)
        {
            if(isOn)
            {
                AlignTarget defaultAlign = CameraSceneManager.Instance.GetDefaultAlign();
                defaultAlign.distance = 100;
                CameraSceneManager.Instance.SetDefaultAlign(defaultAlign);
            }
            else
            {
                CameraSceneManager.Instance.SetDefaultAlign();
            }          
        }
    }

    /// <summary>
    /// 搜索
    /// </summary>
    /// <param name="isOn"></param>
    public  void OnQueryToggleChange(bool isOn)
    {
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        ChangeImage(isOn, QueryToggle);
        Debug.Log("OnQueryToggleChange:" + isOn);
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();

        if (isOn)
        {
            DeviceDataPaging.Instance.ShowdevSearchWindow();
            DeviceDataPaging.Instance.StartDevSeachUI();
        }
        else
        {
            DeviceDataPaging.Instance.ClosedevSearchWindow();
        }
    }
    /// <summary>
    /// 告警系统
    /// </summary>
    /// <param name="isOn"></param>
   public  void OnAlarmToggleChange(bool isOn)
    {
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        ChangeImage(isOn, DevAlarmToggle);
        Debug.Log("OnAlarmToggleChange:" + isOn);
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();
        if (RoomFactory.Instance.FactoryType == FactoryTypeEnum.BaoXin)
        {
            BaoXinDeviceAlarm.Instance.ShowDevAlarm(isOn);
            if (isOn)
            {
                BaoXinDeviceAlarm.Instance.GetDevAlarmList();
            }
        }
        else
        {
            if (isOn)
            {

                DevAlarmListManage.Instance.ShowDevAlarmWindow();

            }
            //else
            //{
            //    DevAlarmListManage.Instance.CloseDevAlarmWindow();//界面自己关闭，这个会导致重复调用
            //}
        }
         
    }
    public bool isDevEdit;
    /// <summary>
    /// 设备编辑
    /// </summary>
    /// <param name="isOn"></param>
    private void OnDevEditToggleChange(bool isOn)
    {
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn);
        ChangeImage(isOn, DevEditorToggle);
        Debug.Log("OnDevEditToggleChange:" + isOn);
        if (isOn)
        {
             AlarmPushManage.Instance.CloseAlarmPushWindow(false);
          //  AlarmPushManage.Instance.IsShow.isOn = false;
            isDevEdit = isOn;
            //if (ViewState.设备编辑 == CurrentState) return;
            //CurrentState = ViewState.设备编辑;
            HideUiOnDevEditStart();
            ObjectAddListManage.Instance.Show();
            ObjectsEditManage.Instance.OpenDevEdit();
            //清空多选缓存
            EditorCamera.Instance.SetObjectVisibilityDirty();
            if (HighlightManage.Instance) HighlightManage.Instance.CancelHighLight();
            FunctionSwitchBarManage.Instance.SetWindow(false);
            SetGizmoTypeState(false);
            StartOutManage.Instance.ShowDevEditButton();
            DeviceEditUIManager.Instacne.SetEmptValue();
            ActionBarManage.Instance.Hide();
            UGUITooltip.Instance.Hide();
            FollowTargetManage.Instance.HideAllFollowUI(FactoryDepManager.currentDep);
        }
        else
        {
            AlarmPushManage.Instance.CloseAlarmPushWindow(true );
            //AlarmPushManage.Instance.IsShow.isOn = true ;
            isDevEdit = isOn;
            ShowUiOnDevEditEnd();
            ActionBarManage.Instance.Show();
            ObjectAddListManage.Instance.Hide();
            ObjectsEditManage.Instance.CloseDevEdit();
            DeviceEditUIManager.Instacne.Close();
            FunctionSwitchBarManage.Instance.SetWindow(true);
            SetGizmoTypeState(true);
            FollowTargetManage.Instance.ShowAllFollowUI(FactoryDepManager.currentDep);
        }
    }
    /// <summary>
    /// 隐藏UI在设备编辑状态下 
    /// </summary>
    private void HideUiOnDevEditStart()
    {
        if (TopoTreeManager.Instance) TopoTreeManager.Instance.CloseWindow();
        if (PersonnelTreeManage.Instance) PersonnelTreeManage.Instance.CloseWindow();
        if (SmallMapController.Instance) SmallMapController.Instance.Hide();
        if (StartOutManage.Instance) StartOutManage.Instance.SetMainPageAndBackState(false);
    }
    /// <summary>
    /// 结束设备编辑时，显示的界面 
    /// </summary>
    private void ShowUiOnDevEditEnd()
    {
        ActionBarManage manage = ActionBarManage.Instance;
        if (manage)
        {
            if (manage.CurrentState == ViewState.设备定位 || manage.CurrentState == ViewState.None)
            {
                bool isDevState = ActionBarManage.Instance.DevToggle.isOn;
                if (TopoTreeManager.Instance && isDevState)
                    TopoTreeManager.Instance.ShowWindow();
            }
            if (manage.CurrentState == ViewState.人员定位 && manage.PersonnelToggle.isOn)
            {
                if (PersonnelTreeManage.Instance)
                    PersonnelTreeManage.Instance.ShowWindow();
            }
        }
        if (SmallMapController.Instance) SmallMapController.Instance.Show();
        if (StartOutManage.Instance) StartOutManage.Instance.SetMainPageAndBackState(true);
    }
    /// <summary>
    /// 保存编辑之前的状态
    /// </summary>
    private Dictionary<GizmoType, bool> LastStateDic = new Dictionary<GizmoType, bool>();
    /// <summary>
    /// 设备编辑模式下，旋转缩放状态设置
    /// </summary>
    /// <param name="isOn"></param>
    private void SetGizmoTypeState(bool isOn)
    {       
        EditorGizmoSystem system = EditorGizmoSystem.Instance;
        if (!isOn)
        {
            LastStateDic.Clear();
            LastStateDic.Add(GizmoType.Translation,system.IsGizmoTypeAvailable(GizmoType.Translation));
            LastStateDic.Add(GizmoType.Rotation, system.IsGizmoTypeAvailable(GizmoType.Rotation));
            LastStateDic.Add(GizmoType.Scale, system.IsGizmoTypeAvailable(GizmoType.Scale));
            LastStateDic.Add(GizmoType.VolumeScale, system.IsGizmoTypeAvailable(GizmoType.VolumeScale));
            system.SetGizmoTypeAvailable(GizmoType.Translation,true);
            system.SetGizmoTypeAvailable(GizmoType.Scale, false);
            system.SetGizmoTypeAvailable(GizmoType.Rotation, false);
            system.SetGizmoTypeAvailable(GizmoType.VolumeScale, false);
            system.ActiveGizmoType=GizmoType.Translation;
        }
        else
        {
            if (LastStateDic.Count == 0) return;
            system.SetGizmoTypeAvailable(GizmoType.Translation, LastStateDic[GizmoType.Translation]);
            system.SetGizmoTypeAvailable(GizmoType.Scale, LastStateDic[GizmoType.Scale]);
            system.SetGizmoTypeAvailable(GizmoType.Rotation, LastStateDic[GizmoType.Rotation]);
            system.SetGizmoTypeAvailable(GizmoType.VolumeScale, LastStateDic[GizmoType.VolumeScale]);
            LastStateDic.Clear();
        }
    }
    #endregion
    /// <summary>
    /// 选中时改变图片
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="tog"></param>
    public void ChangeImage(bool isOn, Toggle tog)
    {
        if (isOn)
        {
            tog.gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            tog.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);


        }
        else
        {
            tog.gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            tog.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
    }
    /// <summary>
    /// 设备管理子系统
    /// </summary>
    /// <param name="b"></param>
    public void DevSubsystemUI(bool  b)
    {
        if (b)
        {
            DevSubsystem.SetActive(true);
        }
        else
        {
            DevSubsystem.SetActive(false);
        }

    }
    private void OnRoamStateChange(bool isOn)
    {
        if (isOn)
        {
            FPSMode.Instance.AfterExitFPS = () =>
            {
                RoamToggle.isOn = false;
            };
            if (UGUITooltip.Instance)UGUITooltip.Instance.Hide();
            EnlargeDoorCollider();
        }
        else
        {
            RecoverDoorCollider();
            HideBuildingDev();
            HideRoamDevInfo();
        }
    }
    public void SetFocusDevInfo(DevNode node,bool isSave)
    {
        if(isSave)
        {
            if (!FocusDevList.Contains(node))
            {
                FocusDevList.Add(node);
            }
        }
        else
        {
            if (FocusDevList.Contains(node))
            {
                FocusDevList.Remove(node);
            }
        }     
    }
    private void HideRoamDevInfo()
    {
        if(FocusDevList!=null&&FocusDevList.Count!=0)
        {
            for(int i=0;i<FocusDevList.Count;i++)
            {
                FocusDevList[i].HighLightOff();
            }
            FocusDevList.Clear();
            if (RoamDevInfoUI.Instance) RoamDevInfoUI.Instance.Close();
        }
    }
    /// <summary>
    /// 是否进入建筑
    /// </summary>
    /// <param name="building"></param>
    /// <returns></returns>
    public bool IsBuildingExist(BuildingController building)
    {
        if (triggerBuildings.Contains(building)) return true;
        else return false;
    }
    /// <summary>
    /// 设置漫游时，进入和退出的建筑
    /// </summary>
    /// <param name="building"></param>
    /// <param name="isRemove"></param>
    public void SetTriggerBuilding(BuildingController building,bool isEnterBuilding)
    {
        if(isEnterBuilding)
        {
            if (!triggerBuildings.Contains(building))
            {
                triggerBuildings.Add(building);
            }
        }
        else
        {
            if (triggerBuildings.Contains(building))
            {
                triggerBuildings.Remove(building);
            }
        }
    }
    /// <summary>
    /// 漫游人物，是否在建筑内
    /// </summary>
    /// <returns></returns>
    public bool IsFPSInBuilding()
    {
        if (triggerBuildings != null && triggerBuildings.Count > 0) return true;
        else return false;
    }
    /// <summary>
    /// 关闭建筑内，所有设备
    /// </summary>
    private void HideBuildingDev()
    {
        if(triggerBuildings!=null&&triggerBuildings.Count!=0)
        {
            foreach(var item in triggerBuildings)
            {
                item.ShowBuildingDev(false);
            }
        }
        triggerBuildings.Clear();
        RoamManage.Instance.SetLight(false);
    }
    /// <summary>
    /// 漫游时，扩大门的Collider
    /// </summary>
    [ContextMenu("EnlargeDoorCollider")]
    public void EnlargeDoorCollider()
    {
        var doorItems = FindObjectsOfType<DoorAccessItem>();
        foreach(var item in doorItems)
        {
            item.EnlargeCollider();
        }
        ColliderChangeList.Clear();
        var collider = FindObjectsOfType<MeshCollider>();
        foreach(var meshT in collider)
        {
            if (!meshT.enabled)
            {
                meshT.enabled = true;
                ColliderChangeList.Add(meshT);
            }
        }
    }

    /// <summary>
    /// 漫游时，扩大门的Collider
    /// </summary>
    public void EnlargeBuildingDoorCollider(GameObject building)
    {
        var doorItems = building.FindComponentsInChildren<DoorAccessItem>();
        foreach (var item in doorItems)
        {
            item.EnlargeCollider();
        }
        //ColliderChangeList.Clear();
        var collider = building.FindComponentsInChildren<MeshCollider>();
        foreach (var meshT in collider)
        {
            if (!meshT.enabled)
            {
                meshT.enabled = true;
                ColliderChangeList.Add(meshT);
            }
        }
    }

    /// <summary>
    /// 退出漫游，恢复门的Collider
    /// </summary>
    [ContextMenu("RecoverDoorCollider")]
    public void RecoverDoorCollider()
    {
        DoorAccessItem[] doorItems = FindObjectsOfType<DoorAccessItem>();
        foreach (var item in doorItems)
        {
            item.RecoverCollider();
        }       
        foreach (var meshT in ColliderChangeList)
        {
            meshT.enabled = false;
        }
    }

}
