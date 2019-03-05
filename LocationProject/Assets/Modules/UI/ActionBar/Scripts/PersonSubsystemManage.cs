using MonitorRange;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Types = Location.WCFServiceReferences.LocationServices.AreaTypes;

public class PersonSubsystemManage : MonoBehaviour {
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
    public Toggle    CoordinateToggle;
    /// <summary>
    /// 人员定位功能的子系统
    /// </summary>
    public GameObject PersonSubsystem;

    void Start () {
        Instance = this;
        InitToggleMethod();
        SceneEvents.DepNodeChanged += SceneEvents_DepNodeChanged;
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
        if (depNodeT.TopoNode.Type == Types.园区)
        {
            SetMultHistoryToggleActive(true);
            LocationManager.Instance.SetPersonInfoHistoryUI(true);
        }
        else
        {
            SetMultHistoryToggleActive(false);
       
            LocationManager.Instance.SetPersonInfoHistoryUI(false);
        }
    }

    /// <summary>
    /// 获取DepType,过滤分组之后的上一个节点
    /// </summary>
    /// <returns></returns>
    public DepNode GetDepType(DepNode depNodeT)
    {
        if (depNodeT == null) return depNodeT;
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
        else if(EditAreaToggle.isOn)
        {
            EditAreaToggle.isOn = false;
            OnEditAreaToggleChange(false);
        }else if (BenchmarkingToggle.isOn)
        {
            BenchmarkingToggle.isOn = false;
            OnBenchmarkingToggleChange(false);
        }
        else if (CoordinateToggle.isOn )
        {
            CoordinateToggle.isOn = false;
        }
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
        CoordinateToggle.GetComponent <Toggle >().onValueChanged.AddListener(SetCoordinateConfiguration);
    }
    /// <summary>
    /// 搜索模式
    /// </summary>
    /// <param name="isOn"></param>
    public void OnSearchToggleChange(bool isOn)
    {
        ChangeImage(isOn ,SearchToggle );
        Debug.Log("OnSearchToggleChange:" + isOn);
      
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
        }
    }
    /// <summary>
    /// 人员告警
    /// </summary>
    /// <param name="isOn"></param>
    public void OnPersonnelAlamToggleChange(bool isOn)
    {
        ChangeImage(isOn, PersonnelAlamToggle);
        Debug.Log("OnQueryToggleChange:" + isOn);
       
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
        IsHistorical = true;
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn );
        ChangeImage(isOn, HistoricalToggle);
        Debug.Log("OnHistoricalToggleChange:" + isOn);
        if (isOn)
        {
            AlarmPushManage.Instance.ShowAlarmPushWindow(false);
            ToggleChangedBefore();
            MultHistoryPlayUI.Instance.ShowT();
           
        }
        else
        {
            AlarmPushManage.Instance.ShowAlarmPushWindow(true );
            MultHistoryPlayUI.Instance.Hide();
            IsHistorical = false;
        }
    }
    public bool IsOnEditArea;
    /// <summary>
    /// 编辑监控区域
    /// </summary>
    /// <param name="isOn"></param>
    private void OnEditAreaToggleChange(bool isOn)
    {
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn);
        ChangeImage(isOn, EditAreaToggle);
        Debug.Log("OnMonitoringToggleChange:" + isOn);
        IsOnEditArea = isOn;
        if (isOn)
        {
            AlarmPushManage.Instance.ShowAlarmPushWindow(false);
            ToggleChangedBefore();
            ObjectsEditManage.Instance.SetEditorGizmoSystem(true);
            MonitorRangeManager.Instance.ShowAreaEdit(SceneEvents.DepNode);
            //FactoryDepManager.Instance.SetAllColliderEnable(false);
            FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(true);
            PersonnelTreeManage.Instance.CloseWindow();
            SmallMapController.Instance.Hide();
            StartOutManage.Instance.Hide();
            FunctionSwitchBarManage.Instance.SetWindow(false);
            
        }
        else
        {
            AlarmPushManage.Instance.ShowAlarmPushWindow(true );
            ObjectsEditManage.Instance.SetEditorGizmoSystem(false);
            MonitorRangeManager.Instance.ExitCurrentEditArea();
            //FactoryDepManager.Instance.SetAllColliderEnable(true);
            FactoryDepManager.Instance.SetAllColliderIgnoreRaycast(false);
            PersonnelTreeManage.Instance.ShowWindow();
            SmallMapController.Instance.Show();
            StartOutManage.Instance.Show();
            FunctionSwitchBarManage.Instance.SetWindow(true);
        }
    }
    public bool IsOnBenchmarking;
    /// <summary>
    /// 基准点
    /// </summary>
    /// <param name="isOn"></param>
    private void OnBenchmarkingToggleChange(bool isOn)
    {
        ParkInformationManage.Instance.ShowParkInfoUI(!isOn);
        ChangeImage(isOn, BenchmarkingToggle);
        Debug.Log("OnBenchmarkingToggleChange:" + isOn);
        if (isOn)
        {
            IsOnBenchmarking = isOn;
            ToggleChangedBefore();
        }
        else
        {
            IsOnBenchmarking = isOn;
        }
    }
    /// <summary>
    /// 选中时改变图片
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="tog"></param>
    public void ChangeImage(bool isOn,Toggle tog)
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
            ChangeImage(Ison, CoordinateToggle);
        ModelAdjustManage.Instance.ShowWindow(Ison);
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
        CoordinateToggle.transform.parent.gameObject.SetActive(isActiveT);
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
}
