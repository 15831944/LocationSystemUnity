using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    public class TwoTicketHistoryUI_N : MonoBehaviour
    {
        enum State
        {
            工作票,
            操作票,
        }
        State curentState = State.工作票;

        public static TwoTicketHistoryUI_N Instance;

        public GameObject window;
        public Dropdown titleDropdown;//标题选择下拉列表框
        public Text txtTitle;//标题文本

        public CalendarChange calendarStart;//开始时间
        public CalendarChange calendarEnd;//开始时间
        private DateTime startTime;//起始时间
        private DateTime endTime;//结束时间

        public Button closeBtn;//关闭Button

        [HideInInspector]
        public List<WorkTicketHistory> workTicketHistoryList;//工作票历史记录
        [HideInInspector]
        public List<OperationTicketHistory> operationTicketHistoryList;//操作票历史记录

        public WorkTicketHistoryGrid workTicketHistoryGrid;//工作票历史列表
        public OperationTicketHistoryGrid operationTicketHistoryGrid;//操作票历史列表

        public WorkTicketHistoryDetailsUI workTicketHistoryDetailsUI;//工作票历史项详情界面
        public OperationTicketHistoryDetailsUI operationTicketHistoryDetailsUI;//操作票历史项详情界面

        // Use this for initialization
        void Start()
        {
            Instance = this;
            titleDropdown.onValueChanged.AddListener(TitleDropdown_OnValueChanged);
            searchInput.onEndEdit.AddListener(SearchInput_OnEndEdit);
            searchInput.onValueChanged.AddListener(SearchInput_OnValueChanged);
            searchBtn.onClick.AddListener(SearchBtn_OnClick);
            calendarStart.onDayClick.AddListener(ShowStartTimeIno);
            calendarEnd.onDayClick.AddListener(ShowEndTimeIno);
            closeBtn.onClick.AddListener(CloseBtn_OnClick);

            SetCalendarOpenAndClose();
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
            txtTitle.text = titleDropdown.options[i].text;
            ChangeState((State)i);
            Show();
        }

        public InputField searchInput;//搜索关键字输入框   
        public Button searchBtn;//搜索按钮
        private List<WorkTicketHistory> workTicketHistorySearchList;//当前搜索出来的工作票历史记录
        private List<OperationItemHistory> operationItemHistorySearchList;//当前搜索出来的操作票历史记录

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            SetContentActive(true);
            searchInput.text = "";

            Loom.StartSingleThread((System.Threading.ThreadStart)(() =>
            {
                if (curentState == State.工作票)
                {
                    GetWorkTicketHistoryData();
                    Loom.DispatchToMainThread((Frankfort.Threading.ThreadDispatchDelegate)(() =>
                    {
                        ShowWorkTicketHistoryInfo();
                    }));
                }
                else if (curentState == State.操作票 )
                {
                    GetOperationTicketHistoryData();
                    Loom.DispatchToMainThread(() =>
                    {
                        ShowOperationTicketInfo();
                    });
                }

            }));
        }
        public void StartShow()
        {
            SetContentActive(true);
            searchInput.text = "";

            if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.工作票)
            {
                TitleDropdown_OnValueChanged(0);
                GetWorkTicketHistoryData();
                Loom.DispatchToMainThread((Frankfort.Threading.ThreadDispatchDelegate)(() =>
                {
                    ShowWorkTicketHistoryInfo();
                }));
            }
            else if (TwoTicketSystemUI_N.Instance.State == TwoTicketState.操作票)
            {
                TitleDropdown_OnValueChanged(1);
                GetOperationTicketHistoryData();
                Loom.DispatchToMainThread(() =>
                {
                    ShowOperationTicketInfo();
                });
            }


        }
        public void ShowStartTimeIno(DateTime dateTime)
        {
            Loom.StartSingleThread((System.Threading.ThreadStart)(() =>
            {
                if (curentState == State.工作票)
                {
                    GetWorkTicketHistoryData();
                    Loom.DispatchToMainThread((Frankfort.Threading.ThreadDispatchDelegate)(() =>
                    {
                        workTicketHistoryGrid.ScreeningStartTime(dateTime);
                    }));
                }
                else if (curentState == State.操作票)
                {
                    GetOperationTicketHistoryData();
                    Loom.DispatchToMainThread(() =>
                    {
                        operationTicketHistoryGrid.ScreeningStartTime(dateTime);
                    });
                }

            }));
        }
        public void ShowEndTimeIno(DateTime dateTime)
        {
            Loom.StartSingleThread((System.Threading.ThreadStart)(() =>
            {
                if (curentState == State.工作票)
                {
                    GetWorkTicketHistoryData();
                    Loom.DispatchToMainThread((Frankfort.Threading.ThreadDispatchDelegate)(() =>
                    {
                        workTicketHistoryGrid.ScreeningSecondTime(dateTime);
                    }));
                }
                else if (curentState == State.操作票)
                {
                    GetOperationTicketHistoryData();
                    Loom.DispatchToMainThread(() =>
                    {
                        operationTicketHistoryGrid.ScreeningSecondTime(dateTime);
                    });
                }
            }));
        }
        /// <summary>
        /// 改变状态
        /// </summary>
        private void ChangeState(State stateT)
        {
            curentState = stateT;
        }

        /// <summary>
        /// 加载工作票历史数据
        /// </summary>
        public void GetWorkTicketHistoryData()
        {
            //后期获取数据加上时间
            workTicketHistoryList = CommunicationObject.Instance.GetWorkTicketHistoryList();
        }

        /// <summary>
        /// 加载工作票历史数据
        /// </summary>
        public void GetOperationTicketHistoryData()
        {
            //后期获取数据加上时间
            operationTicketHistoryList = CommunicationObject.Instance.GetOperationTicketHistoryList();
        }

        /// <summary>
        /// 创建工作票列表
        /// </summary>
        public void CreateWorkTicketGrid()
        {

            workTicketHistoryGrid.Search();

        }

        /// <summary>
        /// 展示两票历史列表信息
        /// </summary>
        public void ShowWorkTicketHistoryInfo()
        {
            operationTicketHistoryGrid.gameObject.SetActive(false);
            workTicketHistoryGrid.gameObject.SetActive(true);
            workTicketHistoryGrid.StartShowWorkTicketHistory();
            workTicketHistoryGrid.TotaiLine(workTicketHistoryList);
        }
        /// <summary>
        /// 创建操作票列表
        /// </summary>
        public void CreateOperationTicketGrid()
        {
            operationTicketHistoryGrid.Search();
        }
        public void ShowOperationTicketInfo()
        {

            workTicketHistoryGrid.gameObject.SetActive(false);
            operationTicketHistoryGrid.gameObject.SetActive(true);
            //     operationTicketHistoryGrid.gameObject.SetActive(true);
            operationTicketHistoryGrid.StartShowOperationTicketInfo();
            operationTicketHistoryGrid.TotaiLine(operationTicketHistoryList);
        }
        /// <summary>
        /// 搜索
        /// </summary>
        public void Search()
        {
            if (curentState == State.工作票)
            {
                //WorkTicketHistorySearch();
                CreateWorkTicketGrid();
            }
            else if (curentState == State.操作票)
            {
                //OperationItemHistorySearch();
                CreateOperationTicketGrid();
            }
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
            Search();
        }


      

        /// <summary>
        /// 关闭
        /// </summary>
        public void CloseBtn_OnClick()
        {
            //SetContentActive(false);
            TwoTicketSystemSubBar.Instance.SetHistoryToggle(false);
        }

        /// <summary>
        /// 展示工作票历史项详情
        /// </summary>
        public void ShowWorkTicketHistoryDetailsUI(WorkTicketHistory workTicketHistoryT)
        {
            SetContentActive(false);
            workTicketHistoryDetailsUI.Show(workTicketHistoryT);
        }

        /// <summary>
        /// 展示操作票历史项详情
        /// </summary>
        public void ShowOperationTicketHistoryDetailsUI(OperationTicketHistory operationTicketHistoryT)
        {
            SetContentActive(false);
            operationTicketHistoryDetailsUI.Show(operationTicketHistoryT);
        }
    }
}
