using MonitorRange;
using System.Collections;
using System.Collections.Generic;
using TwoTicketSystem;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 视图模式
/// </summary>
public enum ViewState
{
    None,
    设备定位,
    //视屏监控,
    //门禁系统,
    人员定位,
    //设备编辑
    两票系统,
    移动巡检
}
public class ActionBarManage : MonoBehaviour
{

    public static ActionBarManage Instance;
    /// <summary>
    /// 设备定位
    /// </summary>
    public Toggle DevToggle;
    
    /// <summary>
    /// 人员定位按钮
    /// </summary>
    public Toggle PersonnelToggle;
   
    /// <summary>
    /// 两票系统按钮
    /// </summary>
    public Toggle TwoVotesToggle;
    
    /// <summary>
    /// 移动巡检
    /// </summary>
    public Toggle MobilePatrolToggle;
     
  
    public GameObject   AlamText;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject Window;

    void Start()
    {
        Instance = this;
        InitToggleMethod();
        SceneEvents.FullViewStateChange += Instance_OnViewChange; ;
    }
    void OnDestroy()
    {
        SceneEvents.FullViewStateChange -= Instance_OnViewChange; ;
    }
   
    #region 控制栏方法部分
    /// <summary>
    /// 显示控制栏
    /// </summary>
    public void Show()
    {
        Window.SetActive(true);
    }
    /// <summary>
    /// 隐藏控制栏
    /// </summary>
    public void Hide()
    {
        Window.SetActive(false);
    }
    /// <summary>
    /// 进入首页
    /// </summary>
    /// <param name="isFullView"></param>
    private void Instance_OnViewChange(bool isFullView)
    {
        //throw new System.NotImplementedException();
        if (isFullView)
        {
            RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);
            ExitCurrentMode();
        }
        else
        {
            // DevToggle.isOn = true;
            // OnDevToggleChange(true);
            PersonnelToggle.isOn = true;
            OnPersonnelToggleChange(true);
            FunctionSwitchBarManage.Instance.SetWindow(true);
        }
    }
    /// <summary>
    /// 当前视图模式
    /// </summary>
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
    /// <summary>
    /// 绑定Toggle触发方法
    /// </summary>
    private void InitToggleMethod()
    {
        DevToggle.onValueChanged.AddListener(OnDevToggleChange);
        PersonnelToggle.onValueChanged.AddListener(OnPersonnelToggleChange);
        TwoVotesToggle.onValueChanged.AddListener(OnTwoVotesToggleChange);
        MobilePatrolToggle.onValueChanged.AddListener(OnMobileInspectionToggleChange);
    }
    private void ExitCurrentMode()
    {
        switch (currentState)
        {
            case ViewState.设备定位:
                OnDevToggleChange(false);
                DevToggle.isOn = false;
                break;
            case ViewState.人员定位:
                OnPersonnelToggleChange(false);
                PersonnelToggle.isOn = false;
                break;
            case ViewState.两票系统:
                OnTwoVotesToggleChange(false);
                TwoVotesToggle.isOn = false;
                break;
            case ViewState.移动巡检:
                OnMobileInspectionToggleChange(false);
                MobilePatrolToggle.isOn = false;
                break;
        }
        currentState = ViewState.None;
    }

    /// <summary>
    /// 设施模式切换
    /// </summary>
    /// <param name="isOn"></param>
    public void OnDevToggleChange(bool isOn)
    {
        ChangeImage(isOn, DevToggle);
        if (isOn)
        {
            if (ViewState.设备定位 == CurrentState) return;
            CurrentState = ViewState.设备定位;
            TopoTreeManager.Instance.ShowWindow();
            PersonnelTreeManage.Instance.CloseWindow();
            HideAndClearLocation();
        }
        else
        {
            TopoTreeManager.Instance.CloseWindow();

            DevSubsystemManage.Instance.ExitDevSubSystem();
            SmallMapController.Instance.Hide();
            //if(FactoryDepManager.currentDep!=null)RoomFactory.Instance.FocusNode(FactoryDepManager.currentDep);
        }
        DevSubsystemManage.Instance.DevSubsystemUI(isOn);
    }


    /// <summary>
    /// 人员定位
    /// </summary>
    /// <param name="isOn"></param>
    public void OnPersonnelToggleChange(bool isOn)
    {
        ChangeImage(isOn, PersonnelToggle);
        //FunctionSwitchBarManage.Instance.ShowSetWindow(isOn);
        SetVideoMonitorFollow(isOn);
        if (isOn)
        {
            if (ViewState.人员定位 == CurrentState) return;
            CurrentState = ViewState.人员定位;
            Debug.Log("开启人员定位！");
            ShowLocation();
            if (CameraGizmoFactory.Instance) CameraGizmoFactory.Instance.Show();
        }
        else
        {
            Debug.Log("关闭人员定位！");
            HideAndClearLocation();

            PersonSubsystemManage.Instance.ExitDevSubSystem();
            HistoryPlayUI.Instance.Hide();
            //if (SceneEvents.DepNode.depType != DepType.Factory)
            //{
            //    StartOutManage.Instance.SetUpperStoryButtonActive(true);
            //}           
            SmallMapController.Instance.Hide();
            if (CameraGizmoFactory.Instance) CameraGizmoFactory.Instance.Close();
        }
        if(PersonSubsystemManage.Instance)
        PersonSubsystemManage.Instance.PersonSubsystemUI(isOn);
        FunctionSwitchBarManage.Instance.SetAlarmAreaToggleActive(isOn);
    }

    public WorkTicketHistoryDetailsUI workTicketHistoryDetailsUI;
    public OperationTicketHistoryDetailsUI operationTicketHistoryDetailsUI;
    public void OnTwoVotesToggleChange(bool isOn)
    {
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn);
        ChangeImage(isOn, TwoVotesToggle);
        if (isOn)
        {
            if (ViewState.两票系统 == CurrentState) return;
            CurrentState = ViewState.两票系统;
            //AlamText.SetActive(true);
            //AlamText.GetComponent <Text >().text = "此功能没有被开放";
            PersonnelTreeManage.Instance.CloseWindow();
            //HideAndClearLocation();
            //if (TwoTicketSystemUI.Instance)
            //{
            //    TwoTicketSystemUI.Instance.Show();
            //}

            TwoTicketSystemUI_N.Instance.Show();
        }
        else
        {
            AlamText.SetActive(false );
            //if (TwoTicketSystemUI.Instance)
            //{
            //    TwoTicketSystemUI.Instance.Hide();
            //}
            TwoTicketSystemUI_N.Instance.Hide();
            workTicketHistoryDetailsUI.SetWindowActive(false);
            operationTicketHistoryDetailsUI.SetWindowActive(false);
            // TwoTicketSystemManage.Instance.HideDemo();
        }
        TwoTicketSystemSubBar.Instance.SetShoworHide(isOn);
        FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(isOn);
    }

    public void OnMobileInspectionToggleChange(bool isOn)
    {
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn);
        ChangeImage(isOn, MobilePatrolToggle);
        if (isOn)
        {
            if (ViewState.移动巡检 == CurrentState) return;
            CurrentState = ViewState.移动巡检;
            //AlamText.SetActive(true);
            //AlamText.GetComponent<Text>().text = "此功能没有被开放";
            PersonnelTreeManage.Instance.CloseWindow();
            //HideAndClearLocation();
            //if (MobileInspectionUI.Instance)
            //{
            //    MobileInspectionUI.Instance.SetWindowActive(true);
            //}
            if (MobileInspectionUI_N.Instance)
            {
                MobileInspectionUI_N.Instance.Show();
            }
        }
        else
        {
            AlamText.SetActive(false);
            MobileInspectionInfoFollow.Instance.Hide();
         //   MobileInspectionManage.Instance.HideDemo();
            //if (MobileInspectionUI.Instance)
            //{
            //    MobileInspectionUI.Instance.SetWindowActive(false);
            //}
            if (MobileInspectionUI_N.Instance)
            {
                MobileInspectionUI_N.Instance.Hide();
            }
        }

            //MobileInspectionUI.Instance.SetWindowActive(isOn);

        MobileInspectionSubBar.Instance.SetShoworHide(isOn);
        FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(isOn);
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
        //LocationHistoryManager.Instance.ClearHistoryPaths();
        //LocationManager.Instance.ClearCharacter();
        PersonnelTreeManage.Instance.CloseWindow();
        //HistoryPlayUI.Instance.SetWindowActive(false);
        SmallMapController.Instance.Show();
    }
    /// <summary>
    /// 设置是否显示摄像头漂浮UI
    /// </summary>
    /// <param name="isShow"></param>
    private void SetVideoMonitorFollow(bool isShow)
    {
        FollowTargetManage followManage = FollowTargetManage.Instance;
        if (followManage==null) return;
        if (isShow)
        {
            followManage.ShowCameraUI();
        }
        else
        {
            followManage.HideCameraUI();
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

}
