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
    //public string RootName;
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
    [System.NonSerialized] public List<Personnel> personnels = new List<Personnel>();

    [System.NonSerialized] public List<Personnel> allPersonnels = new List<Personnel>();

    /// <summary>
    /// 人员信息列表（可能去除隐藏人员）
    /// </summary>
    private List<Personnel> personnelShowlist = new List<Personnel>();
    ///// <summary>
    ///// 部门人员树数据
    ///// </summary>
    //Department topoRoot;
    /// <summary>
    /// 部门人员节点
    /// </summary>
    Dictionary<int, TreeNode<TreeViewItem>> DepartmentPersonDic = new Dictionary<int, TreeNode<TreeViewItem>>();
    Dictionary<int, TreeNode<TreeViewItem>> DepartmentDic = new Dictionary<int, TreeNode<TreeViewItem>>();

    private bool isDepartmentTreeShow;

    [System.NonSerialized]
    private Department departmentInfo;//部门信息

    private bool isHideOfflinePerson;//是否隐藏离线人员
    void Start()
    {
        //DepartmentDivideToggle.onValueChanged.AddListener(ShowDepartmentWindow);
    }
    /// <summary>
    /// 删除部门树的某一个部门节点
    /// </summary>
    /// <param name="ID"></param>
    public void ReshDeleteDepartTree(Department depart)
    {
        bool IsDepart = DepartmentDic.ContainsKey(depart .Id );
        if (!IsDepart) return;
        TreeNode<TreeViewItem> DeleteDpart = DepartmentDic[depart.Id];
        if (depart.ParentId == null)
        {
            nodes.Remove(DeleteDpart);
        }
        else
        {
            
            TreeNode<TreeViewItem> ParentDpartNode = DepartmentDic[(int )depart.ParentId];
            ParentDpartNode.Nodes.Remove(DeleteDpart);
        }        
    }
    /// <summary>
    /// 添加部门
    /// </summary>
    /// <param name="depart"></param>
    public void ReshAddDepartTree(Department depart)
    {
        int ParentId =(int ) depart.ParentId;
        TreeNode<TreeViewItem> ParentDpartNode = DepartmentDic[ParentId];
        TreeNode<TreeViewItem> CreatDapartNode = CreateTopoNode(depart);
        Department CreatDapart = (Department)CreatDapartNode.Item.Tag; 
        CreatDapart.ParentId = depart.ParentId;
        ParentDpartNode.Nodes .Add(CreatDapartNode);
        AddNodes(depart, CreatDapartNode);
    }
    DateTime recordTime;
    public void ShowDepartmentDivideTree()
    {
        //之前存在问题的步骤：
        //1.服务端获取部门信息，通过部门信息获取人员信息
        //2.先创建personnel节点，存在DepartmentPersonDic中
        //3.人员创建完成后，回调中开始创建部门树(如果Department中，leafNodes中有personnel信息，那么上面先创建personnel是多余的)
        //新的步骤（取消了上面第二步，把刷新树和构造树分离开来）
        recordTime = DateTime.Now;
        GetDepartmentData(root =>
        {
            //StructureTree(topoRoot);
            StructureTree(root);
            Tree.Start();
            Tree.Nodes = nodes;
            SetListeners();
            AfterGetDepTree(root);
        });
    }
    /// <summary>
    /// 从服务端获取部门信息
    /// </summary>
    /// <param name="onDataRecieved"></param>
    private void GetDepartmentData(Action<Department> onDataRecieved)
    {
        //之前把下面的方法放在Threadmanager,提示UI线程的错误
        CommunicationObject.Instance.GetDepartmentTree((data) =>
        {
            if (data == null) return;
            departmentInfo = data;
            if (onDataRecieved != null) onDataRecieved(data);
        });
    }
    private int backupRefreshTime = 10;
    /// <summary>
    /// 打开/关闭部门树时，刷新/取消刷新数据
    /// </summary>
    /// <param name="isRefresh"></param>
    private void SetTreeRefresh(bool isRefresh)
    {
        if (isRefresh)
        {
            if (!IsInvoking("GetTopoTree"))
            {
                //如果单例为空，采用备用刷时间
                int refreshTime = CommunicationObject.Instance == null ? backupRefreshTime : CommunicationObject.Instance.RefreshSetting.PersonTree;
                InvokeRepeating("GetTopoTree", 0, refreshTime);
                Debug.LogError("StartRefresh department tree,refresh time:" + refreshTime);
            }
        }
        else
        {
            if (IsInvoking("GetTopoTree"))
            {
                CancelInvoke("GetTopoTree");
                Debug.LogError("Close department tree refresh...");
            }
        }
    }
    /// <summary>
    /// 从服务端获取人员信息
    /// </summary>
    /// <returns></returns>
    public List<Personnel> GetPersonnelFromServer()
    {
        if (CommunicationObject.Instance == null) return new List<Personnel>();
        Personnel[] personGroup = CommunicationObject.Instance.GetPersonnels();
        if (personGroup != null && personGroup.Length != 0)
        {
            return personGroup.ToList();
        }
        else
        {
            return new List<Personnel>();
        }
    }
    /// <summary>
    /// 获取区域数据（实际作用：刷新部门树）
    /// </summary>
    public void GetTopoTree()
    {
        RefleshData();
    }

    /// <summary>
    /// 刷新树数据
    /// </summary>
    private void RefleshData()
    {
        if (isRefresh)
        {
            Debug.LogWarning("RefleshData isRefresh:" + isRefresh);
            return;
        }
        isRefresh = true;
        GetDataFromServer(()=> 
        {
            isRefresh = false;
            SetCurrentShowPerson();
            RefreshDeppartmentTree();
            //一次刷新大概花费4s
        });
    }
    /// <summary>
    /// 从服务端获取数据
    /// </summary>
    private void GetDataFromServer(Action onComplete=null)
    {
        CommunicationObject.Instance.GetDepartmentTree((data) =>
        {            
            if (data == null)
            {
                if (onComplete != null) onComplete();
            }
            else
            {
                ThreadManager.Run(() =>
                {
                    personnels = CommunicationObject.GetPersonnels(data);//todo:这里也可以改成异步的
                    allPersonnels = GetPersonnelFromServer();//改成从服务端获取
                }, () =>
                {
                    if (onComplete != null) onComplete();
                }, "");
            }          
        });
    }

    private void AfterGetDepTree(Department topoRoot)
    {
        ThreadManager.Run(()=> 
        {
            personnels = CommunicationObject.GetPersonnels(topoRoot);//todo:这里也可以改成异步的
            allPersonnels = GetPersonnelFromServer();//改成从服务端获取
        },()=> 
        {
            SetCurrentShowPerson();
            RefreshDeppartmentTree();
            Debug.LogErrorFormat("CreateDepartmentTree,costTime:{0}",(DateTime.Now-recordTime).TotalMilliseconds);
        },"");       
    }
    /// <summary>
    /// 移除离线人员
    /// </summary>
    private void SetCurrentShowPerson()
    {
        try
        {
            if (personnels == null) return;
            personnelShowlist.Clear();
            if (isHideOfflinePerson)
            {
                if(LocationManager.Instance==null)
                {
                    personnelShowlist.AddRange(personnels);
                    return;
                }
                if(LocationManager.Instance.LasTagListInfo != null)
                {
                    List<Tag> showTags = LocationManager.Instance.LasTagListInfo.showTags;
                    Dictionary<int, Personnel> perDic = TryGetPersonDic();
                    foreach(var tag in showTags)
                    {
                        if(perDic.ContainsKey(tag.Id))
                        {
                            personnelShowlist.Add(perDic[tag.Id]);
                        }
                    }
                }                
                Debug.LogError("Active pesonCount:" + personnelShowlist.Count);
            }
            else
            {
                personnelShowlist.AddRange(personnels);
            }
        }catch(Exception e)
        {
            personnelShowlist.AddRange(personnels);
            Debug.LogError("Error:DepartmentDivideTree.RemoveOfflinePerson->"+e.ToString());
        }
       
    }
    /// <summary>
    /// 转换成dic，便于搜索
    /// </summary>
    /// <returns></returns>
    private Dictionary<int,Personnel>TryGetPersonDic()
    {
        Dictionary<int, Personnel> dicTemp = new Dictionary<int, Personnel>();
        foreach (var person in personnels)
        {
            if (person.TagId != null && !dicTemp.ContainsKey((int)person.TagId)) dicTemp.Add((int)person.TagId,person);
        }
        return dicTemp;
    }

    bool isRefresh;

    private void RefreshDeppartmentTree()
    {
        TreeNode<TreeViewItem> node = Tree.SelectedNode;
        foreach (var person in personnelShowlist)
        {
            //if (person.Name == "高磊")
            //{
            //    int id = person.Id;
            //}
            if (DepartmentPersonDic.ContainsKey(person.Id))
            {              
                #region backupCode
                //TreeNode<TreeViewItem> personP = DepartmentPersonDic[person.Id];
                //Personnel personNodenew  = (Personnel)personP.Item.Tag;
                //if (personNodenew == null) continue;
                //int num = personNodenew.Id ;
                //Personnel personNode = personnels.Find((item) => item.Id == num);

                //if (personNode == null) continue;

                //if (person.ParentId != personNode.ParentId)
                //{
                //    if (DepartmentDic.ContainsKey((int)person.ParentId))
                //    {
                //        SetMinusParentNodepersonnelNum(personP.Parent);
                //        personP.Parent = DepartmentDic[(int)person.ParentId];
                //        personNode.ParentId = person.ParentId;
                //        SetAddParentNodepersonnelNum(personP.Parent);
                //    }
                //}
                #endregion
                TreeNode<TreeViewItem> personP = DepartmentPersonDic[person.Id];
                if (personP.Item != null && personP.Item.Tag is Personnel)
                {
                    Personnel personOld = (Personnel)personP.Item.Tag;
                    if (personOld.ParentId != person.ParentId)
                    {
                        if (DepartmentDic.ContainsKey((int)person.ParentId))
                        {
                            UpdatePersonnelNumOfText(personP.Parent,-1);
                            personP.Parent = DepartmentDic[(int)person.ParentId];
                            personOld.ParentId = person.ParentId;
                            UpdatePersonnelNumOfText(personP.Parent, 1);
                        }
                    }
                }
            }
            else
            {
                TreeNode<TreeViewItem> newperson = CreatePersonnalNode(person);
               
                if ((DepartmentDic.ContainsKey((int)person.ParentId)))
                {
                    TreeNode<TreeViewItem> ParentPersonnel = DepartmentDic[(int)person.ParentId];
                    ParentPersonnel.Nodes.Add(newperson);
                    newperson.Parent = DepartmentDic[(int)person.ParentId];
                    UpdatePersonnelNumOfText(newperson.Parent,1);
                }
            }
        }
        RomvePersonnelNode(DepartmentPersonDic, personnelShowlist);
       
    }
    /// <summary>
    /// 只刷新在线/不在线人员
    /// </summary>
    public void RefreshActivePerson()
    {
        if (isHideOfflinePerson == false) return;
        HideOffLinePerson();
    }

    /// <summary>
    /// 隐藏离线人员
    /// </summary>
    public void HideOffLinePerson()
    {
        isHideOfflinePerson = true;
        SetCurrentShowPerson();
        RefreshDeppartmentTree();
        //foreach (var person in personnels )
        //{   
        //    TreeNode<TreeViewItem> personShow = DepartmentPersonDic[person.Id];
        //    if(CheckPersonIsPositioning(person) == null )
        //    {
        //        personShow.IsVisible = false;
        //        //SetParentToggleState(personShow,false);
        //    }             
        //    else
        //    {
        //        personShow.IsVisible = true;
        //    }               
        //}
    }

    /// <summary>
    /// 显示所有人
    /// </summary>
    public void ShowAllPerson()
    {
        isHideOfflinePerson = false;
        SetCurrentShowPerson();
        RefreshDeppartmentTree();
        //foreach(var person in personnels)
        //{
        //    TreeNode<TreeViewItem> personShow = DepartmentPersonDic[person.Id];
        //    personShow.IsVisible = true;
        //}
    }
    /// <summary>
    /// 检测人员是否能定位
    /// </summary>
    public LocationObject CheckPersonIsPositioning(Personnel person)
    {
        if (person.Tag != null)
        {
            if (person.Tag.IsActive)
            {
                List<LocationObject> listT = LocationManager.Instance.GetPersonObjects();
                LocationObject locationObjectT = listT.Find((item) => item.personnel.Id == person.Id);
                return locationObjectT;
            }
        }
        return null;
    }

    /// <summary>
    /// 删除电场中消失的人
    /// </summary>
    /// <param name="perDic"></param>
    /// <param name="perList"></param>
    public void RomvePersonnelNode(Dictionary<int, TreeNode<TreeViewItem>> perDic, List<Personnel> perList)
    {
        var romveIds = GetRemovePersonList(perDic, perList);
        foreach (var id in romveIds)
        {
            RemovePersonNode(id);
        }
    }
    /// <summary>
    /// 移除人员节点
    /// </summary>
    /// <param name="personId"></param>
    private void RemovePersonNode(int personId)
    {
        TreeNode<TreeViewItem> personnelP = DepartmentPersonDic[personId];
        personnelP.Parent.Nodes.Remove(personnelP);//删除节点
        DepartmentPersonDic.Remove(personId);//删除缓存

        UpdatePersonnelNumOfText(personnelP.Parent, -1);
    }
    private List<int> GetRemovePersonList(Dictionary<int, TreeNode<TreeViewItem>> perDic, List<Personnel> perList)
    {
        //perList是从数据库获取的当前的人员列表
        //perDic是当前树上的节点
        //List<PersonNode> romveNode = new List<PersonNode>();
        List<int> removeIds = new List<int>();
        foreach (var id in perDic.Keys)
        {
            Personnel perNode = perList.Find(i => i.Id == id);
            if (perNode == null)//树上的人员id在列表中没有，说明该节点应该被删除
            {
                removeIds.Add(id);
            }
        }
        return removeIds;
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
        {
            foreach (Personnel child in topoNode.LeafNodes)
            {                
                var node = CreatePersonnalNode(child);
                treeNode.Nodes.Add(node);
            }
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
            if(nodeNum.Contains("烧结机械"))
            {
                int ii = 999;
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
                if (parentNode.Item.Tag is Department)
                {
                    SetNumber(parentNode, currentNum, num);
                }
            }
            else
            {
                //2.父节点原来没人，直接把子节点的加上
                if (parentNode.Item.Tag is Department)
                {
                    SetNumber(parentNode, currentNum, num);
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
    /// 添加一个人员后，父节点数量增加
    /// </summary>
    /// <param name="perNode"></param>
    public void UpdatePersonnelNumOfText(TreeNode<TreeViewItem> parentNode, int changeNum)
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
                    else if (array.Length == 1)//原本就没有人在那里的
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

    private void SetNumber(TreeNode<TreeViewItem> parentNode, int currentNum, int changeNum)
    {
        if (parentNode.Item.Tag is Department)
        {
            Department anode = parentNode.Item.Tag as Department; //取出区域的名称
            string name = anode.Name.Replace("\n", "");
            var newNum = currentNum + changeNum;
            if (newNum > 0)
            {
                parentNode.Item.Name = string.Format("{0} ({1})", name, newNum);
            }
            else
            {
                parentNode.Item.Name = string.Format("{0}", name);
            }
        }
    }
  
    private TreeNode<TreeViewItem> CreatePersonnalNode(Personnel personnal)
    {
        Sprite icon = Icons[0];//设备图标 todo:以后可以判断是机柜还是设备，机柜则换上机柜图标
        var item = new TreeViewItem(personnal.Name, icon);
        //item.Tag = personnal.Id;//这个导致其他地方取不到personnel
        item.Tag = personnal;
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
        string title = topoNode.Name.Replace("\n","");//去除换行符
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
        if (!(node.Item.Tag is Personnel))
        {
            //LocationManager.Instance.RecoverBeforeFocusAlign();
            return;
        }
        Personnel person = node.Item.Tag as Personnel;
        if (person != null)
        {
            ParkInformationManage.Instance.ShowParkInfoUI(false);
            AlarmPushManage.Instance.CloseAlarmPushWindow( false);
            //Personnel personnelT = personnels.Find((item) => item.Id == tagT);
            LocationManager.Instance.FocusPersonAndShowInfo((int)person.TagId);
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
        ShowFirstLayerNodes(root);
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
       //PersonnelTreeManage.Instance.DepartAndJob_Bg.overrideSprite = Resources.Load("ChangeUI/treeview_border3", typeof(Sprite)) as Sprite; //切换图片

        if (ison)
        {
            //SetTreeRefresh(true);
            DepartmentWindow.SetActive(true);
            SelectedTextChange();
        }
        else
        {
            //SetTreeRefresh(false);
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
