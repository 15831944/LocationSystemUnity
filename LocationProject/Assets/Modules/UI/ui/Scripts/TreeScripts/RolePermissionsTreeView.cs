using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class RolePermissionsTreeView : MonoBehaviour
{
    public static RolePermissionsTreeView Instance;
    public RoleAreaTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
   
    /// <summary>
    /// 对应区域图片列表
    /// </summary>
    public List<Sprite> Icons;
    public string RootName;
    public  List<int> RolePermissionList;
    public List<int> ScreenRolePermissionList;
    public Sprite  SelectTogImage;
    public List<int> SaveRolePermissionList;
    public bool IsSure=false ;
    void  Start () {
        Instance = this;
     
    }
    public void GetAreaData(int RoleId)
    {
        RolePermissionList = new List<int>();
        RolePermissionList = CommunicationObject.Instance.GetCardRoleAccessAreas(RoleId);
        Instance.IsSure = false  ;
    }
    public void GetRolePermissionsTree()
    {
      
        CommunicationObject.Instance.GetTopoTree((data) =>
        {
            if (data != null)
            {
                StructureTree(data);
                Tree.Start();
                Tree.Nodes = nodes;
                SetListeners();
              //  SelectNode();

            }
            
        });
    }
  
    public void StructureTree(PhysicalTopology root)
    {
        if (root == null || root.Children == null)
        {
            Log.Error("StructureTree root == null");
            return;
        }
     
        nodes = new ObservableList<TreeNode<TreeViewItem>>();      
       
        if (string.IsNullOrEmpty(RootName))
        {
            //不显示根几点，显示根节点下的第一级节点
            ShowFirstLayerNodes(root);
        }
        else
        {
            PhysicalTopology rootNode = root.Children.ToList().Find(i => i.Name == RootName);
            if (rootNode != null)
            {
                ShowFirstLayerNodes(rootNode);//显示某一个一级节点下的内容
            }
            else
            {
                ShowFirstLayerNodes(root);
            }
        }
    }
   
    /// <summary>
    /// 展示第一层的节点
    /// </summary>
    /// <param name="root"></param>
    private void ShowFirstLayerNodes(PhysicalTopology root)
    {   
        foreach (PhysicalTopology child in root.Children)
        {
            var rootNode = CreateTopoNode(child);
            nodes.Add(rootNode);
            AddNodes(child, rootNode);

            //rootNode.IsExpanded = true;
        }
    }
    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <param name="treeNode"></param>
    public void AddNodes(PhysicalTopology topoNode, TreeNode<TreeViewItem> treeNode)
    {
        treeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        if (topoNode.Children != null)
        {
            foreach (PhysicalTopology child in topoNode.Children)
            {
                var node = CreateTopoNode(child);
                treeNode.Nodes.Add(node);
                AddNodes(child, node);
            }
        }

        if (topoNode.LeafNodes != null)
            foreach (DevInfo child in topoNode.LeafNodes)
            {
                var node = CreateDevNode(child);
                treeNode.Nodes.Add(node);
            }
    }
    private TreeNode<TreeViewItem> CreateDevNode(DevInfo dev)
    {
       // Sprite icon = Icons[5];//设备图标 todo:以后可以判断是机柜还是设备，机柜则换上机柜图标
        var item = new TreeViewItem(dev.Name);
        item.Tag = dev;
        var node = new TreeNode<TreeViewItem>(item);
        return node;
    }
    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <returns></returns>
    private TreeNode<TreeViewItem> CreateTopoNode(PhysicalTopology topoNode)
    {
        var item = new TreeViewItem(topoNode.Name);        
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
       
        return node;
       
    }
  
    /// <summary>
    /// 选中节点
    /// </summary>
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {      
    }
  
   
  
    public void SetListeners()
    {
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
     
    }
    public void NodeDeselected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " deselected");
    }
    // Update is called once per frame
    void Update () {
		
	}
}
