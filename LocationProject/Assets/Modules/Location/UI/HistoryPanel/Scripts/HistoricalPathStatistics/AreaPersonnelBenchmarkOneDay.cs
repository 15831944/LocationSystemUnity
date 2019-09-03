using Location.WCFServiceReferences.LocationServices;
using SpringGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaPersonnelBenchmarkOneDay : MonoBehaviour {
    public Text Title;
    public Text MonthsTitle;
    public static AreaPersonnelBenchmarkOneDay Instance;
    public CalendarChange StartcalendarDay;
    public BarChart PersonnelLineChart;
    public UGUI_LineChartDateFill Personnel_X_Data;
    public UGUI_LineChartYValue LineChart_Y_value;
    public Text CurrentTimeText;
    public GameObject Per_X_Grid;
    public GameObject Per_XObj;
    List<PositionList> ScreenPerPositionList;
    List<PositionList> AllList;
    List<PositionList> PersonnelOneDayPositionList;
    public Text FloorTime;
    public Text PerTime;
    string CurrentPerName;
    string FloorName;

    private void Awake()
    {
        Instance = this;
    } 
    void Start () {
        StartcalendarDay.onDayClick.AddListener(ScreenPersonnelTime);
    }
    public void GetOneDayPersonnelDate(List<PositionList> listInfo,string  floorName, string NameStr)
    {
        CurrentPerName = NameStr;
        FloorName = floorName;
        Title.text = NameStr + "在" + FloorName + "定位数据单天折线图";
        MonthsTitle.text = NameStr + "在" + FloorName + "定位数据历史折线图";
        AllList = new List<PositionList>();
        for (int i = 0; i < listInfo.Count; i++)
        {
            if (listInfo[i].Name.Contains("1970") || listInfo[i].Name.Contains("1974"))
            {
                Debug.Log("数据有问题，去掉1970年的和1974年的");
            }
            else
            {
                AllList.Add(listInfo[i]);
            }
        }
        AllList.Sort((x, y) =>
        {
            return x.Name.CompareTo(y.Name);
        });
        DateTime ShowT = Convert.ToDateTime(AllList[0].Name);
        FloorTime.text = AllList[0].Name + "起";
        PerTime.text = AllList[0].Name + "起";
        CurrentTimeText.text = ShowT.ToString(("yyyy年MM月dd日"));
        ScreenPersonnelTime(ShowT);
    }
    public void ScreenPersonnelTime(DateTime timeKey)
    {
        ScreenPerPositionList = new List<PositionList>();
        string TimeT = timeKey.ToString(("yyyy-MM-dd"));
        PositionList item = AllList.Find(j => j.Name.Contains(TimeT));
        if (item != null)
        {
            ScreenPerPositionList.Add(item);
        }
        if (ScreenPerPositionList.Count == 0)
        {

            NullDate();
        }
       else
        {
            ScreenPersonnelOneDayPositionList(ScreenPerPositionList[0].Name );
        }

    }
    public void ScreenPersonnelOneDayPositionList(string name)
    {
        PersonnelOneDayPositionList = new List<PositionList>();
        CommunicationObject.Instance.GetHistoryPositonStatistics(3, FloorName, CurrentPerName, name, list =>
        {
            PersonnelOneDayPositionList = list;
            ShowLineChartInfo(PersonnelOneDayPositionList);
        });
    }
    public void NullDate()
    {    
        DeleteLinePrefabs();
        LineChart_Y_value.DateY(0);  
        List<float> data = new List<float>();
       
        PersonnelLineChart.Inject(data);
        PersonnelLineChart.enabled = false;//这样处理不用点击一下Inspector里面的东西，柱状图才可以出来
        PersonnelLineChart.enabled = true;
        return;
    }
    public void ShowLineChartInfo(List<PositionList> PosList)
    {
        DeleteLinePrefabs();
        if (PosList == null)
        {
            NullDate();
        }
        if (PosList.Count == 0)
        {
            NullDate();
        };
        PosList.Sort((x, y) =>
        {
            return x.Count.CompareTo(y.Count);
        });//根据数量排列    
        LineChart_Y_value.DateY(PosList[PosList.Count -1].Count);
        int MaxNum = PosList[PosList.Count -1].Count;
        List<float> data = new List<float>();
        PosList.Sort((x, y) =>
        {
            return x.Name.CompareTo(y.Name);
        });
        DateTime dt = Convert.ToDateTime(PosList[0].Name + ":00:00");
        string DT = dt.ToString(("yyyy-MM-dd"));
        DateTime MinDt = Convert.ToDateTime(DT);
        int DifferencetIME = 24;
        for (int i = 0; i < DifferencetIME; i++)
        {
            TimeInstantiateLine();
            DateTime HoursAdd = MinDt.AddHours(i);
            string TimeT = HoursAdd.ToString(("yyyy-MM-dd HH"));
            PositionList item = PosList.Find(j => j.Name.Contains(TimeT));
            if (item != null)
            {
                data.Add((float)item.Count / MaxNum);
                //TimeScreenPositionList.Remove(item);
            }
            else
            {
                data.Add(0);//没有数据的日期补上1
            }
        }
     
        SetHourLineChartDate(PosList, MinDt, 24);
       
        PersonnelLineChart.Inject (data);
        PersonnelLineChart.enabled = false;//这样处理不用点击一下Inspector里面的东西，柱状图才可以出来
        PersonnelLineChart.enabled = true;
    }
    public void NullData()
    {
        DeleteLinePrefabs();
        LineChart_Y_value.DateY(0);
        List<float> data = new List<float>();
        PersonnelLineChart.Inject(data);
        PersonnelLineChart.enabled = false;//这样处理不用点击一下Inspector里面的东西，柱状图才可以出来
        PersonnelLineChart.enabled = true;
        return;
    }
  
    private void SetHourLineChartDate(List<PositionList> DataList, DateTime DT, int count)
    {

        if (DataList != null)
        {
            //LastTime = long.Parse(DataList[DataList.Count - 1].RecordTime);
            DateTime dt = Convert.ToDateTime(DataList[DataList.Count - 1].Name + ":00:00");
            Personnel_X_Data.DateFillT(UGUI_LineChartDateFill.DateType.Day, count, DT.AddHours(23));

        }

    }
    public GameObject TimeInstantiateLine()
    {
        GameObject o = Instantiate(Per_XObj);
        o.SetActive(true);
        o.transform.parent = Per_X_Grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
        return o;
    }
    public void DeleteLinePrefabs()
    {
        for (int j = Per_X_Grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(Per_X_Grid.transform.GetChild(j).gameObject);
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
