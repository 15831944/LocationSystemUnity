using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

/// <summary>
/// 轨迹根据历史人员实时画
/// </summary>
public class HistoryPathDrawing : MonoBehaviour
{
    public Texture lineTex;
    public Color lineColor = Color.green;
    public int maxPoints = 500;
    public bool continuousUpdate = true;

    private VectorLine pathLine;
    private int pathIndex = 0;

    Coroutine coroutine;
    private int allPointsNum = 0;//所有的点数

    public Transform lineContentParent;//连线集合容器父物体
    public Transform lineContent;//连线集合容器
    public List<VectorLine> lines;//连线集合
    public bool running;//是否正在执行画线
    public int limitLinesNum = 10;//限制轨迹线
    Vector3 lastPoint;
    Color color = Color.green;
    public float hOffset=0.1f;//高度偏移

    void Start()
    {
        lines = new List<VectorLine>();
        lineContent = new GameObject("DrawingLines").transform;
        if (lineContentParent != null)
        {
            lineContent.SetParent(lineContentParent);
        }

        AddLine();
        //Drawing();
        //coroutine = StartCoroutine(SamplePoints(transform));
        lastPoint = Vector3.zero;
        //SetRunning(true);
        time = Time.time;
    }

    float time;

    void Update()
    {

        if (running && WaitForSeconds(.05f)&&MultHistoryPlayUINew.Instance.isPlay)
        {
            float dist = Vector3.Distance(lastPoint, transform.position);
            if (dist > 50)
            {
                int i = 0;
            }
            if (lastPoint == transform.position) return;
            lastPoint = transform.position;
            allPointsNum++;
            pathIndex++;
            pathLine.points3.Add(transform.position + new Vector3(0, hOffset, 0));
            if (pathIndex == maxPoints)
            {
                AddLine();
                //running = false;
            }

            //Debug.LogError("Drawing。。。");

            //yield return new WaitForSeconds(.05f);

            if (continuousUpdate)
            {
                //pathLine.Draw3D();
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine= StartCoroutine(LinesDraw());
            }
        }
    }

    public IEnumerator LinesDraw()
    {
        for (int i= lines.Count-1; i>=0;i--)
        {
            lines[i].Draw3D();
            //yield return null;
        }
        yield return null;
    }

    bool WaitForSeconds(float s)
    {
        if (Time.time - time > s)
        {
            time = Time.time;
            return true;
        }
        return false;
    }

    public void Init(Transform lineContentParentT,Color colorT)
    {
        lineContentParent = lineContentParentT;
        color = colorT;
    }

    /// <summary>
    /// 绘制
    /// </summary>
    public void Drawing()
    {
        SetRunning(true);
        //running = true;
        //if (coroutine != null)
        //{
        //    StopCoroutine(coroutine);
        //}
        //coroutine = StartCoroutine(SamplePoints(transform));
    }

    /// <summary>
    /// 暂停绘制
    /// </summary>
    public void PauseDraw()
    {
       // running = false;
        SetRunning(false);
    }

    IEnumerator SamplePoints(Transform thisTransform)
    {
        // Gets the position of the 3D object at intervals (20 times/second)
        //running = true;
        Vector3 lastPoint = Vector3.zero;
        while (running)
        {
            if (lastPoint == thisTransform.position) continue;
            lastPoint = thisTransform.position;
            allPointsNum++;
            pathIndex++;
            pathLine.points3.Add(thisTransform.position);
            if (pathIndex == maxPoints)
            {
                AddLine();
                //running = false;
            }

            //Debug.LogError("Drawing。。。");

            yield return new WaitForSeconds(.05f);

            if (continuousUpdate)
            {
                pathLine.Draw3D();
            }
        }
    }

    private void SetRunning(bool b)
    {
        running = b;
        Debug.LogError("SetRunning:" + running);
    }

    /// <summary>
    /// 添加线
    /// </summary>
    public void AddLine()
    {
        pathLine = new VectorLine("Path", new List<Vector3>(), lineTex, 3.0f, LineType.Continuous);
        pathLine.Draw3D();//不绘制一下无法设置父物体
        pathLine.rectTransform.gameObject.transform.SetParent(lineContent);
        color = new Color(color.r, color.g, color.b, 0.7f);
        pathLine.color = color;
        pathLine.textureScale = 1.0f;
        pathIndex = 0;
        Renderer r = pathLine.rectTransform.gameObject.GetComponent<Renderer>();
        //r.material.SetFloat("_InvFade", 0.15f);//原本是1，改为0.2，让线绘制的更加柔和，不会出现断裂
        r.material.renderQueue = 4000;//默认透明度是3000,这里改为4000；让透明物体先渲染，该轨迹后渲染，效果会更好


        lock (lines)
        {
            lines.Add(pathLine);
            if (lines.Count > limitLinesNum)
            {
                int removeNum = lines.Count - limitLinesNum;
                //lines.RemoveRange(0, removeNum);
                for (int i = 0; i < removeNum; i++)
                {
                    Destroy(lines[0].rectTransform.gameObject);
                    lines.RemoveAt(0);
                }
            }

        }
    }

    /// <summary>
    /// 设置lineContent
    /// </summary>
    /// <param name="isbool"></param>
    public void SetLineContent(bool isbool)
    {
        lineContent.gameObject.SetActive(isbool);
    }

    //public void AddLine2()
    //{
    //    pathLine = new VectorLine("Spline", new List<Vector3>(segmentsT + 1), 1.5f, LineType.Continuous);
    //    lines.Add(pathLine);
    //    pathLine.rectTransform.transform.SetParent(lineContent);
    //    pathLine.color = Color.green;
    //    pathLine.textureScale = 1.0f;
    //    pathIndex = 0;
    //    if (lines.Count > limitLinesNum)
    //    {
    //        int removeNum = lines.Count - limitLinesNum;
    //        lines.RemoveRange(0, removeNum);
    //    }
    //}

    //protected void CreateHistoryPath(List<Vector3> splinePointsT, int segmentsT)
    //{

    //    //VectorLine: exceeded maximum vertex count of 65534 for ""...use fewer points (maximum is 16383 points for continuous lines and points, and 32767 points for discrete lines)
    //    VectorLine line = new VectorLine("Spline", new List<Vector3>(segmentsT + 1), 1.5f, LineType.Continuous);
    //    lines.Add(line);
    //    line.color = color;    //    line.MakeSpline(splinePointsT.ToArray(), segmentsT, pathLoop);
    //    line.Draw3D();
    //    //line.Draw3DAuto();
    //    GameObject lineObjT = line.rectTransform.gameObject;
    //    lineObjT.transform.SetParent(pathParent);
    //    Renderer r = lineObjT.GetComponent<Renderer>();
    //    color = new Color(color.r, color.g, color.b, 0.7f);
    //    r.material.SetColor("_TintColor", color);//默认透明度是0.5,这里改为0.7；
    //    r.material.SetFloat("_InvFade", 0.15f);//原本是1，改为0.2，让线绘制的更加柔和，不会出现断裂
    //    r.material.renderQueue = 4000;//默认透明度是3000,这里改为4000；让透明物体先渲染，该轨迹后渲染，效果会更好
    //}


    private void OnDisable()
    {
        //running = false;
        SetRunning(false);
        //StopCoroutine(coroutine);
        lines.Clear();
    }
}
