using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelBenchmarkItem : MonoBehaviour {
    public Text NameText;
    public Text TagText;
    public Text LocationDataText;
    public Toggle LineTog;
    string NameStr;
    List<PositionList> BenchmarkmanageList;
    public void ShowPersonnelBenchmarkInfo(PositionList info)
    {
        NameStr = info.Name;
        var array = info.Name.Split(new char[2] { '(', ')' });
        if (array != null && array.Length > 2)
        {
            if (string.IsNullOrEmpty(array[0]))
            {
                NameText.text = "--";
            }
            else
            {
                NameText.text = array[0];
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
        LineTog.onValueChanged.AddListener(ShowPersonnelChart_Line);
      
    }

    public void ShowPersonnelChart_Line(bool b)
    {
        if (b)
        {
            CommunicationObject.Instance.GetHistoryPositonStatistics(2, NameStr, "", "",list =>
            {
                BenchmarkmanageList = list;
                PersonnelBenchmarkOneDay.Instance.GetOneDayPersonnelDate(BenchmarkmanageList, NameStr);
                PersonnelBenchmarkMonths.Instance . GetPersonnelMonthsDate(BenchmarkmanageList);
            });

        }
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
