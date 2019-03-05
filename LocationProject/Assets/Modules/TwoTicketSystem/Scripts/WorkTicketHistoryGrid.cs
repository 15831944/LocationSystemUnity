using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using TwoTicketSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    public class WorkTicketHistoryGrid : MonoBehaviour
    {
        public VerticalLayoutGroup grid;//列表
        public WorkTicketHistoryItem itemPrefab;

        public InputField InputFieldPage;//输入框，表示当前第几页，或表示要跳到第几页
        public Button previousPageBtn;//上一页
        public Button nextPageBtn;//下一页
        public Text txtPageCount;//总页数Text
                                 //private List<HistoryPersonsSearchUIItem> selectPersonnelList;//当前显示的列表集合
        private int currentPageNum = 0;//当前所在页
        private int pageCount = 1;//页数
        private int showCount = 10;//每页显示人员的个数

        private List<WorkTicketHistory> searchList;//当前搜索出来的工作票历史记录

        public Text StartTimeText;//开始时间
        public Text EndTimeText;//结束时间
        bool IsSearch;
        bool IsStartTime;
        bool IsEndTime;
        public Sprite Singleline;
        public Sprite DoubleLine;
        // Use this for initialization
        void Start()
        {

            searchList = new List<WorkTicketHistory>();
            previousPageBtn.onClick.AddListener(PreviousPageBtn_OnClick);
            nextPageBtn.onClick.AddListener(NextPageBtn_OnClick);
            InputFieldPage.onValueChanged.AddListener(InputFieldPage_OnEndEdit);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void TotaiLine(List<WorkTicketHistory> date)
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
        public void StartShowWorkTicketHistory()
        {
            currentPageNum = 0;
            pageCount = 1;
            IsSearch = false;
            IsStartTime = false;
            IsEndTime = false;
            InputFieldPage.text = pageCount.ToString();
            TotaiLine(TwoTicketHistoryUI_N.Instance.workTicketHistoryList);
            CreateGrid(TwoTicketHistoryUI_N.Instance.workTicketHistoryList);
        }
        /// <summary>
        /// 搜索
        /// </summary>
        public void Search()
        {
            IsSearch = true;

            if (searchList.Count != 0)
            {
                searchList.Clear();
            }
            string key = TwoTicketHistoryUI_N.Instance.searchInput.text.ToString().ToLower();
            string StartTime = StartTimeText.text;
            string EndTime = EndTimeText.text;
            for (int i = 0; i < TwoTicketHistoryUI_N.Instance.workTicketHistoryList.Count; i++)
            {
                DateTime starttime = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].StartTimeOfPlannedWork;
                DateTime endtime = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].EndTimeOfPlannedWork;
                DateTime NewStartTime = Convert.ToDateTime(StartTime);
                DateTime CurrentEndTime = Convert.ToDateTime(EndTime);  
                DateTime NewEndTime = CurrentEndTime.AddHours(24);

                bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
                bool ScreenStartTime = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
                bool ScreenEndTime = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;

                string lssuer = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].Lssuer.ToLower();
                string licensor = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].Licensor.ToLower();
                string noNum = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].No.ToLower();
                string personInCharge = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].PersonInCharge.ToLower();
              

                if (IsTime)
                {
                    if (TwoTicketHistoryUI_N.Instance.searchInput.text == "" && ScreenStartTime && ScreenEndTime)
                    {
                        searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
                    }
                    else if(lssuer!=null&& noNum!=null && licensor!=null && personInCharge!=null )
                    {
                        if (licensor.ToLower().Contains(key) || noNum.ToLower().Contains(key) || personInCharge.ToLower().Contains(key) || lssuer.ToLower().Contains(key))
                        {
                            if (ScreenStartTime && ScreenEndTime)
                            {
                                searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
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
                    if (TwoTicketHistoryUI_N.Instance.searchInput.text == "" && Time2 && Time3)
                    {
                        searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
                    }
                    else if (lssuer != null && noNum != null && licensor != null && personInCharge != null)
                    {
                        if (licensor.ToLower().Contains(key) || noNum.ToLower().Contains(key) || personInCharge.ToLower().Contains(key) || lssuer.ToLower().Contains(key))
                        {
                            if (ScreenStartTime && ScreenEndTime)
                            {
                                searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
                            }
                        }
                    }
                    Invoke("ChangeEndTime", 0.1f);
                }
            }
            TotaiLine(searchList);
            CreateGrid(searchList);

        }
        /// <summary>
        /// 第一个时间筛选
        /// </summary>
        /// <param name="dateTime"></param>
        public void ScreeningStartTime(DateTime dateTime)
        {
            IsStartTime = true;

            if (searchList.Count != 0)
            {
                searchList.Clear();
            }
            string key = TwoTicketHistoryUI_N.Instance.searchInput.text.ToString().ToLower();

            string EndTime = EndTimeText.text;
            for (int i = 0; i < TwoTicketHistoryUI_N.Instance.workTicketHistoryList.Count; i++)
            {
                DateTime starttime = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].StartTimeOfPlannedWork;
                DateTime endtime = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].EndTimeOfPlannedWork;
                DateTime NewStartTime = Convert.ToDateTime(dateTime);
                DateTime CurrentEndTime = Convert.ToDateTime(EndTime);
                DateTime NewEndTime = CurrentEndTime.AddHours(24);

                bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
                bool ScreenStartTime = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
                bool ScreenEndTime = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;


                string lssuer = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].Lssuer.ToLower();
                string licensor = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].Licensor.ToLower();
                string noNum = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].No.ToLower();
                string personInCharge = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].PersonInCharge.ToLower();
           

                if (IsTime)
                {
                    if (key == "" && ScreenStartTime && ScreenEndTime)
                    {
                        searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
                    }
                    else if (lssuer != null && noNum != null && licensor != null && personInCharge != null)
                    {
                        if (licensor.ToLower().Contains(key) || noNum.ToLower().Contains(key) || personInCharge.ToLower().Contains(key) || lssuer.ToLower().Contains(key))
                        {
                            if (ScreenStartTime && ScreenEndTime)
                            {
                                searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
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
                    if (key == "" && Time2 && Time3)
                    {
                        searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
                    }
                    else if (lssuer != null && noNum != null && licensor != null && personInCharge != null)
                    {
                        if (licensor.ToLower().Contains(key) || noNum.ToLower().Contains(key) || personInCharge.ToLower().Contains(key) || lssuer.ToLower().Contains(key))
                        {
                            if (ScreenStartTime && ScreenEndTime)
                            {
                                searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
                            }
                        }
                    }
                    Invoke("ChangeEndTime", 0.1f);
                }
            }


            TotaiLine(searchList);
            CreateGrid(searchList);
        }
        /// <summary>
        /// 第二个时间筛选
        /// </summary>
        /// <param name="dateTime"></param>
        public void ScreeningSecondTime(DateTime dateTime)
        {
            IsEndTime = true;

            if (searchList.Count != 0)
            {
                searchList.Clear();
            }
            string key = TwoTicketHistoryUI_N.Instance.searchInput.text.ToString();
            string StartTime = StartTimeText.text;

            for (int i = 0; i < TwoTicketHistoryUI_N.Instance.workTicketHistoryList.Count; i++)
            {
                DateTime starttime = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].StartTimeOfPlannedWork;
                DateTime endtime = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].EndTimeOfPlannedWork;
                DateTime NewStartTime = Convert.ToDateTime(StartTime);

                DateTime CurrentEndTime = Convert.ToDateTime(dateTime);
                DateTime NewEndTime = CurrentEndTime.AddHours(24);
             

                bool IsTime = DateTime.Compare(NewStartTime, NewEndTime) < 0;
                bool ScreenStartTime = DateTime.Compare(NewStartTime, starttime) <= 0 && DateTime.Compare(NewEndTime, starttime) >= 0;
                bool ScreenEndTime = DateTime.Compare(NewStartTime, endtime) < 0 && DateTime.Compare(NewEndTime, endtime) >= 0;


                string lssuer = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].Lssuer;
                string licensor = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].Licensor;
                string noNum = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].No;
                string personInCharge = TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i].PersonInCharge;
               

                if (IsTime)
                {
                    if (key == "" && ScreenStartTime && ScreenEndTime)
                    {
                        searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
                    }
                    else if (lssuer != null && noNum != null && licensor != null && personInCharge != null)
                    {
                        if (licensor.Contains(key) || noNum.Contains(key) || personInCharge.Contains(key) || lssuer.Contains(key))
                        {
                            if (ScreenStartTime && ScreenEndTime)
                            {
                                searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
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
                    if (key == "" && Time2 && Time3)
                    {
                        searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
                    }
                    else if (lssuer != null && noNum != null && licensor != null && personInCharge != null)
                    {
                        if (licensor.Contains(key) || noNum.Contains(key) || personInCharge.Contains(key) || lssuer.Contains(key))
                        {
                            if (ScreenStartTime && ScreenEndTime)
                            {
                                searchList.Add(TwoTicketHistoryUI_N.Instance.workTicketHistoryList[i]);
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
        public void CreateGrid(List<WorkTicketHistory> date)
        {
            ClearItems();
       
            int startIndex = currentPageNum * showCount;
            int num = showCount;
            if (startIndex + num > date.Count)
            {
                num = date.Count - startIndex;
            }
            if (date.Count == 0) return;
            List<WorkTicketHistory> workTicketHistoryT = date.GetRange(startIndex, num);

            foreach (WorkTicketHistory w in workTicketHistoryT)
            {
                i = i + 1;
                
                WorkTicketHistoryItem item = CreatePersonItem(w);
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

        /// <summary>
        /// 创建人员列表项
        /// </summary>
        public WorkTicketHistoryItem CreatePersonItem(WorkTicketHistory w)
        {
            WorkTicketHistoryItem item = Instantiate(itemPrefab);
            item.Init(w);
            item.transform.SetParent(grid.transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            //item.gameObject.SetActive(true);
            return item;
        }

        /// <summary>
        /// 工作票筛选筛选
        /// </summary>
        public bool WorkTicketContains(WorkTicketHistory workTicketT)
        {
            if (WorkTicketContainsNO(workTicketT)) return true;
            if (WorkTicketContainsPerson(workTicketT)) return true;
            return false;
        }

        /// <summary>
        /// 筛选根据工作票编号
        /// </summary>
        public bool WorkTicketContainsNO(WorkTicketHistory workTicketT)
        {
            if (workTicketT.No.ToLower().Contains(TwoTicketHistoryUI_N.Instance.searchInput.text.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 筛选根据工作票负责人
        /// </summary>
        public bool WorkTicketContainsPerson(WorkTicketHistory workTicketT)
        {
            if (workTicketT.PersonInCharge.ToLower().Contains(TwoTicketHistoryUI_N.Instance.searchInput.text.ToLower()))
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
                //HistoryPersonsSearchUIItem itemT = o.GetComponent<HistoryPersonsSearchUIItem>();
                //if (selectPersonnelList.Contains(itemT))
                //{
                //    o.SetActive(false);
                //    continue;
                //}
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
                currentPageNum = currentPageNum - 1;
                pageCount = pageCount - 1;
                InputFieldPage.text = (pageCount).ToString();
                //int currentPageNumT = currentPageNum;
                //SetPreviousAndNextPageBtn();
                if (IsSearch || IsStartTime || IsEndTime)
                {
                    CreateGrid(searchList);
                }
                else
                {
                    CreateGrid(TwoTicketHistoryUI_N.Instance.workTicketHistoryList);

                }
            }


        }

        /// <summary>
        /// 下一页的按钮触发事件
        /// </summary>
        public void NextPageBtn_OnClick()
        {

            Debug.Log("NextPageBtn_OnClick!");

            //SetPreviousAndNextPageBtn();
            if (IsSearch || IsStartTime || IsEndTime)
            {
                double a = Math.Ceiling((double)searchList.Count / (double)showCount);
                int m = (int)a;

                if (currentPageNum < m && pageCount < m)
                {
                    currentPageNum = currentPageNum + 1;
                    pageCount = pageCount + 1;
                    InputFieldPage.text = (pageCount).ToString();
                    CreateGrid(searchList);
                }
                else if (currentPageNum==m)
                {
                    pageCount = m;
                    InputFieldPage.text = (pageCount).ToString();
                    CreateGrid(searchList);
                }

            }
            else
            {
                double a = Math.Ceiling((double)TwoTicketHistoryUI_N.Instance.workTicketHistoryList.Count / (double)showCount);
                int m = (int)a;
                if (currentPageNum < m && pageCount < m)
                {
                    currentPageNum = currentPageNum + 1;
                    pageCount = pageCount + 1;
                    InputFieldPage.text = (pageCount).ToString();
                    CreateGrid(TwoTicketHistoryUI_N.Instance.workTicketHistoryList);
                }
                else if (currentPageNum == m)
                {
                    pageCount = m;
                    InputFieldPage.text = (pageCount).ToString();
                    CreateGrid(searchList);
                }
            }

        }

        /// <summary>
        /// 设置前一页和后一页是否开启交互，即interactable
        /// </summary>
        public void SetPreviousAndNextPageBtn()
        {
            if (currentPageNum <= 0)
            {
                currentPageNum = 0;
                previousPageBtn.interactable = false;
            }
            else
            {
                previousPageBtn.interactable = true;
            }

            if (currentPageNum >= pageCount - 1)
            {
                currentPageNum = pageCount - 1;
                nextPageBtn.interactable = false;
            }
            else
            {
                nextPageBtn.interactable = true;
            }
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
                int MaxPage = (int)Math.Ceiling((double)TwoTicketHistoryUI_N.Instance.workTicketHistoryList.Count / (double)showCount);
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
                CreateGrid(TwoTicketHistoryUI_N.Instance.workTicketHistoryList);
            }
        }

        public void ChangeEndTime()
        {
            string startTime = StartTimeText.text;
            DateTime NewStartTime = Convert.ToDateTime(startTime);
            string CurrentTime = NewStartTime.ToString("yyyy年MM月dd日");
            EndTimeText.text = CurrentTime.ToString();
        }
    }
}
