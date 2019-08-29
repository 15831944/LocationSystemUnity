using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBenchmarkItem : MonoBehaviour {
    public Text NameText;
    public Text TagText;
    public Text LocationDataText;
    public Toggle LineTog;
    List<PositionList> FloorPositionList;
    public void ShowBuildingBenchmarkInfo(PositionList info)
    {
        if (string .IsNullOrEmpty (info .Name))
        {
            NameText.text = "--";
        }
        else
        {
            NameText.text = info.Name;
        }
       
        LocationDataText.text = info.Count.ToString();
        LineTog.onValueChanged.AddListener(LineTog_Click);
    }
    public void LineTog_Click(bool b)
    {
        if (b)
        {
            ShowFloorBenchmarkInfo();
        }
    }

    public void ShowFloorBenchmarkInfo()
    {
        CommunicationObject.Instance.GetHistoryPositonStatistics(3, NameText.text, "", "",list =>
        {
            FloorPositionList = list;
            FloorBenchmark.Instance.FloorBenchmarkList(FloorPositionList, NameText.text);
        });

    }
}
