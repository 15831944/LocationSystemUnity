using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevAlarmTypeStatisticsDetailInfo : MonoBehaviour {

    public Text TypeText;
    public Text NumText;
    public Scrollbar ScrollbarNum;
    void Start () {
		
	}
	
	public void GetDevAlarmTypeStatisticsDetailData(AlarmGroupCount Info,int MaxNum)
    {
        TypeText.text  = Info.Name;
        NumText.text = Info.Count.ToString();
        ScrollbarNum.size = (float )Info.Count / (float )MaxNum;
    }

    void Destroy () {
        Debug.LogError("Destroy Name:"+transform.name);
	}
}
