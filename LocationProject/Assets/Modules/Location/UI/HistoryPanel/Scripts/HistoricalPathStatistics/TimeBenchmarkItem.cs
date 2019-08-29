using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBenchmarkItem : MonoBehaviour
{

    public PositionList Info;

    public Text TimeText;
    public Text LocationDataText;
    public Toggle LineTog;
    public List<PositionList> PersonnelPositionList;
    void Start () {
		
	}
	public void ShowTimeBenchmarkInfo(PositionList info)
	{
	    this.Info = info;
        if(string .IsNullOrEmpty(info.Name))
        {
            TimeText.text = "--";
        }else
        {
            TimeText.text = info.Name;
        }
        LocationDataText.text = info.Count.ToString();
        LineTog.onValueChanged.AddListener(ShowPersonnelTimeBenchmarkInfo);
    }

    public bool IsSelected = false;

    public void ShowPersonnelTimeBenchmarkInfo(bool b)
    {
        IsSelected = b;
        if (b)
        {
            CommunicationObject.Instance.GetHistoryPositonStatistics(1, TimeText.text, "", "",list =>
            {
                PersonnelPositionList = list;
                PersonnelTimeBenchmark.Instance.GetTimeBenchmarkList(PersonnelPositionList, TimeText.text);               
            });
        }
    }

    void Update () {
		
	}
}
