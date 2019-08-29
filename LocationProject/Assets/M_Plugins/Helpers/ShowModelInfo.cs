using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowModelInfo : MonoBehaviour {

    public int Vertexs;//顶点数

    // Use this for initialization
    void Start ()
    {
        ShowVertexs();
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// 显示顶点数
    /// </summary>
    [ContextMenu("ShowVertexs")]
    public void ShowVertexs()
    {
        Vertexs = 0;
        MeshFilter[] mrs = gameObject.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < mrs.Length; i++)
        {
            if (mrs[i] == null) continue;
            if (mrs[i].sharedMesh == null) continue;
            Vertexs += mrs[i].sharedMesh.vertexCount;
        }
        Debug.Log("Vertexs:" + Vertexs);
    }

    /// <summary>
    /// 显示激活了的模型顶点数
    /// </summary>
    [ContextMenu("ShowActiveVertexs")]
    public void ShowActiveVertexs()
    {
        Vertexs = 0;
        MeshFilter[] mrs = gameObject.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < mrs.Length; i++)
        {
            Vertexs += mrs[i].sharedMesh.vertexCount;
        }
        Debug.Log("Vertexs:" + Vertexs);
    }
}
