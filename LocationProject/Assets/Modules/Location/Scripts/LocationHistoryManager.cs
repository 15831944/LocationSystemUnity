using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using MonitorRange;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定位历史轨迹管理
/// </summary>
public class LocationHistoryManager : MonoBehaviour
{

    public static LocationHistoryManager Instance;
    private Transform historyPathParent;//历史记录父物体

    //public GameObject characterPrefab;//人员预设
    //public GameObject characterWomanPrefab;//女性人员预设

    public List<GameObject> TargetPrefabs;

    public GameObject NameUIPrefab;//名称UI预设
    public GameObject ArrowPrefab;//箭头预设

    /// <summary>
    /// 一般两点时间超过2秒，认为中间为无历史数据,用虚线表示
    /// </summary>
    public float IntervalTime = 10f;

    /// <summary>
    /// 是否开启人员打射线，来设置人员的高度，因为可能存在像主厂房二楼一样，地板平面高低不齐
    /// </summary>
    public bool isSetPersonHeightByRay = true;

    private bool isFocus;//是否聚焦
    public bool IsFocus
    {
        get
        {
            return isFocus;
        }
    }

    private HistoryManController currentFocusController;//当前聚焦项
    public HistoryManController CurrentFocusController
    {
        get
        {
            return currentFocusController;
        }
    }

    /// <summary>
    /// 摄像机聚焦前状态
    /// </summary>
    private AlignTarget beforeFocusAlign;

    private bool isMulHistory;//是否是多人历史轨迹
    public bool IsMulHistory
    {
        get
        {
            return isMulHistory;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //historyPaths = new List<LocationHistoryPath>();

#if !UNITY_EDITOR
        IntervalTime = SystemSettingHelper.locationSetting.HistoryIntervalTime;
#endif


    }

    // Update is called once per frame
    void Update()
    {

    }

    //private GameObject CreateCharacter()
    //{
    //    GameObject o = Instantiate(characterPrefab);
    //    //o.transform.SetParent(tagsParent);
    //    return o;
    //}

    //private GameObject CreateWomanCharacter()
    //{
    //    GameObject o = Instantiate(characterWomanPrefab);
    //    //o.transform.SetParent(tagsParent);


    //    return o;
    //}

    /// <summary>
    /// 创建历史轨迹父物体
    /// </summary>
    public Transform GetHistoryAllPathParent()
    {
        if (historyPathParent == null)
        {
            //historyPathParent = GameObject.Find("HistoryPathParent").transform;
            historyPathParent = new GameObject("HistoryPathParent").transform;
            return historyPathParent;
        }
        else
        {
            return historyPathParent;
        }
    }

    public event Action<GameObject,bool> FocusPersonChanged;

    protected void OnFocusPersonChanged(GameObject obj,bool isFocus)
    {
        if (FocusPersonChanged != null)
        {
            FocusPersonChanged(obj,isFocus);
        }
    }

    /// <summary>
    /// 拉近对焦某一个人员
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="afterFocus"></param>
    public void Focus(HistoryManController controller,Action afterFocus=null)
    {
        if (IsFocus)
        {
            ClearFollowUIState(currentFocusController);
            if (controller == currentFocusController)
            {
                RecoverBeforeFocusAlign();
                return;
            }
            else
            {
                currentFocusController.historyNameUI.SetCameraFollowToggleButtonActive(false);
                //ClearFollowUIState(currentFocusController);
                CameraSceneManager.Instance.SetTheThirdPersonCameraFalse();
            }
        }
        else
        {
            SetLinesActive(false);
        }
        SetFocusController(controller);
        AlignTarget alignTargetT = controller.GetAlignTarget();
        FocusPerson(alignTargetT, afterFocus);

        if(SceneAssetManager.Instance) SceneAssetManager.Instance.subject = controller.transform;

        LoadBuildingOfPerson(controller);//加载人物所在的建筑物

        OnFocusPersonChanged(controller.gameObject,true);
    }

    private void LoadBuildingOfPerson(HistoryManController controller)
    {
        LocationHistoryPath_M path_m = controller.GetComponent<LocationHistoryPath_M>();
        DepNode depnodeNow = MonitorRangeManager.GetDepNodeBuild(path_m.depnode);
        if (depnodeNow != null)
        {
            BuildingBox box = depnodeNow.GetComponent<BuildingBox>();
            if (box)
            {
                box.LoadBuilding((nNode) => {
                    FactoryDepManager.Instance.SetAllColliderIgnoreRaycastOP(true);
                }, false);
            }
        }
    }



    /// <summary>
    /// 清除跟随UI状态
    /// </summary>
    public void ClearFollowUIState(HistoryManController controller)
    {
        if (controller == null) return;
        controller.SetCameraFollowButtonEnable(false);
        controller.historyNameUI.CameraFollowToggleButton.RReset();
        controller.followUIbtn.RReset();
    }

    /// <summary>
    /// 聚焦人员
    /// </summary>
    private void FocusPerson(AlignTarget alignTargetT,Action afterFocus=null)
    {
        if (IsFocus == false)
        {
            beforeFocusAlign = CameraSceneManager.Instance.GetCurrentAlignTarget();
            //SetIsIsFocus(true);
            SetIsFocus(true);
            //SetExitFocusbtn(true);
        }
        SetFollowuiIsCheckCollider(IsFocus);
        IsClickUGUIorNGUI.Instance.SetIsCheck(false);//不关闭UI检测，会导致人员移动时，鼠标移动在UI上，场景出现异常
        CameraSceneManager.Instance.FocusTarget(alignTargetT, () =>
         {
             CameraSceneManager.Instance.alignCamera.SetisCameraCollider(true);
             currentFocusController.SetCameraFollowButtonEnable(true);
             if (MultHistoryPlayUINew.Instance.mode == HistoryMode.Normal)
             {
                 SetLinesActive(true);
                 RefleshDrawLine();
             }
             if (afterFocus != null)
             {
                 afterFocus();
             }
         });
    }

    /// <summary>
    /// 恢复在聚焦之前的摄像机状态
    /// </summary>
    public void RecoverBeforeFocusAlign()
    {
        RecoverFocus();
    }

    private void RecoverFocus()
    {
        if(SceneAssetManager.Instance) SceneAssetManager.Instance.subject = null;

        var temp = currentFocusController;

        CameraSceneManager.Instance.alignCamera.SetisCameraCollider(false);
        CameraSceneManager.Instance.SetTheThirdPersonCameraFalse();
        StartOutManage.Instance.HideBackButton();
        IsClickUGUIorNGUI.Instance.SetIsCheck(true);
        if (RoomFactory.Instance.IsFocusingDep)
        {
            //IsClickUGUIorNGUI.Instance.SetIsCheck(true);
        }
        else
        {
            if (currentFocusController == null) return;
            //currentFocusController.StopNavAgent();
            
            CameraSceneManager.Instance.FocusTarget(beforeFocusAlign, () =>
            {
                //IsClickUGUIorNGUI.Instance.SetIsCheck(true);
                //if (onComplete != null)
                //{
                //    onComplete();
                //}
                RefleshDrawLine();
            });

            //RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);
        }
        if (currentFocusController == null) return;

        SetIsFocus(false);
        //SetExitFocusbtn(false);
        SetFocusController(null);
        SetFollowuiIsCheckCollider(false);

        if (temp != null)
        {
            OnFocusPersonChanged(temp.gameObject, false);
        }
        else
        {
            OnFocusPersonChanged(null, false);
        }
    }

    /// <summary>
    /// 设置当前聚焦定位人员
    /// </summary>
    public void SetFocusController(HistoryManController controllerT)
    {
        currentFocusController = controllerT;
    }

    /// <summary>
    /// 设置是否聚焦
    /// </summary>
    /// <param name="b"></param>
    public void SetIsFocus(bool b)
    {
        if (isFocus == b) return;
        isFocus = b;
        if (isFocus)
        {
            FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
            FunctionSwitchBarManage.Instance.SetlightToggle(true);
            MultHistoryPlayUINew.Instance.SetBackViewBtnActive(true);
        }
        else
        {
            FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
            FunctionSwitchBarManage.Instance.SetlightToggle(false);
            MultHistoryPlayUINew.Instance.SetBackViewBtnActive(false);
        }
        //RefleshDrawLine();
    }

    /// <summary>
    /// 设置跟随UI检测视线遮挡碰撞
    /// </summary>
    public void SetFollowuiIsCheckCollider(bool IsCheck)
    {
        //foreach (LocationObject obj in code_character.Values)
        //{
        //    if (obj.personInfoUI == null) continue;
        //    UGUIFollowTarget follow = obj.personInfoUI.GetComponent<UGUIFollowTarget>();
        //    if (IsCheck)
        //    {
        //        if (obj == currentLocationFocusObj)
        //        {
        //            follow.SetIsRayCheckCollision(false);
        //            //Debug.LogError("SetFollowuiIsCheckCollider:"+obj.name);
        //            continue;//开启检测时，当前聚焦人物不检测
        //        }
        //    }

        //    if (obj.personInfoUI != null)
        //    {
        //        follow.SetIsRayCheckCollision(IsCheck);
        //    }
        //}
    }

    /// <summary>
    /// 设置是否是多人历史轨迹
    /// </summary>
    public void SetIsMulHistory(bool b)
    {
        isMulHistory = b;
    }

    /// <summary>
    /// 刷新轨迹
    /// </summary>
    public void RefleshDrawLine()
    {
        //foreach (LocationHistoryPath path in historyPaths)
        //{
        //    //path.RefleshDrawLineOP();
        //    StartCoroutine(path.RefleshDrawLineOP());
        //}

        //foreach (LocationHistoryPath_M path in historyPath_Ms)
        //{
        //    //path.RefleshDrawLineOP();
        //    StartCoroutine(path.RefleshDrawLineOP());
        //}

        PathList.RefleshDrawLine(this);
    }

    public void SetRateChanged(bool isChanged)
    {
        PathList.SetRateChanged(isChanged);
    }

    /// <summary>
    /// 设置显示隐藏
    /// </summary>
    public void SetLinesActive(bool isActive)
    {
        //foreach (LocationHistoryPath path in historyPaths)
        //{
        //    path.SetLinesActive(isActive);
        //}

        //foreach (LocationHistoryPath_M path in historyPath_Ms)
        //{
        //    path.SetLinesActive(isActive);
        //}

        PathList.SetLinesActive(isActive);
    }

    #region 单人历史轨迹相关管理

    ///// <summary>
    ///// 历史轨迹集合
    ///// </summary>
    //public List<LocationHistoryPath> historyPaths;

    /// <summary>
    /// 获取取定位卡历史位置信息
    /// </summary>
    /// <returns></returns>
    public LocationHistoryPath ShowLocationHistoryPath(PathInfo pathInfo, string name = "HistoryPathObj")
    {
        //GameObject o = CreateCharacter();
        GameObject o = CreatePersonObject(pathInfo.personnelT);
        LocationHistoryPath path = o.AddComponent<LocationHistoryPath>();
        o.name = pathInfo.personnelT.Name + "(" + pathInfo.personnelT.Tag.Code + ")";
        path.Init(pathInfo);
        o.SetActive(true);
        SetNavAgent(path);
        return path;
    }

    /// <summary>
    /// 添加历史轨迹路线
    /// </summary>
    public void AddHistoryPath(LocationHistoryPath path)
    {
        PathList.Add(path);
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    public void SetHistoryPath(float v)
    {
        //foreach (LocationHistoryPath hispath in historyPaths)
        //{
        //    hispath.Set(v);
        //}

        PathList.SetHistoryPath(v);
    }

    /// <summary>
    /// 清除历史轨迹路线
    /// </summary>
    public void ClearHistoryPaths()
    {
        //foreach (LocationHistoryPath path in historyPaths)
        //{
        //    DestroyImmediate(path.pathParent.gameObject);//人员是轨迹的子物体
        //    //DestroyImmediate(path.gameObject);
        //}

        //historyPaths.Clear();
        PathList.ClearHistoryPaths();

        SetFocusController(null);
        SetIsFocus(false);
    }

    #endregion

    #region 多人历史轨迹相关管理

    public LocationHistoryPathList PathList=new LocationHistoryPathList();

    /// <summary>
    /// 是否不存在任何路径
    /// </summary>
    /// <returns></returns>
    public bool IsPathEmpty()
    {
        return PathList == null || PathList.Count == 0;
    }

    public LocationHistoryPath_M GetCurrentPath()
    {
        return PathList.GetCurrentPath();
    }

    public int GetCurrentIndex()
    {
        return PathList.GetCurrentIndex();
    }

    public string GetCurrentPercent()
    {
        return PathList.GetCurrentPercent();
    }

    public PositionInfo SetCurrentIndex(int index)
    {
        return PathList.SetCurrentIndex(index);
    }

    ///// <summary>
    ///// 多人：显示历史轨迹
    ///// </summary>
    ///// <returns></returns>
    //public LocationHistoryPath_M ShowLocationHistoryPath_M(Personnel personnelT, List<Vector3> points, int segmentsT, Color color)
    //{
    //    //GameObject o = CreateCharacter();
    //    GameObject o = CreatePersonObject(personnelT);
    //    if (points.Count > 0)
    //    {
    //        o.transform.position = points[0];
    //    }
    //    LocationHistoryPath_M path = o.AddComponent<LocationHistoryPath_M>();
    //    o.name = personnelT.Name + "(" + personnelT.Tag.Code + ")";
    //    path.Init(personnelT, color, points, segmentsT);
    //    o.SetActive(true);

    //    SetNavAgent(path);
    //    return path;
    //}

    public void CreateHistoryPath(PathInfo pathInfo) 
    {
        //1.LocationHistoryPath_M
        var path = ShowLocationHistoryPath_M(pathInfo);
        AddHistoryPath_M(path);

        //2.HistoryManController
        var controller = path.gameObject.AddComponent<HistoryManController>();
        path.historyManController = controller;
        controller.Init(pathInfo.color, path);

        //3.人员行走动画
        var animationController = path.gameObject.GetComponent<PersonAnimationController>();
        animationController.DoMove();

        //if (PathFindingManager.Instance)
        //{
        //    PathFindingManager.Instance.StartNavAgent(controller);//直接一开始就切换
        //}
    }

    public LocationHistoryPath_M ShowLocationHistoryPath_M(PathInfo pathInfo)
    {
        var personnelT = pathInfo.personnelT;
        GameObject o = CreatePersonObject(personnelT);

        if (pathInfo.posList.Count > 0)
        {
            o.transform.position = pathInfo.posList[0].Vec;
        }
        LocationHistoryPath_M path = o.AddComponent<LocationHistoryPath_M>();
        o.name = personnelT.Name + "(" + personnelT.Tag.Code + ")";
        path.Init(pathInfo);
        o.SetActive(true);

        SetNavAgent(path);
        return path;
    }

    private GameObject CreatePersonObject(Personnel personnelT)
    {
        try
        {
            if (personnelT == null)
            {
                Log.Error("LocationHistoryManager.CreatePersonObject", "personnel == null");
                return null;
            }

            var type = personnelT.TargetType;
            var prefab = TargetPrefabs[type];

            GameObject o = Instantiate(prefab);
            //o.transform.SetParent(personnelT);
            return o;
        }
        catch (Exception e)
        {
            Log.Error("LocationHistoryManager.CreatePersonObject", "Exception:" + e);
            return null;
        }


        //GameObject o = null;
        //if (personnelT != null && personnelT.Sex == "2" && ThroughWallsManage.Instance.isCharacterControllerThroughWallsTest)//女性
        //{
        //    o = CreateWomanCharacter();
        //}
        //else
        //{
        //    o = CreateCharacter();
        //}

        //return o;
    }

    /// <summary>
    /// 添加历史轨迹路线
    /// </summary>
    public void AddHistoryPath_M(LocationHistoryPath_M path)
    {
        PathList.Add(path);
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    public void SetHistoryPath_M(float v)
    {
        PathList.SetHistoryPath_M(v);
    }

    /// <summary>
    /// 多人：清除历史轨迹路线
    /// </summary>
    public void ClearHistoryPaths_M()
    {
        PathList.ClearHistoryPaths_M();
        SetFocusController(null);
        SetIsFocus(false);
    }

    private void SetNavAgent(LocationHistoryPathBase o)
    {
        if (PathFindingManager.Instance)
        {
            PathFindingManager.Instance.SetNavAgent(o);
        }
    }

    #endregion

    public HistoryLineSetting LineSetting = new HistoryLineSetting();
}

[Serializable]
public class PathInfo
{
    public Personnel personnelT;
    public PositionInfoList posList;
    public Color color;
    public double timeLength;
}

[Serializable]
public class HistoryLineSetting
{
    public bool IsAuto = false;

    public float LineWidth = 1.5f;

    public float PointWidth = 1.5f;

    public float LineTransparent = 0.3f;

    public float PointTransparent = 1f;

    public int renderQueue = 4000;

    public bool DrawTestPoint = false;

    public bool DrawDottedline = true;

    /// <summary>
    /// 虚线点的密度
    /// </summary>
    public float PointDensity = 2f;

    /// <summary>
    /// 点的"混合"用颜色
    /// </summary>
    public Color PointColor = Color.red;

    /// <summary>
    /// 加大这个能让线根据圆滑，但是虚线和实线就分开了，而且会出现有中断的情况
    /// </summary>
    public float SegmentPower = 1f;
}
