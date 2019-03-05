using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

public class AreaDivideTree : MonoBehaviour
{
    /// <summary>
    /// 类型字体颜色
    /// </summary>
    public Text AreaText;
    /// <summary>
    /// 部门划分拓朴树
    /// </summary>
    public DepartmentDivideTree departmentDivideTree;
    public string RootName;
    public ChangeTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
    /// <summary>
    /// 人员信息
    /// </summary>
    public List<PersonNode> PersonList;
    /// <summary>
    /// 部门划分
    /// </summary>
    public GameObject AreaWindow;
    /// <summary>
    /// 对应区域图片列表
    /// </summary>
    public List<Sprite> Icons;
    public List<Sprite> AreaRoomIcons;
    /// <summary>
    /// 人员节点
    /// </summary>
    Dictionary<int, TreeNode<TreeViewItem>> personDic = new Dictionary<int, TreeNode<TreeViewItem>>();
    /// <summary>
    /// 区域节点
    /// </summary>
    Dictionary<int, TreeNode<TreeViewItem>> AreaDic = new Dictionary<int, TreeNode<TreeViewItem>>();
    bool isRefresh;
    //public Toggle DivideToggle;
    void Start()
    {
        if (PersonList == null)
        {
            PersonList = new List<PersonNode>();
        }
        //  SceneEvents.FullViewStateChange += OnFullViewStateChange;
    }
    public void OnFullViewStateChange(bool b)
    {
        if (!b)
        {
            ShowAreaDivideWindow(true);
            ShowAreaDivideTree(PersonnelTreeManage.Instance.ClosePersonnelWindow);
            //PersonnelTreeManage.Instance.ClosePersonnelWindow();
        }
    }
    public void ShowAreaDivideTree(Action callback)
    {
        //GetTopoTree();//todo:因为定时获取会导致卡顿，以协程方式创建节点
        //Tree.Start();
        //Tree.Nodes = nodes;
        //SetListeners();
        GetTopoTree(() =>
        {
            Tree.Start();
            Tree.Nodes = nodes;
            SetListeners();
            if (callback != null)
            {
                callback();
            }
        });
    }
    /// <summary>
    /// 
    /// </summary>
    private int? FactoryNodeNum = 2;
    private int? InFactoryNodeNum = 100000;
    public void RefreshPersonnel()
    {
        if (isRefresh) return;
        isRefresh = true;
        CommunicationObject.Instance.GetPersonTreeAsync((topoRoot) =>
        {
            PersonList = GetPersonNode(topoRoot);//获取数据库里面的数据
            RefreshPersonnelTree();
            isRefresh = false;
        });
    }

    private void RefreshPersonnelTree()
    {
        TreeNode<TreeViewItem> node = Tree.SelectedNode;
        foreach (var person in PersonList)
        {
            if (personDic.ContainsKey(person.Id))//若该人员存在树里面
            {

                TreeNode<TreeViewItem> personP = personDic[person.Id];//取出该人员在人员树里面的节点
                PersonNode personNode = (PersonNode)personP.Item.Tag;
                if (person.ParentId != personNode.ParentId)//如果该人员现在所在的区域和在之前的区域不相等
                {
                    if (person.ParentId == FactoryNodeNum) person.ParentId = InFactoryNodeNum;
                    if (AreaDic.ContainsKey((int)person.ParentId))
                    {
                        SetMinusParentNodepersonnelNum(personP.Parent);
                        personP.Parent = AreaDic[(int)person.ParentId];//把该人员移到现在所在的区域中
                        personNode.ParentId = person.ParentId;
                        SetAddParentNodepersonnelNum(personP.Parent);
                    }
                }
            }
            else
            {
                TreeNode<TreeViewItem> newperson = CreatePersonnalNode(person); //添加人物节点 
                if (AreaDic[(int)person.ParentId] != null)
                {
                    newperson.Parent = AreaDic[(int)person.ParentId];
                    SetAddParentNodepersonnelNum(newperson.Parent);
                }

            }
        }

        RomvePersonnelNode(personDic, PersonList);
        if (node != null && node.Item.Tag is PersonNode)
        {
            PersonNode per = node.Item.Tag as PersonNode;
            LocationObject currentLocationFocusObj = LocationManager.Instance.currentLocationFocusObj;
            if (currentLocationFocusObj != null && currentLocationFocusObj.Tag.Id != per.Id)
            {
                Tree.FindSelectNode(node);
            }
        }
    }

    List<PersonNode> romveNode = new List<PersonNode>();
    /// <summary>
    /// 删除电场中消失的人
    /// </summary>
    /// <param name="perDic"></param>
    /// <param name="perList"></param>
    public void RomvePersonnelNode(Dictionary<int, TreeNode<TreeViewItem>> perDic, List<PersonNode> perList)
    {
        romveNode.Clear();
        foreach (var item in perDic.Keys)
        {
            PersonNode perNode = perList.Find(i => i.Id == item);
            if (perNode == null)
            {
                romveNode.Add(perNode);
            }
        }
        foreach (var per in romveNode)
        {
            if (per == null) continue;
            TreeNode<TreeViewItem> personnelP = personDic[per.Id];
            personDic.Remove(per.Id);
            personnelP.Dispose();
        }
    }


    private List<PersonNode> RandomPersonParent(List<PersonNode> lastPList)
    {
        List<PersonNode> newList = new List<PersonNode>();

        foreach (var person in lastPList)
        {
            PersonNode node = new PersonNode()
            {
                Id = person.Id,
                ParentId = UnityEngine.Random.Range(1, 200)
            };
            newList.Add(node);
        }
        return newList;
    }
    /// <summary>
    /// 获取区域数据
    /// </summary>
    public void GetTopoTree(Action callback)
    {

        //AreaNode topoRoot = CommunicationObject.Instance.GetPersonTree();
        //StructureTree(topoRoot);
        //if (PersonList == null)
        //{
        //    PersonList = new List<PersonNode>();
        //}
        //PersonList = GetPersonNode(topoRoot);
        if (isRefresh) return;
        isRefresh = true;

        CommunicationObject.Instance.GetPersonTreeAsync((topoRoot) =>
        {
            StructureTree(topoRoot);
            if (PersonList == null)
            {
                PersonList = new List<PersonNode>();
            }
            PersonList = GetPersonNode(topoRoot);
            if (callback != null)
            {
                callback();
            }
            isRefresh = false;
        });
    }
    /// <summary>
    /// 获取人员信息
    /// </summary>
    /// <param name="topoRoot"></param>
    /// <returns></returns>
    public static List<PersonNode> GetPersonNode(AreaNode topoRoot)
    {
        List<PersonNode> PersonNodeT = new List<PersonNode>();
        if (topoRoot == null) return PersonNodeT;
        if (topoRoot.Children == null) return PersonNodeT;
        foreach (AreaNode child in topoRoot.Children)
        {
            if (child.Persons != null)
            {
                PersonNodeT.AddRange(child.Persons);
            }
            PersonNodeT.AddRange(GetPersonNode(child));
        }
        return PersonNodeT;
    }
    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <param name="treeNode"></param>
    public void AddNodes(AreaNode topoNode, TreeNode<TreeViewItem> treeNode)
    {
        treeNode.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
        if (topoNode.Children != null)
        {
            foreach (AreaNode child in topoNode.Children)
            {
                var node = CreateTopoNode(child);
                treeNode.Nodes.Add(node);
                AddNodes(child, node);
            }
        }
        if (topoNode.Persons != null)//添加子节点的子节点
            foreach (var child in topoNode.Persons)
            {
                var node = CreatePersonnalNode(child);
                treeNode.Nodes.Add(node);
            }
        if (topoNode.Persons != null)
        {
            SetParentNodepersonnelNum(treeNode.Parent, topoNode.Persons.Length);
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
            var nodeNum = "";
            if (parentNode.Item != null)
            {
                nodeNum = parentNode.Item.Name;
            }
            var array = nodeNum.Split(new char[2] { '(', ')' });
            if (array != null && array.Length > 2)
            {
                try
                {
                    string parentName = array[array.Length - 2];
                    currentNum = int.Parse(parentName);
                }
                catch
                {

                }
                if (parentNode.Item.Tag is AreaNode)
                {
                    AreaNode anode = parentNode.Item.Tag as AreaNode;//取出区域的名称
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
            var nodeNum = "";
            if (parentNode.Item != null)
            {
                nodeNum = parentNode.Item.Name;
            }
            var array = nodeNum.Split(new char[2] { '(', ')' });
            if (array != null && array.Length > 2)
            {
                try
                {
                    string parentName = array[array.Length - 2];
                    currentNum = int.Parse(parentName);
                }
                catch
                {

                }
                if (parentNode.Item.Tag is AreaNode)
                {
                    AreaNode anode = parentNode.Item.Tag as AreaNode;//取出区域的名称
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
            
          if (parentNode.Item!=null)
            {
                var nodeNum = parentNode.Item.Name;
                var array = nodeNum.Split(new char[2] { '(', ')' });

                if (array != null&& array.Length>2)
                {
                    string parentName = array[array.Length - 2];
                    currentNum = int.Parse(parentName);


                    if (parentNode.Item.Tag is AreaNode)
                    {
                        AreaNode anode = parentNode.Item.Tag as AreaNode;//取出区域的名称
                        parentNode.Item.Name = string.Format("{0} ({1})", anode.Name, currentNum + 1);
                    }
                }
                if (parentNode.Parent != null)
                {

                    SetAddParentNodepersonnelNum(parentNode.Parent);


                }
            }
        }
         

    }
    private TreeNode<TreeViewItem> CreatePersonnalNode(PersonNode personnal)
    {
        TreeViewItem item = null;
        if (Icons != null && Icons.Count > 0)
        {
            Sprite icon = Icons[0];//设备图标 todo:以后可以判断是机柜还是设备，机柜则换上机柜图标
            item = new TreeViewItem(personnal.Name, icon);
        }
        else
        {
            item = new TreeViewItem(personnal.Name);
        }
        // item.Tag = personnal.Id;
        item.Tag = personnal;
        var node = new TreeNode<TreeViewItem>(item);
       if (!personDic.ContainsKey(personnal.Id))
        {
            personDic.Add(personnal.Id, node);
        }
       
        return node;
    }
    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param name="topoNode"></param>
    /// <returns></returns>
    private TreeNode<TreeViewItem> CreateTopoNode(AreaNode topoNode)
    {
        string title = topoNode.Name;

        if (topoNode.Persons != null)
        {
            title = string.Format("{0} ({1})", topoNode.Name, topoNode.Persons.Length);
        }
        Sprite icon = GetRoomIcon(topoNode);
        var item = new TreeViewItem(title, icon);
        item.Tag = topoNode;
        var node = new TreeNode<TreeViewItem>(item);
        if (AreaDic.ContainsKey(topoNode.Id))
        {
            Debug.LogError(topoNode.Name);
        }
        else
        {
            AreaDic.Add(topoNode.Id, node);
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
    }
    /// <summary>
    /// 选中节点
    /// </summary>
    /// <param name="node"></param>
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {
        //Debug.Log(node.Item.Name + " selected");
        Debug.LogError(node.Item.Name + " selected");
        if (node.Item == null || node.Item.Tag == null) return;
        if (node.Item.Tag is PersonNode)
        {
            ParkInformationManage.Instance.ShowParkInfoUI(false);
            PersonNode personNodeT = (PersonNode)node.Item.Tag;
            LocationObject currentLocationFocusObj = LocationManager.Instance.currentLocationFocusObj;
            if (currentLocationFocusObj == null || currentLocationFocusObj.Tag.Id != personNodeT.Id)
            {
                Personnel personnelT = PersonnelTreeManage.Instance.departmentDivideTree.personnels.Find((item) => item.Id == personNodeT.Id);
                //Personnel personNode = personnels.Find((item) => item.TagId == num);
                Debug.LogError(node.Item.Name + " selected_FocusPersonAndShowInfo");
                //LocationManager.Instance.FocusPersonAndShowInfo(tagP.Id);
                LocationManager.Instance.FocusPersonAndShowInfo((int)personnelT.TagId);
            }
        }
        else
        {

            AreaNode togR = (AreaNode)node.Item.Tag;
            if (togR.Name == "厂区内")
            {
                RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);
            }
            else
            {
                DepNode NodeRoom = RoomFactory.Instance.GetDepNodeById(togR.Id);
                if (NodeRoom != null)
                {
                    RoomFactory.Instance.FocusNode(NodeRoom);
                }
            }
        }
    }
    /// <summary>
    /// 展示树信息
    /// </summary>
    /// <param name="root"></param>
    public void StructureTree(AreaNode root)
    {
        if (root == null)
        {
            Log.Error("StructureTree root == null");
            return;
        }
        personDic.Clear();
        AreaDic.Clear();
        nodes = new ObservableList<TreeNode<TreeViewItem>>();
        //RootName = "四会热电厂";//先写死了
        if (string.IsNullOrEmpty(RootName))
        {
            //不显示根节点，显示根节点下的第一级节点
            ShowFirstLayerNodes(root);
        }
        else
        {
            AreaNode rootNode = root.Children.ToList().Find(i => i.Name == RootName);
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
    private void ShowFirstLayerNodes(AreaNode root)
    {
        if (root.Children == null) return;
        foreach (AreaNode child in root.Children)
        {
            if (child.Name == "厂区内")
            {
                int i = 0;
            }
            var rootNode = CreateTopoNode(child);
            nodes.Add(rootNode);
            AddNodes(child, rootNode);
            //rootNode.IsExpanded = true;
        }
    }
    public void SetListeners()
    {
        Tree.NodeSelected.RemoveAllListeners();
        Tree.NodeDeselected.RemoveAllListeners();
        Tree.NodeSelected.AddListener(NodeSelected);
        Tree.NodeDeselected.AddListener(NodeDeselected);
    }


    /// <summary>
    /// 打开设备拓朴树界面,展示区域划分
    /// </summary>
    public void ShowAreaDivideWindow(bool b)
    {
        if (b)
        {
            AreaWindow.SetActive(true);



            SelectedTextChange();
        }
        else
        {
            NoSelectedTextChange();
            AreaWindow.SetActive(false);
            CloseeRefreshAreaPersonnel();
        }
    }

    //public int RefreshInterval = 5;

    public void StartRefreshAreaPersonnel()
    {
        //注释：用于崩溃测试；
        InvokeRepeating("RefreshPersonnel", 1, CommunicationObject.Instance.PersonTreeRefreshInterval);//todo:定时获取
        //Invoke("RefreshPersonnel", 1);
    }
    public void CloseeRefreshAreaPersonnel()
    {
        if (IsInvoking("RefreshPersonnel"))
        {
            CancelInvoke("RefreshPersonnel");
        }
    }
    /// <summary>
    /// 选中后字体颜色改变
    /// </summary>
    public void SelectedTextChange()
    {
        AreaText.color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 255 / 255f);
    }
    /// <summary>
    /// 没有选中时字体的颜色
    /// </summary>
    public void NoSelectedTextChange()
    {
        AreaText.color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 100 / 255f);
    }

    public Sprite GetRoomIcon(AreaNode roomNode)
    {
        Sprite icon = null;
        int typeNum = (int)roomNode.Type - 1;
        if (typeNum == -1)
        {
            icon = AreaRoomIcons[0];
        }
        else if (AreaRoomIcons.Count > typeNum)
        {
            icon = AreaRoomIcons[typeNum];
        }
        return icon;
    }



}
