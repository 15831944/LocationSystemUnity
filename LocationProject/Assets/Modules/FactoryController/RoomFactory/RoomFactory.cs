using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.M_Plugins.Helpers.Utils;
using Types = Location.WCFServiceReferences.LocationServices.AreaTypes;
using UnityEngine.AI;
using Unity.Modules.Context;

public enum FactoryTypeEnum
{
    SiHui,
    BaoXin,
    ZiBo
}

public class RoomFactory : MonoBehaviour, IDevManager
{
    public FactoryTypeEnum FactoryType;

    /// <summary>
    /// 分装对List<DevNode>的操作
    /// </summary>
    public class DevNodeList
    {
        public List<DevNode> nodes = new List<DevNode>();
       
        public void Add(DevNode node)
        {
            nodes.Add(node);
        }

        public void Remove(DevNode node)
        {
            nodes.Remove(node);
        }

        public int Count
        {
            get
            {
                return nodes.Count;
            }
        }

        public DevNode Find(string devId)
        {
            return nodes.Find(i => i != null && i.Info.DevID == devId);
        }

        public DevNode Find(int devId)
        {
            return nodes.Find(i => i != null && i.Info.Id == devId);
        }

        private void ClearNull()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node == null)
                {
                    nodes.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public class DepDevDictionary
    {
        public Dictionary<int, DevNodeList> dict = new Dictionary<int, DevNodeList>();

        //public void Add(int id, DevNodeList list)
        //{
        //    dict.Add(id, list);
        //}

        //public bool ContainsKey(int id)
        //{
        //    return dict.ContainsKey(id);
        //}

        //public void TryGetValue(int id,out DevNodeList list)
        //{
        //    dict.TryGetValue(id,out list);
        //}

        //public DevNodeList this[int id]
        //{
        //    get
        //    {
        //        return dict[id];
        //    }
        //    set
        //    {
        //        dict[id] = value;
        //    }
        //}

        public DevNode FindDev(string devId)
        {
            DevNode dev = null;
            foreach (DevNodeList devListTemp in dict.Values)
            {
                dev = devListTemp.Find(devId);
                if (dev != null)
                {
                    return dev;
                }
            }
            return dev;
        }

        public DevNode FindDev(int devId)
        {
            DevNode dev = null;
            foreach (DevNodeList devListTemp in dict.Values)
            {
                dev = devListTemp.Find(devId);
                if (dev != null)
                {
                    return dev;
                }
            }
            return dev;
        }

        public void RemoveDev(string devId)
        {
            foreach (DevNodeList devListTemp in dict.Values)
            {
                DevNode dev = devListTemp.Find(devId);
                if (dev)
                {
                    devListTemp.Remove(dev);
                    break;
                }
            }
        }

        /// <summary>
        /// 获取区域下的所有设备
        /// </summary>
        /// <param name="dep">区域</param>
        /// <param name="containRoomDev">是否包含房间设备（Floor）</param>
        /// <returns></returns>
        public List<DevNode> GetDepDevs(DepNode dep, bool containRoomDev = true)
        {
            List<DevNode> depDevs = new List<DevNode>();
            DevNodeList devListTemp;
            dict.TryGetValue(dep.NodeID, out devListTemp);
            if (devListTemp == null) devListTemp = new DevNodeList();
            if (devListTemp.Count != 0) depDevs.AddRange(devListTemp.nodes);
            //楼层下，包括楼层设备+房间设备
            if (dep is FloorController && containRoomDev)
            {
                foreach (var room in dep.ChildNodes)
                {
                    DevNodeList roomDevs;
                    dict.TryGetValue(room.NodeID, out roomDevs);
                    if (roomDevs != null) depDevs.AddRange(roomDevs.nodes);
                }
            }
            return depDevs;
        }

        public void AddDev(int depId, DevNode dev)
        {
            if (!dict.ContainsKey(depId))
            {
                var devList = new DevNodeList();
                devList.Add(dev);
                dict.Add(depId, devList);
            }
            else
            {
                DevNodeList devNodes;
                dict.TryGetValue(depId, out devNodes);
                if (devNodes != null)
                {
                    devNodes.Add(dev);
                }
                else
                {
                    devNodes = new DevNodeList();
                    devNodes.Add(dev);
                    dict[depId] = devNodes;
                }
            }
        }

        /// <summary>
        /// 通过设备Id获取已经创建的设备
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="parentId"></param>
        /// <returns>返回已经创建的设备</returns>
        public DevNode GetDev(string devId, int depId)
        {
            if (dict.ContainsKey(depId))
            {
                var devList = dict[depId];//缓存已经创建的区域设备
                int? devIdTemp = RoomFactory.TryGetDevId(devId);
                if(devIdTemp==null)
                {
                    var dev = devList.Find(devId);
                    return dev;
                }
                else
                {
                    var dev = devList.Find((int)devIdTemp);
                    return dev;
                }               
            }
            return null;
        }
    }

    public class DepNodeList
    {
        public List<DepNode> nodes = new List<DepNode>();

        public DepNode Find(int nodeId)
        {
            return nodes.FirstOrDefault(i => i != null && i.NodeID == nodeId);
        }

        public DepNode Find(string nameT)
        {
            return nodes.FirstOrDefault(i => i != null && i.NodeName == nameT);
        }

        public DepNode Search(int nodeId)
        {
            DepNode result = null;
            //var nodes = GameObject.FindObjectsOfTypeAll(typeof(DepNode));

            var nodes = Resources.FindObjectsOfTypeAll(typeof(DepNode));
            List<DepNode> errorNodes = new List<DepNode>();
            foreach (DepNode node in nodes)
            {
                if (node == null) continue;
                DepNode nodeBuffer = Find(node.NodeID);
                if (nodeBuffer == null)
                {
                    errorNodes.Add(node);
                }
                if (node.NodeID == nodeId)
                {
                    result = node;
                }
            }
            foreach (var node in errorNodes)
            {
                //Debug.Log("FindNode : "+node.name);
                Add(node);
            }
            return result;
        }

        public DepNode FindByName(string nodeName)
        {
            return nodes.FirstOrDefault(i => i != null && i.NodeName == nodeName);
        }

        /// <summary>
        /// 通过区域ID,删除脚本
        /// </summary>
        /// <param name="physicalTopologyId"></param>
        /// <returns></returns>
        public DepNode Remove(int nodeId)
        {
            DepNode node = Find(nodeId);
            if (node != null)
            {
                nodes.Remove(node);
            }
            return node;
        }

        /// <summary>
        /// 通过区域ID,删除脚本
        /// </summary>
        /// <param name="physicalTopologyId"></param>
        /// <returns></returns>
        public DepNode Replace(DepNode newNode)
        {
            DepNode oldNode = Find(newNode.NodeID);
            if (oldNode != null)
            {
                nodes.Remove(oldNode);
            }
            else
            {
                Debug.LogError("RoomFactory.ReplaceNode oldNode == null :" + newNode.NodeID + "," + newNode);
            }
            nodes.Add(newNode);
            return oldNode;
        }

        public bool Contains(DepNode node)
        {
            return nodes.Contains(node);
        }

        public void Refresh()
        {
            Debug.Log("RoomFactory.RefreshNodes");
            nodes.Clear();
            // var findNodes = GameObject.FindObjectsOfTypeAll(typeof(DepNode));
            var findNodes = Resources.FindObjectsOfTypeAll(typeof(DepNode));
            Debug.Log("nodeCount:" + findNodes.Length);
            List<DepNode> errorNodes = new List<DepNode>();
            foreach (DepNode node in findNodes)
            {
                nodes.Add(node);
            }
        }

        public void Add(DepNode node)
        {
            if (node == null)
            {
                Log.Alarm("RoomFactory.AddDepNode", "node == null");
                return;
            }
            if (!nodes.Contains(node))
            {
                if (node.NodeName == "GIS配电装置楼")
                {
                    int i = 0;
                }
                nodes.Add(node);
            }
            else
            {
                Log.Alarm("RoomFactory.AddDepNode", string.Format("存在相同Key的Node,id={0},name={1}", node.NodeID, node.NodeName));
                //NodeDic[key] = node;
            }
        }

        /// <summary>
        /// 区域信息列表（包含区域、建筑、机房）  区域名称(key)DepNode(value)
        /// 移除
        /// </summary>
        public void Remove(DepNode depNodeT)
        {
            if (nodes.Contains(depNodeT))
            {
                Destroy(depNodeT.NodeObject);
                nodes.Remove(depNodeT);
            }
        }
    }

    public static RoomFactory Instance;
    /// <summary>
    /// 是否正在聚焦区域
    /// </summary>
    public bool IsFocusingDep;
    /// <summary>
    /// 区域信息列表（包含区域、建筑、机房）  区域名称(key)DepNode(value)
    /// </summary>
    public DepNodeList NodeDic = new DepNodeList();
    /// <summary>
    /// 区域下设备列表 区域ID(key) DevNode(value)
    /// </summary>
    private DepDevDictionary DepDevDic = new DepDevDictionary();
    /// <summary>
    /// 静态设备列表
    /// </summary>
    public List<FacilityDevController> StaticDevList = new List<FacilityDevController>();

    private List<DevInfo> staticDevInfos = new List<DevInfo>();//已经加载的静态设备信息
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DevType
    {
        DepDev,
        RoomDev,
        CabinetDev
    }

    // Use this for initialization
    void Start()
    {

        //Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Action onInitComplete=null)
    {
        isTopoInited = false;//开始 
        Debug.Log("Roomfatory.Init,store dep info start...");
        StoreDepInfo();//将DepNode存到NodeDic里面
        Debug.Log("Roomfatory.Init,store dep info end...");
        BindingModelIDByNodeName(()=> //获取AreaTree并关联DepNode
        {
            Debug.Log("Roomfatory.Start save static dev info...");
            SaveStaticDevInfo();
            if (onInitComplete != null)
            {
                Debug.Log("Roomfatory.OnInitComplet!=null,init complete...");
                onInitComplete();
            }
        });
        //SaveStaticDevInfo();
    }

    void Awake()
    {
        AppContext.DevManager = this;
        Instance = this;
        SceneEvents.TopoNodeChanged += SceneEvents_TopoNodeChanged;
    }

    private void SceneEvents_TopoNodeChanged(PhysicalTopology arg1, PhysicalTopology arg2)
    {
        FocusNode(arg2);
    }

    void OnDestroy()
    {
        SceneEvents.TopoNodeChanged -= SceneEvents_TopoNodeChanged;
    }
    [ContextMenu("CreateChildRoom")]
    public void CreateChildRoom()
    {
        StoreDepInfo();

    }
    /// <summary>
    /// 获取当前区域节点下的所有子节点区域Id（建筑节点Id），包括当前节点Id
    /// </summary>
    /// <returns></returns>
    public List<int> GetCurrentDepNodeChildNodeIds(DepNode node)
    {
        List<int> nodeIds = new List<int>();
        if (node == null)
        {
            int i = 0;
        }
        PhysicalTopology topoNode = node.TopoNode;
        if (topoNode == null)
        {
            return nodeIds;
        }
        if (topoNode.Name.Contains("集控楼"))
        {
            int i = 0;
        }
        if (topoNode.Transfrom != null && topoNode.Transfrom.IsOnLocationArea)
        {
            int nodeid = topoNode.Id;
            nodeIds.Add(nodeid);
        }
        if (node.ChildNodes != null)
        {
            foreach (DepNode nodeT in node.ChildNodes)
            {
                //if (nodeT == null) continue;
                List<int> nodeIdsT = GetCurrentDepNodeChildNodeIds(nodeT);
                nodeIds.AddRange(nodeIdsT);
            }
        }
        return nodeIds;

    }
    /// <summary>
    /// 通过区域ID,获取区域管理脚本
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public DepNode GetDepNodeById(int nodeId, bool toFind = false)
    {
        DepNode node = NodeDic.Find(nodeId);
        if (node == null && toFind)
        {
            //缓存中找不到则到全部里面找
            Debug.Log("RoomFactory.GetDepNodeById node == null id:" + nodeId);
            Debug.Log("FindDepNodeById");
            node = NodeDic.Search(nodeId);
            if (node == null)
            {
                Debug.Log("node == null");
            }
            else
            {
                Debug.Log("node:" + node.NodeName);
            }
        }
        return node;
    }

    /// <summary>
    /// 通过区域名称,获取区域管理脚本
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public DepNode GetDepNodeByName(string nameT)
    {
        DepNode node = NodeDic.Find(nameT);
        return node;
    }

    public DepNode GetDepNodeByTopo(PhysicalTopology topoNode, bool toFind = false)
    {
        if (topoNode == null) return null;
        DepNode node = NodeDic.Find(topoNode.Id);
        if (node == null && toFind)
        {
            //缓存中找不到则到全部里面找
            Debug.Log("RoomFactory.GetDepNodeByTopo node == null id:" + topoNode.Id + ",name:" + topoNode.Name);
            Debug.Log("FindDepNodeById");
            node = NodeDic.Search(topoNode.Id);
        }
        return node;
    }

    [ContextMenu("RefreshNodes")]
    public void RefreshNodes()
    {
        NodeDic.Refresh();
    }

    /// <summary>
    /// 通过区域ID,删除脚本
    /// </summary>
    /// <param name="physicalTopologyId"></param>
    /// <returns></returns>
    public DepNode RemoveDepNodeById(int nodeId)
    {
        return NodeDic.Remove(nodeId);
    }

    /// <summary>
    /// 通过区域ID,删除脚本
    /// </summary>
    /// <param name="physicalTopologyId"></param>
    /// <returns></returns>
    public DepNode ReplaceNode(DepNode newNode)
    {
        return NodeDic.Replace(newNode);
    }
    public bool Contains(DepNode node)
    {
        return NodeDic.Contains(node);
    }

    /// <summary>
    /// 根据名称，找到对应区域
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public DepNode GetDepNode(string key)
    {
        DepNode node = NodeDic.FindByName(key);
        if (node == null)
        {
            Debug.LogError("RoomFactory.GetDepNode node == null:" + key);
        }
        return node;
    }
    /// <summary>
    /// 设置区域聚焦状态（是否正在聚焦过程）
    /// </summary>
    public void SetDepFoucusingState(bool value)
    {
        IsFocusingDep = value;
    }
    public void AddDepNode(DepNode node)
    {
        NodeDic.Add(node);
    }

    /// <summary>
    /// 添加静态设备信息
    /// </summary>
    /// <param name="devController"></param>
    public void SaveStaticDevInfo(FacilityDevController devController)
    {
        //保存新加载的静态设备，完善设备信息
        if(devController.Info==null)
        {
            DevInfo info = staticDevInfos.Find(i=>i!=null&&i.ModelName==devController.gameObject.name);
            if(info!=null)
            {
                DepNode parentNode = GetDepNodeById((int)info.ParentId);
                if(parentNode!=null)
                {
                    SaveDepDevInfo(parentNode, devController, info);
                    devController.CreateFollowUI();
                }                
            }          
        }
        if (!StaticDevList.Contains(devController))
        {
            StaticDevList.Add(devController);
        }
    }
    /// <summary>
    /// 保存静态设备信息
    /// </summary>
    /// <param name="staticDev"></param>
    private void SaveStaticDevInfo()
    {
        FactoryDepManager manager = FactoryDepManager.Instance;
        if (manager)
        {
            FacilityDevController[] staticDevs = manager.transform.GetComponentsInChildren<FacilityDevController>(true);
            StaticDevList.AddRange(staticDevs);
        }
    }
    #region 建筑ID初始化
    /// <summary>
    /// 保存所有区域信息
    /// </summary>
    private void StoreDepInfo()
    {
        try
        {
            FactoryDepManager depManager = FactoryDepManager.Instance;
            if (depManager)
            {
                AddDepNode(depManager);
                depManager.AllNodes = depManager.transform.GetComponentsInChildren<DepNode>(false).ToList();
                foreach (DepNode item in depManager.ChildNodes)
                {
                    item.RefreshChildrenNodes();//刷新子节点，避免手动设置和模型替换是节点丢失
                    AddDepNode(item);
                    if (item.HaveChildren())
                    {
                        StoreChildInfo(item);
                    }
                }
            }
        }catch(Exception e)
        {
            Debug.LogError("Error:RoomFactory.StoreDepInfo->"+e.ToString());
        }
        
    }
    /// <summary>
    /// 保存所有子区域信息
    /// </summary>
    /// <param name="node"></param>
    private void StoreChildInfo(DepNode node)
    {
        foreach (var child in node.ChildNodes)
        {
            if (child == null)
            {
                //if(node!=null)
                //{
                //    Debug.LogError(string.Format("{0} child is null..", node.NodeName));
                //}
                continue;
            }
            AddDepNode(child);
            if (child.HaveChildren())
                StoreChildInfo(child);
        }
    }
    /// <summary>
    /// 绑定建筑ID
    /// </summary>
    public void BindingModelIDByNodeName(Action callback)
    {
        Debug.Log("RoomFactory->BindingModelIDByNodeName");
        //PhysicalTopology topoRoot = CommunicationObject.Instance.GetTopoTree();
        //StartBindingTopolgy(topoRoot);
        CommunicationObject.Instance.GetTopoTree((topoRoot) =>
        {
            Debug.Log("RoomFactory->GetTopoTree success,start binding...");
            StartBindingTopolgy(topoRoot);
            Debug.Log("RoomFactory->Building bind topoTree complete...");
            if (callback != null)
            {
                callback();
            }
            isTopoInited = true;
        }
        );
    }

    public bool isTopoInited = false;

    private void StartBindingTopolgy(PhysicalTopology toplogy)
    {
        try
        {
            Log.Info("RoomFactory->StartBindingTopolgy Start !!!!!!!!!!!!!!!11");
            if (toplogy == null
                //|| toplogy.Children == null || toplogy.Children.Length == 0  
                )
            {
                Debug.LogError("RoomFactory->PhysicalTopology is null!");
                return;
            }
            Log.Info("RoomFactory->NodeName:" + toplogy.Name);
            var factoryTopo = toplogy;//这里传进来就是园区节点（“四会热电厂”)
                                      //var topologies = toplogy.Children.ToList();
                                      //foreach (var factoryTopo in topologies)
            {
                //if (factoryTopo.Name == "四会热电厂" || factoryTopo.Name == "高新软件园")
                {
                    var rangesT = new List<PhysicalTopology>();
                    if (factoryTopo.Children != null)
                    {
                        var factoryTopologies = factoryTopo.Children.ToList();
                        foreach (var topoNode in factoryTopologies)
                        {
                            var node = GetDepNode(topoNode.Name);
                            if (node != null)
                            {
                                node.SetTopoNode(topoNode);
                                if (topoNode.Children != null)
                                {
                                    BindingChild(node, topoNode.Children.ToList());
                                }
                            }
                            else
                            {
                                if (topoNode.Type == Types.范围)
                                {
                                    rangesT.Add(topoNode);
                                }
                                else
                                {
                                    Log.Alarm("RoomFactory->StartBindingTopolgy", "未找到DepNode:" + topoNode.Name);
                                }
                            }
                        }
                    }
                    
                    DepNode toplogyNode = GetDepNode(factoryTopo.Name);
                    if (toplogyNode != null)
                    {
                        toplogyNode.SetTopoNode(factoryTopo);
                        AddRanges(toplogyNode, rangesT);
                    }
                }
            }
            SetParkNode(factoryTopo);
            Log.Info("RoomFactory->StartBindingTopolgy End !!!!!!!!!!!!!!!11");
        }catch(Exception e)
        {
            Debug.LogError("Error:RoomFactory->StartBindingTopolgy:"+e.ToString());
        }
    }

    private void SetParkNode(PhysicalTopology toplogy)
    {
        var parks = GameObject.FindObjectsOfType<FactoryDepManager>().ToList();
        //var topologies = toplogy.Children.ToList();
        //foreach (var factoryTopo in topologies)
        //{
        //    FactoryDepManager park = parks.Find(i => i.NodeName == factoryTopo.Name);
        //    if (park != null)
        //    {
        //        park.SetTopoNode(factoryTopo);
        //    }
        //    else
        //    {
        //        Log.Alarm("StartBindingTopolgy", "未找到园区:" + factoryTopo.Name);
        //    }
        //}
        {
            FactoryDepManager park = parks.Find(i => i.NodeName == toplogy.Name);
            if (park != null)
            {
                park.SetTopoNode(toplogy);
            }
            else
            {
                Log.Alarm("StartBindingTopolgy", "未找到园区:" + toplogy.Name);
            }
        }
    }

    public void SetTopoNode(DepNode depNode, PhysicalTopology topoNode, bool isReplaceNode)
    {
        if (depNode == null)
        {
            Debug.LogError("RoomFactory.SetTopoNode depNode==null : " + topoNode);
            return;
        }
        depNode.SetTopoNode(topoNode);
        if (isReplaceNode)
        {
            ReplaceNode(depNode);
        }
    }

    public void BindingChild(DepNode node, List<PhysicalTopology> topologies, bool isReplaceNode = false)
    {
        if (node == null)
        {
            Debug.LogError("RoomFactory.BindingChild node == null");
            return;
        }
        List<PhysicalTopology> rangesT = new List<PhysicalTopology>();
        if(node.ChildNodes!=null)
            foreach (var item in node.ChildNodes)
            {
                if (item == null) continue;
                try
                {
                    if (!string.IsNullOrEmpty(item.NodeName))
                    {
                        PhysicalTopology topology = topologies.Find(topo => topo.Name == item.NodeName);
                        if (topology != null)
                        {
                            //item.SetTopoNode(topology);
                            SetTopoNode(item, topology, isReplaceNode);
                            if (topology.Children == null || topology.Children.Length == 0) continue;
                            if (item as FloorController)
                            {
                                FloorController floor = item as FloorController;
                                AddRoomInFloor(floor, topology.Children.ToList(), isReplaceNode);
                            }
                            BindingChild(item, topology.Children.ToList());
                        }
                        else
                        {
                            Log.Alarm("BindingChild", "未找到Topo节点:" + item.NodeName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("RoomFactory.BindingChild:"+ item.NodeName +"|"+ ex);
                }
            }
        AddRanges(node, topologies);
    }

    /// <summary>
    /// 在厂区中添加范围
    /// </summary>
    /// <param name="depNodeT"></param>
    public void AddRanges(DepNode depNodeT, List<PhysicalTopology> roomTopo)
    {
        Log.Error("AddRanges", "depNodeT:" + depNodeT.NodeName);
        foreach (var topo in roomTopo)
        {
            AddRange(depNodeT, topo);
        }
    }

    public RangeController AddRange(DepNode depNodeT, PhysicalTopology topo)
    {
        if (depNodeT == null) return null;
        if (topo.Type != Types.范围) return null;
        PhysicalTopology topoNode = depNodeT.TopoNode;

        if (topoNode == null)
        {
            Debug.Log("TopoNode is null...");
            return null;
        }

        Transform rangesTran = depNodeT.transform.Find("Ranges");
        if (rangesTran == null)
        {
            rangesTran = CreateGameObject("Ranges", depNodeT.transform).transform;
        }
        RangeController rangeController = FindRangeController(rangesTran, topo);//已经创建了
        if (rangeController != null) return null;//已经创建了
        if (GetDepNodeByTopo(topo) != null) return null;
        rangeController = CreateRangeController(topo, rangesTran);
        rangeController.ParentNode = depNodeT;
        if (depNodeT.ChildNodes == null)
        {
            depNodeT.ChildNodes = new List<DepNode>();
        }
        depNodeT.ChildNodes.Add(rangeController);
        //if (topo.Name == "集控楼4.5m层测试范围1")
        //{
        //    int i = 0;
        //}
        NodeDic.Add(rangeController);
        return rangeController;
    }

    private RangeController FindRangeController(Transform rangeRoot, PhysicalTopology topo)
    {
        var rangeControllers = rangeRoot.FindComponentsInChildren<RangeController>();
        foreach (var item in rangeControllers)
        {
            if (item.NodeID == topo.Id)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 区域信息列表（包含区域、建筑、机房）  区域名称(key)DepNode(value)
    /// 移除
    /// </summary>
    public void NodeDic_Remove(DepNode depNodeT)
    {
        NodeDic.Remove(depNodeT);
    }

    /// <summary>
    /// 在楼层中添加机房
    /// </summary>
    /// <param name="floor"></param>
    private void AddRoomInFloor(FloorController floor, List<PhysicalTopology> roomTopo, bool isReplaceNode)
    {
        PhysicalTopology topoNode = floor.TopoNode;
        if (floor == null || floor.ParentNode == null) return;
        PhysicalTopology buildingNode = floor.ParentNode.TopoNode;
        if (topoNode == null || buildingNode == null)
        {
            Debug.Log("TopoNode is null...");
            return;
        }
        Transform roomsRootObj = floor.transform.FindChildByName("Rooms");
        if (roomsRootObj == null)//判断一下 避免重复创建
        {
            roomsRootObj = CreateGameObject("Rooms", floor.transform).transform;
            foreach (var topo in roomTopo)
            {
                var node = GetDepNodeByTopo(topo);
                if (node != null)
                {
                    if (isReplaceNode)
                    {
                        node = RemoveDepNodeById(topo.Id);
                    }
                    else
                    {
                        Debug.LogWarning("AddRoomInFloor. node != null");
                        continue;
                    }
                }
                if (topo.Type == Types.范围) continue;
                AddRoomInFloorOP(floor, roomsRootObj, topo);
            }
        }
        else
        {
            if (isReplaceNode)//已经有了的情况下，也要替换回去
            {
                foreach (DepNode item in floor.ChildNodes)
                {
                    ReplaceNode(item);
                }
            }
        }
    }


    private void AddRoomInFloorOP(FloorController floor, Transform rooms, PhysicalTopology topo)
    {
        var roomController = CreateRoomRoomController(topo, rooms);
        roomController.ParentNode = floor;
        floor.ChildNodes.Add(roomController);
        NodeDic.Add(roomController);
    }

    private static GameObject CreateGameObject(string objName, Transform parent)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.parent = parent;
        obj.transform.localEulerAngles = Vector3.zero;
        //obj.transform.position = Vector3.zero;
        obj.transform.position = parent.parent.transform.position;//设置Ranges的父物体的位置，为默认位置
        obj.transform.localScale = Vector3.one;
        return obj;
    }

    private static RoomController CreateRoomRoomController(PhysicalTopology topo, Transform parent)
    {
        GameObject obj = CreateGameObject(topo.Name, parent);
        RoomController roomController = obj.AddComponent<RoomController>();
        roomController.SetTopoNode(topo);
        roomController.NodeObject = obj;
        roomController.angleFocus = new Vector2(60, 0);
        roomController.camDistance = 15;
        roomController.angleRange = new Mogoson.CameraExtension.Range(5, 90);
        roomController.disRange = new Mogoson.CameraExtension.Range(2, 15);
        roomController.AreaSize = new Vector2(5, 5);
        return roomController;
    }

    private static RangeController CreateRangeController(PhysicalTopology topo, Transform parent)
    {
        GameObject obj = CreateGameObject(topo.Name, parent);
        RangeController rangeController = obj.AddComponent<RangeController>();
        rangeController.SetTopoNode(topo);
        rangeController.NodeObject = obj;
        rangeController.angleFocus = new Vector2(60, 0);
        //是否根据厂区/楼层内，定不同的参数?
        rangeController.camDistance = 20;
        rangeController.angleRange = new Mogoson.CameraExtension.Range(5, 90);
        rangeController.disRange = new Mogoson.CameraExtension.Range(2, 25);
        rangeController.AreaSize = new Vector2(5, 5);
        return rangeController;
    }

    private static T CreateDepNode<T>(PhysicalTopology topo, GameObject parent) where T : DepNode
    {
        GameObject rangeT = CreateGameObject(topo.Name, parent.transform);
        T rangeController = rangeT.AddComponent<T>();
        rangeController.SetTopoNode(topo);
        rangeController.NodeObject = rangeT;
        //rangeController.angleFocus = new Vector2(60, 0);
        //rangeController.camDistance = 15;
        //rangeController.angleRange = new Mogoson.CameraExtension.Range(5, 90);
        //rangeController.disRange = new Mogoson.CameraExtension.Range(2, 15);
        //rangeController.AreaSize = new Vector2(5, 5);
        return rangeController;
    }

    /// <summary>
    /// 获取设备存放处的缩放值
    /// </summary>
    /// <param name="ParentLossyScale"></param>
    /// <returns></returns>
    private Vector3 GetContainerScale(Vector3 ParentLossyScale)
    {
        float x = ParentLossyScale.x;
        float y = ParentLossyScale.y;
        float z = ParentLossyScale.z;
        if (x != 0) x = 1 / x;
        if (y != 0) y = 1 / y;
        if (z != 0) z = 1 / z;
        return new Vector3(x, y, z);
    }
    #endregion
    #region 设备拓扑树点击响应部分
    public void FocusNode(PhysicalTopology topoNode)
    {
        if (topoNode == null)
        {
            Log.Alarm("FocusNode", "topoNode == null");
            return;
        }
        Log.Info("FocusNode:" + topoNode.Name);
        DepNode node = GetDepNodeByTopo(topoNode);
        if (node != null)
        {
            FocusNode(node);
        }
        else
        {
            Log.Alarm("未找到节点区域");
        }
    }

    private BuildingBox GetBuildingBox(DepNode node)
    {
        if (node == null)
        {
            Debug.LogError("RoomFactory.GetBuildingBox node == null");
            return null;
        }
        BuildingController building = node.GetParentNode<BuildingController>();
        if (building != null)
        {
            BuildingBox box = building.gameObject.GetComponent<BuildingBox>();
            return box;
        }
        return null;
    }


    public void FocusNode(DepNode node, Action onDevCreateFinish = null, bool isSetSelectNode = true)
    {

        try
        {
            if (node == null || node.TopoNode == null)
            {
                Debug.LogError("RoomFactory.FocusNode node == null");
                if (onDevCreateFinish != null)
                {
                    onDevCreateFinish();//2019_05_14_cww:继续进行下去
                }
                return;
            }

            Log.Info("RoomFactory.FocusNode", string.Format("nodeId:{0},nodeName:{1}", node.NodeID, node.NodeName));

            BuildingBox box = GetBuildingBox(node);
            if (box)
            {
                Log.Info("RoomFactory.FocusNode", string.Format("box.LoadBuilding AssetName:{0},SceneName:{1}", box.AssetName, box.SceneName));
                box.LoadBuilding((nNode) =>
                {
                    FocusNode(nNode, onDevCreateFinish, isSetSelectNode);//加载完建筑模型后继续原来的对焦工作
                }, true, node);
                return;
            }

            //if (node.TopoNode != null && node.TopoNode.Type == AreaTypes.范围) return;
            if (FactoryDepManager.currentDep == node && IsFocusingDep)
            {
                //处理拓扑树,快速单击两次的问题
                Debug.Log(string.Format("{0} is Focusing...", node.NodeName));
                return;
            }

            //if (LocationManager.Instance)
            //{
            //    LocationManager.Instance.HideCurrentPersonInfoUI();
            //}

            bool isFocusBreak = false;
            if (IsFocusingDep) isFocusBreak = true;
            IsFocusingDep = true;
            if (DevNode.CurrentFocusDev != null) DevNode.CurrentFocusDev.FocusOff(false);
            Log.Info(string.Format("FocusNode ID:{0},Name:{1},Type:{2}", node.NodeID, node.NodeName, node.GetType()));
            DepNode lastNodep = FactoryDepManager.currentDep;
            SceneEvents.OnDepNodeChangeStart(lastNodep, node);
            if (FactoryDepManager.currentDep == node)
            {
                node.FocusOn(() =>
                {
                    IsFocusingDep = false;
                    if (onDevCreateFinish != null) onDevCreateFinish();
                });
                //IsFocusingDep = false;
                //if (onDevCreateFinish != null) onDevCreateFinish();
                if (isFocusBreak) IsFocusingDep = true;
            }
            else
            {
                //FactoryDepManager manager = FactoryDepManager.Instance;
                DeselctLast(FactoryDepManager.currentDep, node);
                node.OpenDep(() =>
                {
                    IsFocusingDep = false;
                    if (onDevCreateFinish != null) onDevCreateFinish();
                    SceneEvents.OnDepCreateCompleted(node);
                });
                if (isFocusBreak) IsFocusingDep = true;
            }

            if (isSetSelectNode)
            {
                if (TopoTreeManager.Instance) TopoTreeManager.Instance.SetSelectNode(lastNodep, node);
                if (PersonnelTreeManage.Instance) PersonnelTreeManage.Instance.areaDivideTree.Tree.AreaSelectNodeByType(node.NodeID);
                //if (node != FactoryDepManager.Instance) PersonnelTreeManage.Instance.areaDivideTree.Tree.AreaSelectNodeByType(node.NodeID);
            }

        }
        catch (Exception ex)
        {
            Log.Error("RoomFactory.FocusNode",ex.ToString());
        }
        
    }

    /// <summary>
    /// 聚焦区域节点，这里是用在聚焦人员时，切换到人员所在的区域节点时用的
    /// </summary>
    /// <param name="node"></param>
    /// <param name="onDevCreateFinish"></param>
    public void FocusNodeForFocusPerson(DepNode node, Action onDevCreateFinish = null, bool isSetSelectNode = true)
    {
        if (node == null)
        {
            Debug.LogError("RoomFactory.FocusNodeForFocusPerson node == null");
            if (onDevCreateFinish != null)
            {
                onDevCreateFinish();
            }
            return;
        }
        BuildingBox box = GetBuildingBox(node);
        if (box)
        {
            box.LoadBuilding((nNode) =>
            {
                FocusNodeForFocusPerson(nNode, onDevCreateFinish, isSetSelectNode);//加载完建筑模型后继续原来的对焦工作
            }, true, node);
            return;
        }

        ////if (node.TopoNode != null && node.TopoNode.Type == AreaTypes.范围) return;
        //if (FactoryDepManager.currentDep == node && IsFocusingDep)
        //{
        //    //处理拓扑树,快速单击两次的问题
        //    Debug.Log(string.Format("{0} is Focusing...", node.NodeName));
        //    return;
        //}
        bool isFocusBreak = false;
        if (IsFocusingDep) isFocusBreak = true;
        IsFocusingDep = true;
        if (DevNode.CurrentFocusDev != null) DevNode.CurrentFocusDev.FocusOff(false);
        Log.Info(string.Format("FocusNode ID:{0},Name:{1},Type:{2}", node.NodeID, node.NodeName, node.GetType()));
        DepNode lastNodep = FactoryDepManager.currentDep;
        SceneEvents.OnDepNodeChangeStart(lastNodep, node);
        if (FactoryDepManager.currentDep == node)
        {
            //node.FocusOn(() =>
            //{
            //    IsFocusingDep = false;
            //    if (onDevCreateFinish != null) onDevCreateFinish();
            //});
            IsFocusingDep = false;
            if (onDevCreateFinish != null) onDevCreateFinish();
            if (isFocusBreak) IsFocusingDep = true;
        }
        else
        {
            //FactoryDepManager manager = FactoryDepManager.Instance;
            DeselctLast(FactoryDepManager.currentDep, node);
            node.OpenDep(() =>
            {
                IsFocusingDep = false;
                if (onDevCreateFinish != null) onDevCreateFinish();
                SceneEvents.OnDepCreateCompleted(node);
            }, false);
            if (isFocusBreak) IsFocusingDep = true;
        }

        //if (isSetSelectNode)
        //{
        //    if (TopoTreeManager.Instance) TopoTreeManager.Instance.SetSelectNode(lastNodep, node);
        //}
    }

    /// <summary>
    /// 取消上一个区域的选中,无视角转换
    /// </summary>
    /// <param name="lastNode"></param>
    /// <param name="currentNode"></param>
    public void DeselctLast(DepNode lastNode, DepNode currentNode)
    {
        if (lastNode == null) return;
        HighlightManage highlight = HighlightManage.Instance;
        if (highlight)
        {
            highlight.CancelHighLight();//取消当前区域,设备的高亮
        }
        lastNode.IsFocus = false;
        if (lastNode.NodeID != currentNode.NodeID)
        {
            lastNode.HideDep();
        }
    }

    /// <summary>
    /// 无动画切换区域
    /// </summary>
    public void ChangeDepNodeNoTween()
    {

        //FactoryDepManager.Instance.ShowOtherBuilding();
        DepNode lastDep = FactoryDepManager.currentDep;
        //lastDep.IsFocus = false;
        FactoryDepManager.currentDep = FactoryDepManager.Instance;
        RoomFactory.Instance.DeselctLast(lastDep, FactoryDepManager.Instance);
        SceneEvents.OnDepNodeChanged(lastDep, FactoryDepManager.Instance);
        FactoryDepManager.Instance.ShowOtherBuilding();
    }

    #endregion
    #region 创建设备部分

    //private Dictionary<int?, List<DevInfo>> DevCreateDic = new Dictionary<int?, List<DevInfo>>();
    /// <summary>
    /// 创建前，存储区域下所有设备
    /// </summary>
    private Dictionary<DepNode, List<DevInfo>> DepDevCreateDic = new Dictionary<DepNode, List<DevInfo>>();
    /// <summary>
    /// 区域下所有门禁信息
    /// </summary>
    private List<Dev_DoorAccess> DoorAccessList = new List<Dev_DoorAccess>();
    /// <summary>
    /// 设备创建完成回调
    /// </summary>
    private Action OnDevCreateAction;
    /// <summary>
    /// 当前创建设备的建筑（服务端获取数据的过程，切换区域）
    /// </summary>
    private DepNode currentFocusDep;
    private DateTime devStartTime;
    private DateTime recordTime;
    public void CreateDepDev(DepNode dep, Action onComplete = null)
    {
        CreateDepDev(dep, false, onComplete);
    }
    /// <summary>
    /// 创建设备
    /// </summary>
    /// <param name="dep">区域</param>
    /// <param name="isRoam">是否漫游模式</param>
    /// <param name="onComplete">设备创建完回调</param>
    public void CreateDepDev(DepNode dep, bool isRoam, Action onComplete = null)
    {
        Debug.Log("RoomFactory.CreateDepDev dep=" + dep);
        if (currentFocusDep == dep) return;
        currentFocusDep = dep;
        ResultText = "";
        OnDevCreateAction = onComplete;

        //Debug.LogError(string.Format("StartCreateDev {0}",dep));
        //ThreadManager.Run(() =>
        //{
        //    GetDevs(dep);
        //}, () =>
        //{
        //    CreateDevs(isRoam);
        //}, "LoadDevInfo...");

        GetDevs(dep);
        CreateDevs(isRoam);
    }

    private void GetDevs(DepNode dep)
    {
        recordTime = DateTime.Now;
        List<DepNode> depList = new List<DepNode>();
        //CommunicationObject service = CommunicationObject.Instance;
        DevCount = 0;
        depList.Add(dep);
        if (dep is BuildingController || dep is FloorController)
        {
            List<DepNode> childList = GetChildNodes(dep);
            if (childList != null && childList.Count != 0) depList.AddRange(childList);
        }
        GetDevInfo(depList);//从服务端获取设备
        ResultText += string.Format("GetDevs cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
    }

    private void CreateDevs(bool isRoam)
    {
        DepNode factoryDep = FactoryDepManager.currentDep;
        bool isRoomState = factoryDep is RoomController && factoryDep.ParentNode == currentFocusDep;
        if (isRoam || isRoomState || currentFocusDep == factoryDep)
        {
            //recordTime = DateTime.Now;
            CreateDepDev();//里面是携程
            //ResultText += string.Format("CreateDepDev cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
            //Debug.Log(ResultText);
        }
    }

    private string ResultText = "";
    /// <summary>
    /// 获取建筑下所有楼层
    /// </summary>
    /// <param name="building"></param>
    /// <returns></returns>
    private List<DepNode> GetChildNodes(DepNode building)
    {
        if (building == null || building.ChildNodes == null)
        {
            return null;
        }
        List<DepNode> depTempList = new List<DepNode>();
        foreach (DepNode child in building.ChildNodes)
        {
            if (child.IsDevCreate) continue;
            child.IsDevCreate = true;
            depTempList.Add(child);
            List<DepNode> childList = GetChildNodes(child);
            if (childList != null && childList.Count != 0) depTempList.AddRange(childList);
        }
        return depTempList;
    }
    /// <summary>
    /// 保存获取的设备信息
    /// </summary>
    /// <param name="parentID"></param>
    private void GetDevInfo(List<DepNode> deps)
    {
        try
        {
            CommunicationObject service = CommunicationObject.Instance;
            if (service)
            {
                RecordTime = DateTime.Now;
                List<int> pidList = GetPidList(deps);
                List<DevInfo> devInfos = RemoveRepeateDev(service.GetDevInfoByParent(pidList));
                int count = devInfos == null ? 0 : devInfos.Count;
                ResultText += string.Format("Get dep info, length:{0} cost :{1}ms\n", count, (DateTime.Now - RecordTime).TotalMilliseconds);
                recordTime = DateTime.Now;
                SaveDepDevInfoInCreating(deps, devInfos);
                GetDoorAccessInfo(pidList);
                ResultText += string.Format("Get DoorAccessInfo cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
            }
            else
            {
                Debug.LogError("RoomFactory.GetDevInfo CommunicationObject.Instance==null");
            }
        }catch(Exception e)
        {
            Log.Error("RoomFactory.GetDevInfo.Exception:"+e.ToString());
        }
    }
    /// <summary>
    /// 移除重复数据
    /// </summary>
    /// <param name="devListTemp"></param>
    /// <returns></returns>
    private List<DevInfo>RemoveRepeateDev(List<DevInfo> devListTemp)
    {
        int repeatDevCount = 0;
        Dictionary<string, DevInfo> devDicNoRepeat = new Dictionary<string, DevInfo>();
        foreach(var item in devListTemp)
        {
            if (!devDicNoRepeat.ContainsKey(item.DevID)) devDicNoRepeat.Add(item.DevID,item);
            else
            {
                if (devDicNoRepeat[item.DevID].Id > item.Id) devDicNoRepeat[item.DevID] = item;
                repeatDevCount++;
            }
        }
        Debug.LogError("RemoveRepeatDev,RepeatDevCount:"+repeatDevCount);
        return devDicNoRepeat.Values.ToList();
    }
    private void GetDoorAccessInfo(List<int> pidList)
    {
        CommunicationObject service = CommunicationObject.Instance;
        if (service)
        {
            List<Dev_DoorAccess> doorAccesses = service.GetDoorAccessInfoByParent(pidList);
            SaveDoorAccessInfo(doorAccesses);
        }
    }
    /// <summary>
    /// 获取Pid(设备所属区域)列表
    /// </summary>
    /// <param name="deps"></param>
    /// <returns></returns>
    private List<int> GetPidList(List<DepNode> deps)
    {
        List<int> pidList = new List<int>();
        foreach (var dep in deps)
        {
            if (!pidList.Contains(dep.NodeID)) pidList.Add(dep.NodeID);
        }
        return pidList;
    }
    /// <summary>
    /// 保存区域下门禁信息
    /// </summary>
    /// <param name="doorAccess"></param>
    private void SaveDoorAccessInfo(List<Dev_DoorAccess> doorAccess)
    {
        DoorAccessList.Clear();
        if (doorAccess != null && doorAccess.Count != 0)
        {
            DoorAccessList.AddRange(doorAccess);
        }
    }
    /// <summary>
    /// 保存区域下设备信息
    /// </summary>
    /// <param name="dep"></param>
    /// <param name="devInfos"></param>
    private void SaveDepDevInfoInCreating(List<DepNode> depList, List<DevInfo> devInfos)
    {
        DepDevCreateDic.Clear();
        if (devInfos != null && devInfos.Count != 0)
        {
            foreach (var dep in depList)
            {
                List<DevInfo> devs = devInfos.FindAll(i => i.ParentId == dep.NodeID);
                if (devs != null && devs.Count != 0)
                {
                    DepDevCreateDic.Add(dep, devs);
                    DevCount += devs.Count;
                }
            }

        }
    }
    /// <summary>
    /// 创建区域下设备
    /// </summary>
    private void CreateDepDev()
    {
        if (DepDevCreateDic != null && DepDevCreateDic.Count != 0)
        {
            devStartTime = DateTime.Now;//开始创建设备
            CurrentCreateIndex = 0;
            foreach (var item in DepDevCreateDic.Keys)
            {
                int id = item.NodeID;
                DevType devType = DevType.DepDev;
                GameObject devContainer = GetDepDevContainer(item, ref devType);
                //StartCoroutine(LoadDevsCorutine(item, devContainer, devType));//开始携程,一个区域开始一个携程？，递归方式
                StartCoroutine(LoadDevsCorutine2(item, devContainer, devType));//开始携程,一个区域开始一个携程？，循环方式
            }
        }
        else
        {
            Log.Error("RoomFactory.CreateDpeDev 区域下没有设备");
            if (OnDevCreateAction != null)
            {
                OnDevCreateAction();
            }
        }
    }
    /// <summary>
    /// 获取存放设备的物体
    /// </summary>
    /// <param name="depNode"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetDepDevContainer(DepNode depNode, ref DevType type)
    {
        if (depNode as FloorController)
        {
            FloorController floor = depNode as FloorController;
            type = DevType.RoomDev;
            return floor.RoomDevContainer;
        }
        else if (depNode as RoomController)
        {
            RoomController room = depNode as RoomController;
            type = DevType.RoomDev;
            return room.RoomDevContainer;
        }
        else
        {
            type = DevType.DepDev;
            return FactoryDepManager.Instance.FactoryDevContainer;
        }
    }
    /// <summary>
    /// 设备数量
    /// </summary>
    private float DevCount;
    /// <summary>
    /// 当前创建设备下标
    /// </summary>
    private float CurrentCreateIndex;

    private DateTime RecordTime;
    //private WaitForSeconds waitTime = new WaitForSeconds(0.1f);
    IEnumerator LoadDevsCorutine(DepNode dep, GameObject container, DevType type)
    {
        //yield return null;
        if (SystemSettingHelper.deviceSetting.NotLoadAllDev == false)
        {
            List<DevInfo> devList;
            DepDevCreateDic.TryGetValue(dep, out devList);
            if (devList != null && devList.Count != 0)
            {
                //float percent =1- devList.Count / DevCount;  
                CurrentCreateIndex++;
                float percent = CurrentCreateIndex / DevCount;
                //Debug.Log(string.Format("当前创建设备Index:{0}  总设备数:{1}  Percent:{2}",CurrentCreateIndex,DevCount,percent));
                ProgressbarLoad.Instance.Show(percent);
                DevInfo dev = devList[devList.Count - 1];
                //Debug.Log(string.Format("创建设备:{0} 剩余设备:{1}",dev.Name,devList.Count));
                devList.Remove(dev);
                //StartCoroutine(LoadSingleDevCorutine(dev, container, type, dep, obj =>
                //{
                //    StartCoroutine(LoadDevsCorutine(dep, container, type));
                //}));
                //yield return LoadSingleDevCorutine(dev, container, type, dep, obj =>
                //{
                //    StartCoroutine(LoadDevsCorutine(dep, container, type));
                //});
                yield return LoadSingleDevCorutine(dev, container, type, dep, null);//创建设备
                yield return LoadDevsCorutine(dep, container, type);//递推调用，继续区域内下一个设备。
            }
            else
            {
                DepDevCreateDic.Remove(dep);
                if (DepDevCreateDic.Count == 0)
                {
                    if (OnDevCreateAction != null) OnDevCreateAction();
                    ProgressbarLoad.Instance.Hide();
                    // SystemModeTweenManage.Instance.StartTweener();

                    ResultText += string.Format("CreateDepDev All cost:{0}ms count:{1} \n", (DateTime.Now - devStartTime).TotalMilliseconds, DevCount);
                    Debug.Log(ResultText);//结束创建
                }
            }
        }
    }

    IEnumerator LoadDevsCorutine2(DepNode dep, GameObject container, DevType type)
    {
        if (SystemSettingHelper.deviceSetting.NotLoadAllDev == false)
        {
            List<DevInfo> devList;
            DepDevCreateDic.TryGetValue(dep, out devList);
            if (devList != null && devList.Count != 0)
            {
                //改动方式1.整理成循环方式，逻辑上更加清晰，效果和性能不变
                //for (int i = 0; i < devList.Count; i++)
                //{
                //    float percent = (i + 1) / DevCount;
                //    ProgressbarLoad.Instance.Show(percent);
                //    var dev = devList[i];
                //    yield return LoadSingleDevCorutine(dev, container, type, dep, null);//创建一个设备
                //}

                //改动方式2.先加载模型，在创建设备。模型加载进度成为进度条进度，设备创建基本不耗时。
                recordTime = DateTime.Now;
                List<string> models = new List<string>();
                for (int i = 0; i < devList.Count; i++)//循环创建设备
                {
                    var dev = devList[i];
                    if (string.IsNullOrEmpty(dev.ModelName)) continue;
                    if (!models.Contains(dev.ModelName) && !TypeCodeHelper.IsStaticDev(dev.TypeCode.ToString()))
                    {
                        models.Add(dev.ModelName);
                    }
                }
                //for (int i = 0; i < models.Count; i++)
                //{
                //    float percent = (i + 1) / models.Count;
                //    ProgressbarLoad.Instance.Show(percent);//模型进度
                //    string model = models[i];
                //    GameObject modelT = ModelIndex.Instance.Get(model);
                //    if (modelT != null) continue;
                //    yield return AssetBundleHelper.LoadAssetObject("Devices", model, AssetbundleGetSuffixalName.prefab, obj =>
                //    {
                //        if (obj == null)
                //        {
                //            Debug.LogError("获取不到模型:" + model);
                //        }
                //        else
                //        {
                //            GameObject g = obj as GameObject;
                //            ModelIndex.Instance.Add(g, model); //添加到缓存中
                //            Debug.Log("加载模型:"+model);
                //        }
                //    }); //携程方式读取模型文件
                //}

                yield return AssetbundleGet.LoadCommonModels(models, true);
                ResultText += string.Format("GetModels cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);

                recordTime = DateTime.Now;
                for (int i = 0; i < devList.Count; i++)//循环创建设备
                {
                    //float percent = (i + 1) / devList.Count;
                    //ProgressbarLoad.Instance.Show(percent);//就是加了进度条也不会出来

                    var dev = devList[i];
                    if (TypeCodeHelper.IsStaticDev(dev.TypeCode.ToString()))
                    {
                        CreateStaticDev(dev, dep, null);
                    }
                    else
                    {
                        if (TypeCodeHelper.IsFireFightDevType(dev.TypeCode.ToString())) continue;
                        GameObject modelT = ModelIndex.Instance.Get(dev.ModelName);
                        CreateDevObject(dev, container, type, dep, modelT);//创建设备的函数
                    }

                    //yield return null;//这个加上的话，进度条才会出来，但是没必要。见后面总结
                }
                ResultText += string.Format("CreatDevs cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
                /*
加载厂区内的设备的耗时：基本都是加载设备asset占用时间的，可以通过提前加载模型，和把常用模型放到resource中优化。
Get dep info, length:142 cost :129.1248ms
Get DoorAccessInfo cost:35.5331ms 
GetDevs cost:35.5331ms 
GetModels cost:2655.0736ms  //加载了两个设备Asset模型 摄像头和基站
CreatDevs cost:71.5566ms  //创建了142个设备模型
CreateDepDev All cost:2729.1469ms count:142 
                 */
            }
            DepDevCreateDic.Remove(dep);//该区域创建好了
            if (DepDevCreateDic.Count == 0)//全部区域的全部设备都创建好了
            {
                if (OnDevCreateAction != null) OnDevCreateAction();
                ProgressbarLoad.Instance.Hide();
                ResultText += string.Format("CreateDepDev All cost:{0}ms count:{1} \n", (DateTime.Now - devStartTime).TotalMilliseconds, DevCount);
                Debug.Log(ResultText);//结束创建
            }
        }
    }


    IEnumerator LoadSingleDevCorutine(DevInfo dev, GameObject container, DevType type, DepNode dep, Action<GameObject> onComplete)
    {
        DevNode devCreate = GetCreateDevById(dev.DevID, dep.NodeID);
        if (string.IsNullOrEmpty(dev.ModelName) || devCreate != null)
        {
            Debug.Log(string.Format("设备：{0} 模型名称不存在,model:{1}", dev.Name, dev.ModelName));
            if (onComplete != null) onComplete(null);
        }
        else
        {
            if (TypeCodeHelper.IsStaticDev(dev.TypeCode.ToString()))
            {
                CreateStaticDev(dev, dep, onComplete);
            }
            else
            {
                GameObject modelT = ModelIndex.Instance.Get(dev.ModelName);
                if (modelT != null)
                {
                    GameObject objInit = CreateDevObject(dev, container, type, dep, modelT);//提取创建设备的函数
                }
                else
                {
                    yield return AssetBundleHelper.LoadAssetObject("Devices", dev.ModelName, AssetbundleGetSuffixalName.prefab, obj =>
                    {
                        if (obj == null)
                        {
                            Debug.LogError("获取不到模型:" + dev.ModelName);
                            //StartCoroutine(LoadDevsCorutine(dep, container, type));//这里不需要
                            if (onComplete != null) onComplete(null);
                            return;
                        }
                        else
                        {
                            GameObject g = obj as GameObject;
                            ModelIndex.Instance.Add(g, dev.ModelName); //添加到缓存中
                            CreateDevObject(dev, container, type, dep, g);//提取创建设备的函数
                        }
                    }); //内部也是LoadAssetObject
                }

                //yield return AssetbundleGet.Instance.GetObjFromCatch(dev.ModelName, AssetbundleGetSuffixalName.prefab, obj =>
                //{
                //    CreateDevObjectEx(dev, container, type, dep, obj, onComplete);//提取创建设备的函数
                //});
            }
        }
    }

    private void CreateDevObjectEx(DevInfo dev, GameObject container, DevType type, DepNode dep, UnityEngine.Object obj, Action<GameObject> onComplete)
    {
        if (obj == null)
        {
            Debug.LogError("拖动获取不到模型:" + dev.ModelName);
            //StartCoroutine(LoadDevsCorutine(dep, container, type));//这里不需要
            if (onComplete != null) onComplete(null);
            return;
        }
        else
        {
            GameObject g = obj as GameObject;
            ModelIndex.Instance.Add(g, dev.ModelName); //添加到缓存中
            GameObject objInit = CreateDevObject(dev, container, type, dep, g);//提取创建设备的函数
            if (onComplete != null) onComplete(objInit);
        }
    }

    private GameObject CreateDevObject(DevInfo dev, GameObject container, DevType type, DepNode dep, GameObject modelT)
    {
        if (modelT == null)
        {
            Debug.LogError(string.Format("{0} info is null,modelName:{1}", dev.Name, dev.ModelName));
            return null;
        }
        GameObject o = Instantiate(modelT);
        o.transform.parent = container.transform;
        o.transform.name = dev.Name;
        o.AddCollider();
        AddDevController(o, dev, type, dep);
        SetDevPos(o, dev.Pos);
        o.SetActive(true);
        o.layer = LayerMask.NameToLayer("DepDevice");
        return o;
    }

    /// <summary>
    /// 创建静态设备
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="parnetDep"></param>
    /// <param name="onComplete"></param>
    private void CreateStaticDev(DevInfo dev, DepNode parnetDep, Action<GameObject> onComplete)
    {
        if(staticDevInfos.Find(devT=>devT!=null&&devT.Id==dev.Id)==null)//改用Dic,主键换成int?
        {
            staticDevInfos.Add(dev);
        }
        FacilityDevController staticDevT = StaticDevList.Find(i => i.gameObject.name == dev.ModelName);
        if (staticDevT != null)
        {
            SaveDepDevInfo(parnetDep, staticDevT, dev);
            staticDevT.CreateFollowUI();
            if (onComplete != null) onComplete(staticDevT.gameObject);
        }
        else
        {
            if (onComplete != null) onComplete(null);
        }
    }
    /// <summary>
    /// 通过设备信息，创建单个设备
    /// </summary>
    /// <param name="devInfo"></param>
    /// <param name="OnSingleDevCreate"></param>
    private void CreateDevByDevId(DevInfo devInfo, Action<DevNode> OnSingleDevCreate)
    {
        if (devInfo == null)
        {
            OnSingleDevCreate(null);
            Debug.LogError("ID为[" + devInfo.DevID + "]的设备找不到");
            return;
        }
        DepNode dep = GetDepNodeById((int)devInfo.ParentId);
        if (dep == null)
        {
            Debug.LogError("DevParentId not find:" + devInfo.ParentId);
            if (OnDevCreateAction != null) OnSingleDevCreate(null);
        }
        if (dep)
        {
            Debug.LogError("RoomFactory.CreateDevByDevId:" + devInfo.Id);
            return;//从这里返回，设备定位就相当与没有反应了
        }
        List<int> pidList = new List<int>() { dep.NodeID };
        GetDoorAccessInfo(pidList);
        DevType devType = DevType.DepDev;
        GameObject devContainer = GetDepDevContainer(dep, ref devType);
        StartCoroutine(LoadSingleDevCorutine(devInfo, devContainer, devType, dep, obj =>
            {
                DevNode dev = obj.GetComponent<DevNode>();
                if (OnSingleDevCreate != null) OnSingleDevCreate(dev);
            }));
    }
    /// <summary>
    /// 设置设备位置
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="pos"></param>
    private void SetDevPos(GameObject obj, DevPos pos)
    {
        if (pos == null)
        {
            SetErrorDevPos(obj);
            return;
        }
        DevNode devNode = obj.GetComponent<DevNode>();
        bool isLocalPos = !(devNode.ParentDepNode == FactoryDepManager.Instance);
        Vector3 cadPos = new Vector3(pos.PosX, pos.PosY, pos.PosZ);
        Vector3 unityPos = LocationManager.CadToUnityPos(cadPos, isLocalPos);
        if (isLocalPos)
        {
            obj.transform.localPosition = new Vector3(unityPos.x, unityPos.y, unityPos.z);
        }
        else
        {
            obj.transform.position = new Vector3(unityPos.x, unityPos.y, unityPos.z);
        }
        obj.transform.eulerAngles = new Vector3(pos.RotationX, pos.RotationY, pos.RotationZ);
        obj.transform.localScale = new Vector3(pos.ScaleX, pos.ScaleY, pos.ScaleZ);
    }

    private void SetErrorDevPos(GameObject obj)
    {
        Debug.Log("Error dev name:" + obj.transform.name);
        obj.transform.position = Vector3.zero;
        obj.transform.eulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        //obj.Reset();
    }
    /// <summary>
    /// 给设备添加脚本
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="info"></param>
    /// <param name="type"></param>
    private void AddDevController(GameObject dev, DevInfo info, DevType type, DepNode depNode)
    {
        try
        {
            if (TypeCodeHelper.IsDoorAccess(info.TypeCode.ToString()))
            {
                DoorAccessDevController doorController = dev.AddComponent<DoorAccessDevController>();
                Dev_DoorAccess doorAccess = DoorAccessList.Find(i => i.DevID == info.DevID);
                if (doorAccess == null)
                {
                    Debug.LogError("DoorAccess not find:" + info.DevID);
                    return;
                }
                doorAccess.DevInfo = info;
                doorController.DoorAccessInfo = doorAccess;
                SaveDepDevInfo(depNode, doorController, info);
                if (depNode.Doors != null)
                {
                    DoorAccessItem doorItem = depNode.Doors.GetDoorItem(doorAccess.DoorId);
                    doorItem.AddDoorAccess(doorController);
                    doorController.DoorItem = doorItem;
                }
                else
                {
                    Log.Error(string.Format("RoomFactory.AddDevController:{0} ，Doors is null", depNode.NodeName));
                }
            }
            else if (TypeCodeHelper.IsBorderAlarmDev(info.TypeCode.ToString()))
            {
                BorderDevController depDev = dev.AddComponent<BorderDevController>();
                SaveDepDevInfo(depNode, depDev, info);
            }
            else if (TypeCodeHelper.IsCamera(info.TypeCode.ToString()))
            {
                CameraDevController depDev = dev.AddComponent<CameraDevController>();
                SaveDepDevInfo(depNode, depDev, info);
            }
            else
            {
                NavMeshObstacle obstacle = dev.gameObject.AddMissingComponent<NavMeshObstacle>();
                obstacle.carving = true; //参考知识：https://www.jianshu.com/p/eae6c84793ac

                switch (type)
                {
                    case DevType.DepDev:
                        DepDevController depDev = dev.AddComponent<DepDevController>();
                        SaveDepDevInfo(depNode, depDev, info);
                        break;
                    case DevType.RoomDev:
                        RoomDevController roomDev = dev.AddComponent<RoomDevController>();
                        SaveDepDevInfo(depNode, roomDev, info);
                        break;
                    default:
                        Debug.Log("DevType not find:" + type);
                        break;
                }
            }
        }catch(Exception e)
        {
            string devId = info == null ? "null" : info.DevID;
            string depId = depNode == null ? "null" : depNode.NodeID.ToString();
            Log.Error(string.Format("RoomFactory.AddDevController,DevInfo.local_Devid:{0} depId:{1}\n Exception:{2}",devId,depId,e.ToString()));
        }
    }
    #endregion
    #region 设备定位模块

    /// <summary>
    /// 保存设备信息
    /// </summary>
    /// <param name="depId"></param>
    /// <param name="dev"></param>
    public void SaveDepDevInfo(DepNode dep, DevNode dev, DevInfo devInfo)
    {
        dev.Info = devInfo;
        dev.ParentDepNode = dep;
        int depId = dep.NodeID;
        DepDevDic.AddDev(depId, dev);
    }
    /// <summary>
    /// 获取区域下的所有设备
    /// </summary>
    /// <param name="dep">区域</param>
    /// <param name="containRoomDev">是否包含房间设备（Floor）</param>
    /// <returns></returns>
    public List<DevNode> GetDepDevs(DepNode dep, bool containRoomDev = true)
    {
        return DepDevDic.GetDepDevs(dep, containRoomDev);
    }
    /// <summary>
    /// 通过设备Id获取已经创建的设备
    /// </summary>
    /// <param name="devId"></param>
    /// <param name="parentId"></param>
    /// <returns>返回已经创建的设备</returns>
    public DevNode GetCreateDevById(string devId, int parentId)
    {
        return DepDevDic.GetDev(devId, parentId);
    }
    /// <summary>
    /// 看是否int类型的DevID 
    /// </summary>
    /// <param name="devId"></param>
    /// <returns></returns>
    public static int? TryGetDevId(string devId)
    {
        try
        {
            int value = int.Parse(devId);
            return value;
        }catch(Exception e)
        {
            return null;
        }
    }
    /// <summary>
    /// 通过设备Id,获取设备
    /// </summary>
    /// <param name="devId"></param>
    /// <returns></returns>
    public void GetDevById(string devId, Action<DevNode> onDevFind)
    {
        DevNode dev = DepDevDic.FindDev(devId);
        if (dev == null)
        {
            DevInfo info = GetDevInfoByDevId(devId);
            if (info == null)
            {
                Debug.LogError("ID为[" + devId + "]的设备找不到");
                onDevFind(null);
                return;
            }
            CreateDevByDevId(info, obj => { if (onDevFind != null) onDevFind(obj); });
        }
        else
        {
            if (onDevFind != null) onDevFind(dev);
        }
    }

    /// <summary>
    /// 通过设备Id(不是字符串DevId),获取设备
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public void GetDevByid(int id, Action<DevNode> onDevFind)
    {
        DevNode dev = DepDevDic.FindDev(id);
        if (dev == null)
        {
            DevInfo info = GetDevInfoByid(id);
            if (info == null||(info!=null&&TypeCodeHelper.IsFireFightDevType(info.TypeCode.ToString())))
            {
                if(info==null)Debug.LogError("ID为[" + id + "]的设备找不到");
                onDevFind(null);
                return;
            }
            CreateDevByDevId(info, obj => { if (onDevFind != null) onDevFind(obj); });
        }
        else
        {
            if (onDevFind != null) onDevFind(dev);
        }
    }

    /// <summary>
    /// 通过设备Id,获取设备信息
    /// </summary>
    /// <param name="devId"></param>
    /// <returns></returns>
    private DevInfo GetDevInfoByDevId(string devId)
    {
        CommunicationObject service = CommunicationObject.Instance;
        if (service)
        {
            return service.GetDevByDevId(devId);
        }
        return null;
    }

    /// <summary>
    /// 通过设备Id,获取设备信息
    /// </summary>
    /// <param name="devId"></param>
    /// <returns></returns>
    private DevInfo GetDevInfoByid(int id)
    {
        CommunicationObject service = CommunicationObject.Instance;
        if (service)
        {
            return service.GetDevByid(id);
        }
        return null;
    }


    /// <summary>
    /// 删除设备信息
    /// </summary>
    /// <param name="dev"></param>
    public void RemoveDevInfo(DevNode dev)
    {
        if (dev.Info == null) return;
        RemoveDevInfo(dev.Info.DevID);
    }
    /// <summary>
    /// 删除设备信息
    /// </summary>
    /// <param name="devId"></param>
    public void RemoveDevInfo(string devId)
    {
        try
        {
            DepDevDic.RemoveDev(devId);
        }
        catch (Exception e)
        {
            Debug.LogError("RoomFactory.RemoveDevinfo :" + e.ToString());
        }
    }

    /// <summary>
    /// 聚焦设备(去掉int类型，统一用string,方法内部区分int还是Guid的devId)
    /// </summary>
    public void FocusDev(string devId, int depId,Action<bool> onFocusComplete=null)
    {
        Log.Info("RoomFactory.FocusDev", string.Format("depId:{0},devId:{1}", depId, devId));
        //2019_04_30_cww_处理问题:处理打开集控楼，打开一楼，创建设备，打开其他建筑，卸载集控楼，打开设备搜索界面，定位集控楼1楼设备，没有反应...       
        DevNode devNode = DepDevDic.GetDev(devId, depId);
        Log.Info("RoomFactory.FocusDev", string.Format("devNode != null:{0}", devNode != null));
        if (devNode != null)
        {
            int? devIdTemp = TryGetDevId(devId);//区分int还是Guid的devId
            Log.Info("RoomFactory.FocusDev", string.Format("devIdTemp :{0}", devIdTemp));
            if (devIdTemp == null)
            {
                FocusDevInner(devId, onFocusComplete);//创建并定位设备
            }
            else
            {
                FocusDevInner((int)devIdTemp, onFocusComplete);//创建并定位设备
            }
        }
        else
        {
            FocusDepAndDev(devId, depId,onFocusComplete);//先定位区域，在定位设备
        }
    }

    private void FocusDepAndDev(string devId, int depId,Action<bool>onFocusComplete=null)
    {
        try
        {
            Log.Info("RoomFactory.FocusDepAndDev", string.Format("depId:{0},devId :{1}", depId,devId));
            DepNode dep = GetDepNodeById(depId);
            if (dep && dep.HaveTopNode)
            {
                FocusNode(dep, () =>
                {
                    int? devIdTemp = TryGetDevId(devId);
                    if(devIdTemp==null)
                    {
                        FocusDevInner(devId, onFocusComplete);
                    }
                    else
                    {
                        FocusDevInner((int)devIdTemp,onFocusComplete);
                    }                                   
                });
            }
            else
            {
                Debug.LogError("RoomFactory.FoucusDev,Dep is null:" + depId);
                if (onFocusComplete != null) onFocusComplete(false);
            }
        }catch(Exception e)
        {
            Debug.LogError("Error: Roomfactory.FoucusDepAndDev:"+e.ToString());
            if (onFocusComplete != null) onFocusComplete(false);
        }
    }

    /// <summary>
    /// 聚焦设备 int类型ID
    /// </summary>
    /// <param name="devId"></param>
    private void FocusDevInner(int devId, Action<bool> onFocusComplete = null)
    {
        Log.Info("RoomFactory.FocusDevInner 2", string.Format("devId :{0}", devId));
        GetDevByid(devId, dev =>
        {
            if (dev)
            {
                dev.FocusOn();
                if (onFocusComplete != null) onFocusComplete(true);
            }
            else
            {
                DevInfo info = GetDevInfoByid(devId);
                if(info!=null&&TypeCodeHelper.IsFireFightDevType(info.TypeCode.ToString()))
                {
                    if (onFocusComplete != null) onFocusComplete(true);
                }
                else
                {
                    Debug.LogError("RoomFactory.FoucusDev,Dev is null :" + devId);
                    if (onFocusComplete != null) onFocusComplete(false);
                }              
            }
        });
    }
    /// <summary>
    /// 聚焦设备s
    /// </summary>
    /// <param name="devId"></param>
    private void FocusDevInner(string devId, Action<bool> onFocusComplete = null)
    {
        Log.Info("RoomFactory.FocusDevInner 1", string.Format("devId :{0}", devId));
        GetDevById(devId, dev =>
        {
            if (dev)
            {
                dev.FocusOn();
                if (onFocusComplete != null) onFocusComplete(true);
            }
            else
            {
                Debug.LogError("RoomFactory.FoucusDev,Dev is null :" + devId);
                if (onFocusComplete != null) onFocusComplete(false);
            }                
        });
    }
    #endregion
}
