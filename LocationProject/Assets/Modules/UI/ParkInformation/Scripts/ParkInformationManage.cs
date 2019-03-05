using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkInformationManage : MonoBehaviour
{
    public static ParkInformationManage Instance;
    public GameObject ParkInfoUI;//园区信息统计界面
    public AlarmSearchArg perAlarmData;
    public List<LocationAlarm> PerAlarmList;
    List<LocationAlarm> AlarmItem;
    public List<LocationAlarm> ParkAlarmInfoList;

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
    bool isRefresh;
    Color ArrowDotColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 102 / 255f);
    //public AreaStatistics AreaInfo;
    void Start()
    {

        Instance = this;
        SceneEvents.OnDepCreateComplete += OnRoomCreateComplete;
        SceneEvents.FullViewStateChange += OnFullViewStateChange;
        ArrowTog.onValueChanged.AddListener(OnArrowTogChange);
        PersonToggle.onValueChanged.AddListener(PersonnelAlarm_Click);


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
       // LoadData();
        if (!IsInvoking("StartRefreshData"))
        {
            //注释：用于崩溃测试；
            InvokeRepeating("StartRefreshData", 0, CommunicationObject.Instance.AreaStatisticsRefreshInterval);//todo:定时获取
        }
        //    GetParkDataInfo(CurrentNode);
    }
    public void StartRefreshData()
    {
        GetParkDataInfo(CurrentNode);
        Debug.Log("园区统计刷新");
    }

    public void GetParkDataInfo(int dep)
    {
       
        if (isRefresh)
        {
            Log.Alarm("GetParkDataInfo", "isRefresh:" + isRefresh);
            return;
        }
        isRefresh = true;
        ParkInfoUI.SetActive(true);
        Log.Info("ParkInfomationManage.GetParkDataInfo");
        //if (!CommunicationObject.Instance.isAsync)
        //{
        //    ThreadManager.Run(() =>
        //    {
        //        return CommunicationObject.Instance.GetAreaStatistics(dep);
        //    }, (data) =>
        //    {
        //        ShowParkDataInfo(data);
        //        isRefresh = false;
        //    }, "GetParkDataInfo");
        //}
        //else
        //{
        //    CommunicationObject.Instance.GetAreaStatisticsAsync(dep, (data) =>
        //    {
        //        ShowParkDataInfo(data);
        //        isRefresh = false;
        //    });
        //}

        CommunicationObject.Instance.GetAreaStatisticsAsync(dep, (data) =>
        {
            ShowParkDataInfo(data);
            isRefresh = false;
        });
    }

    private void ShowParkDataInfo(AreaStatistics data)
    {
        if (data == null)
        {
            Log.Error("ParkInfomationManage.ShowParkDataInfo", "data==null"); return;
        }
        PersonnelNumText.text = data.PersonNum.ToString();
        DevNumText.text = data.DevNum.ToString();
        PosAlarmText.text = data.LocationAlarmNum.ToString();
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
                if (IsInvoking("StartRefreshData"))
                {
                    CancelInvoke("StartRefreshData");

                }
                ParkInfoUI.SetActive(false);
                return;
            }
            else
            {
                ParkInfoUI.SetActive(true);
                if (!IsInvoking("StartRefreshData"))
                {
                    //注释：用于崩溃测试；
                    InvokeRepeating("StartRefreshData", 0, CommunicationObject.Instance.AreaStatisticsRefreshInterval);//todo:定时获取
                    //Invoke("StartRefreshData", 0);
                }
            }

        }
        else
        {
            if (IsInvoking("StartRefreshData"))
            {
                CancelInvoke("StartRefreshData");
            }
            ParkInfoUI.SetActive(false);
        }
    }

    private void LoadData()
    {
        AlarmItem = new List<LocationAlarm>();
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
    public void PersonnelAlarm_Click(bool b)
    {
        if (PersonToggle.isOn == true)
        {
            LoadData();
           
            PersonnelAlarmParkInfo.Instance.ShowPersonnelAlarmParkWindow(true);
        }
        else
        {
            ParkAlarmInfoList.Clear();
            PersonnelAlarmParkInfo.Instance.ShowPersonnelAlarmParkWindow(false);
        }
    }
    public void SetAreaAlarmNodeID(DepNode node,int Id, LocationAlarm info)
    {
        if (node  !=null )
        {
          if (node.NodeID == Id)  

            ParkAlarmInfoList.Add(info);
        }
        if (node.ParentNode!=null)
        {
            SetAreaAlarmNodeID(node.ParentNode, Id, info);
        }
      
    }
}
