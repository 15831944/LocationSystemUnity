using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DevAlarmListManage : MonoBehaviour
{
    public static DevAlarmListManage Instance;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    private int pageLine = 10;
    /// <summary>
    /// 数据
    /// </summary>
    private int StartPageNum = 0;
    /// <summary>
    /// 页数
    /// </summary>
    private int PageNum = 1;
    /// <summary>
    /// 总页数
    /// </summary>
    public Text pegeTotalText;
    public InputField pegeNumText;
    public Button AddPageBut;
    public Button MinusPageBut;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    /// <summary>
    /// 存放10条数据的列表
    /// </summary>
    public List<DeviceAlarm> newDevList;
    /// <summary>
    /// 设备告警界面
    /// </summary>
    public GameObject DevAlarmWindow;

    public Sprite DoubleImage;
    public Sprite OddImage;

    public Text StartTimeText;
    public Text EndTimeText;
    public Button CloseBut;
    private string type;
    private string num;
    private string title;
    private string AlarmTime;
    private string content;

    private string devId;
    private int depId;
    public DeviceAlarm[] deviceAlarm;
    private AlarmSearchArg searchArg;
    public List<DeviceAlarm> devAlarmData;
    public List<DeviceAlarm> ScreenDevAlarmList;

    public CalendarChange StartcalendarDay;
    public CalendarChange EndcalendarDay;

    public DevAlarmdropdownItem devAlarmdropdownItem;
    public DevAlarmType devAlarmType;
    public Text promptText;
    public GameObject BanClick;//正在获取数据时，不能点击
    DeviceAlarmInformation DevAlarmInfo;
    List<DeviceAlarm> ShowDevAlarmList;
    public Button AlarmStatistics;//告警统计
    int AlarmLevel = 0;
    int DevType = 0;
    bool IsGetData = false;
    bool IsStartShow = true;
    void Start()
    {
        Instance = this;

        AddPageBut.onClick.AddListener(AddDevAlarmPage);
        MinusPageBut.onClick.AddListener(MinDevAlarmPage);
        CloseBut.onClick.AddListener(CloseDevAlarmWindow);
        StartcalendarDay.onDayClick.AddListener(ScreeningStartTimeAlaim);
        EndcalendarDay.onDayClick.AddListener(ScreeningSecondTimeAlarm);
        pegeNumText.onValueChanged.AddListener(InputDevPage);
        if (AlarmStatistics!=null)
        {
            AlarmStatistics.onClick.AddListener(() =>
            {
                DevAlarmStatisticalManage.Instance.ShowDevAlarmStatisticalWindow(true);
                ColseDevAlarm();
            });
        }
     
    }
    /// <summary>
    /// 刚打开设备告警时的界面
    /// </summary>
    public void StartDevAlarm()
    {
        pegeNumText.text = "1";
        searchArg = new AlarmSearchArg();
        searchArg.Page = new PageInfo();
        searchArg.Page.Number = 0;

        StartPageNum = 0;
        searchArg.Start = DateTime.Now.Year.ToString() + "年01月01日";
        searchArg.End = DateTime.Now.ToString("yyyy年MM月dd日"); ;
        GetDeviceAlarmInfo(searchArg, searchArg.Page);
    }
    public void GetDeviceAlarmInfo(AlarmSearchArg arg, PageInfo page)
    {
        SaveSelection();
        page.Size = 10;
        page.Number = StartPageNum;
        arg.Level = AlarmLevel;
        arg.DevTypeName = GetDevType(DevType);
        if (IsGetData) return;
        BanClick.SetActive(true);
        Loom.StartSingleThread(() =>
        {
            DateTime start = DateTime.Now;
            IsGetData = true;
            var devAlarms = CommunicationObject.Instance.GetDeviceAlarms(arg);
            if (devAlarms == null)
            {
                IsGetData = false;
            }
            Debug.LogError("--------------------GetDeviceAlarms:" + (DateTime.Now - start).TotalMilliseconds + "ms");
            Loom.DispatchToMainThread(() =>
            {
                IsGetData = false;
                BanClick.SetActive(false );
                pegeNumText.interactable = true ;
                DevAlarmInfo = new DeviceAlarmInformation();
                DevAlarmInfo = devAlarms;
                pegeTotalText.text = DevAlarmInfo.Total.ToString();
                if (DevAlarmInfo.devAlarmList != null && DevAlarmInfo.Total != 0)
                {
                    ShowDevAlarmList = new List<DeviceAlarm>();
                    ShowDevAlarmList.AddRange(DevAlarmInfo.devAlarmList);
                    GetDevAlarmData(ShowDevAlarmList);
                }
            });
        });

    }

    /// <summary>
    /// 加一页信息
    /// </summary>
    public void AddDevAlarmPage()
    {
        StartPageNum += 1;
        //searchArg = new AlarmSearchArg();
        //searchArg.Page = new PageInfo();
        //searchArg.End = EndTimeText.text;
        //searchArg.Start = StartTimeText.text;
        if (int.Parse(pegeNumText.text) >= int.Parse(pegeTotalText.text))
        {

            StartPageNum -= 1;
            pegeNumText.text = pegeTotalText.text.ToString();
        }
        else
        {
            pegeNumText.text = (StartPageNum + 1).ToString();
        }
        //searchArg.Page.Number = StartPageNum;
        //GetDeviceAlarmInfo(searchArg, searchArg.Page);
    }
    public void MinDevAlarmPage()
    {
        //searchArg = new AlarmSearchArg();
        //searchArg.Page = new PageInfo();
        //searchArg.End = EndTimeText.text;
        //searchArg.Start = StartTimeText.text;
        if (StartPageNum > 0)
        {
            StartPageNum--;
            if (StartPageNum == 0)
            {
                pegeNumText.text = "1";
            }
            else
            {
                pegeNumText.text = (StartPageNum + 1).ToString();
            }
            //  searchArg.Page.Number = StartPageNum;
            //  GetDeviceAlarmInfo(searchArg, searchArg.Page);
        }

    }
    /// <summary>
    /// 搜索选中页
    /// </summary>
    /// <param name="value"></param>
    public void InputDevPage(string value)
    {
        pegeNumText.interactable = false;
        Debug.LogError("--------------value:" + value);
        int currentPage;
        if (IsStartShow) return;      
        searchArg = new AlarmSearchArg();
        searchArg.Page = new PageInfo();
        searchArg.End = EndTimeText.text;
        searchArg.Start = StartTimeText.text;
        Debug.LogError("--------------searchArg:" + value);
        if (string.IsNullOrEmpty(pegeNumText.text))
        {
            currentPage = 1;
        }
        else
        {
            currentPage = int.Parse(pegeNumText.text);
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString();//触发事件2
        }
        if (currentPage > int.Parse(pegeTotalText.text))
        {
            searchArg.Page.Number = 78;
            currentPage = int.Parse(pegeTotalText.text);
            pegeNumText.text = pegeTotalText.text.ToString();//触发事件2           
        }
        else
        {
            StartPageNum = currentPage - 1;
            searchArg.Page.Number = StartPageNum;
        }
        //StartPageNum = currentPage - 1;
        //searchArg.Page.Number = StartPageNum;
        GetDeviceAlarmInfo(searchArg, searchArg.Page);

    }


    public void GetDevAlarmData(List<DeviceAlarm> AlarmData)
    {
        for (int i = 0; i < AlarmData.Count; i++)
        {
            GameObject obj = InstantiateLine();
            DevAlarmListItem item = obj.GetComponent<DevAlarmListItem>();
            item.GetDevAlarmData(AlarmData[i]);
            if (i % 2 == 0)
            {
                item.GetComponent<Image>().sprite = DoubleImage;
            }
            else
            {
                item.GetComponent<Image>().sprite = OddImage;
            }
        }

        IsStartShow = false;
    }
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject InstantiateLine()
    {
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
        return o;
    }

    /// <summary>
    /// 打开设备告警界面
    /// </summary>
    public void ShowDevAlarmWindow()
    {
        IsStartShow = true;
        DevAlarmWindow.SetActive(true);
        StartTimeText.text = DateTime.Now.Year.ToString() + "年01月01日"; ;
        EndTimeText.text = DateTime.Now.ToString("yyyy年MM月dd日");
        StartDevAlarm();
    }
    /// <summary>
    /// 关闭设备告警界面
    /// </summary>
    public void CloseDevAlarmWindow()
    {
        ColseDevAlarm();
        //pegeNumText.text = "1";
        //pegeTotalText.text = "1";
        //DevAlarmWindow.SetActive(false);
        //if (devAlarmdropdownItem.tempNames != null && devAlarmdropdownItem.tempNames.Count != 0) devAlarmdropdownItem.devAlarmLeveldropdown.captionText.text = devAlarmdropdownItem.tempNames[0];
        //devAlarmdropdownItem.devAlarmLeveldropdown.transform.GetComponent<Dropdown>().value = 0;

        //if (devAlarmType.tempNames != null && devAlarmType.tempNames.Count != 0) devAlarmType.DevTypedropdownItem.captionText.text = devAlarmType.tempNames[0];
        //devAlarmType.DevTypedropdownItem.transform.GetComponent<Dropdown>().value = 0;

        //DevSubsystemManage.Instance.ChangeImage(false, DevSubsystemManage.Instance.DevAlarmToggle);
        DevSubsystemManage.Instance.DevAlarmToggle.isOn = false;
        //IsGetData = false;
        //IsStartShow = false;
        //BanClick.SetActive(false);
    }
    public void ColseDevAlarm()
    {

        pegeNumText.text = "1";
        pegeTotalText.text = "1";
        DevAlarmWindow.SetActive(false);
        if (devAlarmdropdownItem.tempNames != null && devAlarmdropdownItem.tempNames.Count != 0) devAlarmdropdownItem.devAlarmLeveldropdown.captionText.text = devAlarmdropdownItem.tempNames[0];
        devAlarmdropdownItem.devAlarmLeveldropdown.transform.GetComponent<Dropdown>().value = 0;

        if (devAlarmType.tempNames != null && devAlarmType.tempNames.Count != 0) devAlarmType.DevTypedropdownItem.captionText.text = devAlarmType.tempNames[0];
        devAlarmType.DevTypedropdownItem.transform.GetComponent<Dropdown>().value = 0;

        //DevSubsystemManage.Instance.ChangeImage(false, DevSubsystemManage.Instance.DevAlarmToggle);
        //DevSubsystemManage.Instance.DevAlarmToggle.isOn = false;
        IsGetData = false;
        IsStartShow = false;
        BanClick.SetActive(false);
    }
    List<DeviceAlarm> ScreenAlarmTime = new List<DeviceAlarm>();
    /// <summary>
    /// 第二个筛选时间
    /// </summary>
    /// <param name="dateTime"></param>
    public void ScreeningSecondTimeAlarm(DateTime dateTime)
    {
        DateTime startT = Convert.ToDateTime(StartTimeText.text);
        bool TimeT = DateTime.Compare(startT, dateTime) >= 0;
        pegeNumText.text = "1";
        SaveSelection();
        ScreenDevAlarmList.Clear();
        searchArg = new AlarmSearchArg();
        searchArg.Page = new PageInfo();
        searchArg.Page.Number = 0;
        if (TimeT)
        {
            EndTimeText.text = startT.ToString("yyyy年MM月dd日");
            searchArg.End = StartTimeText.text;

        }
        else
        {
            searchArg.End = dateTime.ToString("yyyy年MM月dd日"); ;
        }
        searchArg.Start = StartTimeText.text;

        StartPageNum = 0;
        GetDeviceAlarmInfo(searchArg, searchArg.Page);
    }
    /// <summary>
    ///告警开始时间筛选
    /// </summary>
    public void ScreeningStartTimeAlaim(DateTime dateTime)
    {
        DateTime endT = Convert.ToDateTime(EndTimeText.text);
        bool TimeT = DateTime.Compare(dateTime, endT) <= 0;
        pegeNumText.text = "1";
        SaveSelection();
        ScreenDevAlarmList.Clear();
        searchArg = new AlarmSearchArg();
        searchArg.Page = new PageInfo();
        searchArg.Page.Number = 0;
        if (TimeT)
        {
            searchArg.Start = dateTime.ToString("yyyy年MM月dd日"); ;
        }
        else
        {
            StartTimeText.text = dateTime.ToString("yyyy年MM月dd日");
            searchArg.Start = EndTimeText.text;
        }
        searchArg.End = EndTimeText.text;

        StartPageNum = 0;
        GetDeviceAlarmInfo(searchArg, searchArg.Page);
    }
    List<DeviceAlarm> ScreenItem = new List<DeviceAlarm>();

    public void GetScreenDevAlarmLevel(int level)
    {
        AlarmLevel = level;
        pegeNumText.text = "1";
        SaveSelection();
        ScreenDevAlarmList.Clear();
        searchArg = new AlarmSearchArg();
        searchArg.Page = new PageInfo();
        searchArg.End = EndTimeText.text;
        searchArg.Start = StartTimeText.text;
        searchArg.Page.Number = 0;

        StartPageNum = 0;
        GetDeviceAlarmInfo(searchArg, searchArg.Page);
    }

    /// <summary>
    /// 筛选设备类型
    /// </summary>
    /// <param name="level"></param>
    public void GetScreenAlarmType(int level)
    {
        DevType = level;
        pegeNumText.text = "1";
        SaveSelection();
        ScreenDevAlarmList.Clear();
        searchArg = new AlarmSearchArg();
        searchArg.Page = new PageInfo();
        searchArg.End = EndTimeText.text;
        searchArg.Start = StartTimeText.text;
        searchArg.Page.Number = 0;

        StartPageNum = 0;
        GetDeviceAlarmInfo(searchArg, searchArg.Page);
    }
    /// <summary>
    /// 设备类型
    /// </summary>
    /// <returns></returns>
    public string GetDevType(int level)
    {
        if (level == 0) return "所有设备";
        if (level == 1) return "基站";
        else if (level == 2) return "摄像头";
        else
        {
            return "生产设备";
        }
    }


    /// <summary>
    /// 保留选中项
    /// </summary>
    public void SaveSelection()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
    /// <summary>
    /// 点击定位设备
    /// </summary>
    /// <param name="devId"></param>
    public void DevBut_Click(string devId, int DepID)
    {

        RoomFactory.Instance.FocusDev(devId, DepID);

        CloseDevAlarmWindow();
        AlarmPushManage.Instance.CloseAlarmPushWindow(false);
        //AlarmPushManage.Instance.IsShow.isOn = false;
        DevSubsystemManage.Instance.OnQueryToggleChange(false);

    }

}
