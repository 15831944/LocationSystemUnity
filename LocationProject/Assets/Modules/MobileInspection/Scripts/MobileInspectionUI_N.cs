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

    public Text txtLineNum;//巡检轨迹线路的数量
    public List<InspectionTrack> InspectionTrackList = new List<InspectionTrack>();
    public MobileInspectionItemUI mobileInspectionItemPrafeb;//列表单项
    public VerticalLayoutGroup grid;//巡检路线列表
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
        searchInput.onEndEdit.AddListener(SearchInput_OnEndEdit);//搜索框编辑后事件
        searchBtn.onClick.AddListener(SearchBtn_OnClick);//搜索按钮点击事件

        //WorkTicketBtn.onClick.AddListener(WorkTicketBtn_OnClick);
        //OperationTicketBtn.onClick.AddListener(OperationTicketBtn_OnClick);

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
        if (MobileInspectionInfoManage.Instance != null && MobileInspectionInfoManage.Instance.window != null)
        {
            MobileInspectionInfoManage.Instance.window.SetActive(false);//关闭巡检点详情窗口
        }
        MobileInspectionHistoryDetailsUI.Instance.SetWindowActive(false);//关闭巡检项详情窗口
        MobileInspectionHistoryDetailInfo.Instance.CloseMobileInspectionHistoyItemWindow();//关闭历史巡检点窗口
        MobileInspectionHistoryRouteDetails.Instance.CloseBtn_OnClick();//关闭历史巡检项窗口
        SetWindowActive(false);
        // MobileInspectionManage.Instance.Hide();
        MobileInspectionInfoFollow.Instance.Hide();
        MobileInspectionDetailsUI.Instance.SetWindowActive(false);//关闭巡检点窗口
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
    /// 巡检轨迹搜索框
    /// </summary>
    public void ShowMobileInspection()
    {    
        TwoTicketSystemManage.Instance.Hide();
        searchInput.transform.Find("Placeholder").GetComponent<Text>().text = "按巡检编号或巡检人名称搜索"; //搜索框提示信息
        Search();
    }		

    /// <summary>
    /// 搜索框编辑结束触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void SearchInput_OnEndEdit(string txt)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.Log("SearchInput_OnEndEdit!");           
            Search();
        }
    }

    /// <summary>
    /// 搜索按钮触发事件
    /// </summary>
    public void SearchBtn_OnClick()
    {
        //Debug.Log("SearchBtn_OnClick!");
        Search();
    }

    /// <summary>
    /// 搜索巡检轨迹列表
    /// </summary>
    public void Search()
    {      
        ShowInspectionTrackList();
    }

    List<PersonnelMobileInspection> personnelMobileInspectionList;

    /// <summary>
    /// 显示移动巡检列表
    /// </summary>
    public void ShowInspectionTrackList()
    {
        //GetPersonnelMobileInspectionList();
        //Debug.Log("ShowInspectionTrackList");
        InspectionTrackList = CommunicationObject.Instance.GetInspectionTrackList();	
		DisplayInspectionTrackList();
    }

	public void DisplayInspectionTrackList(){
        if(InspectionTrackList!=null)
        {
            txtLineNum.text = InspectionTrackList.Count.ToString();    //展示获取的线路数量
        }
        else
        {
            txtLineNum.text = "0";
        }		
		CreateInspectionTicketGrid();
	}

    public int mobileInspectionNum = 0;
	// 创建巡检轨迹列表
	public void CreateInspectionTicketGrid()
    {      
		ClearInspectionTrackItems();
		List<InspectionTrack> listT = InspectionTrackList.FindAll((item) => WorkTicketContains(item));
		listT.Sort ((a, b) => a.Code.CompareTo (b.Code));//根据巡检轨迹单号Code排序列表

		mobileInspectionNum = 0;
        foreach (InspectionTrack w in listT)
        {
            mobileInspectionNum = mobileInspectionNum + 1;
			MobileInspectionItemUI itemT = CreateInspectionTrackItem();
            itemT.Init(w);
        }
    }

	/// <summary>
	/// 创建巡检路线列表项
	/// </summary>
	public MobileInspectionItemUI CreateInspectionTrackItem()
	{
		MobileInspectionItemUI itemT = Instantiate(mobileInspectionItemPrafeb);
		itemT.transform.SetParent(grid.transform);
		itemT.transform.localPosition = Vector3.zero;
		itemT.transform.localScale = Vector3.one;
		itemT.gameObject.SetActive(true);
		return itemT;
	}

	/// <summary>
	/// 清除巡检路线列表项
	/// </summary>
	public void ClearInspectionTrackItems()
	{
		int childCount = grid.transform.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(grid.transform.GetChild(i).gameObject);
		}
	}

    /// <summary>
    /// 获取巡检轨迹数据
    /// </summary>
    public void OnInspectionRecieved(InspectionTrackList info)
    {
        //Debug.Log("OnInspectionRecieved");
		InspectionTrack[] AddTrack = info.AddTrack;//添加巡检轨迹
        InspectionTrack[] ReviseTrack = info.ReviseTrack;//修改
        InspectionTrack[] DeleteTrack = info.DeleteTrack;//删除
        for (int i = 0; i < AddTrack.Length; i++)
        {
            InspectionTrack item = AddTrack[i];
            InspectionTrack it = InspectionTrackList.Find(p=>p.Id == item.Id);
			if (it == null) {
				InspectionTrackList.Add (item);
			} else {
				Debug.LogError ("OnInspectionRecieved AddTrack it != null");
			}
        }

        for (int i = 0; i < ReviseTrack.Length; i++)
        {
            InspectionTrack item = ReviseTrack[i];
            InspectionTrack it = InspectionTrackList.Find(p => p.Id == item.Id);
			if (it != null) {
				int id = InspectionTrackList.IndexOf (it);
				InspectionTrackList [id] = item;
			} else {
				Debug.LogError ("OnInspectionRecieved ReviseTrack it == null");
			}
        }

        for (int i = 0; i < DeleteTrack.Length; i++)
        {
            InspectionTrack item = DeleteTrack[i];
            InspectionTrack it = InspectionTrackList.Find(p => p.Id == item.Id);
            if (it != null)
            {
                InspectionTrackList.Remove(it);
			} else {
				Debug.LogError ("OnInspectionRecieved DeleteTrack it == null");
			}
        }
        //Debug.Log(InspectionTrackList.Count);
		DisplayInspectionTrackList();
    }

    /// <summary>
    /// 巡检路线筛选条件
    /// </summary>
    public bool WorkTicketContains(InspectionTrack personnelMobileInspectionT)
    {
        if (WorkTicketContainsNO(personnelMobileInspectionT)) return true;
        if (WorkTicketContainsPerson(personnelMobileInspectionT)) return true;
        return false;
    }

    /// <summary>
    /// 筛选根据巡检编号
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
    /// 筛选根据巡检人名称
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

