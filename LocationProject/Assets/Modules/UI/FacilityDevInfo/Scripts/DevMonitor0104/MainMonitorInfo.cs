using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMonitorInfo : MonoBehaviour {
    /// <summary>
    /// 子系统按钮预设
    /// </summary>
    public GameObject MainInfoPrefab;
    /// <summary>
    /// 单 按钮背景图
    /// </summary>
    public Sprite singleOddSprite;
    /// <summary>
    /// 双 按钮背景图
    /// </summary>
    public Sprite doubleEvenSprite;
    /// <summary>
    /// 存放物体处
    /// </summary>
    public GameObject SubSystemContent;
    /// <summary>
    /// 空数据显示
    /// </summary>
    public GameObject EmptyValueText;
    /// <summary>
    /// 滚动条
    /// </summary>
    public Scrollbar scrollBar;
    /// <summary>
    /// 监控项集合
    /// </summary>
    private List<MainInfoItem> NodePrefabPoor = new List<MainInfoItem>();
    // Use this for initialization
    // Use this for initialization
    void Start () {
		
	}
    /// <summary>
    /// 初始化监控信息
    /// </summary>
    /// <param name="systemList"></param>
    public void InitMainDevInfo(DevMonitorNode[] monitorNode)
    {
        HideLastSystem();        
        bool isValueEmpty = monitorNode == null || monitorNode.Length == 0 ? true : false;
        SubSystemContent.gameObject.SetActive(!isValueEmpty);
        EmptyValueText.gameObject.SetActive(isValueEmpty);
        if (isValueEmpty) return;
        int lastSystemIndex = NodePrefabPoor.Count - 1;
        List<MainInfoItem> NewItems = new List<MainInfoItem>();
        for (int i = 0; i < monitorNode.Length; i++)
        {
            if (i <= lastSystemIndex)
            {
                InitItemInfo(NodePrefabPoor[i], i, monitorNode[i]);
                NodePrefabPoor[i].gameObject.SetActive(true);
            }
            else
            {
                GameObject obj = Instantiate(MainInfoPrefab);
                obj.SetActive(true);
                obj.transform.parent = SubSystemContent.transform;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                MainInfoItem item = obj.GetComponent<MainInfoItem>();
                InitItemInfo(item, i, monitorNode[i]);
                NewItems.Add(item);
            }
        }
        NodePrefabPoor.AddRange(NewItems);
        if (scrollBar) scrollBar.value = 1;
        //NodePrefabPoor[0].SelectItem();
    }
   
    /// <summary>
    /// 设置按钮背景
    /// </summary>
    /// <param name="item"></param>
    /// <param name="Index"></param>
    private void InitItemInfo(MainInfoItem item, int Index, DevMonitorNode systemInfo)
    {
        if (Index % 2 == 0)
        {
            item.Init(systemInfo, singleOddSprite);
        }
        else
        {
            item.Init(systemInfo, doubleEvenSprite);
        }
    }
    /// <summary>
    /// 关闭上一次的信息
    /// </summary>
    private void HideLastSystem()
    {
        if (NodePrefabPoor.Count != 0)
        {
            foreach (var item in NodePrefabPoor)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
