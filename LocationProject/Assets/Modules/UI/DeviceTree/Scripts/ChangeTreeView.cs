using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine;

public class ChangeTreeView : TreeView
{
    public bool EnableImage = true;
    public Sprite HighlightImage;
    public Sprite SelectColoringImage;
    public Sprite DefaultImage;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 更新树窗体
    /// </summary>
    public void ResizeContent()
    {
        base.Resize();
    }
    protected override void HighlightColoring(TreeViewComponent component)
    {
        base.HighlightColoring(component);
        if (EnableImage)
        {
            component.Background.sprite = HighlightImage;
        }
    }
    protected override void SelectColoring(TreeViewComponent component)
    {
        base.SelectColoring(component);
        if (EnableImage)
        {
            component.Background.sprite = SelectColoringImage;
        }
    }


    protected override void DefaultColoring(TreeViewComponent component)
    {
        if (component == null)
        {
            return;
        }
        base.DefaultColoring(component);
        if (EnableImage)
        {
            component.Background.sprite = DefaultImage;
        }
    }
    /// <summary>
    /// 选中节点，根据数据,区域划分
    /// </summary>
    public TreeNode<TreeViewItem> SelectNodeByType(object Data)
    {
        TreeNode<TreeViewItem> nodeT = FindNodeById(Data, Nodes);
        if (nodeT == null)
        {
            Debug.LogError("ChangeTreeView.SelectNodeByType nodeT == null Data:"+Data);
        }
        else
        {
            FindSelectNode(nodeT);
        }
        return nodeT;
    }

    /// <summary>
    /// 找到选中节点
    /// </summary>
    /// <param name="nodeT"></param>
    /// <param name="isExpandLastNode">是否展开当前节点</param>
    public void FindSelectNode(TreeNode<TreeViewItem> nodeT, bool isExpandLastNode = true)
    {

        if (nodeT != null)
        {
            SelectedNodes = new List<TreeNode<TreeViewItem>>() { nodeT };
            List<TreeNode<TreeViewItem>> nodesT = GetPathNodes(nodeT);
            for (int i = nodesT.Count - 1; i >= 0; i--)
            {
                try
                {
                    var item = nodesT[i];
                    List<int> indexs = Nodes2Indicies(new List<TreeNode<TreeViewItem>>() { item });
                    if (indexs.Count == 0) continue;//应该不会有的
                    var index = indexs[0];
                    if (!item.IsExpanded)
                    {

                        if (!isExpandLastNode && i == 0)
                        {
                            Debug.Log(string.Format("Node {0} not expand.", i));
                        }
                        else
                        {
                            ToggleNode(index);
                        }
                        if (i == 0)
                        {
                            Select(index);
                            ScrollTo(index);
                        }

                    }
                    else//2019_05_09_cww:已经展开了的节点也要滚动滚动条到该节点的位置
                    {
                        if (i == 0)//选中具体节点，其他已经展开了的节点就不用管了
                        {
                            Select(index);
                            ScrollTo(index);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("ChangeTreeView.FindSelectNode:" + ex);
                }
            }
        }
        else
        {
            Debug.LogError("异常：该节点在拓扑树中找不到！");
        }
    }
   
    /// <summary>
    /// 人员区域划分，选中节点
    /// </summary>
    /// <param name="personnelT"></param>
    /// <param name="nodesT"></param>
    /// <returns></returns>
    public TreeNode<TreeViewItem> FindNodeById(object personnelT, IObservableList<TreeNode<TreeViewItem>> nodesT)
    {
        TreeNode<TreeViewItem> node = null;
        if (nodesT != null)
        {
            foreach (TreeNode<TreeViewItem> nodeT in nodesT)
            {
                try
                {       
                    if (nodeT.Item.Tag is PersonNode)
                    {
                        PersonNode tagp = (PersonNode)nodeT.Item.Tag;
                        int tagId = tagp.Id;
                        if (tagId.ToString() == personnelT.ToString())
                        {
                            node = nodeT;
                            break;
                        }
                    }
                 
                    
                    if (nodeT.Nodes != null)
                    {
                        try
                        {
                            node = FindNodeById(personnelT, nodeT.Nodes);
                            if (node != null) break;
                        }
                        catch
                        {
                            int i = 0;
                        }
                    }
                }
                catch
                {
                    int It = 0;
                    return null;
                }
            }
        }
        return node;
    }
    /// <summary>
    /// 选中节点，根据数据,区域划分
    /// </summary>
    public void AreaSelectNodeByType(object Data)
    {
        TreeNode<TreeViewItem> nodeT = AreaFindNodeById(Data, Nodes);
        if (nodeT==null&&(FactoryDepManager.Instance&&Data.ToString()==FactoryDepManager.Instance.NodeID.ToString()))
        {
            TreeNode<TreeViewItem> selectNodeTemp = SelectedNode;
            RetractTreeNode(selectNodeTemp);
        }
        else
        {
            //TreeNode<TreeViewItem> nodeT = AreaFindNodeById(Data, Nodes);
            FindSelectNode(nodeT);
        }        
    }
    /// <summary>
    /// 收起展开的节点
    /// </summary>
    /// <param name="selectNodeTemp"></param>
    private void RetractTreeNode(TreeNode<TreeViewItem> selectNodeTemp)
    {
        if (selectNodeTemp == null) return;
        if(selectNodeTemp.IsExpanded)
        {
            List<int> indexs = Nodes2Indicies(new List<TreeNode<TreeViewItem>>() { selectNodeTemp });
            if (indexs.Count == 0) return;//应该不会有的
            var index = indexs[0];
            ToggleNode(index);
            if(selectNodeTemp.Parent!=null)
            {
                RetractTreeNode(selectNodeTemp.Parent);
            }
        }
    }

    /// <summary>
    /// 区域划分，选中节点
    /// </summary>
    /// <param name="personnelT"></param>
    public TreeNode<TreeViewItem> AreaFindNodeById(object AreaT)
    {
        TreeNode<TreeViewItem> nodeT = AreaFindNodeById(AreaT, Nodes);
        return nodeT;
    }

    /// <summary>
    /// 区域划分，选中节点
    /// </summary>
    /// <param name="personnelT"></param>
    /// <param name="nodesT"></param>
    /// <returns></returns>
    public TreeNode<TreeViewItem> AreaFindNodeById(object AreaT, IObservableList<TreeNode<TreeViewItem>> nodesT)
    {
        TreeNode<TreeViewItem> node = null;
        if (nodesT != null)
        {
            foreach (TreeNode<TreeViewItem> nodeT in nodesT)
            {
                try
                { 
                    if (nodeT.Item.Tag is AreaNode)
                    {
                        AreaNode tagp = (AreaNode)nodeT.Item.Tag;
                        int tagId = tagp.Id;
                        if(tagp .Name == "锅炉补给水处理车间0层")
                        {
                            string name = tagp.Name;
                        }
                        if (tagId.ToString() == AreaT.ToString())
                        {
                            node = nodeT;
                            break;
                        }

                    }
                    if (nodeT.Nodes != null)
                    {
                        try
                        {
                            node = AreaFindNodeById(AreaT, nodeT.Nodes);
                            if (node != null) break;
                        }
                        catch
                        {
                            int i = 0;
                        }
                    }
                }
                catch
                {
                    int It = 0;
                    return null;
                }
            }
        }
        return node;
    }
    /// <summary>
    /// 取消选中节点，根据数据,区域划分
    /// </summary>
    public void AreaDeselectNodeByData(object Data)
    {
        TreeNode<TreeViewItem> nodeT = FindNodeById(Data, Nodes);
        if (nodeT != null)
        {
       
            //SelectedNodes = new List<TreeNode<TreeViewItem>>() { nodeT };
            List<int> indexs = Nodes2Indicies(new List<TreeNode<TreeViewItem>>() { nodeT });
     
            if (indexs != null && indexs.Count == 1)
            {
                Deselect(indexs[0]);
            }
        }
        else
        {
            Debug.LogError("异常：该人员在拓扑树中找不到！");
        }
    }

    public TreeNode<TreeViewItem> FindNodeByData(object Data)
    {
        return FindNodeByData(Data, Nodes);
    }

    /// <summary>
    /// 选中节点，根据数据
    /// </summary>
    public TreeNode<TreeViewItem> SelectNodeByData(object Data)
    {
        //TreeNode<TreeViewItem> nodeT = nodes.Find((item) => item.Item.Tag == personnelT);
        TreeNode<TreeViewItem> nodeT = FindNodeByData(Data, Nodes);
        if (nodeT != null)
        {
            //Image image= nodeT.Item.
            SelectedNodes = new List<TreeNode<TreeViewItem>>() { nodeT };
            List<TreeNode<TreeViewItem>> nodesT = GetPathNodes(nodeT);

            for (int i = nodesT.Count - 1; i >= 0; i--)
            {
                if (!nodesT[i].IsExpanded)
                {
                    List<int> indexs = Nodes2Indicies(new List<TreeNode<TreeViewItem>>() { nodesT[i] });
                    try
                    {
                        ToggleNode(indexs[0]);
                        if (i == 0)
                        {
                            Select(indexs[0]);
                            ScrollTo(indexs[0]);
                        }
                    }
                    catch
                    {
                        int ii = 0;
                    }
                }
            }

            //List<int> indexs = Nodes2Indicies(nodesT);
            Debug.Log("SelectedIndex" + SelectedIndex);
            //Select(SelectedIndex);
            Debug.Log("node1:" + nodeT.Parent);
            Debug.Log("node2:" + nodeT.Parent.Parent);
            //Toggleno
            //Nodes2Indicies()
            //Refresh();
        }
        else
        {
            Debug.LogError("异常：该人员在拓扑树中找不到！："+Data);
        }
        return nodeT;
    }
    /// <summary>
    /// 获取节点的index
    /// </summary>
    /// <param name="nodeT"></param>
    /// <returns></returns>
    public int? NodeToIndex(TreeNode<TreeViewItem> nodeT)
    {
        List<int> indexs = Nodes2Indicies(new List<TreeNode<TreeViewItem>>() { nodeT });
        if (indexs != null && indexs.Count == 1)
        {
            return indexs[0];
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// ToggleNode
    /// </summary>
    /// <param name="index"></param>
    public void ToggleNodeByIndex(int index)
    {
        ToggleNode(index);
    }
    /// <summary>
    /// 取消节点选中
    /// </summary>
    /// <param name="index"></param>
    public void DeselectNodeByIndex(int index)
    {
        Deselect(index);
    }
    /// <summary>
    /// 取消选中节点，根据数据
    /// </summary>
    public void DeselectNodeByData(object Data)
    {
        TreeNode<TreeViewItem> nodeT = FindNodeByData(Data, Nodes);
        if (nodeT != null)
        {
            //SelectedNodes = new List<TreeNode<TreeViewItem>>() { nodeT };
            List<int> indexs = Nodes2Indicies(new List<TreeNode<TreeViewItem>>() { nodeT });

            if (indexs != null && indexs.Count == 1)
            {
                Deselect(indexs[0]);
            }
        }
        else
        {
            Debug.LogError("异常：该人员在拓扑树中找不到！");
        }
    }

    /// <summary>
    /// 找从该节点到父节点的Node列表
    /// </summary>
    /// <param name="nodeT"></param>
    /// <returns></returns>
    public List<TreeNode<TreeViewItem>> GetPathNodes(TreeNode<TreeViewItem> nodeT)
    {
        List<TreeNode<TreeViewItem>> nodesT = new List<TreeNode<TreeViewItem>>();
        if (nodeT.Item != null)
        {
            nodesT.Add(nodeT);
            nodesT.AddRange(GetPathNodes(nodeT.Parent));
        }
        return nodesT;
    }

    public TreeNode<TreeViewItem> FindNodeByData(object personnelT, IObservableList<TreeNode<TreeViewItem>> nodesT)
    {
        TreeNode<TreeViewItem> node = null;
        if (nodesT != null)
        {
            foreach (TreeNode<TreeViewItem> nodeT in nodesT)
            {
                if (nodeT == null) continue;
                try
                {
                    if (nodeT.Item.Tag != null)
                    {
                        object tagTemp = nodeT.Item.Tag;
                        if (tagTemp is Personnel)
                        {
                            //DepartmentTree.Tag由int变成了personnel,这样才能找到节点
                            Personnel person = tagTemp as Personnel;
                            if(person.Id.ToString()==personnelT.ToString())
                            {
                                node = nodeT;
                                break;
                            }
                        }
                        else if (nodeT.Item.Tag.ToString() == personnelT.ToString())
                        {
                            node = nodeT;
                            break;
                        }
                    }
                    if (nodeT.Nodes != null)
                    {
                        try
                        {
                            node = FindNodeByData(personnelT, nodeT.Nodes);
                            if (node != null) break;
                        }
                        catch
                        {
                            int I = 0;
                        }
                    }
                }
                catch
                {
                    int It = 0;
                    return null;
                }
            }

        }

        return node;

    }

    ///// <summary>
    ///// 
    ///// </summary>
    //[ContextMenu("Scroll")]
    //public void Scroll()
    //{
    //    Select(100);
    //    ScrollTo(100);

    //}
}
