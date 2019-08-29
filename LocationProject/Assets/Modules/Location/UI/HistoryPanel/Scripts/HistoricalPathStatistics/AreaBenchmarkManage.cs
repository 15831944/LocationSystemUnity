using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaBenchmarkManage : MonoBehaviour {
    public static AreaBenchmarkManage Instance;
    public GameObject AreaBenchmarkWindow;
   
    public UGUI_PieChartDoTween pieChartDoTween;
    List<PositionList> BenchmarkmanageList;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    List<Color> TagColorList;
    Color one = new Color(97 / 255f, 255 / 255f, 149 / 255f, 255 / 255f);
    Color two = new Color(44 / 255f, 153 / 255f, 255 / 255f, 255 / 255f);
    Color three = new Color(242 / 255f, 109 / 255f, 254 / 255f, 255 / 255f);
    Color four = new Color(255 / 255f, 167 / 255f, 89 / 255f, 255 / 255f);
    Color five = new Color(41 / 255f, 205 / 255f, 206 / 255f, 255 / 255f);
    Color six = new Color(65 / 255f, 89 / 255f, 236 / 255f, 255 / 255f);
    Color seven = new Color(242 / 255f, 81 / 255f, 122 / 255f, 255 / 255f);
    Color eight = new Color(214 / 255f, 255 / 255f, 89 / 255f, 255 / 255f);
    Color nine = new Color(144 / 255f, 85 / 255f, 250 / 255f, 255 / 255f);
    Color ten = new Color(62 / 255f, 103 / 255f, 121 / 255f, 255 / 255f);
    void Start () {
        TagColorList = new List<Color>();
    }
    private void Awake()
    {
        Instance = this;
    }
    
    public void GetPieChartData(List<PositionList> InfoList)
    {
        int TotalNum = 0;
        AddColor();
        BenchmarkmanageList = new List<PositionList>();
        for (int i=0;i < InfoList.Count;i++)
        {
            if (i < 10)
            {
                BenchmarkmanageList.Add(InfoList[i ]);
                TotalNum = TotalNum + InfoList[i].Count;
             
            }
        }
     
        ShowPieChart(BenchmarkmanageList, TotalNum);
        
    }
    public void ShowPieChart(List<PositionList> InfoList,int NUM)
    {
        List<float> DateList = new List<float>();
        for (int i=0;i <InfoList.Count;i++)
        {
            float rate = (float)InfoList[i].Count / NUM;
            DateList.Add(rate);
        }
        pieChartDoTween.Show(DateList);
        ShowRate(DateList);
       
    }
    public void ShowRate(List<float> list)
    {
        for (int i=0;i <list .Count;i++)
        {
            GameObject Obj = InstantiateLine();
            PieChartBenchmark item = Obj.GetComponent<PieChartBenchmark>();
            item.ShowPieChartBenchmarkInfo(list[i], BenchmarkmanageList[i ], TagColorList[i ]);
            
        }
    }
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject InstantiateLine()
    {
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
        return o;
    }
    public void AddColor()
    {
      
        TagColorList.Add(one);
        TagColorList.Add(two);
        TagColorList.Add(three);
        TagColorList.Add(four);
        TagColorList.Add(five );
        TagColorList.Add(six );
        TagColorList.Add(seven );
        TagColorList.Add(eight );
        TagColorList.Add(nine );
        TagColorList.Add(ten );
    }
    /// <summary>
    /// 删除Grid下的子物体
    /// </summary>
    public void DeleteLinePrefabs()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
    // Update is called once per frame
    public void ShowAreaBenchmarkWindow(bool b)
    {
        BuildingBenchmark.Instance.CurrentTime = "";
        FloorBenchmark.Instance.CurrentTime = "";
        AreaBenchmarkWindow.SetActive(b);
        DeleteLinePrefabs();
    }

    void Update () {
		
	}
}
