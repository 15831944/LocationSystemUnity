using HighlightingSystem;
using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using MonitorRange;
using RTEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.UI;
using Types = Location.WCFServiceReferences.LocationServices.AreaTypes;

public class MonitorRangeObject : MonoBehaviour, IRTEditorEventListener
{
    /// <summary>
    /// 所属地图的id
    /// </summary>
    private int Id;
    /// <summary>
    /// 区域信息
    /// </summary>
    public DepNode depNode;
    /// <summary>
    /// 区域信息
    /// </summary>
    public PhysicalTopology info;
    /// <summary>
    /// RangeNode节点
    /// </summary>
    public RangeNode rangeNode;
    /// <summary>
    /// 目标位置
    /// </summary>
    private Vector3 targetPos;
    /// <summary>
    /// 目标角度
    /// </summary>
    private Vector3 targetAngles;
    /// <summary>
    /// 目标大小
    /// </summary>
    private Vector3 targetScale;
    /// <summary>
    /// 原始大小
    /// </summary>
    private Vector3 oriSize;
    /// <summary>
    /// 告警人员
    /// </summary>
    public List<LocationObject> alarmPersons;
    /// <summary>
    /// 定位人员
    /// </summary>
    public List<LocationObject> locationPersons;
    /// <summary>
    /// 跟随该建筑移动(该区域范围是在这个建筑下的)
    /// </summary>
    public GameObject followTarget;
    /// <summary>
    /// 跟随该建筑偏移量(该区域范围是在这个建筑下的)
    /// </summary>
    public Vector3 followOffset;

    //名称UI
    public GameObject followNameUI;

    public AreaNameUIController areaNameUIController;

    private float MinX;
    private float MaxX;
    private float MinY;
    private float MaxY;
    private float MinZ;
    private float MaxZ;

    private bool isUpdate;
    /// <summary>
    /// 是否更新过信息
    /// </summary>
    public bool IsUpdate
    {
        get
        {
            return isUpdate;
        }

        set
        {
            isUpdate = value;
        }
    }

    /// <summary>
    /// 碰撞器
    /// </summary>
    private Collider myCollider;
    /// <summary>
    /// 渲染器
    /// </summary>
    public Renderer render;

    /// <summary>
    /// 是否是普通区域
    /// </summary>
    public bool IsCreateAreaByData;

    /// <summary>
    /// 是否是告警区域范围
    /// </summary>
    public bool IsOnAlarmArea;

    /// <summary>
    /// 是否是定位区域
    /// </summary>
    public bool IsOnLocationArea;
    /// <summary>
    /// 创建区域范围的高度大小差的偏移
    /// </summary>
    public float yOffset = 1.5f;
    /// <summary>
    /// 跟随UI的Image初始颜色
    /// </summary>
    private Color oriFollowUIColor;

    /// <summary>
    /// 区域范围XZ平面的点集合(逆时针)
    /// </summary>
    List<Vector2> XZpointList;
    ///// <summary>
    ///// 是否告警
    ///// </summary>
    //public bool isAlarm;
    /// <summary>
    /// 是否处于告警中
    /// </summary>
    public bool isAlarming;
    /// <summary>
    /// 编辑界面，新增区域
    /// </summary>
    public bool isNewAdd;

    // Use this for initialization
    void Start()
    {
        alarmPersons = new List<LocationObject>();
        locationPersons = new List<LocationObject>();
        InitDepNode();

        SetEditEvent(true);




        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, Vector3.zero);
        followNameUI = UGUIFollowManage.Instance.CreateItem(MonitorRangeManager.Instance.NameUI, targetTagObj, "MapAreasUI", null, false, false);
        areaNameUIController = followNameUI.GetComponent<AreaNameUIController>();
        if (areaNameUIController)
        {
            areaNameUIController.SetMonitorRangeObject(this);
        }
        Text ntxt = followNameUI.GetComponentInChildren<Text>();
        ntxt.text = info.Name;
        oriFollowUIColor = followNameUI.GetComponentInChildren<Image>().color;
        if (MonitorRangeManager.Instance.isShowRangeRender == false)
        {
            if (!MonitorRangeManager.Instance.IsEditState)
            {
                SetFollowNameUIEnable(false);
            }
        }

        if (isNewAdd)
        {
            areaNameUIController.SetImageRaycast(false);
        }

        if (MonitorRangeManager.Instance.IsEditState)
        {
            SetSelectedUI(true);
        }
    }

    public void SetIsNewAdd(bool b)
    {
        isNewAdd = b;
    }

    /// <summary>
    /// 设置是否绑定区域编辑事件
    /// </summary>
    public void SetEditEvent(bool b)
    {
        if (b)
        {
            EditorGizmoSystem.Instance.RotationGizmo.GizmoDragStart += RotationGizmo_GizmoDragStart;

            EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragUpdate += TranslationGizmo_GizmoDragUpdate;
            EditorGizmoSystem.Instance.RotationGizmo.GizmoDragUpdate += RotationGizmo_GizmoDragUpdate;
            EditorGizmoSystem.Instance.ScaleGizmo.GizmoDragUpdate += ScaleGizmo_GizmoDragUpdate;

            EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragEnd += TranslationGizmo_GizmoDragEnd;
            EditorGizmoSystem.Instance.RotationGizmo.GizmoDragEnd += RotationGizmo_GizmoDragEnd;
            EditorGizmoSystem.Instance.ScaleGizmo.GizmoDragEnd += ScaleGizmo_GizmoDragEnd;

            EditorUndoRedoSystem.Instance.UndoEnd += Instance_UndoEnd;//ctrl+z,撤销操作触发事件
        }
        else
        {
            if (EditorGizmoSystem.Instance)
            {
                EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragUpdate -= TranslationGizmo_GizmoDragUpdate;
                EditorGizmoSystem.Instance.RotationGizmo.GizmoDragUpdate -= RotationGizmo_GizmoDragUpdate;
                EditorGizmoSystem.Instance.ScaleGizmo.GizmoDragUpdate -= ScaleGizmo_GizmoDragUpdate;

                EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragEnd -= TranslationGizmo_GizmoDragEnd;
                EditorGizmoSystem.Instance.RotationGizmo.GizmoDragEnd -= RotationGizmo_GizmoDragEnd;
                EditorGizmoSystem.Instance.ScaleGizmo.GizmoDragEnd -= ScaleGizmo_GizmoDragEnd;

                EditorUndoRedoSystem.Instance.UndoEnd -= Instance_UndoEnd;//ctrl+z,撤销操作触发事件
            }
        }
    }

    /// <summary>
    /// 设置DepNode
    /// </summary>
    private void InitDepNode()
    {
        if (depNode == null)
        {
            depNode = RoomFactory.Instance.GetDepNodeByTopo(info);
        }
        if (depNode != null)
        {
            //followTarget = depNode.NodeObject;
            SetFollowTarget(depNode.NodeObject);
            //depNode.SetMonitorRangeObject(this);
            if (followTarget)
            {
                followOffset = transform.position - followTarget.transform.position;
            }
        }
    }

    /// <summary>
    /// 设置区域的跟随建筑物
    /// </summary>
    /// <param name="o"></param>
    public void SetFollowTarget(GameObject o)
    {
        followTarget = o;
    }

    /// <summary>
    /// 设置区域监控范围
    /// </summary>
    private void SetDepMonitorRange()
    {
        if (depNode != null) depNode.SetMonitorRangeObject(this);
    }
    private void OnEnable()
    {
        if (followNameUI != null)
        {
            //followNameUI.SetActive(true);
            SetFollowNameUIEnable(true);
        }
    }

    private void OnDisable()
    {
        if (followNameUI != null)
        {
            //followNameUI.SetActive(false);
            SetFollowNameUIEnable(false);
        }
    }

    private void OnDestroy()
    {
        //if (EditorGizmoSystem.Instance)
        //{
        //    EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragUpdate -= TranslationGizmo_GizmoDragUpdate;
        //    EditorGizmoSystem.Instance.RotationGizmo.GizmoDragUpdate -= RotationGizmo_GizmoDragUpdate;
        //    EditorGizmoSystem.Instance.ScaleGizmo.GizmoDragUpdate -= ScaleGizmo_GizmoDragUpdate;

        //    EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragEnd -= TranslationGizmo_GizmoDragEnd;
        //    EditorGizmoSystem.Instance.RotationGizmo.GizmoDragEnd -= RotationGizmo_GizmoDragEnd;
        //    EditorGizmoSystem.Instance.ScaleGizmo.GizmoDragEnd -= ScaleGizmo_GizmoDragEnd;

        //    EditorUndoRedoSystem.Instance.UndoEnd -= Instance_UndoEnd;//ctrl+z,撤销操作触发事件
        //}

        SetEditEvent(false);

        DestroyImmediate(followNameUI);
    }

    private void Instance_UndoEnd()
    {
        Debug.Log("Instance_UndoEnd!!!");
        List<GameObject> gs = new List<GameObject>(EditorObjectSelection.Instance.SelectedGameObjects);
        if (gs.Contains(gameObject))
        {
            DoSaveInfo();
            RangeEditWindow.Instance.Show(this);
        }
    }

    /// <summary>
    /// 保存相关修改
    /// </summary>
    [ContextMenu("DoSaveInfo")]
    public void DoSaveInfo()
    {
        UpdateSize();
        UpdatePos();
        UpdateAngle();
        SaveInfo();
    }

    private void ScaleGizmo_GizmoDragUpdate(Gizmo gizmo)
    {
        List<GameObject> gs = new List<GameObject>(gizmo.ControlledObjects);
        if (gs.Contains(gameObject))
        {
            UpdateSize();
            RangeEditWindow.Instance.Show(this);
        }
    }

    private void ScaleGizmo_GizmoDragEnd(Gizmo gizmo)
    {
        List<GameObject> gs = new List<GameObject>(gizmo.ControlledObjects);
        if (gs.Contains(gameObject))
        {
            SaveInfo();
        }
        //throw new System.NotImplementedException();
    }

    Transform Parent;

    private void RotationGizmo_GizmoDragStart(Gizmo gizmo)
    {
        List<GameObject> gs = new List<GameObject>(gizmo.ControlledObjects);
        if (gs.Contains(gameObject))
        {
            Parent = transform.parent;
        }



    }

    private void RotationGizmo_GizmoDragUpdate(Gizmo gizmo)
    {
        List<GameObject> gs = new List<GameObject>(gizmo.ControlledObjects);
        if (gs.Contains(gameObject))
        {
            UpdateAngle();
            if (Parent != null)
            {
                transform.SetParent(null);
            }

            RangeEditWindow.Instance.Show(this);

        }
    }

    private void RotationGizmo_GizmoDragEnd(Gizmo gizmo)
    {
        List<GameObject> gs = new List<GameObject>(gizmo.ControlledObjects);
        if (gs.Contains(gameObject))
        {
            if (Parent != null)
            {
                transform.SetParent(Parent);
            }
            SaveInfo();
        }
        //throw new System.NotImplementedException();
    }

    private void TranslationGizmo_GizmoDragUpdate(Gizmo gizmo)
    {
        List<GameObject> gs = new List<GameObject>(gizmo.ControlledObjects);
        if (gs.Contains(gameObject))
        {
            UpdatePos();
            RangeEditWindow.Instance.Show(this);
        }
    }

    private void TranslationGizmo_GizmoDragEnd(Gizmo gizmo)
    {
        if (gameObject == null) return;
        List<GameObject> gs = new List<GameObject>(gizmo.ControlledObjects);
        if (gs.Contains(gameObject))
        {
            SaveInfo();
        }
        //throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        //SetPosSize();

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(PhysicalTopology areaInfo, RangeNode rangeNode, DepNode depNode)
    {
        this.Id = areaInfo.Id;
        this.info = areaInfo;
        this.depNode = depNode;
        this.rangeNode = rangeNode;
        InitDepNode();
        oriSize = gameObject.GetGlobalSize();//三维物体的大小
        UpdatePosSize(areaInfo);
        SetPosSize();
        SetDepMonitorRange();
        //isUpdate = true;
        //CreateBoundPoints(info.EditBound);

        if (areaInfo.Transfrom != null)
        {
            IsCreateAreaByData = areaInfo.Transfrom.IsCreateAreaByData;
            IsOnAlarmArea = areaInfo.Transfrom.IsOnAlarmArea;
            IsOnLocationArea = areaInfo.Transfrom.IsOnLocationArea;
        }

        //if (gameObject.name.Contains("电子设备间"))
        //{
        //    int i = 0;
        //}
        InitXZpointList();
    }

    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        SetRendererEnable(true);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        SetRendererEnable(false);
    }

    /// <summary>
    /// 设置区域范围正确的在建筑中的位置
    /// </summary>
    public void SetFollowTarget()
    {
        //SetDepNode();
        if (followTarget != null)
        {
            transform.position = followOffset + followTarget.transform.position;
        }
    }

    //private void CreateBoundPoints(Bound bound)
    //{
    //    if (bound != null && bound.Points != null)
    //    {
    //        GameObject pointsObj = new GameObject(gameObject.name + "_Points");
    //        pointsObj.transform.SetParent(gameObject.transform.parent);
    //        foreach (Point point in bound.Points)
    //        {
    //            GameObject pObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //            pObj.name = string.Format("{0},({1},{2})", point.Index, point.X, point.Y);
    //            pObj.transform.SetParent(pointsObj.transform);
    //            pObj.transform.localScale = new Vector3(0.1f, 50f, 0.1f);
    //            pObj.transform.localPosition =
    //                LocationManager.GetRealVector(new Vector3((float)point.X, (float)point.Z, (float)point.Y));
    //        }
    //    }
    //    else
    //    {
    //        Log.Info("CreateBoundPoints", "bound == null || bound.Points == null");
    //    }
    //}

    /// <summary>
    /// 初始化位置和大小信息
    /// </summary>
    private void UpdatePosSize(PhysicalTopology p)
    {
        UpdatePosSizeInfo(p.Transfrom);
    }

    /// <summary>
    /// 初始化位置和大小信息
    /// </summary>
    private void UpdatePosSizeInfo(float minx, float maxx, float miny, float maxy, float minz, float maxz)
    {
        Vector3 pos = new Vector3((maxx + minx) / 2, (maxy + miny) / 2, (maxz + minz) / 2);
        Vector3 size = new Vector3(maxx - minx, maxy - miny, maxz - minz);
        //Vector3 offset = LocationManager.Instance.GetPosOffset();
        //targetPos = pos + offset;
        targetPos = LocationManager.GetRealVector(pos);
        targetScale = LocationManager.GetRealSizeVector(size);
    }

    /// <summary>
    /// 初始化位置和大小信息
    /// </summary>
    private void UpdatePosSizeInfo(TransformM tranM)
    {
        if (tranM == null) return;
        MonitorRangeObject rangeParent = null;
        if (depNode != null && depNode.ParentNode != null)
        {
            rangeParent = depNode.ParentNode.monitorRangeObject;
        }
        rangeNode.rangeObject = this; //创建自身区域范围

        if (!tranM.IsCreateAreaByData && info.Type == AreaTypes.范围)//tranM.IsOnAlarmArea && 
        {
            Debug.LogErrorFormat("告警区域：[{0}]，没根据CAD数据来创建（IsCreateAreaByData没设置为true），可能导致创建出错！", info.Name);
        }

        if (tranM.IsCreateAreaByData) //利用数据创建区域范围
        {
            //Log.Info("MonitorRangeObject.UpdatePosSizeInfo","brach 1");
            UpdatePosSizeByTransform(tranM);
        }
        else//利用自身大小创建区域范围
        {
            if (followTarget != null)
            {
                //Log.Info("MonitorRangeObject.UpdatePosSizeInfo", "brach 2");
                UpdatePosSizeByFollowTarget();
            }
            else
            {
                //Log.Info("MonitorRangeObject.UpdatePosSizeInfo", "brach 3");
                UpdatePosSizeByTransform(tranM);
            }
        }

        if (transform.parent != null)
        {//因为这里的计算出来的比例为真实大小比例，而不是相对比例

        }
    }

    private void UpdatePosSizeByFollowTarget()
    {
        Vector3 pos = Vector3.zero;
        Vector3 angles = Vector3.zero;
        pos = followTarget.transform.position; //获取相对父区域范围的坐标
        angles = followTarget.transform.eulerAngles; //获取相对父区域范围的角度

        targetPos = pos;
        Vector3 size = followTarget.GetGlobalSize();
        if (size.y > yOffset) //为了处理楼层的区域范围计算不用太高，以至于超出楼层高度
        {
            size = new Vector3(size.x, size.y - yOffset, size.z);
            //targetPos = new Vector3(targetPos.x, targetPos.y - (1f / 2), targetPos.z);
        }

        targetScale = new Vector3(Mathf.Abs(size.x / oriSize.x), Mathf.Abs(size.y / oriSize.y),
            Mathf.Abs(size.z / oriSize.z));
        ; //就跟BoxCollider一样大
        if (targetScale == Vector3.zero)
        {
            targetScale = Vector3.one;
        }

        targetAngles = angles;
    }

    private void UpdatePosSizeByTransform(TransformM tranM)
    {
        Vector3 pos = new Vector3((float) tranM.X, (float) tranM.Y, (float) tranM.Z);
        Vector3 angles = new Vector3((float) tranM.RX, (float) tranM.RY, (float) tranM.RZ);
        Vector3 size = new Vector3((float) tranM.SX, (float) tranM.SY, (float) tranM.SZ);

        targetPos = LocationManager.GetRealSizeVector(pos);
        Vector3 realsize = LocationManager.GetRealSizeVector(size);
        targetScale = new Vector3(Mathf.Abs(realsize.x / oriSize.x), Mathf.Abs(realsize.y / oriSize.y),
            Mathf.Abs(realsize.z / oriSize.z));
        if (!info.Transfrom.IsRelative) //绝对坐标，加上二维和三维转换时的坐标偏移就行
        {
            //Log.Info("MonitorRangeObject.UpdatePosSizeByTransform", "brach 0 : axisZero");
            targetPos += LocationManager.Instance.axisZero;
        }
        else //相对坐标，要考虑父物体的坐标
        {
            if (rangeNode.parentNode != null)
            {
                RangeNode parentRangeNode = rangeNode.parentNode;
                var parentRangeObject = parentRangeNode.rangeObject;
                PhysicalTopology parentArea = parentRangeNode.info;
                TransformM parentTransform = parentArea.Transfrom;

                //Log.Info("MonitorRangeObject.UpdatePosSizeByTransform", string.Format("parentArea:"+ parentArea.Name));

                //接下来计算建筑物的左下角坐标
                Vector3 buildPos = Vector3.zero;
                if (parentTransform != null && parentTransform.IsCreateAreaByData)
                {
                    //Log.Info("MonitorRangeObject.UpdatePosSizeByTransform", "brach 1 : by parentTransform");
                    Vector3 sizeT = new Vector3((float) (parentTransform.SX / 2f), (float) (parentTransform.SY / 2),
                        (float) (parentTransform.SZ / 2)); //二维坐标尺寸
                    var offset = LocationManager.GetRealSizeVector(sizeT); //三维坐标尺寸，也就是偏移量
                    buildPos = parentRangeObject.transform.position - offset; //父物体（中心）位置-父物体一半大小=父物体左下角坐标
                    //Log.Info("MonitorRangeObject.UpdatePosSizeByTransform",string.Format("sizeT:{0},sizeR:{1},buildPos:{2}", sizeT, offset, buildPos));
                }
                else
                {
                    if (parentRangeObject)
                    {
                        //Log.Info("MonitorRangeObject.UpdatePosSizeByTransform", "brach 2 : parentRangeObject");
                        Vector3 sizeR = parentRangeObject.gameObject.GetGlobalSize();//父物体真实大小
                        Vector3 offset= new Vector3((float)(sizeR.x / 2f),
                            (float)(-(sizeR.y + parentRangeObject.yOffset) / 2),
                            (float)(sizeR.z / 2)); //
                        buildPos = parentRangeObject.transform.position+offset;//父物体（中心）位置-偏移=父物体左下角坐标

                        //Log.Info("MonitorRangeObject.UpdatePosSizeByTransform",string.Format("sizeR:{0},offset:{1},buildPos:{2}", sizeR, offset, buildPos));
                    }
                    else
                    {
                        if (rangeNode.parentNode.info.Type == Types.范围)
                        {
                            //Log.Info("MonitorRangeObject.UpdatePosSizeByTransform", "brach 3");
                            targetPos += LocationManager.Instance.axisZero;
                        }
                        else
                        {
                            //Log.Info("MonitorRangeObject.UpdatePosSizeByTransform", "brach 4");
                        }
                    }
                }

                targetPos += buildPos;

                //Log.Info("MonitorRange.UpdatePosSizeByTransform",string.Format("targetPos:{0},buildPos:{1}", targetPos, buildPos));

                //ShowTestPoint(buildPos, "buildPos_"+ parentArea.Name);
                //ShowTestPoint(targetPos, "targetPos_"+info.Name);
            }
            else
            {
                //Log.Error("MonitorRange.UpdatePosSizeByTransform", "rangeNode.parentNode == null");
            }
        }

        targetAngles = new Vector3((float) tranM.RX, (float) tranM.RY, (float) tranM.RZ);
    }

    private void ShowTestPoint(Vector3 v,string n)
    {
#if  UNITY_EDITOR
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        p.name = n;
        p.transform.position = v;
#endif
    }

    ///// <summary>
    ///// 初始化位置和大小信息
    ///// </summary>
    //private void UpdatePosSizeInfo(TransformM tranM)
    //{
    //    Vector3 pos = new Vector3((float)tranM.X, (float)tranM.Y, (float)tranM.Z);
    //    Vector3 angles = new Vector3((float)tranM.RX, (float)tranM.RY, (float)tranM.RZ);
    //    Vector3 size = new Vector3((float)tranM.SX, (float)tranM.SY, (float)tranM.SZ);
    //    //Vector3 offset = LocationManager.Instance.GetPosOffset();
    //    //targetPos = pos + offset;
    //    targetPos = LocationManager.GetRealSizeVector(pos);
    //    Vector3 realsize = LocationManager.GetRealSizeVector(size);
    //    targetScale = new Vector3(Mathf.Abs(realsize.x / oriSize.x), Mathf.Abs(realsize.y / oriSize.y), Mathf.Abs(realsize.z / oriSize.z));

    //    if (transform.parent != null)
    //    {//因为这里的计算出来的比例为真实大小比例，而不是相对比例
    //        float scalex = targetScale.x / transform.parent.lossyScale.x;
    //        float scaley = targetScale.y / transform.parent.lossyScale.y;
    //        float scalez = targetScale.z / transform.parent.lossyScale.z;
    //        targetScale = new Vector3(scalex, scaley, scalez);
    //        float posx = targetPos.x / transform.parent.lossyScale.x;
    //        float posy = targetPos.y / transform.parent.lossyScale.y;
    //        float posz = targetPos.z / transform.parent.lossyScale.z;
    //        targetPos = new Vector3(posx, posy, posz);
    //    }
    //}
    /// <summary>
    /// 设置位置和大小
    /// </summary>
    public void SetPosSize()
    {
        transform.localPosition = targetPos;
        transform.localScale = new Vector3(targetScale.x, targetScale.y, targetScale.z);

        Transform p = transform.parent;
        if (p != null)
        {
            transform.SetParent(null);
            transform.eulerAngles = targetAngles;
            transform.SetParent(p);
        }
        else
        {
            transform.eulerAngles = targetAngles;
        }

    }

    /// <summary>
    /// 检查该区域是否需要删除
    /// </summary>
    public void CheckDestroyArea()
    {
        if (IsUpdate == false)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            IsUpdate = false;
        }
    }

    /// <summary>
    /// 告警闪烁开启
    /// </summary>
    [ContextMenu("AlarmOn")]
    public void AlarmOn()
    {
        //SetRendererEnable(true);
        //Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        //Color colorStart = Color.red;
        //Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0);
        //h.FlashingOn(Color.red, colorEnd, 2f);
        FlashingOn(Color.red, 2f);
    }

    /// <summary>
    /// 告警闪烁关闭
    /// </summary>
    [ContextMenu("AlarmOff")]
    public void AlarmOff()
    {
        //Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        //h.FlashingOff();
        //SetRendererEnable(false);
        FlashingOff();
    }

    /// <summary>
    /// 开启闪烁
    /// </summary>
    /// <param name="flashColor"></param>
    /// <param name="frequency">Flashing frequency (times per second)</param>
    public void FlashingOn(Color flashColor, float frequency)
    {
        bool isbool = MonitorRangeManager.IsBelongtoCurrentDep(depNode);
        if (isbool)
        {
            SetRendererEnable(true);
        }
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        Color colorStart = flashColor;
        Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0);
        h.FlashingOn(colorStart, colorEnd, frequency);
    }

    /// <summary>
    /// 关闭闪烁
    /// </summary>
    public void FlashingOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.FlashingOff();
        if (!MonitorRangeManager.Instance.IsEditState)
        {
            SetRendererEnable(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (info.Transfrom == null) return;
        LocationObject locationObject = other.gameObject.GetComponent<LocationObject>();
        if (locationObject)
        {
            //if (gameObject.name.Contains("四会热电厂"))
            //{
            //    Debug.Log("四会热电厂:" + locationObject.name);
            //}
            //if (locationObject.name == "标签60007赵一男")
            //{
            //    int i = 1;
            //}
            bool isSameFloor = CheckFloorDepNode(locationObject.currentDepNode);
            isSameFloor = true;//以后这个判断就不要了，在做完编辑区域之后

            //三维里通过碰撞检测，来触发人员区域告警，这里注释是改成服务端发送过来触发告警（下面的ShowAlarm方法）
            //if (info.Transfrom.IsOnAlarmArea)
            //{
            //    if (!alarmPersons.Contains(locationObject) && isSameFloor)
            //    {
            //        alarmPersons.Add(locationObject);
            //        AlarmOn();
            //    }
            //}

            if (info.Transfrom.IsOnLocationArea)
            {
                if (!locationPersons.Contains(locationObject) && isSameFloor)
                {
                    locationPersons.Add(locationObject);
                    //if (gameObject.name.Contains("四会热电厂"))
                    //{
                    //    Debug.Log("四会热电厂添加locationPersons");
                    //}
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (info.Transfrom != null && !info.Transfrom.IsOnAlarmArea) return;
        LocationObject locationObject = other.gameObject.GetComponent<LocationObject>();
        OnTriggerExitEx(locationObject);
    }

    public void OnTriggerExitEx(LocationObject locationObject)
    {

        if (locationObject)
        {
            //三维里通过碰撞检测来检测人员是否在告警区域内部，来关闭人员和区域告警，这里注释是改成服务端发送过来关闭告警（下面的HideAlarm方法）
            //alarmPersons.Remove(locationObject);
            //if (alarmPersons.Count == 0)
            //{
            //    AlarmOff();
            //}

            locationPersons.Remove(locationObject);
            if (gameObject.name.Contains("四会热电厂"))
            {
                //Debug.Log("四会热电厂移除locationPersons");
            }
        }
    }

    /// <summary>
    /// 更新位置信息
    /// </summary>
    private void UpdatePos()
    {
        if (!info.Transfrom.IsCreateAreaByData) return;
        Log.Info("SaveInfo", string.Format("Pos1:{0},Pos2:{1}", transform.localPosition, transform.position));

        Vector3 pos = Vector3.zero;
        if (info.Transfrom.IsRelative)//是否是相对位置
        {
            Vector3 pSize = Vector3.zero;
            Vector3 pPos = Vector3.zero;
            if (depNode != null && depNode.ParentNode != null && depNode.ParentNode.monitorRangeObject != null)
            {
                pSize = depNode.ParentNode.monitorRangeObject.gameObject.GetGlobalSize();
                pPos = depNode.ParentNode.monitorRangeObject.transform.position;
            }
            //pSize = transform.parent.gameObject.GetGlobalSize();

            Vector3 pos2D = new Vector3((float)(-pSize.x / 2f), (float)((pSize.y + yOffset) / 2), (float)(-pSize.z / 2));//建筑物的左下角坐标

            //float x = transform.localPosition.x * transform.parent.lossyScale.x;
            //float y = transform.localPosition.y * transform.parent.lossyScale.y;
            //float z = transform.localPosition.z * transform.parent.lossyScale.z;
            //Vector3 posT = new Vector3(x, y, z) + pos2D;
            Vector3 posT = transform.position - pPos + pos2D;
            pos = LocationManager.GetDisRealSizeVector(posT);
        }
        else//厂区内的建筑物
        {
            pos = LocationManager.GetDisRealVector(transform.position);
        }

        info.Transfrom.X = pos.x;
        info.Transfrom.Y = pos.y;
        info.Transfrom.Z = pos.z;
    }

    /// <summary>
    /// 根据现实世界位置，设置位置
    /// </summary>
    /// <param name="angleVec"></param>
    public void SetPos(Vector3 realPos)
    {
        if (!info.Transfrom.IsCreateAreaByData) return;

        Vector3 pos = new Vector3((float)realPos.x, (float)realPos.y, (float)realPos.z);

        Vector3 targetposT = LocationManager.GetRealSizeVector(pos);
        if (!info.Transfrom.IsRelative)
        {
            targetposT += LocationManager.Instance.axisZero;
        }
        else
        {
            if (rangeNode.parentNode != null)
            {
                PhysicalTopology buldingNode = rangeNode.parentNode.info;
                TransformM tm = buldingNode.Transfrom;
                Vector3 buildPos = Vector3.zero;
                if (tm != null && tm.IsCreateAreaByData)
                {
                    Vector3 pos2D = new Vector3((float)(tm.SX / 2f), (float)(tm.SY / 2), (float)(tm.SZ / 2));//建筑物的左下角坐标

                    buildPos = -LocationManager.GetRealSizeVector(pos2D);
                    buildPos += rangeNode.parentNode.rangeObject.transform.position;
                }
                else
                {
                    Vector3 pSize = rangeNode.parentNode.rangeObject.gameObject.GetGlobalSize();
                    buildPos += rangeNode.parentNode.rangeObject.transform.position;
                    buildPos += new Vector3((float)(pSize.x / 2f), (float)(-(pSize.y + rangeNode.parentNode.rangeObject.yOffset) / 2), (float)(pSize.z / 2));//建筑物的左下角坐标
                }

                targetposT += buildPos;
            }
        }

        transform.localPosition = targetposT;

        info.Transfrom.X = realPos.x;
        info.Transfrom.Y = realPos.y;
        info.Transfrom.Z = realPos.z;
    }

    /// <summary>
    /// 更新位置信息
    /// </summary>
    private void UpdateAngle()
    {
        if (!info.Transfrom.IsCreateAreaByData) return;

        info.Transfrom.RX = transform.eulerAngles.x;
        info.Transfrom.RY = transform.eulerAngles.y;
        info.Transfrom.RZ = transform.eulerAngles.z;
    }

    /// <summary>
    /// 根据现实世界角度，设置角度
    /// </summary>
    /// <param name="angleVec"></param>
    public void SetAngle(Vector3 angleVec)
    {
        if (!info.Transfrom.IsCreateAreaByData) return;
        transform.eulerAngles = angleVec;

        info.Transfrom.RX = transform.eulerAngles.x;
        info.Transfrom.RY = transform.eulerAngles.y;
        info.Transfrom.RZ = transform.eulerAngles.z;
    }

    /// <summary>
    /// 更新位置信息
    /// </summary>
    private void UpdateSize()
    {
        if (!info.Transfrom.IsCreateAreaByData) return;
        Log.Info("SaveInfo", string.Format("Pos1:{0},Pos2:{1}", transform.localPosition, transform.position));
        Vector3 size = gameObject.GetGlobalSize();
        size = LocationManager.GetDisRealSizeVector(size);
        info.Transfrom.SX = Mathf.Abs(size.x);
        info.Transfrom.SY = Mathf.Abs(size.y);
        info.Transfrom.SZ = Mathf.Abs(size.z);
    }

    /// <summary>
    /// 根据现实世界真实尺寸设置大小
    /// </summary>
    public void SetSize(Vector3 realSizeT)
    {
        if (!info.Transfrom.IsCreateAreaByData) return;

        Vector3 realSize = LocationManager.GetRealSizeVector(realSizeT);
        realSize = new Vector3(Math.Abs(realSize.x), Math.Abs(realSize.y), Math.Abs(realSize.z));

        //Vector3 size = gameObject.GetGlobalSize();
        //targetScale = new Vector3(Mathf.Abs(realSize.x / oriSize.x), Mathf.Abs(realSize.y / oriSize.y), Mathf.Abs(realSize.z / oriSize.z)); ;//就跟BoxCollider一样大
        float scaleX = realSize.x / oriSize.x;
        float scaleY = realSize.y / oriSize.y;
        float scaleZ = realSize.z / oriSize.z;
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        info.Transfrom.SX = Mathf.Abs(realSizeT.x);
        info.Transfrom.SY = Mathf.Abs(realSizeT.y);
        info.Transfrom.SZ = Mathf.Abs(realSizeT.z);
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    public void UpdateData(string nameT, Vector3 realposT, Vector3 realangleT, Vector3 realsizeT)
    {
        SetAreaName(nameT);
        SetPos(realposT);
        SetAngle(realangleT);
        SetSize(realsizeT);
        SaveInfo();
    }

    /// <summary>
    /// 设置建筑名称
    /// </summary>
    public void SetAreaName(string nameT)
    {
        info.Name = nameT;
        Text ntxt = followNameUI.GetComponentInChildren<Text>();
        ntxt.text = info.Name;
    }

    /// <summary>
    /// 保存信息
    /// </summary>
    public void SaveInfo()
    {
        //if (!info.Transfrom.IsCreateAreaByData) return;--宝信新增一个区域后，不能修改，所以把这句屏蔽掉（现在好像没有告警区域这个概念）
        Debug.LogError("MonitorRangeObject.SaveInfo");
        //Log.Info("SaveInfo", string.Format("Pos1:{0},Pos2:{1}", transform.localPosition, transform.position));

        //Vector3 pos = Vector3.zero;
        //if (info.Transfrom.IsRelative)//是否是相对位置
        //{
        //    Vector3 pSize = transform.parent.gameObject.GetGlobalSize();
        //    Vector3 pos2D = new Vector3((float)(-pSize.x / 2f), (float)((pSize.y + yOffset) / 2), (float)(-pSize.z / 2));//建筑物的左下角坐标

        //    float x = transform.localPosition.x * transform.parent.lossyScale.x;
        //    float y = transform.localPosition.y * transform.parent.lossyScale.y;
        //    float z = transform.localPosition.z * transform.parent.lossyScale.z;
        //    Vector3 posT = new Vector3(x, y, z) + pos2D;
        //    pos = LocationManager.GetDisRealSizeVector(posT);
        //}
        //else//厂区内的建筑物
        //{
        //    pos = LocationManager.GetDisRealVector(transform.position);
        //}

        //info.Transfrom.X = pos.x;
        //info.Transfrom.Y = pos.y;
        //info.Transfrom.Z = pos.z;

        //info.Transfrom.RX = transform.eulerAngles.x;
        //info.Transfrom.RY = transform.eulerAngles.y;
        //info.Transfrom.RZ = transform.eulerAngles.z;

        //Vector3 size = gameObject.GetGlobalSize();
        //size = LocationManager.GetDisRealSizeVector(size);
        //info.Transfrom.SX = Mathf.Abs(size.x);
        //info.Transfrom.SY = Mathf.Abs(size.y);
        //info.Transfrom.SZ = Mathf.Abs(size.z);

        //CommunicationObject.Instance.EditArea(info);
        Log.Info("EditMonitorRange", TransformMToString(info.Transfrom));
        InitXZpointList();
        //CommunicationObject.Instance.EditMonitorRange(info);
        PhysicalTopology p = new PhysicalTopology();
        //p.Name = info.Name;
        //p.Id = info.Id;
        //p.Transfrom = info.Transfrom;
        //p.InitBound = info.InitBound;
        //p.IsCreateAreaByData = info.IsCreateAreaByData;

        p = info;
        var parentDep = depNode.ParentNode;
        if (parentDep!=null&&parentDep.TopoNode.Type == AreaTypes.楼层) //宝信项目坐标系偏移，应该也是兼容其他项目的
        {
            p.Transfrom.X += parentDep.TopoNode.InitBound.MinX;
            p.Transfrom.Z += parentDep.TopoNode.InitBound.MinY;
        }

        CommunicationObject.Instance.EditMonitorRange(p);
    }

    public static string TransformMToString(TransformM t)
    {
        return string.Format("({0},{1},{2}),({3},{4},{5}),({6},{7},{8}) Id={9}", t.X, t.Y, t.Z, t.RX, t.RY, t.RZ, t.SX,
            t.SY,
            t.SZ, t.Id);
    }

    /// <summary>
    /// 获取碰撞器
    /// </summary>
    /// <returns></returns>
    public Collider GetCollider()
    {
        if (myCollider == null)
        {
            myCollider = gameObject.GetComponent<Collider>();
        }
        return myCollider;
    }

    /// <summary>
    /// 设置碰撞器是否启用
    /// </summary>
    public void SetColliderEnable(bool isEnable)
    {
        if (gameObject.name.Contains("四会热电厂"))
        {
            int i = 0;
        }
        GetCollider();
        myCollider.enabled = isEnable;
        if (isEnable)
        {
        }
        else
        {
            AlarmOff();
            ClearPersons();
        }

    }

    /// <summary>
    /// 清除人员的相关信息
    /// </summary>
    private void ClearPersons()
    {
        if (alarmPersons != null)
        {
            foreach (LocationObject obj in alarmPersons)
            {
                //obj.OnTriggerExitEx(this);
                obj.HideAlarm(depNode.NodeID);
            }
            alarmPersons.Clear();
        }
        if (locationPersons != null)
        {
            foreach (LocationObject obj in locationPersons)
            {
                obj.OnTriggerExitEx(this);
            }

            locationPersons.Clear();

            if (gameObject.name.Contains("四会热电厂"))
            {
                Debug.Log("四会热电厂清除locationPersons");
            }
        }
    }

    /// <summary>
    /// 获取渲染器
    /// </summary>
    /// <returns></returns>
    public Renderer GetRenderer()
    {
        if (render == null)
        {
            render = gameObject.GetComponent<Renderer>();
        }
        return render;
    }

    /// <summary>
    /// 设置渲染器是否启用
    /// </summary>
    public void SetRendererEnable(bool isEnable)
    {
        //if (MonitorRangeManager.Instance.IsShowAlarmArea && IsOnAlarmArea && !isEnable) return;
        if (depNode!=null&&depNode.NodeID == 499)
        {
            int i = 0;
        }
        GetRenderer();
        render.enabled = isEnable;
        SetFollowNameUIEnable(isEnable);
    }

    /// <summary>
    /// 设置跟随的名称UI是否启用
    /// </summary>
    /// <param name="isEnable"></param>
    public void SetFollowNameUIEnable(bool isEnable)
    {
        if (followNameUI)
        {
            followNameUI.SetActive(isEnable);
        }
    }

    /// <summary>
    /// 检查是否属于同一楼层
    /// </summary>
    public bool CheckFloorDepNode(DepNode depNodeT)
    {
        if (depNode == null)
        {
            depNode = RoomFactory.Instance.GetDepNodeByTopo(info);
        }

        //if(depNodeT==null || depNodeT.TopoNode.Type== Types.楼层|| depNodeT.TopoNode.Type == Types.r)

        if (depNodeT == depNode)
        {
            return true;
        }
        else
        {
            if (depNodeT == null)
            {
                return false;
            }
            if (depNodeT.depType == DepType.Factory && depNode == null)
            {
                return true;
            }
            else
            {
                if (depNode == null)
                {
                    return false;
                }
            }
        }



        if (depNodeT.NodeObject != null && depNode.NodeObject != null)
        {
            FloorController floorcontroller1 = depNodeT.NodeObject.GetComponentInParent<FloorController>();
            FloorController floorcontroller2 = depNode.NodeObject.GetComponentInParent<FloorController>();
            if (floorcontroller1 == floorcontroller2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (depNodeT.NodeObject != depNode.NodeObject)
            {
                return false;
            }
            else//两个都为空的情况
            {
                return true;
            }
        }
    }

    /// <summary>
    /// 设置是否可编辑
    /// </summary>
    public void SetEditEnable(bool isAbleT)
    {
        if (isAbleT)
        {
            SetLayer(Layers.Range);

        }
        else
        {
            SetLayer(Layers.IgnoreRaycast);

        }
    }

    /// <summary>
    /// 展示告警
    /// </summary>
    public void ShowAlarm(LocationObject locationObject)
    {
        if (locationObject.name.Contains("耿宜"))
        {
            int i = 0;
        }
        if (!alarmPersons.Contains(locationObject))
        {
            //Debug.LogErrorFormat("区域：{0},告警了！", info.Name);
            alarmPersons.Add(locationObject);
            //if (isAlarming) return;
            //isAlarming = true;
            //AlarmOn();
        }
        isAlarming = true;
        AlarmOn();
    }

    /// <summary>
    /// 显示告警
    /// </summary>
    public void ShowAlarm()
    {
        if (alarmPersons != null && alarmPersons.Count > 0)
        {
            if (isAlarming) return;
            isAlarming = true;
            AlarmOn();
        }
    }

    /// <summary>
    /// 关闭告警
    /// </summary>
    /// <param name="locationObject"></param>
    public void HideAlarm(LocationObject locationObject)
    {
        alarmPersons.Remove(locationObject);
        if (alarmPersons.Count == 0)
        {
            //Debug.LogErrorFormat("区域：{0},消警了！", info.Name);
            if (isAlarming == false) return;
            AlarmOff();
            isAlarming = false;
        }
    }

    /// <summary>
    /// 关闭告警
    /// </summary>
    public void HideAlarm()
    {
        if (alarmPersons != null && alarmPersons.Count == 0)
        {
            if (isAlarming == false) return;
            AlarmOff();
            isAlarming = false;
        }
    }

    /// <summary>
    /// 设置层
    /// </summary>
    private void SetLayer(string layernameT)
    {
        gameObject.layer = LayerMask.NameToLayer(layernameT);
    }

    public bool OnCanBeSelected(ObjectSelectEventArgs selectEventArgs)
    {
        //throw new NotImplementedException();
        return true;
    }

    public void OnSelected(ObjectSelectEventArgs selectEventArgs)
    {
        ////throw new NotImplementedException();
        //GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, Vector3.zero);
        //followNameUI = UGUIFollowManage.Instance.CreateItem(MonitorRangeManager.Instance.NameUI, targetTagObj, "MapAreasUI", null, false, false);
        //Text ntxt = followNameUI.GetComponentInChildren<Text>();
        //ntxt.text = info.Name;

        ////if (MonitorRangeManager.Instance.isShowRangeRender == false)
        ////{
        ////    SetFollowNameUIEnable(false);
        ////}

        SetSelectedUI(true);
    }



    public void OnDeselected(ObjectDeselectEventArgs deselectEventArgs)
    {
        //throw new NotImplementedException();
        //SetFollowNameUIEnable(false);
        SetSelectedUI(false);

    }

    public void OnAlteredByTransformGizmo(Gizmo gizmo)
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// 设置选中UI
    /// </summary>
    public void SetSelectedUI(bool isSelected)
    {
        if (followNameUI != null)
        {
            Image imageT = followNameUI.GetComponent<Image>();
            if (isSelected)
            {
                imageT.color = new Color(imageT.color.r, imageT.color.g, imageT.color.b, 1F);
            }
            else
            {
                imageT.color = oriFollowUIColor;
            }
        }
    }


    /// <summary>
    /// 是否在区域范围内
    /// </summary>
    public bool IsInRange(double x, double y)
    {
        //InitXZpointList();
        if (XZpointList == null) return true;
        List<double> vertx = new List<double>();
        List<double> verty = new List<double>();
        foreach (Vector2 v in XZpointList)
        {
            vertx.Add(v.x);
            verty.Add(v.y);
        }
        return PositionPnpoly(XZpointList.Count, vertx, verty, x, y);
    }

    /// <summary>
    /// 初始化区域范围XZ平面的点集合
    /// </summary>
    private void InitXZpointList()
    {
        Vector3 pos = transform.position;
        Vector3 realsize = gameObject.GetSize();
        XZpointList = new List<Vector2>();//点逆时针
        XZpointList.Add(new Vector2(pos.x - realsize.x / 2, pos.z - realsize.z / 2));
        XZpointList.Add(new Vector2(pos.x + realsize.x / 2, pos.z - realsize.z / 2));
        XZpointList.Add(new Vector2(pos.x + realsize.x / 2, pos.z + realsize.z / 2));
        XZpointList.Add(new Vector2(pos.x - realsize.x / 2, pos.z + realsize.z / 2));
    }

    /// <summary>
    /// 计算某个点是否在多边形内部
    /// </summary>
    /// <param name="nvert">不规则形状的顶点数</param>
    /// <param name="vertx">不规则形状x坐标集合</param>（点逆时针，顺时针没测过）
    /// <param name="verty">不规则形状y坐标集合</param>（点逆时针，顺时针没测过）
    /// <param name="testx">当前x坐标</param>
    /// <param name="testy">当前y坐标</param>
    /// <returns></returns>
    private bool PositionPnpoly(int nvert, List<double> vertx, List<double> verty, double testx, double testy)
    {
        int i, j, c = 0;
        for (i = 0, j = nvert - 1; i < nvert; j = i++)
        {
            if (((verty[i] > testy) != (verty[j] > testy)) && (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
            {
                c = 1 + c; ;
            }
        }
        if (c % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 计算区域在XZ平面,到位置点最短距离的点的坐标
    /// </summary>
    /// <param name="point">位置点</param>
    /// <param name="verts">区域底面顶点集合（顺，逆时针都可以）</param>
    /// <returns></returns>
    public Vector2 PointForPointToPolygon(Vector2 point)
    {
        return MonitorRangeManager.PointForPointToPolygon(point, XZpointList);
    }

    public void Focus()
    {
        ////Vector3 dir = transform.position - CameraSceneManager.Instance.alignCamera.transform.position;
        ////dir = new Vector3(dir.x, 0, dir.z);
        ////Quaternion quaDir = Quaternion.LookRotation(dir, Vector3.up);
        ////AlignTarget alignTarget = new AlignTarget(transform, new Vector2(60, quaDir.eulerAngles.y), 10, new Range(5, 90), new Range(1, 40));
        ////CameraSceneManager.Instance.FocusTarget(alignTarget);
        ////CameraSceneManager.Instance.FocusTargetWithTranslate(alignTarget, null, null);

        //Quaternion quaDir = Quaternion.LookRotation(-transform.forward, Vector3.up);
        //AlignTarget currentAlignTarget = CameraSceneManager.Instance.GetCurrentAlign();
        //Vector3 dir = new Vector3(60, quaDir.eulerAngles.y, 0);
        //Vector3 pos = Vector3.zero;
        //RaycastHit hit;

        //if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, LayerMask.GetMask("MouseTranslatePlane")))
        //{
        //    if (hit.collider != null)
        //    {
        //        pos = hit.point;
        //    }
        //}

        //CameraSceneManager.Instance.mouseTranslate.SetTranslatePosition(pos);
        //currentAlignTarget.distance = 10f;
        //currentAlignTarget.angles = dir;
        ////CameraSceneManager.Instance.FocusTargetWithTranslate(currentAlignTarget, null, null);
    }
}
