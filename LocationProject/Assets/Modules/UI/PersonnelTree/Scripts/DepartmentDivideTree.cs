using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DepartmentDivideTree : MonoBehaviour
{

    public Text DepartmentText;
    public string RootName;
    public ChangeTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
    public AreaDivideTree areaDivideTree;
    /// <summary>
    /// 部门划分
    /// </summary>
    public GameObject DepartmentWindow;
    /// <summary>
    /// 对应区域图片列表
    /// </summary>
    public List<Sprite> Icons;
    /// <summary>
    /// 人员信息列表
    /// </summary>
    public List<Personnel> personnels;
    ///// <summary>
    ///// 部门人员树数据
    ///// </summary>
    //Department topoRoot;
    /// <summary>
    /// 部门人员节点
    /// </summary>
    Dictionary<int, TreeNode<TreeViewItem>> DepartmentPersonDic = new Dictionary<int, TreeNode<TreeViewItem>>();
    Dictionary<int, TreeNode<TreeViewItem>> DepartmentDic = new Dictionary<int, TreeNode<TreeViewItem>>();

    void Start()
    {
        if (personnels == null)
        {
            personnels = new List<Personnel>();
        }
        //DepartmentDivideToggle.onValueChanged.AddListener(ShowDepartmentWindow);
    }
    public void ShowDepartmentDivideTree()
    {
        //Loom.StartSingleThread(() =>
        //{
        //    RefleshData(root =>
        //    {
        //        Loom.DispatchToMainThread(() =>
        //        {
        //            //StructureTree(topoRoot);
        //            StructureTree(root);
        //            Tree.Start();
        //            Tree.Nodes = nodes;
        //            SetListeners();
        //            //注释：用于崩溃测试；
        //            InvokeRepeating("RefleshDataThread", 1, CommunicationObject.Instance.DepartmentTreeRefreshInterval);
        //            //Invoke("RefleshDataThread", 1);
        //        });
        //    });
        //});

        RefleshData(root =>
        {
            //StructureTree(topoRoot);
            StructureTree(root);
            Tree.Start();
            Tree.Nodes = nodes;
            SetListeners();

            //注释：用于崩溃测试；
            InvokeRepeating("RefleshDataThread", 1, CommunicationObject.Instance.DepartmentTreeRefreshInterval);
            //Invoke("RefleshDataThread", 1);
        });

        //GetTopoTree();
        //Tree.Start();
        //Tree.Nodes = nodes;
        //SetListeners();
    }
    /// <summary>
    /// 获取区域数据
    /// </summary>
    public void GetTopoTree()
    {
        //topoRoot = CommunicationObject.Instance.GetDepTree();
        //StructureTree(topoRoot);
        //if (personnels == null)
        //{
        //    personnels = new List<Personnel>();
        //}
        //personnels = CommunicationObject.GetPersonnels(topoRoot);

        //Loom.StartSingleThread(() =>
        //{
        RefleshData(null);
        //    Loom.DispatchToMainThread(() =>
        //    {
        //        StructureTree(topoRoot);
        //    });
        //});

        //InvokeRepeating("RefleshDataThread", 1, 1);
    }

    /// <summary>
    /// 刷新树数据
    /// </summary>
    private void RefleshData(Action<Department> callback)
    {
        if (isRefresh)
        {
            Debug.LogWarning("RefleshData isRefresh:"+ isRefresh);
            return;
        }
        isRefresh = true;
        //if (!CommunicationObject.Instance.isAsync)
        //{
        //    var topoRoot = CommunicationObject.Instance.GetDepartmentTree();
        //    AfterGetDepTree(topoRoot, callback);
        //}
        //else
        //{
        //    CommunicationObject.Instance.GetDepartmentTreeAsync((data) =>
        //    {
        //        AfterGetDepTree(data, callback);
        //    });
        //}
        CommunicationObject.Instance.GetDepartmentTreeAsync((data) =>
        {
            AfterGetDepTree(data, callback);
            isRefresh = false;
        });
    }

    private void AfterGetDepTree(Department topoRoot, Action<Department> callback)
    {
        if (personnels == null)
        {
            personnels = new List<Personnel>();
        }
        personnels = CommunicationObject.GetPersonnels(topoRoot);//todo:这里也可以改成异步的

        RefreshDeppartmentTree();
        if (callback != null)
        {
            callback(topoRoot);
        }
    }

    bool isRefresh;
    /// <summary>
    /// 刷新树数据
    /// </summary>
    private void RefleshDataThread()
    {
        if (isRefresh) return;
        isRefresh = true;

        //if (!CommunicationObject.Instance.isAsync)
        //{
        //    Loom.StartSingleThread(() =>
        //    {
        //        try
        //        {
        //            var topoRoot = CommunicationObject.Instance.GetDepartmentTree();
        //            //StructureTree(topoRoot);
        //            if (personnels == null)
        //            {
        //                personnels = new List<Personnel>();
        //            }
        //            personnels = CommunicationObject.GetPersonnels(topoRoot);//没有发消息
        //            Loom.DispatchToMainThread(() =>
        //            {
        //                RefreshDeppartmentTree();
        //            });
        //        }
        //        catch
        //        {
        //            Debug.LogError("刷新部门树数据，出错！");
        //        }
        //        isRefresh = false;
        //    });
        //}
        //else
        //{
        //    CommunicationObject.Instance.GetDepartmentTreeAsync((data) =>
        //    {
        //        AfterGetDepTree(data, null);
        //    });
        //}

        CommunicationObject.Instance.GetDepartmentTreeAsync((data) =>
        {
            AfterGetDepTree(data, null);
            isRefresh = false;
        });
    }

    private void RefreshDeppartmentTree()
    {
        TreeNode<TreeViewItem> node = Tree.SelectedNode;
        foreach (var person in personnels)
        {
            if (DepartmentPersonDic.ContainsKey(person.Id))
            {
                TreeNode<TreeViewItem> personP = DepartmentPersonDic[person.Id];

                //Personnel personNode = (Personnel)personP.Item.Tag;

                int num = (int)personP.Item.Tag;
                Personnel personNode = personnels.Find((item) => item.TagId == num);

                if (personNode == null) continue;

                if (person.ParentId != personNode.ParentId)
                {
                    if (DepartmentDic.ContainsKey((int)person.ParentId))
                    {
                        SetMinusParentNodepersonnelNum(personP.Parent);
                        personP.Parent = DepartmentDic[(int)person.ParentId];
                        personNode.ParentId = person.ParentId;
                        SetAddParentNodepersonnelNum(personP.Parent);
                    }
                }
            }
            else
            {
                TreeNode<TreeViewItem> newperson = CreatePersonnalNode(person);
                if ((DepartmentDic.ContainsKey((int)person.ParentId)))
                {
                    if (DepartmentDic[(int)person.ParentId] != null)
                    {
                        newperson.Parent = DepartmentDic[(int)person.ParentId];
                        SetAddParentNodepersonnelNum(newperson.Parent);
                    }
                }
            }
        }
        RomvePersonnelNode(DepartmentPersonDic, personnels);
    }

    List<Personnel> romveNode = new List<Personnel>();
    /// <summary>
    /// 删除电场中消失的人
    /// </summary>
    /// <param name="perDic"></param>
    /// <param name="perList"></param>
    public void RomvePersonnelNode(Dictionary<int, TreeNode<TreeViewItem>> perDic, List<Personnel> perList)
    {
        romveNode.Clear();
        foreach (var item in perDic.Keys)
        {
            Personnel perNode = perList.Find(i => i.Id == item);
            if (perNode == null)
            {
                romveNode.Add(perNode);
            }
        }
        foreach (var per in romveNode)
        {
            TreeNode<TreeViewItem> personnelsP = DepartmentPersonDic[per.Id];
            DepartmentDic.Remove(per.Id);
            personnelsP.Dispose();
        }
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
        if (topoNode.LeafNodes != null)//添加子节点的子节点
            foreach (Personnel child in topoNode.LeafNodes)
            {
                var node = CreatePersonnalNode(child);
                treeNode.Nodes.Add(node);
            }
        if (topoNode.LeafNodes != null)
        {
            SetParentNodepersonnelNum(treeNode.Parent, topoNode.LeafNodes.Length);
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
                    SetParentNodepersonnelNum(parentNode.Parent, num);
                }
                catch
                {
                    int i = 1;
                }
            }
        }

    }
    /// <summary>
    /// 移除一个人员后，父节点数量减少
    /// </summary>
    /// <param name="perNode"></param>
    public void SetMinusParentNodepersonnelNum(TreeNode<TreeViewItem> parentNode)
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
                    if (currentNum - 1 > 0)
                    {
                        parentNode.Item.Name = string.Format("{0} ({1})", anode.Name, currentNum - 1);
                    }
                    else
                    {
                        parentNode.Item.Name = string.Format("{0} ", anode.Name);
                    }

                }
            }
            if (parentNode.Parent != null)
            {
                try
                {
                    SetMinusParentNodepersonnelNum(parentNode.Parent);
                }
                catch
                {
                    int i = 1;
                }
            }
        }

    }
    /// <summary>
    /// 添加一个人员后，父节点数量增加
    /// </summary>
    /// <param name="perNode"></param>
    public void SetAddParentNodepersonnelNum(TreeNode<TreeViewItem> parentNode)
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
                    parentNode.Item.Name = string.Format("{0} ({1})", anode.Name, currentNum + 1);
                }
            }
            if (parentNode.Parent != null)
            {
                try
                {
                    SetAddParentNodepersonnelNum(parentNode.Parent);
                }
                catch
                {
                    int i = 1;
                }
            }
        }

    }
    private TreeNode<TreeViewItem> CreatePersonnalNode(Personnel personnal)
    {
        Sprite icon = Icons[0];//设备图标 todo:以后可以判断是机柜还是设备，机柜则换上机柜图标
        var item = new TreeViewItem(personnal.Name, icon);
        item.Tag = personnal.Id;
        var node = new TreeNode<TreeViewItem>(item);
        if (!DepartmentPersonDic.ContainsKey(personnal.Id))
        {
            DepartmentPersonDic.Add(personnal.Id, node);
        }
        return node;
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
            title = string.Format("{0} ({1})", title, topoNode.LeafNodes.Length);
        }

        var item = new TreeViewItem(title);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        if (DepartmentDic.ContainsKey(topoNode.Id))
        {
            Debug.LogError(topoNode.Name);
        }
        else
        {
            DepartmentDic.Add(topoNode.Id, node);
        }
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

        Debug.Log(node.Item.Name + " selected");
        if (node.Item == null || node.Item.Tag == null)
        {
            //LocationManager.Instance.RecoverBeforeFocusAlign();
            return;
        }
        if (node.Item.Tag is Department)
        {
            LocationManager.Instance.RecoverBeforeFocusAlign();
            return;
        }
        if (!(node.Item.Tag is int) && !(node.Item.Tag is string))
        {
            //LocationManager.Instance.RecoverBeforeFocusAlign();
            return;
        }
        //Personnel tagT = node.Item.Tag as Personnel;
        int tagT = (int)node.Item.Tag;
        if (tagT != null)
        {
            ParkInformationManage.Instance.ShowParkInfoUI(false);
            AlarmPushManage.Instance.ShowAlarmPushWindow(false);
            //LocationManager.Instance.FocusPerson(tagT.Id);
            Personnel personnelT = personnels.Find((item) => item.Id == tagT);
            //LocationManager.Instance.FocusPersonAndShowInfo(tagT);
            LocationManager.Instance.FocusPersonAndShowInfo((int)personnelT.TagId);
        }
    }
    /// <summary>
    /// 展示树信息
    /// </summary>
    /// <param name="root"></param>
    public void StructureTree(Department root)
    {
        if (root == null)
        {
            Log.Error("StructureTree root == null");
            return;
        }
        nodes = new ObservableList<TreeNode<TreeViewItem>>();
        if (string.IsNullOrEmpty(RootName))
        {
            //不显示根节点，显示根节点下的第一级节点
            ShowFirstLayerNodes(root);
        }
        else
        {
            Department rootNode = root.Children.ToList().Find(i => i.Name == RootName);
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
    private void ShowFirstLayerNodes(Department root)
    {


        foreach (Department child in root.Children)
        {
            var rootNode = CreateTopoNode(child);
            nodes.Add(rootNode);
            AddNodes(child, rootNode);
            //rootNode.IsExpanded = true;
        }
    }
    public void SetListeners()
    {
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
    }
    /// <summary>
    /// 关闭部门划分
    /// </summary>
    public void CloseDepartmentWindow()
    {
        NoSelectedTextChange();
        DepartmentWindow.SetActive(false);


    }
    /// <summary>
    /// 打开设备拓朴树界面
    /// </summary>
    public void ShowDepartmentWindow(bool ison)
    {
        if (ison)
        {
            //areaDivideTree.ShowAreaDivideWindow(false);
            DepartmentWindow.SetActive(true);
            SelectedTextChange();
        }
        else
        {
            CloseDepartmentWindow();
        }
    }
    /// <summary>
    /// 选中后字体颜色改变
    /// </summary>
    public void SelectedTextChange()
    {
        DepartmentText.color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 255 / 255f);
    }
    /// <summary>
    /// 没有选中时字体的颜色
    /// </summary>
    public void NoSelectedTextChange()
    {
        DepartmentText.color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 100 / 255f);
    }


}
