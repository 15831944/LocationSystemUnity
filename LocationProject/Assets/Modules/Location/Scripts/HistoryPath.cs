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
    /// <summary>
    /// 实际点位置
    /// </summary>
    protected List<Vector3> splinePoints;//所有点的集合
    public List<DateTime> timelist;//所有点的时间集合
    protected List<List<Vector3>> splinePointsList;
    protected List<List<DateTime>> timelistLsit;

    /// <summary>
    /// 路径是否闭合
    /// </summary>
    public bool pathLoop = false;

    protected List<VectorLine> lines;
    protected List<VectorLine> dottedlines;

    //protected double timeLength;//播放时间长度，单位秒
    //protected double progressValue;//轨迹播放进度值
    //protected double progressTargetValue;//轨迹播放目标值
    //protected double timeStart;//轨迹播放起始时间Time.time

    //protected Collider collider;//碰撞器
    //protected Renderer render;//render

    //protected bool IsShowRenderer = true;//是否显示了Renderer

    //protected GameObject followUI;//跟随UI

    public Transform pathParent;//该历史路径的父物体

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

    /// <summary>
    /// 创建历史轨迹
    /// </summary>
    /// <param name="splinePointsT">点集合</param>
    /// <param name="segmentsT">多少段，比点数量少1</param>
    protected void CreateHistoryPath(List<Vector3> splinePointsT, int segmentsT)
    {

        //VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
        VectorLine line = new VectorLine("Spline", new List<Vector3>(segmentsT + 1), 1.5f, LineType.Continuous);
        lines.Add(line);
        //line.lineColors
        line.color = color;        line.MakeSpline(splinePointsT.ToArray(), segmentsT, pathLoop);        SetLineTransparentAndDottedline();
        line.Draw3D();
        //line.Draw3DAuto();
        GameObject lineObjT = line.rectTransform.gameObject;
        lineObjT.transform.SetParent(pathParent);
        Renderer r = lineObjT.GetComponent<Renderer>();
        color = new Color(color.r, color.g, color.b, 0.7f);
        r.material.SetColor("_TintColor", color);//默认透明度是0.5,这里改为0.7；
        r.material.SetFloat("_InvFade", 0.15f);//原本是1，改为0.2，让线绘制的更加柔和，不会出现断裂
        r.material.renderQueue = 4000;//默认透明度是3000,这里改为4000；让透明物体先渲染，该轨迹后渲染，效果会更好
    }

    /// <summary>
    /// 创建轨迹间连接线
    /// </summary>
    /// <param name="splinePointsT">点集合</param>
    /// <param name="segmentsT">多少段，比点数量少1</param>
    protected void CreatePathLink(List<Vector3> splinePointsT, int segmentsT)
    {

        //VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
        VectorLine line = new VectorLine("Spline", new List<Vector3>(segmentsT + 1), 1.5f, LineType.Continuous);
        //lines.Add(line);
        line.name = "LineLink";
        line.color = color;        line.MakeSpline(splinePointsT.ToArray(), segmentsT, pathLoop);
        line.Draw3D();
        //line.Draw3DAuto();
        GameObject lineObjT = line.rectTransform.gameObject;
        lineObjT.transform.SetParent(pathParent);
        Renderer r = lineObjT.GetComponent<Renderer>();
        color = new Color(color.r, color.g, color.b, 0.7f);
        r.material.SetColor("_TintColor", color);//默认透明度是0.5,这里改为0.7；
        r.material.SetFloat("_InvFade", 0.15f);//原本是1，改为0.2，让线绘制的更加柔和，不会出现断裂
        r.material.renderQueue = 4000;//默认透明度是3000,这里改为4000；让透明物体先渲染，该轨迹后渲染，效果会更好
    }

    /// <summary>
    /// 创建历史轨迹中检测不到的虚线轨迹
    /// </summary>
    protected void CreateHistoryPathDottedline(List<Vector3> splinePointsT, int segmentsT)
    {

        //VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
        VectorLine line = new VectorLine("Spline", new List<Vector3>(segmentsT + 1), 3f, LineType.Points);
        dottedlines.Add(line);
        //line.lineColors
        line.color = color;        line.MakeSpline(splinePointsT.ToArray(), segmentsT, pathLoop);
        line.Draw3D();
        //line.Draw3DAuto();
        GameObject lineObjT = line.rectTransform.gameObject;
        lineObjT.name = "dottedline";
        lineObjT.transform.SetParent(pathParent);
        Renderer r = lineObjT.GetComponent<Renderer>();
        color = new Color(color.r, color.g, color.b, 0.4f);
        r.material.SetColor("_TintColor", color);//默认透明度是0.5,这里改为0.7；
        r.material.SetFloat("_InvFade", 0.15f);//原本是1，改为0.2，让线绘制的更加柔和，不会出现断裂
        r.material.renderQueue = 4000;//默认透明度是3000,这里改为4000；让透明物体先渲染，该轨迹后渲染，效果会更好
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
    protected void RefleshDrawLine()
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

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2) || isScrollWheelEnd)
        {
            //RefleshDrawLineOP();
            StartCoroutine(RefleshDrawLineOP());
        }
    }

    public IEnumerator RefleshDrawLineOP()
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
            yield return null;
        }
    }

    /// <summary>
    /// 数据点太多创建，数据拆开以便创建多条轨迹
    /// 因为创建一条连续线最多为16383个点
    /// </summary>
    public void GroupingLine(int segmentsMaxT = 16000)
    {
        if (splinePoints.Count != timelist.Count)
        {
            return;
        }

        int n = splinePoints.Count / segmentsMaxT;
        if (splinePoints.Count % segmentsMaxT > 0)
        {
            n += 1;
        }

        for (int i = 0; i < n; i++)
        {
            if (i < n - 1)
            {
                List<Vector3> listT = splinePoints.GetRange(i * segmentsMaxT, segmentsMaxT);
                splinePointsList.Add(listT);
            }
            else
            {
                List<Vector3> listT = splinePoints.GetRange(i * segmentsMaxT, splinePoints.Count - i * segmentsMaxT);
                splinePointsList.Add(listT);
            }
        }

        for (int i = 0; i < n; i++)
        {
            if (i < n - 1)
            {
                List<DateTime> listT = timelist.GetRange(i * segmentsMaxT, segmentsMaxT);
                timelistLsit.Add(listT);
            }
            else
            {
                List<DateTime> listT = timelist.GetRange(i * segmentsMaxT, splinePoints.Count - i * segmentsMaxT);
                timelistLsit.Add(listT);
            }
        }

        //if (splinePoints.Count> 16000)
        //{
        //    splinePointsList.AddRange(splinePoints.GetRange())
        //}

    }

    /// <summary>
    /// 设置部分线透明并创建虚线，表示无历史数据，一般两点时间超过10秒，认为中间为无历史数据
    /// </summary>
    public void SetLineTransparentAndDottedline()
    {

        Dictionary<List<Vector3>, double> dottedlines = new Dictionary<List<Vector3>, double>();
        for (int i = 0; i < timelist.Count; i++)
        {
            if (i < timelist.Count - 1)
            {
                double secords = (timelist[i + 1] - timelist[i]).TotalSeconds;
                if (secords > LocationHistoryManager.Instance.IntervalTime)//一般两点时间超过10秒，认为中间为无历史数据
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
                            ps.Add(splinePoints[i]);
                            ps.Add(splinePoints[i + 1]);
                        }
                        //Debug.LogError("SetLineTransparent!");
                        dottedlines.Add(ps, secords);
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
                        ls.Add(splinePoints[i]);
                        ls.Add(splinePoints[i + 1]);
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
            float nfloat = dis / unit;//无数据轨迹每隔0.2个单位画一个点
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
            CreateHistoryPathDottedline(vList, n);
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

}
