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
    public AlarmSearchArg DevAlarmlist;
    public List<DeviceAlarm> devAlarmData;
    public CalendarChange StartcalendarDay;
    public CalendarChange EndcalendarDay;
    public StartTime showStartTime;
    public DevAlarmdropdownItem devAlarmdropdownItem;
    public DevAlarmType devAlarmType;
    public Text promptText;

    void Start()
    {
        Instance = this;
        DevAlarmlist = new AlarmSearchArg();
        AddPageBut.onClick.AddListener(AddDevAlarmPage);
        MinusPageBut.onClick.AddListener(MinDevAlarmPage);
        CloseBut.onClick.AddListener(CloseDevAlarmWindow);
        StartcalendarDay.onDayClick.AddListener(ScreeningStartTimeAlaim);
        EndcalendarDay.onDayClick.AddListener(ScreeningSecondTimeAlarm);
        pegeNumText.onValueChanged.AddListener(InputDevPage);
        
    }
    /// <summary>
    /// 刚打开设备告警时的界面
    /// </summary>
    public void StartDevAlarm()
    {
       
        var devAlarms = CommunicationObject.Instance.GetDeviceAlarms(DevAlarmlist);
        devAlarmData = new List<DeviceAlarm>(devAlarms);
        PageNum = 1;
        StartPageNum = 0;
        GetDevAlarmPageData(devAlarmData);
        TotaiLine(devAlarmData);
        pegeNumText.text = "1";
        DateTime CurrentTime = System.DateTime.Now;
        string currenttime = CurrentTime.ToString("yyyy年MM月dd日");
        StartTimeText.text = currenttime;
        EndTimeText.text = currenttime;
       // devAlarmdropdownItem.ShowFirstDropdowData();
       // devAlarmType.ShowDropdownFirstData();
        showStartTime.ShowStartTime();
        IsScreen = false;
        IsTime1 = false;
        IsTIme2 = false;
        IsDevType = false;
    }
    /// <summary>
    /// zongye
    /// </summary>
    /// <param name="DevData"></param>
    public void TotaiLine(List<DeviceAlarm> data)
    {
        if (data.Count != 0)
        {
            if (data.Count % pageLine == 0)
            {
                pegeTotalText.text = (data.Count / pageLine).ToString();
            }
            else
            {
                pegeTotalText.text = Convert.ToString(Math.Ceiling((double)data.Count / (double)pageLine));
            }
        }
        else
        {
            pegeTotalText.text = "1";
        }
    }
    /// <summary>
    /// 上一页信息
    /// </summary>
    public void AddDevAlarmPage()
    {


        StartPageNum += 1;
        if (IsTIme2 || IsTime1 || IsScreen || IsDevType)
        {
            Double a = Math.Ceiling((double)ScreenItem.Count / (double)pageLine);
            int m = (int)a;

            if (StartPageNum <= m)
            {
                PageNum += 1;
                pegeNumText.text = PageNum.ToString();
                GetDevAlarmPageData(ScreenItem);
            }
        }
        else
        {
            Double a = Math.Ceiling((double)devAlarmData.Count / (double)pageLine);
            int m = (int)a;

            if (StartPageNum <= m)
            {
                PageNum += 1;
                pegeNumText.text = PageNum.ToString();
                GetDevAlarmPageData(devAlarmData);
            }
        }

    }
    public void MinDevAlarmPage()
    {
        if (IsTIme2 || IsTime1 || IsScreen || IsDevType)
        {
            if (StartPageNum > 0)
            {
                StartPageNum--;
                PageNum -= 1;
                if (PageNum == 0)
                {
                    pegeNumText.text = "1";
                }
                else
                {
                    pegeNumText.text = PageNum.ToString();
                }
                GetDevAlarmPageData(ScreenItem);
            }
        }
        else
        {
            if (StartPageNum > 0)
            {
                StartPageNum--;
                PageNum -= 1;
                if (PageNum == 0)
                {
                    pegeNumText.text = "1";
                }
                else
                {
                    pegeNumText.text = PageNum.ToString();
                }
                GetDevAlarmPageData(devAlarmData);
            }
        }

    }
    /// <summary>
    /// 搜索选中页
    /// </summary>
    /// <param name="value"></param>
    public void InputDevPage(string value)
    {
        int currentPage;
        if (string.IsNullOrEmpty(pegeNumText.text))
        {
            currentPage = 1;
        }
        else
        {
            currentPage = int.Parse(pegeNumText.text);
        }
        if (IsTIme2 || IsTime1 || IsScreen || IsDevType)
        {
            
            int maxPage = (int)Math.Ceiling((double)ScreenItem.Count / (double)pageLine);
            if (currentPage > maxPage)
            {
                currentPage = maxPage;
                pegeNumText.text = currentPage.ToString();
            }
            if (currentPage <= 0)
            {
                currentPage = 1;
                pegeNumText.text = currentPage.ToString();
            }
            StartPageNum = currentPage - 1;
            PageNum = currentPage;
            GetDevAlarmPageData(ScreenItem);
        }
        else
        {
            
            int maxPage = (int)Math.Ceiling((double)devAlarmData.Count / (double)pageLine);
            if (currentPage > maxPage)
            {
                currentPage = maxPage;
                pegeNumText.text = currentPage.ToString();
            }
            if (currentPage <= 0)
            {
                currentPage = 1;
                pegeNumText.text = currentPage.ToString();
            }
            StartPageNum = currentPage - 1;
            PageNum = currentPage;
            GetDevAlarmPageData(devAlarmData);
        }

    }
    /// <summary>
    /// 获取几页数据
    /// </summary>
	public void GetDevAlarmPageData(List<DeviceAlarm> AlarmData)
    {
        newDevList.Clear();
        if (StartPageNum * pageLine < AlarmData.Count)
        {
            var QueryData = AlarmData.Skip(pageLine * StartPageNum).Take(pageLine);
            foreach (var devAlarm in QueryData)
            {
                newDevList.Add(devAlarm);
            }
            GetDevAlarmData(newDevList);
        }
        TotaiLine(AlarmData);
    }


    /// <summary>
    /// 得到设备告警搜索的数据
    /// </summary>
    public void GetDevAlarmData(List<DeviceAlarm> newDevList)
    {

        for (int i = 0; i < newDevList.Count; i++)
        {
            type = newDevList[i].Dev.TypeName;
            num = newDevList[i].Id.ToString();
            title = newDevList[i].Title;
            string time = newDevList[i].CreateTime.ToString();
            DateTime NewTime = Convert.ToDateTime(time);
            AlarmTime = NewTime.ToString("yyyy年MM月dd日 HH:mm:ss");
            content = newDevList[i].Message;
            devId = newDevList[i].Dev.DevID.ToString();//设备ID
            depId = (int)newDevList[i].Dev.ParentId;//区域ID
            SetInstantiateLine(newDevList.Count);
            Transform line = grid.transform.GetChild(i);

            if (newDevList[i].Level != Abutment_DevAlarmLevel.无)
            {
                line.GetChild(6).GetComponent<Text>().text = "<color=#C66BABFF>未消除</color>";
            }
            else
            {
                line.GetChild(6).GetComponent<Text>().text = "<color=#FFFFFFFF>已消除</color>";
            }
            if (newDevList[i].Level == Abutment_DevAlarmLevel.高)
            {

                line.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(250 / 255f, 57 / 255f, 114 / 255f, 255 / 255f);

            }
            else if (newDevList[i].Level == Abutment_DevAlarmLevel.中)
            {
                line.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(250 / 255f, 146 / 255f, 55 / 255f, 255 / 255f);

            }
            else if (newDevList[i].Level == Abutment_DevAlarmLevel.低)
            {
                line.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(249 / 255f, 250 / 255f, 55 / 255f, 255 / 255f);

            }

            SetDevAlarmData(i, type, num, title, AlarmTime, content, devId, depId);
        }
    }
    /// <summary>
    /// 給预设赋值
    /// </summary>
    public void SetDevAlarmData(int i, string type, string num, string title, string AlarmTime, string content, string devId, int depId)
    {
        Transform Line = grid.transform.GetChild(i);
        Line.GetChild(1).GetComponent<Text>().text = type;
        Line.GetChild(2).GetComponent<Text>().text = num;

        Line.GetChild(3).GetComponent<Text>().text = title;
        Line.GetChild(4).GetComponent<Text>().text = AlarmTime;
        Line.GetChild(5).GetComponent<Text>().text = content;
        Button but = Line.GetChild(7).GetChild(0).GetComponent<Button>();
        but.onClick.RemoveAllListeners();
        but.onClick.AddListener(() =>
  {
      DevBut_Click(devId, depId);
  });
        if (i % 2 == 0)
        {
            Line.GetComponent<Image>().sprite = DoubleImage;
        }
        else
        {
            Line.GetComponent<Image>().sprite = OddImage;
        }
    }
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public void InstantiateLine()
    {
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
    }
    /// <summary>
    /// 生成多少预设
    /// </summary>
    /// <param name="num"></param>
    public void SetInstantiateLine(int num)
    {
        if (grid.transform.childCount < num)
        {
            InstantiateLine();
        }
        else
        {
            for (int j = grid.transform.childCount - 1; j > num; j--)
            {
                DestroyImmediate(grid.transform.GetChild(j).gameObject);
            }
        }
    }
    /// <summary>
    /// 打开设备告警界面
    /// </summary>
    public void ShowDevAlarmWindow()
    {
        DevAlarmWindow.SetActive(true);
        StartDevAlarm();
       
    }
    /// <summary>
    /// 关闭设备告警界面
    /// </summary>
    public void CloseDevAlarmWindow()
    {
        devAlarmdropdownItem.devAlarmLeveldropdown.captionText.text = devAlarmdropdownItem.tempNames[0];
        devAlarmdropdownItem.devAlarmLeveldropdown.transform.GetComponent<Dropdown>().value = 0;

        devAlarmType.DevTypedropdownItem.captionText.text = devAlarmType.tempNames[0];
        devAlarmType.DevTypedropdownItem.transform.GetComponent<Dropdown>().value = 0;
        DevAlarmWindow.SetActive(false);
        DevSubsystemManage.Instance.ChangeImage(false, DevSubsystemManage.Instance.DevAlarmToggle);
        DevSubsystemManage.Instance.DevAlarmToggle.isOn = false;
    }

    List<DeviceAlarm> ScreenAlarmTime = new List<DeviceAlarm>();
    /// <summary>
    /// 第二个筛选时间
    /// </summary>
    /// <param name="dateTime"></param>
    public void ScreeningSecondTimeAlarm(DateTime dateTime)
    {
        IsTIme2 = true;
        SaveSelection();
        ScreenItem.Clear();
        string StartTime = StartTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(StartTime);

        DateTime CurrentEndTime = Convert.ToDateTime(dateTime);
        DateTime NewEndTime = CurrentEndTime.AddHours(24);


        for (int i = 0; i < devAlarmData.Count; i++)
        {
            DateTime AlarmTime = devAlarmData[i].CreateTime;
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;

            if (IsTime)
            {
                if (ScreenTime && DevTypes(devAlarmData[i]) && ScreenDevType(devAlarmData[i]))
                {
                    ScreenItem.Add(devAlarmData[i]);
                }

            }
            else
            {

                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevTypes(devAlarmData[i]) && ScreenDevType(devAlarmData[i]))
                {

                    ScreenItem.Add(devAlarmData[i]);

                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (ScreenItem.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(ScreenItem);
            GetDevAlarmPageData(ScreenItem);
        }


    }
    /// <summary>
    ///告警时间筛选
    /// </summary>
    public void ScreeningStartTimeAlaim(DateTime dateTime)
    {
        IsTime1 = true;
        SaveSelection();
        ScreenItem.Clear();
        string EndTime = EndTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(dateTime);

        DateTime CurrentEndTime = Convert.ToDateTime(EndTime);
        DateTime NewEndTime = CurrentEndTime.AddHours(24);

        for (int i = 0; i < devAlarmData.Count; i++)
        {
            DateTime AlarmTime = devAlarmData[i].CreateTime;
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;

            if (IsTime)
            {
                if (ScreenTime && DevTypes(devAlarmData[i]) && ScreenDevType(devAlarmData[i]))
                {
                    ScreenItem.Add(devAlarmData[i]);
                }

            }
            else
            {

                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevTypes(devAlarmData[i]) && ScreenDevType(devAlarmData[i]))
                {

                    ScreenItem.Add(devAlarmData[i]);

                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (ScreenItem.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(ScreenItem);
            GetDevAlarmPageData(ScreenItem);
        }


    }
    List<DeviceAlarm> ScreenItem = new List<DeviceAlarm>();
    bool IsScreen = false;
    bool IsTime1 = false;
    bool IsTIme2 = false;
    bool IsDevType = false;
    public void GetScreenDevAlarmItems(int level)
    {
        IsScreen = true;
        SaveSelection();
        ScreenItem.Clear();
        for (int i = 0; i < devAlarmData.Count; i++)
        {
            DateTime AlarmTime = devAlarmData[i].CreateTime;
            string StartTime = StartTimeText.GetComponent<Text>().text;
            string EndTime = EndTimeText.GetComponent<Text>().text;
            DateTime NewStartTime = Convert.ToDateTime(StartTime);
            DateTime NewEndTime = Convert.ToDateTime(EndTime);
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;

            if (IsTime)
            {
                if (ScreenTime && DevTypes(devAlarmData[i]) && ScreenDevType(devAlarmData[i]))
                {
                    ScreenItem.Add(devAlarmData[i]);
                }
            }
            else
            {

                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevTypes(devAlarmData[i]) && ScreenDevType(devAlarmData[i]))
                {
                    ScreenItem.Add(devAlarmData[i]);
                }
                Invoke("ChangeEndTime", 0.1f);
            }

        }
        if (ScreenItem.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(ScreenItem);
            GetDevAlarmPageData(ScreenItem);
        }

    }

    /// <summary>
    /// 筛选设备类型
    /// </summary>
    /// <param name="level"></param>
    public void GetScreenAlarmType(int level)
    {
        IsDevType = true;
        SaveSelection();
        ScreenItem.Clear();
        for (int i = 0; i < devAlarmData.Count; i++)
        {
            DateTime AlarmTime = devAlarmData[i].CreateTime;
            string StartTime = StartTimeText.GetComponent<Text>().text;
            string EndTime = EndTimeText.GetComponent<Text>().text;
            DateTime NewStartTime = Convert.ToDateTime(StartTime);
            DateTime NewEndTime = Convert.ToDateTime(EndTime);
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;

            if (IsTime)
            {
                if (ScreenTime && DevTypes(devAlarmData[i]) && ScreenDevType(devAlarmData[i]))
                {
                    ScreenItem.Add(devAlarmData[i]);
                }
            }
            else
            {

                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevTypes(devAlarmData[i]) && ScreenDevType(devAlarmData[i]))
                {
                    ScreenItem.Add(devAlarmData[i]);
                }
                Invoke("ChangeEndTime", 0.1f);
            }

        }
        if (ScreenItem.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(ScreenItem);
            GetDevAlarmPageData(ScreenItem);
        }

    }
    /// <summary>
    /// 告警等级
    /// </summary>
    /// <returns></returns>
    public Abutment_DevAlarmLevel GetDevAlarmdropdownItems()
    {
        int level = DevAlarmdropdownItem.instance.devAlarmLeveldropdown.value;
        if (level == 1) return Abutment_DevAlarmLevel.高;
        else if (level == 2) return Abutment_DevAlarmLevel.中;
        else
        {
            return Abutment_DevAlarmLevel.低;
        }

    }
    /// <summary>
    /// 告警等级
    /// </summary>
    /// <param name="alarm"></param>
    /// <returns></returns>
    public bool DevTypes(DeviceAlarm alarm)
    {
        int level = DevAlarmdropdownItem.instance.devAlarmLeveldropdown.value;
        if (level == 0) return true;
        else
        {
            if (alarm.Level == GetDevAlarmdropdownItems())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    /// <summary>
    /// 设备类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool ScreenDevType(DeviceAlarm type)
    {
        int level = DevAlarmType.instance.DevTypedropdownItem.value;
        if (level == 0) return true;
        else
        {
            if (type.Dev.TypeName == GetDevType())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    /// <summary>
    /// 设备类型
    /// </summary>
    /// <returns></returns>
    public string GetDevType()
    {
        int level = DevAlarmType.instance.DevTypedropdownItem.value;
        if (level == 1) return "基站";
        else if (level == 2) return "摄像头";
        else
        {
            return "生产设备";
        }
    }

    public void ChangeEndTime()
    {
        string StartTime = StartTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(StartTime);
        string currenttime = NewStartTime.ToString("yyyy年MM月dd日");
        EndTimeText.GetComponent<Text>().text = currenttime.ToString();
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
        DevSubsystemManage.Instance.OnQueryToggleChange(false);
        CloseDevAlarmWindow();
        AlarmPushManage.Instance.ShowAlarmPushWindow(false);

    }

}
