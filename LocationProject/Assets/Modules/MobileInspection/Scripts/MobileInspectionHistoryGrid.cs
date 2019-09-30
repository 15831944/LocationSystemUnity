using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryGrid : MonoBehaviour
{
    public static MobileInspectionHistoryGrid Instance;
    public VerticalLayoutGroup grid;//列表
    public MobileInspectionHistoryItem itemPrefab;

    public InputField InputFieldPage;//输入框，表示当前第几页，或表示要跳到第几页
    public Button previousPageBtn;//上一页
    public Button nextPageBtn;//下一页
    public Text txtPageCount;//总页数Text
                             //private List<HistoryPersonsSearchUIItem> selectPersonnelList;//当前显示的列表集合
    private int currentPageNum;//当前所在页
    private int pageCount = 1;//总页数
    private int showCount = 10;//每页显示人员的个数

    public List<InspectionTrackHistory> searchList;//当前搜索出来的巡检历史路线列表
    public List<InspectionTrackHistory> InspectionTrackHistoryList;//巡检历史路线列表
    bool IsSearch;
    bool IsStartTime;
    bool IsEndTime;
    public Text StartTimeText;//开始时间
    public Text EndTimeText;//结束时间
    public Sprite Singleline;
    public Sprite DoubleLine;
    // private List<PersonnelMobileInspectionHistory> searchList;//当前搜索出来的工作票历史记录

    // Use this for initialization
    void Start()
    {
        Instance = this;
        InspectionTrackHistoryList = new List<InspectionTrackHistory>();
        previousPageBtn.onClick.AddListener(PreviousPageBtn_OnClick);
        nextPageBtn.onClick.AddListener(NextPageBtn_OnClick);
        InputFieldPage.onValueChanged.AddListener(InputFieldPage_OnEndEdit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //显示巡检历史路线列表
    public void StartShowMobilenspectionHistory()
    {
        MobileInspectionHistoryDetailInfo.Instance.CloseMobileInspectionHistoyItemWindow();//关闭历史巡检点窗口
        MobileInspectionHistoryRouteDetails.Instance.CloseBtn_OnClick();//关闭历史巡检项窗口
        InspectionTrackHistoryList = MobileInspectionHistory_N.Instance.mobileInspectionHistoryList;//巡检轨迹历史记录
        currentPageNum = 0;//当前所在页
        IsSearch = false;
        IsStartTime = false;
        IsEndTime = false;
        //InputFieldPage.text = pageCount.ToString();
        InputFieldPage.text = "1";
        if(searchList!=null) searchList.Clear();
        searchList.AddRange(InspectionTrackHistoryList) ;

        TotaiLine(InspectionTrackHistoryList);
        CreateGrid(InspectionTrackHistoryList);
    }

    //根据列表数量生成页数
    public void TotaiLine(List<InspectionTrackHistory> date)
    {
        if (date.Count != 0)
        {
            if (date.Count % showCount == 0)
            {
                txtPageCount.text = (date.Count / showCount).ToString();
            }
            else
            {
                txtPageCount.text = Convert.ToString(Math.Ceiling((double)date.Count / (double)showCount));
            }
        }
        else
        {
            txtPageCount.text = "1";
        }
    }

    /// <summary>
    /// 搜索
    /// </summary>
    public void Search()
    {
        InputFieldPage.text = "1";
        IsSearch = true;
        if (searchList.Count != 0)
        {
            searchList.Clear();
        }
        string key = MobileInspectionHistory_N.Instance.searchInput.text.ToString().ToLower();
        string StartTime = StartTimeText.text;
        string EndTime = EndTimeText.text;
        for (int i = 0; i < InspectionTrackHistoryList.Count; i++)
        {
            DateTime starttime = InspectionTrackHistoryList[i].dtStartTime;
            DateTime endtime = InspectionTrackHistoryList[i].dtEndTime;
            DateTime NewStartTime = Convert.ToDateTime(StartTime);
            DateTime CurrentEndTime = Convert.ToDateTime(EndTime);
            DateTime NewEndTime = CurrentEndTime.AddHours(24);

            bool isTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenStartTime = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
            bool ScreenEndTime = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;

            string Num = InspectionTrackHistoryList[i].Id.ToString().ToLower();
            string InspectionTrackNum = InspectionTrackHistoryList[i].Code.ToLower();
            string RouteName = InspectionTrackHistoryList[i].Name.ToLower();
            if (isTime)
            {
                if (MobileInspectionHistory_N.Instance.searchInput.text == "" && ScreenStartTime && ScreenEndTime)
                {
                    searchList.Add(InspectionTrackHistoryList[i]);
                }
                else if (Num != null && InspectionTrackNum != null && RouteName != null)
                {
                    if (Num.ToLower().Contains(key) || InspectionTrackNum.ToLower().Contains(key) || RouteName.ToLower().Contains(key))
                    {
                        if (ScreenStartTime && ScreenEndTime)
                        {
                            searchList.Add(InspectionTrackHistoryList[i]);
                        }
                    }
                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
                bool Time3 = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;
                if (MobileInspectionHistory_N.Instance.searchInput.text == "" && ScreenStartTime && ScreenEndTime)
                {
                    searchList.Add(InspectionTrackHistoryList[i]);
                }
                else if (Num != null && InspectionTrackNum != null && RouteName != null)
                {
                    if (Num.ToLower().Contains(key) || InspectionTrackNum.ToLower().Contains(key) || RouteName.ToLower().Contains(key))
                    {
                        if (ScreenStartTime && ScreenEndTime)
                        {
                            searchList.Add(InspectionTrackHistoryList[i]);
                        }
                    }
                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        TotaiLine(searchList);
        CreateGrid(searchList);
        if (searchList != null && searchList.Count == 0)
        {
            UGUIMessageBox.Show("当前时间段内，无历史巡检路线！");
        }
    }

    public void ScreeningStartTime(DateTime datetime)
    {
        IsStartTime = true;
        if (searchList.Count != 0)
        {
            searchList.Clear();
        }
        string key = MobileInspectionHistory_N.Instance.searchInput.text.ToString().ToLower();
        string EndTime = EndTimeText.text;
        for (int i = 0; i < InspectionTrackHistoryList.Count; i++)
        {
            DateTime starttime = InspectionTrackHistoryList[i].dtStartTime;
            DateTime endtime = InspectionTrackHistoryList[i].dtEndTime;
            DateTime NewStartTime = Convert.ToDateTime(datetime);
            DateTime CurrentEndTime = Convert.ToDateTime(EndTime);
            DateTime NewEndTime = CurrentEndTime.AddHours(24);

            bool isTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenStartTime = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
            bool ScreenEndTime = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;

            string Num = InspectionTrackHistoryList[i].Id.ToString().ToLower();
            string InspectionTrackNum = InspectionTrackHistoryList[i].Code.ToLower();
            string RouteName = InspectionTrackHistoryList[i].Name.ToLower();
            if (isTime)
            {
                if (MobileInspectionHistory_N.Instance.searchInput.text == "" && ScreenStartTime && ScreenEndTime)
                {
                    searchList.Add(InspectionTrackHistoryList[i]);
                }
                else if (Num != null && InspectionTrackNum != null && RouteName != null)
                {
                    if (Num.ToLower().Contains(key) || InspectionTrackNum.ToLower().Contains(key) || RouteName.ToLower().Contains(key))
                    {
                        if (ScreenStartTime && ScreenEndTime)
                        {
                            searchList.Add(InspectionTrackHistoryList[i]);
                        }
                    }
                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
                bool Time3 = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;
                if (MobileInspectionHistory_N.Instance.searchInput.text == "" && ScreenStartTime && ScreenEndTime)
                {
                    searchList.Add(InspectionTrackHistoryList[i]);
                }
                else if (Num != null && InspectionTrackNum != null && RouteName != null)
                {
                    if (Num.ToLower().Contains(key) || InspectionTrackNum.ToLower().Contains(key) || RouteName.ToLower().Contains(key))
                    {
                        if (ScreenStartTime && ScreenEndTime)
                        {
                            searchList.Add(InspectionTrackHistoryList[i]);
                        }
                    }
                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        TotaiLine(searchList);
        CreateGrid(searchList);
    }

    public void ScreeningSecondTime(DateTime datetime)
    {
        IsEndTime = true;
        if (searchList.Count != 0)
        {
            searchList.Clear();
        }
        string key = MobileInspectionHistory_N.Instance.searchInput.text.ToString().ToLower();
        string StartTime = StartTimeText.text;
        for (int i = 0; i < InspectionTrackHistoryList.Count; i++)
        {
            DateTime starttime = InspectionTrackHistoryList[i].dtStartTime;
            DateTime endtime = InspectionTrackHistoryList[i].dtEndTime;
            DateTime NewStartTime = Convert.ToDateTime(StartTime);
            DateTime CurrentEndTime = Convert.ToDateTime(datetime);
            DateTime NewEndTime = CurrentEndTime.AddHours(24);

            bool isTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
            bool ScreenStartTime = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
            bool ScreenEndTime = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;

            string Num = InspectionTrackHistoryList[i].Id.ToString().ToLower();
            string InspectionTrackNum = InspectionTrackHistoryList[i].Code.ToLower();
            string RouteName = InspectionTrackHistoryList[i].Name.ToLower();
            if (isTime)
            {
                if (MobileInspectionHistory_N.Instance.searchInput.text == "" && ScreenStartTime && ScreenEndTime)
                {
                    searchList.Add(InspectionTrackHistoryList[i]);
                }
                else if (Num != null && InspectionTrackNum != null && RouteName != null)
                {
                    if (Num.ToLower().Contains(key) || InspectionTrackNum.ToLower().Contains(key) || RouteName.ToLower().Contains(key))
                    {
                        if (ScreenStartTime && ScreenEndTime)
                        {
                            searchList.Add(InspectionTrackHistoryList[i]);
                        }
                    }
                }
            }
            else
            {
                DateTime time1 = NewStartTime.AddHours(24);
                NewEndTime = time1;
                bool Time2 = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
                bool Time3 = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;
                if (MobileInspectionHistory_N.Instance.searchInput.text == "" && ScreenStartTime && ScreenEndTime)
                {
                    searchList.Add(InspectionTrackHistoryList[i]);
                }
                else if (Num != null && InspectionTrackNum != null && RouteName != null)
                {
                    if (Num.ToLower().Contains(key) || InspectionTrackNum.ToLower().Contains(key) || RouteName.ToLower().Contains(key))
                    {
                        if (ScreenStartTime && ScreenEndTime)
                        {
                            searchList.Add(InspectionTrackHistoryList[i]);
                        }
                    }
                }
                Invoke("ChangeEndTime", 0.1f);
            }
        }
        TotaiLine(searchList);
        CreateGrid(searchList);
    }

    int i = 0;
    /// <summary>
    /// 创建人员列表
    /// </summary>
    public void CreateGrid(List<InspectionTrackHistory> date)
    {
        ClearItems();
        int startIndex = currentPageNum * showCount;
        int num = showCount;
        if (startIndex + num > searchList.Count)
        {
            num = searchList.Count - startIndex;
        }
        if (date.Count == 0) return;
        List<InspectionTrackHistory> historyList = searchList.GetRange(startIndex, num);
        i = 0;
        foreach (InspectionTrackHistory w in historyList)
        {
            i = i + 1;
            int currentIndex=startIndex+i;
            MobileInspectionHistoryItem item = CreatePersonItem(w, currentIndex);
            item.gameObject.SetActive(true);
            if (i % 2 == 0)
            {
                item.transform.gameObject.GetComponent<Image>().sprite = DoubleLine;
            }
            else
            {
                item.transform.gameObject.GetComponent<Image>().sprite = Singleline;
            }
        }
        //   SetPreviousAndNextPageBtn();
    }

    public string titleHistory;
    /// <summary>
    /// 创建人员列表项
    /// </summary>
    public MobileInspectionHistoryItem CreatePersonItem(InspectionTrackHistory w,int currentIndex)
    {
        titleHistory = w.Code + "-" + w.Name;
        MobileInspectionHistoryItem item = Instantiate(itemPrefab);
        item.Init(w, currentIndex);
        item.transform.SetParent(grid.transform);
        item.transform.localScale = Vector3.one;
        item.transform.localPosition = Vector3.zero;
        //item.gameObject.SetActive(true);
        return item;
    }

    /// <summary>
    /// 巡检历史路线筛选条件
    /// </summary>
    public bool WorkTicketContains(InspectionTrackHistory inspectionTrackT)
    {
        if (InspectionTrackHistoryContainsNO(inspectionTrackT)) return true;
        if (InspectionTrackHistoryContainsName(inspectionTrackT)) return true;
        return false;
    }

    /// <summary>
    /// 筛选根据巡检编号
    /// </summary>
    public bool InspectionTrackHistoryContainsNO(InspectionTrackHistory inspectionTrackT)
    {
        if (inspectionTrackT.Id.ToString().ToLower().Contains(MobileInspectionHistory_N.Instance.searchInput.text.ToLower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 筛选根据路线名称
    /// </summary>
    public bool InspectionTrackHistoryContainsName(InspectionTrackHistory inspectionTrackT)
    {
        string nameT = "";
        if (inspectionTrackT.Code != null)
        {
            nameT = inspectionTrackT.Code;
        }
        if (nameT.ToLower().Contains(MobileInspectionHistory_N.Instance.searchInput.text.ToLower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 清除列表
    /// </summary>
    public void ClearItems()
    {
        int n = grid.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
        {
            GameObject o = grid.transform.GetChild(i).gameObject;            
            DestroyImmediate(o);
        }
    }

    /// <summary>
    /// 前一页的按钮触发事件
    /// </summary>
    public void PreviousPageBtn_OnClick()
    {
        if (currentPageNum > 0 && pageCount > 0)
        {
            currentPageNum = currentPageNum - 1;//当前页
            pageCount = pageCount - 1;//总页数
            InputFieldPage.text = pageCount.ToString();
            if (IsSearch || IsStartTime || IsEndTime)
            {
                CreateGrid(searchList);
            }
        }
        else
        {
            CreateGrid(InspectionTrackHistoryList);
        }
    }

    /// <summary>
    /// 下一页的按钮触发事件
    /// </summary>
    public void NextPageBtn_OnClick()
    {
        if (IsSearch || IsStartTime || IsEndTime)
        {
            double a = Math.Ceiling((double)searchList.Count / (double)showCount);
            int m = (int)a;
            if (currentPageNum < m && pageCount < m)
            {
                currentPageNum = currentPageNum + 1;
                pageCount = pageCount + 1;
                InputFieldPage.text = pageCount.ToString();
            }
            else if (currentPageNum == m)
            {
                pageCount = m;
                InputFieldPage.text = pageCount.ToString();
            }
            CreateGrid(searchList);
        }
        else
        {
            double a = Math.Ceiling((double)InspectionTrackHistoryList.Count / (double)showCount);
            int m = (int)a;
            if (currentPageNum < m && pageCount < m)
            {
                currentPageNum = currentPageNum + 1;
                pageCount = pageCount + 1;
                InputFieldPage.text = pageCount.ToString();
                CreateGrid(InspectionTrackHistoryList);
            }
            else if (currentPageNum == m)
            {
                pageCount = m;
                InputFieldPage.text = pageCount.ToString();
                CreateGrid(searchList);
            }
        }
    }

    public void ChangeEndTime()
    {
        string startTime = StartTimeText.text;
        DateTime NewStartTime = Convert.ToDateTime(startTime);
        string CurrentTime = NewStartTime.ToString("yyyy年MM月dd日");
        EndTimeText.text = CurrentTime.ToString();
    }

    /// <summary>
    /// 输入页数文本框编辑结束触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void InputFieldPage_OnEndEdit(string txt)
    {
        int currentPage = int.Parse(InputFieldPage.text);
        if (IsSearch || IsStartTime || IsEndTime)
        {
            int MaxPage = (int)Math.Ceiling((double)searchList.Count / (double)showCount);
            if (currentPage > MaxPage)
            {
                currentPage = MaxPage;
                InputFieldPage.text = currentPage.ToString();
            }
            if (currentPage <= 0)
            {
                currentPage = 1;
                InputFieldPage.text = currentPage.ToString();
            }
            currentPageNum = int.Parse(InputFieldPage.text) - 1;
            pageCount = int.Parse(InputFieldPage.text);
            CreateGrid(searchList);
        }
        else
        {
            int MaxPage = (int)Math.Ceiling((double)InspectionTrackHistoryList.Count / (double)showCount);
            if (currentPage > MaxPage)
            {
                currentPage = MaxPage;
                InputFieldPage.text = currentPage.ToString();
            }
            if (currentPage <= 0)
            {
                currentPage = 1;
                InputFieldPage.text = currentPage.ToString();
            }
            pageCount = int.Parse(InputFieldPage.text);
            currentPageNum = int.Parse(InputFieldPage.text) - 1;
            CreateGrid(InspectionTrackHistoryList);
        }
    }
}
