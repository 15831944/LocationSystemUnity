using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelTimeBenchmarkItem : MonoBehaviour {

    public PositionList Info;

    public Text NameText;
    public Text TagText;
    public Text LocationDataText;
    public Toggle LineTog;
    public UGUI_LineChart TimeLineChart;
    public UGUI_LineChartDateFill TimeData;
    List<PositionList> PersonnelPositionList;
    string InfoName;
    void Start()
    {

    }
    string str;
    public void ShowPersonnelBenchmarkInfo(PositionList info,string StrText)
    {
        Info = info;
        InfoName = info.Name;
        str = StrText;
        var array = info.Name .Split(new char[2] { '(', ')' });
        if (array != null && array.Length > 2)
        {
            if (string .IsNullOrEmpty(array[0]))
            {
                NameText.text = "--";
            }
            else
            {
                NameText.text = array[0] ;
            }
            if (string.IsNullOrEmpty(array[0]))
            {
                TagText.text = "--";
            }
             else
            {
                TagText.text = array[1];
            }   
        }
        LocationDataText.text = info.Count.ToString();
        LineTog.onValueChanged.RemoveAllListeners();
        LineTog.onValueChanged.AddListener(ShowPersonnelDayLineChart);
       
    }

    public bool IsSelected = false;
    public void ShowPersonnelDayLineChart(bool b)
    {
        IsSelected = b;
        if (b)
        {
            CommunicationObject.Instance.GetHistoryPositonStatistics(1, str, InfoName,"", list =>
            {
                PersonnelPositionList = list;
                ShowLineChartInfo(PersonnelPositionList, NameText.text );
            });

        }

    }

    public void ShowLineChartInfo(List<PositionList> PosList,string NAME)
    {

       PersonnelTimeBenchmark.Instance.ShowLineChartInfo(PosList, NAME);
    }
  
    // Update is called once per frame
    void Update () {
		
	}
}
