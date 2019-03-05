using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
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

    protected Renderer[] renders;

    protected Quaternion targetQuaternion;//目标转向


    protected override void Start()    {
        StartInit();
        //将方向转换为四元数
        targetQuaternion = Quaternion.LookRotation(Vector3.zero, Vector3.up);
        //if (segments >= 16000)
        //{
        //    segments = 16000;
        //}
        foreach (List<Vector3> splinePointsT in splinePointsList)
        {
            CreateHistoryPath(splinePointsT, splinePointsT.Count - 1);
        }
        isCreatePathComplete = true;
        //if (MultHistoryPlayUI.Instance.isNewWalkPath)
        //{
        //    //CreatePathSphere();
        //}
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
        foreach (Vector3 p in splinePoints)
        {
            //GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject o = Instantiate(ot);

            o.transform.SetParent(op.transform);
            o.transform.position = new Vector3(p.x, p.y, p.z);
            //o.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            if (i < splinePoints.Count - 1)
            {
                o.transform.forward = splinePoints[i + 1] - splinePoints[i];
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
    //    line.color = color;    //    line.MakeSpline(splinePointsT.ToArray(), segmentsT, pathLoop);    //    SetLineTransparentAndDottedline();
    //    line.Draw3D();
    //    //line.Draw3DAuto();    //    GameObject lineObjT = line.rectTransform.gameObject;
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
    //    line.color = color;    //    line.MakeSpline(splinePointsT.ToArray(), segmentsT, pathLoop);
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
        lines = new List<VectorLine>();
        dottedlines = new List<VectorLine>();
        splinePointsList = new List<List<Vector3>>();        timelistLsit = new List<List<DateTime>>();        CreatePathParent();        //LocationHistoryManager.Instance.AddHistoryPath(this as LocationHistoryPath);
        transform.SetParent(pathParent);        if (splinePoints.Count <= 1) return;
        render = gameObject.GetComponent<Renderer>();
        renders = gameObject.GetComponentsInChildren<Renderer>();
        collider = gameObject.GetComponent<Collider>();

        GameObject targetTagObj = UGUIFollowTarget.CreateTitleTag(gameObject, new Vector3(0, 0.1f, 0));
        followUI = UGUIFollowManage.Instance.CreateItem(LocationHistoryManager.Instance.NameUIPrefab, targetTagObj, "LocationNameUI");
        Text nametxt = followUI.GetComponentInChildren<Text>();
        nametxt.text = name;

        GroupingLine();
    }


    protected override void Update()    {
        base.Update();
        //RefleshDrawLine();
    }
    protected override void FixedUpdate()    {
        base.FixedUpdate();
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        //缓慢转动到目标点
        transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, Time.fixedDeltaTime * 10);
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
    /// <summary>
    /// 执行轨迹演变
    /// </summary>
    protected void ExcuteHistoryPath(int currentPointIndexT, float rateT = 1f, bool isLerp = true)
    {
        //if (!MultHistoryPlayUI.Instance.isNewWalkPath)
        //{
        #region 用画线插件来设置人员在轨迹上的位置
        //ExcuteHistoryPathOP(currentPointIndexT, isLerp);
        #endregion
        //}
        //else
        //{
        #region 自己计算设置人员在轨迹上的位置

        if (previousCurrentPointIndex != currentPointIndexT)
        {
            count = 0;
            timesum = 0;
            previousCurrentPointIndex = currentPointIndexT;
            if (currentPointIndexT - 1 >= 0 && currentPointIndexT - 1 < splinePoints.Count)
            {
                previousPos = splinePoints[currentPointIndexT - 1];
                timesub = (float)(timelist[currentPointIndexT] - timelist[currentPointIndexT - 1]).TotalSeconds;
            }
        }
        count++;
        timesum += Time.deltaTime;
        Vector3 targetPos = splinePoints[currentPointIndexT];
        Vector3 showPos = targetPos;
        if (isLerp)
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

        //Debug.LogError("显示位置：" + showPos + ",索引：" + currentPointIndexT + "，起始点：" + previousPos + ",计数：" + count + ",系数：" + count * speed * rateT * Time.deltaTime + ",speed:" + speed + ",Time.fixedDeltaTime" + Time.fixedDeltaTime);
        //Debug.LogError("timesum / timesub:" + timesum / timesub);

        //Vector3 dir = showPos - transform.position;
        Vector3 dir = targetPos - transform.position;

        dir = new Vector3(dir.x, 0, dir.z);
        if (dir != Vector3.zero)
        {
            //将方向转换为四元数
            targetQuaternion = Quaternion.LookRotation(dir, Vector3.up);
            //缓慢转动到目标点
            //transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.fixedDeltaTime * 10);
        }
        //transform.position = showPos;
        transform.position = new Vector3(showPos.x, showPos.y - 0.4f, showPos.z);//减去0.4是为了让人员高度贴近地面，通常为人的一半高度
        #endregion
        //}
    }

    private void ExcuteHistoryPathOP(int currentPointIndexT, bool isLerp)
    {
        Vector3 targetPos;
        //Debug.Log("currentPointIndex:" + currentPointIndex);
        //progressTargetValue = (float)currentPointIndexT / segmentsMax;
        progressTargetValue = (double)(currentPointIndexT + 1) / splinePoints.Count;
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
        Vector3 dir = targetPos - transform.position;

        dir = new Vector3(dir.x, 0, dir.z);
        if (dir != Vector3.zero)
        {
            //将方向转换为四元数
            targetQuaternion = Quaternion.LookRotation(dir, Vector3.up);
            //缓慢转动到目标点
            //transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.fixedDeltaTime * 10);
        }
        transform.position = targetPos;
    }

    /// <summary>
    /// 初始化时间长度,秒为单位
    /// </summary>
    public void InitData(double timeLengthT, List<DateTime> timelistT)
    {
        timeLength = timeLengthT;
        timelist = timelistT;
    }

    /// <summary>
    /// 初始化编码
    /// </summary>
    /// <param name="codeT"></param>
    public void InitCode(string codeT)
    {
        code = codeT;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Personnel personnelT, Color colorT, List<Vector3> splinePointsT, int segmentsT, float speedT, bool doLoopT, bool pathLoopT)
    {
        personnel = personnelT;
        code = personnel.Tag.Code;
        segments = segmentsT;
        doLoop = doLoopT;
        //obj = cubeT;
        speed = speedT;
        splinePoints = splinePointsT;
        color = colorT;
        pathLoop = pathLoopT;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Personnel personnelT, Color colorT, List<Vector3> splinePointsT, int segmentsT)
    {
        personnel = personnelT;
        code = personnel.Tag.Code;
        //obj = cubeT;
        splinePoints = splinePointsT;
        color = colorT;
        segments = segmentsT;
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

    /// <summary>
    /// 设置轨迹执行位置,点的索引
    /// </summary>
    public void Set(float value)
    {
        //isSetHistoryPath = true;
        Loom.StartSingleThread(() =>
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
                    currentPointIndex = -1;
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
            Loom.DispatchToMainThread(() =>
            {
                //Debug.Log("currentPointIndex:" + currentPointIndex);
                if (currentPointIndex >= 0)
                {
                    ExcuteHistoryPath(currentPointIndex, 1f, false);
                }
                else
                {
                    Hide();
                }
                //isSetHistoryPath = false;
            });
        });

    }

    public void Show()
    {
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
    private void SetFollowUI(bool b)
    {
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
    protected virtual int GetCompareTime(double f, float accuracy = 0.1f)
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
        return pathParent;
    }

    /// <summary>
    /// 设置Render显示隐藏
    /// </summary>
    public void SetRenderIsEnable(bool isEnable)
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



    #region 计算历史轨迹人员的所在区域






    #endregion
}
