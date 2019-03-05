using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistory_N : MonoBehaviour
{

   

    public static MobileInspectionHistory_N Instance;

    public GameObject window;

    public Text txtTitle;//标题文本

    public CalendarChange calendarStart;//开始时间
    public CalendarChange calendarEnd;//开始时间
   

    public Button closeBtn;//关闭Button

    [HideInInspector]
    public List < InspectionTrackHistory> mobileInspectionHistoryList;//巡检轨迹历史纪录

    public InspectionTrackHistory InspectionTrackHistoryItem;

    public MobileInspectionHistoryGrid mobileInspectionHistoryGrid;//巡检路线历史列表
    public InspectionTrack InspectionTrackItems;

    public  DateTime dtBeginTime;
    public  DateTime dtEndTime;
    bool bFlag = true;
    private string zeroTime;

    public InputField searchInput;//搜索关键字输入框   
    public Button searchBtn;//搜索按钮

    public ChangeTimeStyle StartTime;
    public ChangeTimeStyle EndTime;
    // Use this for initialization
    void Start()
    {
        Instance = this;
       
        DateTime dtBeginTime = DateTime.Today.Date;
        dtEndTime = DateTime.Now;
        //titleDropdown.onValueChanged.AddListener(TitleDropdown_OnValueChanged);
       // searchInput.onEndEdit.AddListener(SearchInput_OnEndEdit);
        searchInput.onValueChanged.AddListener(SearchInput_OnValueChanged);
        searchBtn.onClick.AddListener(SearchBtn_OnClick);
        calendarStart.onDayClick.AddListener(CalendarStart_onDayClick);
        calendarEnd.onDayClick.AddListener(CalendarEnd_onDayClick);
        closeBtn.onClick.AddListener(CloseBtn_OnClick);

     //   SetCalendarOpenAndClose();
    }

    /// <summary>
    /// 日历的打开和关闭相关设置
    /// </summary>
    private void SetCalendarOpenAndClose()
    {
        calendarStart.transform.parent.Find("PickButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (calendarEnd.gameObject.activeInHierarchy)
            {
                calendarEnd.gameObject.SetActive(false);
            }
        });

        calendarEnd.transform.parent.Find("PickButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (calendarStart.gameObject.activeInHierarchy)
            {
                calendarStart.gameObject.SetActive(false);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// SetContentActive
    /// </summary>
    /// <param name="isActive"></param>
    public void SetContentActive(bool isActive)
    {
        window.SetActive(isActive);
    }

    /// <summary>
    /// 标题下拉列表框
    /// </summary>
    public void TitleDropdown_OnValueChanged(int i)
    {
        Show();
    }

   


    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        SetContentActive(true);
        searchInput.text = "";
        Loom.StartSingleThread((System.Threading.ThreadStart)(() =>
        {
            GetMobileInspectionHistoryData();
            Loom.DispatchToMainThread((Frankfort.Threading.ThreadDispatchDelegate)(() =>
            {
                CreateGetMobileInspectionHistoryGrid();
            }));
        }));
    }

    

    /// <summary>
    /// 加载移动巡检票历史数据
    /// </summary>
    public void GetMobileInspectionHistoryData()
    {
        //后期获取数据加上时间
          mobileInspectionHistoryList = CommunicationObject.Instance.Getinspectionhistorylist(dtBeginTime, dtEndTime, bFlag);
    }



    /// <summary>
    /// 创建移动巡检票列表
    /// </summary>
    public void CreateGetMobileInspectionHistoryGrid()
    {
        MobileInspectionHistoryGrid.Instance.StartShowMobilenspectionHistory();
        //operationTicketHistoryGrid.gameObject.SetActive(false);
        mobileInspectionHistoryGrid.gameObject.SetActive(true);
        
    }


    /// <summary>
    /// 搜索
    /// </summary>
    public void Search()
    {

        CreateGetMobileInspectionHistoryGrid();

    }

    /// <summary>
    /// 搜索框编辑结束触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void SearchInput_OnEndEdit(string txt)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("SearchInput_OnEndEdit!");
            //currentPageNum = 0;
            Search();
        }

    }

    /// <summary>
    /// 搜索框改变触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void SearchInput_OnValueChanged(string txt)
    {
        Debug.Log("SearchInput_OnValueChanged!");
    }

    /// <summary>
    /// 搜索按钮触发事件
    /// </summary>
    public void SearchBtn_OnClick()
    {
        Debug.Log("SearchBtn_OnClick!");
        MobileInspectionHistoryGrid.Instance.Search();
    }


    public void CalendarStart_onDayClick(DateTime dateTimeT)
    {

        MobileInspectionHistoryGrid.Instance.ScreeningStartTime(dateTimeT);
    }
    public void CalendarEnd_onDayClick(DateTime dateTimeT)
    {
        MobileInspectionHistoryGrid.Instance.ScreeningSecondTime(dateTimeT);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void CloseBtn_OnClick()
    {
        //SetContentActive(false);
        MobileInspectionSubBar.Instance.SetHistoryToggle(false);
        DateTime startTime = DateTime.Today.Date;
        
        string newStartTime = startTime.ToString ("yyyy年MM月dd日");
        StartTime.StartTimeText.text = newStartTime;

        DateTime endTime = DateTime.Now;
        string newEndTime = endTime.ToString("yyyy年MM月dd日");
        EndTime.StartTimeText.text = newEndTime;
    }


 
}

