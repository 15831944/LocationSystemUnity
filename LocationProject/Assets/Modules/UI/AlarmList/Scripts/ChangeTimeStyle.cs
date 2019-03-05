using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTimeStyle : MonoBehaviour {

    public Text StartTimeText;
    public CalendarChange StartcalendarDay;

    void Start () {
        StartcalendarDay.onDayClick.AddListener(StartTimeStyle);
    }
    public void StartTimeStyle(DateTime dateTime)
    {
        DateTime startTime = Convert.ToDateTime(dateTime);
        
        string newStartTime = startTime.ToString("yyyy年MM月dd日");
        StartTimeText.text = newStartTime;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
