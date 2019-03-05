using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UGUI_LineChartDateFill : MonoBehaviour {
    public enum DateType
    {        
        Day,
        Week,
        Month,
        Hour
    }
	// Use this for initialization
	void Start () {
        
    }    
    /// <summary>
    /// 填充表格日期
    /// </summary>
    /// <param name="Type">时间类型</param>
    /// <param name="Count">时间分段</param>
    /// <param name="CurrentTime">当前时间</param>
    public void DateFill(DateType Type,int Count,DateTime CurrentTime)
    {
        long TimeStamps = ConvertDateTimeLong(CurrentTime);
        List<DateTime>TimeList= GetTime(Type,Count,TimeStamps);
        ShowDate(TimeList,Type);
    }
    /// <summary>
    /// 填充表格日期
    /// </summary>
    /// <param name="Type">时间类型</param>
    /// <param name="Count">时间分段</param>
    /// <param name="Timestamps">当前时间（时间戳）</param>
    public void DateFill(DateType Type,int Count,long Timestamps)
    {
        List<DateTime> TimeList = GetTime(Type, Count, Timestamps);
        ShowDate(TimeList,Type);
    }
    /// <summary>
    /// 显示时间数据
    /// </summary>
    /// <param name="DateTimes"></param>
    /// <param name="type"></param>
    private void ShowDate(List<DateTime> DateTimes, DateType type)
    {
        string format = GetDateStringFormat(type);
        int TextCount = transform.childCount;
        if (TextCount == 0)
        {
            Debug.LogError("Date Text is null..");
            return;
        }
        int TimeListCount = 0;
        for (int i = TextCount; i > 0; i--)
        {
            if (TimeListCount >= DateTimes.Count) break;
            Text t= transform.GetChild(i-1).GetComponentInChildren<Text>();
            t.text = DateTimes[TimeListCount].ToString(format);
            TimeListCount++;
        }
    }
    /// <summary>
    /// 获取时间显示格式
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetDateStringFormat(DateType type)
    {
        if(type==DateType.Hour)
        {
            return "HH:mm";
        }
        else if(type==DateType.Day)
        {
            return "dd日HH:mm";
        }else if(type==DateType.Week)
        {
            return "MM月dd日";
        }
        else
        {
            return "yyyy/MM/dd";
        }
    }
    private long HourSecond = 3600;
    private long DaySecond = 86400;
    private long WeekSecond = 604800;
    private long MonthSecond = 2592000;
    /// <summary>
    /// 填充时间（日）
    /// </summary>
    /// <param name="Type">时间类型</param>
    /// <param name="Count">时间分段</param>
    /// <param name="TimeStamp">时间戳</param>
    public List<DateTime> GetTime(DateType Type, int Count, long TimeStamp)
    {
        long TimeInterval;
        if(Type==DateType.Hour)
        {
            TimeInterval = HourSecond / Count;
        }
        else if (Type==DateType.Day)
        {
            TimeInterval = DaySecond / Count;
        }
        else if(Type==DateType.Week)
        {
            TimeInterval = WeekSecond / Count;
        }
        else
        {
            TimeInterval = MonthSecond / Count;
        }        
        List<DateTime> Times = new List<DateTime>();
        Times.Add(NormalizeTimpstamp(TimeStamp));
        for(int i=0;i<Count;i++)
        {
            TimeStamp -= TimeInterval;
            Times.Add(NormalizeTimpstamp(TimeStamp));
        }
        return Times;
    }
    static DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
    /// <summary>
    /// 将时间戳转换为DateTime
    /// </summary>
    /// <param name="timpStamp"></param>
    /// <returns></returns>
    public static DateTime NormalizeTimpstamp(long timpStamp)
    {
        long unixTime = timpStamp * 10000000L;
        TimeSpan toNow = new TimeSpan(unixTime);
        DateTime dt = dtStart.Add(toNow);
        return dt;
    }
    /// <summary>
    /// DateTime转换为时间戳
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static long ConvertDateTimeLong(DateTime time)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(dtStart);
        return (long)(time - startTime).TotalSeconds;
    }
}
