using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ParkDevAlarmInfo : MonoBehaviour
{
    public static ParkDevAlarmInfo Instance;
    public Text title;
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
    /// 设备告警界面
    /// </summary>
    public GameObject DevAlarmWindow;

    public Sprite DoubleImage;
    public Sprite OddImage;

    public Text StartTimeText;
    public Text EndTimeText;
    public Button CloseBut;
    List<DeviceAlarm> ScreenDevAlarmList;
    List<DeviceAlarm> AllDevAlarmList;
    List<DeviceAlarm> ShowDevAlarmList;
    public CalendarChange StartcalendarDay;
    public CalendarChange EndcalendarDay;
    void Start()
    {
        Instance = this;
        AddPageBut.onClick.AddListener(AddDevAlarmPage);
        MinusPageBut.onClick.AddListener(MinDevAlarmPage);
        CloseBut.onClick.AddListener(CloseDevAlarmWindow);
        StartcalendarDay.onDayClick.AddListener(ScreenStartTimeAlarm);
        EndcalendarDay.onDayClick.AddListener(ScreenSecondTime);
        pegeNumText.onEndEdit .AddListener(InputDevPage);

        //代码方式隐藏界面 不修改场景
        if (StartcalendarDay != null)
        {
           // StartcalendarDay.transform.parent.gameObject.SetActive(false);
        }
        if (EndcalendarDay != null)
        {
          //  EndcalendarDay.transform.parent.gameObject.SetActive(false);
        }

        if (EndcalendarDay != null)
        {
            //var pp = EndcalendarDay.transform.parent.parent;
            //pp.gameObject.SetActive(false);
            //var text=pp.parent.FindChildByName("Text");
            //if (text != null)
            //{
            //    text.gameObject.SetActive(false);
            //}
        }
    }
    public void GetDevAlarmList(List<DeviceAlarm> date, string name)
    {
        
        DateTime CurrentTime = System.DateTime.Now;
        string currenttime = CurrentTime.ToString("yyyy年MM月dd日");
        StartTimeText.text = currenttime;
        EndTimeText.text = currenttime;
        title.text = "设备告警(当天)—" + name;
        ScreenDevAlarmList = new List<DeviceAlarm>();
        AllDevAlarmList = new List<DeviceAlarm>();
        ScreenDevAlarmList.AddRange(date);
        AllDevAlarmList.AddRange(date);
        SaveSelection();
        GetDevAlarmPageData(date);
        TotaiLine(date);
        PageNum = 1;
        StartPageNum = 0;
        pegeNumText.text = "1";
    }
    /// <summary>
    /// 获取几页数据
    /// </summary>
    public void GetDevAlarmPageData(List<DeviceAlarm> AlarmData)
    {
        ShowDevAlarmList = new List<DeviceAlarm>();
        if (ShowDevAlarmList.Count != 0)
        {
            ShowDevAlarmList.Clear();
        }
        if (StartPageNum * pageLine < AlarmData.Count)
        {
            var QueryData = AlarmData.Skip(pageLine * StartPageNum).Take(pageLine);
            foreach (var devAlarm in QueryData)
            {
                ShowDevAlarmList.Add(devAlarm);
            }
            SaveSelection();
            GetDevAlarmData(ShowDevAlarmList);
        }
        TotaiLine(AlarmData);
    }
    public void GetDevAlarmData(List<DeviceAlarm> AlarmData)
    {
        for (int i = 0; i < AlarmData.Count; i++)
        {
            GameObject obj = InstantiateLine();
            ParkDevAlarmItem item = obj.GetComponent<ParkDevAlarmItem>();
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
    }
    public void AddDevAlarmPage()
    {
        StartPageNum += 1;
        Double a = Math.Ceiling((double)ScreenDevAlarmList.Count / (double)pageLine);
        int m = (int)a;

        if (StartPageNum <= m)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            GetDevAlarmPageData(ScreenDevAlarmList);
        }

    }
    public void MinDevAlarmPage()
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
            GetDevAlarmPageData(ScreenDevAlarmList);
        }
    }
    public void InputDevPage(string value)
    {
        int currentPage;
        if (string.IsNullOrEmpty(pegeNumText.text))
        {
            currentPage = 1;
        }
        else
        {
            if (value.Contains("-") || value.Contains("—"))
            {
                pegeNumText.text = "1";
                currentPage = 1;
            }
            else
            {
                currentPage = int.Parse(value);
            }
        }

        int maxPage = (int)Math.Ceiling((double)ScreenDevAlarmList.Count / (double)pageLine);
        if (maxPage == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            return;
        };//不处理的话会死循环的
        if (currentPage > maxPage)
        {
            currentPage = maxPage;
            pegeNumText.text = currentPage.ToString();//触发事件1->触发事件2->触发事件1->....
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString();//触发事件2
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetDevAlarmPageData(ScreenDevAlarmList);
    }
    public void ScreenStartTimeAlarm(DateTime dateTime)
    {
        pegeNumText.text = "1";
        SaveSelection();
        ScreenDevAlarmList.Clear();
        string EndTime = EndTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(dateTime);

        DateTime CurrentEndTime = Convert.ToDateTime(EndTime);
        DateTime NewEndTime = CurrentEndTime.AddHours(24);
        for (int i = 0; i < AllDevAlarmList.Count; i++)
        {
            DateTime AlarmTime = AllDevAlarmList[i].CreateTime;
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (ScreenTime && DevLevels(AllDevAlarmList[i]) && ScreenDevType(AllDevAlarmList[i], AlarmType))
                {
                    ScreenDevAlarmList.Add(AllDevAlarmList[i]);
                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevLevels(AllDevAlarmList[i]) && ScreenDevType(AllDevAlarmList[i], AlarmType))
                {

                    ScreenDevAlarmList.Add(AllDevAlarmList[i]);

                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (ScreenDevAlarmList.Count == 0)
        {

            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            GetDevAlarmPageData(ScreenDevAlarmList);
        }
        else
        {

            TotaiLine(ScreenDevAlarmList);
            GetDevAlarmPageData(ScreenDevAlarmList);
        }
    
    }
    public void ScreenSecondTime(DateTime dateTime)
    {
        pegeNumText.text = "1";
        SaveSelection();
        ScreenDevAlarmList.Clear();
        string StartTime = StartTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(StartTime);

        DateTime CurrentEndTime = Convert.ToDateTime(dateTime);
        DateTime NewEndTime = CurrentEndTime.AddHours(24);
        for (int i = 0; i < AllDevAlarmList.Count; i++)
        {
            DateTime AlarmTime = AllDevAlarmList[i].CreateTime;
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (ScreenTime && DevLevels(AllDevAlarmList[i]) && ScreenDevType(AllDevAlarmList[i], AlarmType))
                {
                    ScreenDevAlarmList.Add(AllDevAlarmList[i]);
                }

            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevLevels(AllDevAlarmList[i]) && ScreenDevType(AllDevAlarmList[i], AlarmType))
                {

                    ScreenDevAlarmList.Add(AllDevAlarmList[i]);

                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (ScreenDevAlarmList.Count == 0)
        {

            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            GetDevAlarmPageData(ScreenDevAlarmList);
        }
        else
        {

            TotaiLine(ScreenDevAlarmList);
            GetDevAlarmPageData(ScreenDevAlarmList);
        }

    }
    private int AlarmLevel=0;
    private int AlarmType = 0;
    public void GetScreenDevAlarmItem(int level)
    {
        AlarmLevel = level;
        //全部:0,高:1,中:2,低:3

        //[System.Runtime.Serialization.EnumMemberAttribute()]
        //    低 = 1,
        //    [System.Runtime.Serialization.EnumMemberAttribute()]
        //中 = 2,
        //    [System.Runtime.Serialization.EnumMemberAttribute()]
        //高 = 3,

        // int alarmLevel = 4 - level;

        // Debug.Log("GetScreenDevAlarmItem:level=" + level+ ",alarmLevel="+ alarmLevel);
        SaveSelection();
        ScreenDevAlarmList.Clear();
        pegeNumText.text = "1";
        string StartTime = StartTimeText.GetComponent<Text>().text;
        string EndTime = EndTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(StartTime);
        DateTime EndTimecurrent = Convert.ToDateTime(EndTime);
        DateTime NewEndTime = EndTimecurrent.AddHours(24);

        for (int i = 0; i < AllDevAlarmList.Count; i++)
        {
            var alarm = AllDevAlarmList[i];
            //var alarmLv = (int)alarm.Level;
            //if (alarmLv != alarmLevel) continue;
            DateTime AlarmTime = alarm.CreateTime;
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (ScreenTime && DevLevels(alarm) && ScreenDevType(alarm, AlarmType))
                {
                    ScreenDevAlarmList.Add(alarm);
                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevLevels(alarm) && ScreenDevType(alarm, AlarmType))
                {
                    ScreenDevAlarmList.Add(alarm);
                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (ScreenDevAlarmList.Count == 0)
        {

            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            GetDevAlarmPageData(ScreenDevAlarmList);
        }
        else
        {

            TotaiLine(ScreenDevAlarmList);
            GetDevAlarmPageData(ScreenDevAlarmList);
        }
    }
    public void GetScreenAlarmType(int level)
    {
        AlarmType = level;
        pegeNumText.text = "1";
        SaveSelection();
        ScreenDevAlarmList.Clear();
        string StartTime = StartTimeText.GetComponent<Text>().text;
        string EndTime = EndTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(StartTime);
        DateTime EndTimecurrent = Convert.ToDateTime(EndTime);
        DateTime NewEndTime = EndTimecurrent.AddHours(24);

        for (int i = 0; i < AllDevAlarmList.Count; i++)
        {
            DateTime AlarmTime = AllDevAlarmList[i].CreateTime;
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (ScreenTime && DevLevels(AllDevAlarmList[i]) && ScreenDevType(AllDevAlarmList[i], AlarmType))
                {
                    ScreenDevAlarmList.Add(AllDevAlarmList[i]);
                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevLevels(AllDevAlarmList[i]) && ScreenDevType(AllDevAlarmList[i], AlarmType))
                {

                    ScreenDevAlarmList.Add(AllDevAlarmList[i]);

                }
                Invoke("ChangeEndTime", 0.1f);
            }

        }
        if (ScreenDevAlarmList.Count == 0)
        {

            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            GetDevAlarmPageData(ScreenDevAlarmList);
        }
        else
        {

            TotaiLine(ScreenDevAlarmList);
            GetDevAlarmPageData(ScreenDevAlarmList);
        }
    }
    /// <summary>
    /// 告警等级
    /// </summary>
    /// <returns></returns>
    public Abutment_DevAlarmLevel GetDevAlarmdropdownItems()
    {
        int level = ParkDevAlarmLevel.instance.devAlarmLeveldropdown.value;
      
        if (level == 1) return Abutment_DevAlarmLevel.高;
        else if (level == 2) return Abutment_DevAlarmLevel.中;
        else if (level == 3) return Abutment_DevAlarmLevel.低;
        else
        {
            return Abutment_DevAlarmLevel.未定;
        }

    }
    /// <summary>
    /// 告警等级
    /// </summary>
    /// <param name="alarm"></param>
    /// <returns></returns>
    public bool DevLevels(DeviceAlarm alarm)
    {
        //int level = ParkDevAlarmLevel.instance.devAlarmLeveldropdown.value;
        if (AlarmLevel == 0) return true;     
          else  if (alarm.Level == GetDevAlarmdropdownItems())
            {
                return true;
            }
            else
            {
                return false;
            }
       
    }
    /// <summary>
    /// 设备类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool ScreenDevType(DeviceAlarm type,int level)
    {
        //if (ParkDevAlarmType.instance == null) return true;
        //if (ParkDevAlarmType.instance.DevTypedropdownItem == null) return true;
        //int level = ParkDevAlarmType.instance.DevTypedropdownItem.value;
        if (level == 0) return true;
        else
        {
            if (type.DevTypeName == GetDevType())
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
       // int level = ParkDevAlarmType.instance.DevTypedropdownItem.value;
        if (AlarmType == 1) return "基站";
        else if (AlarmType == 2) return "摄像头";
        else
        {
            return "生产设备";
        }
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
    /// 保留选中项
    /// </summary>
    public void SaveSelection()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
    public void ChangeEndTime()
    {
        string StartTime = StartTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(StartTime);
        string currenttime = NewStartTime.ToString("yyyy年MM月dd日");
        EndTimeText.GetComponent<Text>().text = currenttime.ToString();
    }
    public void CloseDevAlarmWindow()
    {
        DevAlarmWindow.SetActive(false);
        ParkInformationManage.Instance.DevToggle.isOn = false;
    }
    public void ShowDevAlarm()
    {
        ParkInformationManage.Instance.DevToggle.isOn = true ;
        DevAlarmWindow.SetActive(true);
    }
}
