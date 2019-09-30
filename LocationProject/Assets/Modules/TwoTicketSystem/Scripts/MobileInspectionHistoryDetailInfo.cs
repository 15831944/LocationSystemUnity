using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryDetailInfo : MonoBehaviour {
    public static MobileInspectionHistoryDetailInfo Instance;
    public GameObject MobileInspectionHistoyItemWindow;
    public GameObject ItemPrefab;//措施单项
    public VerticalLayoutGroup Grid;//措施列表
    public Button closeBtn;//关闭

    public List<PatrolPointHistory> PatrolPointHistoryList;//巡检点历史列表
    public MobileInspectionHistoryDetailInfoItem MobileInspectionHistoyItemPrafeb;
    public Text TitleText;

    [System.NonSerialized]
    List<PatrolPointHistory> newPatrolPointHistoryList = new List<PatrolPointHistory>(); //一页存放的列表
    public int pageLine = 10; //每页显示条数
    private int startPageNum = 0; //切页数据
    private int pageNum = 1; //页数
    public Text pageTotalNum; //总页数
    public InputField pageNumText; //输入页数
    public Button nextPageBtn; //下一页
    public Button prevPageBtn; //上一页

    public Sprite Singleline;
    public Sprite DoubleLine;

    void Start()
    {
        Instance = this;
        closeBtn.onClick.AddListener(CloseMobileInspectionHistoyItemWindow);

        pageNumText.onValueChanged.AddListener(InputPage);
        nextPageBtn.onClick.AddListener(NextPage);
        prevPageBtn.onClick.AddListener(PreviousPage);
    }

    public void DateUpdate(InspectionTrackHistory list)
    {
        PatrolPointHistoryList.Clear();
        PatrolPointHistoryList.AddRange (list.Route);
        //CreatInspectionHistoyDetailInfo();
        TitleText.text = list.Code + "-" + list.Name;

        startPageNum = 0;
        pageNum = 1;
        GetListPages();
        pageNumText.text = "1";
        TotalLine();
    }

    //根据列表数量生成页数
    public void TotalLine()
    {
        if (PatrolPointHistoryList.Count != 0)
        {
            if (PatrolPointHistoryList.Count % pageLine == 0)
            {
                pageTotalNum.text = (PatrolPointHistoryList.Count / pageLine).ToString();
            }
            else
            {
                pageTotalNum.text = Convert.ToString(Math.Ceiling((double)PatrolPointHistoryList.Count / (double)pageLine));//有小数加1
            }
        }
        else
        {
            pageTotalNum.text = "1";
        }
    }

    //生成的页数
    public void GetListPages()
    {
        newPatrolPointHistoryList.Clear();
        if (startPageNum * pageLine < PatrolPointHistoryList.Count)
        {
            var QueryData = PatrolPointHistoryList.Skip(startPageNum * pageLine).Take(pageLine);
            foreach (var list in QueryData)
            {
                newPatrolPointHistoryList.Add(list);
            }
            CreatInspectionHistoyDetailInfo(newPatrolPointHistoryList);
        }
    }

    int i = 0;
    public void CreatInspectionHistoyDetailInfo(List<PatrolPointHistory> newPatrolPointHistoryList)
    {
        i = 0;
        foreach (PatrolPointHistory w in newPatrolPointHistoryList)
        {
            i = i + 1;
            int currentIndex = startPageNum * pageLine + i;
            MobileInspectionHistoryDetailInfoItem item = CreateMeasuresItem();
            item.ShowInspectionTrackHistory(w,currentIndex);
            if (i % 2 == 0)
            {
                item.transform.gameObject.GetComponent<Image>().sprite = DoubleLine;
            }
            else
            {
                item.transform.gameObject.GetComponent<Image>().sprite = Singleline;
            }

        }
        ShowMobileInspectionHistoyItemWindow();
    }

    /// <summary>
    /// 创建措施项
    /// </summary>
    public MobileInspectionHistoryDetailInfoItem CreateMeasuresItem()
    {
        MobileInspectionHistoryDetailInfoItem itemT = Instantiate(MobileInspectionHistoyItemPrafeb);
        itemT.transform.SetParent(Grid.transform);
        itemT.transform.localPosition = Vector3.zero;
        itemT.transform.localScale = Vector3.one;
        itemT.gameObject.SetActive(true);
        return itemT;
    }

    /// <summary>
    /// 清空措施列表
    /// </summary>
    public void ClearMeasuresItems()
    {
        int childCount = Grid.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(Grid.transform.GetChild(i).gameObject);
        }
    }

    //下一页
    public void NextPage()
    {
        startPageNum += 1;
        if (startPageNum <= PatrolPointHistoryList.Count / pageLine)
        {
            pageNum += 1;
            pageNumText.text = pageNum.ToString();
            ClearMeasuresItems();
            GetListPages();
        }
    }

    //上一页
    public void PreviousPage()
    {
        if (startPageNum > 0)
        {
            startPageNum--;
            pageNum -= 1;
            if (pageNum == 1)
            {
                pageNumText.text = "1";
            }
            else
            {
                pageNumText.text = pageNum.ToString();
            }
            ClearMeasuresItems();
            GetListPages();
        }
    }

    //选中页输入框
    public void InputPage(string value)
    {
        int currentPage = 0;
        currentPage = int.Parse(pageNumText.text);
        int maxPage = (int)Math.Ceiling((double)PatrolPointHistoryList.Count / (double)pageLine);
        if (maxPage == 0)
        {
            pageNumText.text = "1";
        }
        else
        {
            if (string.IsNullOrEmpty(pageNumText.text))
            {
                currentPage = 1;
            }
            else if (currentPage >= maxPage)
            {
                currentPage = maxPage;
                pageNumText.text = currentPage.ToString();
            }
            else if (currentPage <= 0)
            {
                currentPage = 1;
                pageNumText.text = currentPage.ToString();
            }
            startPageNum = currentPage - 1;
            pageNum = currentPage;
            ClearMeasuresItems();
            GetListPages();
        }
    }

    public void ShowMobileInspectionHistoyItemWindow()
    {      
        MobileInspectionHistoyItemWindow.SetActive(true);        
    }

    public void CloseMobileInspectionHistoyItemWindow()
    {
        MobileInspectionHistoyItemWindow.SetActive(false);
        ClearMeasuresItems();
    }
}
