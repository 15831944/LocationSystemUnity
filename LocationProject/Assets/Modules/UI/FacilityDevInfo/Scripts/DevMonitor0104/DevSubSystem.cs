using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Location.WCFServiceReferences.LocationServices;
public class DevSubSystem : MonoBehaviour {

    /// <summary>
    /// 子系统按钮预设
    /// </summary>
    public GameObject SubSystemPrefab;
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
    /// 监控项集合
    /// </summary>
    private List<DevSubSystemItem> NodePrefabPoor = new List<DevSubSystemItem>();


    /// <summary>
    /// 初始化监控信息
    /// </summary>
    /// <param name="systemList"></param>
    public void InitDevSubSystem(Dev_Monitor[] subSystem)
    {
        HideLastSystem();
        bool isValueEmpty = subSystem == null || subSystem.Length == 0 ? true : false;
        SubSystemContent.gameObject.SetActive(!isValueEmpty);
        EmptyValueText.gameObject.SetActive(isValueEmpty);
        if (isValueEmpty) return;
        int lastSystemIndex = NodePrefabPoor.Count - 1;
        List<DevSubSystemItem> NewItems = new List<DevSubSystemItem>();
        for (int i = 0; i < subSystem.Length; i++)
        {
            if (i <= lastSystemIndex)
            {
                InitItemInfo(NodePrefabPoor[i], i, subSystem[i]);
                NodePrefabPoor[i].gameObject.SetActive(true);
            }
            else
            {
                GameObject obj = Instantiate(SubSystemPrefab);
                obj.SetActive(true);
                obj.transform.parent = SubSystemContent.transform;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                DevSubSystemItem item = obj.GetComponent<DevSubSystemItem>();
                InitItemInfo(item, i, subSystem[i]);
                NewItems.Add(item);
            }
        }
        NodePrefabPoor.AddRange(NewItems);
        NodePrefabPoor[0].SelectItem();
    }
    /// <summary>
    /// 设置按钮背景
    /// </summary>
    /// <param name="item"></param>
    /// <param name="Index"></param>
    private void InitItemInfo(DevSubSystemItem item, int Index, Dev_Monitor systemInfo)
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
