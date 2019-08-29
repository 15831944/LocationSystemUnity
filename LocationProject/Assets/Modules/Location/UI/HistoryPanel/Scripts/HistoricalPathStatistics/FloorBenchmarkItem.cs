using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorBenchmarkItem : MonoBehaviour {

    public Text NumText;
    public Text LocationDataText;
    public Text NameText;
    public Toggle LineTog;
    List<PositionList> PersonnelPositionList;
    List<PositionList> PersonnelPositionListMonths;
    string FloorName;
    string PerName;

    public void ShowFloorBenchmarkInfo(PositionList info, string Name, List<PositionList> PosList)
    {
       
        FloorName = Name;
        PerName = info.Name;
        PersonnelPositionListMonths = new List<PositionList>();
        PersonnelPositionListMonths.AddRange(PosList);
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
                NumText.text = "--";
            }
            else
            {
                NumText.text = array[1];
            }

        }     
        LocationDataText.text = info.Count.ToString();
        LineTog.onValueChanged.AddListener(LineTog_Click);
    }
    public void LineTog_Click(bool b)
    {
        if (b)
        {
            ShowPersonnelBenchmarkInfo();
        }
       
    }
    public void ShowPersonnelBenchmarkInfo()
    {
             CommunicationObject.Instance.GetHistoryPositonStatistics(3, FloorName, PerName,"", list =>
            {
                PersonnelPositionList = list;
                AreaPersonnelBenchmarkOneDay.Instance.GetOneDayPersonnelDate(PersonnelPositionList, FloorName, PerName);
                AreaPersonnelBenchmarkMonths.Instance.GetPersonnelMonthsDate(PersonnelPositionList);
            });
       
    }
}
