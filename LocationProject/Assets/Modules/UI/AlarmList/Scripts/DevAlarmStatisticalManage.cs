
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevAlarmStatisticalManage : MonoBehaviour
{
    SearchArg arg;
    SearchArg argChart;
    public static DevAlarmStatisticalManage Instance;
    List<AlarmGroupCount> DevTypeAlarmCountList;
    List<AlarmLine> DevTypeAlarmLineList;
    public GameObject DevAlarmStatisticalWindow;
    public Button CloseBut;
    public TimeDropdown DevAlarmTypeDropdown;
    public DevAlarmLineChartDropdown devAlarmLineChartDropdown;
    public TimeDropdown DevAlarmLineChartTimeDropdown;
    public List<string> TypeNameList;//告警类型名称
    List<AlarmLinePoint> TypePosLIst;//折线图数据
    string TypeName;
    bool IsMonth;
    DateTime CurrentEndTime;//
    string StartTimeLineChart;
    string EndTimeLineChart;
    public bool IsGetDevData;
    public bool IsGetDevChartData;
    void Start()
    {
        CurrentEndTime = DateTime.Now;
        CloseBut.onClick.AddListener(() =>
        {
            ShowDevAlarmStatisticalWindow(false);
        });
        DevAlarmTypeDropdown.timeDropdown.onValueChanged.AddListener(ScreenDevMonths);
        devAlarmLineChartDropdown.TypeDropdown.onValueChanged.AddListener(ScreenTypeLineChartData);
        DevAlarmLineChartTimeDropdown.timeDropdown.onValueChanged.AddListener(ScreenTimeLineChartData);
    }
    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 得到设备告警类型数据
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void GetDevAlarmTypeStatisticsData(string start, string end)
    {
        arg = new SearchArg();
        arg.StartTime = start;
        arg.EndTime = end;
        if (!IsGetDevData) return;
        IsGetDevData = false;
        var DevAlarmTypeStatisticsList = CommunicationObject.Instance.GetDevAlarmStatistics(arg);
        IsGetDevData = true;
        DevTypeAlarmCountList = new List<AlarmGroupCount>();
        DevTypeAlarmLineList = new List<AlarmLine>();
        DevTypeAlarmCountList.AddRange(DevAlarmTypeStatisticsList.DevTypeAlarms);
        DevAlarmTypeStatistics.Instance.SetDeviceAlarmTypeInfo(DevTypeAlarmCountList);
    }

    /// <summary>
    /// 得到设备告警类型折线图数据
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void GetDevAlarmTypeStatisticsChartData(string start, string end)
    {
        argChart = new SearchArg();
        argChart.StartTime = start;
        argChart.EndTime = end;
        if (!IsGetDevData) return;
        IsGetDevChartData = false;
        var DevAlarmTypeStatisticsList = CommunicationObject.Instance.GetDevAlarmStatistics(arg);
        IsGetDevChartData = true;
        DevTypeAlarmLineList = new List<AlarmLine>();
        if (DevAlarmTypeStatisticsList.Lines == null)
        {
            DevAlarmStatisticsLineChart.Instance.NullDate();
        }
        else
        {
            DevTypeAlarmLineList.AddRange(DevAlarmTypeStatisticsList.Lines);
        }
        TypeNameList = new List<string>();
        TypeNameList.AddRange(DevAlarmTypeStatisticsList.itemList);
        devAlarmLineChartDropdown.AddName();
        TypeName = TypeNameList[Value];
        devAlarmLineChartDropdown.TypeDropdown.captionText.text = TypeName;
        TypePosLIst = new List<AlarmLinePoint>();
        foreach (var item in DevTypeAlarmLineList)
        {
            if (TypeName == item.Name)
            {
                TypePosLIst.AddRange(item.Points);
            }
        }
        if (TypePosLIst.Count == 0)
        {
            DevAlarmStatisticsLineChart.Instance.NullDate();
        }
        else
        {
            DevAlarmStatisticsLineChart.Instance.SetLineChart(TypePosLIst);
        }
        
    }
    public void ScreenDevMonths(int level)
    {
        DevAlarmTypeStatistics.Instance.DelectItem();
        string StartTime = TimeType(level).ToString() + "年01月01日";
        string EndTime = CurrentEndTime.ToString("yyyy年MM月dd日");
        GetDevAlarmTypeStatisticsData(StartTime, EndTime);
    }
    /// <summary>
    /// 刚打开界面展示的信息
    /// </summary>
    public void StartShowDevAlarmStatisticalInfo()
    {
        string StartTime = DateTime.Now.Year.ToString() + "年01月01日";
        string EndTime = DateTime.Now.ToString("yyyy年MM月dd日");
        ScreenDevMonths(3);

        ScreenTimeLineChartData(3);
    }
    /// <summary>
    /// 得到所有设备告警折线图数据信息
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void GetDevTypeAlarmLineData(string start, string end)
    {
        arg = new SearchArg();
        arg.StartTime = start;
        arg.EndTime = end;
        GetDevAlarmTypeStatisticsChartData(start, end);

    }
    int Value = 0;
    /// <summary>
    /// 折线图设备类型筛选
    /// </summary>
    /// <param name="value"></param>
    public void ScreenTypeLineChartData(int value)
    {
        Value = value;
        GetDevTypeAlarmLineData(StartTimeLineChart, EndTimeLineChart);
    }
    /// <summary>
    /// 折线图时间类型筛选
    /// </summary>
    /// <param name="value"></param>
    public void ScreenTimeLineChartData(int value)
    {
        StartTimeLineChart = TimeType(value).ToString() + "年01月01日";
        EndTimeLineChart = CurrentEndTime.ToString("yyyy年MM月dd日");
        GetDevTypeAlarmLineData(StartTimeLineChart, EndTimeLineChart);
    }
    public DateTime TimeType(int level)
    {
        DateTime StartTime = DateTime.Now;
        if (level == 0)
        {
            StartTime = CurrentEndTime.AddMonths(-1);

        }
        else if (level == 1)
        {
            StartTime = CurrentEndTime.AddMonths(-3);
        }
        else if (level == 2)
        {
            StartTime = CurrentEndTime.AddMonths(-6);

        }
        else if (level == 3)
        {
            StartTime = CurrentEndTime.AddMonths(-12);
        }
        return StartTime;
    }


    public void ShowDevAlarmStatisticalWindow(bool b)
    {
        DevAlarmStatisticalWindow.SetActive(b);
        IsGetDevData = true;
        IsGetDevChartData = true;
        if (b == false)
        {
            DevAlarmTypeStatistics.Instance.DelectItem();
            DevAlarmListManage.Instance.ShowDevAlarmWindow();
        }
        else
        {
            StartShowDevAlarmStatisticalInfo();
        }
    }

    void Update()
    {

    }
}
