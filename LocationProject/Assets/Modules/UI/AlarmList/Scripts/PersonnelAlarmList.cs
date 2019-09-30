
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelAlarmList : MonoBehaviour
{
    public static PersonnelAlarmList Instance;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    public int pageLine = 10;
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

    public InputField pegeNumText;//输入选中页
    public Text promptText;
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
    /// 10条数据存放的列表
    /// </summary>
    [System.NonSerialized]
    public List<LocationAlarm> newPerAlarmList;
    /// <summary>
    /// 人员告警界面
    /// </summary>
    public GameObject personAlarmUI;
    public Sprite DoubleImage;
    public Sprite OddImage;

    public Button CloseBut;//关闭
    public Button SearchBut;
    public Text StartTimeText;
    public Text DealTimeText;
    public InputField InputPerAlarm;//输入筛选内容
    public CalendarChange StartcalendarDay;
    public CalendarChange DealcalendarDay;
    private string num;
    private string nameT;
    private string job;
    private string content;
    private string startTime;
    private string handleTime;
    private string PerID;
    private string AlarmType;
    [System.NonSerialized]
    public AlarmSearchArg perAlarmData;
    [System.NonSerialized]
    public LocationAlarm[] perAlarmInfo;
    [System.NonSerialized]
    public List<LocationAlarm> PerAlarmList;

    [System.NonSerialized]
    List<LocationAlarm> ScreenAlarmItem;
    public StartTime showStartTime;
    //public PersonnelAlarmType perAlarmType;
    [System.NonSerialized]
    List<LocationAlarm> AlarmItem;
    bool IsGetData = false;
    public Button PerStatisticalBut;
    [System.NonSerialized]
    List<LocationAlarm> SeachPerItems ;
    void Start()
    {

        AlarmItem = new List<LocationAlarm>();
        //  ScreenAlarmItem = new List<LocationAlarm>();
        Instance = this;
        perAlarmData = new AlarmSearchArg();
        pegeTotalText.text = "1";
        pegeNumText.text = "1";
        //LoadData();
        //StartPerAlarmUI();
        AddPageBut.onClick.AddListener(AddPerAlarmPage);
        MinusPageBut.onClick.AddListener(MinPerAlarmPage);
        CloseBut.onClick.AddListener(ClosePersonAlarmUI);
        StartcalendarDay.onDayClick.AddListener(ScreeningStartTimeAlaim);
        DealcalendarDay.onDayClick.AddListener(ScreeningSecondTimeAlarm);
        SearchBut.onClick.AddListener(PerAlarmSearchBut_Click);
        pegeNumText.onEndEdit .AddListener(InputPersonnelPage);
        InputPerAlarm.onEndEdit .AddListener(PerAlarmSearch);
        if (RoomFactory.Instance.FactoryType != FactoryTypeEnum.BaoXin)
        {
            PerStatisticalBut.onClick.AddListener(() =>
            {
                PerAlarmStatisticalManage.Instance.ShowPerAlarmStatisticalWindow(true);
                Close_PersonnelAlarm();
            });
        }
        else
        {
            if (PerStatisticalBut != null)
            {
                PerStatisticalBut.gameObject.SetActive(false);
            }
        }

    }

    private void LoadData()
    {
        if (IsGetData) return;
        perAlarmData.Start = DateTime.Now.Year.ToString() + "年01月01日";
        perAlarmData.End = DateTime.Now.ToString("yyyy年MM月dd日"); ;
        perAlarmData.IsAll = true;
        IsGetData = true;
        var personnelAlarm = CommunicationObject.Instance.GetLocationAlarms(perAlarmData);
        if (personnelAlarm != null)
        {
            PerAlarmList = new List<LocationAlarm>(personnelAlarm);
            foreach (var perAlarm in PerAlarmList)
            {
                if (perAlarm.AlarmLevel != LocationAlarmLevel.正常)
                {
                    AlarmItem.Add(perAlarm);
                }

            }
        }
        IsGetData = false;
        SeachPerItems = new List<LocationAlarm>();
        SeachPerItems.AddRange(AlarmItem);
    }

    /// <summary>
    /// 打开人员告警界面
    /// </summary>
    public void StartPerAlarmUI()
    {
        //LoadData();

        //StartPageNum = 0;
        //PageNum = 1;
        //GetPersonnelAlarmPage(PerAlarmList);
        //TotaiLine(PerAlarmList);
        //pegeNumText.text = "1";
        //InputPerAlarm.text = "";
        //DateTime CurrentTime = System.DateTime.Now;
        //string currenttime = CurrentTime.ToString("yyyy年MM月dd日");
        //StartTimeText.text = currenttime;
        //DealTimeText.text = currenttime;
        //promptText.gameObject.SetActive(false);
        //showStartTime.ShowStartTime();
        //perAlarmType.ShowDropdownFirstData();

        Loom.StartSingleThread(() =>
        {
            LoadData();
            Loom.DispatchToMainThread(() =>
            {
                if (PerAlarmList.Count == 0)
                {
                    pegeTotalText.text = "1";
                    pegeNumText.text = "1";
                }
                else
                {

                    StartPageNum = 0;
                    PageNum = 1;
                    GetPersonnelAlarmPage(SeachPerItems);
                    pegeNumText.text = "1";
                    InputPerAlarm.text = "";
                    TotaiLine(SeachPerItems);
                    DateTime CurrentTime = System.DateTime.Now;
                    string currenttime = CurrentTime.ToString("yyyy年MM月dd日");
                    StartTimeText.text = currenttime;
                    DealTimeText.text = currenttime;
                    promptText.gameObject.SetActive(false);
                    showStartTime.ShowStartTime();
                    //  perAlarmType.ShowDropdownFirstData();
                }

            });
        });
    }
    /// <summary>
    /// 生成多少页
    /// </summary>
    public void TotaiLine(List<LocationAlarm> date)
    {
        if (date.Count != 0)
        {
            if (date.Count % pageLine == 0)
            {
                pegeTotalText.text = (date.Count / pageLine).ToString();
            }
            else
            {
                pegeTotalText.text = Convert.ToString(Math.Ceiling((double)date.Count / (double)pageLine));
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
    public void AddPerAlarmPage()
    {
        StartPageNum += 1;
        double a = Math.Ceiling((double)SeachPerItems.Count / (double)pageLine);
        int m = (int)a;
        if (StartPageNum <= m)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            GetPersonnelAlarmPage(SeachPerItems);
        }
        else
        {
            StartPageNum -= 1;
        }

    }
    public void MinPerAlarmPage()
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
            GetPersonnelAlarmPage(SeachPerItems);
        }


    }
    bool ispage = false;
    /// <summary>
    /// 输入搜索页
    /// </summary>
    /// <param name="value"></param>
    public void InputPersonnelPage(string value)
    {
        if (ispage == true) return;
        ispage = true;
        int currentPage;
        if (string.IsNullOrEmpty(value))
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
        int MaxPage = (int)Math.Ceiling((double)SeachPerItems.Count / (double)pageLine);
        if (currentPage > MaxPage)
        {
            currentPage = MaxPage;
            pegeNumText.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPersonnelAlarmPage(SeachPerItems);
        ispage = false;

    }
    /// <summary>
    /// 得到人员告警数据
    /// </summary>
    public void GetPersonnelAlarmData()
    {

        for (int i = 0; i < newPerAlarmList.Count; i++)
        {

            if (newPerAlarmList[i].Tag != null)
            {
                num = newPerAlarmList[i].Tag.Code;

            }
            else
            {
                num = "";

            }
            if (newPerAlarmList[i].Personnel == null)
            {
                nameT = "";
                job = "";
            }
            else
            {

                nameT = newPerAlarmList[i].Personnel.Name.ToString();
                if (newPerAlarmList[i].Personnel.Pst == null)
                {
                    job = "";
                }
                else
                {
                    job = newPerAlarmList[i].Personnel.Pst.ToString();
                }

            }
            content = newPerAlarmList[i].Content;
            string startTime1 = newPerAlarmList[i].CreateTime.ToString();
            if (startTime1 == "1/1/0001 12:00:00 AM")
            {
                startTime = "";
            }
            else
            {
                DateTime NewTime = Convert.ToDateTime(startTime1);
                startTime = NewTime.ToString("yyyy年MM月dd日 HH:mm:ss");
            }

            string HandleTime1 = newPerAlarmList[i].HistoryTime.ToString();

            if (string.IsNullOrEmpty(HandleTime1))
            {
                handleTime = "<color=#C66BABFF>未消除</color>"; ;
            }
            else
            {
                DateTime NewTime = Convert.ToDateTime(HandleTime1);
                handleTime = "<color=#FFFFFFFF>已消除</color>" + " " + NewTime.ToString("yyyy年MM月dd日 HH:mm:ss");
            }
            AlarmType = newPerAlarmList[i].AlarmType.ToString();
            PerID = newPerAlarmList[i].TagId.ToString();
            SetInstantiateLine(newPerAlarmList.Count);
            SetPersonnelAlarmData(i, num, nameT, job, AlarmType, content, startTime, handleTime, PerID, newPerAlarmList[i]);
        }
    }
    /// <summary>
    /// 生成的页数
    /// </summary>
    public void GetPersonnelAlarmPage(List<LocationAlarm> data)
    {
        newPerAlarmList = new List<LocationAlarm>();
        if (StartPageNum * pageLine < data.Count)
        {
            var QueryData = data.Skip(pageLine * StartPageNum).Take(pageLine);
            foreach (var devAlarm in QueryData)
            {
                //if (devAlarm.AlarmLevel != LocationAlarmLevel.正常)
                //{
                newPerAlarmList.Add(devAlarm);
                //}

            }
            TotaiLine(data);
            GetPersonnelAlarmData();
        }
    }
    public void SetPersonnelAlarmData(int i, string num, string name, string job, string AlarmType, string content, string startTime, string endTime, string PerID, LocationAlarm per)
    {
        Transform line = grid.transform.GetChild(i);
        line.GetChild(0).GetComponent<Text>().text = num;
        line.GetChild(1).GetComponent<Text>().text = name;
        line.GetChild(2).GetComponent<Text>().text = job;
        line.GetChild(3).GetComponent<Text>().text = AlarmType;
        line.GetChild(4).GetComponent<Text>().text = content;
        line.GetChild(5).GetComponent<Text>().text = startTime;
        line.GetChild(6).GetComponent<Text>().text = endTime;
        Button but = line.GetChild(7).GetChild(0).GetComponent<Button>();
        but.onClick.RemoveAllListeners();
        but.onClick.AddListener(() =>
  {
      PerAlarmBut_Click(PerID, per);
  });
        if (i % 2 == 0)
        {
            line.GetComponent<Image>().sprite = DoubleImage;
        }
        else
        {
            line.GetComponent<Image>().sprite = OddImage;
        }
    }
    /// <summary>
    /// 人员订位
    /// </summary>
    /// <param name="tagNum"></param>
    public void PerAlarmBut_Click(string tagNum, LocationAlarm per)
    {
        AlarmPushManage.Instance.CloseAlarmPushWindow(false);
        ParkInformationManage.Instance.ShowParkInfoUI(false);
        int tagID = int.Parse(tagNum);
        LocationManager.Instance.FocusPersonAndShowInfo(tagID);
        PersonSubsystemManage.Instance.ChangeImage(false, PersonSubsystemManage.Instance.PersonnelAlamToggle);
        PersonSubsystemManage.Instance.PersonnelAlamToggle.isOn = false;
        Close_PersonnelAlarm();
        JudgePerOnLine(tagID, per);
    }
    public void JudgePerOnLine(int tagNum, LocationAlarm per)
    {
        List<LocationObject> listT = LocationManager.Instance.GetPersonObjects();
        LocationObject locationObjectT = listT.Find((item) => item.personnel.TagId == tagNum);
        if (per.Tag == null || locationObjectT == null || per.Tag.IsActive == false)
        {
            UGUIMessageBox.Show("当前人员已离线或者不在监控区域！");
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
    /// 人员告警界面
    /// </summary>
    /// <param name="b"></param>
    public void ShowPersonAlarmUI()
    {
        personAlarmUI.SetActive(true);
        StartPerAlarmUI();
    }
    /// <summary>
    /// 关闭人员告警界面
    /// </summary>
    public void ClosePersonAlarmUI()
    {
        Close_PersonnelAlarm();
        PersonSubsystemManage.Instance.ChangeImage(false, PersonSubsystemManage.Instance.PersonnelAlamToggle);
        PersonSubsystemManage.Instance.PersonnelAlamToggle.isOn = false;
    }

    public void Close_PersonnelAlarm()
    {
        
        pegeNumText.text = "1";
        // perAlarmType.PerTypedropdownItem.captionText.text = perAlarmType.tempNames[0];
        //  perAlarmType.PerTypedropdownItem.transform.GetComponent<Dropdown>().value = 0;
        if (PerAlarmList != null) PerAlarmList.Clear();
        if (AlarmItem != null) AlarmItem.Clear();
        //  if (ScreenAlarmItem != null) ScreenAlarmItem.Clear();
        personAlarmUI.SetActive(false);
        IsGetData = false;
    }
    //List<LocationAlarm> ScreenAlarmTime = new List<LocationAlarm>();
    /// <summary>
    /// 第二个筛选时间
    /// </summary>
    /// <param name="dateTime"></param>
    public void ScreeningSecondTimeAlarm(DateTime dateTime)
    {
        pegeNumText.text = "1";
        SaveSelection();
        SeachPerItems.Clear();
        string StartTime = StartTimeText.GetComponent<Text>().text;

        DateTime NewStartTime = Convert.ToDateTime(StartTime);


        DateTime CurrentEndTime = Convert.ToDateTime(dateTime);
        DateTime NewDealTime = CurrentEndTime.AddHours(24);
        string key = InputPerAlarm.text.ToString().ToLower();
        for (int i = 0; i < AlarmItem.Count; i++)
        {
            DateTime AlarmTime = AlarmItem[i].CreateTime;
            bool IsTime = DateTime.Compare(NewStartTime, NewDealTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewDealTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (key == "" && ScreenTime)
                {
                    SeachPerItems.Add(AlarmItem[i]);
                }
                else if ((AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key)) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                {
                    if (ScreenTime)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }

                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);

                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewDealTime, AlarmTime) >= 0;
                if (key == "" && Time2)
                {
                    SeachPerItems.Add(AlarmItem[i]);
                }
                else if ((AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key)) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                {
                    if (Time2)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }
                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (SeachPerItems.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            // TotaiLine(SeachPerItems);
            GetPersonnelAlarmPage(SeachPerItems);
        }


    }
    /// <summary>
    ///告警时间筛选
    /// </summary>
    public void ScreeningStartTimeAlaim(DateTime dateTime)
    {
        pegeNumText.text = "1";
        SaveSelection();
        SeachPerItems.Clear();
        string key = InputPerAlarm.text.ToString().ToLower();
        string DealTime = DealTimeText.GetComponent<Text>().text;

        DateTime NewStartTime = Convert.ToDateTime(dateTime);

        DateTime CurrentEndTime = Convert.ToDateTime(DealTime);
        DateTime NewEndTime = CurrentEndTime.AddHours(24);
        for (int i = 0; i < AlarmItem.Count; i++)
        {
            DateTime AlarmTime = AlarmItem[i].CreateTime;
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (key == "" && ScreenTime)
                {
                    SeachPerItems.Add(AlarmItem[i]);
                }
                else if ((AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key)) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                {
                    if (ScreenTime)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }

                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);

                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (key == "" && Time2)
                {
                    SeachPerItems.Add(AlarmItem[i]);
                }
                else if ((AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key)) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                {
                    if (Time2)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }

                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (SeachPerItems.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            //TotaiLine(SeachPerItems);
            GetPersonnelAlarmPage(SeachPerItems);
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

  
    public void PerAlarmSearch(string str)
    {
        pegeNumText.text = "1";
        SaveSelection();
        SeachPerItems.Clear();
        string key = str;
        key = InputPerAlarm.text.ToString().ToLower();
        string StartTime = StartTimeText.GetComponent<Text>().text;
        string DealTime = DealTimeText.GetComponent<Text>().text;
        for (int i = 0; i < AlarmItem.Count; i++)
        {
            DateTime AlarmTime = AlarmItem[i].CreateTime;
            DateTime NewStartTime = Convert.ToDateTime(StartTime);

            DateTime NewEndTime = Convert.ToDateTime(DealTime);
            DateTime AddEndTime = NewEndTime.AddHours(24);
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(AddEndTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (string.IsNullOrEmpty(key))
                {
                    if (ScreenTime)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }
                }

                else
                {
                    if (AlarmItem[i].Tag != null)
                    {
                        if (AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                        {
                            if (ScreenTime)
                            {
                                SeachPerItems.Add(AlarmItem[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (string.IsNullOrEmpty(key))
                {
                    if (Time2)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }
                }

                else
                {
                    if (AlarmItem[i].Tag != null)
                    {
                        if ((AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key)) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                        {
                            if (Time2)
                            {
                                SeachPerItems.Add(AlarmItem[i]);
                            }
                        }
                    }



                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (SeachPerItems.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            //  TotaiLine(SeachPerItems);
            GetPersonnelAlarmPage(SeachPerItems);
        }
    }
    /// <summary>
    /// 搜索人员
    /// </summary>
    public void PerAlarmSearchBut_Click()
    {
        pegeNumText.text = "1";
        SaveSelection();
        SeachPerItems.Clear();
        string key = InputPerAlarm.text.ToString().ToLower();
        string StartTime = StartTimeText.GetComponent<Text>().text;
        string DealTime = DealTimeText.GetComponent<Text>().text;
        for (int i = 0; i < AlarmItem.Count; i++)
        {
            DateTime AlarmTime = AlarmItem[i].CreateTime;
            DateTime NewStartTime = Convert.ToDateTime(StartTime);

            DateTime NewEndTime = Convert.ToDateTime(DealTime);
            DateTime AddEndTime = NewEndTime.AddHours(24);
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(AddEndTime, AlarmTime) >= 0;
            if (IsTime)
            {
                if (string.IsNullOrEmpty(key))
                {
                    if (ScreenTime)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }
                }

                else
                {
                    if (AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                    {
                        if (ScreenTime)
                        {
                            SeachPerItems.Add(AlarmItem[i]);
                        }
                    }

                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (string.IsNullOrEmpty(key))
                {
                    if (Time2)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }
                }

                else
                {
                    if ((AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key)) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                    {
                        if (Time2)
                        {
                            SeachPerItems.Add(AlarmItem[i]);
                        }
                    }


                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (SeachPerItems.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            //  TotaiLine(SeachPerItems);
            GetPersonnelAlarmPage(SeachPerItems);
        }

    }
    //public LocationAlarmType GetPersonnelAlarmType()
    //{
    //    int level = PersonnelAlarmType.instance.PerTypedropdownItem.value;
    //    if (level == 1) return LocationAlarmType.区域告警;
    //    else if (level == 2) return LocationAlarmType.消失告警;
    //    else if (level == 3) return LocationAlarmType.低电告警;
    //    else if (level == 4) return LocationAlarmType.传感器告警;
    //    else if (level == 5) return LocationAlarmType.重启告警;
    //    else
    //    {
    //        return LocationAlarmType.非法拆卸;
    //    }
    //}
    //public bool PersonnelType(LocationAlarm type)
    //{
    //    int level = PersonnelAlarmType.instance.PerTypedropdownItem.value;
    //    if (level == 0) return true;
    //    else
    //    {
    //        if (type.AlarmType == GetPersonnelAlarmType())
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //}
    /// <summary>
    /// 筛选告警类型
    /// </summary>
    /// <param name="level"></param>
    public void GetScreenPersonnelAlarmItems(int level)
    {
        pegeNumText.text = "1";
        SaveSelection();
        SeachPerItems.Clear();
        string StartTime = StartTimeText.GetComponent<Text>().text;
        string DealTime = DealTimeText.GetComponent<Text>().text;
        for (int i = 0; i < AlarmItem.Count; i++)
        {
            DateTime AlarmTime = AlarmItem[i].CreateTime;
            DateTime NewStartTime = Convert.ToDateTime(StartTime);
            DateTime NewEndTime = Convert.ToDateTime(DealTime).AddHours(24);
            string key = InputPerAlarm.text.ToString().ToLower();
            bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenTime = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
            if (IsTime)
            {

                if (key == "" && ScreenTime)
                {
                    SeachPerItems.Add(AlarmItem[i]);
                }
                else if ((AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key)) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                {
                    if (ScreenTime)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }

                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, AlarmTime) <= 0 && DateTime.Compare(NewEndTime, AlarmTime) >= 0;
                if (key == "" && Time2)
                {
                    SeachPerItems.Add(AlarmItem[i]);
                }
                else if (((AlarmItem[i].Tag.Code.ToString().ToLower().Contains(key))) || AlarmItem[i].Personnel.Name.ToLower().Contains(key))
                {
                    if (Time2)
                    {
                        SeachPerItems.Add(AlarmItem[i]);
                    }

                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        if (SeachPerItems.Count == 0)
        {
            promptText.gameObject.SetActive(true);
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            promptText.gameObject.SetActive(false);
            // TotaiLine(SeachPerItems);
            GetPersonnelAlarmPage(SeachPerItems);
        }

    }
    public void ChangeEndTime()
    {
        string StartTime = StartTimeText.GetComponent<Text>().text;
        DateTime NewStartTime = Convert.ToDateTime(StartTime);
        string currenttime = NewStartTime.ToString("yyyy年MM月dd日");
        DealTimeText.GetComponent<Text>().text = currenttime.ToString();
    }
}
