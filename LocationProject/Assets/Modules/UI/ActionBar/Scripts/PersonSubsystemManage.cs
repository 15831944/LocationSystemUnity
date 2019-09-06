using MonitorRange;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Types = Location.WCFServiceReferences.LocationServices.AreaTypes;

public class PersonSubsystemManage : MonoBehaviour
{
    public static PersonSubsystemManage Instance;
    /// <summary>
    /// 搜索
    /// </summary>
    public Toggle SearchToggle;

    /// <summary>
    /// 人员定位告警
    /// </summary>
    public Toggle PersonnelAlamToggle;


    /// <summary>
    /// 历史路径
    /// </summary>
    public Toggle HistoricalToggle;

    /// <summary>
    /// 编辑监控区域
    /// </summary>
    public Toggle EditAreaToggle;

    /// <summary>
    /// 设置基准点
    /// </summary>
    public Toggle BenchmarkingToggle;

    /// <summary>
    /// 坐标系设置
    /// </summary>
   // public Toggle CoordinateToggle;
    /// <summary>
    /// 人员定位功能的子系统
    /// </summary>
    public GameObject PersonSubsystem;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

        InitToggleMethod();
        SceneEvents.DepNodeChanged += SceneEvents_DepNodeChanged;
        ToggleAuthoritySet();
    }

    private void SceneEvents_DepNodeChanged(DepNode arg1, DepNode arg2)
    {
        DepNode depNodeT = GetDepType(arg2);
        if (depNodeT == null)
        {
            SetMultHistoryToggleActive(true);

            LocationManager.Instance.SetPersonInfoHistoryUI(true);
            return;
        }
        //throw new System.NotImplementedException();
        //在配置文件InitInfo.xml，区域类型设置为根节点或园区节点，否则在是否可以展示多人历史记录会有点问题
        if (depNodeT.TopoNode == null)
        {
            Debug.LogError("PersonSubsystemManage.SceneEvents_DepNodeChanged depNodeT.TopoNode == null:" + arg1 + "," + arg2);
            return;
        }

        //if (depNodeT.TopoNode.Type == Types.园区)
        //{
        //    SetMultHistoryToggleActive(true);
        //    LocationManager.Instance.SetPersonInfoHistoryUI(true);
        //}
        //else
        //{
        //    SetMultHistoryToggleActive(false);

        //    LocationManager.Instance.SetPersonInfoHistoryUI(false);
        //}
    }

    /// <summary>
    /// 获取DepType,过滤分组之后的上一个节点
    /// </summary>
    /// <returns></returns>
    public DepNode GetDepType(DepNode depNodeT)
    {
        if (depNodeT == null || depNodeT.TopoNode == null) return depNodeT;
        if (depNodeT.TopoNode.Type != Types.分组)
        {
            return depNodeT;
        }
        else
        {
            return GetDepType(depNodeT.ParentNode);
        }
    }

    /// <summary>
    /// 退出子系统
    /// </summary>
    public void ExitDevSubSystem()
    {
        if (SearchToggle.isOn)
        {
            SearchToggle.isOn = false;
            OnSearchToggleChange(false);
        }
        else if (PersonnelAlamToggle.isOn)
        {
            PersonnelAlamToggle.isOn = false;
            OnPersonnelAlamToggleChange(false);
        }

        else if (HistoricalToggle.isOn)
        {
            HistoricalToggle.isOn = false;
            OnHistoricalToggleChange(false);
        }
        else if (EditAreaToggle.isOn)
        {
            EditAreaToggle.isOn = false;
            OnEditAreaToggleChange(false);
        }
        else if (BenchmarkingToggle.isOn)
        {
            BenchmarkingToggle.isOn = false;
            OnBenchmarkingToggleChange(false);
        }
        //else if (CoordinateToggle.isOn)
        //{
        //    CoordinateToggle.isOn = false;
        //}
    }
    /// <summary>
    /// UI绑定方法
    /// </summary>
    private void InitToggleMethod()
    {
        SearchToggle.onValueChanged.AddListener(OnSearchToggleChange);
        PersonnelAlamToggle.onValueChanged.AddListener(OnPersonnelAlamToggleChange);

        HistoricalToggle.onValueChanged.AddListener(OnHistoricalToggleChange);
        EditAreaToggle.onValueChanged.AddListener(OnEditAreaToggleChange);
        BenchmarkingToggle.onValueChanged.AddListener(OnBenchmarkingToggleChange);
       // CoordinateToggle.GetComponent<Toggle>().onValueChanged.AddListener(SetCoordinateConfiguration);

        //如果是访客模式，就关闭编辑功能
        if (CommunicationObject.Instance.IsGuest())
        {
            SetEditAreaToggleActive(false);
            SetCoordinateToggleActive(false);
        }
    }
    /// <summary>
    /// 搜索模式
    /// </summary>
    /// <param name="isOn"></param>
    public void OnSearchToggleChange(bool isOn)
    {
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        ChangeImage(isOn, SearchToggle);
        Debug.Log("OnSearchToggleChange:" + isOn);
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();
        //DataPaging.Instance.StartPerSearchUI();
        if (isOn)
        {
            ToggleChangedBefore();
            DataPaging.Instance.StartPerSearchUI();
            DataPaging.Instance.ShowpersonnelSearchWindow();
        }
        else
        {
            DataPaging.Instance.ClosepersonnelSearchWindow();
            DataPaging.Instance.CloseAllPerWindow();
        }
    }
    /// <summary>
    /// 人员告警
    /// </summary>
    /// <param name="isOn"></param>
    public void OnPersonnelAlamToggleChange(bool isOn)
    {
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        ChangeImage(isOn, PersonnelAlamToggle);
        Debug.Log("OnQueryToggleChange:" + isOn);
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();
        if (isOn)
        {
            ToggleChangedBefore();
            PersonnelAlarmList.Instance.ShowPersonAlarmUI();

        }
        else
        {
            PersonnelAlarmList.Instance.ClosePersonAlarmUI();
        }
    }
    public bool IsHistorical = false;
    /// <summary>
    /// 历史路径
    /// </summary>
    /// <param name="isOn"></param>
    private void OnHistoricalToggleChange(bool isOn)
    {
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        //IsHistorical = true;
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn);
        ChangeImage(isOn, HistoricalToggle);
        Debug.Log("OnHistoricalToggleChange:" + isOn);
        if (isOn)
        {
            AlarmPushManage.Instance.CloseAlarmPushWindow(false);
            IsHistorical = true;
            if (LocationManager.Instance.IsFocus)
            {
                LocationManager.Instance.RecoverBeforeFocusAlign(() =>
                {
                    OnHistoricalToggleTrue();

                }, false);
            }
            else {
                OnHistoricalToggleTrue();
            }
        }
        else
        {
            
            if (SceneAssetManager.Instance) SceneAssetManager.Instance.RecoverEnableLoadBuilding();
             AlarmPushManage.Instance.CloseAlarmPushWindow(true);
            //AlarmPushManage.Instance.IsShow.isOn = true ;
            ActionBarManage.Instance.Show();
            FunctionSwitchBarManage.Instance.SetWindow(true);
            MonitorRangeManager.Instance.ShowRanges(FactoryDepManager.currentDep);
            LocationHistoryUITool.Hide();
            if (BaoXinTitle.Instance) BaoXinTitle.Instance.Show();
            IsHistorical = false;
        }
        SetOtherUIStateInHistoricalMode(isOn);
    }
    /// <summary>
    /// 历史轨迹模式下，设置其他界面的显示和隐藏
    /// </summary>
    /// <param name="isOn"></param>
    private void SetOtherUIStateInHistoricalMode(bool isOn)
    {
        FunctionSwitchBarManage funcManage = FunctionSwitchBarManage.Instance;
        if (isOn)
        {
            if (funcManage)
            {
                if(funcManage.CameraToggle.ison) funcManage.CameraToggle.SetToggle(false);
                if(funcManage.BuildingToggle.ison) funcManage.BuildingToggle.SetToggle(false);
                if (funcManage.ArchorInfoToggle.ison) funcManage.ArchorInfoToggle.SetToggle(false);
            }
        }
    }
    private void OnHistoricalToggleTrue()
    {
        Log.Info("OnHistoricalToggleTrue");
        RoomFactory.Instance.FocusNode(FactoryDepManager.Instance, () =>
        {
            if(SceneAssetManager.Instance) SceneAssetManager.Instance.SaveEnableLoadBuilding(false);
             AlarmPushManage.Instance.CloseAlarmPushWindow(false);
            //AlarmPushManage.Instance.IsShow.isOn = false;
            ToggleChangedBefore();

            ActionBarManage.Instance.Hide();
            FunctionSwitchBarManage.Instance.SetWindow(false);
            MonitorRangeManager.Instance.HideAllRanges();
            LocationHistoryUITool.Show();
            AlarmPushManage.Instance.CloseAlarmPushWindow(false);
            if (AutoSelectHistoryPath.Instance)
            {
                AutoSelectHistoryPath.Instance.Select();
            }
            if (BaoXinTitle.Instance) BaoXinTitle.Instance.Close();
        });
    }

    /// <summary>
    /// 如果是编辑区域状态
    /// </summary>
    public bool IsOnEditArea;
    /// <summary>
    /// 编辑监控区域
    /// </summary>
    /// <param name="isOn"></param>
    private void OnEditAreaToggleChange(bool isOn)
    {
        if (isOn&&IsRangeMode())
        {
            Debug.LogError("当前处于非楼层模式");
            UGUIMessageBox.Show("当前处于区域层级!\n请点击左下角的返回上一层按钮，再进行区域编辑。");
            EditAreaToggle.isOn = false;
            return;
        }
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn);
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();
        ChangeImage(isOn, EditAreaToggle);
        Debug.Log("OnMonitoringToggleChange:" + isOn);
        IsOnEditArea = isOn;
        if (isOn)
        {           
            LocationManager.Instance.HideLocation();
            AlarmPushManage.Instance.CloseAlarmPushWindow(false);        
            ObjectsEditManage.Instance.SetEditorGizmoSystem(true);
            MonitorRangeManager.Instance.ShowAreaEdit(SceneEvents.DepNode);
            //FactoryDepManager.Instance.SetAllColliderEnable(false);
            FactoryDepManager.Instance.SetAllColliderIgnoreRaycastOP(true);
            PersonnelTreeManage.Instance.CloseWindow();
            SmallMapController.Instance.Hide();
            StartOutManage.Instance.Hide();
            FunctionSwitchBarManage.Instance.SetWindow(false);
            ActionBarManage.Instance.Hide();
            RangeEditWindow.Instance.Show();
        }
        else
        {
            LocationManager.Instance.ShowLocation();
             AlarmPushManage.Instance.CloseAlarmPushWindow(true);
        
            ObjectsEditManage.Instance.SetEditorGizmoSystem(false);
            MonitorRangeManager.Instance.ExitCurrentEditArea();
            //FactoryDepManager.Instance.SetAllColliderEnable(true);
            FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(false);
            PersonnelTreeManage.Instance.ShowWindow();
            SmallMapController.Instance.Show();
            StartOutManage.Instance.Show();
            FunctionSwitchBarManage.Instance.SetWindow(true);
            ActionBarManage.Instance.Show();
            RangeEditWindow.Instance.Hide();
        }
    }
    /// <summary>
    /// 是否聚焦楼层下区域
    /// </summary>
    /// <returns></returns>
    private bool IsRangeMode()
    {
        DepNode currentNode = FactoryDepManager.currentDep;
        if(currentNode!=null&&(currentNode is RangeController||currentNode is RoomController))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsOnBenchmarking;
    /// <summary>
    /// 基准点
    /// </summary>
    /// <param name="isOn"></param>
    private void OnBenchmarkingToggleChange(bool isOn)
    {
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
        ChangeImage(isOn, BenchmarkingToggle);
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();

        if (isOn)
        {
            IsOnBenchmarking = isOn;
            LocationCardManagement.Instance.GetLocationCardManagementData();
            LocationCardManagement.Instance.ShowLocationCardManagementWindow();
            ToggleChangedBefore();
        }
        else
        {
            LocationCardManagement.Instance.CloseLocationCardWindow();
            LocationCardManagement.Instance.CloseAllCardRoleWindow();

            IsOnBenchmarking = isOn;
        }
    }
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
    /// 坐标系设置
    /// </summary>
    /// <param name="Ison"></param>
    public void SetCoordinateConfiguration(bool Ison)
    {
      //  ChangeImage(Ison, CoordinateToggle);
        ModelAdjustManage.Instance.ShowWindow(Ison);
        ParkInformationManage.Instance.ClosePerAndDevAlarmWindow();
    }


    /// <summary>
    /// 设置多人历史Toggle是否选中
    /// </summary>
    public void SetMultHistoryToggle(bool isOnT)
    {
        if (isOnT)
        {
            HistoricalToggle.isOn = true;
        }
        else
        {
            HistoricalToggle.isOn = false;
        }
    }

    /// <summary>
    /// 设置搜索Toggle，显示隐藏
    /// </summary>
    public void SetSearchToggleActive(bool isActiveT)
    {
        SearchToggle.transform.parent.gameObject.SetActive(isActiveT);
    }

    /// <summary>
    /// 设置人员告警Toggle，显示隐藏
    /// </summary>
    public void SetPersonnelAlamToggleActive(bool isActiveT)
    {
        PersonnelAlamToggle.transform.parent.gameObject.SetActive(isActiveT);
    }

    /// <summary>
    /// 设置多人历史Toggle，显示隐藏
    /// </summary>
    public void SetMultHistoryToggleActive(bool isActiveT)
    {
        HistoricalToggle.transform.parent.gameObject.SetActive(isActiveT);
    }

    /// <summary>
    /// 设置多人历史Toggle是否选中
    /// </summary>
    public void SetEditAreaToggle(bool isOnT)
    {
        if (isOnT)
        {
            EditAreaToggle.isOn = true;
        }
        else
        {
            EditAreaToggle.isOn = false;
        }
    }

    /// <summary>
    /// 设置编辑区域Toggle，显示隐藏
    /// </summary>
    public void SetEditAreaToggleActive(bool isActiveT)
    {
        EditAreaToggle.transform.parent.gameObject.SetActive(isActiveT);
    }

    /// <summary>
    /// 设置基准点Toggle，显示隐藏
    /// </summary>
    public void SetBenchmarkingToggleActive(bool isActiveT)
    {
        BenchmarkingToggle.transform.parent.gameObject.SetActive(isActiveT);
    }

    /// <summary>
    /// 设置坐标系设置Toggle，显示隐藏
    /// </summary>
    public void SetCoordinateToggleActive(bool isActiveT)
    {
      //  CoordinateToggle.transform.parent.gameObject.SetActive(isActiveT);
    }

    /// <summary>
    /// 设置所有的Toggle是否隐藏
    /// </summary>
    public void SetAllToggleActive(bool isActiveT)
    {
        SetSearchToggleActive(isActiveT);
        SetPersonnelAlamToggleActive(isActiveT);
        SetMultHistoryToggleActive(isActiveT);
        SetEditAreaToggleActive(isActiveT);
        SetBenchmarkingToggleActive(isActiveT);
        SetCoordinateToggleActive(isActiveT);
    }

    /// <summary>
    /// 人员定位功能的子系统
    /// </summary>
    public void PersonSubsystemUI(bool B)
    {
        if (B)
        {
            PersonSubsystem.SetActive(true);
        }
        else
        {
            PersonSubsystem.SetActive(false);
        }
    }

    /// <summary>
    /// 在点击人定位子工具栏之前要做的操作
    /// </summary>
    public void ToggleChangedBefore()
    {
        //HistoryPlayUI.Instance.Hide();
    }
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            SearchToggle.transform.GetComponentInParent<ControlMenuToolTip>().TipContent = "搜索";
        }
        else
        {
            SearchToggle.transform.GetComponentInParent<ControlMenuToolTip>().TipContent = "人员管理";
        }
    }
}
