using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class SubSytemTree : MonoBehaviour {
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
    /// 是否正常排序(正常/告警)
    /// </summary>
    private bool IsNormalState=true;
    /// <summary>
    /// 树节点
    /// </summary>
    ObservableList<TreeNode<TreeViewItem>> nodes;
    /// <summary>
    /// 当前系统
    /// </summary>
    private FacilitySystem CurrentSystem;
    // Use this for initialization
    void Start()
    {
        SetListeners();
        StateButton.onClick.AddListener(Sort);
    }
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.S))
        //{
        //    SortAlarm();
        //}else if(Input.GetKeyDown(KeyCode.A))
        //{
        //    SearchItem();
        //}
    }
    /// <summary>
    /// 排序
    /// </summary>
    private void Sort()
    {
        if(IsNormalState)
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
        FacilitySystem system1 = (FacilitySystem)x.Item.Tag;
        FacilitySystem system2 = (FacilitySystem)y.Item.Tag;
        if (system1.Status == "告警" && system2.Status != "告警") return -1;
        else if ((system1.Status == "告警" && system2.Status == "告警")||(system1.Status != "告警" && system2.Status != "告警")) return 0;
        else return 1;
    };
    /// <summary>
    /// 告警排序
    /// </summary>
    private void SortAlarm()
    {
        IsNormalState = false;
        //nodes.Sort(comparisonAlarm);
        ApplyNodesSort(nodes,comparisonAlarm);
        StateTitleText.text = "状态(告警)";
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
    private void SortChildAlarm()
    {

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
    public void SearchItem()
    {
        ObservableList<TreeNode<TreeViewItem>>nodeTemp = nodes.FindAll(i => i.Item.Name.Contains("7"));
        Debug.Log(nodeTemp.Count);
        Tree.Nodes = nodeTemp;
    }
    public void SetListeners()
    {
        //Tree.NodeSelected.AddListener(NodeSelected);
        //Tree.NodeDeselected.AddListener(NodeDeselected);
    }

    // called when node selected
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {
        FacilitySystem dev = node.Item.Tag as FacilitySystem;
        if (dev != null)
        {
            Debug.Log(string.Format("Status:{0}  Value:{1}",dev.Status,dev.Value));
        }
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
    public void InitTree(FacilitySystem SubSystem)
    {
        CurrentSystem = SubSystem;
        nodes = new ObservableList<TreeNode<TreeViewItem>>();
        foreach (FacilitySystem child in SubSystem.SubDevs)
        {
            var rootNode = CreateDevNode(child);
            nodes.Add(rootNode);
            AddNodes(child, rootNode);
            //rootNode.IsExpanded = true;
        }
        Tree.Start();
        Tree.Nodes = nodes;
        if (!IsNormalState) SortAlarm();
    }

    /// <summary>
    /// 创建设备节点
    /// </summary>
    /// <param name="devSystem"></param>
    /// <returns></returns>
    private TreeNode<TreeViewItem> CreateDevNode(FacilitySystem devSystem)
    {
        //Sprite icon = GetTopoIcon(topoNode);
        var item = new TreeViewItem(devSystem.DevName);
        item.Tag = devSystem;
        var node = new TreeNode<TreeViewItem>(item);
        return node;
    }
    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="devNode"></param>
    /// <param name="treeNode"></param>
    private void AddNodes(FacilitySystem devNode, TreeNode<TreeViewItem> treeNode)
    {
        treeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        if (devNode.SubDevs != null)
        {
            foreach (FacilitySystem child in devNode.SubDevs)
            {
                var node = CreateDevNode(child);
                treeNode.Nodes.Add(node);
                AddNodes(child, node);
            }
        }      
    }
}
