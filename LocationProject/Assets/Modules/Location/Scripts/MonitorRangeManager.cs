using Location.WCFServiceReferences.LocationServices;
using RTEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Types = Location.WCFServiceReferences.LocationServices.AreaTypes;

/// <summary>
/// 监控范围（区域范围）
/// </summary>
namespace MonitorRange
{
    public class MonitorRangeManager : MonoBehaviour
    {
        private static MonitorRangeManager _instance;
        public static MonitorRangeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<MonitorRangeManager>();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        /// <summary>
        /// 显示范围Render
        /// </summary>
        public bool isShowRangeRender = true;
        /// <summary>
        /// 监控范围列表
        /// </summary>
        private List<MonitorRangeObject> rangelist;
        /// <summary>
        /// 区域预设
        /// </summary>
        public GameObject areaPrefab;
        /// <summary>
        /// 区域飘浮文字UI;
        /// </summary>
        public GameObject NameUI;
        /// <summary>
        /// 区块物体父物体
        /// </summary>
        private Transform areasParent;
        /// <summary>
        /// 根区域范围节点
        /// </summary>
        public RangeNode rootRangeNode;
        ///// <summary>
        ///// 区域信息根节点
        ///// </summary>
        //public DepNode rootDepNode;
        /// <summary>
        /// 是否是区域编辑状态
        /// </summary>
        public bool IsEditState;
        ///// <summary>
        ///// 是否显示告警区域
        ///// </summary>
        //private bool isShowAlarmArea;
        ///// <summary>
        ///// 是否显示告警区域
        ///// </summary>
        //public bool IsShowAlarmArea
        //{
        //    get
        //    {
        //        return isShowAlarmArea;
        //    }

        //    set
        //    {
        //        isShowAlarmArea = value;
        //    }
        //}

        /// <summary>
        /// 创建区域结束
        /// </summary>
        public bool IsCreateAreaComplete
        {
            get
            {
                return isCreateAreaComplete;
            }

            set
            {
                isCreateAreaComplete = value;
            }
        }

        /// <summary>
        /// 创建区域结束
        /// </summary>
        private bool isCreateAreaComplete;

        // Use this for initialization
        void Start()
        {
            Instance = this;
            rangelist = new List<MonitorRangeObject>();
            //ShowAreas();

            SceneEvents.DepNodeChanged += SceneEventsDepNodeChanged;
            SceneEvents.BuildingOpenStartAction += SceneEvents_BuildingOpenStartAction;
            SceneEvents.BuildingOpenCompleteAction += SceneEvents_BuildingOpenCompleteAction;
            SceneEvents.BuildingStartCloseAction += SceneEvents_BuildingStartCloseAction;
            SceneEvents.BuildingCloseCompleteAction += SceneEvents_BuildingCloseCompleteAction;
        }

        /// <summary>
        /// 大楼开始合拢监听方法
        /// </summary>
        private void SceneEvents_BuildingStartCloseAction()
        {
            Debug.Log("大楼开始合拢了！");
            HideAllRanges();
        }

        /// <summary>
        /// 大楼开始展开监听方法
        /// </summary>
        private void SceneEvents_BuildingOpenStartAction()
        {
            Debug.Log("大楼开始展开了！");
            //HideAllRanges();
            HideOtherRanges();
        }

        /// <summary>
        /// 大楼合拢完毕监听方法
        /// </summary>
        private void SceneEvents_BuildingCloseCompleteAction()
        {
            Debug.Log("大楼合拢完毕了！");

            //else
            //{
            FilterRangesByMRObject(SceneEvents.DepNode);
            //}
            if (IsEditState)
            {
                ShowAreaEdit(SceneEvents.DepNode);
            }
            SetMonitorRangeFollowPosition(FactoryDepManager.currentDep);
        }

        /// <summary>
        /// 大楼展开完毕监听方法
        /// </summary>
        private void SceneEvents_BuildingOpenCompleteAction(BuildingController building)
        {
            Debug.Log("大楼展开了完毕！");
            //else
            //{
            FilterRangesByMRObject(SceneEvents.DepNode);
            //}

            if (IsEditState)
            {
                ShowAreaEdit(SceneEvents.DepNode);
            }
            if (SceneEvents.DepNode.monitorRangeObject)
                SceneEvents.DepNode.monitorRangeObject.SetRendererEnable(false);//把大楼的区域范围隐藏掉
            SetMonitorRangeFollowPosition(FactoryDepManager.currentDep);
        }

        private void SceneEventsDepNodeChanged(DepNode arg1, DepNode arg2)
        {
            //Log.Info("MonitorRangeManager.SceneEvents_DepModeChanged");
            Debug.Log("SceneEventsDepNodeChanged！");
            if (arg1 != null)
            {
                Log.Info("LastDep:" + arg1);
            }
            if (arg2 != null)
            {
                Log.Info("CurrentDep:" + arg2);
                //else
                //{
                if (ActionBarManage.Instance.CurrentState == ViewState.人员定位)
                {
                    ShowRanges(arg2);
                    if (IsEditState)
                    {
                        ShowAreaEdit(arg2);
                    }
                }
                //}

            }
        }

        Transform rootparent;

        /// <summary>
        /// 创建所有区域范围
        /// </summary>
        public void CreateAllRanges()
        {
            DepNode rootDepNode = FactoryDepManager.Instance;
            PhysicalTopology topoNode = rootDepNode.TopoNode;
            rootparent = CreateRangeParent(); //区域根物体
            rootRangeNode = new RangeNode();
            CreateRangesByRootNode(topoNode, rootRangeNode);
            if (!isShowRangeRender)
            {
                HideAllRanges(true);
            }
            MonitorRangeObject[] rangeObjs = GetMonitorRangeObjectList();
            rangelist = new List<MonitorRangeObject>(rangeObjs);
            isCreateAreaComplete = true;
        }

        public void AddRangeToList(MonitorRangeObject o)
        {
            rangelist.Add(o);
        }

        /// <summary>
        /// 创建定位区域
        /// create by cww
        /// </summary>
        /// <param name="node"></param>
        public void CreateRanges(DepNode node)
        {

            //Log.Info("CreateChilrenRange", node.ToString());
            //ClearRanges();
            PhysicalTopology topoNode = node.TopoNode;
            if (topoNode == null)
            {
                Log.Alarm("CreateChilrenRange", "topoNode == null");
                return;
            }
            if (node.depType == DepType.Factory)//厂区
            {
                //CreateRanges(FilterParkMonitorRange(topoNode.Children));
                Transform pparent = CreateRangeParent(); //区域根物体
                rootRangeNode = new RangeNode();
                CreateRangesByRootNode(topoNode, rootRangeNode);
            }
            else if (topoNode.Type == Types.楼层)
            {
                //CreateChilrenRange_Floor(node);
            }
            else if (topoNode.Type == Types.大楼)
            {
                //CreateChilrenRange_Building(node);
            }
            else
            {
                Log.Alarm("CreateChilrenRange", "不是楼层和厂区");
            }
        }

        /// <summary>
        /// 创建所有区域范围，根据顶部节点（递归）
        /// </summary>
        /// <param name="newArea">数据节点</param>
        /// <param name="newNode">区域范围节点</param>
        /// <param name="pparentT">父物体</param>
        public void CreateRangesByRootNode(PhysicalTopology newArea, RangeNode newNode)
        {
            if (newArea == null)
            {
                Debug.LogError("MonitorRangeManager.CreateRangesByRootNode: newArea == null");
                return;
            }
            if (newNode == null)
            {
                Debug.LogError("MonitorRangeManager.CreateRangesByRootNode: newNode == null");
                return;
            }
            newNode.SetInfo(newArea);
            MonitorRangeObject thisRangeObject = null;
            if (newArea.Type != Types.分组 && newArea.Type != Types.CAD)
            {
                thisRangeObject = CreateRange(newArea, newNode); //创建自身区域范围
            }

            Transform content = null;
            if (thisRangeObject == null)
            {
                //content = CreateRangeParent(pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name, pparent); //创建父物体
                string pName = newArea.Type.ToString() + "(" + newArea.Id + ")" + ":" + newArea.Name;
                GameObject o = new GameObject(pName); ;
                o.transform.SetParent(rootparent);
                content = o.transform;
            }
            else
            {
                content = thisRangeObject.transform;
                thisRangeObject.name = newArea.Type.ToString() + "(" + newArea.Id + ")" + ":" + newArea.Name;
            }

            if (newArea.Children != null)
            {
                foreach (PhysicalTopology child in newArea.Children)
                {
                    RangeNode subNode = new RangeNode();
                    subNode.parentNode = newNode;
                    newNode.subNodes.Add(subNode);

                    CreateRangesByRootNode(child, subNode);//递归

                }
            }
        }

        ///// <summary>
        ///// 创建所有区域范围根据顶部节点（递归）
        ///// </summary>
        ///// <param name="pNode">数据节点</param>
        ///// <param name="rangeNode">区域范围节点</param>
        ///// <param name="pparentT">父物体</param>
        //public void CreateRangesByRootNode(PhysicalTopology pNode, RangeNode rangeNode, Transform pparentT)
        //{
        //    Transform pparent = pparentT; //区域根物体
        //    //Transform content = CreateRangeParent(pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name, pparent); //创建父物体
        //    Transform content = null;
        //    rangeNode.info = pNode;
        //    rangeNode.ranges = new List<MonitorRangeObject>();
        //    rangeNode.subNodes = new List<RangeNode>();
        //    //if (pNode.Name.Contains("集控楼"))
        //    //{
        //    //    int i = 0;
        //    //}
        //    MonitorRangeObject thisRangeObject = CreateRange(pNode, pparent); //创建自身区域范围
        //    if (thisRangeObject == null)
        //    {
        //        content = CreateRangeParent(pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name, pparent); //创建父物体
        //    }
        //    else
        //    {

        //        content = thisRangeObject.transform;
        //        thisRangeObject.name = pNode.Type.ToString() + "(" + pNode.Id + ")" + ":" + pNode.Name;
        //        if (!pNode.Transfrom.IsRelative)
        //        {
        //            //float posx = LocationManager.Instance.axisZero.x * thisRangeObject.transform.parent.lossyScale.x;
        //            //float posy = LocationManager.Instance.axisZero.y * thisRangeObject.transform.parent.lossyScale.y;
        //            //float posz = LocationManager.Instance.axisZero.z * thisRangeObject.transform.parent.lossyScale.z;
        //            //Vector3 posT = new Vector3(posx, posy, posz);
        //            float posx = thisRangeObject.transform.localPosition.x * thisRangeObject.transform.parent.lossyScale.x;
        //            float posy = thisRangeObject.transform.localPosition.y * thisRangeObject.transform.parent.lossyScale.y;
        //            float posz = thisRangeObject.transform.localPosition.z * thisRangeObject.transform.parent.lossyScale.z;
        //            Vector3 posT = new Vector3(posx, posy, posz);
        //            thisRangeObject.transform.position = LocationManager.Instance.axisZero + posT;
        //        }
        //        else
        //        {
        //            if (rangeNode.parentNode != null)
        //            {
        //                PhysicalTopology buldingNode = rangeNode.parentNode.info;
        //                TransformM tm = buldingNode.Transfrom;
        //                if (tm != null)
        //                {
        //                    //Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + pNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
        //                    Vector3 pos2D = new Vector3((float)(-tm.SX / 2f), (float)(-tm.SY / 2), (float)(-tm.SZ / 2));//建筑物的右下角坐标
        //                    //Log.Info("建筑物的右下角坐标:" + pos2D);
        //                    Vector3 buildPos = LocationManager.GetRealSizeVector(pos2D);
        //                    //content.transform.localPosition = buildPos;
        //                    float posx = buildPos.x / thisRangeObject.transform.parent.lossyScale.x;
        //                    float posy = buildPos.y / thisRangeObject.transform.parent.lossyScale.y;
        //                    float posz = buildPos.z / thisRangeObject.transform.parent.lossyScale.z;
        //                    Vector3 posT = new Vector3(posx, posy, posz);
        //                    thisRangeObject.transform.localPosition += posT;

        //                }
        //            }
        //        }
        //        rangeNode.thisRange = thisRangeObject; //创建自身区域范围
        //    }

        //    if (pNode.Children != null)
        //    {
        //        foreach (PhysicalTopology child in pNode.Children)
        //        {
        //            RangeNode subNode = new RangeNode();
        //            subNode.parentNode = rangeNode;
        //            rangeNode.subNodes.Add(subNode);
        //            CreateRangesByRootNode(child, subNode, content);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 创建子区域范围
        ///// </summary>
        ///// <param name="node"></param>
        //public void CreateSubRanges(DepNode node)
        //{
        //    if (node == null) return;

        //}

        ///// <summary>
        ///// 创建大楼里面所有的范围区域
        ///// </summary>
        ///// <param name="node"></param>
        //private void CreateChilrenRange_Building(DepNode node)
        //{
        //    LocationManager.LoadTransferOfAxesConfig();

        //    PhysicalTopology topoNode = node.TopoNode;
        //    if (topoNode == null) return;
        //    if (topoNode.Children == null)
        //    {
        //        Log.Alarm("CreateChilrenRange_Floor", "topoNode.Children == null");
        //        return;
        //    }
        //    Transform root = CreateRangeParent(); //区域根物体
        //                                          //MonitorRangeObject rootRange=CreateRange(topoNode, root);
        //    Transform parent = CreateRangeParent(topoNode.Id + ":" + topoNode.Name, root); //创建父物体

        //    //List<MonitorRangeObject> ranges = new List<MonitorRangeObject>();
        //    foreach (PhysicalTopology child in topoNode.Children)//房间
        //    {
        //        Log.Info("CreateChild", child.ToString());
        //        if (child.Type == Types.楼层)
        //        {
        //            //MonitorRangeObject rangeObject = CreateRange(child, parent); //创建子区域
        //            //if (rangeObject == null) continue;
        //            //rangeObject.transform.localPosition -= LocationManager.Instance.axisZero;
        //            ////ranges.Add(rangeObject);

        //            //rangelist.Add(rangeObject);

        //            CreateChilrenRange_BuildingFloor(child, parent);
        //        }
        //    }


        //}

        //private void CreateChilrenRange_BuildingFloor(PhysicalTopology pnode, Transform p_parent = null)
        //{
        //    PhysicalTopology topoNode = pnode;
        //    if (topoNode == null) return;
        //    if (topoNode.Children == null)
        //    {
        //        Log.Alarm("CreateChilrenRange_Floor", "topoNode.Children == null");
        //        return;
        //    }

        //    if (p_parent == null)
        //    {
        //        p_parent = CreateRangeParent(); //区域根物体
        //    }
        //    //MonitorRangeObject rootRange=CreateRange(topoNode, root);
        //    Transform parent = CreateRangeParent(topoNode.Id + ":" + topoNode.Name, p_parent); //创建父物体

        //    List<MonitorRangeObject> ranges = new List<MonitorRangeObject>();
        //    foreach (PhysicalTopology child in topoNode.Children)//房间
        //    {
        //        Log.Info("CreateChild", child.ToString());

        //        MonitorRangeObject rangeObject = CreateRange(child, parent); //创建子区域
        //        if (rangeObject == null) continue;
        //        rangeObject.transform.localPosition -= LocationManager.Instance.axisZero;
        //        ranges.Add(rangeObject);

        //        rangelist.Add(rangeObject);
        //    }

        //    PhysicalTopology buldingNode = pnode;
        //    TransformM tm = buldingNode.Transfrom;
        //    if (tm != null)
        //    {
        //        Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + topoNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
        //        Log.Info("建筑物的右下角坐标:" + pos2D);
        //        Vector3 buildPos = LocationManager.GetRealVector(pos2D);
        //        parent.localPosition = buildPos;
        //    }

        //}

        //private void CreateChilrenRange_Floor(DepNode node)
        //{
        //    LocationManager.LoadTransferOfAxesConfig();

        //    PhysicalTopology topoNode = node.TopoNode;
        //    if (topoNode == null) return;
        //    if (topoNode.Children == null)
        //    {
        //        Log.Alarm("CreateChilrenRange_Floor", "topoNode.Children == null");
        //        return;
        //    }
        //    Transform root = CreateRangeParent(); //区域根物体
        //                                          //MonitorRangeObject rootRange=CreateRange(topoNode, root);
        //    Transform parent = CreateRangeParent(topoNode.Id + ":" + topoNode.Name, root); //创建父物体

        //    List<MonitorRangeObject> ranges = new List<MonitorRangeObject>();
        //    foreach (PhysicalTopology child in topoNode.Children)//房间
        //    {
        //        Log.Info("CreateChild", child.ToString());

        //        MonitorRangeObject rangeObject = CreateRange(child, parent); //创建子区域
        //        if (rangeObject == null) continue;
        //        rangeObject.transform.localPosition -= LocationManager.Instance.axisZero;
        //        ranges.Add(rangeObject);

        //        rangelist.Add(rangeObject);
        //    }

        //    PhysicalTopology buldingNode = node.ParentNode.TopoNode;
        //    TransformM tm = buldingNode.Transfrom;
        //    if (tm != null)
        //    {
        //        Vector3 pos2D = new Vector3((float)(tm.X - tm.SX / 2f), (float)(tm.Y - tm.SY / 2 + topoNode.Transfrom.SY), (float)(tm.Z - tm.SZ / 2));//建筑物的右下角坐标
        //        Log.Info("建筑物的右下角坐标:" + pos2D);
        //        Vector3 buildPos = LocationManager.GetRealVector(pos2D);
        //        parent.localPosition = buildPos;
        //    }
        //    //Vector3 size = parent.gameObject.GetLocalSize();
        //    //parent.localPosition = parent.localPosition - LocationManager.Instance.axisZero +
        //    //                       new Vector3(size.x/2, 0, size.z/2); //偏移位置

        //    //foreach (MonitorRangeObject range in ranges)
        //    //{
        //    //    range.transform.localPosition += new Vector3(size.x / 2, 0, size.z / 2);
        //    //}
        //}



        void OnDestroy()
        {
            SceneEvents.DepNodeChanged -= SceneEventsDepNodeChanged;
            SceneEvents.BuildingOpenCompleteAction -= SceneEvents_BuildingOpenCompleteAction;
            SceneEvents.BuildingCloseCompleteAction -= SceneEvents_BuildingCloseCompleteAction;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 显示区域
        /// </summary>
        [ContextMenu("ShowAreas")]
        public void ShowRanges(DepNode depNodeT)
        {
            //Log.Error("")
            if (depNodeT == null || FullViewController.Instance.IsFullView) return;
            if (depNodeT.depType == DepType.Factory)
            {
                //if (rootDepNode != null)
                if (isCreateAreaComplete)
                {
                    ShowAllRanges();
                }
                else
                {
                    CreateAllRanges();
                }
            }
            else
            {
                FilterRangesByMRObject(depNodeT);
            }
            //LocationManager.Instance.RefleshLocationAlarms();
            LocationManager.Instance.InitLocationAlarms();
        }

        /// <summary>
        /// 筛选园区下的监控范围
        /// </summary>
        /// isDeep,是否继续往子节点下面搜
        private List<PhysicalTopology> FilterParkMonitorRange(IEnumerable<PhysicalTopology> listT)
        {
            List<PhysicalTopology> ps = new List<PhysicalTopology>();
            if (listT == null)
            {
                return ps;
            }
            foreach (PhysicalTopology p in listT)
            {
                if (p.Type == Types.区域 || p.Type == Types.分组 || p.Type == Types.大楼 || p.Type == Types.范围)
                {
                    if (p.Transfrom != null)
                    {
                        ps.Add(p);
                    }
                    ps.AddRange(FilterParkMonitorRange(p.Children));
                }
            }
            return ps;
        }

        ///// <summary>
        ///// 获取园区下监控范围
        ///// </summary>
        //[ContextMenu("GetParkRangesInfo")]
        //public List<PhysicalTopology> GetParkRangesInfo()
        //{
        //    IList<PhysicalTopology> listT = CommunicationObject.Instance.GetParkMonitorRange();
        //    List<PhysicalTopology> infolist = new List<PhysicalTopology>(listT);
        //    return infolist;
        //}

        ///// <summary>
        ///// 创建监控范围
        ///// </summary>
        //public void CreateRanges(IEnumerable<PhysicalTopology> infolist)
        //{
        //    Log.Info("CreateRanges");
        //    Transform parent = CreateRangeParent();
        //    if (infolist == null) return;
        //    foreach (PhysicalTopology topoNode in infolist)
        //    {
        //        MonitorRangeObject rangeObject = CreateRange(topoNode, parent);
        //        if (rangeObject != null)
        //            rangelist.Add(rangeObject);
        //        //}
        //    }
        //}

        public MonitorRangeObject CreateRange(PhysicalTopology newArea, RangeNode newRangeNode)
        {
            if (newArea == null)
            {
                Log.Alarm("MonitorRangeManager.CreateRange", "newArea == null:"+newArea.Name);
                return null;
            }

            //if (newArea.Transfrom == null) //2019_09_30_cww:为了能够在没有设定范围的园区下创建区域，删除这个
            //{
            //    Log.Alarm("MonitorRangeManager.CreateRange", "newArea.Transfrom == null:" + newArea.Name);
            //    return null;
            //}

            //Log.Info("MonitorRangeManager.CreateRange", string.Format("CreateRange：{0},({1},{2},{3})", newArea.Name, newArea.Transfrom.X, newArea.Transfrom.Y, newArea.Transfrom.Z));

            DepNode areaDepNode = RoomFactory.Instance.GetDepNodeByTopo(newArea);//找到该区域数据（PhysicalTopology）对应的（三维）区域节点
            MonitorRangeObject rangeObject = rangelist.Find((item) => item.info.Id == newArea.Id);
            if (!rangeObject)
            {
                //新建
                //GameObject o = CreateAreaObject(parent);
                GameObject o = CreateAreaObject();//创建预设
                o.name = GetTopoNodeObjectName(newArea);
                o.SetLayerAll(Layers.IgnoreRaycast);//Layers.Range
                MonitorRangeObject newMonitorRange = o.AddComponent<MonitorRangeObject>();
                newMonitorRange.Init(newArea, newRangeNode, areaDepNode);//应该是在这里设置位置信息的
                rangeObject = newMonitorRange;
                o.SetActive(true);
            }
            else
            {
                //已经存在
                rangeObject.Init(newArea, newRangeNode, areaDepNode);//应该是在这里设置位置信息的
                rangeObject.gameObject.SetActive(true);
            }
            return rangeObject;
        }


        private string GetTopoNodeObjectName(PhysicalTopology p)
        {
            return string.Format("{0}:{1}", p.Id, p.Name);
        }


        ///// <summary>
        ///// 清除监控范围
        ///// </summary>
        //public void ClearRanges()
        //{
        //    foreach (MonitorRangeObject range in rangelist)
        //    {
        //        if (range != null)
        //        {
        //            DestroyImmediate(range.gameObject);
        //        }
        //    }

        //    rangelist.Clear();
        //}

        /// <summary>
        /// 创建区域物体
        /// </summary>
        /// <returns></returns>
        public GameObject CreateAreaObject()
        {
            GameObject o = Instantiate(areaPrefab);
            //o.transform.SetParent(parent);
            o.transform.SetParent(rootparent);
            //BoxCollider boxCollider = o.GetComponent<BoxCollider>();
            //if (boxCollider)
            //{
            //    boxCollider.isTrigger = true;
            //}
            return o;
        }

        /// <summary>
        /// 创建定位卡的空间父物体
        /// </summary>
        private static Transform CreateRangeParent(string pName, Transform parent)
        {
            Transform tParent = parent.Find(pName);
            if (tParent == null)
            {
                GameObject o = new GameObject(pName);
                tParent = o.transform;
                tParent.SetParent(parent);
                tParent.transform.localPosition = Vector3.zero;
            }
            return tParent;
        }

        /// <summary>
        /// 创建定位卡的空间父物体
        /// </summary>
        private Transform CreateRangeParent()
        {
            Transform tParent = CreateRangeParent("Areas Parent", transform);
            areasParent = tParent;
            return tParent;
        }

        /// <summary>
        /// 设置监控区域范围跟随位置
        /// </summary>
        public void SetMonitorRangeFollowPosition(DepNode depNodeT)
        {
            //MonitorRangeObject[] rangesT = o.GetComponentsInChildren<MonitorRangeObject>();
            //foreach (MonitorRangeObject rangeT in rangesT)
            //{
            //    rangeT.SetFollowTarget();
            //}
            if (depNodeT == null) return;
            if (depNodeT.monitorRangeObject != null)
            {
                depNodeT.monitorRangeObject.SetFollowTarget();
            }

            if (depNodeT.ChildNodes != null)
            {
                foreach (DepNode childNodeT in depNodeT.ChildNodes)
                {
                    //if (childNodeT.monitorRangeObject == null)
                    //{
                    //    continue;
                    //}
                    SetMonitorRangeFollowPosition(childNodeT);
                }
            }
        }

        /// <summary>
        /// 过滤区域范围根据区域范围节点,只显示当前区域范围节点下的子区域节点
        /// </summary>
        public void FilterRangesByMRObject(DepNode depNodeT)
        {
            if (depNodeT == null) return;
            //HideAllRanges();
            HideOtherRanges();
            FilterRangesByMRObjectop(depNodeT);

        }

        private void FilterRangesByMRObjectop(DepNode depNodeT)
        {
            if (depNodeT.name.Contains("调压站告警区域"))
            {
                int i = 0;
            }
            if (depNodeT.monitorRangeObject != null)
            {
                if (isShowRangeRender)
                {
                    depNodeT.monitorRangeObject.SetRendererEnable(true);
                }
                else
                {
                    //if (depNodeT.monitorRangeObject.isAlarming && MonitorRangeManager.Instance.IsShowAlarmArea)
                    if (depNodeT.monitorRangeObject.isAlarming)
                    {
                        depNodeT.monitorRangeObject.SetRendererEnable(true);
                    }
                }
                depNodeT.monitorRangeObject.SetColliderEnable(true);
            }

            if (depNodeT.ChildNodes != null)
            {
                foreach (DepNode childNodeT in depNodeT.ChildNodes)
                {
                    //if (childNodeT.monitorRangeObject == null)
                    //{
                    //    continue;
                    //}
                    FilterRangesByMRObjectop(childNodeT);
                }
            }

            //if (depNodeT == null || depNodeT.TopoNode == null)
            //{
            //    int i = 0;
            //}

            if (depNodeT.TopoNode != null && depNodeT.TopoNode.Type == Types.机房)//如果过滤的节点是机房类型，则与该节点兄弟节点的Collider不应该被关闭
            {
                if (depNodeT.ParentNode != null && depNodeT.ParentNode.ChildNodes != null)
                {
                    foreach (DepNode childNodeT in depNodeT.ParentNode.ChildNodes)
                    {
                        SetRangeCollider(childNodeT, true);
                    }
                }
            }
        }

        /// <summary>
        /// 设置范围的碰撞器是否启用
        /// </summary>
        public void SetRangeCollider(DepNode depNodeT, bool isEnable)
        {
            if (depNodeT.monitorRangeObject != null)
            {
                depNodeT.monitorRangeObject.SetColliderEnable(isEnable);
            }

            if (depNodeT.ChildNodes != null)
            {
                foreach (DepNode childNodeT in depNodeT.ChildNodes)
                {
                    SetRangeCollider(childNodeT, isEnable);
                }
            }
        }

        private MonitorRangeObject[] GetMonitorRangeObjectList()
        {
            if (areasParent == null)
            {
                Debug.LogError("MonitorRangeManager.GetMonitorRangeObjectList areasParent == null");
                return new MonitorRangeObject[0];
            }
            MonitorRangeObject[] rangeObjs = areasParent.GetComponentsInChildren<MonitorRangeObject>();
            return rangeObjs;
        }

        /// <summary>
        /// 显示所有区域范围
        /// </summary>
        public void ShowAllRanges()
        {
            MonitorRangeObject[] rangeObjs = GetMonitorRangeObjectList();
            foreach (MonitorRangeObject obj in rangeObjs)
            {
                if (obj.name.Contains("调压站告警区域"))
                {
                    int i = 0;
                }
                if (isShowRangeRender)
                {
                    obj.SetRendererEnable(true);
                }
                else
                {
                    //if (obj.isAlarming && MonitorRangeManager.Instance.IsShowAlarmArea)
                    if (obj.isAlarming)
                    {
                        obj.SetRendererEnable(true);
                    }
                }
                obj.SetColliderEnable(true);
            }
        }

        /// <summary>
        /// 隐藏所有区域范围
        /// </summary>
        /// <param name="isCollider">设置Collder状态</param>
        public void HideAllRanges(bool isCollider = false)
        {
            MonitorRangeObject[] rangeObjs = GetMonitorRangeObjectList();
            foreach (MonitorRangeObject obj in rangeObjs)
            {
                obj.SetRendererEnable(false);
                if (!isCollider)
                {
                    obj.SetColliderEnable(false);
                }
            }
        }

        /// <summary>
        /// 隐藏其他区域范围,根据当前区域
        /// </summary>
        private void HideOtherRanges()
        {
            MonitorRangeObject[] rangeObjs = GetMonitorRangeObjectList();
            foreach (MonitorRangeObject obj in rangeObjs)
            {
                if (!IsBelongtoCurrentDep(obj.depNode))
                {
                    obj.SetRendererEnable(false);
                    obj.SetColliderEnable(false);
                }
            }
        }

        ///// <summary>
        ///// 设置告警区域
        ///// </summary>
        ///// <param name="b"></param>
        //public void SetIsAlarmArea(bool b)
        //{
        //    IsShowAlarmArea = b;
        //}

        /// <summary>
        /// 显示告警区域
        /// </summary>
        public void ShowAlarmArea()
        {
            //IsShowAlarmArea = true;
            ////MonitorRangeObject[] rangeObjs = GetMonitorRangeObjectList();
            ////foreach (MonitorRangeObject obj in rangeObjs)
            ////{
            ////    if (obj.IsOnAlarmArea)
            ////    {
            ////        obj.SetRendererEnable(true);
            ////    }
            ////}

            //FilterRangesByMRObject(SceneEvents.DepNode);
        }

        /// <summary>
        /// 隐藏告警区域
        /// </summary>
        public void HideAlarmArea()
        {
            //IsShowAlarmArea = false;
            //MonitorRangeObject[] rangeObjs = GetMonitorRangeObjectList();
            //foreach (MonitorRangeObject obj in rangeObjs)
            //{
            //    if (obj.isAlarming && obj.alarmPersons.Count == 0)
            //    {
            //        obj.SetRendererEnable(false);
            //    }
            //}
        }

        /// <summary>
        /// 获取MonitorRangeObject根据区域id
        /// </summary>
        /// <param name="areaId"></param>
        public MonitorRangeObject GetMonitorRangeObjectByAreaId(int areaId)
        {
            try
            {
                //MonitorRangeObject m = rangelist.Find((i) => i.depNode.TopoNode.Id == areaId);
                MonitorRangeObject m = rangelist.Find((i) => i.info.Id == areaId);
                return m;
            }
            catch
            {
                return null;
            }

            //foreach (MonitorRangeObject m in rangelist)
            //{

            //}
            //rootDepNode.TopoNode.
        }

        #region 区域编辑

        /// <summary>
        /// 当前编辑区域的建筑节点
        /// </summary>
        public DepNode currentEditDepNode;

        /// <summary>
        /// 展示区域可编辑
        /// </summary>
        public void ShowAreaEdit(DepNode depnodeT)
        {
            //ObjectsEditManage.Instance.SetEditorGizmoSystem(true);
            SetAreaEditEnable(currentEditDepNode, false);//清除当前编辑
            SetAreaEditEnable(depnodeT, true);
            SetEditorObjectSelection(true);
        }

        /// <summary>
        /// 设置当前建筑节点下的区域（类型为范围，机房）是否可编辑
        /// </summary>
        /// <param name="depnodeT">建筑节点</param>
        public void SetAreaEditEnable(DepNode depnodeT, bool isEnable)
        {
            if (depnodeT != null)
            {
                IsEditState = isEnable;
                if (isEnable)
                {
                    currentEditDepNode = depnodeT;
                }
                else
                {
                    currentEditDepNode = null;
                }
                SetAreaEditEnableOP(depnodeT, isEnable);
            }
        }

        private static void SetAreaEditEnableOP(DepNode depnodeT, bool isEnable)
        {
            if (depnodeT.ChildNodes != null)
            {
                foreach (DepNode childNodeT in depnodeT.ChildNodes)
                {
                    if (childNodeT.TopoNode == null)
                    {
                        continue;
                    }
                    if (childNodeT.IsRoom())
                    {
                        if (childNodeT.monitorRangeObject == null) continue;

                        childNodeT.monitorRangeObject.SetEditEnable(isEnable);
                        //if (childNodeT.monitorRangeObject.isAlarming && MonitorRangeManager.Instance.IsShowAlarmArea && !isEnable)
                        if (childNodeT.monitorRangeObject.isAlarming && !isEnable)
                        {
                        }
                        else
                        {
                            childNodeT.monitorRangeObject.SetRendererEnable(isEnable);
                        }
                    }
                    else if (childNodeT.TopoNode.Type == Types.分组)
                    {
                        //foreach (DepNode cchildNodeT in childNodeT.ChildNodes)
                        //{
                        SetAreaEditEnableOP(childNodeT, isEnable);
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// 退出当前的区域编辑
        /// </summary>
        public void ExitCurrentEditArea()
        {
            //ObjectsEditManage.Instance.SetEditorGizmoSystem(false);
            SetAreaEditEnable(currentEditDepNode, false);//清除当前编辑
            SetEditorObjectSelection(false);
            EditorObjectSelection.Instance.ClearSelection(false);
        }

        /// <summary>
        /// 设置EditorObjectSelection
        /// </summary>
        public void SetEditorObjectSelection(bool isEditAreaT)
        {
            if (isEditAreaT)
            {
                EditorObjectSelection.Instance.ObjectSelectionSettings.CanMultiSelect = false;
                //RotationGizmo.
                EditorGizmoSystem.Instance.RotationGizmo.SetAxisVisibility(false, 0);
                EditorGizmoSystem.Instance.RotationGizmo.SetAxisVisibility(false, 2);
                EditorGizmoSystem.Instance.RotationGizmo.ShowRotationSphere = false;
                EditorGizmoSystem.Instance.RotationGizmo.ShowCameraLookRotationCircle = false;

            }
            else
            {
                EditorObjectSelection.Instance.ObjectSelectionSettings.CanMultiSelect = true;
                //RotationGizmo.
                EditorGizmoSystem.Instance.RotationGizmo.SetAxisVisibility(true, 0);
                EditorGizmoSystem.Instance.RotationGizmo.SetAxisVisibility(true, 2);
                EditorGizmoSystem.Instance.RotationGizmo.ShowRotationSphere = true;
                EditorGizmoSystem.Instance.RotationGizmo.ShowCameraLookRotationCircle = true;
            }
        }

        #endregion


        #region 计算多边形到点最近距离的点的位置坐标

        /// <summary>
        /// 计算多边形到点最短距离的点的位置坐标（只适用于二维）
        /// </summary>
        /// <param name="point">位置点</param>
        /// <param name="verts">多边形顶点集合（顺，逆时针都可以）</param>
        /// <returns></returns>
        public static Vector2 PointForPointToPolygon(Vector2 point, List<Vector2> verts)
        {
            if (verts == null || verts.Count == 0)
            {
                return point;
            }

            if (verts.Count == 1)
            {
                return verts[0];
            }

            if (verts.Count == 2)
            {
                Vector2 vt = PointForPointToABLine(point.x, point.y, verts[0].x, verts[0].y, verts[1].x, verts[1].y);
                return verts[0];
            }

            //Dictionary< int, Vector2> distanceDic = new Dictionary<int, Vector2>();//该边到位置点最短距离的点和边的编号
            List<Vector2> distanceList = new List<Vector2>();

            for (int i = 0; i < verts.Count; i++)
            {
                if (i < verts.Count - 1)
                {
                    Vector2 vt = PointForPointToABLine(point.x, point.y, verts[i].x, verts[i].y, verts[i + 1].x, verts[i + 1].y);
                    distanceList.Add(vt);
                }
                else
                {
                    Vector2 vt = PointForPointToABLine(point.x, point.y, verts[i].x, verts[i].y, verts[0].x, verts[0].y);
                    distanceList.Add(vt);
                }
            }

            float dis = 100000f;
            Vector2 v = point;
            foreach (Vector2 p in distanceList)
            {
                float disT = Vector2.Distance(p, point);
                if (disT < dis)
                {
                    dis = disT;
                    v = p;
                }
            }

            return v;
        }

        /// <summary>
        ///  点到线段最短距离的那条直线与线段的交点，{x=...,y=...}（只适用于二维）
        /// </summary>
        /// <param name="x">线段外的点的x坐标</param>
        /// <param name="y">线段外的点的y坐标</param>
        /// <param name="x1">线段顶点1的x坐标</param>
        /// <param name="y1">线段顶点1的y坐标</param>
        /// <param name="x2">线段顶点2的x坐标</param>
        /// <param name="y2">线段顶点2的y坐标</param>
        /// <returns></returns>
        public static Vector2 PointForPointToABLine(float x, float y, float x1, float y1, float x2, float y2)
        {
            Vector2 reVal = new Vector2();
            // 直线方程的两点式转换成一般式
            // A = Y2 - Y1
            // B = X1 - X2
            // C = X2*Y1 - X1*Y2
            float a1 = y2 - y1;
            float b1 = x1 - x2;
            float c1 = x2 * y1 - x1 * y2;
            float x3, y3;
            if (a1 == 0)
            {
                // 线段与x轴平行
                reVal = new Vector2(x, y1);
                x3 = x;
                y3 = y1;
            }
            else if (b1 == 0)
            {
                // 线段与y轴平行
                reVal = new Vector2(x1, y);
                x3 = x1;
                y3 = y;
            }
            else
            {
                // 普通线段
                float k1 = -a1 / b1;
                float k2 = -1 / k1;
                float a2 = k2;
                float b2 = -1;
                float c2 = y - k2 * x;
                // 直线一般式和二元一次方程的一般式转换
                // 直线的一般式为 Ax+By+C=0
                // 二元一次方程的一般式为 Ax+By=C
                c1 = -c1;
                c2 = -c2;

                // 二元一次方程求解(Ax+By=C)
                // a=a1,b=b1,c=c1,d=a2,e=b2,f=c2;
                // X=(ce-bf)/(ae-bd)
                // Y=(af-cd)/(ae-bd)
                x3 = (c1 * b2 - b1 * c2) / (a1 * b2 - b1 * a2);
                y3 = (a1 * c2 - c1 * a2) / (a1 * b2 - b1 * a2);
            }
            // 点(x3,y3)作为点(x,y)到(x1,y1)和(x2,y2)组成的直线距离最近的点,那(x3,y3)是否在(x1,y1)和(x2,y2)的线段之内(包含(x1,y1)和(x2,y2))
            if (((x3 > x1) != (x3 > x2) || x3 == x1 || x3 == x2) && ((y3 > y1) != (y3 > y2) || y3 == y1 || y3 == y2))
            {
                // (x3,y3)在线段上
                reVal = new Vector2(x3, y3);
            }
            else
            {
                // (x3,y3)在线段外
                float d1_quadratic = (x - x1) * (x - x1) + (y - y1) * (y - y1);
                float d2_quadratic = (x - x2) * (x - x2) + (y - y2) * (y - y2);
                if (d1_quadratic <= d2_quadratic)
                {
                    reVal = new Vector2(x1, y1);
                }
                else
                {
                    reVal = new Vector2(x2, y2);
                }
            }
            return reVal;
        }
        #endregion

        #region 针对主厂房区域的特殊处理

        public DepNode ZhuchangfangFirstDepNode;
        public DepNode ZhuchangfangSecordDepNode;

        /// <summary>
        /// 对主厂房的人员进行特殊处理，定位高度要是大于2，就返回主厂房二层
        /// </summary>
        public DepNode GetDoZhuchangfang(DepNode depnodeT, float y)
        {
            InitZhuchangfang();
            bool b = GetZhuchangfangDepNode(depnodeT);
            if (b)
            {
                if (y > 2f)
                {
                    //Debug.LogErrorFormat("楼层：主厂房4.5m层，  高度：{0}", y);
                    bool isbool = IsBelongDepNodeByName("主厂房4.5m层", depnodeT);
                    if (isbool)
                    {
                        return depnodeT;
                    }
                    return ZhuchangfangSecordDepNode;
                }
                else
                {
                    //Debug.LogErrorFormat("楼层：主厂房0m层，  高度：{0}", y);
                    bool isbool = IsBelongDepNodeByName("主厂房0m层", depnodeT);
                    if (isbool)
                    {
                        return depnodeT;
                    }
                    return ZhuchangfangFirstDepNode;
                }
            }
            return null;
        }

        /// <summary>
        /// 针对主厂房做特殊处理
        /// </summary>
        public void InitZhuchangfang()
        {
            //if (ZhuchangfangFirstDepNode == null)
            //{
            //ZhuchangfangFirstDepNode = GetZhuchangfangFirstDepNode(rootDepNode);
            ZhuchangfangFirstDepNode = RoomFactory.Instance.GetDepNodeByName("主厂房0m层");
            //}

            //if (ZhuchangfangSecordDepNode == null)
            //{
            //ZhuchangfangSecordDepNode = GetZhuchangfangSecordDepNode(rootDepNode);
            ZhuchangfangSecordDepNode = RoomFactory.Instance.GetDepNodeByName("主厂房4.5m层");
            //}
        }

        /// <summary>
        /// 是否是主厂房下子区域
        /// </summary>
        public bool GetZhuchangfangDepNode(DepNode depnodeT)
        {
            if (depnodeT == null) return false;
            if (depnodeT.NodeName == "主厂房")
            {
                return true;
            }
            else
            {
                return GetZhuchangfangDepNode(depnodeT.ParentNode);
            }
        }

        /// <summary>
        /// 获取主厂房0m层区域
        /// </summary>
        public DepNode GetZhuchangfangFirstDepNode(DepNode depnodeT)
        {
            if (depnodeT == null) return null;
            if (depnodeT.NodeName == "主厂房0m层")
            {
                return depnodeT;
            }
            else
            {
                if (depnodeT.ChildNodes == null) return null;
                foreach (DepNode nodeT in depnodeT.ChildNodes)
                {
                    DepNode nodeTT = GetZhuchangfangFirstDepNode(nodeT);
                    if (nodeTT != null)
                    {
                        return nodeTT;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 获取主厂房4.5m层区域
        /// </summary>
        /// <returns></returns>
        public DepNode GetZhuchangfangSecordDepNode(DepNode depnodeT)
        {
            if (depnodeT == null) return null;
            if (depnodeT.NodeName == "主厂房4.5m层")
            {
                return depnodeT;
            }
            else
            {
                if (depnodeT.ChildNodes == null) return null;
                foreach (DepNode nodeT in depnodeT.ChildNodes)
                {
                    DepNode nodeTT = GetZhuchangfangSecordDepNode(nodeT);
                    if (nodeTT != null)
                    {
                        return nodeTT;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 判断是否属于某个区域的子区域
        /// </summary>
        /// <returns></returns>
        public bool IsBelongDepNodeByName(string depnodeName, DepNode depnodeT)
        {
            if (depnodeT == null) return false;
            if (depnodeT.NodeName == depnodeName)
            {
                return depnodeT;
            }
            else
            {
                return IsBelongDepNodeByName(depnodeName, depnodeT.ParentNode);
            }
        }

        ///// <summary>
        ///// 判断当前区域节点节点是否在当前区域下
        ///// </summary>
        ///// <param name="locationObjectT"></param>
        ///// <returns></returns>
        //public static bool IsBelongtoCurrentDep(DepNode depnodeT)
        //{
        //    if (depnodeT == null) return false;
        //    DepNode currentDepT = FactoryDepManager.currentDep;

        //    List<DepNode> depNodeListT = currentDepT.GetComponentsInChildren<DepNode>().ToList();
        //    if (depNodeListT.FindIndex((i) => i.NodeID == depnodeT.NodeID) > -1)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// 判断当前区域节点节点是否在当前区域下
        /// </summary>
        /// <returns></returns>
        public static bool IsBelongtoCurrentDep(DepNode depnodeT)
        {
            if (depnodeT == null || FactoryDepManager.currentDep == null) return false;
            if (depnodeT.NodeID == FactoryDepManager.currentDep.NodeID)
            {
                return depnodeT;
            }
            else
            {
                return IsBelongtoCurrentDep(depnodeT.ParentNode);
            }
        }

        /// <summary>
        /// 判断节点是否在楼层或楼层以下节点
        /// </summary>
        /// <returns></returns>
        public static bool IsBelongtoFloor(DepNode depnodeT)
        {
            if (depnodeT == null) return false;
            if (depnodeT.IsFloor())
            {
                return true;
            }
            else
            {
                return IsBelongtoCurrentDep(depnodeT.ParentNode);
            }
        }


        /// <summary>
        /// 判断当前人员是否在当前区域下，并执行相关操作
        /// </summary>
        /// <param name="locationObjectT"></param>
        /// <returns></returns>
        public static bool IsBelongtoCurrentDep(LocationObject locationObjectT)
        {
            List<DepNode> depNodeListT = FactoryDepManager.currentDep.GetComponentsInChildren<DepNode>().ToList();
            if (!depNodeListT.Contains(locationObjectT.currentDepNode))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 判断节点所在的大楼节点
        /// </summary>
        /// <returns></returns>
        public static DepNode GetDepNodeBuild(DepNode depnodeT)
        {
            if (depnodeT == null) return null;
            if (depnodeT.IsBuild())
            {
                return depnodeT;
            }
            else
            {
                return GetDepNodeBuild(depnodeT.ParentNode);
            }
        }

        #endregion
    }

    #region 范围节点类
    /// <summary>
    /// 范围节点类
    /// </summary>
    public class RangeNode
    {
        /// <summary>
        /// 节点信息
        /// </summary>
        public PhysicalTopology info;
        /// <summary>
        /// 父范围节点
        /// </summary>
        public RangeNode parentNode;
        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<RangeNode> subNodes;

        /// <summary>
        /// 该节点自身的区域范围
        /// </summary>
        public MonitorRangeObject rangeObject;
        /// <summary>
        /// 该节点下的区域范围物体集合
        /// </summary>
        public List<MonitorRangeObject> subRangeList;

        public RangeNode NewNode()
        {
            RangeNode node = new RangeNode();
            node.parentNode = this;
            if (this.subNodes == null)
            {
                this.subNodes = new List<RangeNode>();
            }
            this.subNodes.Add(node);

            node.subRangeList = new List<MonitorRangeObject>();
            node.subNodes = new List<RangeNode>();

            return node;
        }

        public void SetInfo(PhysicalTopology info)
        {
            this.info = info;
            this.subRangeList = new List<MonitorRangeObject>();
            this.subNodes = new List<RangeNode>();
        }

    }
    #endregion

}