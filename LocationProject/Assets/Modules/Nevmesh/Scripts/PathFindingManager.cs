using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager : MonoBehaviour
{
    public static PathFindingManager Instance;

    /// <summary>
    /// 使用NavAgent跟随人员，
    /// </summary>
    public bool useFollowNavAgent = true;

    /// <summary>
    /// 使用NavAgent设置人员位置
    /// </summary>
    public bool useNavAgent = false;

    public NavAgentFollowPerson FollowAgent;

    /// <summary>
    /// 女性人物
    /// </summary>
    public NavAgentFollowPerson FollowAgentFemale;

    public float MaxDistance = 15;

    public bool enableJump = true;

    public bool ShowOriginalPersonWhenEditor = true;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        LocationHistoryManager.Instance.FocusPersonChanged += Instance_FocusPersonChanged;
    }

    private void EnableArcReactor_Trail(GameObject obj)
    {
        if (ArcManager.Instance)
        {
            var parent = obj.transform;
            LocationHistoryPath_M path_m = obj.GetComponent<LocationHistoryPath_M>();
            if (path_m != null && path_m.navAgentFollow)
            {
                parent = path_m.navAgentFollow.transform;
            }
            ArcManager.Instance.EnableTrail(parent);
        }
    }

    private void Instance_FocusPersonChanged(GameObject obj,bool isFocus)
    {
        try
        {
            Log.Info("PathFindingManager.Instance_FocusPersonChanged:" + isFocus);
            if (isFocus)
            {
                EnableArcReactor_Trail(obj);//在人物脚下绘制运动轨迹

                //StartNavAgent();
                var bs = GameObject.FindObjectsOfType<BuildingController>();
                foreach (var b in bs)
                {
                    b.HideWalls();
                }
            }
            else
            {
                if (ArcManager.Instance)
                {
                    //ArcReactor_Trail.Instance.transform.parent = obj.transform;
                    //ArcReactor_Trail.Instance.transform.localPosition = Vector3.zero;
                    ArcManager.Instance.DisenableTrail();
                }

                //StopNavAgent();
                var bs = GameObject.FindObjectsOfType<BuildingController>();
                foreach (var b in bs)
                {
                    b.ShowWalls();
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("PathFindingManager.Instance_FocusPersonChanged", e.ToString());
        }
    }

    private void StartNavAgent()
    {
        var mans = GameObject.FindObjectsOfType<HistoryManController>();
        foreach (var item in mans)
        {
            //item.StartNavAgent();
            StartNavAgent(item);
        }
    }

    /// <summary>
    /// 对焦拉近切换到跟随人员
    /// </summary>
    /// <param name="man"></param>
    [ContextMenu("StartNavAgent")]
    public void StartNavAgent(HistoryManController man)
    {
        Log.Info("PathFindingManager.StartNavAgent");
        if (man == null)
        {
            Log.Error("PathFindingManager.StartNavAgent man == null");
            return;
        }
        LocationHistoryPath_M path_m = man.GetComponent<LocationHistoryPath_M>();
        
        if (path_m == null)
        {
            Log.Error("PathFindingManager.StartNavAgent path_m == null");
            return;
        }
        if (path_m.navAgentFollow)//有跟谁人员
        {
            path_m.navAgentFollow.MaxDistance = MaxDistance;
            path_m.navAgentFollow.enableJump = enableJump;

            if (ShowOriginalPersonWhenEditor)
            {
#if UNITY_EDITOR
                man.gameObject.SetTransparent(0.5f);
#else
    //man.DisableRenderer();
    man.DestroyRenderer();
#endif
            }
            else
            {
                //man.DisableRenderer();
                man.DestroyRenderer();//直接删除不需要再出现了
            }


            var target = path_m.navAgentFollow.gameObject;
            target.SetActive(true);

            var uiFollowTarget = UGUIFollowTarget.CreateTitleTag(target, new Vector3(0, 0.1f, 0));
            UGUIFollowTarget follow = man.followUI.GetComponent<UGUIFollowTarget>();
            follow.Target = uiFollowTarget;

            CameraSceneManager.Instance.FocusTarget(target.transform);

            target.HighlightOn();

            path_m.enableMoveByController = false;

            path_m.heightOffset = 0.55f;

            LocationHistoryManager.Instance.isSetPersonHeightByRay = false;

            man.followTarget = target.transform;
            man.followTitle = uiFollowTarget.transform;
        }
        //AroundAlignCamera.
    }


    [ContextMenu("StopNavAgent")]
    public void StopNavAgent(HistoryManController man)
    {
        LocationHistoryPath_M path_m = man.GetComponent<LocationHistoryPath_M>();
        if (path_m == null)
        {
            Log.Error("PathFindingManager.StopNavAgent path_m == null");
            return;
        }
        if (path_m.navAgentFollow)//有跟谁人员
        {
            man.EnableRenderer();

            var target = man.gameObject;
            path_m.navAgentFollow.gameObject.SetActive(false);

            var uiFollowTarget = UGUIFollowTarget.CreateTitleTag(target, new Vector3(0, 0.1f, 0));
            UGUIFollowTarget follow = man.followUI.GetComponent<UGUIFollowTarget>();
            follow.Target = uiFollowTarget;

            CameraSceneManager.Instance.FocusTarget(target.transform);

            target.HighlightOn();

            path_m.enableMoveByController = true;

            path_m.heightOffset = 0.85f;

            LocationHistoryManager.Instance.isSetPersonHeightByRay = false;//还是false

            man.followTarget = this.transform;
            man.followTitle = man.titleTag;
        }
    }

    private void StopNavAgent()
    {
        var mans = GameObject.FindObjectsOfType<HistoryManController>();
        foreach (var item in mans)
        {
            //item.StopNavAgent();
            StopNavAgent(item);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public static void EnableRenderer(GameObject go)
    {
        Renderer[]  renderers = go.GetComponentsInChildren<Renderer>();
        foreach (var render in renderers)
        {
            render.enabled = false;
        }
    }

    public static void DisableRenderer(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        foreach (var render in renderers)
        {
            render.enabled = false;
        }
    }

    public void SetNavAgent(LocationObject o)
    {
        if (SystemSettingHelper.locationSetting.EnableNavMesh == false) return;//不启用NavMesh
        if (o == null) return;
        if (useNavAgent)
        {
            var agent = o.gameObject.AddComponent<NavAgentController>();//往原来物体上加NavAgentController来控制物体移动
            o.navAgent = agent;
        }
        else if (useFollowNavAgent)
        {
            if (FollowAgent)
            {
                if (o.navAgentFollow == null)
                {
                    Log.Debug("PathFindingManager.SetNavAgent",string.Format("{0},{1},{2}",o.name,o.transform.position,o.gameObject.activeInHierarchy));

                    var agent = GameObject.Instantiate<NavAgentFollowPerson>(FollowAgent);//创建一个Agent跟随
                    agent.name = o.gameObject.name + "(Nav)";
                    agent.gameObject.layer = o.gameObject.layer;
                    agent.gameObject.tag = o.gameObject.tag;
                    //agent.transform.parent = o.CreatePathParent();
                    agent.transform.position = o.transform.position;
                    agent.transform.parent = o.transform.parent;


                    o.navAgentFollow = agent;
                    agent.SetFollowTarget(o.transform);
                    //agent.gameObject.SetActive(false);

                    //o.uiTarget = agent.gameObject;//UI跟谁的目标

                    DisableRenderer(o.gameObject);
                }
            }
        }
        else
        {
            //不使用NavMesh
        }
    }

    public void SetNavAgent(LocationHistoryPathBase o)
    {
        Log.Error("PathFindingManager", "SetNavAgent");
        if (SystemSettingHelper.locationSetting.EnableNavMesh == false) return;//不启用NavMesh
        if (o == null) return;
        if (useNavAgent)
        {
            var agent = o.gameObject.AddComponent<NavAgentController>();//往原来物体上加NavAgentController来控制物体移动
            o.navAgent = agent;
        }
        else if (useFollowNavAgent)
        {
            if (FollowAgent)
            {
                if (o.navAgentFollow == null)
                {


                    var agent = GameObject.Instantiate<NavAgentFollowPerson>(FollowAgent);//创建一个Agent跟随
                    agent.name = o.gameObject.name + "(Nav)";
                    agent.gameObject.layer = o.gameObject.layer;
                    agent.gameObject.tag = o.gameObject.tag;

                    agent.transform.position = o.transform.position;
                    agent.transform.parent = o.CreatePathParent();
                    o.navAgentFollow = agent;
                    agent.SetFollowTarget(o.transform);

                    agent.gameObject.SetActive(false);

                    if (o is LocationHistoryPath_M)
                    {
                        LocationHistoryPath_M path = o as LocationHistoryPath_M;
                        if (path.historyPathDrawing != null)
                        {
                            path.historyPathDrawing.target = agent.transform;//绘图目标修改
                        }

                    }
                }
            }
        }
        else
        {
            //不使用NavMesh
        }
    }
}
