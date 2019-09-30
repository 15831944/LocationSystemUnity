using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MonitorRange;

public class TopoTreeManager : MonoBehaviour
{
    public static TopoTreeManager Instance;
    //public string RootName;
    /// <summary>
    /// 滑动条
    /// </summary>
    public ScrollRect scrollRect;
    public ChangeTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
    public GameObject Window;
    /// <summary>
    /// 对应区域图片列表
    /// </summary>
    public List<Sprite> Icons;
    //TreeNode<TreeViewItem> rootNode;
    /// <summary>
    /// 收缩窗体动画
    /// </summary>
    private Tween ScaleWindowTween;
    /// <summary>
    /// 动画是否初始化
    /// </summary>
    private bool IsTweenInit;

    /// <summary>
    /// 区域列表
    /// </summary>
    private List<TreeNode<TreeViewItem>> AreaList = new List<TreeNode<TreeViewItem>>();
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        GetTopoTree();
    }
    /// <summary>
    /// 通过区域ID，获取区域节点
    /// </summary>
    /// <param name="depId"></param>
    /// <returns></returns>
    public TreeNode<TreeViewItem> TryGetAreaNode(int depId)
    {
        foreach (var item in AreaList)
        {
            PhysicalTopology topoTemp = item.Item.Tag as PhysicalTopology;
            if (topoTemp != null && topoTemp.Id == depId)
            {
                return item;
            }
        }
        return null;
    }
    #region 窗体收缩动画部分
    /// <summary>
    /// 初始化动画
    /// </summary>
    private void InitTween()
    {
        IsTweenInit = true;
        RectTransform rect = transform.GetComponent<RectTransform>();
        Vector2 endValue = rect.sizeDelta - new Vector2(0, 280);
        ScaleWindowTween = transform.GetComponent<RectTransform>().DOSizeDelta(endValue, 0.3f);
        ScaleWindowTween.SetAutoKill(false);
        ScaleWindowTween.Pause();
    }
    /// <summary>
    /// 缩放窗体
    /// </summary>
    /// <param name="isExpand">是否扩大窗体</param>
    public void ScaleWindow(bool isExpand)
    {
        if (!IsTweenInit)
        {
            InitTween();
        }
        if (isExpand)
        {
            ScaleWindowTween.OnRewind(ResizeTree).PlayBackwards();
        }
        else
        {
            ScaleWindowTween.OnComplete(ResizeTree).PlayForward();
        }
    }
    /// <summary>
    /// 刷新树控件
    /// </summary>
    public void ResizeTree()
    {
        if (Tree != null)
        {
            Tree.ResizeContent();
        }
        else
        {
            Debug.LogError("TopoTree is null...");
        }
    }
    #endregion
    /// <summary>
    /// 获取区域数据
    /// </summary>
    public void GetTopoTree()
    {
        Log.Info("TopoTreeManager->GetTopoTree");
        CommunicationObject.Instance.GetTopoTree((topoRoot) =>
        {
            if (topoRoot != null)
            {
                StructureTree(topoRoot);
                Tree.Start();
                Tree.Nodes = nodes;
                SetListeners();
                scrollRect.horizontal = true;
            }
            Log.Info("TopoTreeManager->GetTopoTree complete.");
        });
    }

    public void SetListeners()
    {
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
    }

    // called when node selected
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " selected");
        PhysicalTopology topoNode = node.Item.Tag as PhysicalTopology;
        if (topoNode != null)
        {
            SceneEvents.OnTopoNodeChanged(topoNode);
        }
    }

    // called when node deselected
    public void NodeDeselected(TreeNode<TreeViewItem> node)
    {
        Debug.Log(node.Item.Name + " deselected");
    }



    /// <summary>
    /// 展示区域树
    /// </summary>
    /// <param name="root"></param>
    public void StructureTree(PhysicalTopology root)
    {
        if (root == null 
            //|| root.Children == null //这部分要去掉，不如，新项目的空的厂区无法关联
            )
        {
            Log.Error("StructureTree root == null");
            return;
        }
        AreaList.Clear();
        nodes = new ObservableList<TreeNode<TreeViewItem>>();
        ShowFirstLayerNodes(root);
    }

    private void ShowFirstLayerNodes(PhysicalTopology root)
    {
        SceneEvents.TopoRootNode = root;
        foreach (PhysicalTopology child in root.Children)
        {
            var rootNode = CreateTopoNode(child);
            nodes.Add(rootNode);
            AddNodes(child, rootNode);
            if (IsExpandAll)
            {
                rootNode.IsExpanded = true;
            }
        }
    }

    public bool IsExpandAll = false;

    /// <summary>
    /// 添加根节点
    /// </summary>
    /// <param name="nodeName"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public TreeNode<TreeViewItem> AddRootNode(PhysicalTopology root)
    {
        if (root == null)
        {
            return null;
        }
        var treeNode = CreateTopoNode(root);
        nodes.Add(treeNode);
        var roomNodeFirst = nodes[0];
        return roomNodeFirst;
    }
    /// <summary>
    /// 设置选择节点
    /// </summary>
    /// <param name="dep"></param>
    public void SetSelectNode(DepNode lastDep, DepNode currentDep)
    {
        TreeNode<TreeViewItem> lastNode = null;
        TreeNode<TreeViewItem> currentNode = null;
        for (int i = 0; i < AreaList.Count; i++)
        {
            PhysicalTopology topoTemp = AreaList[i].Item.Tag as PhysicalTopology;
            if (topoTemp.Id == currentDep.NodeID) currentNode = AreaList[i];
            if (lastDep != null)
            {
                if (topoTemp.Id == lastDep.NodeID) lastNode = AreaList[i];
            }
            if (currentNode != null && lastNode != null) break;
        }

        if (lastDep != currentDep)
        {
            if (lastNode != null)
            {
                NarrowLastNode(lastNode);
            }
            if (currentNode != null) Tree.FindSelectNode(currentNode, false);
        }
    }
    /// <summary>
    /// 收起上一个节点
    /// </summary>
    /// <param name="lastDep"></param>
    public void NarrowLastNode(TreeNode<TreeViewItem> lastDep)
    {
        if (lastDep == null || lastDep.Parent == null) return;
        int? indexT = Tree.NodeToIndex(lastDep.Parent);
        if (indexT != null) Tree.DeselectNodeByIndex((int)indexT);
        if (indexT != null && lastDep.Parent.IsExpanded) Tree.ToggleNodeByIndex((int)indexT);
        NarrowLastNode(lastDep.Parent);
    }
    private TreeNode<TreeViewItem> CreateTopoNode(PhysicalTopology topoNode)
    {
        Sprite icon = GetTopoIcon(topoNode);
        var item = new TreeViewItem(topoNode.Name, icon);
        //var item = new TreeViewItem(string.Format("{0}[{1}]",topoNode.Name,topoNode.Transfrom!=null), icon);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        if (!AreaList.Contains(node)) AreaList.Add(node);
        return node;
    }

    private TreeNode<TreeViewItem> CreateDevNode(DevInfo dev)
    {
        Sprite icon = Icons[5];//设备图标 todo:以后可以判断是机柜还是设备，机柜则换上机柜图标
        var item = new TreeViewItem(dev.Name, icon);
        item.Tag = dev;
        var node = new TreeNode<TreeViewItem>(item);
        return node;
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
                if (IsExpandAll)
                {
                    node.IsExpanded = true;
                }
            }
        }

        if (topoNode.LeafNodes != null)
            foreach (DevInfo child in topoNode.LeafNodes)
            {
                var node = CreateDevNode(child);
                treeNode.Nodes.Add(node);
            }
    }
    /// <summary>
    /// 设置各物体的图标
    /// </summary>
    /// <param name="tpNode"></param>
    /// <returns></returns>
    public Sprite GetTopoIcon(PhysicalTopology tpNode)
    {
        Sprite icon = null;
        int typeNumber = (int)tpNode.Type - 1;
        if (typeNumber == -1)
        {
            icon = Icons[3];
        }
        else if (Icons.Count > typeNumber)
            icon = Icons[typeNumber];
        return icon;
    }
    /// <summary>
    /// 关闭设备拓朴树界面
    /// </summary>
    public void CloseWindow()
    {
        if (Window.activeInHierarchy)
            Window.SetActive(false);
    }
    /// <summary>
    /// 打开设备拓朴树界面
    /// </summary>
    public void ShowWindow()
    {
        if (!Window.activeInHierarchy)
            Window.SetActive(true);
    }
}
