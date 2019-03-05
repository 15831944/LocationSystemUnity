using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MapNavigationItem : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// 当前Item对应区域
    /// </summary>
    [HideInInspector]
    public DepNode ItemNode;
    /// <summary>
    /// Item文本框
    /// </summary>
    public Text ItemText;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetItem(DepNode node,int index,int listCount)
    {
        string NodeName = node.NodeName;
        if(string.IsNullOrEmpty(node.NodeName))
        {
            NodeName = node.name;
        }
        if (index != listCount - 1) ItemText.text = string.Format(" {0}  /", NodeName);
        else ItemText.text = string.Format(" {0} ",NodeName);
        ItemNode = node;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(FactoryDepManager.currentDep==ItemNode)
        {
            Debug.Log("Node is alreay select.");
            return;
        }
        Debug.Log("SelectNode:"+ItemNode.NodeID);
        int currentIndex = transform.GetSiblingIndex();
        ClearInvalidChild(currentIndex, transform.parent);
        ResetPos();
        OpenDep(ItemNode);
    }
    /// <summary>
    /// 打开对应区域
    /// </summary>
    private void OpenDep(DepNode node)
    {
        RoomFactory factory = RoomFactory.Instance;
        if(factory)
        {
            factory.FocusNode(node);
        }
    }
    /// <summary>
    /// 重新设置所有标签位置
    /// </summary>
    private void ResetPos()
    {
        SmallMapController mapController = SmallMapController.Instance;
        if(mapController&&mapController.MapItemManager)
        {
            mapController.MapItemManager.ReculateSizePos();
        }
    }
    /// <summary>
    /// 清除多余节点
    /// </summary>
    /// <param name="index"></param>
    /// <param name="parent"></param>
    private void ClearInvalidChild(int index, Transform parent)
    {
        int childCount = parent.childCount;
        for (int i = childCount - 1; i > index; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}
