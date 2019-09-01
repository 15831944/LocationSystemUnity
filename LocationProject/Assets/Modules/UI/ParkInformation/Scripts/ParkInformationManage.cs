using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkInformationManage : MonoBehaviour
{
    public static ParkInformationManage Instance;
    public GameObject ParkInfoUI;//园区信息统计界面
    [System.NonSerialized]
    public AlarmSearchArg perAlarmData;
    [System.NonSerialized]
    public List<LocationAlarm> PerAlarmList;
    [System.NonSerialized]
    List<LocationAlarm> AlarmItem;
    [System.NonSerialized]
    public List<LocationAlarm> ParkAlarmInfoList;
    [System.NonSerialized]
    public List<DeviceAlarm> DeviceAlarmList;
    [System.NonSerialized]
    public List<DeviceAlarm> ParkDeviceAlarmList;
    //[System.NonSerialized]
    //List<DeviceAlarm> DevAlarmItem;

    public Toggle ArrowTog;
    public int CurrentNode;
    /// <summary>
    /// 统计类型
    /// </summary>
    public Text TitleText;
    /// <summary>
    /// 人员总数
    /// </summary>
    public Text PersonnelNumText;
    /// <summary>
    /// 设备总数
    /// </summary>
    public Text DevNumText;
    /// <summary>
    /// 定位告警总数
    /// </summary>
    public Text PosAlarmText;
    /// <summary>
    /// 设备告警总数
    /// </summary>
    public Text DevAlarmText;
    public Toggle PersonToggle;
    public Toggle DevToggle;
    [System.NonSerialized]
    private AlarmSearchArg searchArg;
    bool isRefresh=false ;
    [System.NonSerialized]
    DepNode CurrentSelectNode;
    //Color ArrowDotColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 102 / 255f);
    //public AreaStatistics AreaInfo;
   public  bool IsGetPerData = false;
    void Start()
    {

        Instance = this;

        SceneEvents.OnDepCreateComplete += OnRoomCreateComplete;
        SceneEvents.FullViewStateChange += OnFullViewStateChange;
        SceneEvents.DepNodeChanged += SceneEvents_DepNodeChanged;

        ArrowTog.onValueChanged.AddListener(OnArrowTogChange);
        PersonToggle.onValueChanged.AddListener(PersonnelAlarm_Click);
        DevToggle.onValueChanged.AddListener(DevAlarm_Click);

    }

    void OnDestroy()
    {
        SceneEvents.OnDepCreateComplete -= OnRoomCreateComplete;
        SceneEvents.FullViewStateChange -= OnFullViewStateChange;
        SceneEvents.DepNodeChanged -= SceneEvents_DepNodeChanged;
    }

    private void SceneEvents_DepNodeChanged(DepNode arg1, DepNode arg2)
    {
        if (arg2 != null)
        {
            CurrentSelectNode = arg2;
            CurrentNode = arg2.NodeID;
            GetParkDataInfo(CurrentNode);//切换场景节点时更新统计数据
        }
    }

    public void OnFullViewStateChange(bool b)
    {
        if (b)
        {
            ShowParkInfoUI(false);
        }
        else
        {

            DepNode dep = FactoryDepManager.Instance;
            //ParkInfoUI.SetActive(true);

            TitleText.text = dep.NodeName.ToString();
            RefreshParkInfo(dep.NodeID);

        }
    }

    public void OnRoomCreateComplete(DepNode dep)
    {

        FullViewController mainPage = FullViewController.Instance;
        if (mainPage && mainPage.IsFullView)
        {
            ShowParkInfoUI(false);
            return;
        }
        else
        {
            if (PersonSubsystemManage.Instance.IsOnBenchmarking == false && PersonSubsystemManage.Instance.IsOnEditArea == false && DevSubsystemManage.Instance.isDevEdit == false && PersonSubsystemManage.Instance.IsHistorical == false)
            {

                TitleText.text = dep.NodeName.ToString();
                //GetParkDataInfo(dep.NodeID);
                RefreshParkInfo(dep.NodeID);
            }

        }

    }

    /// <summary>
    /// 刷新时间间隔
    /// </summary>
    public int RefreshInterval = 5;

    public void RefreshParkInfo(int dep)
    {
        CurrentNode = dep;
        return;
        //// LoadData();
        // if (!IsInvoking("StartRefreshData")&& ParkInfoUI.activeSelf)
        // {
        //     //注释：用于崩溃测试；
        //     InvokeRepeating("StartRefreshData", 0, CommunicationObject.Instance.RefreshSetting.AreaStatistics);//todo:定时获取
        //     Debug.LogError("刷新统计信息");
        // }
        // //    GetParkDataInfo(CurrentNode);
    }
    public void StartRefreshData()
    {
        GetParkDataInfo(CurrentNode);
        //Debug.Log("园区统计刷新");
    }
    /// <summary>
    /// 当前园区人数
    /// </summary>
    private int currentPerson = -1;
    /// <summary>
    /// 设置厂区在线人员数
    /// </summary>
    /// <param name="num"></param>
    public void SetFactoryLocatedPerson(int num)
    {
        if (currentPerson == num) return;
        currentPerson = num;
        if (PersonnelNumText != null) PersonnelNumText.text = num.ToString();
    }
    public void GetParkDataInfo(int dep)
    {

        if (isRefresh)
        {
            //Log.Alarm("GetParkDataInfo", "isRefresh:" + isRefresh);
            return;
        }
        isRefresh = true;
        //    ParkInfoUI.SetActive(true);

        CommunicationObject.Instance.GetAreaStatistics(dep, (data) =>
        {
            ShowParkDataInfo(data);         
        });
        isRefresh = false;
    }

    private void ShowParkDataInfo(AreaStatistics data)
    {
        TitleText.text = CurrentSelectNode.NodeName.ToString();
        if (data == null)
        {
            Log.Error("ParkInfomationManage.ShowParkDataInfo", "data==null"); return;
        }
        //if(LocationManager.Instance&&!LocationManager.Instance.IsShowLocation)
        //{
        //    PersonnelNumText.text = data.PersonNum.ToString();
        //}
        DevNumText.text = data.DevNum.ToString();
        PosAlarmText.text = data.AlarmPersonNum.ToString();
        DevAlarmText.text = data.DevAlarmNum.ToString();
    }


    public void OnArrowTogChange(bool isOn)
    {
        if (ArrowTog.isOn = isOn)
        {
            //     ArrowTog.GetComponent<Image>().color = Color.white;

            ParkInfoTypeTween.Instance.BaseImage.SetActive(true);
            ParkInfoTypeTween.Instance.PlayBack();

        }
        else
        {
            //    ArrowTog.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 51 / 255f);

            //ParkInfoTypeTween.Instance.SetObjCloseAndDisappearTween();
            ParkInfoTypeTween.Instance.Play();
        }
    }

    void Update()
    {

    }
    /// <summary>
    /// 是否打开园区统计信息界面
    /// </summary>
    /// <param name="isOn"></param>
    public void ShowParkInfoUI(bool isOn)
    {

        if (isOn)
        {
            FullViewController mainPage = FullViewController.Instance;
            if (mainPage && mainPage.IsFullView)
            {
                //if (IsInvoking("StartRefreshData"))
                //{
                //    CancelInvoke("StartRefreshData");
                //    Debug.LogError("刷新统计信息");
                //}
                ParkInfoUI.SetActive(false);
                return;
            }
            else
            {
                ParkInfoUI.SetActive(true);
                //if (!IsInvoking("StartRefreshData"))
                //{
                //    StartRefreshData();
                //}
                StartRefreshData();
            }

        }
        else
        {
            //if (IsInvoking("StartRefreshData"))
            //{
            //    CancelInvoke("StartRefreshData");
            //}
            ParkInfoUI.SetActive(false);
        }
    }
    public void DevLoadData()
    {
        //DevAlarmItem = new List<DeviceAlarm>();
        DeviceAlarmList = new List<DeviceAlarm>();
        ParkDeviceAlarmList = new List<DeviceAlarm>();

        searchArg = new AlarmSearchArg();
        if (FactoryDepManager.currentDep != null)
            searchArg.AreaId = FactoryDepManager.currentDep.NodeID;
        //todo:设置时间、告警等级。

        var devAlarms = CommunicationObject.Instance.GetDeviceAlarms(searchArg);
        if (devAlarms != null)
            DeviceAlarmList = new List<DeviceAlarm>();
        if (devAlarms.devAlarmList != null )
        {
            DeviceAlarmList.AddRange(devAlarms.devAlarmList);
        }
        
        DevScreenAlarm(DeviceAlarmList);
    }
    public void DevScreenAlarm(List<DeviceAlarm> DevList)
    {
        if (FactoryDepManager.currentDep == null) return;
        string AreaDevName = FactoryDepManager.currentDep.NodeName;
        int ParkDevId = FactoryDepManager.currentDep.NodeID;
        //List<DeviceAlarm>alarms = DevList.FindAll(i=> i.AreaId==ParkDevId);//只显示当前区域下的告警数据
        ParkDeviceAlarmList.AddRange(DevList);
        ParkDevAlarmInfo.Instance.ShowDevAlarm();
        ParkDevAlarmInfo.Instance.GetDevAlarmList(ParkDeviceAlarmList, AreaDevName);

    }
     
    private void LoadData()
    {
        if (IsGetPerData) return;
        AlarmItem = new List<LocationAlarm>();
        perAlarmData = new AlarmSearchArg();
        perAlarmData.IsAll = false ;
        IsGetPerData = true;
        var personnelAlarm = CommunicationObject.Instance.GetLocationAlarms(perAlarmData);
        if (personnelAlarm != null)
        {
            PerAlarmList = new List<LocationAlarm>(personnelAlarm);
            foreach (var devAlarm in PerAlarmList)
            {
                if (devAlarm.AlarmLevel != LocationAlarmLevel.正常)
                {
                    AlarmItem.Add(devAlarm);
                }

            }
        }
        IsGetPerData = false;
        PersonnelScreenAlarm();
    }
    public void PersonnelScreenAlarm()
    {
        string AreaName = FactoryDepManager.currentDep.NodeName;
        int ParkPerId = FactoryDepManager.currentDep.NodeID;
        ParkAlarmInfoList = new List<LocationAlarm>();
        for (int i = 0; i < AlarmItem.Count; i++)
        {
            DepNode dep = RoomFactory.Instance.GetDepNodeById(AlarmItem[i].AreaId);

            SetAreaAlarmNodeID(dep, ParkPerId, AlarmItem[i]);

        }
        PersonnelAlarmParkInfo.Instance.GetPerAlarmList(ParkAlarmInfoList, AreaName);
    }
    /// <summary>
    /// 点击人员告警
    /// </summary>
    /// <param name="b"></param>
    public void PersonnelAlarm_Click(bool b)
    {
        if (PersonToggle.isOn == true)
        {
            if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
            ParkDevAlarmInfo.Instance.CloseDevAlarmWindow();
            PersonSubsystemManage.Instance.ExitDevSubSystem();
            DevSubsystemManage.Instance.ExitDevSubSystem();
            LoadData();
            PersonnelAlarmParkInfo.Instance.ShowPersonnelAlarmParkWindow(true);
        }
        else
        {
            ParkAlarmInfoList.Clear();
            PersonnelAlarmParkInfo.Instance.ShowPersonnelAlarmParkWindow(false);
        }
    }
    public void DevAlarm_Click(bool b)
    {
        if (DevToggle.isOn)
        {
            if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
            PersonnelAlarmParkInfo.Instance.ShowPersonnelAlarmParkWindow(false);
            PersonSubsystemManage.Instance.ExitDevSubSystem();
            DevSubsystemManage.Instance.ExitDevSubSystem();
            DevLoadData();

        }
        else
        {
            ParkDeviceAlarmList.Clear();
            ParkDevAlarmInfo.Instance.CloseDevAlarmWindow();
        }
    }


    public void SetAreaAlarmNodeID(DepNode node, int Id, LocationAlarm info)
    {
        if (node != null)
        {
            if (node.NodeID == Id)

                ParkAlarmInfoList.Add(info);
        }
        if (node.ParentNode != null)
        {
            SetAreaAlarmNodeID(node.ParentNode, Id, info);
        }

    }
    public void ClosePerAndDevAlarmWindow()
    {
        ParkDevAlarmInfo.Instance.CloseDevAlarmWindow();
        PersonnelAlarmParkInfo.Instance.ShowPersonnelAlarmParkWindow(false);
    }
}
