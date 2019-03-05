using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullViewTime : MonoBehaviour {
    /// <summary>
    /// 星期数组
    /// </summary>
    private string[] Day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
    /// <summary>
    /// 获取当前星期
    /// </summary>
    private string Week
    {
        get
        {
            return Day[Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d"))].ToString();
        }
    }
    /// <summary>
    /// 小时，分钟
    /// </summary>
    public Text Time_Hour;
    /// <summary>
    /// 年月日，星期
    /// </summary>
    public Text Time_Year;
	// Use this for initialization
	void Awake () {
        SceneEvents.FullViewStateChange += OnFullViewChange;
	}
	void OnDestroy()
    {
        SceneEvents.FullViewStateChange -= OnFullViewChange;
    }
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 进入/退出全景模式
    /// </summary>
    /// <param name="isFullView"></param>
    private void OnFullViewChange(bool isFullView)
    {
        if(isFullView)
        {
            ShowCurrentTime();
            int startTime = 60 - DateTime.Now.Second;
            //Debug.Log("current Second:"+DateTime.Now.Second);
            InvokeRepeating("ShowCurrentTime",startTime,30);
        }
        else
        {
            if(IsInvoking("ShowCurrentTime"))
            {
                //Debug.Log("Cancel Time Show.");
                CancelInvoke("ShowCurrentTime");
            }
        }
    }
    /// <summary>
    /// 显示当前时间
    /// </summary>
    private void ShowCurrentTime()
    {
        DateTime now = DateTime.Now;
        //Time_Hour.text = string.Format("{0}:{1}",now.Hour,now.Minute);
        Time_Hour.text = now.ToString("HH:mm");
        string date = now.ToString("yyyy/MM/dd");
        //Time_Year.text = string.Format("{0},{1}/{2}/{3}",Week,now.Year,now.Month,now.Day);
        Time_Year.text = string.Format("{0},{1}", Week,date);
    }
}
