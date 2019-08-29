using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaoXinDeviceAlarm : MonoBehaviour {
    public static BaoXinDeviceAlarm Instance;
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
    public InputField InputKey;
    List<CameraAlarmInfo> ScreenDevAlarmList;
    List<CameraAlarmInfo> AllDevAlarmList;
    List<CameraAlarmInfo> ShowDevAlarmList;
    public CalendarChange StartcalendarDay;
    public CalendarChange EndcalendarDay;
    private int AlarmType = 0;
    string DevKey;
    DateTime StartTime ;
    DateTime EndTime;
    void Start () {
        Instance = this;
        AddPageBut.onClick.AddListener(AddDevAlarmPage);
        MinusPageBut.onClick.AddListener(MinDevAlarmPage);
        CloseBut.onClick.AddListener(()=>
        {
            ShowDevAlarm(false);
        });
        StartcalendarDay.onDayClick.AddListener(ScreenStartTimeAlarm);
        EndcalendarDay.onDayClick.AddListener(ScreenEndTimeAlarm);
        pegeNumText.onValueChanged.AddListener(InputDevPage);
        InputKey.onValueChanged.AddListener(InputScreenSecondDevAlarm);
    }
    public void GetDevAlarmList()
    {
        AllDevAlarmList = new List<CameraAlarmInfo>();
        AllDevAlarmList = CommunicationObject.Instance.GetAllCameraAlarms(true);
        DateTime CurrentTime = System.DateTime.Now;
        string currenttime = CurrentTime.ToString("yyyy年MM月dd日");
        StartTimeText.text = DateTime.Now.Year.ToString() + "年01月01日"; ;
        EndTimeText.text = currenttime;
        StartTime = Convert.ToDateTime( DateTime.Now.Year.ToString() + "年01月01日");
        EndTime = CurrentTime;
        ScreenDevAlarmList = new List<CameraAlarmInfo>();       
        ScreenDevAlarmList.AddRange(AllDevAlarmList);       
        SaveSelection();
        GetDevAlarmPageData(AllDevAlarmList);
        TotaiLine(AllDevAlarmList);
        PageNum = 1;
        StartPageNum = 0;
        pegeNumText.text = "1";
    }
    /// <summary>
    /// 获取几页数据
    /// </summary>
    public void GetDevAlarmPageData(List<CameraAlarmInfo> AlarmData)
    {
        ShowDevAlarmList = new List<CameraAlarmInfo>();
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
    public void GetDevAlarmData(List<CameraAlarmInfo> AlarmData)
    {
        for (int i = 0; i < AlarmData.Count; i++)
        {
            GameObject obj = InstantiateLine();
            CameraAlarmFollowUIItem item = obj.GetComponent<CameraAlarmFollowUIItem>();              
            item.GetCameraAlarmData(AlarmData[i]);
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
            currentPage = int.Parse(pegeNumText.text);
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
        StartTime = dateTime;
        GetScreenAlarmInfo(AlarmType, DevKey, StartTime, EndTime);
    }
    public void ScreenEndTimeAlarm(DateTime dateTime)
    {
        EndTime = dateTime;
        GetScreenAlarmInfo(AlarmType, DevKey, StartTime, EndTime);
    }

    public void InputScreenSecondDevAlarm(string key)
    {
        DevKey = key;
        GetScreenAlarmInfo(AlarmType, DevKey, StartTime, EndTime);
    }
    public void GetScreenAlarmType(int level)
    {
        AlarmType = level;
        GetScreenAlarmInfo(AlarmType, DevKey, StartTime, EndTime);


    }
    public void GetScreenAlarmInfo(int level,string key, DateTime startTime, DateTime endTime)
    {
        pegeNumText.text = "1";
        SaveSelection();
        ScreenDevAlarmList.Clear();
        DateTime NewEndTime = endTime.AddHours(24);

        for (int i = 0; i < AllDevAlarmList.Count; i++)
        {
            string devName = AllDevAlarmList[i].DevName.ToLower();
            string devIP = AllDevAlarmList[i].cid_url.ToString().ToLower();
            DateTime AlarmTime = GetDataTime(AllDevAlarmList[i].time_stamp);
            bool IsTime = DateTime.Compare(startTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(startTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (AllDevAlarmList[i].AlarmType != 1)
                {
                    int a = i;
                }
                if (ScreenTime && DevAlarmType(AllDevAlarmList[i].AlarmType, AlarmType))
                {
                    if (string .IsNullOrEmpty(DevKey))
                    {
                        ScreenDevAlarmList.Add(AllDevAlarmList[i]);
                    }
                    else
                    {
                        if (devIP.ToLower().Contains(DevKey) || devName.ToLower().Contains(DevKey))
                        {
                            ScreenDevAlarmList.Add(AllDevAlarmList[i]);
                        }
                    }
                    
                }
            }
            else
            {
                DateTime time1 = startTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(startTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (Time2 && DevAlarmType(AllDevAlarmList[i].AlarmType, AlarmType))
                {
                    if (string.IsNullOrEmpty(DevKey))
                    {
                        ScreenDevAlarmList.Add(AllDevAlarmList[i]);
                    }
                    else
                    {
                        if (devIP.ToLower().Contains(DevKey) || devName.ToLower().Contains(DevKey))
                        {
                            ScreenDevAlarmList.Add(AllDevAlarmList[i]);
                        }
                    }
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
  public bool  DevAlarmType(int typr, int devType)
    {
        if (devType == 0)
        {
            return true;
        }
        else 
        {
            if (typr == devType)
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
    public void TotaiLine(List<CameraAlarmInfo> data)
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

    public void ShowDevAlarm(bool b)
    {
        DevAlarmWindow.SetActive(b);
        InputKey.text  = "";
        if (b ==false)
        {
            //DevSubsystemManage.Instance.ChangeImage(false, DevSubsystemManage.Instance.QueryToggle);
            //DevSubsystemManage.Instance.QueryToggle.isOn = false;
            DevSubsystemManage.Instance.DevAlarmToggle.isOn = false;
        }
    }
    public DateTime GetDataTime(long time_stamp)
    {
        DateTime dtStart = new DateTime(1970, 1, 1);
        long lTime = ((long)time_stamp * 10000000);
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime AlarmTime = dtStart.Add(toNow);
        return AlarmTime;
    }


}
