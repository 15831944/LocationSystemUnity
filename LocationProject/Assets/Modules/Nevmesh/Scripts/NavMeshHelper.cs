using Dest.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshHelper
{


    public class NavMeshInfo
    {

        public List<Triangle3> triangle3s = new List<Triangle3>();
        public List<Plane3> planes = new List<Plane3>();//平面 （三角形）
        public List<Segment3> segments = new List<Segment3>();
        public int indicesCount = 0;

        private Vector3[] vertices;

        public NavMeshInfo()
        {
            var triangles = NavMesh.CalculateTriangulation();
            vertices = triangles.vertices;//顶点
            indicesCount = vertices.Length;
            var indices = triangles.indices;//索引
            if (indices.Length % 3 != 0)
            {
                Debug.LogError("顶点数量不对 不是3的倍数 :" + indices.Length);
            }
            else
            {
                for (int i = 0; i < indices.Length; i += 3)
                {
                    var i1 = indices[i];
                    var i2 = indices[i + 1];
                    var i3 = indices[i + 2];
                    var p1 = vertices[i1];
                    var p2 = vertices[i2];
                    var p3 = vertices[i3];

                    segments.Add(new Segment3(p1, p2));
                    segments.Add(new Segment3(p2, p3));
                    segments.Add(new Segment3(p3, p1));

                    planes.Add(new Plane3(p1, p2, p3));
                    triangle3s.Add(new Triangle3(p1, p2, p3));
                }
            }
        }
    }

    public static Dictionary<string,GameObject> navPoints = new Dictionary<string, GameObject>();

    public static void RefreshNavMeshInfo()
    {
        navMeshInfo = new NavMeshInfo();
    }

    public static string currentName = "";

    public static void GetClosetPointAsync(Vector3 target,string name, Action<Vector3,GameObject> callback)
    {
        currentName = name;
           Log = "";
        var psList = GetSampleAndEdge(target);
       

        CreatePoint(target, "target", 0.1f, Color.green);//目标点

        if (navMeshInfo == null)
            navMeshInfo = new NavMeshInfo();
        var p1 = Vector3.zero;
        var p2 = Vector3.zero;
        ThreadManager.Run(() =>
        {
            try
            {
                p1 = GetClosetPointInTriangle(target);
                p2 = GetClosetPointBySegment(target);
            }
            catch (Exception e)
            {
                Debug.LogError("GetClosetPointAsync:"+e.ToString());
            }
        }, () =>
        {
            try
            {
                p1 = GetSamplePosition(p1);
                psList.Add(p1);
                CreatePoint(p1, "ByTriangle", 0.1f, Color.red);//用三角形平面计算出来的最近的点
                p2 = GetSamplePosition(p2);
                psList.Add(p2);
                CreatePoint(p2, "BySegment", 0.1f, Color.yellow);//用线段计算出来的最近的点

                var p3 = GetDownPosition(target);
                CreatePoint(p3, "Down", 0.1f, Color.magenta);
                var p4 = GetUpPosition(target);
                CreatePoint(p4, "Up", 0.1f, Color.blue);

                var r = GetClosedPoint(psList.ToArray(), target);
                var p=CreatePoint(r, "Closed", 0.1f, Color.cyan);//前面这些点中的最近的点，及结果。

                if (callback != null)
                {
                    callback(r,p);
                }
            }
            catch (Exception e)
            {
                if (callback != null)
                {
                    callback(target,null);
                }
            }
        }, "GetClosetPointByTriangleEx+GetClosetPointBySegmentEx");
    }

    private static void ClearTestPoints()
    {
        if (IsDebug)
        {
            foreach (var point in navPoints)
            {
                GameObject.DestroyImmediate(point.Value);
            }

            navPoints.Clear();
        }
        
    }

    private static List<Vector3> GetSampleAndEdge(Vector3 target)
    {
        ClearTestPoints();

        List<Vector3> psList = new List<Vector3>();
        NavMeshHit hitInfo;

        if (NavMesh.FindClosestEdge(target, out hitInfo, -1))
        {
            psList.Add(hitInfo.position);
            CreatePoint(hitInfo.position, "Edge", 0.1f, Color.green);//Edge：边缘上的点
        }
        else
        {
            //Debug.LogError("GetClosetPoint No FindClosestEdge ");
        }

        if (NavMesh.SamplePosition(target, out hitInfo, 100, -1))
        {
            psList.Add(hitInfo.position);
            CreatePoint(hitInfo.position, "Sample", 0.1f, Color.blue);//Sample：采样点
        }
        else
        {
            //Debug.LogError("GetClosetPoint No SamplePosition ");
        }

        return psList;
    }

    public static Vector3 GetClosetPoint(Vector3 target)
    {
        var psList = GetSampleAndEdge(target);

        var p1 = GetClosetPointByTriangleEx(target);
        psList.Add(p1);
        CreatePoint(p1, "ByTriangle", 0.1f, Color.red);

        var p2 = GetClosetPointBySegmentEx(target);
        psList.Add(p2);
        CreatePoint(p2, "BySegment", 0.1f, Color.yellow);

        var r = GetClosedPoint(psList.ToArray(), target);
        CreatePoint(r, "Closed", 0.1f, Color.cyan);
        return r;
    }



    public static Vector3 GetClosetPointBySegmentEx(Vector3 target)
    {
        var r = GetClosetPointBySegment(target);
        r = GetSamplePosition(r);
        return r;

    }
    public static Vector3 GetClosetPointByTriangleEx(Vector3 target)
    {
        var r = GetClosetPointInTriangle(target);
        r = GetSamplePosition(r);
        return r;

    }

    public static Vector3 GetSamplePosition(Vector3 r)
    {
        NavMeshHit hitInfo;
        if (NavMesh.SamplePosition(r, out hitInfo, 100, -1))
        {
            r = hitInfo.position;
        }
        else
        {
            Debug.LogError("GetClosetPointByVertices No SamplePosition ");
        }

        return r;
    }

    public static Vector3 GetDownPosition(Vector3 r)
    {
        NavMeshHit hitInfo;
        Vector3 r1 = new Vector3(r.x, r.y - 100, r.z);
        if (NavMesh.Raycast(r, r1,out hitInfo, -1))
        {
            r = hitInfo.position;
        }
        else
        {
            //Debug.LogError("GetDownPosition No DownPosition ");
        }
        return r;
    }

    public static Vector3 GetUpPosition(Vector3 r)
    {
        NavMeshHit hitInfo;
        Vector3 r1 = new Vector3(r.x, r.y + 100, r.z);
        if (NavMesh.Raycast(r, r1, out hitInfo, -1))
        {
            r = hitInfo.position;
        }
        else
        {
            //Debug.LogError("GetUpPosition No UpPosition ");
        }
        return r;
    }

    private static Vector3 GetClosetPointBySegment(Vector3 target)
    {
        var distance = float.MaxValue;
        DateTime start = DateTime.Now;

        Vector3 r = target;
        if (navMeshInfo == null)
            navMeshInfo = new NavMeshInfo();

        for (int i = 0; i < navMeshInfo.segments.Count; i++)
        {
            Segment3 segment = navMeshInfo.segments[i];
            Vector3 point = target;
            Vector3 closestPoint;
            float dist0 = Distance.Point3Segment3(ref point, ref segment, out closestPoint);
            if (dist0 < distance)
            {
                distance = dist0;
                r = closestPoint;
            }
        }

        TimeSpan time = DateTime.Now - start;
        Log+=(string.Format(
            "GetClosetPointBySegment,indices count:{0},distance:{1},closestPoint{2},time:{3}ms ",
            navMeshInfo.indicesCount,
            distance, r, time.TotalMilliseconds));
        return r;
    }

    //private static Vector3[] vertices;
    private static NavMeshInfo navMeshInfo = null;

    public static string Log = "";
    private static Vector3 GetClosetPointInTriangle(Vector3 target)
    {
        DateTime start = DateTime.Now;
        Vector3 r = target;
        //if (vertices == null)
        {
            var distance = float.MaxValue;

            //NavMeshInfo navMeshInfo = null;
            if (navMeshInfo == null)
                navMeshInfo = new NavMeshInfo();

            for (int i = 0; i < navMeshInfo.triangle3s.Count; i++)
            {
                Triangle3 triangle3 = navMeshInfo.triangle3s[i];
                Plane3 plane = navMeshInfo.planes[i];

                Vector3 point = target;
                Vector3 closestPoint;
                float dist0 = Distance.Point3Plane3(ref point, ref plane, out closestPoint);

                Line3 line = new Line3(point, target);
                Triangle3 triangle = triangle3;
                Line3Triangle3Intr info;
                bool find = Intersection.FindLine3Triangle3(ref line, ref triangle, out info);

                if (find) //只有点在三角形内才有效
                {
                    //Debug.Log(string.Format("[{0}]{1} {2} {3}", i, dist0, closestPoint, find));

                    if (dist0 < distance)
                    {
                        distance = dist0;
                        r = closestPoint;
                    }
                }
            }

            TimeSpan time = DateTime.Now - start;
            Log+=(string.Format(
                "GetClosetPointByTriangle,indices count:{0},distance:{1},closestPoint{2},target:{3},time:{4}ms ",
                navMeshInfo.indicesCount, distance, r, target, time.TotalMilliseconds));
        }
        return r;
    }

    //public static Vector3 GetClosetPointByVertices(Vector3 target, int sampleCount)
    //{
    //    if (vertices == null)
    //    {
    //        var triangles = NavMesh.CalculateTriangulation();
    //        vertices = triangles.vertices;//顶点
    //    }
    //    var i1 = GetClosedPointIndex(vertices, target);

    //    List<Vector3> psList = new List<Vector3>();
    //    if (i1 < vertices.Length - 1)
    //    {
    //        var ps1 = GetPointsOfLine(vertices[i1], vertices[i1 + 1], sampleCount);
    //        psList.AddRange(ps1);
    //    }

    //    if (i1 > 0)
    //    {
    //        var ps2 = GetPointsOfLine(vertices[i1], vertices[i1 - 1], sampleCount);
    //        psList.AddRange(ps2);
    //    }

    //    Vector3 r = GetClosedPoint(psList.ToArray(), target);
    //    r = GetSamplePosition(r);
    //    return r;
    //}

    //public static List<Vector3> GetPointsOfLine(Vector3 start, Vector3 end, int count)
    //{
    //    List<Vector3> list = new List<Vector3>();
    //    var distance = Vector3.Distance(end, start);
    //    var interval = distance / count;
    //    Debug.Log("count:" + count);
    //    Debug.Log("distance:" + distance);
    //    Debug.Log("interval:" + interval);
    //    var direction = (end - start).normalized;
    //    for (int i = 0; i < count; i++)
    //    {
    //        var p = start + direction * interval * i;
    //        list.Add(p);
    //    }
    //    return list;
    //}

    public static int ScalePower = 1;

    public static bool IsDebug = false;

    public static GameObject CreatePoint(Vector3 v, string name, float scale, Color color)
    {
        if (IsDebug)
        {
            name = currentName + name;
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(scale * ScalePower, scale * ScalePower, scale * ScalePower);
            sphere.transform.position = v;
            sphere.name = name;
            sphere.GetComponent<Renderer>().material.color = color;
            sphere.SetTransparent(0.5f);
            //Name存在，会导致异常（不停的创建ByTriangle）
            if(navPoints!=null&&navPoints.ContainsKey(name))
            {
                GameObject.DestroyImmediate(navPoints[name]);
                navPoints.Remove(name);
            }
            navPoints.Add(name,sphere);
            return sphere;
        }
        else
        {
            return null;
        }
    }

    public static Vector3 GetClosedPoint(Vector3[] ps, Vector3 p)
    {
        Vector3 r = Vector3.zero;
        float dis = float.MaxValue;
        foreach (Vector3 i in ps)
        {
            var d = Vector3.Distance(i, p);
            if (d < dis)
            {
                dis = d;
                r = i;
            }
        }
        return r;
    }

    public static int GetClosedPointIndex(Vector3[] ps, Vector3 p)
    {
        int r = 0;
        float dis = float.MaxValue;
        for (int i1 = 0; i1 < ps.Length; i1++)
        {
            Vector3 i = ps[i1];
            var d = Vector3.Distance(i, p);
            if (d < dis)
            {
                dis = d;
                r = i1;
            }
        }
        return r;
    }
}
