using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryRouteDetails : MonoBehaviour {
    public static MobileInspectionHistoryRouteDetails Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    public PatrolPointHistory info;//工作票信息
    public List<PatrolPointItemHistory> HistoryPatrolPointItemList;//巡检项历史列表

    public Text TxtPersonnelNum;//巡检人工号
    public Text TxtPerson;//巡检人
    public Text devText;//巡检设备名称
    public Text Title;
    public GameObject ItemPrefab;//措施单项
    public VerticalLayoutGroup Grid;//措施列表

    [System.NonSerialized]
    List<PatrolPointItemHistory> newHistoryPatrolPointItemLis = new List<PatrolPointItemHistory>(); //一页存放的列表
    public int pageLine = 10; //每页显示条数
    private int startPageNum = 0; //切页数据
    private int pageNum = 1; //页数
    public Text pageTotalNum; //总页数
    public InputField pageNumText; //输入页数
    public Button nextPageBtn; //下一页
    public Button prevPageBtn; //上一页

    public Button closeBtn;//关闭
    public Sprite Singleline;
    public Sprite DoubleLine;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        closeBtn.onClick.AddListener(CloseBtn_OnClick);

        pageNumText.onValueChanged.AddListener(InputPage);
        nextPageBtn.onClick.AddListener(NextPage);
        prevPageBtn.onClick.AddListener(PreviousPage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(PatrolPointHistory infoT)
    {
        if(infoT.Checks == null)
        {
            UGUIMessageBox.Show("历史巡检项为空！");
            return;
        }
        HistoryPatrolPointItemList.Clear();
        info = infoT ;
        HistoryPatrolPointItemList.AddRange (info.Checks );
        UpdateData();
        //CreateMeasuresItems();
        SetWindowActive(true);

        startPageNum = 0;
        pageNum = 1;
        GetListPages();
        pageNumText.text = "1";
        TotalLine();
    }

    //根据列表数量生成页数
    public void TotalLine()
    {
        if (HistoryPatrolPointItemList.Count != 0)
        {
            if (HistoryPatrolPointItemList.Count % pageLine == 0)
            {
                pageTotalNum.text = (HistoryPatrolPointItemList.Count / pageLine).ToString();
            }
            else
            {
                pageTotalNum.text = Convert.ToString(Math.Ceiling((double)HistoryPatrolPointItemList.Count / (double)pageLine));//有小数加1
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
        newHistoryPatrolPointItemLis.Clear();
        if (startPageNum * pageLine < HistoryPatrolPointItemList.Count)
        {
            var QueryData = HistoryPatrolPointItemList.Skip(startPageNum * pageLine).Take(pageLine);
            foreach (var list in QueryData)
            {
                newHistoryPatrolPointItemLis.Add(list);
            }
            CreateMeasuresItems(newHistoryPatrolPointItemLis);
        }
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void UpdateData()
    {
        TxtPersonnelNum.text = info.StaffCode.ToString();
        TxtPerson.text = info.StaffName;
        devText.text = info.DevName.ToString();
        Title.text = MobileInspectionHistoryDetailInfo.Instance .TitleText .text .ToString() + info.ParentId.ToString() ;

    }
    int i = 0;
    /// <summary>
    /// 创建措施列表
    /// </summary>
    public void CreateMeasuresItems(List<PatrolPointItemHistory> newHistoryPatrolPointItemLis)
    {

        ClearMeasuresItems();
        if (newHistoryPatrolPointItemLis == null || newHistoryPatrolPointItemLis.Count == 0) return;
        foreach (PatrolPointItemHistory sm in newHistoryPatrolPointItemLis)
        {
            i = i + 1;
            GameObject itemT = CreateMeasuresItem();
            Text[] ts = itemT.GetComponentsInChildren<Text>();
            if (ts.Length > 0)
            {
                ts[0].text = sm.CheckId.ToString();
            }
            if (ts.Length > 1)
            {
                ts[1].text = sm.CheckItem;
            }
            if (ts.Length > 2)
            {

                if (sm.dtCheckTime == null)
                {
                    ts[2].text = "";
                }
                else
                {
                    DateTime timeT = Convert.ToDateTime(sm.dtCheckTime);
                    ts[2].text = timeT.ToString("yyyy年MM月dd日 HH:mm");
                }
            }
            if (i % 2 == 0)
            {
                itemT.transform.gameObject.GetComponent<Image>().sprite = DoubleLine;
            }
            else
            {
                itemT.transform.gameObject.GetComponent<Image>().sprite = Singleline;
            }

        }
    }

    /// <summary>
    /// 创建措施项
    /// </summary>
    public GameObject CreateMeasuresItem()
    {
        GameObject itemT = Instantiate(ItemPrefab);
        itemT.transform.SetParent(Grid.transform);
        itemT.transform.localPosition = Vector3.zero;
        itemT.transform.localScale = Vector3.one;
        LayoutElement layoutElement = itemT.GetComponent<LayoutElement>();
      //  layoutElement.ignoreLayout = false;
        itemT.SetActive(true);
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
        if (startPageNum <= HistoryPatrolPointItemList.Count / pageLine)
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
        int maxPage = (int)Math.Ceiling((double)HistoryPatrolPointItemList.Count / (double)pageLine);
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

    /// <summary>
    /// 是否显示传统
    /// </summary>
    public void SetWindowActive(bool isActive)
    {
        window.SetActive(isActive);
    }

    /// <summary>
    /// 关闭按钮
    /// </summary>
    public void CloseBtn_OnClick()
    {
        SetWindowActive(false);
       
    }
}
