using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTime : MonoBehaviour {
    public static StartTime instance;
    public Text StartTimeText;
    public CalendarChange StartcalendarDay;
   
    void Start () {
        ShowStartTime();
        StartcalendarDay.onDayClick.AddListener(StartTimeStyle);

    }
    public void StartTimeStyle(DateTime dateTime)
    {
        DateTime startTime = Convert.ToDateTime(dateTime);
        string newStartTime = startTime.ToString("yyyy年MM月dd日");
        StartTimeText.text = newStartTime;
    }

   void Update () {
		
	}
    public void ShowStartTime()
    {
        StartTimeText.text = DateTime.Now.Year.ToString() + "年01月01日"; ;
    }
}
