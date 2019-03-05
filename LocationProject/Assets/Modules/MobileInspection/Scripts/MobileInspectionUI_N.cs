using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionUI_N : MonoBehaviour
{

    public static MobileInspectionUI_N Instance;

    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    public Text txtLineNum;//线路的数量

    public List<InspectionTrack> InspectionTrackList;

    public MobileInspectionItemUI mobileInspectionItemPrafeb;//列表单项
    public VerticalLayoutGroup grid;//列表
    public InputField searchInput;//搜索关键字输入框   
    public Button searchBtn;//搜索按钮
    [HideInInspector]
    public ToggleGroup toggleGroup;//Toggle组

    // Use this for initialization
    void Start()
    {
        Instance = this;
        InspectionTrackList = new List<InspectionTrack>();
        CommunicationCallbackClient.Instance.inspectionTrackHub.OnInspectionTrackRecieved += OnInspectionRecieved;
        personnelMobileInspectionList = new List<PersonnelMobileInspection>();

        toggleGroup = grid.GetComponent<ToggleGroup>();

       
    
        //WorkTicketBtn.onClick.AddListener(WorkTicketBtn_OnClick);
        //OperationTicketBtn.onClick.AddListener(OperationTicketBtn_OnClick);
        searchInput.onEndEdit.AddListener(SearchInput_OnEndEdit);
        searchBtn.onClick.AddListener(SearchBtn_OnClick);

        //WorkTicketToggle.onValueChanged.AddListener(WorkTicketToggle_ValueChanged);
        //OperationTicketToggle.onValueChanged.AddListener(OperationTicketToggle_ValueChanged);
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        mobileInspectionNum = 0;
      
        SetWindowActive(true);
        ShowMobileInspection();
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        SetWindowActive(false);

        // MobileInspectionManage.Instance.Hide();
        MobileInspectionInfoFollow.Instance.Hide();
        MobileInspectionDetailsUI.Instance.SetWindowActive(false);
        
        FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
    }



    /// <summary>
    /// 是否显示传统
    /// </summary>
    public void SetWindowActive(bool isActive)
    {
        window.SetActive(isActive);
   
    }


    /// <summary>
    /// 工作票按钮
    /// </summary>
    public void ShowMobileInspection()
    {
     
        TwoTicketSystemManage.Instance.Hide();
       
        searchInput.transform.Find("Placeholder").GetComponent<Text>().text = "请巡检编号或巡检人名称";
      
        Search();
      
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

       
        ShowWorkTicketGrid();
      
    }

   

    List<PersonnelMobileInspection> personnelMobileInspectionList;

    /// <summary>
    /// 创建显示工作票列表
    /// </summary>
    public void ShowWorkTicketGrid()
    {

        // GetPersonnelMobileInspectionList();

        txtLineNum.text = InspectionTrackList.Count.ToString();
        CreateWorkTicketGrid();

    }
    public int mobileInspectionNum = 0;

    public void CreateWorkTicketGrid()
    {
       
        ClearItems();
        List<InspectionTrack> listT = InspectionTrackList.FindAll((item) => WorkTicketContains(item));

        foreach (InspectionTrack w in listT)
        {
            mobileInspectionNum = mobileInspectionNum + 1;
            MobileInspectionItemUI itemT = CreateWorkTicketItem();
            itemT.Init(w);
        }

    }

    /// <summary>
    /// 获取工作票数据
    /// </summary>
  
    public void OnInspectionRecieved(List<InspectionTrack> info)
    {
        for (int i = 0; i < info.Count; i++)
        {
            InspectionTrackList.Add(info[i]);
        }
        
           
        
 
    }
    /// <summary>
    /// 创建工作票列表项
    /// </summary>
    public MobileInspectionItemUI CreateWorkTicketItem()
    {
        MobileInspectionItemUI itemT = Instantiate(mobileInspectionItemPrafeb);
        itemT.transform.SetParent(grid.transform);
        itemT.transform.localPosition = Vector3.zero;
        itemT.transform.localScale = Vector3.one;
        itemT.gameObject.SetActive(true);
        return itemT;
    }

    /// <summary>
    /// 工作票筛选筛选
    /// </summary>
    public bool WorkTicketContains(InspectionTrack personnelMobileInspectionT)
    {
        if (WorkTicketContainsNO(personnelMobileInspectionT)) return true;
        if (WorkTicketContainsPerson(personnelMobileInspectionT)) return true;
        return false;
    }

    /// <summary>
    /// 筛选根据工作票编号
    /// </summary>
    public bool WorkTicketContainsNO(InspectionTrack personnelMobileInspectionT)
    {
        if (personnelMobileInspectionT.Code.ToLower().Contains(searchInput.text.ToLower()))
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
    public bool WorkTicketContainsPerson(InspectionTrack personnelMobileInspectionT)
    {
        if (personnelMobileInspectionT.Name.ToLower().Contains(searchInput.text.ToLower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
  

    /// <summary>
    /// 设置字体颜色
    /// </summary>
    public void SetToggleTextColor(Toggle toggleT, bool isClicked)
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

