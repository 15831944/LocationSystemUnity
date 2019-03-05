using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoTicketSystem
{
    public enum TwoTicketState
    {
        工作票,
        操作票,
        None
    }
    public class TwoTicketSystemUI_N : MonoBehaviour
    {
        public static TwoTicketSystemUI_N Instance;

        public TwoTicketState State = TwoTicketState.None;

        /// <summary>
        /// 窗体
        /// </summary>
        public GameObject window;

        //public Button WorkTicketBtn;//工作票按钮
        //public Button OperationTicketBtn;//操作票按钮

        public Toggle WorkTicketToggle;//工作票Toggle
        public Toggle OperationTicketToggle;//操作票Toggle

        public WorkTicketItem_N workTicketItemPrefab;//列表单项
        public OperationTicketItem_N OperationTicketItemPrefab;//列表单项
        public VerticalLayoutGroup grid;//列表
        public InputField searchInput;//搜索关键字输入框   
        public Button searchBtn;//搜索按钮
        [HideInInspector]
        public ToggleGroup toggleGroup;//Toggle组

        // Use this for initialization
        void Start()
        {
            Instance = this;
            workTicketList = new List<WorkTicket>();

            toggleGroup = grid.GetComponent<ToggleGroup>();

            //WorkTicketBtn.onClick.AddListener(WorkTicketBtn_OnClick);
            //OperationTicketBtn.onClick.AddListener(OperationTicketBtn_OnClick);
            searchInput.onEndEdit.AddListener(SearchInput_OnEndEdit);
            searchBtn.onClick.AddListener(SearchBtn_OnClick);

            WorkTicketToggle.onValueChanged.AddListener(WorkTicketToggle_ValueChanged);
            OperationTicketToggle.onValueChanged.AddListener(OperationTicketToggle_ValueChanged);
        }

        // Update is called once per frame
        void Update()
        {

        }

        //private void OnDisable()
        //{
        //    WorkTicketToggle.isOn = false;
        //    OperationTicketToggle.isOn = false;
        //    State = TwoTicketState.None;
        //}

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            //ShowWorkTicket();
            WorkTicketToggle.isOn = true;
            SetWindowActive(true);
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            SetWindowActive(false);
            WorkTicketToggle.isOn = false;
            OperationTicketToggle.isOn = false;
            State = TwoTicketState.None;
            TwoTicketSystemManage.Instance.Hide();
            WorkTicketDetailsUI_N.Instance.SetWindowActive(false);
            OperationTicketDetailsUI_N.Instance.SetWindowActive(false);
            FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
        }

        public void WorkTicketToggle_ValueChanged(bool isOn)
        {
            if (isOn)
            {
                ShowWorkTicket();
                SetToggleTextColor(WorkTicketToggle, true);
            }
            else
            {
                SetToggleTextColor(WorkTicketToggle, false);
                WorkTicketDetailsUI_N.Instance.SetWindowActive(false);
            }
        }

        public void OperationTicketToggle_ValueChanged(bool isOn)
        {
            if (isOn)
            {
                ShowOperationTicket();
                SetToggleTextColor(OperationTicketToggle, true);
            }
            else
            {
                SetToggleTextColor(OperationTicketToggle, false);
                OperationTicketDetailsUI_N.Instance.SetWindowActive(false);
            }
        }

        public void ChangState(TwoTicketState stateT)
        {
            if (State != stateT)
            {
                State = stateT;
                searchInput.text = "";
                FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
            }
        }

        /// <summary>
        /// 是否显示传统
        /// </summary>
        public void SetWindowActive(bool isActive)
        {
            window.SetActive(isActive);
            //if (isActive == false)
            //{
            //    WorkTicketDetailsUI.Instance.SetWindowActive(false);
            //    OperationTicketDetailsUI.Instance.SetWindowActive(false);
            //}
        }


        /// <summary>
        /// 工作票按钮
        /// </summary>
        public void ShowWorkTicket()
        {
            if (State != TwoTicketState.工作票)
            {
                TwoTicketSystemManage.Instance.Hide();
                ChangState(TwoTicketState.工作票);
                searchInput.transform.Find("Placeholder").GetComponent<Text>().text = "请输入编号或负责人名称";
                //TwoTicketSystemManage.Instance.HideDemo();
                Search();
            }
        }

        /// <summary>
        /// 操作票按钮
        /// </summary>
        public void ShowOperationTicket()
        {
            if (State != TwoTicketState.操作票)
            {
                TwoTicketSystemManage.Instance.Hide();
                ChangState(TwoTicketState.操作票);
                searchInput.transform.Find("Placeholder").GetComponent<Text>().text = "请输入编号或监护人名称";
                //TwoTicketSystemManage.Instance.HideDemo();
                Search();
            }
        }



        /// <summary>
        /// 清除列表项
        /// </summary>
        public void ClearItems()
        {
            int childCount = grid.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(grid.transform.GetChild(i).gameObject);
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
                //WorkTicket
                Search();
            }

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
        /// 搜索
        /// </summary>
        public void Search()
        {

            if (State == TwoTicketState.工作票)
            {
                //searchPersonnels = personnels.FindAll((item) => Contains(item));
                ShowWorkTicketGrid();
            }
            else if (State == TwoTicketState.操作票)
            {
                ShowOperationTicketGrid();
            }


            //ShowPersonsGrid();
        }

        #region 工作票相关

        List<WorkTicket> workTicketList;

        /// <summary>
        /// 创建显示工作票列表
        /// </summary>
        public void ShowWorkTicketGrid()
        {
            Loom.StartSingleThread(() =>
            {
                GetWorkTicketData();
                Loom.DispatchToMainThread(() =>
                {
                    CreateWorkTicketGrid();
                });

            });
        }
        public int WorkNum = 0;

        public void CreateWorkTicketGrid()
        {
            ClearItems();
            WorkNum = 0;
            List<WorkTicket> searchWorkTicket = workTicketList.FindAll((item) => WorkTicketContains(item));

            foreach (WorkTicket w in searchWorkTicket)
            {
                WorkNum = WorkNum + 1;
                WorkTicketItem_N itemT = CreateWorkTicketItem();
                itemT.Init(w);
            }

        }

        /// <summary>
        /// 获取工作票数据
        /// </summary>
        public void GetWorkTicketData()
        {
            workTicketList = CommunicationObject.Instance.GetWorkTicketList();
        }

        /// <summary>
        /// 创建工作票列表项
        /// </summary>
        public WorkTicketItem_N CreateWorkTicketItem()
        {
            WorkTicketItem_N itemT = Instantiate(workTicketItemPrefab);
            itemT.transform.SetParent(grid.transform);
            itemT.transform.localPosition = Vector3.zero;
            itemT.transform.localScale = Vector3.one;
            itemT.gameObject.SetActive(true);
            return itemT;
        }

        /// <summary>
        /// 工作票筛选筛选
        /// </summary>
        public bool WorkTicketContains(WorkTicket workTicketT)
        {
            if (WorkTicketContainsNO(workTicketT)) return true;
            if (WorkTicketContainsPerson(workTicketT)) return true;
            return false;
        }

        /// <summary>
        /// 筛选根据工作票编号
        /// </summary>
        public bool WorkTicketContainsNO(WorkTicket workTicketT)
        {
            if (workTicketT.No.ToLower().Contains(searchInput.text.ToLower()))
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
        public bool WorkTicketContainsPerson(WorkTicket workTicketT)
        {
            if (workTicketT.PersonInCharge.ToLower().Contains(searchInput.text.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 操作票相关

        List<OperationTicket> operationTicketList;

        /// <summary>
        /// 创建显示工作票列表
        /// </summary>
        public void ShowOperationTicketGrid()
        {
            Loom.StartSingleThread(() =>
            {
                GetOperationTicketData();
                Loom.DispatchToMainThread(() =>
                {
                    CreateOperationTicketGrid();
                });

            });
        }
        public int OperationNum = 0;
        /// <summary>
        /// 创建操作票列表
        /// </summary>
        public void CreateOperationTicketGrid()
        {
            ClearItems();
            OperationNum = 0;
            List<OperationTicket> searchOperationTicket = operationTicketList.FindAll((item) => OperationTicketContains(item));

            foreach (OperationTicket w in searchOperationTicket)
            {
                OperationNum = OperationNum + 1;
                OperationTicketItem_N itemT = CreateOperationTicketItem();
                itemT.Init(w);
            }

        }

        /// <summary>
        /// 获取工作票数据
        /// </summary>
        public void GetOperationTicketData()
        {
            operationTicketList = CommunicationObject.Instance.GetOperationTicketList();
        }



        /// <summary>
        /// 创建操作票列表项
        /// </summary>
        public OperationTicketItem_N CreateOperationTicketItem()
        {
            OperationTicketItem_N itemT = Instantiate(OperationTicketItemPrefab);
            itemT.transform.SetParent(grid.transform);
            itemT.transform.localPosition = Vector3.zero;
            itemT.transform.localScale = Vector3.one;
            itemT.gameObject.SetActive(true);
            return itemT;
        }

        /// <summary>
        /// 操作票筛选筛选
        /// </summary>
        public bool OperationTicketContains(OperationTicket operationTicketT)
        {
            if (OperationTicketContainsNO(operationTicketT)) return true;
            if (OperationTicketContainsPerson(operationTicketT)) return true;
            return false;
        }

        /// <summary>
        /// 筛选根据操作票编号
        /// </summary>
        public bool OperationTicketContainsNO(OperationTicket operationTicketT)
        {
            if (operationTicketT.No.ToLower().Contains(searchInput.text.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 筛选根据操作票监护人
        /// </summary>
        public bool OperationTicketContainsPerson(OperationTicket operationTicketT)
        {
            if (operationTicketT.Guardian.ToLower().Contains(searchInput.text.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 设置字体颜色
        /// </summary>
        public void SetToggleTextColor(Toggle toggleT,  bool isClicked)
        {
            Text t = toggleT.GetComponentInChildren<Text>();
            if (t != null)
            {
                if (isClicked)
                {

                    t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);
                }
                else
                {
                    t.color = new Color(t.color.r, t.color.g, t.color.b, 0.5f);
                }
            }
        }

        /// <summary>
        /// Toggle组添加
        /// </summary>
        public void ToggleGroupAdd(Toggle toggleT)
        {
            toggleT.group = toggleGroup;
        }

        /// <summary>
        /// 设置所有的TogglesOff
        /// </summary>
        public void SetAllTogglesOff()
        {
            toggleGroup.SetAllTogglesOff();
        }
    }
}
