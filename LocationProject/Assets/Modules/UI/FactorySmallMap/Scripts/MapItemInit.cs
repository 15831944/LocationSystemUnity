using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapItemInit : MonoBehaviour {
    /// <summary>
    /// 首页按钮
    /// </summary>
    public Button HomeButton;
    /// <summary>
    /// 导航预设
    /// </summary>
    public GameObject MapItemPrefab;
    /// <summary>
    /// 导航容器
    /// </summary>
    public RectTransform NavigationContent;

    /// <summary>
    /// 遮罩的宽度
    /// </summary>
    private float MaskWidth;
    /// <summary>
    /// 导航自动布局组件
    /// </summary>
    private HorizontalLayoutGroup LayoutGroup;
    /// <summary>
    /// 组件是否初始化
    /// </summary>
    private bool IsLayoutInit;
    /// <summary>
    /// 界面显示，是否重新刷新
    /// </summary>
    private bool IsEnableRefresh;
    // Use this for initialization
    void Start () {
        HomeButton.onClick.AddListener(BackToMainFactory);
        InitLayout();
    }
    void OnEnable()
    {
        //Debug.LogError("MapItemInit.OnEnable");
        if(IsEnableRefresh)
        {
            IsEnableRefresh = false;
            ReculateSizePos();
        }
    }
    /// <summary>
    /// 返回主厂区
    /// </summary>
    public void BackToMainFactory()
    {
        if (LocationManager.Instance.IsFocus)
        {
            LocationManager.Instance.RecoverBeforeFocusAlign(()=>
            {
                Debug.LogError("BackToMainFactory");
                FactoryDepManager Manager = FactoryDepManager.Instance;
                if (Manager)
                {
                    AddItems(FactoryDepManager.Instance);
                    RoomFactory.Instance.FocusNode(Manager);
                }
            },false);
        }
        else
        {
            FactoryDepManager Manager = FactoryDepManager.Instance;
            if (Manager)
            {
                AddItems(FactoryDepManager.Instance);
                RoomFactory.Instance.FocusNode(Manager);
            }
        }
    }

    /// <summary>
    /// 返回主厂区
    /// </summary>
    public void BackToMainFactoryCall(Action callAction)
    {
        FactoryDepManager Manager = FactoryDepManager.Instance;
        if (Manager)
        {
            AddItems(FactoryDepManager.Instance);
            RoomFactory.Instance.FocusNode(Manager, callAction);
        }
    }
    /// <summary>
    /// 初始化布局组件
    /// </summary>
    /// <returns></returns>
    private void InitLayout()
    {
        if (IsLayoutInit) return;
        IsLayoutInit = true;
        MaskWidth = NavigationContent.parent.GetComponent<RectTransform>().sizeDelta.x;
        LayoutGroup = NavigationContent.GetComponent<HorizontalLayoutGroup>();
        //if (LayoutGroup != null) return true;
        //else return false;
    }
    /// <summary>
    /// 添加一个区域
    /// </summary>
    /// <param name="currentNode"></param>
    public void AddItems(DepNode currentNode)
    {
        Clear();
        AddAllNode(currentNode);
        ReculateSizePos();
    }
    private void AddAllNode(DepNode node)
    {
        List<DepNode> nodeList = new List<DepNode>();
        nodeList.Add(node);
        AddParentNode(nodeList,node);
        Debug.Log("NodeListCount:"+nodeList.Count);
        for(int i=0;i<nodeList.Count;i++)
        {
            AddChildItem(nodeList[i],i,nodeList.Count);
        }
    }
    private void AddParentNode(List<DepNode> nodeList,DepNode node)
    {
        if (nodeList != null && node.ParentNode != null)
        {
            nodeList.Insert(0, node.ParentNode);
            AddParentNode(nodeList,node.ParentNode);
        }
    }
    /// <summary>
    /// 清除所有导航栏
    /// </summary>
    private void Clear()
    {
        for(int i=NavigationContent.childCount-1;i>=0;i--)
        {
            //DestroyImmediate(NavigationContent.GetChild(i).gameObject);
            Destroy(NavigationContent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 添加导航子区域按钮
    /// </summary>
    /// <param name="currentNode"></param>
    /// <param name="index"></param>
    /// <param name="listCount"></param>
    private void AddChildItem(DepNode currentNode,int index,int listCount)
    {
        if(currentNode==null)
        {
            Debug.Log("Node is null..");
            return;
        }
        Log.Info("AddChildItem", currentNode.ToString());
        GameObject obj = Instantiate(MapItemPrefab, NavigationContent) as GameObject;
        obj.SetActive(true);
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        MapNavigationItem item = obj.GetComponent<MapNavigationItem>();
        item.SetItem(currentNode,index,listCount);
    }
    #region Layout布局刷新部分
    /// <summary>
    /// 重新计算位置信息
    /// </summary>
    /// <param name="action"></param>
    public void ReculateSizePos(Action action = null)
    {
        if (LayoutGroup == null) InitLayout();
        if (!gameObject.activeInHierarchy)
        {
            IsEnableRefresh = true;
            return;
        }
        StopCoroutine("CaculateSize");      
        StartCoroutine(CaculateSize(LayoutGroup.preferredWidth, NavigationContent, action));
    }
    /// <summary>
    /// 刷新Layout控件
    /// </summary>
    /// <param name="lastValue"></param>
    /// <param name="rectWordHor"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator CaculateSize(float lastValue, RectTransform rectWordHor, Action action = null)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectWordHor);
        yield return new WaitForEndOfFrame();
        int WaitTime = 0;
        while (LayoutGroup.preferredWidth == lastValue)
        {
            WaitTime++;
            //Debug.Log("Layout Width:" + LayoutGroup.preferredWidth);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectWordHor);
            yield return new WaitForEndOfFrame();
            if (WaitTime == 5) break;
        }
        ChangeNavigaionPos();
        if (action != null) action();
    }
    /// <summary>
    /// 设置导航栏位置
    /// </summary>
    private void ChangeNavigaionPos()
    {
        float CurrentWidth = NavigationContent.sizeDelta.x;
        if (CurrentWidth > MaskWidth)
        {
            float offset = -(CurrentWidth - MaskWidth) - (MaskWidth / 2);
            NavigationContent.localPosition = new Vector3(offset, NavigationContent.localPosition.y, 0);
        }
        else
        {
            float offset = -(MaskWidth / 2);
            NavigationContent.localPosition = new Vector3(offset, NavigationContent.localPosition.y, 0);

        }
    }
    #endregion
}
