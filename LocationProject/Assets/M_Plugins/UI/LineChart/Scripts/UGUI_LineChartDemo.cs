using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UGUI_LineChartDemo : MonoBehaviour {
    public List<string> strList;

    public static UGUI_LineChartDemo Instance;
    // Use this for initialization
    void Start()
    {
        Instance = this;
        strList = new List<string>();
        strList.Add("111");
        strList.Add("222");
        strList.Add("333");
        strList.Add("444");
        strList.Add("555");

        strList.Add("666");
        strList.Add("777");
        strList.Add("888");
        strList.Add("999");
        random = new System.Random();
    }

    void Update()
    {

    }

    public System.Random random;
    public int Count = 200;
    public List<float> intList;
    public UGUI_LineChart lineChart;
    public void ON_Click()
    {
        intList = new List<float>();
        for (int i = 0; i < Count; i++)
        {
            intList.Add(random.Next(0, 100));
        }

        lineChart.UpdateData(intList);
    }
}
