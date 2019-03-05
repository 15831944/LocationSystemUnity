using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasRenderer))]
public class UGUIRadar : Graphic
{

    public List<float> valueList;//值列表

    //网格模型顶点数量
    private int VERTICES_COUNT;
    public int radius = 100;
    //public Color ccolor = Color.white;
    //private bool IsCanDraw;//可以进行绘制了

    [Tooltip("顶点数必须小于>3")]
    //顶点数组
    Vector3[] vertices;
    //三角形数组索引
    int[] triangles;
    //偏移系数
    [Range(-0.5f, 0.5f)]
    public float offset = 0;


    float pi = Mathf.PI;

    public UGUIRadar()
    {
        valueList = new List<float>();
        valueList.Add(1f);
        valueList.Add(1f);
        valueList.Add(1f);
    }

    //public void Awake()
    //{
    //    SetVertices();
    //}

    //void Start()
    //{
    //    //SetVertices();
    //    print("");
    //}
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (valueList.Count < 3) return;
        vh.Clear();
        SetVertices(vh);
    }

    private void SetVertices(VertexHelper vh)
    {
        vertices = new Vector3[valueList.Count + 1];
        VERTICES_COUNT = vertices.Length;//包括中心点
        int triangles_count = VERTICES_COUNT - 1;

        triangles = new int[triangles_count * 3];

        List<UIVertex> verts = new List<UIVertex>();

        //设定原点坐标
        vertices[0] = new Vector3(0, 0, 0);
        //首个在x轴上的坐标点
        vertices[1] = new Vector3(radius, 0, 0);
        UIVertex uiVertex0 = CreateUIVertex(vertices[0]);
        UIVertex uiVertex1 = CreateUIVertex(vertices[1]* GetValue(0));
        verts.Add(uiVertex0);
        verts.Add(uiVertex1);

        //每个三角形角度
        float everyAngle = 360 / triangles_count;

        for (int i = 2; i < vertices.Length; i++)
        {
            var angle = GetRadians(everyAngle * (i - 1));
            vertices[i] = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
            Vector3 v = vertices[i] * GetValue(i - 1);

            UIVertex uiVertexT = CreateUIVertex(v);
            verts.Add(uiVertexT);
        }

        SetTriangles();
        vh.AddUIVertexStream(verts, new List<int>(triangles));
    }

    private void SetTriangles()
    {
        int idx = 0;
        int value = 0;
        for (int i = 0; i < triangles.Length; i++)
        {
            if (i % 3 == 0)
            {
                triangles[i] = 0;
                value = idx;
                idx++;
            }
            else
            {
                value++;
                if (value == VERTICES_COUNT)
                    value = 1;
                //Debug.Log("value " + value);

                triangles[i] = value;
            }
        }
    }

    public UIVertex CreateUIVertex(Vector3 pos)
    {
        UIVertex uiVertex = new UIVertex();
        uiVertex.position = pos;
        uiVertex.color = color;
        uiVertex.uv0 = Vector2.zero;
        return uiVertex;
    }

    float GetValue(int i)
    {
        valueList[i] = valueList[i] > 1 ? 1 : valueList[i];
        valueList[i] = valueList[i] < 0 ? 0 : valueList[i];

        //valueList[i] = valueList[i] + offset;

        return valueList[i] + offset;
    }

    float GetRadians(float angle)
    {
        return pi / 180 * angle;
    }

    //void Apply()
    //{
    //    Vector3[] tmps = new Vector3[vertices.Length];
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        //tmps[i] = vertices[i] * vertices[i].z * scale;
    //        tmps[i] = vertices[i] * vertices[i].z;
    //    }

    //}

    public void SetValue(List<float> values)
    {
        //valueList = values;
        valueList.Clear();
        for (int i = 0; i < values.Count; i++)
        {
            valueList.Add(values[i]);
        }
        //很重要，界面才会重新刷新
        SetVerticesDirty();
    }
}
