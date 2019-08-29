using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChartBenchmark : MonoBehaviour {
    public Text oneText;
    public Image tagImage;
    // Use this for initialization
    void Start () {
		
	}
	public void ShowPieChartBenchmarkInfo(float list, PositionList InfoList, Color colorImage)
    {
        oneText.text = InfoList.Name + "  " + Math.Round(list, 2) * 100 + "%";
        tagImage.color = colorImage;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
