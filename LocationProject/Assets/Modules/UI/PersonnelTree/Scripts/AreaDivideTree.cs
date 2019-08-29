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
    //public string RootName;
    public ChangeTreeView Tree;
    ObservableList<TreeNode<TreeViewItem>> nodes;
    /// <summary>
    /// 人员信息
    /// </summary>
    [System.NonSerialized] private List<PersonNode> PersonList;

    public PersonNode FindPersonNode(int perId)
    {
        return PersonList.Find((item) => item.Id == perId);
    }

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
    /// <summary>
    /// 根节点AreaNode,园区
    /// </summary>
    [System.NonSerialized] public AreaNode rootAreaNode;
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

    //public void RefreshShowAreaDivideTree()
    //{
    //    ShowAreaDivideTree(null);
    //}

    /// <summary>
    /// 
    /// </summary>
    private int? FactoryNodeNum = 2;
    private int? InFactoryNodeNum = 100000;

    //public int RefreshInterval = 5;

    public void StartRefreshAreaPersonnel()
    {
        return;
        ////注释：用于崩溃测试；
        //if(!IsInvoking("RefreshPersonnel"))//不加这个判断，可能会出现多个Repeating
        //{
        //    Debug.LogError("Start refresh personnel tree...");
        //    InvokeRepeating("RefreshPersonnel", 1, CommunicationObject.Instance.RefreshSetting.PersonTree);//todo:定时获取
        //    //Invoke("RefreshPersonnel", 1);
        //}
    }
    public void CloseeRefreshAreaPersonnel()
    {
        if (IsInvoking("RefreshPersonnel"))
        {
            Debug.LogError("Close Personnel Refresh....");
            CancelInvoke("RefreshPersonnel");
        }
    }

    /// <summary>
    /// 用InvokeRepeating，不断刷新树内容
    /// </summary>
    public void RefreshPersonnel()
    {
        if (isRefresh) return;
        isRefresh = true;
        CommunicationObject.Instance.GetPersonTree((topoRoot) =>
        {
            try
            {
                if (topoRoot == null)
                {
                    isRefresh = false;
                    return;
                }
                else
                {
                    PersonList = GetPersonNode(topoRoot);//获取数据库里面的数据
                    RefreshPersonnelTree();
                    isRefresh = false;//放到后面
                }
            }
            catch (Exception e)
            {
                Log.Error("AreaDivideTree.RefreshPersonnel", e.ToString());
                isRefresh = false;
            }

        });
    }

    public void RefreshPersonnelTree()
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
                        UpdatePersonnelNumOfText(personP.Parent,1);
                    }
                }
            }
            else
            {
                Log.Info("RefreshPersonnelTree", "AddPerson:" + person.Name);

                TreeNode<TreeViewItem> newperson = CreatePersonnalNode(person); //添加人物节点 
                if (newperson != null)
                {
                    //if (AreaDic[(int)person.ParentId] != null)
                    int CurrentParentID = (int)person.ParentId;
                    if (AreaDic.ContainsKey(CurrentParentID) || CurrentParentID == FactoryNodeNum)//上面的写法有问题
                    {
                        if (CurrentParentID == FactoryNodeNum)
                        {
                            CurrentParentID = (int)InFactoryNodeNum;
                        }
                        TreeNode<TreeViewItem> ParentPersonnel = AreaDic[CurrentParentID];
                        newperson.Parent = AreaDic[CurrentParentID];
                        //ParentPersonnel.Nodes.Add(newperson);//这个和上面一句重复了，会导致增加两个相同的节点
                        Log.Info("RefreshPersonnelTree", "SetAddParentNodepersonnelNum:" + person.Name);
                        UpdatePersonnelNumOfText(newperson.Parent, 1);
                    }
                }
            }
        }

        RomvePersonnelNode(personDic, PersonList);
        if (node != null && node.Item.Tag is PersonNode)
        {
            PersonNode per = node.Item.Tag as PersonNode;
            LocationObject currentLocationFocusObj = LocationManager.Instance.currentLocationFocusObj;
            if (currentLocationFocusObj != null && currentLocationFocusObj.personnel.Id != per.Id)
            {
                Tree.FindSelectNode(node);
            }
        }
    }

    /// <summary>
    /// 删除电场中消失的人
    /// </summary>
    /// <param name="perDic"></param>
    /// <param name="perList"></param>
    public void RomvePersonnelNode(Dictionary<int, TreeNode<TreeViewItem>> perDic, List<PersonNode> perList)
    {
        var romveIds = GetRemovePersonList(perDic, perList);
        foreach (var id in romveIds)
        {
            RemovePersonNode(id);
        }
    }

    //private void RemovePersonNode(PersonNode per)
    //{
    //    if (per == null) return;
    //    RemovePersonNode(per.Id, (int)per.ParentId);
    //}

    private void RemovePersonNode(int personId)
    {
        TreeNode<TreeViewItem> personnelP = personDic[personId];
        personnelP.Parent.Nodes.Remove(personnelP);//删除节点
        personDic.Remove(personId);//删除缓存

        UpdatePersonnelNumOfText(personnelP.Parent,-1);
    }



    //private void RemovePersonNode(int id,int pId)
    //{
    //    TreeNode<TreeViewItem> personnelP = personDic[id];
    //    bool IsPer = personDic.ContainsKey(pId);
    //    if (!IsPer) return;
    //    TreeNode<TreeViewItem> ParentPerId = AreaDic[pId];
    //    personDic.Remove(id);
    //    ParentPerId.Nodes.Remove(personnelP);
    //}

    private static List<int> GetRemovePersonList(Dictionary<int, TreeNode<TreeViewItem>> perDic, List<PersonNode> perList)
    {
        //perList是从数据库获取的当前的人员列表
        //perDic是当前树上的节点
        //List<PersonNode> romveNode = new List<PersonNode>();
        List<int> removeIds = new List<int>();
        foreach (var id in perDic.Keys)
        {
            PersonNode perNode = perList.Find(i => i.Id == id);
            if (perNode == null)//树上的人员id在列表中没有，说明该节点应该被删除
            {
                removeIds.Add(id);
            }
        }

        return removeIds;
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
        if (isRefresh) return;
        isRefresh = true;

        CommunicationObject.Instance.GetPersonTree((topoRoot) =>
        {
            isRefresh = false;
            if (topoRoot == null) return;

            rootAreaNode = topoRoot;
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

                if (IsExpandAll)
                {
                    node.IsExpanded = true;
                }
            }
        }
        if (topoNode.Persons != null)//添加子节点的子节点
        {
            foreach (var child in topoNode.Persons)
            {
                var node = CreatePersonnalNode(child);
                if (node != null)
                    treeNode.Nodes.Add(node);
            }
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
            //1.父节点原本就有人
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
            else
            {
                //2.父节点原来没人，直接把子节点的加上
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
    public void UpdatePersonnelNumOfText(TreeNode<TreeViewItem> parentNode,int changeNum)
    {
        if (parentNode != null)
        {
            int currentNum = 0;

            if (parentNode.Item != null)
            {
                var nodeNum = parentNode.Item.Name;
                var array = nodeNum.Split(new char[2] { '(', ')' });

                if (array != null)
                {
                    if (array.Length > 2)
                    {
                        string temp = array[array.Length - 2];
                        currentNum = int.Parse(temp);
                        SetNumber(parentNode, currentNum, changeNum);
                    }
                    else if(array.Length==1)//原本就没有人在那里的
                    {
                        SetNumber(parentNode, currentNum, changeNum);
                    }
                }
                if (parentNode.Parent != null)
                {
                    UpdatePersonnelNumOfText(parentNode.Parent, changeNum);
                }
            }
        }
    }

    private static void SetNumber(TreeNode<TreeViewItem> parentNode, int currentNum, int changeNum)
    {
        if (parentNode.Item.Tag is AreaNode)
        {
            AreaNode anode = parentNode.Item.Tag as AreaNode; //取出区域的名称
            var newNum = currentNum + changeNum;
            if (newNum > 0)
            {
                parentNode.Item.Name = string.Format("{0} ({1})", anode.Name, newNum);
            }
            else
            {
                parentNode.Item.Name = string.Format("{0}", anode.Name);
            }
        }
    }

    private TreeNode<TreeViewItem> CreatePersonnalNode(PersonNode personnal)
    {
        if (personnal == null) return null;
        Log.Info("CreatePersonnalNode", "p:" + personnal.Name);
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
            if (currentLocationFocusObj == null || currentLocationFocusObj.Tag.PersonId != personNodeT.Id)
            {
                Personnel personnelT = PersonnelTreeManage.Instance.GetPerson(personNodeT.Id);
                //Personnel personNode = personnels.Find((item) => item.TagId == num);
                Debug.LogError(node.Item.Name + " selected_FocusPersonAndShowInfo");
                //LocationManager.Instance.FocusPersonAndShowInfo(tagP.Id);
                LocationManager.Instance.FocusPersonAndShowInfo((int)personnelT.TagId);
            }
        }
        else
        {
            if (LocationManager.Instance.IsFocus)
            {
                LocationManager.Instance.RecoverBeforeFocusAlign(() =>
                {
                    SelectAreaNode(node);
                });
            }
            else
            {
                SelectAreaNode(node);
            }
        }
    }

    private void SelectAreaNode(TreeNode<TreeViewItem> node)
    {
        AreaNode togR = (AreaNode)node.Item.Tag;
        if (togR.Name == "厂区内")
        {
            RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);
        }
        else
        {
            try
            {
                DepNode NodeRoom = RoomFactory.Instance.GetDepNodeById(togR.Id);
                if (NodeRoom != null)
                {
                    RoomFactory.Instance.FocusNode(NodeRoom);
                }
                else
                {
                    Debug.LogError("AreaDivideTree.SelectAreaNode NodeRoom==null");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("AreaDivideTree.NodeSelected:" + ex);
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
            Log.Error("AreaDeviceTree.StructureTree","root == null");
            return;
        }

        Log.Info("AreaDeviceTree.StructureTree", "root:" + root.Name);
        personDic.Clear();
        AreaDic.Clear();
        nodes = new ObservableList<TreeNode<TreeViewItem>>();
        CreateTreeNodes(root);

    }
    /// <summary>
    /// 展示第一层的节点
    /// </summary>
    /// <param name="root"></param>
    private void CreateTreeNodes(AreaNode root)
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

            if (IsExpandAll)
            {
                rootNode.IsExpanded = true;
            }
        }
    }

    public bool IsExpandAll = false;

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
        PersonnelTreeManage.Instance.DepartAndJob_Bg.overrideSprite = Resources.Load("ChangeUI/treeview_border4", typeof(Sprite)) as Sprite; //切换图片

        if (b)
        {
            AreaWindow.SetActive(true);
            StartRefreshAreaPersonnel();
            SelectedTextChange();
        }
        else
        {
            NoSelectedTextChange();
            AreaWindow.SetActive(false);
            CloseeRefreshAreaPersonnel();
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

    private Sprite GetRoomIcon(AreaNode roomNode)
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

    /// <summary>
    /// 编辑区域： 新增区域时，添加树子节点
    /// </summary>
    /// <param name="parentAreaid"></param>
    /// <param name="childp"></param>
    public void AddAreaChild(PhysicalTopology parentp, PhysicalTopology childp)
    {
        int parentAreaid = parentp.Id;
        AreaNode childareaNode = PhysicalTopologyToAreaNode(childp);
        var node = CreateTopoNode(childareaNode);
        AreaNode parentNode;
        if (parentp.Type == AreaTypes.园区)//当等于2时，就是四会电厂区域根节点
        {
            parentNode = rootAreaNode;
            nodes.Add(node);
        }
        else
        {
            TreeNode<TreeViewItem> parentTreeNode = PersonnelTreeManage.Instance.FindAreaNode(parentAreaid);
            parentTreeNode.Nodes.Add(node);
            parentNode = (AreaNode)parentTreeNode.Item.Tag;
        }

        //AreaNode parentNode = (AreaNode)parentTreeNode.Item.Tag;
        List<AreaNode> parentNodeChildrenList;
        if (parentNode.Children != null)
        {
            parentNodeChildrenList = new List<AreaNode>(parentNode.Children);
        }
        else
        {
            parentNodeChildrenList = new List<AreaNode>();
        }
        parentNodeChildrenList.Add(childareaNode);

        parentNode.Children = parentNodeChildrenList.ToArray();
    }

    /// <summary>
    /// 编辑区域： 新增区域时，添加树子节点
    /// </summary>
    /// <param name="areaid"></param>
    /// <param name="childp"></param>
    public void RemoveAreaChild(int areaid)
    {
        TreeNode<TreeViewItem> treeNode = PersonnelTreeManage.Instance.FindAreaNode(areaid);

        AreaNode areaNode = (AreaNode)treeNode.Item.Tag;
        AreaNode parentAreaNode=null;
        if (areaNode.ParentId == 2)//当等于2时，就是四会电厂区域根节点
        {
            parentAreaNode = rootAreaNode;
        }
        else
        {
            if (treeNode.Parent.Item != null && treeNode.Parent.Item.Tag != null)
            {
                parentAreaNode = (AreaNode)treeNode.Parent.Item.Tag;
            }
        }
        if (parentAreaNode != null)
        {
            List<AreaNode> areaNodeChildrenList = new List<AreaNode>(parentAreaNode.Children);

            //if(areaNodeChildrenList.)
            AreaNode areaNodeT = areaNodeChildrenList.Find((i) => i.Id == areaid);
            if (areaNodeT != null)
            {
                areaNodeChildrenList.Remove(areaNodeT);
            }
            parentAreaNode.Children = areaNodeChildrenList.ToArray();
        }
        treeNode.Parent.Nodes.Remove(treeNode);
    }

    private static AreaNode PhysicalTopologyToAreaNode(PhysicalTopology item1)
    {
        if (item1 == null) return null;
        var item2 = new AreaNode();
        item2.Id = item1.Id;
        item2.Name = item1.Name;
        item2.ParentId = item1.ParentId;
        //item2.Parent = item1.Parent;
        item2.Type = item1.Type;
        //item2.Children = item1.Children.ToTModelS();
        //item2.LeafNodes = item1.LeafNodes.ToTModelS();
        item2.KKS = item1.KKS;

        return item2;
    }


    //public static AreaNode[] PhysicalTopologytoAreaNode(PhysicalTopology[] list1)
    //{
    //    if (list1 == null) return null;
    //    var list2 = new List<AreaNode>();
    //    foreach (var item1 in list1)
    //    {
    //        list2.Add(PhysicalTopologyToAreaNode(item1));
    //    }
    //    return list2.ToArray();
    //}

}
