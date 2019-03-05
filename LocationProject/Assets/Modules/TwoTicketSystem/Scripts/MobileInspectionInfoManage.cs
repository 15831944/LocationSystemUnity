using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionInfoManage : MonoBehaviour
{
    public static MobileInspectionInfoManage Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    public VerticalLayoutGroup grid;//列表
    public Text TitleText;

    public string DevName;
    public string PersonnelName;
    public Button Close_But;//关闭当前路线节点信息
    public MobileInspectionInfoDetail MobileInspectionInfoPrafeb;
    public Sprite Singleline;
    public Sprite DoubleLine;
    void Start()
    {
        Instance = this;
        Close_But.onClick.AddListener(CloseWindow);
    }
    public void CreatMobileInspectionInfo(List<PatrolPoint> patrolPointList)
    {
        int i = 0;
        TitleText.text = MobileInspectionDetailsUI.Instance.TitleText;
        foreach (PatrolPoint w in patrolPointList)
        {
            i = i + 1;
            MobileInspectionInfoDetail item = CreateWorkTicketItem();
            item.Init(w);
            DevName = w.DevName;
            PersonnelName = w.StaffName;
            if (i % 2 == 0)
            {
                item.transform.gameObject.GetComponent<Image>().sprite = DoubleLine;
            }
            else
            {
                item.transform.gameObject.GetComponent<Image>().sprite = Singleline;
            }
        }
        ShowWidow();
    }
    /// <summary>
    /// 创建移动巡检列表项
    /// </summary>
    public MobileInspectionInfoDetail CreateWorkTicketItem()
    {
        MobileInspectionInfoDetail itemT = Instantiate(MobileInspectionInfoPrafeb);
        itemT.transform.SetParent(grid.transform);
        itemT.transform.localPosition = Vector3.zero;
        itemT.transform.localScale = Vector3.one;
        itemT.gameObject.SetActive(true);
        return itemT;
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
    public void ShowWidow()
    {
        window.SetActive(true);
    }
    public void CloseWindow()
    {
        window.SetActive(false);
        ClearItems();
    }
}
