using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelBenchmarkManage : MonoBehaviour
{
    public static PersonnelBenchmarkManage Instance;
    public GameObject PersonnelBenchmarkWindow;
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
    string CurrentTime;
    void Start()
    {
        AddPageBut.onClick.AddListener(AddPerBenchmarkPage);
        MinusPageBut.onClick.AddListener(MinusPerBenchmarkPage);
        pegeNumTex.onValueChanged.AddListener(InputPerBenchmarkPage);
    }
    private void Awake()
    {
        Instance = this;
    }
    public void PersonnelBenchmarkList(List<PositionList> PosList)
    {
        if (PosList != null && PosList.Count != 0)
        {
            AllPositionList = new List<PositionList>();
            AllPositionList.AddRange(PosList);
            TotaiLine();
            GetPageData(PosList);
            pegeNumTex.text = "1";
        }
        else
        {
            NullData();
        }
    }
    public void NullData()
    {
        pegeTotalText.text = "1";
        pegeNumTex.text = "1";
        DeleteLinePrefabs();
        PersonnelBenchmarkOneDay.Instance.NullDate();
        PersonnelBenchmarkMonths.Instance.NullDate();
    }

    public void ShowPersonnelBenchmarkInfo(List<PositionList> PosList)
    {
        DeleteLinePrefabs();
        for (int i = 0; i < PosList.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            PersonnelBenchmarkItem item = Obj.GetComponent<PersonnelBenchmarkItem>();
            item.ShowPersonnelBenchmarkInfo(PosList[i]);
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
    public void InputPerBenchmarkPages(string value)
    {
        JudgeCurrentSelectItem();
        InputPerBenchmarkPage(value);
    }
    /// <summary>
    /// 输入跳转的页数
    /// </summary>
    /// <param name="value"></param>
    public void InputPerBenchmarkPage(string value)
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
        if (AllPositionList == null) return;
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
    public void AddPerBenchmarkPage()
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
    public void MinusPerBenchmarkPage()
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
        ShowPersonnelBenchmarkInfo(ShowList);
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
    public void ShowPersonnelBenchmarkWindow(bool b)
    {
        PersonnelBenchmarkWindow.SetActive(b);
    }
}
