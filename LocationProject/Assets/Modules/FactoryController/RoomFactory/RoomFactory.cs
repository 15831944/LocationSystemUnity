using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.M_Plugins.Helpers.Utils;
using Types = Location.WCFServiceReferences.LocationServices.AreaTypes;

public class RoomFactory : MonoBehaviour
{
    public static RoomFactory Instance;
    /// <summary>
    /// 是否正在聚焦区域
    /// </summary>
    public bool IsFocusingDep;
    /// <summary>
    /// 区域信息列表（包含区域、建筑、机房）  区域名称(key)DepNode(value)
    /// </summary>
    private List<DepNode> NodeDic = new List<DepNode>();
    /// <summary>
    /// 区域下设备列表 区域ID(key) DevNode(value)
    /// </summary>
    private Dictionary<int, List<DevNode>> DepDevDic = new Dictionary<int, List<DevNode>>();
    /// <summary>
    /// 静态设备列表
    /// </summary>
    private List<FacilityDevController> StaticDevList = new List<FacilityDevController>();
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
        Instance = this;
        //Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        StoreDepInfo();
        BindingModelIDByNodeName((SaveStaticDevInfo));
        //SaveStaticDevInfo();
    }

    void Awake()
    {
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
                List<int> nodeIdsT = GetCurrentDepNodeChildNodeIds(nodeT);
                nodeIds.AddRange(nodeIdsT);
            }
        }
        return nodeIds;

    }
    /// <summary>
    /// 通过区域ID,获取区域管理脚本
    /// </summary>
    /// <param name="physicalTopologyId"></param>
    /// <returns></returns>
    public DepNode GetDepNodeById(int physicalTopologyId)
    {
        return NodeDic.FirstOrDefault(dep => dep.NodeID == physicalTopologyId);
    }

    /// <summary>
    /// 根据名称，找到对应区域
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public DepNode GetDepNode(string key)
    {
        DepNode node = NodeDic.FirstOrDefault(dep => dep.NodeName == key);
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
        if (node == null)
        {
            Log.Alarm("RoomFactory.AddDepNode", "node == null");
            return;
        }
        if (!NodeDic.Contains(node))
        {
            if (node.NodeName == "集控楼4.5m层测试范围1")
            {
                int i = 0;
            }
            NodeDic.Add(node);
        }
        else
        {
            Log.Alarm("RoomFactory.AddDepNode", string.Format("存在相同Key的Node,id={0},name={1}", node.NodeID, node.NodeName));
            //NodeDic[key] = node;
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
        FactoryDepManager depManager = FactoryDepManager.Instance;
        if (depManager)
        {
            AddDepNode(depManager);
            depManager.AllNodes = depManager.transform.GetComponentsInChildren<DepNode>(false).ToList();
            foreach (DepNode item in depManager.ChildNodes)
            {
                AddDepNode(item);
                if (item.HaveChildren())
                {
                    StoreChildInfo(item);
                }
            }
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
            if(child==null)
            {
                if(node!=null)
                {
                    Debug.LogError(string.Format("{0} child is null..", node.NodeName));
                }
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
        CommunicationObject.Instance.GetTopoTreeAsync((topoRoot)=>
        {
            StartBindingTopolgy(topoRoot);
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
        Log.Info("StartBindingTopolgy Start !!!!!!!!!!!!!!!11");
        if (toplogy == null || toplogy.Children==null || toplogy.Children.Length == 0)
        {
            Debug.LogError("PhysicalTopology is null!");
            return;
        }
        RoomFactory factory = RoomFactory.Instance;
        if (factory)
        {
            List<FactoryDepManager> parks = GameObject.FindObjectsOfType<FactoryDepManager>().ToList();
            List<PhysicalTopology> topologies = toplogy.Children.ToList();
            foreach (var factoryTopo in topologies)
            {
                if (factoryTopo.Name == "四会热电厂" || factoryTopo.Name == "高新软件园")
                {
                    List<PhysicalTopology> rangesT = new List<PhysicalTopology>();
                    List<PhysicalTopology> factoryTopologies = factoryTopo.Children.ToList();
                    foreach (PhysicalTopology topoNode in factoryTopologies)
                    {
                        DepNode node = GetDepNode(topoNode.Name);
                        if (node != null)
                        {
                            node.TopoNode = topoNode;
                            if (topoNode.Children != null)
                            {
                                BindingChild(node, topoNode.Children.ToList());
                            }
                        }
                        else
                        {
                            if (topoNode.Type == Types.范围)
                            {
                                //DepNode toplogyNode = GetDepNode(factoryTopo.Name);
                                //AddRange(toplogyNode, topoNode.Children.ToList());
                                rangesT.Add(topoNode);
                            }
                            else
                            {
                                Log.Alarm("StartBindingTopolgy", "未找到DepNode:" + topoNode.Name);
                            }
                        }
                    }
                    DepNode toplogyNode = GetDepNode(factoryTopo.Name);
                    if (toplogyNode != null)
                    {
                        toplogyNode.TopoNode = factoryTopo;
                        AddRange(toplogyNode, rangesT);
                    }
                }
                FactoryDepManager park = parks.Find(i => i.NodeName == factoryTopo.Name);
                if (park != null)
                {
                    park.TopoNode = factoryTopo;
                }
                else
                {
                    Log.Alarm("StartBindingTopolgy", "未找到园区:" + factoryTopo.Name);
                }
            }
        }
        else
        {
            Log.Error("StartBindingTopolgy", "RoomFactory.Instance==null");
        }

        Log.Info("StartBindingTopolgy End !!!!!!!!!!!!!!!11");
    }

    private void BindingChild(DepNode node, List<PhysicalTopology> topologies)
    {
        List<PhysicalTopology> rangesT = new List<PhysicalTopology>();
        foreach (var item in node.ChildNodes)
        {
            if (item == null) continue;
            if (!string.IsNullOrEmpty(item.NodeName))
            {
                PhysicalTopology topology = topologies.Find(topo => topo.Name == item.NodeName);
                if (topology != null)
                {
                    item.TopoNode = topology;
                    if (topology.Children == null || topology.Children.Length == 0) continue;

                    if (item as FloorController)
                    {
                        FloorController floor = item as FloorController;
                        AddRoomInFloor(floor, topology.Children.ToList());
                    }

                    BindingChild(item, topology.Children.ToList());
                }
                else
                {
                    Log.Alarm("BindingChild", "未找到Topo节点:" + item.NodeName);
                }
            }
        }

        AddRange(node, topologies);
    }

    /// <summary>
    /// 在厂区中添加范围
    /// </summary>
    /// <param name="depNodeT"></param>
    private void AddRange(DepNode depNodeT, List<PhysicalTopology> roomTopo)
    {
        if (depNodeT == null) return;
        PhysicalTopology topoNode = depNodeT.TopoNode;
        //PhysicalTopology buildingNode = depNodeT.ParentNode.TopoNode;
        if (topoNode == null)
        {
            Debug.Log("TopoNode is null...");
            return;
        }

        GameObject ranges = new GameObject("Ranges");
        ranges.transform.parent = depNodeT.transform;
        ranges.transform.localEulerAngles = Vector3.zero;
        ranges.transform.position = Vector3.zero;

        ranges.transform.localScale = Vector3.one;
        foreach (var topo in roomTopo)
        {
            if (GetDepNodeById(topo.Id) != null) continue;
            if (topo.Type != Types.范围) continue;
            GameObject rangeT = new GameObject(topo.Name);
            rangeT.transform.parent = ranges.transform;
            rangeT.transform.localScale = Vector3.one;
            rangeT.transform.localEulerAngles = Vector3.zero;
            RangeController rangeController = rangeT.AddComponent<RangeController>();
            rangeController.TopoNode = topo;
            rangeController.NodeName = topo.Name;
            rangeController.NodeObject = rangeT;

            rangeT.transform.position = Vector3.zero;

            rangeController.angleFocus = new Vector2(60, 0);
            rangeController.camDistance = 15;
            rangeController.angleRange = new Mogoson.CameraExtension.Range(5, 90);
            rangeController.disRange = new Mogoson.CameraExtension.Range(2, 15);
            rangeController.AreaSize = new Vector2(5, 5);


            rangeController.ParentNode = depNodeT;
            depNodeT.ChildNodes.Add(rangeController);
            if (topo.Name == "集控楼4.5m层测试范围1")
            {
                int i = 0;
            }
            NodeDic.Add(rangeController);
        }
    }

    /// <summary>
    /// 在楼层中添加机房
    /// </summary>
    /// <param name="floor"></param>
    private void AddRoomInFloor(FloorController floor, List<PhysicalTopology> roomTopo)
    {
        PhysicalTopology topoNode = floor.TopoNode;
        PhysicalTopology buildingNode = floor.ParentNode.TopoNode;
        if (topoNode == null || buildingNode == null)
        {
            Debug.Log("TopoNode is null...");
            return;
        }
        //TransformM tm = buildingNode.Transfrom;
        ////Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + topoNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
        //Vector3 pos2D = new Vector3((float)(-tm.SX / 2f), (float)(-tm.SY / 2), (float)(-tm.SZ / 2));
        //Log.Info("建筑物的右下角坐标:" + pos2D);
        //Vector3 buildPos = LocationManager.GetRealVector(pos2D);

        GameObject rooms = new GameObject("Rooms");
        rooms.transform.parent = floor.transform;
        rooms.transform.localEulerAngles = Vector3.zero;
        rooms.transform.position = Vector3.zero;
        //rooms.transform.eulerAngles= new Vector3((float)tm.RX, (float)tm.RY, (float)tm.RZ);
        //rooms.transform.position = buildPos;
        rooms.transform.localScale = Vector3.one;
        foreach (var topo in roomTopo)
        {
            if (GetDepNodeById(topo.Id) != null) continue;
            if (topo.Type == Types.范围) continue;
            AddRoomInFloorOP(floor, rooms, topo);
        }
    }

    private void AddRoomInFloorOP(FloorController floor, GameObject rooms, PhysicalTopology topo)
    {
        GameObject room = new GameObject(topo.Name);
        room.transform.parent = rooms.transform;
        room.transform.localScale = Vector3.one;
        room.transform.localEulerAngles = Vector3.zero;
        RoomController roomController = room.AddComponent<RoomController>();
        roomController.TopoNode = topo;
        roomController.NodeName = topo.Name;
        roomController.NodeObject = room;

        //if (topo.Name.Contains("集控楼"))
        //{
        //    Debug.Log("111");
        //}
        //Todo:计算出房间中心点的位置
        //TransformM tranM = topo.Transfrom;
        //if (tranM == null)
        //{
        //    Debug.LogWarning("tranM == null");
        //    continue;
        //}
        //Vector3 pos = new Vector3((float)tranM.X, (float)tranM.Y, (float)tranM.Z);
        //Vector3 targetPos = LocationManager.CadToUnityPos(pos,true);
        room.transform.position = Vector3.zero;
        //room.transform.position = buildPos;

        roomController.angleFocus = new Vector2(60, 0);
        roomController.camDistance = 15;
        roomController.angleRange = new Mogoson.CameraExtension.Range(5, 90);
        roomController.disRange = new Mogoson.CameraExtension.Range(2, 15);
        roomController.AreaSize = new Vector2(5, 5);
        if (topo.Name == "集控楼4.5m层测试范围1")
        {
            int i = 0;
        }

        roomController.ParentNode = floor;
        floor.ChildNodes.Add(roomController);
        NodeDic.Add(roomController);
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
        DepNode node = GetDepNodeById(topoNode.Id);
        if (node != null)
        {
            FocusNode(node);
        }
        else
        {
            Log.Alarm("未找到节点区域");
        }
    }
    public void FocusNode(DepNode node, Action onDevCreateFinish = null)
    {
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
        SceneEvents.OnDepNodeChangeStart(lastNodep,node);
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
            FactoryDepManager manager = FactoryDepManager.Instance;
            if (FactoryDepManager.currentDep != null)
            {
                DeselctLast(FactoryDepManager.currentDep, node);
            }
            node.OpenDep(() =>
            {
                IsFocusingDep = false;
                if (onDevCreateFinish != null) onDevCreateFinish();
                SceneEvents.OnDepCreateCompleted(node);
            });
            if (isFocusBreak) IsFocusingDep = true;
        }

        if (TopoTreeManager.Instance) TopoTreeManager.Instance.SetSelectNode(lastNodep, node);        
        if(node!=FactoryDepManager.Instance)PersonnelTreeManage.Instance.areaDivideTree.Tree.AreaSelectNodeByType(node.NodeID);       
    }

    /// <summary>
    /// 聚焦区域节点，这里是用在聚焦人员时，切换到人员所在的区域节点时用的
    /// </summary>
    /// <param name="node"></param>
    /// <param name="onDevCreateFinish"></param>
    public void FocusNodeForFocusPerson(DepNode node, Action onDevCreateFinish = null, bool isSetSelectNode = true)
    {
        //if (node.TopoNode != null && node.TopoNode.Type == AreaTypes.范围) return;
        if (FactoryDepManager.currentDep == node && IsFocusingDep)
        {
            //处理拓扑树,快速单击两次的问题
            Debug.Log(string.Format("{0} is Focusing...", node.NodeName));
            return;
        }
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
            FactoryDepManager manager = FactoryDepManager.Instance;
            if (FactoryDepManager.currentDep != null)
            {
                DeselctLast(FactoryDepManager.currentDep, node);
            }
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

    ///// <summary>
    ///// 聚焦区域节点，这里是用聚焦人员时，切换到人员所在的区域节点时用的
    ///// </summary>
    ///// <param name="node"></param>
    ///// <param name="onDevCreateFinish"></param>
    //public void FocusNodeForFocusPerson(DepNode node, Action onDevCreateFinish = null)
    //{
    //    if (FactoryDepManager.currentDep == node && IsFocusingDep)
    //    {
    //        //处理拓扑树,快速单击两次的问题
    //        Debug.Log(string.Format("{0} is Focusing...", node.NodeName));
    //        return;
    //    }
    //    bool isFocusBreak = false;
    //    if (IsFocusingDep) isFocusBreak = true;
    //    IsFocusingDep = true;
    //    if (DevNode.CurrentFocusDev != null) DevNode.CurrentFocusDev.FocusOff(false);
    //    Log.Info(string.Format("FocusNode ID:{0},Name:{1},Type:{2}", node.NodeID, node.NodeName, node.GetType()));
    //    DepNode lastNodep = FactoryDepManager.currentDep;
    //    if (FactoryDepManager.currentDep == node)
    //    {
    //        //node.FocusOn(() =>
    //        //{
    //        //    IsFocusingDep = false;
    //        //    if (onDevCreateFinish != null) onDevCreateFinish();
    //        //});
    //        IsFocusingDep = false;
    //        if (onDevCreateFinish != null) onDevCreateFinish();
    //        if (isFocusBreak) IsFocusingDep = true;
    //    }
    //    else
    //    {
    //        FactoryDepManager manager = FactoryDepManager.Instance;
    //        if (FactoryDepManager.currentDep != null)
    //        {
    //            DeselctLast(FactoryDepManager.currentDep, node);
    //        }
    //        node.OpenDep(() =>
    //        {
    //            IsFocusingDep = false;
    //            if (onDevCreateFinish != null) onDevCreateFinish();
    //            SceneEvents.OnDepCreateCompleted(node);
    //        });
    //        if (isFocusBreak) IsFocusingDep = true;
    //    }
    //    if (TopoTreeManager.Instance) TopoTreeManager.Instance.SetSelectNode(lastNodep, node);
    //}

    /// <summary>
    /// 取消上一个区域的选中,无视角转换
    /// </summary>
    /// <param name="lastNode"></param>
    /// <param name="currentNode"></param>
    public void DeselctLast(DepNode lastNode, DepNode currentNode)
    {
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
        if (currentFocusDep == dep) return;
        currentFocusDep = dep;
        ResultText = "";
        OnDevCreateAction = onComplete;
        //Debug.LogError(string.Format("StartCreateDev {0}",dep));
        ThreadManager.Run(() =>
        {
            recordTime = DateTime.Now;
            List<DepNode> depList = new List<DepNode>();
            CommunicationObject service = CommunicationObject.Instance;
            DevCount = 0;
            depList.Add(dep);
            if (dep is BuildingController || dep is FloorController)
            {
                List<DepNode> childList = GetChildNodes(dep);
                if (childList != null && childList.Count != 0) depList.AddRange(childList);
            }
            ResultText += string.Format("Save roomInfo cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
            GetDevInfo(depList);
        }, () =>
        {
            DepNode factoryDep = FactoryDepManager.currentDep;
            bool isRoomState = factoryDep is RoomController && factoryDep.ParentNode == currentFocusDep;
            if (isRoam || isRoomState || currentFocusDep == factoryDep)
            {
                recordTime = DateTime.Now;
                CreateDepDev();
                ResultText += string.Format("CreateDev cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
                Debug.Log(ResultText);
            }
        }, "LoadDevInfo...");
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
        CommunicationObject service = CommunicationObject.Instance;
        if (service)
        {
            RecordTime = DateTime.Now;
            List<int> pidList = GetPidList(deps);
            List<DevInfo> devInfos = service.GetDevInfoByParent(pidList);
            int count = devInfos == null ? 0 : devInfos.Count;
            ResultText += string.Format("Get dep info, length:{0} cost :{1}ms\n", count, (DateTime.Now - RecordTime).TotalMilliseconds);
            recordTime = DateTime.Now;
            SaveDepDevInfoInCreating(deps, devInfos);
            GetDoorAccessInfo(pidList);
            ResultText += string.Format("Get DoorAccessInfo cost:{0}ms \n", (DateTime.Now - recordTime).TotalMilliseconds);
        }
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
            CurrentCreateIndex = 0;
            foreach (var item in DepDevCreateDic.Keys)
            {
                int id = item.NodeID;
                DevType devType = DevType.DepDev;
                GameObject devContainer = GetDepDevContainer(item, ref devType);
                StartCoroutine(LoadDevsCorutine(item, devContainer, devType));
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
        yield return null;
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
            StartCoroutine(LoadSingleDevCorutine(dev, container, type, dep, obj =>
            {
                StartCoroutine(LoadDevsCorutine(dep, container, type));
            }));
        }
        else
        {
            DepDevCreateDic.Remove(dep);
            if (DepDevCreateDic.Count == 0)
            {
                if (OnDevCreateAction != null) OnDevCreateAction();
                ProgressbarLoad.Instance.Hide();
                SystemModeTweenManage.Instance.StartTweener();
            }
        }
    }
    IEnumerator LoadSingleDevCorutine(DevInfo dev, GameObject container, DevType type, DepNode dep, Action<GameObject> onComplete)
    {
        DevNode devCreate = GetCreateDevById(dev.DevID, dep.NodeID);
        if (string.IsNullOrEmpty(dev.ModelName) || devCreate != null)
        {
            Debug.Log(string.Format("设备：{0} 模型名称不存在", dev.Name));
            if (onComplete != null) onComplete(null);
        }
        else
        {
            if (TypeCodeHelper.IsStaticDev(dev.TypeCode.ToString()))
            {
                if(dev.ModelName=="J1_65")
                {
                    int iii = 0;
                }
                CreateStaticDev(dev, dep, onComplete);
            }
            else
            {
                yield return null;
                GameObject modelT = ModelIndex.Instance.Get(dev.ModelName);
                if (modelT != null)
                {
                    GameObject o = Instantiate(modelT);
                    o.transform.parent = container.transform;
                    o.transform.name = dev.Name;
                    o.AddCollider();
                    AddDevController(o, dev, type, dep);
                    SetDevPos(o, dev.Pos);
                    o.SetActive(true);
                    o.layer = LayerMask.NameToLayer("DepDevice");
                    if (onComplete != null) onComplete(o);
                }
                else
                {
                    AssetbundleGet.Instance.GetObj(dev.ModelName, AssetbundleGetSuffixalName.prefab, obj =>
                    {
                        if (obj == null)
                        {
                            Debug.LogError("拖动获取不到模型:" + dev.ModelName);
                            StartCoroutine(LoadDevsCorutine(dep, container, type));
                            return;
                        }
                        GameObject g = obj as GameObject;
                        ModelIndex.Instance.Add(g, dev.ModelName); //添加到缓存中
                        GameObject objInit = Instantiate(g);
                        objInit.transform.parent = container.transform;
                        objInit.transform.name = dev.Name;
                        objInit.AddCollider();
                        AddDevController(objInit, dev, type, dep);
                        SetDevPos(objInit, dev.Pos);
                        objInit.SetActive(true);
                        objInit.layer = LayerMask.NameToLayer("DepDevice");
                        if (onComplete != null) onComplete(objInit);
                    });
                }
            }
        }
    }
    /// <summary>
    /// 创建静态设备
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="parnetDep"></param>
    /// <param name="onComplete"></param>
    private void CreateStaticDev(DevInfo dev, DepNode parnetDep, Action<GameObject> onComplete)
    {
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
    }
    /// <summary>
    /// 给设备添加脚本
    /// </summary>
    /// <param name="dev"></param>
    /// <param name="info"></param>
    /// <param name="type"></param>
    private void AddDevController(GameObject dev, DevInfo info, DevType type, DepNode depNode)
    {
        if (TypeCodeHelper.IsDoorAccess(info.ModelName))
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
                Debug.Log(string.Format("{0} ，Doors is null", depNode.NodeName));
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
        if (!DepDevDic.ContainsKey(depId))
        {
            List<DevNode> devList = new List<DevNode>();
            devList.Add(dev);
            DepDevDic.Add(depId, devList);
        }
        else
        {
            List<DevNode> devNodes;
            DepDevDic.TryGetValue(depId, out devNodes);
            if (devNodes != null)
            {
                devNodes.Add(dev);
            }
            else
            {
                devNodes = new List<DevNode>();
                devNodes.Add(dev);
                DepDevDic[depId] = devNodes;
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
        List<DevNode> devListTemp;
        DepDevDic.TryGetValue(dep.NodeID, out devListTemp);
        if (devListTemp == null) devListTemp = new List<DevNode>();
        if (devListTemp.Count != 0) depDevs.AddRange(devListTemp);
        //楼层下，包括楼层设备+房间设备
        if (dep as FloorController && containRoomDev)
        {
            foreach (var room in dep.ChildNodes)
            {
                List<DevNode> RoomDevs;
                DepDevDic.TryGetValue(room.NodeID, out RoomDevs);
                if (RoomDevs != null) depDevs.AddRange(RoomDevs);
            }
        }
        return depDevs;
    }
    /// <summary>
    /// 通过设备Id获取已经创建的设备
    /// </summary>
    /// <param name="devId"></param>
    /// <param name="parentId"></param>
    /// <returns>返回已经创建的设备</returns>
    public DevNode GetCreateDevById(string devId, int parentId)
    {
        List<DevNode> devListTemp;
        DepDevDic.TryGetValue(parentId, out devListTemp);
        if (devListTemp != null && devListTemp.Count != 0)
        {
            DevNode dev = devListTemp.Find(i => i.Info.DevID == devId);
            return dev;
        }
        else
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
        DevNode dev = null;
        foreach (List<DevNode> devListTemp in DepDevDic.Values)
        {
            dev = devListTemp.Find(i => i.Info.DevID == devId);
            if (dev != null)
            {
                if (onDevFind != null) onDevFind(dev);
                return;
            }
        }
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
    }

    /// <summary>
    /// 通过设备Id(不是字符串DevId),获取设备
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public void GetDevByid(int id, Action<DevNode> onDevFind)
    {
        DevNode dev = null;
        foreach (List<DevNode> devListTemp in DepDevDic.Values)
        {
            dev = devListTemp.Find(i => i.Info.Id == id);
            if (dev != null)
            {
                if (onDevFind != null) onDevFind(dev);
                return;
            }
        }
        if (dev == null)
        {
            DevInfo info = GetDevInfoByid(id);
            if (info == null)
            {
                Debug.LogError("ID为[" + id + "]的设备找不到");
                onDevFind(null);
                return;
            }
            CreateDevByDevId(info, obj => { if (onDevFind != null) onDevFind(obj); });
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
            foreach (List<DevNode> devListTemp in DepDevDic.Values)
            {
                DevNode dev = devListTemp.Find(i => i.Info.DevID == devId);
                devListTemp.Remove(dev);
                break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("RoomFactory.RemoveDevinfo :" + e.ToString());
        }
    }
    /// <summary>
    /// 聚焦设备
    /// </summary>
    /// <param name="devID"></param>
    /// <param name="depId"></param>
    public void FocusDev(int devID, int depId)
    {
        if (!DepDevDic.ContainsKey(depId))
        {
            DepNode dep = GetDepNodeById(depId);
            if (dep)
            {
                FocusNode(dep, () =>
                {
                    FocusDev(devID);
                });
            }
            else
            {
                Debug.LogError("RoomFactory.FoucusDev,Dep is null:" + depId);
            }
        }
        else
        {
            FocusDev(devID);
        }
    }
    /// <summary>
    /// 聚焦厂区设备
    /// </summary>
    /// <param name="devID"></param>
    public void FocusDev(string Local_DevID, int depId)
    {
        if (!DepDevDic.ContainsKey(depId))
        {
            DepNode dep = GetDepNodeById(depId);
            if (dep)
            {
                FocusNode(dep, () =>
                 {
                     FocusDev(Local_DevID);
                 });
            }
            else
            {
                Debug.LogError("RoomFactory.FoucusDev,Dep is null:" + depId);
            }
        }
        else
        {
            FocusDev(Local_DevID);
        }
    }
    /// <summary>
    /// 聚焦设备 int类型ID
    /// </summary>
    /// <param name="devId"></param>
    private void FocusDev(int devId)
    {
        GetDevByid(devId, dev =>
        {
            if (dev) dev.FocusOn();
            else
                Debug.LogError("RoomFactory.FoucusDev,Dev is null :" + devId);
        });
    }
    /// <summary>
    /// 聚焦设备s
    /// </summary>
    /// <param name="devId"></param>
    private void FocusDev(string Local_DevID)
    {
        GetDevById(Local_DevID, dev =>
        {
            if (dev) dev.FocusOn();
            else
                Debug.LogError("RoomFactory.FoucusDev,Dev is null :" + Local_DevID);
        });
    }
    #endregion
}
