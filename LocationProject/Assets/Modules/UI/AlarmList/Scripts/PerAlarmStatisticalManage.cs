
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerAlarmStatisticalManage : MonoBehaviour
{

    SearchArg arg;
    SearchArg Chartarg;
    public static PerAlarmStatisticalManage Instance;
    List<AlarmGroupCount> PerAlarmCountList;
    List<AlarmLine> PerAlarmLineList;
    public GameObject PerAlarmStatisticalWindow;
    public Button CloseBut;
    public TimeDropdown PerlarmDropdownTop;
    public TimeDropdown PerAlarmLineChartTimeDropdown;
    List<AlarmLinePoint> PerTypePosLIst;//折线图数据
    string TypeName;
    bool IsMonth;
    DateTime CurrentEndTime;//
    string StartTimeLineChart;
    string EndTimeLineChart;
    public bool IsGetPerData;
    public bool IsGetPerChartData;
    void Start()
    {
        CurrentEndTime = DateTime.Now;
        CloseBut.onClick.AddListener(() =>
        {
            ShowPerAlarmStatisticalWindow(false);
        });
        PerlarmDropdownTop.timeDropdown.onValueChanged.AddListener(ScreenPersonnelMonths);
        PerAlarmLineChartTimeDropdown.timeDropdown.onValueChanged.AddListener(ScreenTimeLineChartData);
    }
    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 刚打开界面展示的信息
    /// </summary>
    public void StartShowPerAlarmStatisticalInfo()
    {
        string StartTime = DateTime.Now.Year.ToString() + "年01月01日";
        string EndTime = DateTime.Now.ToString("yyyy年MM月dd日");
        PerlarmDropdownTop.timeDropdown.captionText.text = "一年";
        PerlarmDropdownTop.timeDropdown.value = 3;
        PerAlarmLineChartTimeDropdown.timeDropdown.captionText.text = "一年";
        PerAlarmLineChartTimeDropdown.timeDropdown.value = 3;

        ScreenPersonnelMonths(3);
        ScreenTimeLineChartData(3);
    }
    /// <summary>
    /// 得到设备告警类型数据
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void GetPerAlarmStatisticsData(string start, string end)
    {
        arg = new SearchArg();
        arg.StartTime = start;
        arg.EndTime = end;
        if (!IsGetPerData) return;
        IsGetPerData = false;
        var DevAlarmTypeStatisticsList = CommunicationObject.Instance.GetLocationAlarmStatistics(arg);
        IsGetPerData = true;
        PerAlarmCountList = new List<AlarmGroupCount>();
        PerAlarmCountList.AddRange(DevAlarmTypeStatisticsList.DevTypeAlarms);
        PersonnelAlarmStatistics.Instance.SetDeviceAlarmTypeInfo(PerAlarmCountList);
    }
    public void ScreenPersonnelMonths(int level)
    {
        PersonnelAlarmStatistics.Instance.DelectItem();
        string StartTime = TimeType(level).ToString() + "年01月01日";
        string EndTime = CurrentEndTime.ToString("yyyy年MM月dd日");
        GetPerAlarmStatisticsData(StartTime, EndTime);
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
    /// <summary>
    /// 得到所有设备告警折线图数据信息
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void GetDevTypeAlarmLineData(string start, string end)
    {
        Chartarg = new SearchArg();
        Chartarg.StartTime = start;
        Chartarg.EndTime = end;
        if (!IsGetPerChartData) return;
        IsGetPerChartData = false;
        var DevAlarmTypeStatisticsList = CommunicationObject.Instance.GetLocationAlarmStatistics(arg);
        IsGetPerChartData = true;
        PerAlarmLineList = new List<AlarmLine>();
        PerAlarmLineList.AddRange(DevAlarmTypeStatisticsList.Lines);
        List<AlarmLinePoint> prePosLIst = new List<AlarmLinePoint>();
        prePosLIst.AddRange(PerAlarmLineList[0].Points);
        PerAlarmStatisticsLineChart.Instance.SetLineChart(prePosLIst);
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
    public void ShowPerAlarmStatisticalWindow(bool b)
    {
        PerAlarmStatisticalWindow.SetActive(b);
        IsGetPerData = true;
        IsGetPerChartData = true;
        if (b == false)
        {
            PersonnelAlarmStatistics.Instance.DelectItem();
            PersonnelAlarmList.Instance.ShowPersonAlarmUI();
        }
        else
        {
            StartShowPerAlarmStatisticalInfo();
        }
    }
}
