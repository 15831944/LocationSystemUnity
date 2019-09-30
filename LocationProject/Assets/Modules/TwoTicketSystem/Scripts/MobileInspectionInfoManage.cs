using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionInfoManage : MonoBehaviour
{
    public static MobileInspectionInfoManage Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    public VerticalLayoutGroup grid;//巡检点详情列表
    public Text TitleText;
    public string DevName;
    public string PersonnelName;
    public Button Close_But;//关闭当前路线节点信息
    public MobileInspectionInfoDetail MobileInspectionInfoPrafeb;

	public GameObject TemplateInformation;//行的模板
	public int pageLine = 10; //每页显示条数
	private int startPageNum = 0; //切页数据
	private int pageNum = 1; //页数
	public Text pageTotalNum; //总页数
	public InputField pageNumText; //输入页数
	public Button nextPageBtn; //下一页
	public Button prevPageBtn; //上一页

	[System.NonSerialized]
	List<PatrolPoint> newPatrolPointList = new List<PatrolPoint>(); //一页存放的列表
	[System.NonSerialized]
	public List<PatrolPoint> allPatrolPointList; //巡检点全部列表

    public Sprite Singleline;
    public Sprite DoubleLine;

    void Start()
    {
        Instance = this;
        Close_But.onClick.AddListener(CloseWindow);

		allPatrolPointList = new List<PatrolPoint> ();
		pageNumText.onValueChanged.AddListener(InputPage);
		nextPageBtn.onClick.AddListener(NextPage);
		prevPageBtn.onClick.AddListener(PreviousPage);
    }

	//获取当前巡检轨迹中巡检点全部详情列表数据
	public void GetInspectionPointInfoList(List<PatrolPoint> data)
	{
        MobileInspectionHistoryDetailsUI.Instance.CloseBtn_OnClick();//关闭巡检项窗口
        MobileInspectionSubBar.Instance.SetHistoryToggle(false);//关闭巡检历史路线窗口
        MobileInspectionHistoryDetailInfo.Instance.CloseMobileInspectionHistoyItemWindow();//关闭历史巡检点窗口
        MobileInspectionHistoryRouteDetails.Instance.CloseBtn_OnClick();//关闭历史巡检项窗口
        SaveSelected();
		TitleText.text = MobileInspectionDetailsUI.Instance.TitleText;//巡检轨迹路线文本内容
		if (data.Count == 0) 
		{
			pageNumText.text = "1"; //输入页数
			pageTotalNum.text = "1"; //总页数
			return;
		} 
		else 
		{
			allPatrolPointList = new List<PatrolPoint> ();
			allPatrolPointList.AddRange (data);
			startPageNum = 0;
			pageNum = 1;
			GetListPages (allPatrolPointList);
			pageNumText.text = "1";
			TotalLine (allPatrolPointList);
			ShowWidow();
		}       
    }

	//根据列表数量生成页数
	public void TotalLine(List<PatrolPoint> data)
	{
		if (data.Count != 0) {
			if (data.Count % pageLine == 0) 
			{
				pageTotalNum.text = (data.Count / pageLine).ToString ();
			} 
			else
			{
				pageTotalNum.text = Convert.ToString (Math.Ceiling ((double)data.Count / (double)pageLine));//有小数加1
			}
		} 
		else 
		{
			pageTotalNum.text = "1";
		}
	}

	//设置移动巡检点详情列表界面
	public void SetInspectionInfo(List<PatrolPoint> data,int pageNum)
	{		
		for (int i = 0; i < data.Count; i++)
		{
			GameObject obj = InstantiateLine ();
			MobileInspectionInfoDetail item = obj.GetComponent<MobileInspectionInfoDetail> ();
            int dataIndex=pageNum*pageLine+(i+1);
			item.Init(data[i],dataIndex);
			DevName = data [i].DevName;
			PersonnelName = data [i].StaffName;
			if (i % 2 == 0)
			{
				item.GetComponent<Image>().sprite = DoubleLine;
			}
			else
			{
				item.GetComponent<Image>().sprite = Singleline;
			}
		}
	}

	//每行的预设
	public GameObject InstantiateLine()
	{
		GameObject Obj = Instantiate(TemplateInformation);
		Obj.SetActive(true);
		Obj.transform.parent = grid.transform;
		Obj.transform.localScale = Vector3.one;
		Obj.transform.localPosition = new Vector3 (Obj.transform.localPosition.x, Obj.transform.localPosition.y, 0);
		return Obj;
	}

	//生成的页数
	public void GetListPages(List<PatrolPoint> data)
	{
		newPatrolPointList.Clear ();
		if (startPageNum * pageLine < data.Count) 
		{
			var QueryData = data.Skip (startPageNum * pageLine).Take (pageLine);
			foreach(var list in QueryData)
			{
				newPatrolPointList.Add(list);
			}
			SetInspectionInfo(newPatrolPointList,startPageNum);
		}
	}

	//public void ShowWindows(bool b)
	//{
	//	window.SetActive(b);
	//	if (allPatrolPointList != null && allPatrolPointList.Count != 0) 
	//	{
	//		allPatrolPointList.Clear();
	//	}
	//	SaveSelected();
	//}

	//保留选中项
	public void SaveSelected()
	{
		for (int j = grid.transform.childCount - 1; j >= 0; j--) 
		{
			DestroyImmediate (grid.transform.GetChild (j).gameObject);
		}
	}

	//下一页
	public void NextPage()
	{
		startPageNum += 1;
		if (startPageNum <= allPatrolPointList.Count / pageLine) 
		{
			pageNum += 1;					
			pageNumText.text = pageNum.ToString ();
			SaveSelected ();
			GetListPages (allPatrolPointList);
		}
	}

	//上一页
	public void PreviousPage()
	{		
		if (startPageNum > 0) 
		{
			startPageNum--;
			pageNum -= 1;
			if (pageNum == 1) {
				pageNumText.text = "1";
			}
			else 
			{
				pageNumText.text = pageNum.ToString ();
			}
			SaveSelected ();
			GetListPages (allPatrolPointList);
		}
	}

	//选中页输入框
	public void InputPage(string value)
    {
        int currentPage = 0;
		currentPage = int.Parse(pageNumText.text);
		int maxPage = (int)Math.Ceiling((double)allPatrolPointList.Count /(double)pageLine);
		if (maxPage == 0)
        {
			pageNumText.text = "1";
        }else
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
			SaveSelected();
			GetListPages(allPatrolPointList);
        }
	}

    public void ShowWidow()
    {
        window.SetActive(true);
    }

    public void CloseWindow()
    {
		allPatrolPointList.Clear();
		SaveSelected();
        window.SetActive(false);
    }

    //public void CreatMobileInspectionInfo(List<PatrolPoint> patrolPointList)
    //{
    //    int i = 0;
    //    TitleText.text = MobileInspectionDetailsUI.Instance.TitleText;
    //    foreach (PatrolPoint w in patrolPointList)
    //    {
    //        i = i + 1;
    //        MobileInspectionInfoDetail item = CreateWorkTicketItem();
    //        item.Init(w);
    //        DevName = w.DevName;
    //        PersonnelName = w.StaffName;
    //        if (i % 2 == 0)
    //        {
    //            item.transform.gameObject.GetComponent<Image>().sprite = DoubleLine;
    //        }
    //        else
    //        {
    //            item.transform.gameObject.GetComponent<Image>().sprite = Singleline;
    //        }
    //    }
    //    ShowWidow();
    //}
    ///// <summary>
    ///// 创建移动巡检列表项
    ///// </summary>
    //public MobileInspectionInfoDetail CreateWorkTicketItem()
    //{
    //    MobileInspectionInfoDetail itemT = Instantiate(MobileInspectionInfoPrafeb);
    //    itemT.transform.SetParent(grid.transform);
    //    itemT.transform.localPosition = Vector3.zero;
    //    itemT.transform.localScale = Vector3.one;
    //    itemT.gameObject.SetActive(true);
    //    return itemT;
    //}
    ///// <summary>
    ///// 清除列表项
    ///// </summary>
    //public void ClearItems()
    //{
    //    int childCount = grid.transform.childCount;
    //    for (int i = childCount - 1; i >= 0; i--)
    //    {
    //        DestroyImmediate(grid.transform.GetChild(i).gameObject);
    //    }
    //}
}
