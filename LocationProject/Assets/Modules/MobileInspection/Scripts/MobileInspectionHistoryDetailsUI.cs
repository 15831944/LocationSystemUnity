using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryDetailsUI : MonoBehaviour {

    public static MobileInspectionHistoryDetailsUI Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    public VerticalLayoutGroup Grid;//巡检项详情列表

    public Text Title;//巡检项窗口标题
    public Text TxtPersonnelNum;//巡检人工号
    public Text TxtPerson;//巡检人
    public Text devText;//巡检设备名称

    public PatrolPoint info;//巡检点信息
    public List<PatrolPointItem> PatrolPointItemList;//巡检项信息列表
    public PatrolPointItem PatrolPointItems;
    public GameObject ItemPrefab;//巡检项单项(行的数据)

    [System.NonSerialized]
    List<PatrolPointItem> newPatrolPointItemList = new List<PatrolPointItem>(); //一页存放的列表
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

    public void Show(PatrolPoint infoT)
    {
        if(infoT.Checks==null)
        {
            UGUIMessageBox.Show("巡检项详情为空！");
            return;
        }
        PatrolPointItemList.Clear();
        info = infoT;
        PatrolPointItemList.AddRange (info.Checks);
        UpdateData();
        //CreateMeasuresItems();
        //if (MobileInspectionInfoManage.Instance.window.activeInHierarchy)
        //{
        //    MobileInspectionInfoManage.Instance.CloseWindow();
        //}
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
        if (PatrolPointItemList.Count != 0)
        {
            if (PatrolPointItemList.Count % pageLine == 0)
            {
                pageTotalNum.text = (PatrolPointItemList.Count / pageLine).ToString();
            }
            else
            {
                pageTotalNum.text = Convert.ToString(Math.Ceiling((double)PatrolPointItemList.Count / (double)pageLine));//有小数加1
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
        newPatrolPointItemList.Clear();
        if (startPageNum * pageLine < PatrolPointItemList.Count)
        {
            var QueryData = PatrolPointItemList.Skip(startPageNum * pageLine).Take(pageLine);
            foreach (var list in QueryData)
            {
                newPatrolPointItemList.Add(list);
            }
            CreateMeasuresItems(newPatrolPointItemList,startPageNum);
        }
    }

    /// <summary>
    /// 刷新数据（巡检员、巡检工号、巡检设备）
    /// </summary>
    public void UpdateData()
    {
        TxtPersonnelNum.text = string.IsNullOrEmpty(info.StaffCode) ? "--" : info.StaffCode;
        string devName = string.IsNullOrEmpty(info.DevName) ? info.DeviceCode : info.DevName;
        devText.text = string.IsNullOrEmpty(devName) ? "--" : devName;
        TxtPerson.text = string.IsNullOrEmpty(info.StaffName) ? "--" : info.StaffName;       
        Title.text = MobileInspectionDetailsUI.Instance.TitleText;
    }

    int recordIndex = 0;
    /// <summary>
    /// 创建巡检项列表
    /// </summary>
    public void CreateMeasuresItems(List<PatrolPointItem> newPatrolPointItemList,int pageNum)
    {
        ClearMeasuresItems();
        if (newPatrolPointItemList == null || newPatrolPointItemList.Count  == 0) return;
        recordIndex = 0;
        foreach (PatrolPointItem sm in newPatrolPointItemList)
        {
            recordIndex = recordIndex + 1;
            int dataIndex = pageNum * pageLine + recordIndex;
            GameObject itemT = CreateMeasuresItem();
            Text[] ts = itemT.GetComponentsInChildren<Text>();          
            if (ts.Length > 0)
            {
                ts[0].text = dataIndex.ToString();
            }
            if (ts.Length > 1)
            {
                ts[1].text = sm.CheckItem;
            }
            if (ts.Length > 2)
            {
                if (sm.dtCheckTime == null)
                {
                    ts[2].text = "--";
                }
                else
                {
                    DateTime timeT = Convert.ToDateTime(sm.dtCheckTime);
                    ts[2].text = timeT.ToString("yyyy/MM/dd HH:mm");
                }
            }
            if(ts.Length>3)
            {
                ts[3].text = string.IsNullOrEmpty(sm.CheckResult) ? "--" : sm.CheckResult;
            }
            if(ts.Length>4)
            {
                ts[4].text = string.IsNullOrEmpty(sm.CheckId) ? "--" : sm.CheckId;
            }
            if (recordIndex % 2 == 0)
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
        itemT.SetActive(true);
        return itemT;
    }

    /// <summary>
    /// 清空巡检项列表
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
        if (startPageNum <= PatrolPointItemList.Count / pageLine)
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
        int maxPage = (int)Math.Ceiling((double)PatrolPointItemList.Count / (double)pageLine);
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
      //  MobileInspectionHistory_N.Instance.SetContentActive(true);
    }
}
