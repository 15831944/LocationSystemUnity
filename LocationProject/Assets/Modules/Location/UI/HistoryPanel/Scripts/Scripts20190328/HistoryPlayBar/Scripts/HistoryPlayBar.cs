using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class HistoryPlayBar : Graphic
{

    //图形方面
    public float height = 1f;//Bar的粗细
    //public float height = 100f;//高度
    public float width = 200f;//宽度
    [Tooltip("折线的底下是否进行填充")]

    //数据方面
    private int Vcount;
    private double[] valueList; //Bar的值列表，这里保存时间戳，Bar的长度为时间长度
    private List<Position> positions;//历史数据信息

    private double timeLength;//时间长度

    public float timeInterval = 10;//时间间隔，单位秒


    public HistoryPlayBar()
    {
        valueList = new double[] { 1f, 10f, 15f, 30f, 35f };
    }

    public void UpdateData(List<double> values)
    {
        valueList = values.ToArray();
        //很重要，界面才会重新刷新
        SetVerticesDirty();
    }

    public void UpdateData(List<double> values, List<Position> positionsT,double timeLengthT, Color colorT,float timeIntervalT= 10)
    {
        values.Insert(0, 0);//插入首点，便于画线
        values.Add(timeLengthT);//插入结尾点，便于画线
        valueList = values.ToArray();
        positions = positionsT;
        timeLength = timeLengthT;
        color = colorT;
        timeInterval = timeIntervalT;
        //很重要，界面才会重新刷新
        SetVerticesDirty();
    }


    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        //print("OnPopulateMesh");
        vh.Clear();
        Vcount = valueList.Length;
        if (Vcount <= 0) return;
        Vector2 pStart = GetVector2(0);

        //yScale = height / (yMax - yMin);
        for (int i = 0; i < valueList.Length; i++)
        {
            Vector2 p = GetVector2(i);
            Color colorT = color;
            if (i == 0) continue;
            //if (valueList[i] - valueList[i - 1] > timeInterval) continue;
            if (valueList[i] - valueList[i - 1] > timeInterval)
            {
                Vector2 _p = GetVector2(i - 1);
                AddUIVertexQuad(vh, pStart, _p, colorT);

                colorT = new Color((float)10 / 256, (float)34 / 256, (float)42 / 256);
                AddUIVertexQuad(vh, _p, p, colorT);

                pStart = p;
                continue;
            }

            if (i == valueList.Length - 1)
            {
                AddUIVertexQuad(vh, pStart, p, colorT);
            }

        }

    }




    /// <summary>
    /// 创建Line
    /// </summary>
    /// <param name="vh"></param>
    /// <param name="index"></param>
    public void AddUIVertexQuad(VertexHelper vh, Vector2 p1, Vector2 p2,Color colorT)
    {

        Vector2 v0;
        Vector2 v1;
        Vector2 v2;
        Vector2 v3;

        v0 = new Vector2(p1.x, p1.y - height / 2);
        v1 = new Vector2(p1.x, p1.y + height / 2);
        v2 = new Vector2(p2.x, p2.y + height / 2);
        v3 = new Vector2(p2.x, p2.y - height / 2);


        UIVertex[] verts = new UIVertex[4];
        verts[0].position = v0;
        verts[0].color = colorT;
        verts[0].uv0 = Vector2.zero;

        verts[1].position = v1;
        verts[1].color = colorT;
        verts[1].uv0 = Vector2.zero;

        verts[2].position = v2;
        verts[2].color = colorT;
        verts[2].uv0 = Vector2.zero;

        verts[3].position = v3;
        verts[3].color = colorT;
        verts[3].uv0 = Vector2.zero;

        vh.AddUIVertexQuad(verts);
    }


    public Vector2 GetVector2(int index)
    {
        return new Vector2((float)(valueList[index]* width) / (float)timeLength, 0);
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        UpdateData(new List<double>());
    }
}
