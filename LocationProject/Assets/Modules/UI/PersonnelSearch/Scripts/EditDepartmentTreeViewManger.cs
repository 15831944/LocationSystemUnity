using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class EditDepartmentTreeViewManger : MonoBehaviour
{
    public static EditDepartmentTreeViewManger Instance;
    public EditDepartTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
    /// <summary>
    /// 部门划分
    /// </summary>
    public GameObject DepartmentWindow;
    /// <summary>
    /// 对应区域图片列表
    /// </summary>
    public List<Sprite> Icons;
    public Toggle  DepBut;
    //public TreeNode<TreeViewItem> Currentnode;
    public int  DepId;
    public string DepName;
    TreeNode<TreeViewItem> selectNode;
    void Start()
    {
        // ShowDepartmentDivideTree();
        DepBut.onValueChanged .AddListener(showDepUI);
    }
    public void showDepUI(bool B)
    {
        if (B)
        {
            DepartmentWindow.SetActive(true);
            ShowDepartmentDivideTree();
        }
        else
        {
            DepartmentWindow.SetActive(false);
            if (selectNode == null) return;
            Department SelectdepNode = selectNode.Item.Tag as Department;          
            EditPersonnelInformation.Instance.CurrentId = (int)SelectdepNode.Id ;
            EditPersonnelInformation.Instance.departmentText.text = SelectdepNode.Name;
            
  
        }
    }
    public void Awake()
    {
        Instance = this;
    } 
    public void ShowDepartmentDivideTree()
    {
        CommunicationObject.Instance.GetDepartmentTree((data) =>
        {
            if (data == null) return;
            nodes = new ObservableList<TreeNode<TreeViewItem>>();
            ShowFirstLayerNodes(data);
            Tree.Start();
            Tree.Nodes = nodes;
            SetListeners();

        });
             
    }
 
    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <param name="treeNode"></param>
    public void AddNodes(Department topoNode, TreeNode<TreeViewItem> treeNode)
    {
        treeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        if (topoNode.Children != null)
        {
            foreach (Department child in topoNode.Children)
            {
                var node = CreateTopoNode(child);
                treeNode.Nodes.Add(node);
                AddNodes(child, node);
            }
        }
       
       
    }
    /// <summary>
    /// 设置父节点的数量
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="num"></param>
    public void SetParentNodepersonnelNum(TreeNode<TreeViewItem> parentNode, int num)
    {
        if (parentNode != null)
        {
            int currentNum = 0;
            var nodeNum = parentNode.Item.Name;
            var array = nodeNum.Split(new char[2] { '(', ')' });
            if (array != null)
            {
                try
                {
                    string parentName = array[array.Length - 2];
                    currentNum = int.Parse(parentName);
                }
                catch
                {

                }
                if (parentNode.Item.Tag is Department)
                {
                    Department anode = parentNode.Item.Tag as Department;//取出区域的名称
                    parentNode.Item.Name = string.Format("{0} ({1})", anode.Name, currentNum + num);

                }
            }
            if (parentNode.Parent != null)
            {
                try
                {
                    //SetParentNodepersonnelNum(parentNode.Parent, num);
                }
                catch
                {
                    int i = 1;
                }
            }
        }

    }
    
  
    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <returns></returns>
    private TreeNode<TreeViewItem> CreateTopoNode(Department topoNode)
    {
        string title = topoNode.Name;
        if (topoNode.LeafNodes != null)
        {
            title = string.Format("{0} ", title);
        }

        var item = new TreeViewItem(title);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        
        return node;
    }

    /// <summary>
    /// 去掉节点
    /// </summary>
    /// <param name="node"></param>
    public void NodeDeselected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " deselected");
        //LocationManager.Instance.RecoverBeforeFocusAlign();
    }
    /// <summary>
    /// 选中节点
    /// </summary>
    /// <param name="node"></param>
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {

        //Currentnode = Tree.SelectedNode;
        if (node.Item.Tag is Department)
        {
            Department depNode = node.Item.Tag as Department;   
            EditPersonnelInformation.Instance.departmentText.text = depNode.Name;

            selectNode=node ;
        }
    }
    
    /// <summary>
    /// 展示第一层的节点
    /// </summary>
    /// <param name="root"></param>
    private void ShowFirstLayerNodes(Department root)
    {
        foreach (Department child in root.Children)
        {
            var rootNode = CreateTopoNode(child);
            nodes.Add(rootNode);
            AddNodes(child, rootNode);
            
        }
    }
    public void SetListeners()
    {
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
    }
}