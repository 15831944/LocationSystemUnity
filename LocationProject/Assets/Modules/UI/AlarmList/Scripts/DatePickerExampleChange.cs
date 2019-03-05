using SpringGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatePickerExampleChange : MonoBehaviour {

        public DatePicker DatePicker = null;
        public CalendarChange Calendar = null;

        public void Awake()
        {
            Calendar.onDayClick.AddListener(time =>
            {
                Debug.Log(string.Format("Today is {0}Yeah{1}Month{2}Day",
                    time.Year, time.Month, time.Day));
            });
            Calendar.onMonthClick.AddListener(time =>
            {
                Debug.Log(string.Format("This month is {0}Yeah{1}Month",
                time.Year, time.Month));
            });
            Calendar.onYearClick.AddListener(time =>
            {
                Debug.Log(string.Format("This yeah{0}Yeah", time.Year));
            });
        }
    }