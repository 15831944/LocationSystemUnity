using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UGUI_PieChart : MonoBehaviour {

    public List<Color> colors;//颜色列表
    public List<float> values;//值列表，值得大小：0到1
    public Image cicle;//圆形模板
    private Transform circleParent;

    public UGUI_PieChart()
    {
        colors = new List<Color>();
        colors.Add(Color.red);
        colors.Add(Color.green);
        colors.Add(Color.blue);
        colors.Add(Color.white);
        colors.Add(Color.yellow);
        colors.Add(Color.gray);
        colors.Add(Color.black);
    }

    // Use this for initialization
    void Start()
    {
        if (circleParent == null)
        {
            circleParent = transform.Find("CircleParent");
            if (circleParent == null)
            {
                //circleParent = new GameObject("CircleParent").transform;
                //////circleParent = new RectTransform().transform;
                ////RectTransform rect= circleParent.gameObject.AddMissingComponent<RectTransform>();
                //////circleParent.name = "CircleParent";
                ////circleParent = rect.transform;
                //circleParent.SetParent(transform);
                //circleParent.localPosition = Vector3.zero;
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
    /// 设置值列表
    /// </summary>
    public void SetValues(List<float> valuesT)
    {
        values = valuesT;
    }

    public void Show(List<float> valuesT)
    {
        if (circleParent == null)
        {
            Start();
            //Debug.LogError("缺少CircleParent！");
        }
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
            if (i < colors.Count)
            {
                img.color = colors[i];
            }
            else
            {
                img.color = colors[colors.Count];
            }
            img.gameObject.SetActive(true);
            img.transform.SetAsFirstSibling();
            if (i == values.Count - 1)
            {
                img.fillAmount = 1;
                continue;
            }
            img.fillAmount = sum;
        }

        for (int i = values.Count; i < childcount; i++)
        {
            Transform tran = circleParent.GetChild(i);
            tran.gameObject.SetActive(false);
        }
    }


}
