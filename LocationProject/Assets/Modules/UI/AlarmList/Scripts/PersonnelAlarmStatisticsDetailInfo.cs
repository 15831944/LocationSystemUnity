using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelAlarmStatisticsDetailInfo : MonoBehaviour {

    public Text TypeText;
    public Text NumText;
    public Scrollbar ScrollbarNum;
    void Start()
    {

    }

    public void GetPersonnelAlarmStatisticsDetailData(AlarmGroupCount Info, int MaxNum)
    {
        TypeText.text = Info.Name;
        NumText.text = Info.Count.ToString();
        ScrollbarNum.size = (float)Info.Count / (float)MaxNum;
    }


    // Update is called once per frame
    void Update () {
		
	}
}
