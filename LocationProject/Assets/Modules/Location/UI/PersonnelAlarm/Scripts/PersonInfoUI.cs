using DG.Tweening;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PersonInfoUIState
{
    Normal,//正常
    Standby,//待机
    StandbyLong,//长时间不动
    Leave//离开
}

public class PersonInfoUI : MonoBehaviour
{
    //public static PersonInfoUI Instance;
    /// <summary>
    /// 界面节点标志管理
    /// </summary>
    public PersonnelNodeManage personnelNodeManage;
    /// <summary>
    /// 人员信息数据
    /// </summary>
    [HideInInspector]
    public Personnel personnel;
    /// <summary>
    /// 定位卡的状态
    /// </summary>
    public PersonInfoUIState state = PersonInfoUIState.Normal;
    ///// <summary>
    ///// 窗体
    ///// </summary>
    //public GameObject window;
    /// <summary>
    /// 内容窗体
    /// </summary>
    public GameObject contentGrid;
    /// <summary>
    /// 告警项
    /// </summary>
    public GameObject alarmItem;
    /// <summary>
    /// 界面有下角告警显示图标
    /// </summary>
    public GameObject rightBottonAlarmIcon;

    public Image photo;//头像

    public Text txtName;//名字
    public Text txtJob;//岗位
    public Text txtAlarmArea;//告警区域
    public Text txtArea;//区域

    public GameObject phoneBtn;//拨打电话按钮
    public GameObject SMSBtn;//短信通知
    public GameObject EntranceGuardBtn;//经过门禁
    public GameObject historyBtn;//历史轨迹按钮
                                 // public Button historyBtn;//历史轨迹按钮
    public GameObject videomonitorBtn;//视频监控按钮

    private Tweener contentGridTweener;//内容列表动画
    private CanvasGroup contentGridCanvasGroup;//内容列表CanvasGroup

    public GameObject TagName;//悬浮标志

    private UGUIFollowTarget uguifollow;//ui跟随组件
    [HideInInspector]
    public LocationObject locationObj;//人员
    public Text infoStandbyTime;//待机时间
    public Text nameStandbyTime;//待机时间

    void Start()
    {
        //Instance = this;
        CreateContentGridTweener();
        historyBtn.GetComponent<Button>().onClick.AddListener(HistoryBtn_OnClick);
        EntranceGuardBtn.GetComponent<Button>().onClick.AddListener(EntranceGuardBtn_OnClick);
        videomonitorBtn.GetComponent<Button>().onClick.AddListener(VideoMonitorBtn_OnClick);
        ButtonGroup();
    }

    // Update is called once per frame
    void Update()
    {
        //if (contentGrid.activeInHierarchy)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        LocationManager.Instance.SetUIGraphicRaycaster(true);
        //    }

        //    if (Input.GetMouseButtonUp(0))
        //    {
        //        LocationManager.Instance.SetUIGraphicRaycaster(false);
        //    }
        //}
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Personnel personnelT, LocationObject objT)
    {
        //Log.Info("PersonInfoUI.Init", "name:" + personnelT.Name);
        personnel = personnelT;
        locationObj = objT;
        if (uguifollow == null)
        {
            uguifollow = GetComponent<UGUIFollowTarget>();
        }
        if (personnel == null)
        {
            Log.Alarm("PersonInfoUI.Init", "personnel == null");
            return;
        }

        txtName.text = personnel.Name;
        TagName.GetComponentInChildren<Text>(true).text = personnel.Name;
        txtJob.text = "(" + personnel.Pst + ")";
        //SetPhoto(personnelT.Sex);

        gameObject.name = personnel.Name;
        if (personnel.Tag != null)
        {
            gameObject.name += personnel.Tag.Code;
        }

        LocationUIManage.Instance.SetPhoto(photo, personnelT.Sex);
    }

    /// <summary>
    /// 显示告警
    /// </summary>
    [ContextMenu("ShowAlarm")]
    public void ShowAlarm()
    {
        personnelNodeManage.ShowAlarm();
        SetAlarmSameUI(true);
    }

    /// <summary>
    /// 显示正常界面
    /// </summary>
    [ContextMenu("ShowNormal")]
    public void ShowNormal()
    {
        personnelNodeManage.ShowNormal();
        SetAlarmSameUI(false);
    }

    /// <summary>
    /// 显示隐藏告警相关UI
    /// </summary>
    /// <param name="b"></param>
    public void SetAlarmSameUI(bool b)
    {
        alarmItem.SetActive(b);
        rightBottonAlarmIcon.SetActive(b);
    }

    /// <summary>
    /// 单击Toggle按钮去触发，设置内容是否激活
    /// </summary>
    public void SetContentGridActive(bool b)
    {
        if (contentGrid.activeInHierarchy == b)
        {
            //LocationManager.Instance.RecoverBeforeFocusAlign();
            return;
        }
        //contentGrid.SetActive(b);

        if (b)
        {
            ShowDisplayUI();

        }
        else
        {
            HideDisplayUI();
            if (PersonSubsystemManage.Instance.IsHistorical == false)
            {
                ParkInformationManage.Instance.ShowParkInfoUI(true);
            }
            LocationManager.Instance.RecoverBeforeFocusAlign();

        }
    }

    /// <summary>
    /// 显示展示信息UI
    /// </summary>
    private void ShowDisplayUI()
    {
        if (uguifollow == null)
        {
            uguifollow = GetComponent<UGUIFollowTarget>();
        }
        uguifollow.SetIsUp(true);

        ContentGrid_PlayForward();
        //LocationManager.Instance.FocusPerson((int)personnel.TagId);
        SetTagName(false);

        StartOutManage.Instance.SetUpperStoryButtonActive(false);
        StartOutManage.Instance.ShowBackButton(()
            =>
        {
            LocationManager.Instance.HideCurrentPersonInfoUI();
            //StartOutManage.Instance.HideBackButton();
            if (LocationManager.Instance.IsFocus)
            {
                LocationManager.Instance.RecoverBeforeFocusAlign();
            }
        });
    }

    /// <summary>
    /// 关闭展示信息UI
    /// </summary>
    private void HideDisplayUI()
    {
        uguifollow.SetIsUp(false);
        SetTagName(true);
        ContentGrid_PlayBackwards();

        if (SceneEvents.DepNode.depType != DepType.Factory)//厂区
        {
            StartOutManage.Instance.SetUpperStoryButtonActive(true);
        }

        StartOutManage.Instance.HideBackButton();
    }

    private void SetTagName(bool b)
    {
        TagName.SetActive(b);
    }

    /// <summary>
    /// 设置Toggle
    /// </summary>
    public void SetContentToggle(bool isOn)
    {
        if (personnelNodeManage.personnelAlarmNode)
        {
            personnelNodeManage.personnelAlarmNode.AlarmToggle.isOn = isOn;
        }
        if (personnelNodeManage.personnelNormalNode)
        {
            personnelNodeManage.personnelNormalNode.PersonnelToggle.isOn = isOn;
        }
    }

    ///// <summary>
    ///// 设置Toggle.ison去关闭HideContentGrid
    ///// </summary>
    //public void HideContentGrid()
    //{
    //    if (personnelNodeManage.personnelAlarmNode)
    //    {
    //        personnelNodeManage.personnelAlarmNode.AlarmToggle.isOn = false;
    //    }
    //    if (personnelNodeManage.personnelNormalNode)
    //    {
    //        personnelNodeManage.personnelNormalNode.PersonnelToggle.isOn = false;
    //    }
    //}

    ///// <summary>
    ///// 设置窗体是否激活
    ///// </summary>
    //public void SetWindowActive(bool b)
    //{
    //    if (window.activeInHierarchy && b) return;
    //    window.SetActive(b);

    //}

    /// <summary>
    /// 设置告警区域名称
    /// </summary>
    /// <param name="nameT"></param>
    public void SetTxtAlarmAreaName(string nameT)
    {
        txtAlarmArea.text = nameT;
    }

    /// <summary>
    /// 设置区域名称
    /// </summary>
    public void SetTxtAreaName(string nameT)
    {
        txtArea.text = nameT;
    }

    /// <summary>
    /// 创建内容列表打开动画
    /// </summary>
    public void CreateContentGridTweener()
    {
        if (contentGridTweener == null)
        {
            contentGridCanvasGroup = contentGrid.GetComponent<CanvasGroup>();
            contentGridTweener = contentGridCanvasGroup.DOFade(1, 0.3f);
            contentGridTweener.SetAutoKill(false);
            contentGridTweener.Pause();
            contentGridTweener.OnComplete(() => { contentGrid.SetActive(true); });
            contentGridTweener.OnRewind(() => { contentGrid.SetActive(false); });

        }
    }

    /// <summary>
    /// 内容列表打开动画执行
    /// </summary>
    public void ContentGrid_PlayForward()
    {
        CreateContentGridTweener();
        contentGridTweener.PlayForward();

        contentGridCanvasGroup.interactable = true;
        contentGridCanvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// 内容列表关闭动画执行
    /// </summary>
    public void ContentGrid_PlayBackwards()
    {
        CreateContentGridTweener();
        contentGridTweener.PlayBackwards();

        contentGridCanvasGroup.interactable = false;
        contentGridCanvasGroup.blocksRaycasts = false;
    }

    ///// <summary>
    ///// 打开人员历史轨迹控制界面
    ///// </summary>
    //public void HistoryBtn_OnClick()
    //{
    //    HistoryPlayUI.Instance.Show(personnel);
    //    LocationManager.Instance.EnterHistory_One();
    //    //ControlMenuController.Instance.HideLocation();
    //    HideLocation();
    //}

    /// <summary>
    /// 打开人员历史轨迹控制界面
    /// </summary>
    public void HistoryBtn_OnClick()
    {
        HistoryPlayUI.Instance.Show(personnel);
        LocationManager.Instance.EnterHistory_One();
        //ControlMenuController.Instance.HideLocation();
        HideLocation();
    }
    /// <summary>
    /// 经过门禁控制界面
    /// </summary>
    public void EntranceGuardBtn_OnClick()
    {
        AfterEntranceGuardManage.Instance.ShowWindow();
        AfterEntranceGuardManage.Instance.GetEntranceGuardData(personnel.Id);
    }

    /// <summary>
    /// 显示附近监控
    /// </summary>
    public void VideoMonitorBtn_OnClick()
    {
      
        
        NearPersonnelCameraManage.Instance.Personnel.text = personnel.Name.ToString();
        if (personnel.AreaName == null)
        {
            NearPersonnelCameraManage.Instance.CurrentArea.text = "厂区内";
        }
        else
        {
            NearPersonnelCameraManage.Instance.CurrentArea.text = personnel.AreaName.ToString();
        }

        NearPersonnelCameraManage.Instance.ShowNearPersonnelCameraWindow();
        
        NearPersonnelCameraManage.Instance. isRefresh = false;
        StartRefershNearPersonnelCameraData();

    }
    public void StartRefershNearPersonnelCameraData()
    {
        if (!IsInvoking("RefershNearPersonnelCameraData"))
        {
            InvokeRepeating("RefershNearPersonnelCameraData",0, CommunicationObject.Instance.NearCameraRefreshInterval);
        }
    }
    public void RefershNearPersonnelCameraData()
    {
        NearPersonnelCameraManage.Instance.SaveSelection();
        float dis = 250;
       // Debug.LogError("蔡露露");
        Vector3 PerRotion = new Vector3(locationObj.transform.eulerAngles.x, locationObj.transform.eulerAngles.z, locationObj.transform.eulerAngles.y);
        NearPersonnelCameraManage.Instance.PersonnelRotation.GetComponent<RectTransform>().localEulerAngles = PerRotion;
        NearPersonnelCameraManage.Instance.GetNearPerCamData(personnel.Id, dis, 1);
       // Debug.LogError("caixulu");
    }
    public void CloseRefershNearPersonnelCameraData()
    {
        if (IsInvoking("RefershNearPersonnelCameraData"))
        {
            CancelInvoke("RefershNearPersonnelCameraData");
        }
    }
    /// <summary>
    /// 关闭定位相关功能
    /// </summary>
    public void HideLocation()
    {
        LocationManager.Instance.HideLocation();
        LocationUIManage.Instance.Hide();
        LocationHistoryManager.Instance.ClearHistoryPaths();
        //LocationManager.Instance.ClearCharacter();
        PersonnelTreeManage.Instance.CloseWindow();
        SmallMapController.Instance.Hide();
    }
    /// <summary>
    /// 改变按钮颜色
    /// </summary>
    /// <param name="button"></param>
    public void ChangeButtonColor(GameObject button)
    {
        EventTriggerListener colorButton = EventTriggerListener.Get(button);
        colorButton.onEnter = CheckButton;
        colorButton.onExit = NormalButton;

        colorButton.onDown = PressedButton;
        colorButton.onUp = NormalButton;
    }

    /// <summary>
    /// 点击按钮颜色
    /// </summary>
    /// <param name="button"></param>
    public void CheckButton(GameObject button)
    {
        Color CheckColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
        button.GetComponentInChildren<Image>(true).color = CheckColor;
        button.GetComponentInChildren<Text>(true).color = CheckColor;
    }
    /// <summary>
    /// 不点击颜色
    /// </summary>
    /// <param name="button"></param>
    public void NormalButton(GameObject button)
    {
        Color NormalColor = new Color(108 / 255f, 237 / 255f, 253 / 255f, 255 / 255f);
        button.GetComponentInChildren<Image>(true).color = NormalColor;
        button.GetComponentInChildren<Text>(true).color = NormalColor;
    }
    /// <summary>
    /// 点击后的颜色
    /// </summary>
    /// <param name="button"></param>
    public void PressedButton(GameObject button)
    {
        Color PressedColor = new Color(108 / 255f, 237 / 255f, 253 / 255f, 204 / 255f);
        button.GetComponentInChildren<Image>(true).color = PressedColor;
        button.GetComponentInChildren<Text>(true).color = PressedColor;
    }
    /// <summary>
    /// Button组
    /// </summary>
    public void ButtonGroup()
    {
        ChangeButtonColor(phoneBtn);
        ChangeButtonColor(SMSBtn);
        ChangeButtonColor(videomonitorBtn);
        ChangeButtonColor(historyBtn);
        ChangeButtonColor(EntranceGuardBtn);
    }

    /// <summary>
    /// 设置历史按钮的显示和关闭
    /// </summary>
    public void SetHistoryButton(bool isActive)
    {
        historyBtn.SetActive(isActive);
    }

    /// <summary>
    /// 设置信息界面的展开或关闭,
    /// </summary>
    /// <param name="b"></param>
    public void SetOpenOrClose(bool b)
    {
        //SetContentGridActive(b);
        if (b)
        {
            ShowDisplayUI();
        }
        else
        {
            HideDisplayUI();
        }
        SetContentToggle(b);
    }

    /// <summary>
    /// 显示待机时间
    /// </summary>
    public void ShowStandByTime()
    {
        TimeSpan time = DateTime.Now - LocationManager.GetTimestampToDateTime(locationObj.tagPosInfo.Time);
        //infoStandbyTime.text = time.TotalSeconds.ToString();
        //nameStandbyTime.text = "(" + time.TotalSeconds.ToString() + ")";

        //infoStandbyTime.text = "已待机";
        //nameStandbyTime.text = "(" + time.TotalSeconds.ToString() + ")";

        if (time.TotalSeconds < 60)//如果小于1分钟显示秒
        {
            infoStandbyTime.text = "(" + Math.Round(time.TotalSeconds, 0).ToString() + "秒)";
        }
        else if (time.TotalSeconds < 3600)//如果小于1小时显示分钟和秒
        {
            infoStandbyTime.text = "(" + time.Minutes.ToString() + "分" + time.Seconds + "秒)";
        }
        else if (time.TotalSeconds < 3600 * 24)//如果小于天显示小时和分钟
        {
            infoStandbyTime.text = "(" + time.Hours.ToString() + "时" + time.Minutes.ToString() + "分)";
        }
        else if (time.TotalSeconds < 3600 * 24 * 365) //如果大于一天显示天和小时
        {
            infoStandbyTime.text = "(" + time.Days.ToString() + "天" + time.Hours.ToString() + "时)";
        }
        else
        {
            infoStandbyTime.text = "(" + Math.Round(time.TotalDays, 0).ToString() + "天)";
        }
        infoStandbyTime.text = "待机" + infoStandbyTime.text;
        nameStandbyTime.text = infoStandbyTime.text;

        infoStandbyTime.gameObject.SetActive(true);
        nameStandbyTime.gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭显示待机时间
    /// </summary>
    public void HideStandByTime()
    {
        infoStandbyTime.gameObject.SetActive(false);
        nameStandbyTime.gameObject.SetActive(false);
    }
}
