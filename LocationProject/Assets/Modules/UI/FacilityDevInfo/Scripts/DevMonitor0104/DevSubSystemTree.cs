using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class DevSubSystemTree : MonoBehaviour {

    /// <summary>
    /// 设备树
    /// </summary>
    public FacilitySystemTreeView Tree;
    /// <summary>
    /// 状态排序按钮
    /// </summary>
    public Button StateButton;
    /// <summary>
    /// 状态文本框
    /// </summary>
    public Text StateTitleText;
    /// <summary>
    /// 空数据
    /// </summary>
    public GameObject EmptyValueText;
    public Scrollbar scrollBar;
    /// <summary>
    /// 有效数据
    /// </summary>
    public GameObject EffectiveInfoContainer;
    /// <summary>
    /// 是否正常排序(正常/告警)
    /// </summary>
    private bool IsNormalState = true;
    /// <summary>
    /// 树节点
    /// </summary>
    [System.NonSerialized]
    ObservableList<TreeNode<TreeViewItem>> nodes;
    /// <summary>
    /// 当前系统
    /// </summary>
    [System.NonSerialized]
    private Dev_Monitor CurrentSystem;
    // Use this for initialization
    void Start()
    {
        SetListeners();
        StateButton.onClick.AddListener(Sort);
    }

    /// <summary>
    /// 排序
    /// </summary>
    private void Sort()
    {
        if (IsNormalState)
        {
            SortAlarm();
        }
        else
        {
            SortNormal();
        }
    }

    // 排序
    Comparison<TreeNode<TreeViewItem>> comparisonAlarm = (x, y) => {
        DevMonitorNode system1 = (DevMonitorNode)x.Item.Tag;
        DevMonitorNode system2 = (DevMonitorNode)y.Item.Tag;
        if (system1.Value == "告警" && system2.Value != "告警") return -1;
        else if ((system1.Value == "告警" && system2.Value == "告警") || (system1.Value != "告警" && system2.Value != "告警")) return 0;
        else return 1;
    };
    /// <summary>
    /// 告警排序
    /// </summary>
    private void SortAlarm()
    {
        IsNormalState = false;
        ApplyNodesSort(nodes, comparisonAlarm);
        StateTitleText.text = "状态(告警)";
    }
    /// <summary>
    /// 默认排序
    /// </summary>
    private void SortNormal()
    {
        IsNormalState = true;
        InitTree(CurrentSystem);
        StateTitleText.text = "状态(默认)";
    }
    /// <summary>
    /// 节点排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nodes"></param>
    /// <param name="comparison"></param>
    private void ApplyNodesSort<T>(ObservableList<TreeNode<T>> nodes, Comparison<TreeNode<T>> comparison)
    {
        nodes.Sort(comparison);
        nodes.ForEach(node => {
            if (node.Nodes != null)
            {
                ApplyNodesSort(node.Nodes as ObservableList<TreeNode<T>>, comparison);
            }
        });
    }

  
    /// <summary>
    /// 搜索（测试代码）
    /// </summary>
    public void SearchItem()
    {
        ObservableList<TreeNode<TreeViewItem>> nodeTemp = nodes.FindAll(i => i.Item.Name.Contains("7"));
        Debug.Log(nodeTemp.Count);
        Tree.Nodes = nodeTemp;
    }

    /// <summary>
    /// 设置树的点击相应
    /// </summary>
    public void SetListeners()
    {
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
    }

    // called when node selected
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " selected");
    }

    // called when node deselected
    public void NodeDeselected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " deselected");
    }
    /// <summary>
    /// 创建树
    /// </summary>
    /// <param name="SubSystem"></param>
    public void InitTree(Dev_Monitor SubSystem)
    {
        bool isEmpty = SubSystem == null || ((SubSystem.ChildrenList == null || SubSystem.ChildrenList.Length == 0) && SubSystem.MonitorNodeList == null);
        EffectiveInfoContainer.SetActive(!isEmpty);
        EmptyValueText.SetActive(isEmpty);
        if (isEmpty) return;

        CurrentSystem = SubSystem;
        nodes = new ObservableList<TreeNode<TreeViewItem>>();
        CreateRootMonitorNode(SubSystem.MonitorNodeList,nodes);
        if(SubSystem.ChildrenList!=null)
        {
            foreach (var child in SubSystem.ChildrenList)
            {
                var rootNode = GetDevNode(child);
                nodes.Add(rootNode);
                CreateChildNode(child, rootNode);
            }

        }       
        Tree.Start();
        Tree.Nodes = nodes;
        if (!IsNormalState) SortAlarm();
        if (scrollBar) scrollBar.value = 1;
    }
    /// <summary>
    /// 创建根节点(监控)
    /// </summary>
    /// <param name="dMonitorNode"></param>
    /// <param name="treeNodes"></param>
    private void CreateRootMonitorNode(DevMonitorNode[] dMonitorNode, ObservableList<TreeNode<TreeViewItem>> treeNodes)
    {
        //List<DevMonitorNode> nodeList = new List<DevMonitorNode>();
        //GetEffectiveNodeList(dMonitorNode,ref nodeList);
        if (dMonitorNode==null||dMonitorNode.Length == 0) return;
        foreach(var item in dMonitorNode)
        {
            TreeNode<TreeViewItem> node = GetDevMonitorNode(item);
            treeNodes.Add(node);
        }
    }
    /// <summary>
    /// 创建子节点(监控+设备)
    /// </summary>
    /// <param name="subDev"></param>
    /// <param name="treeNode"></param>
    private void CreateChildNode(Dev_Monitor subDev,TreeNode<TreeViewItem>treeNode)
    {
        DevMonitorNode[] nodeList = subDev.MonitorNodeList;
        if(treeNode.Nodes==null)treeNode.Nodes=new ObservableList<TreeNode<TreeViewItem>>();
        if (nodeList!=null&&nodeList.Length != 0)
        {
            foreach (var item in nodeList)
            {
                TreeNode<TreeViewItem> node = GetDevMonitorNode(item);
                treeNode.Nodes.Add(node);
            }
        }
        if(subDev.ChildrenList!=null)
        {
            foreach (var child in subDev.ChildrenList)
            {
                var devNode = GetDevNode(child);
                treeNode.Nodes.Add(devNode);
                CreateChildNode(child, devNode);
            }
        }      
    }
    /// <summary>
    /// 获取设备节点
    /// </summary>
    /// <param name="devNode"></param>
    /// <returns></returns>
    private TreeNode<TreeViewItem>GetDevNode(Dev_Monitor devNode)
    {
        int monitorNodeNum = GetEffectiveNodeList(devNode);
        string name = string.Format("{0} ({1})",devNode.Name,monitorNodeNum);
        var treeItem = new TreeViewItem(name);
        treeItem.Tag = devNode;
        var node = new TreeNode<TreeViewItem>(treeItem);
        return node;
    }
    /// <summary>
    /// 获取设备监控节点
    /// </summary>
    /// <param name="devNode"></param>
    /// <returns></returns>
    private TreeNode<TreeViewItem> GetDevMonitorNode(DevMonitorNode devNode)
    {
        var treeItem = new TreeViewItem(devNode.Describe);
        treeItem.Tag = devNode;
        var node = new TreeNode<TreeViewItem>(treeItem);
        return node;
    }
    /// <summary>
    /// 获取有效监控节点
    /// </summary>
    /// <param name="node"></param>
    /// <param name="nodeList"></param>
    /// <returns></returns>
    private int GetEffectiveNodeList(Dev_Monitor dev)
    {
        int value = 0;
        if (dev.MonitorNodeList != null)
        {
            value += dev.MonitorNodeList.Length;
        }
        if (dev.ChildrenList != null)
        {
            foreach (var subDev in dev.ChildrenList)
            {
                value += GetEffectiveNodeList(subDev);
            }
        }
        return value;
    }
}
