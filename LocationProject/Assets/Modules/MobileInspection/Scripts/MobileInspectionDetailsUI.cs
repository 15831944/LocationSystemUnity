using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionDetailsUI : MonoBehaviour
{
    public static MobileInspectionDetailsUI Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;

    public InspectionTrack info;//巡检轨迹信息
    public Text TxtNumber;//编号
                          //T1
	public Text CreateTime;//路线创建时间
    public Text TxtEstimatedStartingTime;//预计开始时间
    public Text TxtEstimatedEndingTime;//预计结束时间

    public Button DetailBut;//巡检点信息详情按钮
    public string TitleText;//列表标题文本
    // public Text StateText;
    // public Text InspectionNum;//巡检编号

    public PatrolPoint patrolPointItems;//巡检点列表
    public List<PatrolPoint> patrolPointList;
    //public Text TxtDutyOfficer;//值班负责人
    //public Text TxtDispatchingOfficer;//调度负责人

    public GameObject itemPrefab;//单项预设
    public VerticalLayoutGroup Grid;//巡检点列表

    // Use this for initialization
    void Start()
    {
        Instance = this;
        patrolPointList = new List<PatrolPoint>();

        //closeBtn.onClick.AddListener(CloseBtn_OnClick);
		DetailBut.onClick.AddListener(() =>
		{
			MobileInspectionInfoManage.Instance.GetInspectionPointInfoList(patrolPointList);
		});
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(InspectionTrack infoT)
    {
        patrolPointList.Clear();
        info = infoT;
		patrolPointList.AddRange(info.Route);
		patrolPointList.Sort ((a, b) => a.DeviceId.CompareTo (b.DeviceId));//巡检点列表根据巡检编号DeviceCode排序
        UpdateData();//刷新巡检轨迹详情数据
        CreateMeasuresItems();      
        SetWindowActive(true);
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void UpdateData()
    {
        TitleText = info.Code + "--" + info.Name; //列表标题文本
        TxtNumber.text = info.Name + "--" + info.Code + "(" + info.State + ")"; //编号文本
        CreateTime.text = info.dtCreateTime.ToString("yyyy年MM月dd日 HH:mm"); //路线创建时间
        TxtEstimatedStartingTime.text = info.dtStartTime.ToString("yyyy年MM月dd日 HH:mm"); //预计开始时间
        TxtEstimatedEndingTime.text = info.dtEndTime.ToString("yyyy年MM月dd日 HH:mm"); //预计结束时间
    }

    /// <summary>
	/// 创建措施(巡检点)列表
    /// </summary>
    public void CreateMeasuresItems()
    {
        ClearMeasuresItems();
        if (patrolPointList == null || patrolPointList.Count == 0) return;
        for (int i = 0; i < patrolPointList.Count; i++)
        {
            GameObject itemT = CreateMeasuresItem();
            Text[] ts = itemT.GetComponentsInChildren<Text>();
            PatrolPoint point = patrolPointList[i];
            string staffName=string.IsNullOrEmpty(point.StaffName)?point.StaffCode:point.StaffName;
            //string devName=string.IsNullOrEmpty(point.DevName)?point.DeviceCode:point.DevName;
            if (ts.Length > 0)
            {
                ts[0].text = string.IsNullOrEmpty(staffName) ? "--" : staffName;
            }
            if (ts.Length > 1)
            {

                ts[1].text = string.IsNullOrEmpty(point.KksCode) ? "--" : point.KksCode;
            }
        }
    }

    /// <summary>
	/// 创建措施(巡检点)项
    /// </summary>
    public GameObject CreateMeasuresItem()
    {
        GameObject itemT = Instantiate(itemPrefab);
        itemT.transform.SetParent(Grid.transform);
        itemT.transform.localPosition = Vector3.zero;
        itemT.transform.localScale = Vector3.one;
        LayoutElement layoutElement = itemT.GetComponent<LayoutElement>();
        layoutElement.ignoreLayout = false;
        itemT.SetActive(true);
        return itemT;
    }

    /// <summary>
	/// 清空措施(巡检点)列表
    /// </summary>
    public void ClearMeasuresItems()
    {
        int childCount = Grid.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(Grid.transform.GetChild(i).gameObject);
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
