using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AreaPersonnelBenchmark : MonoBehaviour {

    public static AreaPersonnelBenchmark Instance;
   
    public UGUI_LineChart PersonnelLineChart;
    public UGUI_LineChartDateFill Personnel_X_Data;
    public UGUI_LineChartYValue LineChart_Y_value;

    public GameObject Per_X_Grid;
    public GameObject Per_XObj;
    public List<PositionList> ShowList;
    List<PositionList> AllPositionList;
    void Start()
    {
     
    }
    private void Awake()
    {
        Instance = this;
    }
    public void AreaPersonnelBenchmarkList(List<PositionList> PosList)
    {
        DeleteLinePrefabs();
        AllPositionList = new List<PositionList>();
        for (int i = 0; i < PosList.Count; i++)
        {
            if (PosList[i].Name.Contains("1970") || PosList[i].Name.Contains("1974"))
            {
                Debug.Log("数据有问题，去掉1970年的和1974年的");
            }
            else
            {
                AllPositionList.Add(PosList[i]);
                TimeInstantiateLine();
            }

        }
        AllPositionList.Sort((x, y) =>
        {
            return x.Name.CompareTo(y.Name);
        });
        SetHourLineChartDate(AllPositionList, Personnel_X_Data);
        List<float> data = new List<float>();
        DateTime dt = Convert.ToDateTime(AllPositionList[0].Name);
        DateTime day = dt.AddDays(10);
        for (int i = 0; i <= 31; i++)
        {
            DateTime MonthsLess = day.AddDays(-i);
            string timeT = MonthsLess.ToString(("yyyy-MM-dd"));
            PositionList item = AllPositionList.Find(j => j.Name.Contains(timeT));
            if (item != null)
            {
                data.Add(float.Parse(item.Count.ToString()));
                //TimeScreenPositionList.Remove(item);
            }
            else
            {
                data.Add(1);
            }
        }

        AllPositionList.Sort((x, y) =>
        {
            return x.Count.CompareTo(y.Count);
        });

        LineChart_Y_value.DateY(AllPositionList[AllPositionList.Count - 1].Count);
        PersonnelLineChart.yMax = (float)AllPositionList[AllPositionList.Count - 1].Count;
        PersonnelLineChart.UpdateData(data);


    }
    private void SetHourLineChartDate(List<PositionList> DataList, UGUI_LineChartDateFill LineChart)
    {
        //var array = DataList[0].Name.Split(new char[1] { ' ' });
        //if (array != null && array.Length == 2)
        //{
        if (DataList != null)
        {
            //LastTime = long.Parse(DataList[DataList.Count - 1].RecordTime);
            DateTime dt = Convert.ToDateTime(DataList[DataList.Count - 1].Name);
            LineChart.DateFill(UGUI_LineChartDateFill.DateType.Month, 7, dt);
            // }

        }

    }
    public GameObject TimeInstantiateLine()
    {
        GameObject o = Instantiate(Per_XObj);
        o.SetActive(true);
        o.transform.parent = Per_X_Grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
        return o;
    }
    public void DeleteLinePrefabs()
    {
        for (int j = Per_X_Grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(Per_X_Grid.transform.GetChild(j).gameObject);
        }
    }
}
