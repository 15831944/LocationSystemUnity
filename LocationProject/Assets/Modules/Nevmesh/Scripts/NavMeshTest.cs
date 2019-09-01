using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour
{
    public GameObject target;
    public GameObject agent;

    public GameObject meshShowObj;

    public NavMeshTriangulation triangles;
    public Mesh mesh;

    public int indicesCount = 0;
    public Vector3[] vertices;
    public int[] indices;

    public Vector3 targetPos;

    void Update()
    {
        if (target.transform.position == targetPos)
        {
            return;
        }

        targetPos = target.transform.position;

        if (mesh == null)
        {
            ShowNavMesh();
        }
        else
        {
            ShowNavMeshPoints();
        }
    }

    [ContextMenu("ShowNavMesh")]
    void ShowNavMesh()
    {
        
        triangles = NavMesh.CalculateTriangulation();
        mesh = new Mesh();

        vertices= triangles.vertices;
        indices= triangles.indices;;
        mesh.vertices = vertices;
        mesh.triangles = indices;
        indicesCount = indices.Length;

        if (meshShowObj)
        {
            Mesh mesh2 = meshShowObj.GetComponent<MeshFilter>().mesh;
            mesh2.Clear();
            mesh2.vertices = vertices;
            mesh2.triangles = indices;
            //mesh.Optimize();
            mesh2.RecalculateNormals();
            //if (meshShowObj.GetComponent<MeshCollider>() == null)
            //{
            //    meshShowObj.AddComponent<MeshCollider>();
            //}

            //Collider collider = meshShowObj.GetComponent<Collider>();
            //var p=collider.ClosestPoint(targetPos);
            //NavMeshHelper.CreatePoint(p, "MeshCollider", 0.5f, Color.yellow);
        }

        //ShowVertices();
        ShowNavMeshPoints();

    }

    private List<GameObject> navPoints = new List<GameObject>();

    public int navMask = -1;

    public int sampleDistance = 100;

    public int PointScale = 10;

    public void ShowNavMeshPoints()
    {
        DateTime start = DateTime.Now;

        //NavMeshHit hitInfo;

        //foreach (GameObject point in navPoints)
        //{
        //    GameObject.DestroyImmediate(point);
        //}

        //if (NavMesh.FindClosestEdge(targetPos, out hitInfo, navMask))
        //{
        //    GameObject o = NavMeshHelper.CreatePoint(hitInfo.position, "Edge", 0.5f, Color.green);
        //    navPoints.Add(o);
        //}
        //else
        //{
        //    //Debug.LogError("FindClosestEdge No");
        //}

        //if (NavMesh.SamplePosition(targetPos, out hitInfo, sampleDistance, navMask))
        //{
        //    GameObject o = NavMeshHelper.CreatePoint(hitInfo.position, "Sample", 0.5f, Color.blue);
        //    navPoints.Add(o);
        //}
        //else
        //{
        //    //Debug.LogError("SamplePosition No");
        //}

        //if (NavMesh.Raycast(targetPos, agent.transform.position, out hitInfo, -1))
        //{
        //    GameObject o = NavMeshHelper.CreatePoint(hitInfo.position, "Raycast", 0.5f, Color.red);
        //    navPoints.Add(o);
        //}
        //else
        //{
        //    //Debug.LogError("No Raycast!!!!");
        //}


        NavMeshHelper.ScalePower = PointScale;

        //var pr = NavMeshHelper.GetClosetPoint(targetPos);
        ////navPoints.Add(NavMeshHelper.CreatePoint(pr, "ClosedPoint", 1f, Color.cyan));

        //TimeSpan time = DateTime.Now - start;
        //Debug.Log("ShowNavMeshPoints:" + time.TotalMilliseconds + "ms");

        NavMeshHelper.GetClosetPointAsync(targetPos,this.name,null, (pr,p) =>
        {
            TimeSpan time = DateTime.Now - start;
            Debug.Log("ShowNavMeshPoints:" + time.TotalMilliseconds + "ms");
        });
    }

    public int sampleCount = 10;





    //void OnDrawGizmos()
    //{
    //    //ShowVertices();
    //}

    public List<GameObject> points;

    [ContextMenu("ShowVertices")]
    public void ShowVertices()
    {
        if (points != null)
        {
            foreach (GameObject point in points)
            {
                GameObject.DestroyImmediate(point);
            }
        }
        if(vertices!=null)
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vector3 = vertices[i];
                //Gizmos.DrawSphere(vector3, 1);
                //GameObject o = NavMeshHelper.CreatePoint(vector3,"Vertice"+i,0.2f,Color.gray);
                //o.transform.parent = this.transform;
                //points.Add(o);
            }

       
    }


}
