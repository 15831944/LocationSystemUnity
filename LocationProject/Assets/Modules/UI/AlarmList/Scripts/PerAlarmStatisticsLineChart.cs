using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerAlarmStatisticsLineChart : MonoBehaviour {

    public static PerAlarmStatisticsLineChart Instance;
    public GameObject scrollView;
    public GameObject coordinate;
    public GameObject PointParent;
    public GameObject PerAlarm_X_Grid;
    public GameObject PerAlarm_XObj;
    public UGUI_LineChart PerAlarmLineChart;
    public UGUI_LineChartDateFill PerAlarm_X_Data;
    public UGUI_LineChartYValue LineChart_Y_value;
    List<AlarmLine> DevAlarmList;
    public GameObject UGUI_LineChartObj;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }
    public void SetLineChart(List<AlarmLinePoint> PosLIst)
    {
        PosLIst.Sort((x, y) =>
        {
            return x.Value.CompareTo(y.Value);
        });
        DeleteLinePrefabs();
        LineChart_Y_value.DateY((int)PosLIst[PosLIst.Count - 1].Value);
        PerAlarmLineChart.yMax = (float)PosLIst[PosLIst.Count - 1].Value;
        List<float> data = new List<float>();
        PosLIst.Sort((x, y) =>
        {
            return x.Key.CompareTo(y.Key);
        });

        DateTime dt = Convert.ToDateTime(PosLIst[PosLIst.Count - 1].Key);
        DateTime MinDt = Convert.ToDateTime(PosLIst[0].Key);
        int DifferencetIME = int.Parse((dt - MinDt).TotalDays.ToString());
       
        for (int i = 0; i <= DifferencetIME; i++)
        {
            TimeInstantiateLine();
            DateTime HoursAdd = MinDt.AddDays(i);
            string TimeT = HoursAdd.ToString(("yyyy-MM-dd"));
            AlarmLinePoint item = PosLIst.Find(j => j.Key.Contains(TimeT));
            if (item != null)
            {
                data.Add(float.Parse(item.Value.ToString()));
                //TimeScreenPositionList.Remove(item);
            }
            else
            {
                data.Add(1);//没有数据的日期补上1
            }
        }
        if (data.Count == 1)
        {
            TimeInstantiateLine();
            data.Insert(0, 1);
        }
        float Width = data.Count * 120f;
        scrollView.transform.GetComponent<RectTransform>().sizeDelta = new Vector2((data.Count + 1) * 120f, 500);
        coordinate.transform.GetComponent<RectTransform>().sizeDelta = new Vector2((data.Count + 1) * 120f, 445);
        UGUI_LineChartObj.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(((data.Count - 1) * 120f), 440);
        PerAlarmLineChart.width = ((data.Count - 1) * 120f);
        if (PerAlarmLineChart.pointImageList.Count != 0)
        {
            PerAlarmLineChart.pointImageList.Clear();
            ClearLinePoint();
        }
        SetHourLineChartDate(dt, data.Count);
        PerAlarmLineChart.UpdateData(data);
    }
    private void SetHourLineChartDate(DateTime dt, int count)
    {
        PerAlarm_X_Data.DateFillT(UGUI_LineChartDateFill.DateType.Month, count, dt.AddDays (1));

    }
    public GameObject TimeInstantiateLine()
    {
        GameObject o = Instantiate(PerAlarm_XObj);
        o.SetActive(true);
        o.transform.parent = PerAlarm_X_Grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
        return o;
    }
    public void NullDate()
    {
        if (PerAlarmLineChart.pointImageList.Count != 0)
        {
            PerAlarmLineChart.pointImageList.Clear();
            ClearLinePoint();
        }
        DeleteLinePrefabs();
        LineChart_Y_value.DateY(0);
        PerAlarmLineChart.yMax = 0;
        List<float> data = new List<float>();
       PerAlarmLineChart.UpdateData(data);
        return;
    }
    public void DeleteLinePrefabs()
    {
        for (int j = PerAlarm_X_Grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(PerAlarm_X_Grid.transform.GetChild(j).gameObject);
        }
    }
    public void ClearLinePoint()
    {
        for (int i = PointParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(PointParent.transform.GetChild(i).gameObject);
        }

    }
}
