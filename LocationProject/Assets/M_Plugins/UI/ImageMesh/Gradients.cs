using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
[AddComponentMenu("UI/Effects/Gradients")]
public class Gradients : BaseMeshEffect {

    [SerializeField]
    private Color32
     leftColor = Color.white;
    [SerializeField]
    private Color32
        rightColor = Color.black;
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }
        var count = vh.currentVertCount;
        if (count == 0)return;
        var vertexs = new List<UIVertex>();
        for (var i = 0; i < count; i++)
        {
            var vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);
            vertexs.Add(vertex);
        }
        var topX = vertexs[0].position.x;
        var bottomX = vertexs[0].position.x;
        for (var i = 1; i < count; i++)
        {
            var x = vertexs[i].position.x;
            if (x > topX)
            {
                topX = x;
            }
            else if (x < bottomX)
            {
                bottomX = x;
            }
        }
        var width = topX - bottomX;
        for (var i = 0; i < count; i++)
        {
            var vertex = vertexs[i];
            var color = Color32.Lerp(leftColor, rightColor, (vertex.position.x - bottomX) / width);
            vertex.color = color;
            vh.SetUIVertex(vertex, i);
        }

    }
    //private void test(VertexHelper vh)
    //{
    //    if (!IsActive())
    //    {
    //        return;
    //    }
    //    var count = vh.currentVertCount;
    //    if (count == 0) return;
    //    var vertexs = new List<UIVertex>();
    //    for (var i = 0; i < count; i++)
    //    {
    //        var vertex = new UIVertex();
    //        vh.PopulateUIVertex(ref vertex, i);
    //        vertexs.Add(vertex);
    //    }
    //    var topY = vertexs[0].position.y;
    //    var bottomY = vertexs[0].position.y;
    //    for (var i = 1; i < count; i++)
    //    {
    //        var y = vertexs[i].position.y;
    //        if (y > topY)
    //        {
    //            topY = y;
    //        }
    //        else if (y < bottomY)
    //        {
    //            bottomY = y;
    //        }
    //    }
    //    var height = topY - bottomY;
    //    for (var i = 0; i < count; i++)
    //    {
    //        var vertex = vertexs[i];
    //        var color = Color32.Lerp(bottomColor, topColor, (vertex.position.y - bottomY) / height);
    //        vertex.color = color;
    //        vh.SetUIVertex(vertex, i);
    //    }
    //}
}
