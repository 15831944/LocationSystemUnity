using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class UGUI_PieChartDoTween : MonoBehaviour
{
    public enum Type
    {
        /// <summary>
        /// 颜色
        /// </summary>
        Color,
        /// <summary>
        /// 图片
        /// </summary>
        Sprite
    }
    /// <summary>
    /// 饼状图类型
    /// </summary>
    public Type type = Type.Color;
    /// <summary>
    /// 是否是设置图片
    /// </summary>
    public bool isPieShowSprite;
    /// <summary>
    /// 颜色列表
    /// </summary>
    public List<Color> colors;
    /// <summary>
    /// 图片列表
    /// </summary>
    public List<Sprite> sprites;
    /// <summary>
    /// 值列表，值得大小：0到1
    /// </summary>
    public List<float> values;//
    /// <summary>
    /// 圆形预设
    /// </summary>
    public Image cicle;
    private Transform circleParent;
    /// <summary>
    /// 动画时间
    /// </summary>
    [Range(0, 1)]
    public float duration = 0.3f;
    /// <summary>
    /// 角度
    /// </summary>
    private float tempAngle = 0;
    /// <summary>
    /// 间隔角度
    /// </summary>
    private float intervalAngle = 2f;

    private List<PieItem> items;

    public UGUI_PieChartDoTween()
    {
        colors = new List<Color>();
        colors.Add(Color.red);
        colors.Add(Color.green);
        colors.Add(Color.blue);
        colors.Add(Color.white);
        colors.Add(Color.yellow);
        colors.Add(Color.gray);
        colors.Add(Color.black);
        colors.Add(Color.cyan);
        colors.Add(Color.magenta);
    }

    // Use this for initialization
    void Start()
    {       
        if (circleParent == null)
        {
            items = new List<PieItem>();
            circleParent = transform.Find("CircleParent");
            if (circleParent == null)
            {
                circleParent = new GameObject("CircleParent").transform;
                ////circleParent = new RectTransform().transform;
                //RectTransform rect= circleParent.gameObject.AddMissingComponent<RectTransform>();
                ////circleParent.name = "CircleParent";
                //circleParent = rect.transform;
                circleParent.SetParent(transform);
                circleParent.localPosition = Vector3.zero;
                if (circleParent == null)
                {
                    Debug.LogError("缺少CircleParent！");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 清除数据
    /// </summary>
    public void Clear()
    {
        List<float> fs = new List<float>();
        Show(fs);
    }
    /// <summary>
    /// 设置值列表
    /// </summary>
    public void SetValues(List<float> valuesT)
    {
        values = valuesT;
    }

    public void Show(List<float> valuesT)
    {
        if (type == Type.Color)
        {
            ShowPie_ColorType(valuesT);
        }
        else if (type == Type.Sprite)
        {
            ShowPie_SpriteType(valuesT);
        }
        //else
        //{
        //    ShowPie_SpriteType(valuesT);
        //}
    }

    /// <summary>
    /// 显示设置了图片的饼状图
    /// </summary>
    /// <param name="valuesT"></param>
    private void ShowPie_SpriteType(List<float> valuesT)
    {
        if (circleParent == null)
        {
            Start();
            //Debug.LogError("缺少CircleParent！");
        }

        SetChildsActive(false);

        items.Clear();
        tempAngle = 0f;
        values = valuesT;
        int childcount;
        childcount = circleParent.childCount;
        float sum = 0f;
        for (int i = 0; i < values.Count; i++)
        {
            sum += values[i];
            Image img;
            if (i < childcount)
            {
                GameObject o = circleParent.GetChild(i).gameObject;
                img = o.GetComponent<Image>();
            }
            else
            {
                img = Instantiate(cicle);
                RectTransform ret = img.gameObject.GetComponent<RectTransform>();
                img.transform.SetParent(circleParent);
                ret.localPosition = Vector3.zero;
                ret.anchorMax = new Vector2(1, 1);
                ret.anchorMin = new Vector2(0, 0);
                ret.offsetMax = Vector2.zero;
                ret.offsetMin = Vector2.zero;
                img.transform.localScale = Vector3.one;
            }
            img.color = Color.white;
            img.transform.eulerAngles = new Vector3(0, 0, -tempAngle);
            if (sprites.Count != 0)
            {
                if (i < sprites.Count)
                {
                    img.overrideSprite = sprites[i];
                }
                else
                {
                    img.overrideSprite = sprites[sprites.Count - 1];
                }
            }
            else
            {
                img.overrideSprite = new Sprite();
            }

            //img.gameObject.SetActive(true);
            //img.fillAmount = values[i];

            tempAngle = sum * 360F;
            PieItem p = new PieItem(img, values[i]);
            items.Add(p);
        }

        for (int i = values.Count; i < childcount; i++)
        {
            Transform tran = circleParent.GetChild(i);
            tran.gameObject.SetActive(false);
        }
        DoTweenOpen(items);
    }

    /// <summary>
    /// 显示饼状图
    /// </summary>
    /// <param name="valuesT"></param>
    private void ShowPie_ColorType(List<float> valuesT)
    {
        if (circleParent == null)
        {
            Start();
            //Debug.LogError("缺少CircleParent！");
        }

        SetChildsActive(false);

        items.Clear();
        tempAngle = 0f;
        values = valuesT;
        int childcount;
        childcount = circleParent.childCount;
        float sum = 0f;
        for (int i = 0; i < values.Count; i++)
        {
            sum += values[i];
            Image img;
            if (i < childcount)
            {
                GameObject o = circleParent.GetChild(i).gameObject;
                img = o.GetComponent<Image>();
            }
            else
            {
                img = Instantiate(cicle);
                RectTransform ret = img.gameObject.GetComponent<RectTransform>();
                img.transform.SetParent(circleParent);
                ret.localPosition = Vector3.zero;
                ret.anchorMax = new Vector2(1, 1);
                ret.anchorMin = new Vector2(0, 0);
                ret.offsetMax = Vector2.zero;
                ret.offsetMin = Vector2.zero;
                img.transform.localScale = Vector3.one;
            }
            img.overrideSprite = null;
            img.transform.eulerAngles = new Vector3(0, 0, -tempAngle);
            if (i < colors.Count)
            {
                img.color = colors[i];
            }
            else
            {
                img.color = colors[colors.Count];
            }

            //img.gameObject.SetActive(true);
            //img.fillAmount = values[i];

            tempAngle = sum * 360F;
            PieItem p = new PieItem(img, values[i]);
            items.Add(p);
        }

        for (int i = values.Count; i < childcount; i++)
        {
            Transform tran = circleParent.GetChild(i);
            tran.gameObject.SetActive(false);
        }
        DoTweenOpen(items);
    }

    /// <summary>
    /// 设置子物体Active为false
    /// </summary>
    private void SetChildsActive(bool b)
    {
        for (int i = 0; i < circleParent.childCount; i++)
        {
            Transform tran = circleParent.GetChild(i);
            tran.gameObject.SetActive(b);
        }
    }

    /// <summary>
    /// 动画展开
    /// </summary>
    public void DoTweenOpen(List<PieItem> items)
    {
        if (items.Count == 0) return;
        //float time = 0.5f;
        //float timeT = time / items.Count;
        int num = 0;
        DoTweenOpen(num);
    }

    /// <summary>
    /// 动画展开
    /// </summary>
    public void DoTweenOpen(int num)
    {
        if (num >= items.Count) return;      
        float timeT = duration * items[num].value;
        Image img = items[num].img;
        float value = items[num].value;
        img.fillAmount = 0;
        img.gameObject.SetActive(true);
        img.DOFillAmount(value, timeT).OnComplete(() =>
        {
            num++;
            DoTweenOpen(num);
        });

    }

    public class PieItem
    {
        public Image img;
        public float value;

        public PieItem(Image img, float value)
        {
            this.img = img;
            this.value = value;
        }
    }
}
