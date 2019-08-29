using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

public class LocationHistoryPathBase : HistoryPath
{

    /// <summary>
    /// 编码
    /// </summary>
    public string code;
    /// <summary>
    /// 人员信息
    /// </summary>
    public Personnel personnel;
    ///// <summary>
    ///// 路径点数，这里的点数跟传入的点无关
    ///// </summary>
    //protected int segments = 250;
    ///// <summary>
    ///// 路径点数最大值设为16000;
    ///// VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
    ///// </summary>
    //public static int segmentsMax = 16000;
    ///// <summary>
    ///// 路径是否循环
    ///// </summary>
    //public bool doLoop = true;
    ///// <summary>
    ///// 路径演变物体
    ///// </summary>
    //public Transform obj;
    /// <summary>
    /// 路径演变速度
    /// </summary>
    public float speed = 0.8f;
    ///// <summary>
    ///// 路径颜色
    ///// </summary>
    //protected Color color;
    ///// <summary>
    ///// 实际点位置
    ///// </summary>
    //protected List<Vector3> splinePoints;
    //protected List<DateTime> timelist;//点的时间集合
    //protected List<List<Vector3>> splinePointsList;
    //protected List<List<DateTime>> timelistLsit;

    ///// <summary>
    ///// 路径是否闭合
    ///// </summary>
    //public bool pathLoop = false;

    //protected List<VectorLine> lines;
    //protected List<VectorLine> dottedlines;

    protected double timeLength;//播放时间长度，单位秒
    protected double progressValue;//轨迹播放进度值
    protected double progressTargetValue;//轨迹播放目标值
    protected double timeStart;//轨迹播放起始时间Time.time

    protected Collider collider;//碰撞器
    protected Renderer render;//render

    protected bool IsShowRenderer = true;//是否显示了Renderer

    protected GameObject followUI;//跟随UI

    //public Transform pathParent;//该历史路径的父物体

    //protected bool isCreatePathComplete;//创建历史轨迹是否完成
    public int currentPointIndex = 0;

    /// <summary>
    /// 当前点和上一点的时间差(s)
    /// </summary>
    public double CurrentPosInterval
    {
        get
        {
            if (currentPointIndex - 1 >= 0 && currentPointIndex < PosCount)
            {
                double temp = (PosInfoList[currentPointIndex].Time - PosInfoList[currentPointIndex - 1].Time).TotalSeconds;
                return temp;
            }
            else
            {
                return -1;
            }
        }
    }

    protected Renderer[] renders;

    protected Quaternion targetQuaternion;//目标转向

    public bool isCanShowPerson = true;//是否可以显示人员

    public DepNode depnode;//当前所在区域
    /// <summary>
    /// CharacterController：控制人物移动，
    /// </summary>
    PersonMove personmove;

    protected override void Start()
    {
        try
        {
            Log.Info("LocationHistoryPathBase.Start Start");//2019_06_26_cww:昨天做代码重构，提取了PositionInfo PositionInfoList PositionInfoGroup，崩溃了，加打印语句，加着加着就不崩溃了。上次崩溃时是有运行到这里的。
            personmove = gameObject.GetComponent<PersonMove>();
            StartInit();
            //将方向转换为四元数
            targetQuaternion = Quaternion.LookRotation(Vector3.zero, Vector3.up);

            CreateHistoryPath();

            isCreatePathComplete = true;
            //if (MultHistoryPlayUI.Instance.isNewWalkPath)
            //{
            //    //CreatePathSphere();
            //}
            Log.Info("LocationHistoryPathBase.Start End");
        }
        catch (Exception ex)
        {
            Log.Error("LocationHistoryPathBase.Start",ex.ToString());
        }
        
    }

    /// <summary>
    /// 创建历史轨迹（轨迹线）
    /// </summary>
    private void CreateHistoryPath()
    {
        Log.Info("CreateHistoryPath Start");
        bool isNormalMode = LocationHistoryUITool.GetIsNormalMode();

        if (PosInfoGroup != null)
        {
            foreach (PositionInfoList positionInfoList in PosInfoGroup)
            {
                try
                {
                    Log.Info("CreateHistoryPath Group Start");
                    var posList = positionInfoList.GetVector3List();
                    //var posList = positionInfoList.GetNavMeshPosList();//改成根据NavMesh上的点获取坐标
                    GameObject o = CreateHistoryPath(posList, posList.Count - 1, isNormalMode);//创建历史轨迹（轨迹线）
                    Log.Info("CreateHistoryPath Group End");
                }
                catch (Exception ex1)
                {
                    Log.Error("LocationHistoryPathBase.CreateHistoryPath", ex1.ToString());
                }
            }
        }
        else
        {
            Log.Error("LocationHistoryPathBase.CreateHistoryPath", "PosInfoGroup=null");
        }

        Log.Info("CreateHistoryPath End");
    }

    /// <summary>
    /// 创建路径球，用于测试轨迹画的对不对
    /// </summary>
    public void CreatePathSphere()
    {
        GameObject op = new GameObject("PathSpheresw");
        op.transform.SetParent(transform.parent);
        op.transform.position = Vector3.zero;

        GameObject ot = Instantiate(LocationHistoryManager.Instance.ArrowPrefab);
        Renderer r = ot.GetComponentInChildren<Renderer>();
        color = new Color(color.r, color.g, color.b, 0.7f);
        r.material.SetColor("_TintColor", color);

        int i = 0;
      
        foreach (PositionInfo pi in PosInfoList)
        {
            //GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject o = Instantiate(ot);
            var p = pi.Vec;
            o.transform.SetParent(op.transform);
            o.transform.position = new Vector3(p.x, p.y, p.z);
            //o.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            if (i < PosCount - 1)
            {
                o.transform.forward = PosInfoList[i + 1].Vec - PosInfoList[i].Vec;
            }
            i++;
        }

        GameObject.DestroyImmediate(ot);
    }

    ///// <summary>
    ///// 创建历史轨迹
    ///// </summary>
    //private void CreateHistoryPath(List<Vector3> splinePointsT, int segmentsT)
    //{

    //    //VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
    //    VectorLine line = new VectorLine("Spline", new List<Vector3>(segmentsT + 1), 2f, LineType.Continuous);
    //    lines.Add(line);
    //    //line.lineColors
    //    line.color = color;
    //    line.MakeSpline(splinePointsT.ToArray(), segmentsT, pathLoop);
    //    SetLineTransparentAndDottedline();
    //    line.Draw3D();
    //    //line.Draw3DAuto();
    //    GameObject lineObjT = line.rectTransform.gameObject;
    //    lineObjT.transform.SetParent(pathParent);
    //    Renderer r = lineObjT.GetComponent<Renderer>();
    //    r.material.SetColor("_TintColor", color);//默认透明度是0.5,这里改为1；
    //    r.material.SetFloat("_InvFade", 0.01f);//原本是1，改为0.2，让线绘制的更加柔和，不会出现断裂
    //    r.material.renderQueue = 4000;//默认透明度是3000,这里改为4000；让透明物体先渲染，该轨迹后渲染，效果会更好
    //}

    ///// <summary>
    ///// 创建历史轨迹中检测不到的虚线轨迹
    ///// </summary>
    //private void CreateHistoryPathDottedline(List<Vector3> splinePointsT, int segmentsT)
    //{

    //    //VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
    //    VectorLine line = new VectorLine("Spline", new List<Vector3>(segmentsT + 1), 4f, LineType.Points);
    //    dottedlines.Add(line);
    //    //line.lineColors
    //    line.color = color;
    //    line.MakeSpline(splinePointsT.ToArray(), segmentsT, pathLoop);
    //    line.Draw3D();
    //    //line.Draw3DAuto();
    //    GameObject lineObjT = line.rectTransform.gameObject;
    //    lineObjT.name = "dottedline";
    //    lineObjT.transform.SetParent(pathParent);
    //    Renderer r = lineObjT.GetComponent<Renderer>();
    //    r.material.SetColor("_TintColor", color);//默认透明度是0.5,这里改为1；
    //    r.material.SetFloat("_InvFade", 0.01f);//原本是1，改为0.2，让线绘制的更加柔和，不会出现断裂
    //    r.material.renderQueue = 4000;//默认透明度是3000,这里改为4000；让透明物体先渲染，该轨迹后渲染，效果会更好
    //}

    protected override void StartInit()
    {
        Log.Info("LocationHistoryPathBase.StartInit Start");
        lines = new List<VectorLine>();
        dottedlines = new List<VectorLine>();
        CreatePathParent();
        //LocationHistoryManager.Instance.AddHistoryPath(this as LocationHistoryPath);
        transform.SetParent(pathParent);
        if (PosCount <= 1) return;
        render = gameObject.GetComponent<Renderer>();
        renders = gameObject.GetComponentsInChildren<Renderer>();
        collider = gameObject.GetComponent<Collider>();

        targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, new Vector3(0, 0.1f, 0));
        followUI = UGUIFollowManage.Instance.CreateItem(LocationHistoryManager.Instance.NameUIPrefab, targetTagObj, "LocationNameUI", null, true);
        Text nametxt = followUI.GetComponentInChildren<Text>();
        nametxt.text = name;

        GroupingLine();
        Log.Info("LocationHistoryPathBase.StartInit End");
    }

    GameObject targetTagObj;


    protected override void Update()
    {
        base.Update();
        //RefleshDrawLine();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (MultHistoryPlayUINew.Instance.isPlay)
        {
            //缓慢转动到目标点
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.fixedDeltaTime * 10);
        }
    }

    //private bool isScrollWheel;//滚轮是否滚动

    ///// <summary>
    ///// 这里用来触发视角旋转完毕或，切换完毕，需要重新画一下线（这里线是个平面，不同视角不一样）
    ///// 还有中键滚轮滚动结束，也需要重新绘制一下
    ///// </summary>
    //protected void RefleshDrawLine()
    //{
    //    float mouseScrollWheelValue = Input.GetAxis("Mouse ScrollWheel");
    //    Debug.Log("mouseScrollWheelValue:" + mouseScrollWheelValue);
    //    if (mouseScrollWheelValue != 0)
    //    {
    //        isScrollWheel = true;
    //    }

    //    bool isScrollWheelEnd = false;
    //    if (mouseScrollWheelValue != 0 && isScrollWheel)
    //    {
    //        isScrollWheelEnd = true;//是否是滚轮滚动结束
    //    }

    //    if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2) || isScrollWheelEnd)
    //    {
    //        //RefleshDrawLineOP();
    //        StartCoroutine(RefleshDrawLineOP());
    //    }
    //}

    //protected IEnumerator RefleshDrawLineOP()
    //{
    //    foreach (VectorLine line in lines)
    //    {
    //        line.Draw3D();
    //        yield return null;
    //    }

    //    foreach (VectorLine line in dottedlines)
    //    {
    //        //line.AddNormals();
    //        line.Draw3D();
    //        yield return null;
    //    }
    //}

    private int previousCurrentPointIndex = -1;
    Vector3 previousPos;
    int count = 0;
    float timesum = 0;
    float timesub = 0;
    bool rateChanged;//执行过程中，切换过倍数

    public void SetRateChanged(bool b)
    {
        rateChanged = b;
    }

    /// <summary>
    /// 清除上一个的缓存信息
    /// </summary>
    public void ClearPreviousInfo()
    {
        previousCurrentPointIndex = -1;
        previousPos = transform.position;
        count = 0;
        timesum = 0;
        timesub = 0;
    }

    private HisPosInfo hisPosInfo;

    /// <summary>
    /// 执行轨迹演变
    /// </summary>
    protected PositionInfo ExcuteHistoryPath(int currentIndex, float rateT = 1f, bool isLerp = true)
    {
        HisPosInfo info = new HisPosInfo();
        info.PreHisInfo = hisPosInfo;
        hisPosInfo = info;
        hisPosInfo.currentIndex = currentIndex;

        //Debug.Log("ExcuteHistoryPath:"+ currentIndex);
        HighlightTestPoint(currentIndex);

        //if (!MultHistoryPlayUI.Instance.isNewWalkPath)
        //{

        #region 用画线插件来设置人员在轨迹上的位置

        //ExcuteHistoryPathOP(currentIndex, isLerp);

        #endregion

        //}
        //else
        //{

        #region 自己计算设置人员在轨迹上的位置


        if (rateChanged)
        {
            previousPos = transform.position;
            SetRateChanged(false);
            timesub = timesub - timesum * rateT;
            timesum = 0;
        }

        if (previousCurrentPointIndex != currentIndex)
        {
            count = 0;
            timesum = 0;
            previousCurrentPointIndex = currentIndex;
            if (currentIndex - 1 >= 0 && currentIndex - 1 < PosCount)
            {
                hisPosInfo.PrePosInfo = PosInfoList[currentIndex - 1];
                previousPos = PosInfoList[currentIndex - 1].Vec;
                timesub = (float) PosInfoList.GetTimeSpane(currentPointIndex);
            }
        }

        count++;
        timesum += Time.deltaTime;
        PositionInfo posInfo = PosInfoList[currentIndex];
        hisPosInfo.CurrentPosInfo = posInfo;


        Vector3 targetPos = posInfo.Vec;
        Vector3 showPos = targetPos;
        if (isLerp && currentIndex > 0)
        {
            //showPos = Vector3.Lerp(previousPos, targetPos, count * speed * rateT * Time.fixedDeltaTime);//speed小的话会导致计算出来的演变点比实际要演变的点慢很多（两个位置点差距比较大）
            if (timesub == 0)
            {
                showPos = Vector3.Lerp(transform.position, targetPos, count * speed * rateT * Time.deltaTime);
                //Debug.LogError("timesub == 0!!!！！");
            }
            else
            {
                showPos = Vector3.Lerp(previousPos, targetPos, timesum * rateT / timesub);
            }
        }
        else
        {
            showPos = targetPos;
        }

        //Debug.LogError("显示位置：" + showPos + ",索引：" + currentIndex + "，起始点：" + previousPos + ",计数：" + count + ",系数：" + count * speed * rateT * Time.deltaTime + ",speed:" + speed + ",Time.fixedDeltaTime" + Time.fixedDeltaTime);
        //Debug.LogError("timesum / timesub:" + timesum / timesub);

        SetRotaion(targetPos); //设置旋转角度
        //transform.position = new Vector3(showPos.x, showPos.y - historyOffsetY, showPos.z);//减去0.4是为了让人员高度贴近地面，通常为人的一半高度

        if (LocationHistoryManager.Instance.isSetPersonHeightByRay)
        {
            showPos = SetPersonHeightByRay(showPos);
        }
        else
        {
            showPos = new Vector3(showPos.x, showPos.y - heightOffset, showPos.z); //卡拿着的大约高度
        }
        //transform.position = showPos;

        hisPosInfo.TargetPos = targetPos;
        hisPosInfo.ShowPos = showPos;
        hisPosInfo.CurrentPos = transform.position;
        SetPosition(hisPosInfo, rateT);

        #endregion

        //}
        return posInfo;
    }

    /// <summary>
    /// 设置旋转角度
    /// </summary>
    /// <param name="targetPos"></param>
    private void SetRotaion(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        if (dir != Vector3.zero)
        {
            //将方向转换为四元数
            targetQuaternion = Quaternion.LookRotation(dir, Vector3.up); //targetQuaternion在LateUpdate里面缓慢旋转
            //缓慢转动到目标点
            //transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.fixedDeltaTime * 10);
        }
    }

    public float heightOffset = 0.85f;

    public float maxMoveLength = 1.5f;

    /// <summary>
    /// 是否使用CharactorController
    /// </summary>
    public bool enableMoveByController = true;

    List<GameObject> testPoints = new List<GameObject>();

    GameObject lastTestPoint = null;

    private Vector3 testPointScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 testPointScale2 = new Vector3(0.3f, 0.3f, 0.3f);
    private void HighlightTestPoint(int i)
    {
        if (!SystemSettingHelper.IsDebug()) return;
        if (LocationHistoryManager.Instance.LineSetting.DrawTestPoint == false) return;
        if (lastTestPoint != null)
        {
            lastTestPoint.HighlightOff();
            lastTestPoint.transform.DOScale(testPointScale, 1);
            lastTestPoint.SetColor(new Color(0.7f,0.7f,0.7f,1f));
            lastTestPoint.RecoverTransparent();
        }
        lastTestPoint = testPoints[i];
        testPoints[i].HighlightOn();
        lastTestPoint.transform.DOScale(testPointScale2, 1);
        lastTestPoint.SetColor(Color.green);
        //if (ArcReactor_Arc.Instance != null)
        //{
        //    ArcReactor_Arc.Instance.AddTransform(lastTestPoint.transform, 10);
        //}
    }

    private void ShowTestPoint()
    {
        if (!SystemSettingHelper.IsDebug()) return;
        if (LocationHistoryManager.Instance.LineSetting.DrawTestPoint == false) return;
        try
        {
            if (null == testPointsRoot)
            {
                testPointsRoot = new GameObject("testPointsRoot");
                testPointsRoot.transform.parent = CreatePathParent();
            }

            for (int i = 0; i < PosCount; i++)
            {
                GameObject p = CreateTestPoint(i, PosInfoList[i]);
                testPoints.Add(p);
            }


            //if (ArcReactor_Arc.Instance != null)
            //{
            //    ArcReactor_Arc.Instance.shapeTransforms = new Transform[0];
            //    List<Vector3> ps = new List<Vector3>();
            //    for (int i = 0; i < PosCount && i < 20; i++)
            //    {
            //        ps.Add(PosInfoList[i].Vec);
            //        //GameObject p = CreateTestPoint(i, PosInfoList[i].Vec);
            //        //testPoints.Add(p);
            //    }

            //    ArcReactor_Arc.Instance.shapePoints = ps.ToArray();
            //}
        }
        catch (Exception ex)
        {
            Debug.LogError("LocationHistoryPathBase.ShowTestPoint:" + ex);
        }
    }

    [ContextMenu("CreateNavAgent")]
    private void CreateNavAgent()
    {
        navAgent = gameObject.AddComponent<NavAgentController>();
    }

    private void SetPosition(HisPosInfo hisPosInfo, float rateT)
    {
        Vector3 showPos = hisPosInfo.ShowPos;
        if (navAgent && useNavAgent) //使用NavAgent修改位置
        {
            navAgent.SetDestination(hisPosInfo, rateT);//新的设置位置的有效代码
        }
        else
        {
            //原来的有效代码
            float disT = Vector3.Distance(showPos, transform.position);
            if (disT < maxMoveLength && enableMoveByController)//如果当前位置与目标位置超过三维里的一个单位(不考虑y轴方向)，就可以进行穿墙移动,小于一个单位会被阻挡
            {

                if (personmove != null)
                {
                    personmove.SetPosition_History(showPos);//CharactorController控制走路，会被墙等物体挡住。
                }
            }
            else
            {
                transform.position = showPos;//根据历史轨迹走路
            }
        }

        if (navAgentFollow)//跟谁人员，不用管原来的代码，设置跟随人员的位置就行
        {
            navAgentFollow.SetDestination(hisPosInfo, rateT);
        }
    }

    public bool useNavAgent = true;

    /// <summary>
    /// 当前的
    /// </summary>
    public NavAgentControllerBase navAgent;

    /// <summary>
    /// 跟随的
    /// </summary>
    public NavAgentFollowPerson navAgentFollow;


    private GameObject testPointsRoot;

    private List<GameObject> testPointList = new List<GameObject>();

    private GameObject CreateTestPoint(int id, PositionInfo pos)
    {
        GameObject pObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pObj.transform.position = pos.Vec;
        if (testPointsRoot)
        {
            pObj.transform.parent = testPointsRoot.transform;
        }
        else
        {
            pObj.transform.parent = this.transform.parent;
        }
        pObj.transform.localScale = testPointScale;
        pObj.name = string.Format("[{0}]{1}|{2}", id, pos.Vec,pos.Time.ToString("HH:mm:ss.f"));
        pObj.SetTransparent(0.5f);
        Collider collider = pObj.GetComponent<Collider>();
        GameObject.Destroy(collider);
        return pObj;
    }


    /// <summary>
    /// 设置人员高度，通过打射线。来判断最近的楼层地板来设置位置
    /// </summary>
    public Vector3 SetPersonHeightByRay(Vector3 targetPos)
    {
        Vector3 targetPosTT = targetPos;
        float yHeight = targetPos.y;

        float downFloorHeight = targetPos.y;
        bool isRaycasDownFloor = false;
        RaycastHit downhit;
        if (Physics.Raycast(targetPos, Vector3.down, out downhit, 200f, LayerMask.GetMask("Floor")))
        {
            downFloorHeight = downhit.point.y;
            isRaycasDownFloor = true;
        }

        if (!isRaycasDownFloor)//如果第一次向下射线检测没有打到地板，就位置上移0.3个单位打射线
        {
            Vector3 targetPosT = new Vector3(targetPos.x, targetPos.y + 0.3f, targetPos.z);
            if (Physics.Raycast(targetPosT, Vector3.down, out downhit, 200f, LayerMask.GetMask("Floor")))
            {
                downFloorHeight = downhit.point.y;
                isRaycasDownFloor = true;
            }
        }

        float upFloorHeight = targetPos.y;
        bool isRaycasUpFloor = false;
        RaycastHit uphit;
        if (Physics.Raycast(targetPos, Vector3.up, out uphit, 200f, LayerMask.GetMask("Floor")))
        {
            upFloorHeight = uphit.point.y;
            isRaycasUpFloor = true;
        }

        if (isRaycasUpFloor)//如果第一次向下射线检测打到打到地板，
        {
            if (!isRaycasDownFloor || (upFloorHeight - yHeight) < (yHeight - downFloorHeight))
            {
                float offsetT = 0.5f;
                Vector3 targetPosT = new Vector3(targetPos.x, upFloorHeight + offsetT, targetPos.z);
                if (Physics.Raycast(targetPosT, Vector3.down, out uphit, 200f, LayerMask.GetMask("Floor")))//改变打射线方向，位置上移0.3个单位向下打射线
                {
                    if (Mathf.Abs(uphit.point.y - upFloorHeight) < offsetT)
                    {
                        upFloorHeight = uphit.point.y;
                        isRaycasUpFloor = true;
                    }
                }
            }
        }

        //Debug.DrawLine(transform.position, transform.position + Vector3.up * 50, Color.green, 1f);
        //float yTemp = (upFloorHeight - yHeight) <= (yHeight - downFloorHeight) ? upFloorHeight : downFloorHeight;
        float yTemp = targetPos.y;
        if (isRaycasDownFloor || isRaycasUpFloor)
        {
            if (isRaycasDownFloor && isRaycasUpFloor)
            {
                yTemp = (upFloorHeight - yHeight) <= (yHeight - downFloorHeight) ? upFloorHeight : downFloorHeight;
            }
            else
            {
                if (isRaycasDownFloor)
                {
                    yTemp = downFloorHeight;
                }
                else if (isRaycasUpFloor)
                {
                    yTemp = upFloorHeight;
                }
            }
        }

        //float yLerp = Mathf.Lerp(transform.position.y, yTemp, LocationManager.Instance.damper * Time.deltaTime);
        //transform.position = new Vector3(transform.position.x, yLerp, transform.position.z);
        //if (yTemp > 0.8f) return;//如果上下调整距离需要超过0.8个单位，就不上下移动了
        yTemp = yTemp + 0.005f;//为了防止人物脚部与地板碰撞发生抖动，可以适当向上偏移一点点
        targetPos = new Vector3(targetPos.x, yTemp, targetPos.z);

        return targetPos;
    }

    /// <summary>
    /// 是否NavMesh跟随
    /// </summary>
    /// <returns></returns>
    public bool IsNavAgentFollow()
    {
        return navAgentFollow == null ? false : true;
    }
    private float historyOffsetY = 0.4f;

    /// <summary>
    /// 设置第一个点的位置
    /// </summary>
    public void SetFisrtPos()
    {
        if (PosCount > 0)
        {
            var p = PosInfoList[0].Vec;
            transform.position = new Vector3(p.x, p.y - historyOffsetY, p.z);
        }
    }

    /// <summary>
    /// 用画线插件来设置人员在轨迹上的位置
    /// </summary>
    /// <param name="currentPointIndexT"></param>
    /// <param name="isLerp"></param>
    private void ExcuteHistoryPathOP(int currentPointIndexT, bool isLerp)
    {
        Vector3 targetPos;
        //Debug.Log("currentPointIndex:" + currentPointIndex);
        //progressTargetValue = (float)currentIndex / segmentsMax;
        progressTargetValue = (double)(currentPointIndexT + 1) / PosCount;
        if (isLerp)
        {
            progressValue = Mathf.Lerp((float)progressValue, (float)progressTargetValue, speed * Time.deltaTime);
        }
        else
        {
            progressValue = progressTargetValue;
        }
        int n = Mathf.FloorToInt((float)progressValue);//
        targetPos = lines[n].GetPoint3D01((float)(progressValue - n));
        SetRotaion(targetPos);
        transform.position = targetPos;
    }

    ///// <summary>
    ///// 初始化时间长度,秒为单位
    ///// </summary>
    //public void InitData(double timeLengthT)
    //{
    //    timeLength = timeLengthT;
    //    //timelist = timelistT;
    //}

    /// <summary>
    /// 初始化编码
    /// </summary>
    /// <param name="codeT"></param>
    public void InitCode(string codeT)
    {
        code = codeT;
    }

    ///// <summary>
    ///// 初始化
    ///// </summary>
    //public void Init(Personnel personnelT, Color colorT, List<Vector3> splinePointsT, int segmentsT, float speedT, bool doLoopT, bool pathLoopT)
    //{
    //    personnel = personnelT;
    //    code = personnel.Tag.Code;
    //    segments = segmentsT;
    //    doLoop = doLoopT;
    //    //obj = cubeT;
    //    speed = speedT;
    //    splinePoints = splinePointsT;
    //    color = colorT;
    //    pathLoop = pathLoopT;
    //}

    ///// <summary>
    ///// 初始化
    ///// </summary>
    //public void Init(Personnel personnelT, Color colorT, List<Vector3> splinePointsT, int segmentsT)
    //{
    //    personnel = personnelT;
    //    code = personnel.Tag.Code;
    //    //splinePoints = splinePointsT;
    //    color = colorT;
    //    segments = segmentsT;
    //    ShowTestPoint();
    //}

    public void Init(PathInfo pathInfo)
    {
        personnel = pathInfo.personnelT;
        code = personnel.Tag.Code;
        //obj = cubeT;
        PosInfoList = pathInfo.posList;
        color = pathInfo.color;
        segments = PosCount;

        timeLength = pathInfo.timeLength;
        ShowTestPoint();
    }

    ///// <summary>
    ///// 数据点太多创建，数据拆开以便创建多条轨迹
    ///// 因为创建一条连续线最多为16383个点
    ///// </summary>
    //public void GroupingLine(int segmentsMaxT = 16000)
    //{
    //    if (splinePoints.Count != timelist.Count)
    //    {
    //        return;
    //    }

    //    int n = splinePoints.Count / segmentsMaxT;
    //    if (splinePoints.Count % segmentsMaxT > 0)
    //    {
    //        n += 1;
    //    }

    //    for (int i = 0; i < n; i++)
    //    {
    //        if (i < n - 1)
    //        {
    //            List<Vector3> listT = splinePoints.GetRange(i * segmentsMaxT, segmentsMaxT);
    //            splinePointsList.Add(listT);
    //        }
    //        else
    //        {
    //            List<Vector3> listT = splinePoints.GetRange(i * segmentsMaxT, splinePoints.Count - i * segmentsMaxT);
    //            splinePointsList.Add(listT);
    //        }
    //    }

    //    for (int i = 0; i < n; i++)
    //    {
    //        if (i < n - 1)
    //        {
    //            List<DateTime> listT = timelist.GetRange(i * segmentsMaxT, segmentsMaxT);
    //            timelistLsit.Add(listT);
    //        }
    //        else
    //        {
    //            List<DateTime> listT = timelist.GetRange(i * segmentsMaxT, splinePoints.Count - i * segmentsMaxT);
    //            timelistLsit.Add(listT);
    //        }
    //    }

    //    //if (splinePoints.Count> 16000)
    //    //{
    //    //    splinePointsList.AddRange(splinePoints.GetRange())
    //    //}

    //}

    protected bool isNeedHide;//当获取的currentPointIndex值小于0，等于-1

    /// <summary>
    /// 设置轨迹执行位置,点的索引
    /// </summary>
    public void Set(float value)
    {
        //isSetHistoryPath = true;
        Loom.StartSingleThread(() =>
        {
            SetCurrentIndexByProcess(value);//根据进度条的指设置currentIndex
            Loom.DispatchToMainThread(() =>
            {
                MoveToCurrentIndex();
            });
        });

    }

    /// <summary>
    /// 移动到某一个点
    /// </summary>
    /// <param name="index"></param>
    public PositionInfo MoveToCurrentIndex(int index)
    {
        //Debug.Log("MoveToCurrentIndex:"+ index);
        currentPointIndex = index;
        return MoveToCurrentIndex();
    }

    public PositionInfo MoveToCurrentIndex()
    {
        PositionInfo posInfo = null;
        //Debug.Log("MoveToCurrentIndex");
        if (isNeedHide)
        {
            isNeedHide = false;
            Hide();
        }
        else
        {
            //Debug.Log("currentPointIndex:" + currentPointIndex);
            //if (currentPointIndex > 0 && currentPointIndex < splinePoints.Count)
            if (currentPointIndex >= 0 && currentPointIndex < PosCount)
            {
                Show();
                posInfo = ExcuteHistoryPath(currentPointIndex, 1f, false);
            }
            else
            {
                Hide();
            }
        }

        return posInfo;
        //isSetHistoryPath = false;
    }

    private void SetCurrentIndexByProcess(float value)
    {
        int r = GetCurrentIndex(value);
        if (r >= 0)//大于0表示能找到点
        {
            currentPointIndex = r;
        }
        else
        {
            int indexT = GetNextPoint(value);//如果等于-1，则后面没有数据点了
            if (indexT == 0)//如果拖动到首个点数据之前，是需要将人员隐藏的，这里的currentPointIndex值设为-1
            {
                //currentPointIndex = -1;
                currentPointIndex = 0;
                isNeedHide = true;
            }
            else
            {
                //r = indexT;
                currentPointIndex = indexT;
            }
            //if (indexT >= 0)
            //{
            //    currentPointIndex = indexT;

            //}
            //Hide();
        }
    }

    public void Show()
    {
        if (isCanShowPerson == false) return;
        if (collider.enabled != true)
        {
            collider.enabled = true;
        }
        //if (render.enabled != true)
        //{
        //    render.enabled = true;
        //}
        SetRenderIsEnable(true);
        SetFollowUI(true);
    }

    public virtual void Hide()
    {
        if (collider.enabled != false)
        {
            collider.enabled = false;
        }
        //if (render.enabled != false)
        //{
        //    render.enabled = false;
        //}
        SetRenderIsEnable(false);
        SetFollowUI(false);
    }

    /// <summary>
    /// 设置跟随UI的显示隐藏
    /// </summary>
    protected void SetFollowUI(bool b)
    {
        if (followUI == null) return;
        followUI.SetActive(b);
    }

    /// <summary>
    /// 获取离它最近的下一个播放点
    /// </summary>
    public virtual int GetNextPoint(float value)
    {
        //DateTime startTimeT = HistoryPlayUI.Instance.GetStartTime();
        //double f = timeLength * value;
        ////相匹配的第一个元素,结果为-1表示没找到
        //return timelist.FindIndex((item) =>
        //{
        //    double timeT = (item - startTimeT).TotalSeconds;
        //    if (timeT > f)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //});
        return 0;
    }

    /// <summary>
    /// 根据进度值，获取当前需要执行的点的索引
    /// </summary>
    public int GetCurrentIndex(float value)
    {
        double f = timeLength * value;
        int r = GetCompareTime(f, 0.5f);
        if (r >= 0)
        {
            return r;
        }
        //r = GetCompareTime(f, 1f);
        //if (r >= 0)
        //{
        //    return r;
        //}
        r = GetCompareTime(f, 3f);
        if (r >= 0)
        {
            return r;
        }
        return -1;

    }

    /// <summary>
    /// 根据进度值，获取当前需要执行的点的索引
    /// </summary>
    /// <param name="f"></param>
    /// <param name="accuracy">精确度：时间相差accuracy秒</param>
    public virtual int GetCompareTime(double f, float accuracy = 0.1f)
    {
        //DateTime startTimeT = HistoryPlayUI.Instance.GetStartTime();
        ////相匹配的第一个元素,结果为-1表示没找到
        //return timelist.FindIndex((item) =>
        //{
        //    double timeT = (item - startTimeT).TotalSeconds;
        //    if (Math.Abs(f - timeT) < accuracy)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //});

        return 0;
    }

    //public Color32 CCOLOR;

    ///// <summary>
    ///// 设置部分线透明并创建虚线，表示无历史数据，一般两点时间超过10秒，认为中间为无历史数据
    ///// </summary>
    //public void SetLineTransparentAndDottedline()
    //{

    //    Dictionary<List<Vector3>, double> dottedlines = new Dictionary<List<Vector3>, double>();
    //    for (int i = 0; i < timelist.Count; i++)
    //    {
    //        if (i < timelist.Count - 1)
    //        {
    //            double secords = (timelist[i + 1] - timelist[i]).TotalSeconds;
    //            if (secords > 10)//一般两点时间超过10秒，认为中间为无历史数据
    //            {
    //                try
    //                {
    //                    List<Vector3> ps = new List<Vector3>();
    //                    int n = i / segmentsMax;
    //                    int nf = i % segmentsMax;
    //                    if (nf == segmentsMax && i != 0)//考虑两条线的分界点的情况
    //                    {
    //                        //lines[n-1].SetColor(new Color32(0, 0, 0, 0), nf, nf + 1);
    //                        //lines[n].SetColor(new Color32(0, 0, 0, 0), nf, nf + 1);
    //                        //ps.Add(splinePoints[i]);
    //                        //ps.Add(splinePoints[i + 1]);
    //                    }
    //                    else
    //                    {
    //                        //Color32 colorT = color;
    //                        //colorT = new Color32(colorT.r, colorT.g, colorT.b, (byte)80);
    //                        //CCOLOR = colorT;
    //                        //lines[n].SetColor(colorT, nf, nf + 1);
    //                        lines[n].SetColor(new Color32(0, 0, 0, 0), nf, nf + 1);
    //                        ps.Add(splinePoints[i]);
    //                        ps.Add(splinePoints[i + 1]);
    //                    }
    //                    //Debug.LogError("SetLineTransparent!");
    //                    dottedlines.Add(ps, secords);
    //                }
    //                catch
    //                {
    //                    int m = 0;
    //                }
    //            }
    //        }
    //    }

    //    //根据两点距离画虚线
    //    foreach (List<Vector3> vList in dottedlines.Keys)
    //    {
    //        Vector3 p1 = vList[0];
    //        Vector3 p2 = vList[1];

    //        float dis = Vector3.Distance(p1, p2);
    //        float unit = 2f;
    //        float nfloat = dis / unit;//无数据轨迹每隔0.2个单位画一个点
    //        int n = (int)Math.Round(nfloat, 0) + 1;
    //        if (n % 2 > 0)
    //        {
    //            n += 1;
    //        }
    //        if (n < 2)
    //        {
    //            n = 2;
    //        }
    //        if (n > segmentsMax)//要是超过最大点数可以考虑分组建（基本不可能超过），所以不用考虑
    //        {
    //            n = segmentsMax;
    //        }
    //        CreateHistoryPathDottedline(vList, n);
    //    }
    //}

    /// <summary>
    /// 创建路径父物体
    /// </summary>
    public override Transform CreatePathParent()
    {
        if (pathParent == null)
        {
            Transform parent = LocationHistoryManager.Instance.GetHistoryAllPathParent();
            pathParent = new GameObject("" + code).transform;
            pathParent.transform.SetParent(parent);
        }

        if (lineParent == null)
        {
            lineParent = new GameObject("HistoryLines").transform;
            lineParent.transform.SetParent(pathParent);
        }
        return pathParent;
    }

    /// <summary>
    /// 设置Render显示隐藏
    /// </summary>
    public virtual void SetRenderIsEnable(bool isEnable)
    {
        if (IsShowRenderer != isEnable)
        {
            foreach (Renderer render in renders)
            {
                render.enabled = isEnable;
            }
            IsShowRenderer = isEnable;
        }
    }

    /// <summary>
    /// 设置是否可以显示人员
    /// </summary>
    public void SetIsCanShowPerson(bool b)
    {
        isCanShowPerson = b;
    }


    #region 计算历史轨迹人员的所在区域






    #endregion


}
