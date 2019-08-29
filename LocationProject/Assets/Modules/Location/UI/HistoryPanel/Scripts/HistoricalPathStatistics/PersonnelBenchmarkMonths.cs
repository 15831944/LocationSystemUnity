using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelBenchmarkMonths : MonoBehaviour
{

    public static PersonnelBenchmarkMonths Instance;
    List<PositionList> AllList;
    public Text CurrentTimeText;
    public CalendarChange StartcalendarDay;
    public GameObject Per_X_Grid;
    public GameObject Per_XObj;
    public UGUI_LineChart PersonnelLineChart;
    public UGUI_LineChartDateFill Personnel_X_Data;
    public UGUI_LineChartYValue LineChart_Y_value;
    public TimeDropdown timeDropdown;
    List<PositionList> ScreenPersonnelList;
    DateTime CurrentStartTime ;
    public GameObject scrollView;
    public GameObject coordinate;
    public GameObject PointParent;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        CurrentStartTime = Convert.ToDateTime(DateTime.Now.ToString(("yyyy年MM月dd日")));
        StartcalendarDay.onDayClick.AddListener(ScreenPersonnelStartTime);
        timeDropdown.timeDropdown.onValueChanged.AddListener(ScreenPersonnelMonths);
    }
    int MaxNum = 0;
    public void GetPersonnelMonthsDate(List<PositionList> listInfo)
    {

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
        MaxNum = AllList[0].Count;
        AllList.Sort((x, y) =>
        {
            return x.Name.CompareTo(y.Name);
        });
        DateTime MaxT = Convert.ToDateTime(AllList[AllList.Count -1].Name );
        CurrentTimeText.text = MaxT.ToString(("yyyy年MM月dd日"));
        ScreenPersonnelStartTime(MaxT);   
        timeDropdown.timeDropdown.captionText.text = "一年";
        timeDropdown.timeDropdown.value = 3;
        ScreenPersonnelMonths(3);
    }
   
    public void ScreenPersonnelStartTime(DateTime timeKey)
    {
        NullDate();
        CurrentStartTime = Convert.ToDateTime(timeKey.ToString(("yyyy年MM月dd日")));
         
        ScreenPersonnel();
    }
    int CurrentLevel = 0;
    public void ScreenPersonnelMonths(int level)
    {
        NullDate();
        CurrentLevel = level;
        ScreenPersonnel();
        PersonnelLineChart.pointImageList.Clear ();
    }
    public void NullDate()
    {
        if (PersonnelLineChart.pointImageList.Count != 0)
        {
            PersonnelLineChart.pointImageList.Clear();
            ClearLinePoint();
        }
        DeleteLinePrefabs();
        LineChart_Y_value.DateY(0);
        PersonnelLineChart.yMax = 0;
        List<float> data = new List<float>();
        PersonnelLineChart.UpdateData(data);
        return;
    }
    public void ScreenPersonnel()
    {
        ScreenPersonnelList = new List<PositionList>();
        if (AllList==null)
        {
            NullDate();
           
        }
        if (AllList!=null && AllList.Count == 0)
        {
            NullDate();
        }
        for (int i = 0; i < AllList.Count; i++)
        {
            if (TimeType(AllList[i ].Name ))
            {
                ScreenPersonnelList.Add (AllList[i]);
            }
        }
        if (ScreenPersonnelList.Count == 0)
        {
            NullDate();
        }
        else
        {
            SetLineChart(ScreenPersonnelList);
        }
      
    }
    public void SetLineChart(List<PositionList> PosLIst)
    {
        PosLIst.Sort((x, y) =>
        {
            return x.Count .CompareTo(y.Count);
        });
        DeleteLinePrefabs();
        LineChart_Y_value.DateY(PosLIst[PosLIst.Count -1].Count);
        PersonnelLineChart.yMax = (float)PosLIst[PosLIst.Count - 1].Count;
        List<float> data = new List<float>();
        PosLIst.Sort((x, y) =>
        {
            return x.Name.CompareTo(y.Name);
        });

        DateTime dt = Convert.ToDateTime(PosLIst[PosLIst.Count - 1].Name );
        DateTime MinDt = Convert.ToDateTime(PosLIst[0].Name );
        int DifferencetIME = int.Parse((dt - MinDt).TotalDays.ToString());
        for (int i = 0; i <= DifferencetIME; i++)
        {
            TimeInstantiateLine();
            DateTime HoursAdd = MinDt.AddDays(i);
            string TimeT = HoursAdd.ToString(("yyyy-MM-dd"));
            PositionList item = PosLIst.Find(j => j.Name.Contains(TimeT));
            if (item != null)
            {
                data.Add(float.Parse(item.Count.ToString()));
                //TimeScreenPositionList.Remove(item);
            }
            else
            {
                data.Add(1);//没有数据的日期补上1
            }
        }
        float Width = data.Count * 120f;
        scrollView.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, 375);       
        coordinate.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, 325);
        PersonnelLineChart.width = Width;
        if (PersonnelLineChart.pointImageList.Count != 0)
        {
            PersonnelLineChart.pointImageList.Clear();
            ClearLinePoint();
        }
        SetHourLineChartDate(PosLIst, Personnel_X_Data, data.Count );
        PersonnelLineChart.UpdateData(data);
    }
    public void ClearLinePoint()
    {
        for (int i = PointParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(PointParent.transform.GetChild(i).gameObject);
        }

    }
    private void SetHourLineChartDate(List<PositionList> DataList, UGUI_LineChartDateFill LineChart,int count)
    {

        if (DataList != null)
        {
            //LastTime = long.Parse(DataList[DataList.Count - 1].RecordTime);
            DateTime dt = Convert.ToDateTime(DataList[DataList.Count - 1].Name);
            LineChart.DateFillT(UGUI_LineChartDateFill.DateType.Month, count, dt);
            //if (CurrentLevel == 0)
            //{
            //    LineChart.DateFillT(UGUI_LineChartDateFill.DateType.Month, count, dt);
            //}
            //else if(CurrentLevel == 1)
            //{
            //    LineChart.DateFillT(UGUI_LineChartDateFill.DateType.ThreeMonths  , count, dt);
            //}
            //else if (CurrentLevel == 2)
            //{
            //    LineChart.DateFillT(UGUI_LineChartDateFill.DateType.HalfAYear, count, dt);
            //}
            //else if (CurrentLevel == 3)
            //{
            //    LineChart.DateFillT(UGUI_LineChartDateFill.DateType.year, count, dt);
            //}
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
    bool IsMonth;

    public void ScreenMonths(string time, int Num)
    {
        DateTime EndtTime = CurrentStartTime.AddMonths(-Num);
        DateTime dt = Convert.ToDateTime(time);
        IsMonth = DateTime.Compare(CurrentStartTime, dt) >= 0 && DateTime.Compare(EndtTime, dt) <= 0;
    }
    public bool TimeType(string time)
    {
        if (CurrentLevel == 0)
        {
            ScreenMonths(time, 1);
        }
        else if (CurrentLevel == 1)
        {
            ScreenMonths(time, 3);
        }
        else if (CurrentLevel == 2)
        {
            ScreenMonths(time, 6);
        }
        else if (CurrentLevel == 3)
        {
            ScreenMonths(time, 12);
        }
        if (IsMonth)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Update()
    {

    }
}
