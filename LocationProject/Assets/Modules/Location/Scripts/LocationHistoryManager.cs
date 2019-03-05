using Location.WCFServiceReferences.LocationServices;
using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定位历史轨迹管理
/// </summary>
public class LocationHistoryManager : MonoBehaviour {

    public static LocationHistoryManager Instance;
    private Transform historyPathParent;//历史记录父物体
    public GameObject characterPrefab;//人员预设
    public GameObject NameUIPrefab;//名称UI预设
    public GameObject ArrowPrefab;//箭头预设

    public float IntervalTime = 2f;//一般两点时间超过2秒，认为中间为无历史数据,用虚线表示

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

    // Use this for initialization
    void Start () {
        Instance = this;
        historyPaths = new List<LocationHistoryPath>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}



    private GameObject CreateCharacter()
    {
        GameObject o = Instantiate(characterPrefab);
        //o.transform.SetParent(tagsParent);
        return o;
    }

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

    public void Focus(HistoryManController controller)
    {
        if (IsFocus)
        {
            if (controller == currentFocusController)
            {
                RecoverBeforeFocusAlign();
                return;
            }
        }
        SetFocusController(controller);
        AlignTarget alignTargetT = controller.GetAlignTarget();
        FocusPerson(alignTargetT);
    }

    /// <summary>
    /// 聚焦人员
    /// </summary>
    private void FocusPerson(AlignTarget alignTargetT)
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
        CameraSceneManager.Instance.FocusTarget(alignTargetT,()=>
        {
            RefleshDrawLine();
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

        StartOutManage.Instance.HideBackButton();
        IsClickUGUIorNGUI.Instance.SetIsCheck(true);
        if (RoomFactory.Instance.IsFocusingDep)
        {
            //IsClickUGUIorNGUI.Instance.SetIsCheck(true);
        }
        else
        {
            if (currentFocusController == null) return;
            CameraSceneManager.Instance.FocusTarget(beforeFocusAlign, () =>
            {
                //IsClickUGUIorNGUI.Instance.SetIsCheck(true);
                //if (onComplete != null)
                //{
                //    onComplete();
                //}
            });

            //RoomFactory.Instance.FocusNode(FactoryDepManager.Instance);
        }
        if (currentFocusController == null) return;

        SetIsFocus(false);
        //SetExitFocusbtn(false);
        SetFocusController(null);
        SetFollowuiIsCheckCollider(false);
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
        isFocus = b;
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
        foreach (LocationHistoryPath path in historyPaths)
        {
            //path.RefleshDrawLineOP();
            StartCoroutine(path.RefleshDrawLineOP());
        }

        foreach (LocationHistoryPath_M path in historyPath_Ms)
        {
            //path.RefleshDrawLineOP();
            StartCoroutine(path.RefleshDrawLineOP());
        }
    }

    #region 单人历史轨迹相关管理

    /// <summary>
    /// 历史轨迹集合
    /// </summary>
    public List<LocationHistoryPath> historyPaths;

    /// <summary>
    /// 获取取定位卡历史位置信息
    /// </summary>
    /// <returns></returns>
    public LocationHistoryPath ShowLocationHistoryPath(Personnel personnelT, List<Vector3> points, int segmentsT, Color color, string name = "HistoryPathObj")
    {
        GameObject o = CreateCharacter();
        LocationHistoryPath path = o.AddComponent<LocationHistoryPath>();
        o.name = personnelT.Name + "(" + personnelT.Tag.Code + ")";
        path.Init(personnelT, color, points, segmentsT);
        o.SetActive(true);

        return path;
    }

    /// <summary>
    /// 添加历史轨迹路线
    /// </summary>
    public void AddHistoryPath(LocationHistoryPath path)
    {
        historyPaths.Add(path);
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    public void SetHistoryPath(float v)
    {
        foreach (LocationHistoryPath hispath in historyPaths)
        {
            hispath.Set(v);
        }
    }

    /// <summary>
    /// 清除历史轨迹路线
    /// </summary>
    public void ClearHistoryPaths()
    {
        foreach (LocationHistoryPath path in historyPaths)
        {
            DestroyImmediate(path.pathParent.gameObject);//人员是轨迹的子物体
            //DestroyImmediate(path.gameObject);
        }

        historyPaths.Clear();

        SetFocusController(null);
        SetIsFocus(false);
    }

    #endregion

    #region 多人历史轨迹相关管理

    public List<LocationHistoryPath_M> historyPath_Ms;// 历史轨迹集合

    /// <summary>
    /// 多人：显示历史轨迹
    /// </summary>
    /// <returns></returns>
    public LocationHistoryPath_M ShowLocationHistoryPath_M(Personnel personnelT, List<Vector3> points, int segmentsT, Color color)
    {
        GameObject o = CreateCharacter();
        LocationHistoryPath_M path = o.AddComponent<LocationHistoryPath_M>();
        o.name = personnelT.Name + "(" + personnelT.Tag.Code + ")";
        path.Init(personnelT, color, points, segmentsT);
        o.SetActive(true);
        return path;
    }

    /// <summary>
    /// 添加历史轨迹路线
    /// </summary>
    public void AddHistoryPath_M(LocationHistoryPath_M path)
    {
        historyPath_Ms.Add(path);
    }

    /// <summary>
    /// 设置历史轨迹执行的值
    /// </summary>
    public void SetHistoryPath_M(float v)
    {
        foreach (LocationHistoryPath_M hispath in historyPath_Ms)
        {
            hispath.Set(v);
        }
    }

    /// <summary>
    /// 多人：清除历史轨迹路线
    /// </summary>
    public void ClearHistoryPaths_M()
    {
        foreach (LocationHistoryPath_M path in historyPath_Ms)
        {
            DestroyImmediate(path.pathParent.gameObject);//人员是轨迹的子物体
            //DestroyImmediate(path.gameObject);
        }

        historyPath_Ms.Clear();
        SetFocusController(null);
        SetIsFocus(false);
    }

    #endregion
}
