using Location.WCFServiceReferences.LocationServices;
using SpringGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelTimeBenchmark : MonoBehaviour
{
    public static PersonnelTimeBenchmark Instance;

    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;

    /// <summary>
    /// 每页显示的条数
    /// </summary>
    const int pageSize = 8;
    /// <summary>
    /// 数据
    /// </summary>
    private int StartPageNum = 0;
    /// <summary>
    /// 页数
    /// </summary>
    private int PageNum = 1;
    /// <summary>
    /// 总页数
    /// </summary>
    public Text pegeTotalText;
    /// <summary>
    /// 输入页数
    /// </summary>
    public InputField pegeNumTex;
    /// <summary>
    /// 下一页
    /// </summary>
    public Button AddPageBut;

    /// <summary>
    /// 上一页
    /// </summary>
    public Button MinusPageBut;
    public Sprite DoubleImage;
    public Sprite OddImage;
    public List<PositionList> ShowList;
    List<PositionList> AllPositionList;
    public BarChart PersonnelLineChart;
    public UGUI_LineChartDateFill Personnel_X_Data;
    public UGUI_LineChartYValue LineChart_Y_value;
    List<PositionList> PersonnelScreenPositionList;
  
    public GameObject Per_X_Grid;
    public GameObject Per_XObj;
    public Text promptText;
  
    string PerName;
  public string   CurrentTime ;
    void Start()
    {
        AddPageBut.onClick.AddListener(AddPerTimeBenchmarkPage);
        MinusPageBut.onClick.AddListener(MinusPerTimeBenchmarkPage);
        pegeNumTex.onValueChanged.AddListener(InputPerTimeBenchmarkPages);
    }
    private void Awake()
    {
        Instance = this;
    }
    string StrText;
    public void GetTimeBenchmarkList(List<PositionList> PosList, string str)
    {
        StrText = str;
       
        AllPositionList = new List<PositionList>();
        AllPositionList.AddRange(PosList);
        GetPageData(PosList);
        TotaiLine();
        pegeNumTex.text = "1";
    }
    public void ShowLineChartInfo(List<PositionList> PosList,string Name)
    {
        PerName = Name;
        promptText.text = PerName + Convert.ToDateTime(StrText).ToString("yyyy年MM月dd日") + "定位数据量折线图"; ;
        DeleteLinePrefabsX();
        if (PosList == null) return;
        if (PosList.Count == 0) return;
        PersonnelScreenPositionList = new List<PositionList>();
        for (int i = 0; i < PosList.Count; i++)
        {
            if (PosList[i].Name.Contains("1970") || PosList[i].Name.Contains("1974"))
            {
                Debug.Log("数据有问题，去掉1970年的和1974年的");
            }
            else
            {
                PersonnelScreenPositionList.Add(PosList[i]);
            }
        }
        PersonnelScreenPositionList.Sort((x, y) =>
        {
            return x.Count.CompareTo(y.Count);
        });//根据数量排列
        LineChart_Y_value.DateY(PersonnelScreenPositionList[PersonnelScreenPositionList.Count -1].Count);
        int MaxNum = PersonnelScreenPositionList[PersonnelScreenPositionList.Count - 1].Count;
        List<float> data = new List<float>();    
        PersonnelScreenPositionList.Sort((x, y) =>
        {
            return x.Name.CompareTo(y.Name);
        });
       
       
      
        DateTime dt = Convert.ToDateTime(PersonnelScreenPositionList[PersonnelScreenPositionList.Count - 1].Name + ":00:00");
        string DT = dt.ToString(("yyyy-MM-dd"));
        DateTime MinDt = Convert.ToDateTime(DT);
        int DifferencetIME = 24;
        for (int i = 0; i < DifferencetIME; i++)
        {
            TimeInstantiateLine();
            DateTime HoursAdd = MinDt.AddHours(i);
            string TimeT = HoursAdd.ToString(("yyyy-MM-dd HH"));
            PositionList item = PersonnelScreenPositionList.Find(j => j.Name.Contains(TimeT));
            if (item != null)
            {
                data.Add((float)item.Count / MaxNum);
                //TimeScreenPositionList.Remove(item);
            }
            else
            {
                data.Add(0);//没有数据的日期补上0
            }
        }

        SetHourLineChartDate(PersonnelScreenPositionList, MinDt, 24 );
        PersonnelLineChart.Inject(data);
        PersonnelLineChart.enabled = false;//这样处理不用点击一下Inspector里面的东西，柱状图才可以出来
        PersonnelLineChart.enabled = true;

    }
   
    /// <summary>
    /// 设置折线图日期
    /// </summary>
    /// <param name="DataList"></param>
    private void SetHourLineChartDate(List<PositionList> DataList, DateTime DT,  int Num)
    {
        if (DataList != null)
        {
            
          
            Personnel_X_Data.DateFillT(UGUI_LineChartDateFill.DateType.Day, Num, DT);
        }
    }
    public void JudgeCurrentSelectItem()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Toggle Items = grid.transform.GetChild(i).GetComponent<Toggle>();
            if (Items.isOn == true)
            {
                CurrentTime = grid.transform.GetChild(i).GetChild(0).GetComponent<Text>().text;
            }
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

    private List<PersonnelTimeBenchmarkItem> Items = new List<PersonnelTimeBenchmarkItem>();

    public PersonnelTimeBenchmarkItem GetSelectedItem()
    {
        var item = Items.Find(i => i != null && i.Info != null && i.IsSelected);
        return item;
    }

    public void ShowTimeBenchmarkInfo(List<PositionList> PosList)
    {
        DeleteLinePrefabs();
        for (int i = 0; i < PosList.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            PersonnelTimeBenchmarkItem item = Obj.GetComponent<PersonnelTimeBenchmarkItem>();
            Items.Add(item);
            item.ShowPersonnelBenchmarkInfo(PosList[i], StrText);
            if (i % 2 == 0)
            {
                item.GetComponent<Image>().sprite = DoubleImage;
            }
            else
            {
                item.GetComponent<Image>().sprite = OddImage;
            }

        }
        SetScelectItem(PosList);
    }
    public void SetScelectItem(List<PositionList> PosList)
    {
        if (string.IsNullOrEmpty(CurrentTime))
        {
            grid.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
        }
        else
        {
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                string Items = grid.transform.GetChild(i).GetChild(0).GetComponent<Text>().text;
                if (Items == CurrentTime)
                {
                    grid.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                }
            }
        }
    }
    public void InputPerTimeBenchmarkPages(string value)
    {
        JudgeCurrentSelectItem();
        InputPerTimeBenchmarkPage(value);
    }
    /// <summary>
    /// 输入跳转的页数
    /// </summary>
    /// <param name="value"></param>
    public void InputPerTimeBenchmarkPage(string value)
    {
        int currentPage;
        if (string.IsNullOrEmpty(pegeNumTex.text))
        {
            currentPage = 1;
        }
        else
        {
            currentPage = int.Parse(pegeNumTex.text);
        }

        int maxPage = (int)Math.Ceiling((double)(AllPositionList.Count) / (double)pageSize);
        if (currentPage > maxPage)
        {
            currentPage = maxPage;
            pegeNumTex.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumTex.text = currentPage.ToString();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPageData(AllPositionList);
    }
    public void AddPerTimeBenchmarkPage()
    {
        JudgeCurrentSelectItem();
        StartPageNum += 1;
        if (StartPageNum <= AllPositionList.Count / pageSize)
        {
            PageNum += 1;
            pegeNumTex.text = PageNum.ToString();
            GetPageData(AllPositionList);
        }
        else
        {
            StartPageNum -= 1;
        }
    }
    public void MinusPerTimeBenchmarkPage()
    {
        JudgeCurrentSelectItem();
        if (StartPageNum > 0)
        {
            StartPageNum--;
            PageNum -= 1;
            if (PageNum == 0)
            {
                pegeNumTex.text = "1";
            }
            else
            {
                pegeNumTex.text = PageNum.ToString();
            }
        }
    }
    /// <summary>
    /// 得到第几页数据
    /// </summary>
    /// <param name="depList"></param>
    /// <param name="perInfo"></param>
    public void GetPageData(List<PositionList> PosList)
    {
        if (ShowList == null)
        {
            ShowList = new List<PositionList>();
        }
        else
        {
            ShowList.Clear();
        }
        if (StartPageNum * pageSize < PosList.Count)
        {
            var QueryData = PosList.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var per in QueryData)
            {
                ShowList.Add(per);
            }
        }
        ShowTimeBenchmarkInfo(ShowList);
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
    public void DeleteLinePrefabsX()
    {
        for (int j = Per_X_Grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(Per_X_Grid.transform.GetChild(j).gameObject);
        }
    }
    /// <summary>
    /// 有几页数据
    /// </summary>
    /// <param name="data"></param>
    public void TotaiLine()
    {
        if (AllPositionList.Count % pageSize == 0)
        {
            pegeTotalText.text = (AllPositionList.Count / pageSize).ToString();
        }
        else
        {
            pegeTotalText.text = Convert.ToString(Math.Ceiling((double)(AllPositionList.Count) / (double)pageSize));
        }
    }
    // Update is called once per frame

    void Update()
    {

    }
}
