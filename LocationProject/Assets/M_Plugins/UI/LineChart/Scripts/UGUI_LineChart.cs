using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UGUI_LineChart : MaskableGraphic
{
    //图形方面
    public Canvas canvas;
    public float thickness = 1f;//折线的粗细
    public float height=100f;//高度
    public float width=200f;//宽度
    [Tooltip("折线的底下是否进行填充")]
    public bool IsFill=false;//折线的底下是否进行填充
    public Color fillColor;
    public RectTransform rect;
    public RectTransform pointImagePrafeb;//点图标预设
    public Vector2 pointImageSize=new Vector2(1,1);//点图标大小
    public List<RectTransform> pointImageList;
    public GameObject pointImageParent;//点图标父物体
    private bool IsCanCreatePointImage=false;
    public bool IsTooltipPanel= false;

    //数据方面
    public float yMax = 100;//Y最大值
    public float yMin = 0;//Y最小值
    public float xStart;//X起始值
    public float xEnd;//X结束值
    private int Vcount;
    public float[] valueList; //折线图值列表
    private bool isValueChanged;//折线图值列表是否改变过
    public float floatEmpty=-1;//设定的空值（就是不画线和点）
    private float yScale;//系数


    public UGUI_LineChart()
    {
        //lineColor = Color.yellow;
        valueList = new float[]{ 30f, 50f, 80f, 30f, 20f};
        pointImageList = new List<RectTransform>();
    }

    public void UpdateData(List<float> values)
    {
        //valueList.Clear();
        valueList = values.ToArray();
        isValueChanged = true;
        //很重要，界面才会重新刷新
        SetVerticesDirty();
    }

    // Use this for initialization
    void Start () {
        rect = GetComponent<RectTransform>();
        rect.pivot = Vector2.zero;
        //pointImagePrafeb.gameObject.SetActive(false);
        fillColor = new Color(color.r, color.g, color.b, 0.5f);
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }



    protected override void OnDisable()
    {
        base.OnDisable();
        ClearPointImages();
    }
    // Update is called once per frame
    void Update () {
        if (isValueChanged)
        {
            //if (IsCanCreatePointImage)
            //{
                isValueChanged = false;
                //IsCanCreatePointImage = false;
                StartCoroutine(ShowPointImage());
            //}
        }
	}

    public void Init(float yMinT, float yMaxT)
    {
        yMin = yMinT;
        yMax = yMaxT;

    }
    public void Init(float yMinT, float yMaxT, float xStartT, float xEndT)
    {
        yMin = yMinT;
        yMax = yMaxT;

        xStart = xStartT;
        xEnd = xEndT;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        print("OnPopulateMesh");
        vh.Clear();
        Vcount = valueList.Length;
        //if (Application.isPlaying)
        //{
        //    IsCanCreatePointImage = true;
        //}
        //StartCoroutine(ShowPointImage());
        yScale= height/(yMax-yMin);
        for (int i = 0; i < valueList.Length; i++)
        {
            Vector2 p = GetVector2(i);
            if (i == 0) continue;
            Vector2 _p = GetVector2(i - 1);
            //if (valueList[i - 1] == floatEmpty || valueList[i] == floatEmpty)
            //{
            //    continue;
            //}

            if (CheckFalseValue(valueList[i - 1]) || CheckFalseValue(valueList[i]))
            {
                continue;
            }
            AddUIVertexQuad(vh, _p, p);
            if (IsFill)
            {
                AddUIVertexQuadFill(vh, _p, p);
            }
        }

    }
    public bool CheckFalseValue(float v)
    {
        if (v == floatEmpty || v < yMin || v > yMax)
        {
            return true;
        }

        return false;
    }

    public IEnumerator ShowPointImage()
    {
        if (valueList.Length <= 1) yield break;
        yield return null;

        for (int i = 0; i < valueList.Length; i++)
        {
            if (isValueChanged) yield break;
            Vector2 p = GetVector2(i);
            SetPointImage(p, i);
        }
        HideSurplusPointImages();
    }

    public void SetPointImage(Vector3 p, int index)
    {
        if (pointImagePrafeb == null) return;
        RectTransform img;
        if (pointImageList.Count > index)
        {
            img = pointImageList[index];
            //img.gameObject.SetActive(true);
        }
        else
        {
            img = Instantiate(pointImagePrafeb);
            pointImageList.Add(img);
        }
        if (CheckFalseValue(valueList[index]))
        {
            img.gameObject.SetActive(false);
        }
        else
        {
            img.gameObject.SetActive(true);
        }

        img.transform.SetParent(pointImageParent.transform);
        //print(p);
        img.anchoredPosition = p;
        //anchoredPosition
        //img.localPosition = p;
        img.sizeDelta = pointImageSize;
        img.name = ""+index;
        img.SetSiblingIndex(index);
        img.transform.localPosition = new Vector3(img.transform.localPosition.x, img.transform.localPosition.y, 0);
        img.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 隐藏多余的点图片
    /// </summary>
    public void HideSurplusPointImages()
    {
        int childCount = pointImageList.Count;
        if (childCount > Vcount)
        {
            for (int i = childCount - 1; i >= Vcount; i--)
            {
                pointImageList[i].gameObject.SetActive(false);
            }
        }

    }

    /// <summary>
    /// 删除所有点图片
    /// </summary>
    public void ClearPointImages()
    {
        int childCount = pointImageList.Count;
        foreach (RectTransform rec in pointImageList)
        {
            DestroyImmediate(rec.gameObject);
        }
        pointImageList.Clear();
    }

    /// <summary>
    /// 创建Line
    /// </summary>
    /// <param name="vh"></param>
    /// <param name="index"></param>
    public void AddUIVertexQuad(VertexHelper vh, Vector2 p1, Vector2 p2)
    {



        //print(h);
        Vector2 v0;
        Vector2 v1;
        Vector2 v2;
        Vector2 v3;
        float k = (p2.y - p1.y) / (p2.x - p1.x);
        if (k == 0)
        {
            v0 = new Vector2(p1.x, p1.y - thickness / 2);
            v1 = new Vector2(p1.x, p1.y + thickness / 2);
            v2 = new Vector2(p2.x, p2.y + thickness / 2);
            v3 = new Vector2(p2.x, p2.y - thickness / 2);
        }
        else
        {
            float dy = (float)Math.Sqrt(Math.Pow(thickness/2, 2) / (1 + Math.Pow(k, 2)));
            float absK = Math.Abs(k);

            if (k < 0)
            {
                v0 = new Vector2(p1.x - absK * dy, p1.y - dy);
                v1 = new Vector2(p1.x + absK * dy, p1.y +  dy);
                v2 = new Vector2(p2.x + absK * dy, p2.y +  dy);
                v3 = new Vector2(p2.x - absK * dy, p2.y -  dy);
            }
            else
            {
                v0 = new Vector2(p1.x + absK * dy, p1.y -  dy);
                v1 = new Vector2(p1.x - absK * dy, p1.y +  dy);
                v2 = new Vector2(p2.x - absK * dy, p2.y +  dy);
                v3 = new Vector2(p2.x + absK * dy, p2.y -  dy);
            }
        }

        UIVertex[] verts = new UIVertex[4];
        verts[0].position = v0;
        verts[0].color = color;
        verts[0].uv0 = Vector2.zero;

        verts[1].position = v1;
        verts[1].color = color;
        verts[1].uv0 = Vector2.zero;

        verts[2].position = v2;
        verts[2].color = color;
        verts[2].uv0 = Vector2.zero;

        verts[3].position = v3;
        verts[3].color = color;
        verts[3].uv0 = Vector2.zero;

        vh.AddUIVertexQuad(verts);
    }

    public void AddUIVertexQuadFill(VertexHelper vh, Vector2 p1, Vector2 p2)
    {
        //print(h);
        Vector2 v0 = new Vector2(p1.x, 0);
        Vector2 v1 = p1;
        Vector2 v2 = p2;
        Vector2 v3 = new Vector2(p2.x, 0);

        UIVertex[] verts = new UIVertex[4];
        verts[0].position = v0;
        verts[0].color = fillColor;
        verts[0].uv0 = Vector2.zero;

        verts[1].position = v1;
        verts[1].color = fillColor;
        verts[1].uv0 = Vector2.zero;

        verts[2].position = v2;
        verts[2].color = fillColor;
        verts[2].uv0 = Vector2.zero;

        verts[3].position = v3;
        verts[3].color = fillColor;
        verts[3].uv0 = Vector2.zero;

        vh.AddUIVertexQuad(verts);
    }


    public Vector2 GetVector2(int index)
    {
        return new Vector2((index) * width / (valueList.Length-1), (valueList[index] - yMin)* yScale);
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        UpdateData(new List<float>());
    }
}
