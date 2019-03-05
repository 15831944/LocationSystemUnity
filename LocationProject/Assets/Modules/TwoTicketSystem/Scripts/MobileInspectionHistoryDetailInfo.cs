using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryDetailInfo : MonoBehaviour {
    public static MobileInspectionHistoryDetailInfo Instance;
    public GameObject MobileInspectionHistoyItemWindow;
    public GameObject ItemPrefab;//措施单项
    public VerticalLayoutGroup Grid;//措施列表

    public Button closeBtn;//关闭

    public List<PatrolPointHistory> PatrolPointHistoryList;
    public MobileInspectionHistoryDetailInfoItem MobileInspectionHistoyItemPrafeb;
    public Text TitleText;

    public Sprite Singleline;
    public Sprite DoubleLine;
    void Start()
    {
        Instance = this;
        closeBtn.onClick.AddListener(CloseMobileInspectionHistoyItemWindow);
    }
    public void DateUpdate(InspectionTrackHistory list)
    {
        PatrolPointHistoryList.AddRange (list .Route);
        CreatInspectionHistoyDetailInfo();
        TitleText.text = list.Code + "-" + list.Name;
    }
    int i = 0;
    public void CreatInspectionHistoyDetailInfo()
    {
        foreach (PatrolPointHistory w in PatrolPointHistoryList)
        {
            i = i + 1;
            MobileInspectionHistoryDetailInfoItem item = CreateMeasuresItem();
            item.ShowInspectionTrackHistory(w);
            if (i % 2 == 0)
            {
                item.transform.gameObject.GetComponent<Image>().sprite = DoubleLine;
            }
            else
            {
                item.transform.gameObject.GetComponent<Image>().sprite = Singleline;
            }

        }
        ShowMobileInspectionHistoyItemWindow();

    }
    /// <summary>
    /// 创建措施项
    /// </summary>
    public MobileInspectionHistoryDetailInfoItem CreateMeasuresItem()
    {
        MobileInspectionHistoryDetailInfoItem itemT = Instantiate(MobileInspectionHistoyItemPrafeb);
        itemT.transform.SetParent(Grid.transform);
        itemT.transform.localPosition = Vector3.zero;
        itemT.transform.localScale = Vector3.one;
        itemT.gameObject.SetActive(true);
        return itemT;
    }

    /// <summary>
    /// 清空措施列表
    /// </summary>
    public void ClearMeasuresItems()
    {
        int childCount = Grid.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(Grid.transform.GetChild(i).gameObject);
        }
    }

    public void ShowMobileInspectionHistoyItemWindow()
    {
        
        MobileInspectionHistoyItemWindow.SetActive(true);
      
           
        
    }
    public void CloseMobileInspectionHistoyItemWindow()
    {
        MobileInspectionHistoyItemWindow.SetActive(false);
        ClearMeasuresItems();
    }
}
