using UnityEngine;
using System.Collections;
using System;

public class UGUI_LineChartDateFillTest : MonoBehaviour {
    public UGUI_LineChartDateFill DateFill;
	// Use this for initialization
	void Start () {
        DateFill = transform.GetComponent<UGUI_LineChartDateFill>();
        Test();
    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }
	}
    public UGUI_LineChartDateFill.DateType CurrentType;
    private void Test()
    {
        if(DateFill==null)
        {
            Debug.LogError("UGUI_LineChartDateFill is null...");
        }
        DateTime CurrentTime = DateTime.Now;
        if (CurrentType == UGUI_LineChartDateFill.DateType.Hour)
        {
            DateFill.DateFill(UGUI_LineChartDateFill.DateType.Hour, 6, CurrentTime);
        }
        else if (CurrentType == UGUI_LineChartDateFill.DateType.Day)
        {
            DateFill.DateFill(UGUI_LineChartDateFill.DateType.Day, 6, CurrentTime);
        }
        else if (CurrentType == UGUI_LineChartDateFill.DateType.Week)
        {
            DateFill.DateFill(UGUI_LineChartDateFill.DateType.Week, 7, CurrentTime);
        }
        else
        {
            DateFill.DateFill(UGUI_LineChartDateFill.DateType.Month, 10, CurrentTime);
        }
    }
}
