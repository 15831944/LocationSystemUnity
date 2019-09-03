using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class TimeBenchmarkManage : MonoBehaviour
{
    public static TimeBenchmarkManage Instance;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    public GameObject TimeBenchmarkWindow;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    const int pageSize = 15;
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
    public UGUI_LineChart TimeLineChart;
    public UGUI_LineChartDateFill TimeData;
    //List<PositionList> TimeScreenPositionList;
    public UGUI_LineChartYValue LineChartYvalue;
    public GameObject TimeGrid;
    public GameObject TimeObj;
    public GameObject scrollView;
    public GameObject Panel;
    public GameObject coordinate;
    public Text promptText;
    public GameObject PointParent;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        AddPageBut.onClick.AddListener(AddTimeBenchmarkPage);
        MinusPageBut.onClick.AddListener(MinusTimeBenchmarkPage);
        pegeNumTex.onValueChanged.AddListener(InputTimeBenchmarkPages);
    }

    private List<PositionList> ParseLineChartInfo(List<PositionList> PosList)
    {
        List<PositionList> newList = new List<PositionList>();
        if (PosList == null) return newList;
        if (PosList.Count == 0) return newList;

        PosList.Sort((x, y) =>
        {
            return x.Name.CompareTo(y.Name);
        });//根据名称排序，即根据时间排序

        //scrollView.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(PosList.Count * 150, 280);

        DateTime start = DateTime.Now;
        for (int i = 0; i < PosList.Count; i++)
        {
            if (PosList[i].Name.Contains("1970") || PosList[i].Name.Contains("1974"))
            {
                Debug.Log("数据有问题，去掉1970年的和1974年的");
            }
            else
            {
                newList.Add(PosList[i]);

            }
        }


        Debug.LogError("------------去掉1970年的和1974年:" + (DateTime.Now - start).TotalMilliseconds + "ms");
        return newList;
    }
    public void NullDate()
    {
        if (TimeLineChart.pointImageList.Count != 0)
        {
            TimeLineChart.pointImageList.Clear();
            ClearLinePoint();
        }
        promptText.text = "";
        pegeTotalText.text = "1";
       
        DeleteLinePrefabs();
        LineChartYvalue.DateY(0);
        TimeLineChart.yMax = 0;
        List<float> data = new List<float>();
        TimeLineChart.UpdateData(data);
        PersonnelTimeBenchmark.Instance.NullData();
        return;
    }
    public void ShowLineChartInfo(List<PositionList> posList)
    {
        if (posList == null || posList.Count == 0)
        {
            NullDate();
        }
        else
        {
            TimeLineChart.pointImageList.Clear();
            DateTime start = DateTime.Now;
            DeleteLinePrefabsX();
            var posListNew = ParseLineChartInfo(posList);//按时间排序，删除错误数据
            List<float> data = GetDayLineChartData(posListNew);//得到按天折线图的数据
            SetHourLineChartDate(posListNew, TimeData, data.Count);//设置小时折线图
            promptText.text = Convert.ToDateTime(posListNew[0].Name).ToString("yyyy年MM月dd日") + "起定位数据量折线图"; ;
            posListNew.Sort((x, y) =>
            {
                return x.Count.CompareTo(y.Count);
            });//根据数量排列
            float Width = data.Count * 86f;
            scrollView.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, 340);
            Panel.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, 340);
            coordinate.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, 273);
            LineChartYvalue.DateY(posListNew[posListNew.Count - 1].Count);
            TimeLineChart.yMax = (float)posListNew[posListNew.Count - 1].Count;
            //LineChartYvalue.DateY(posListNew[0].Count);
            //TimeLineChart.yMax = (float)posListNew[0].Count;
            TimeLineChart.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Width, 266f);
            if (TimeLineChart.pointImageList.Count != 0)
            {
                TimeLineChart.pointImageList.Clear();
                ClearLinePoint();
            }

            TimeLineChart.width = (float)Width;
            TimeLineChart.UpdateData(data);

            Debug.LogError("--------------------ShowLineChartInfo:" + (DateTime.Now - start).TotalMilliseconds + "ms");
        }

    }
    public void ClearLinePoint()
    {
        for (int i = 0; i <= PointParent.transform.childCount; i++)
        {
            DestroyImmediate(PointParent.transform.GetChild(i).gameObject);
        }
    }
    private List<float> GetDayLineChartData(List<PositionList> posList)
    {
        List<float> data = new List<float>();
        if (posList == null || posList.Count == 0) return data;
        DateTime dt = Convert.ToDateTime(posList[posList.Count - 1].Name);
        DateTime MinDt = Convert.ToDateTime(posList[0].Name);
        int DifferencetIME = int.Parse((dt - MinDt).TotalDays.ToString());
        DateTime start = DateTime.Now;
        for (int i = 0; i <= DifferencetIME; i++)
        {
            DateTime DayAdd = MinDt.AddDays(i);
            string TimeT = DayAdd.ToString(("yyyy-MM-dd"));
            PositionList item = posList.Find(j => j.Name.Contains(TimeT));
            if (item != null)
            {
                data.Add(float.Parse(item.Count.ToString()));
                //TimeScreenPositionList.Remove(item);
            }
            else
            {
                data.Add(1);//没有数据的日期补上1
            }
            TimeInstantiateLine();
        }
        Debug.LogError("--------------------GetDayLineChartData:" + (DateTime.Now - start).TotalMilliseconds + "ms");
        return data;
    }

    /// <summary>
    /// 设置折线图日期
    /// </summary>
    /// <param name="DataList"></param>
    private void SetHourLineChartDate(List<PositionList> DataList, UGUI_LineChartDateFill LineChart, int Num)
    {
        if (DataList != null && DataList.Count != 0)
        {
            //LastTime = long.Parse(DataList[DataList.Count - 1].RecordTime);
            DateTime dt = Convert.ToDateTime(DataList[DataList.Count - 1].Name);
            LineChart.DateFillT(UGUI_LineChartDateFill.DateType.Month, Num, dt);
        }
    }

    public void GetTimeBenchmarkList(List<PositionList> PosList)
    {
        AllPositionList = new List<PositionList>();
        if (PosList != null) AllPositionList.AddRange(PosList);
        TotaiLine();
        GetPageData(PosList);
        pegeNumTex.text = "1";
    }

    private List<TimeBenchmarkItem> Items = new List<TimeBenchmarkItem>();

    public TimeBenchmarkItem GetSelectedItem()
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
            TimeBenchmarkItem item = Obj.GetComponent<TimeBenchmarkItem>();
            Items.Add(item);
            item.ShowTimeBenchmarkInfo(PosList[i]);

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
    public void InputTimeBenchmarkPages(string value)
    {
        JudgeCurrentSelectItem();
        InputTimeBenchmarkPage(value);
    }
    /// <summary>
    /// 输入跳转的页数
    /// </summary>
    /// <param name="value"></param>
    public void InputTimeBenchmarkPage(string value)
    {
        if (AllPositionList == null || AllPositionList.Count == 0) return;
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
    public void AddTimeBenchmarkPage()
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
    public void MinusTimeBenchmarkPage()
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
    public void GetPageData(List<PositionList> posList)
    {
        if (posList == null) return;
        if (ShowList == null)
        {
            ShowList = new List<PositionList>();
        }
        else
        {
            ShowList.Clear();
        }
        if (StartPageNum * pageSize < posList.Count)
        {
            var QueryData = posList.Skip(pageSize * StartPageNum).Take(pageSize);
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
    public GameObject TimeInstantiateLine()
    {
        GameObject o = Instantiate(TimeObj);
        o.SetActive(true);
        o.transform.parent = TimeGrid.transform;
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
        for (int j = TimeGrid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(TimeGrid.transform.GetChild(j).gameObject);
        }
    }
    string CurrentTime;
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
    /// <summary>
    /// 是否打开历史路径时间基准界面
    /// </summary>
    /// <param name="b"></param>
	public void ShowTimeBenchmarkUI(bool b)
    {
        CurrentTime = "";
        PersonnelTimeBenchmark.Instance.CurrentTime = "";
        DeleteLinePrefabs();
        TimeBenchmarkWindow.SetActive(b);
    }
    // Update is called once per frame

    void Update()
    {

    }
}
