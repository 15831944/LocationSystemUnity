using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultHistoryProgressWindow : MonoBehaviour
{

    public Canvas canvas;//画布
    public GameObject PromptBox;//提示框
    public Transform infoGrid;//信息列表
    public MultHistoryPromptBoxItem itemPrefab;//单项预设
    public RectTransform LinesContent;//进度线容器
    public Dictionary<LocationHistoryPath_M, MultHistoryPromptBoxItem> HistoryPath_MultHistoryPromptBoxItem;
    public Text timeText;

    // Use this for initialization
    void Start()
    {
        EventTriggerListener lis = EventTriggerListener.Get(LinesContent.gameObject);
        lis.onHover = LinesContent_OnHover;
        lis.onExit = LinesContent_OnExit;
        lis.onClick = LinesContent_OnClick;
        HistoryPath_MultHistoryPromptBoxItem = new Dictionary<LocationHistoryPath_M, MultHistoryPromptBoxItem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector3 mousePositionTemp=Vector3.zero;
    Vector2 currentPos;

    /// <summary>
    /// 鼠标移动线容器上显示相关信息
    /// </summary>
    public void LinesContent_OnHover(GameObject o)
    {
        if (LinesContent.childCount == 0)
        {
            HidePromptBox();
            return;
        }

        if (mousePositionTemp == Input.mousePosition) return;

        if (UGUIHelper.ScreenPointToLocalPointInRectangle(LinesContent, Input.mousePosition, canvas, out currentPos))
        {
            PromptBox.transform.localPosition = new Vector3(currentPos.x, PromptBox.transform.localPosition.y, PromptBox.transform.localPosition.z);
            ShowPromptBox();
            RefleshPromptBox();
        }
        else
        {
            HidePromptBox();
        }
    }

    /// <summary>
    /// 鼠标离开线容器上显示相关信息
    /// </summary>
    public void LinesContent_OnExit(GameObject o)
    {
        HidePromptBox();
    }

    /// <summary>
    /// 鼠标点击容器，移动进度条
    /// </summary>
    public void LinesContent_OnClick(GameObject o)
    {
        float value = (currentPos.x + LinesContent.sizeDelta.x / 2) / LinesContent.sizeDelta.x;

        MultHistoryPlayUINew.Instance.SetIsMouseDragSlider(true);
        MultHistoryPlayUINew.Instance.SetProcessSliderValue(value);
        MultHistoryPlayUINew.Instance.MouseDragSlider(value);
        //double t = value * MultHistoryPlayUINew.Instance.timeLength;
        //MultHistoryPlayUINew.Instance.SetTimeSum(t);
        MultHistoryPlayUINew.Instance.SetIsMouseDragSlider(false);
        MultHistoryPlayUINew.Instance.istest = true;
    }


    /// <summary>
    /// 显示提示框
    /// </summary>
    public void ShowPromptBox()
    {
        if (!PromptBox.activeInHierarchy)
        {
            PromptBox.SetActive(true);
        }
    }

    /// <summary>
    /// 刷新提示框
    /// </summary>
    public void RefleshPromptBox()
    {
        float w = LinesContent.rect.width;
        float v = (PromptBox.transform.localPosition.x + w / 2) / w;
        float timeT = (float)MultHistoryPlayUINew.Instance.timeLength * v;
        DateTime timeDateTime = MultHistoryPlayUINew.Instance.GetStartTime();
        string currentTimestr = timeDateTime.AddSeconds(timeT).ToString("HH:mm:ss");
        SetTimeText(currentTimestr);

        foreach (LocationHistoryPath_M patht in LocationHistoryManager.Instance.PathList.Items)
        {
            if (HistoryPath_MultHistoryPromptBoxItem.ContainsKey(patht))
            {
                MultHistoryPromptBoxItem o = HistoryPath_MultHistoryPromptBoxItem[patht];
                Color colort = patht.GetColor();
                o.SetIconColor(new Color(colort.r, colort.g, colort.b, 1));
                o.SetPersonName(patht.personnel.Name);

                int indext = patht.GetCompareTime(timeT, 1f);
                if (indext >= 0)
                {
                    List<Position> ps = MultHistoryPlayUINew.Instance.GetPositionsByPersonnel(patht.personnel);
                    if (ps != null)
                    {
                        Position p = ps[indext];

                        DepNode depnode = RoomFactory.Instance.GetDepNodeById((int)p.TopoNodeId);
                        if (depnode)
                        {
                            o.SetAreaName(depnode.NodeName);
                        }
                        else
                        {
                            o.SetAreaName("         ");
                        }
                    }

                }
                else
                {
                    o.SetAreaName("         ");
                }
            }
        }
    }

    /// <summary>
    /// 隐藏提示框
    /// </summary>
    public void HidePromptBox()
    {
        if (PromptBox.activeInHierarchy)
        {
            PromptBox.SetActive(false);
        }
    }

    /// <summary>
    /// 创建项
    /// </summary>
    /// <returns></returns>
    public MultHistoryPromptBoxItem CreateTipItem()
    {
        MultHistoryPromptBoxItem o = Instantiate(itemPrefab);
        o.transform.SetParent(infoGrid.transform);
        return o;
    }

    /// <summary>
    /// 创建提示框Items
    /// </summary>
    public void CreateTipItems()
    {
        ClearItems();
        HistoryPath_MultHistoryPromptBoxItem.Clear();
        foreach (LocationHistoryPath_M patht in LocationHistoryManager.Instance.PathList.Items)
        {
            MultHistoryPromptBoxItem o = CreateTipItem();
            SetTimeText("");
            Color colort = patht.GetColor();
            o.SetIconColor(new Color(colort.r, colort.g, colort.b, 1));
            o.SetPersonName(patht.personnel.Name);
            o.SetAreaName("");
            HistoryPath_MultHistoryPromptBoxItem.Add(patht, o);
            o.gameObject.SetActive(true);
            //o.Init()
        }
    }

    public void ClearItems()
    {
        int count = infoGrid.transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            DestroyImmediate(infoGrid.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 设置时间
    /// </summary>
    public void SetTimeText(string timeStr)
    {
        timeText.text = timeStr;
    }
}
