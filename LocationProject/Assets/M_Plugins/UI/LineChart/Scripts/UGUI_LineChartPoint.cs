using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UGUI_LineChartPoint : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    //public int index;
    public Canvas canvas;//当前Canvas
    private RectTransform lineChartRect;
    public UGUI_LineChart lineChart;
    public  RectTransform rectTooltipPanel;

    public string Xstr;
    public string Ystr;

    // Use this for initialization
    protected void Start () {
        lineChartRect = lineChart.GetComponent<RectTransform>();
        canvas = lineChart.canvas;
    }

    // Update is called once per frame
    protected void Update () {
        //ShowTooltipPanel();

    }

    //public void SetIndex(int indexT)
    //{
    //    index = indexT;
    //}

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltipPanel();
        UpdateData();
        UpdateTooltipPanel(Xstr, Ystr);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        rectTooltipPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 展示提示窗口列表
    /// </summary>
    public void ShowTooltipPanel()
    {
        Vector2 pos;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(lineChartRect, Input.mousePosition, canvas.worldCamera, out pos))
        {
            rectTooltipPanel.anchoredPosition = pos;
            Debug.Log(pos);
        }
        rectTooltipPanel.gameObject.SetActive(true);
    }

    public virtual void UpdateTooltipPanel(string x,string y)
    {
        if (lineChart.IsTooltipPanel == false) return;
        UGUI_LineChartToolTip toolTip = rectTooltipPanel.GetComponent<UGUI_LineChartToolTip>();
        toolTip.Show(x, y);
    }

    public virtual void UpdateData()
    {

    }

    public void SetData(string x, string y)
    {
        Xstr = x;
        Ystr = y;
    }
}
