using MonitorRange;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ViewState
//{
//    None,
//    设备定位,
//    视屏监控,
//    门禁系统,
//    人员定位,
//    设备编辑
//}
public class ControlMenuController : MonoBehaviour {
    public static ControlMenuController Instance;
    /// <summary>
    /// 菜单控制栏UI部分
    /// </summary>
    public ControlMenuUI UIPart;
    public GameObject Window;

    private ViewState currentState = ViewState.None;

    /// <summary>
    /// 当前模式状态
    /// </summary>
    public ViewState CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            currentState = value;
        }
    }

    // Use this for initialization
    void Start () {
        Instance = this;
        BindingUIMethod();
        SceneEvents.FullViewStateChange += Instance_OnViewChange; ;
    }
    void OnDestroy()
    {
        SceneEvents.FullViewStateChange -= Instance_OnViewChange; ;
    }
    /// <summary>
    /// 进入首页
    /// </summary>
    /// <param name="isFullView"></param>
    private void Instance_OnViewChange(bool isFullView)
    {
        //throw new System.NotImplementedException();
        if(isFullView)
        {
            RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);
            ExitCurrentMode();
        }
        else
        {
            UIPart.DeviceLoacation.isOn = true;
            OnDeviceLocationToggleChange(true);
        }
    }

    private void ExitCurrentMode()
    {
        switch (currentState)
        {
            case ViewState.设备定位:
                OnDeviceLocationToggleChange(false);
                UIPart.DeviceLoacation.isOn = false;
                break;
            //case ViewState.视屏监控:
            //    OnVideoMonitorToggleChange(false);
            //    UIPart.VideoMonitorToggle.isOn = false;
            //    break;
            //case ViewState.门禁系统:
            //    OnDoorAccessToggleChange(false);
            //    UIPart.DoorAccessToggle.isOn = false;
            //    break;
            //case ViewState.人员定位:
            //    OnPersonalPositonToggleChange(false);
            //    UIPart.PersonalPositonToggle.isOn = false;
            //    break;
            //case ViewState.设备编辑:
            //    OnDeviceCreateToggleChange(false);
            //    UIPart.DeviceCreateToggle.isOn = false;
            //    break;
        }
        currentState = ViewState.None;
    }

    /// <summary>
    /// 绑定UI部分，相应的事件
    /// </summary>
    private void BindingUIMethod()
    {
        if(UIPart==null)
        {
            Debug.LogError("ControlMenuUI is null,please check!");
            return;
        }
        UIPart.DeviceCreateToggle.onValueChanged.AddListener(OnDeviceCreateToggleChange);
        UIPart.DeviceLoacation.onValueChanged.AddListener(OnDeviceLocationToggleChange);
        UIPart.VideoMonitorToggle.onValueChanged.AddListener(OnVideoMonitorToggleChange);
        UIPart.DoorAccessToggle.onValueChanged.AddListener(OnDoorAccessToggleChange);
        UIPart.PersonalPositonToggle.onValueChanged.AddListener(OnPersonalPositonToggleChange);
        UIPart.SearchToggle.onValueChanged.AddListener(OnSearchToggleChange);
        UIPart.AlarmToggle.onValueChanged.AddListener(OnAlarmToggleChange);
        UIPart.MultHistoryToggle.onValueChanged.AddListener(OnMultHistoryToggleChange);
    }
    /// <summary>
    /// 设备编辑
    /// </summary>
    /// <param name="isOn"></param>
    public void OnDeviceCreateToggleChange(bool isOn)
    {
        if(isOn)
        {
            //if (ViewState.设备编辑 == CurrentState) return;
            //CurrentState = ViewState.设备编辑;
            Debug.Log("Close TopoTree!");
            HideUIOnDevEditStart();
            ObjectAddListManage.Instance.Show();
            ObjectsEditManage.Instance.OpenDevEdit();
            //Todo:打开拖拽缩放功能
        }
        else
        {
            ShowUIOnDevEditEnd();
            Debug.Log("ShowTopoTree!");
            ObjectAddListManage.Instance.Hide();
            ObjectsEditManage.Instance.CloseDevEdit();
        }
    }
    public void OnDeviceLocationToggleChange(bool ison)
    {
        if(ison)
        {
            if (ViewState.设备定位 == CurrentState) return;
            CurrentState = ViewState.设备定位;
            TopoTreeManager.Instance.ShowWindow();
        }
        else
        {
            TopoTreeManager.Instance.CloseWindow();
        }
    }
    /// <summary>
    /// 视屏监控
    /// </summary>
    /// <param name="isOn"></param>
    public void OnVideoMonitorToggleChange(bool isOn)
    {
        if (isOn)
        {
            //if (ViewState.视屏监控 == CurrentState) return;
            //CurrentState = ViewState.视屏监控;

        }
        else
        {

        }
    }
    /// <summary>
    /// 门禁系统
    /// </summary>
    /// <param name="isOn"></param>
    public void OnDoorAccessToggleChange(bool isOn)
    {
        if (isOn)
        {
            //if (ViewState.门禁系统 == CurrentState) return;
            //CurrentState = ViewState.门禁系统;

        }
        else
        {

        }
    }
    /// <summary>
    /// 人员定位
    /// </summary>
    /// <param name="isOn"></param>
    public void OnPersonalPositonToggleChange(bool isOn)
    {
        if (isOn)
        {
            if (ViewState.人员定位 == CurrentState) return;
            CurrentState = ViewState.人员定位;
            Debug.Log("开启人员定位！");
            ShowLocation();
            SetMultHistoryToggle(true);
        }
        else
        {
            Debug.Log("关闭人员定位！");
            HideAndClearLocation();
            SetMultHistoryToggle(false);
        }
    }

    /// <summary>
    /// 展示人员定位
    /// </summary>
    public void ShowLocation()
    {
        LocationManager.Instance.ShowLocation();
        //MonitorRangeManager.Instance.ShowRanges();
        LocationUIManage.Instance.Show();
        PersonnelTreeManage.Instance.ShowWindow();
        SmallMapController.Instance.Show();
    }

    /// <summary>
    /// 关闭并清除人员定位
    /// </summary>
    public void HideAndClearLocation()
    {
        LocationManager.Instance.RecoverBeforeFocusAlign();
        LocationManager.Instance.HideAndClearLocation();
        //MonitorRangeManager.Instance.ClearRanges();
        LocationUIManage.Instance.Hide();
        LocationHistoryManager.Instance.ClearHistoryPaths();
        //LocationManager.Instance.ClearCharacter();
        PersonnelTreeManage.Instance.CloseWindow();
        HistoryPlayUI.Instance.SetWindowActive(false);
        SmallMapController.Instance.Show();
    }

    /// <summary>
    /// 关闭人员定位
    /// </summary>
    public void HideLocation()
    {
        LocationManager.Instance.HideLocation();
        //MonitorRangeManager.Instance.ClearRanges();
        LocationUIManage.Instance.Hide();
        LocationHistoryManager.Instance.ClearHistoryPaths();
        //LocationManager.Instance.ClearCharacter();
        PersonnelTreeManage.Instance.CloseWindow();
        SmallMapController.Instance.Hide();
    }

    /// <summary>
    /// 搜索(设备、告警、人员等)
    /// </summary>
    /// <param name="isOn"></param>
    public void OnSearchToggleChange(bool isOn)
    {
        Debug.Log("Show search monitor:"+ isOn);
    }
    /// <summary>
    /// 告警列表
    /// </summary>
    /// <param name="isOn"></param>
    public void OnAlarmToggleChange(bool isOn)
    {

    }
    /// <summary>
    /// 关闭菜单
    /// </summary>
    public void Show()
    {
        Window.SetActive(true);
    }
    /// <summary>
    /// 显示菜单
    /// </summary>
    public void Hide()
    {
        Window.SetActive(false);
    }
    /// <summary>
    /// 隐藏UI在设备编辑状态下 
    /// </summary>
    private void HideUIOnDevEditStart()
    {
        if(TopoTreeManager.Instance) TopoTreeManager.Instance.CloseWindow();
        if (PersonnelTreeManage.Instance) PersonnelTreeManage.Instance.CloseWindow();
        if (SmallMapController.Instance) SmallMapController.Instance.Hide();       
    }
    /// <summary>
    /// 显示UI在设备编辑状态下 
    /// </summary>
    private void ShowUIOnDevEditEnd()
    {
        if (currentState==ViewState.设备定位||currentState==ViewState.None)
        {
            if(TopoTreeManager.Instance&&UIPart.DeviceCreateToggle.isOn)
            TopoTreeManager.Instance.ShowWindow();
        }
        if (currentState==ViewState.人员定位 && UIPart.PersonalPositonToggle.isOn)
        {
            if(PersonnelTreeManage.Instance)
            PersonnelTreeManage.Instance.ShowWindow();
        }
        if (SmallMapController.Instance) SmallMapController.Instance.Show();
    }

    /// <summary>
    /// 设置多人历史Toggle，显示隐藏
    /// </summary>
    public void SetMultHistoryToggle(bool isActive)
    {
        if (isActive)
        {
            UIPart.MultHistoryToggle.gameObject.SetActive(true);
        }
        else
        {
            UIPart.MultHistoryToggle.gameObject.SetActive(false);
            UIPart.MultHistoryToggle.isOn = false;
        }
    }

    private void OnMultHistoryToggleChange(bool ison)
    {
        if (ison)
        {
            MultHistoryPlayUI.Instance.ShowT();
        }
        else
        {
            MultHistoryPlayUI.Instance.Hide();
        }
    }
}
