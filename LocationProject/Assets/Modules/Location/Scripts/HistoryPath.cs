using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class HistoryPath : MonoBehaviour {

    ///// <summary>
    ///// 编码
    ///// </summary>
    //public string code;
    ///// <summary>
    ///// 人员信息
    ///// </summary>
    //protected Personnel personnel;
    /// <summary>
    /// 路径点数，这里的点数跟传入的点无关
    /// </summary>
    protected int segments = 250;
    /// <summary>
    /// 路径点数最大值设为16000;
    /// VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
    /// </summary>
    public static int segmentsMax = 16000;
    /// <summary>
    /// 路径是否循环
    /// </summary>
    public bool doLoop = true;
    ///// <summary>
    ///// 路径演变速度
    ///// </summary>
    //public float speed = .05f;
    /// <summary>
    /// 路径颜色
    /// </summary>
    protected Color color;
    ///// <summary>
    ///// 实际点位置
    ///// </summary>
    //[HideInInspector]
    //public List<Vector3> splinePoints;//所有点的集合
    //                                  //[HideInInspector]
    //public List<DateTime> timelist;//所有点的时间集合

    //protected List<List<DateTime>> timelistLsit;

    //protected List<List<Vector3>> splinePointsList;

    [HideInInspector]
    public PositionInfoList PosInfoList;//所有点的集合

    public int PosCount
    {
        get
        {
            if (PosInfoList == null) return -1;
            return PosInfoList.Count;
        }
    }

    protected PositionInfoGroup PosInfoGroup = new PositionInfoGroup();

    /// <summary>
    /// 路径是否闭合
    /// </summary>
    public bool pathLoop = false;

    public List<VectorLine> lines;
    public List<VectorLine> dottedlines;

    //protected double timeLength;//播放时间长度，单位秒
    //protected double progressValue;//轨迹播放进度值
    //protected double progressTargetValue;//轨迹播放目标值
    //protected double timeStart;//轨迹播放起始时间Time.time

    //protected Collider collider;//碰撞器
    //protected Renderer render;//render

    //protected bool IsShowRenderer = true;//是否显示了Renderer

    //protected GameObject followUI;//跟随UI

    public Transform pathParent;//该历史路径的父物体

    public Transform lineParent;//轨迹线集合的父物体

    protected bool isCreatePathComplete;//创建历史轨迹是否完成
                                        //protected int currentPointIndex = 0;
                                        //protected Renderer[] renders;

    //protected Quaternion targetQuaternion;//目标转向
    protected bool isScrollWheel;//滚轮是否滚动

    protected virtual void Start()    {

    }

    public void Init(Color colorT, bool pathLoopT)
    {
        color = colorT;
        pathLoop = pathLoopT;
    }

    [ContextMenu("RefreshLine")]
    public void RefreshLine()
    {

    }

    private void Draw3DLine(VectorLine line)
    {
        if (LocationHistoryManager.Instance.LineSetting.IsAuto)
        {
            line.Draw3DAuto();//点线多了 确实会影响性能
        }
        else
        {
            line.Draw3D();
        }
    }

    private GameObject SetLineParent(VectorLine line)
    {
        GameObject lineObjT = line.rectTransform.gameObject;
        //lineObjT.transform.SetParent(pathParent);
        lineObjT.transform.SetParent(lineParent);
        return lineObjT;
    }

    private void Set3DLineMaterial(VectorLine line, Color color2,float transparent)
    {
        GameObject lineObjT = line.rectTransform.gameObject;
        Renderer r = lineObjT.GetComponent<Renderer>();

        //var transparent = 0.1f;//0.7f->0.1f
        //color = new Color(color.r, color.g, color.b, transparent);
        Color color3 = new Color(color2.r, color2.g, color2.b, transparent);
        r.material.SetColor("_TintColor", color3);//默认透明度是0.5,这里改为0.7；
        //r.material.SetFloat("_InvFade", 0.15f);//原本是1，改为0.2，让线绘制的更加柔和，不会出现断裂

        r.material.renderQueue = LocationHistoryManager.Instance.LineSetting.renderQueue;//默认透明度是3000,这里改为4000；让透明物体先渲染，该轨迹后渲染，效果会更好
    }

    private VectorLine Create3DLine(List<Vector3> points, int segmentsT,float width, LineType lineType)
    {
        segmentsT = (int)(segmentsT*LocationHistoryManager.Instance.LineSetting.SegmentPower);
        //VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
        VectorLine line = new VectorLine("Spline", new List<Vector3>(segmentsT + 1), width, lineType);
        lines.Add(line);
        //line.lineColors
        line.color = color;
        line.MakeSpline(points.ToArray(), segmentsT , pathLoop);
        return line;
    }

    /// <summary>
    /// 创建历史轨迹（轨迹线）
    /// </summary>
    /// <param name="points">点集合</param>
    /// <param name="segmentsT">多少段，比点数量少1</param>
    protected GameObject CreateHistoryPath(List<Vector3> points, int segmentsT,bool isActive=true)
    {
        if (points == null)
        {
            Log.Error("HistoryPath.CreateHistoryPath points == null");
            return null;
        }

        //var width = 1.5f;
        var width = LocationHistoryManager.Instance.LineSetting.LineWidth;
        VectorLine line = Create3DLine(points, segmentsT, width, LineType.Continuous);

        if (LocationHistoryManager.Instance.LineSetting.DrawDottedline)
        {
            SetLineTransparentAndDottedline(isActive);//设置部分线透明并创建虚线，表示无历史数据，一般两点时间超过10秒，认为中间为无历史数据
        }       
        Draw3DLine(line);//具体用Draw3D还是Draw3DAuto，在里面统一修改
        GameObject lineObjT= SetLineParent(line);
        var transparent = LocationHistoryManager.Instance.LineSetting.LineTransparent;
        Set3DLineMaterial(line,color, transparent);
        if (!isActive)
        {
            lineObjT.SetActive(isActive);
        }
        return lineObjT;
    }



    /// <summary>
    /// 创建轨迹间连接线
    /// </summary>
    /// <param name="splinePointsT">点集合</param>
    /// <param name="segmentsT">多少段，比点数量少1</param>
    protected void CreatePathLink(List<Vector3> splinePointsT, int segmentsT)
    {
        //var width = 1.5f;
        var width = LocationHistoryManager.Instance.LineSetting.LineWidth;
        VectorLine line = Create3DLine(splinePointsT, segmentsT, width, LineType.Continuous);
        line.name = "LineLink";
        Draw3DLine(line);
        SetLineParent(line);
        var transparent = LocationHistoryManager.Instance.LineSetting.LineTransparent;
        Set3DLineMaterial(line, color, transparent);
    }

    /// <summary>
    /// 创建历史轨迹中检测不到的虚线轨迹
    /// </summary>
    protected void CreateHistoryPathDottedline(List<Vector3> splinePointsT, int segmentsT,bool isActive=true)
    {
        var width = LocationHistoryManager.Instance.LineSetting.PointWidth;
        VectorLine line = Create3DLine(splinePointsT, segmentsT, width, LineType.Points);
        Draw3DLine(line);
        GameObject lineObjT = SetLineParent(line);
        lineObjT.name = "dottedline";
        var transparent = LocationHistoryManager.Instance.LineSetting.PointTransparent;
        Color pointColor = LocationHistoryManager.Instance.LineSetting.PointColor;
        Color colorNew = new Color((color.r + pointColor.r) / 2, (color.g + pointColor.g) / 2, (color.b + pointColor.b) / 2);//点的颜色区分一下
        Set3DLineMaterial(line, colorNew, transparent);
        if (!isActive)
        {
            lineObjT.SetActive(isActive);
        }
    }



    protected virtual void StartInit()
    {

    }


    protected virtual void Update()    {
        RefleshDrawLine();
    }

    protected virtual void FixedUpdate()    {
    }

    protected virtual void LateUpdate()
    {

    }

    /// <summary>
    /// 这里用来触发视角旋转完毕或，切换完毕，需要重新画一下线（这里线是个平面，不同视角不一样）
    /// 还有中键滚轮滚动结束，也需要重新绘制一下
    /// </summary>
    public void RefleshDrawLine(bool isReflesh=false)
    {
        float mouseScrollWheelValue = Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log("mouseScrollWheelValue:" + mouseScrollWheelValue);
        if (mouseScrollWheelValue != 0)
        {
            isScrollWheel = true;
        }

        bool isScrollWheelEnd = false;
        if (mouseScrollWheelValue != 0 && isScrollWheel)
        {
            isScrollWheelEnd = true;//是否是滚轮滚动结束
        }

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2) || isScrollWheelEnd|| isReflesh)
        {
            //RefleshDrawLineOP();
            StartCoroutine(RefleshDrawLineOP());
        }
    }

    /// <summary>
    /// 摄像头视角改变后，刷新轨迹。
    /// 主要是拉远拉近时轨迹要刷新 不然 拉远会变细 拉近会变粗。
    /// </summary>
    /// <returns></returns>
    public IEnumerator RefleshDrawLineOP()
    {
        Debug.Log("HistoryPath.RefleshDrawLineOP");

        if (LocationHistoryManager.Instance.LineSetting.IsAuto == false)
        {
            foreach (VectorLine line in lines)
            {
                line.Draw3D();
                yield return null;
            }

            foreach (VectorLine line in dottedlines)
            {
                //line.AddNormals();
                line.Draw3D();
                //yield
            }
            //yield return null;
        }
        yield return null;
    }

    public void SetLinesActive(bool isActive)
    {
        foreach (VectorLine line in lines)
        {
            line.rectTransform.gameObject.SetActive(isActive);
        }

        foreach (VectorLine line in dottedlines)
        {
            line.rectTransform.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// 数据点太多创建，数据拆开以便创建多条轨迹
    /// 因为创建一条连续线最多为16383个点
    /// </summary>
    public void GroupingLine(int segmentsMaxT = 16000)
    {
        Log.Info("LocationHistoryPathBase.GroupingLine Start");
        if (PosInfoList == null)
        {
            Debug.LogError("HistoryPath.GroupingLine PosInfoList == null");
            return;
        }
        //if (splinePoints.Count != timelist.Count)
        //{
        //    return;
        //}

        int n = PosCount / segmentsMaxT;
        if (PosCount % segmentsMaxT > 0)
        {
            n += 1;
        }

        for (int i = 0; i < n; i++)
        {
            if (i < n - 1)
            {
                var listT = PosInfoList.GetRange(i * segmentsMaxT, segmentsMaxT);
                PosInfoGroup.AddList(listT);
            }
            else
            {
                var listT = PosInfoList.GetRange(i * segmentsMaxT, PosCount - i * segmentsMaxT);
                PosInfoGroup.AddList(listT);
            }
        }

        Log.Info("LocationHistoryPathBase.GroupingLine End");

        //for (int i = 0; i < n; i++)
        //{
        //    if (i < n - 1)
        //    {
        //        List<DateTime> listT = timelist.GetRange(i * segmentsMaxT, segmentsMaxT);
        //        timelistLsit.Add(listT);
        //    }
        //    else
        //    {
        //        List<DateTime> listT = timelist.GetRange(i * segmentsMaxT, splinePoints.Count - i * segmentsMaxT);
        //        timelistLsit.Add(listT);
        //    }
        //}

        //if (splinePoints.Count> 16000)
        //{
        //    splinePointsList.AddRange(splinePoints.GetRange())
        //}

    }

    /// <summary>
    /// 设置部分线透明并创建虚线，表示无历史数据，一般两点时间超过10秒，认为中间为无历史数据
    /// </summary>
    private void SetLineTransparentAndDottedline(bool isActive = true)
    {

        Dictionary<List<Vector3>, double> dottedlines = new Dictionary<List<Vector3>, double>();
        for (int i = 0; i < PosCount; i++)
        {
            if (i < PosCount - 1)
            {
                double seconds = (PosInfoList[i + 1].Time - PosInfoList[i].Time).TotalSeconds;
                if (seconds > LocationHistoryManager.Instance.IntervalTime)//一般两点时间超过10秒，认为中间为无历史数据
                {
                    try
                    {
                        List<Vector3> ps = new List<Vector3>();
                        int n = i / segmentsMax;
                        int nf = i % segmentsMax;
                        if (nf == segmentsMax - 1 && lines.Count - 1 > n)//考虑两条线的分界点的情况
                        {
                            //lines[n+1].SetColor(new Color32(0, 0, 0, 0), nf);
                            //ps.Add(splinePoints[i]);
                            //ps.Add(splinePoints[i + 1]);
                        }
                        else
                        {
                            //Color32 colorT = color;
                            //colorT = new Color32(colorT.r, colorT.g, colorT.b, (byte)80);
                            //CCOLOR = colorT;
                            //lines[n].SetColor(colorT, nf, nf + 1);
                            lines[n].SetColor(new Color32(0, 0, 0, 0), nf);
                            ps.Add(PosInfoList[i].Vec);
                            ps.Add(PosInfoList[i + 1].Vec);
                        }
                        //Debug.LogError("SetLineTransparent!");
                        dottedlines.Add(ps, seconds);
                    }
                    catch
                    {
                        int m = 0;
                    }
                }
                else
                {
                    int n = i / segmentsMax;
                    int nf = i % segmentsMax;
                    if (nf == segmentsMax - 1 && lines.Count - 1 > n)//考虑两条线的分界点的情况
                    {
                        List<Vector3> ls = new List<Vector3>();
                        ls.Add(PosInfoList[i].Vec);
                        ls.Add(PosInfoList[i + 1].Vec);
                        CreatePathLink(ls, 1);
                    }
                }
            }
        }

        //根据两点距离画虚线
        foreach (List<Vector3> vList in dottedlines.Keys)
        {
            Vector3 p1 = vList[0];
            Vector3 p2 = vList[1];

            float dis = Vector3.Distance(p1, p2);
            float unit = 1f;
            float nfloat = dis / unit * LocationHistoryManager.Instance.LineSetting.PointDensity;//无数据轨迹每隔0.2个单位画一个点
            int n = (int)Math.Round(nfloat, 0) + 1;
            if (n % 2 > 0)
            {
                n += 1;
            }
            if (n < 2)
            {
                n = 2;
            }
            if (n > segmentsMax)//要是超过最大点数可以考虑分组建（基本不可能超过），所以不用考虑
            {
                n = segmentsMax;
            }
            CreateHistoryPathDottedline(vList, n, isActive);
        }
    }

    /// <summary>
    /// 创建路径父物体
    /// </summary>
    public virtual Transform CreatePathParent()
    {
        //if (pathParent == null)
        //{
        //    Transform parent = LocationHistoryManager.Instance.GetHistoryAllPathParent();
        //    pathParent = new GameObject("" + code).transform;
        //    pathParent.transform.SetParent(parent);
        //}
        //return pathParent;
        return null;
    }

    /// <summary>
    /// 获取颜色
    /// </summary>
    public Color GetColor()
    {
        return color;
    }
}
