using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Location.WCFServiceReferences.LocationServices;

/// <summary>
/// 首页信息
/// </summary>
public class FVIntroduce : MonoBehaviour {
    /// <summary>
    /// 标题文本
    /// </summary>
    public Text TitleText;
    /// <summary>
    /// 内容文本框
    /// </summary>
    public Text ContentText;
    /// <summary>
    /// 地理位置按钮
    /// </summary>
    public Button PosButton;
    /// <summary>
    /// 项目规模按钮
    /// </summary>
    public Button ProjectScaleButton;
    /// <summary>
    /// 程序退出按钮
    /// </summary>
    public Button ButtonExit;
    /// <summary>
    /// 区域信息
    /// </summary>
    public GameObject AreaInfoContent;
    /// <summary>
    /// 区域统计信息
    /// </summary>
    //private AreaStatistics AreaInfo;

    private bool isRefresh;//是否刷新

    [HideInInspector]
    public string posTitle="地理位置";
    [HideInInspector]
    public string posContent= "四会市位于广东省中部偏西，珠江三角洲西北部，西、北、绥三江下游，属珠江三角洲经济区范围。四会东面与佛山三水区交界，南面与鼎湖区相连，西北面与广宁县接壤。";
    [HideInInspector]
    public string projectScaleTitle="项目规模";
    [HideInInspector]
    public string projectScaleContent= "项目建设规模指的是项目的占地面积、建筑面积、投资总额等\n 应选择合理的建设规模，以达到规模经济的要求。但规模扩大所产生效益不是无限的，它受到技术进步、管理水平、项目经济技术环境等多种因素的制约。";
	// Use this for initialization
	void Start () {
        PosButton.onClick.AddListener(showPos);
        ProjectScaleButton.onClick.AddListener(showProjectScale);
        ButtonExit.onClick.AddListener(ExitApplication);
        StartRefresh(true);//第一次进入程序，手动刷新一次
        SceneEvents.FullViewStateChange += StartRefresh;
    }

    ///// <summary>
    ///// 刷新时间间隔
    ///// </summary>
    //public int RefreshInterval = 5;

    public void StartRefresh(bool isOn)
    {
        //Debug.LogError("StartRefresh"+isOn);
        if(isOn)
        {
            if (!IsInvoking("TryGetAreaInfo"))
            {
                InvokeRepeating("TryGetAreaInfo", 0, CommunicationObject.Instance.AreaStatisticsRefreshInterval);
            }
        }
        else
        {
            CancelInvoke("TryGetAreaInfo");
        }
     
    }
    /// <summary>
    /// 获取区域统计信息
    /// </summary>
    private void TryGetAreaInfo()
    {
        FactoryDepManager dep = FactoryDepManager.Instance;
        if (dep == null ||isRefresh) return;
        isRefresh = true;

        //Log.Error("FVIntroduce.TryGetAreaInfo");
        //if (!CommunicationObject.Instance.isAsync)
        //{
        //    ThreadManager.Run(() =>
        //    {
        //         return CommunicationObject.Instance.GetAreaStatistics(dep.NodeID);
        //    }, AfterGetAreaInfo, "TryGetAreaInfo");
        //}
        //else
        //{
        //    CommunicationObject.Instance.GetAreaStatisticsAsync(dep.NodeID, AfterGetAreaInfo);
        //}

        CommunicationObject.Instance.GetAreaStatisticsAsync(dep.NodeID, AfterGetAreaInfo);
    }

    private void AfterGetAreaInfo(AreaStatistics areaStatistics)
    {
        if (areaStatistics == null)
        {
            Log.Error("FVIntroduce.AfterGetAreaInfo", "areaStatistics==null");return;
        }
        SetInfo(AreaInfoContent.transform.GetChild(1), areaStatistics.PersonNum.ToString());
        SetInfo(AreaInfoContent.transform.GetChild(2), areaStatistics.DevNum.ToString());
        SetInfo(AreaInfoContent.transform.GetChild(3), areaStatistics.LocationAlarmNum.ToString());
        SetInfo(AreaInfoContent.transform.GetChild(4), areaStatistics.DevAlarmNum.ToString());
        isRefresh = false;
    }

    private void SetInfo(Transform obj,string value)
    {
        Text t = obj.GetComponent<Text>();
        if(t)
        {
            t.text = value;
        }
    }

    /// <summary>
    /// 立刻完成动画
    /// </summary>
    private void CompleteTween()
    {
        TitleText.DOComplete();
        ContentText.DOComplete();
    }
    /// <summary>
    /// 显示地理位置
    /// </summary>
    public void showPos()
    {
        CompleteTween();
        ChangeButtonColor(PosButton,ProjectScaleButton);
        TitleText.text = "";
        ContentText.text = "";
        TitleText.DOText(posTitle,0.5f);
        ContentText.DOText(posContent,0.8f);
        TitleText.DOFade(0.1f, 0.05f).SetLoops(6, LoopType.Yoyo);
        ContentText.DOFade(0.1f, 0.05f).SetLoops(6, LoopType.Yoyo);
    }
    /// <summary>
    /// 显示项目规模
    /// </summary>
    public void showProjectScale()
    {
        CompleteTween();
        ChangeButtonColor(ProjectScaleButton, PosButton);
        TitleText.text = "";
        ContentText.text = "";
        TitleText.DOText(projectScaleTitle, 0.5f);       
        ContentText.DOText(projectScaleContent, 0.8f);
        TitleText.DOFade(0.1f, 0.05f).SetLoops(6, LoopType.Yoyo);
        ContentText.DOFade(0.1f,0.05f).SetLoops(6,LoopType.Yoyo);
    }
    /// <summary>
    /// 退出当前程序
    /// </summary>
    private void ExitApplication()
    {
        UGUIMessageBox.Show("是否确定退出软件？", () =>
        {
            Application.Quit();
        }, () =>
        {

        });
        //Application.Quit();
    }
    /// <summary>
    /// 改变按钮高亮的颜色
    /// </summary>
    /// <param name="highLightBtn"></param>
    /// <param name="normalBtn"></param>
    private void ChangeButtonColor(Button highLightBtn,Button normalBtn)
    {
        Text highLight = highLightBtn.GetComponentInChildren<Text>();
        Text normal = normalBtn.GetComponentInChildren<Text>();
        if(highLight&&normal)
        {
            SetTextAlpha(highLight,1);
            SetImageAlpha(highLightBtn, 1);

            SetTextAlpha(normal,0.3f);
            SetImageAlpha(normalBtn, 0.3f);
        }
    }
    /// <summary>
    /// 设置文本框透明度
    /// </summary>
    /// <param name="t"></param>
    /// <param name="alpha"></param>
    private void SetTextAlpha(Text t,float alpha)
    {
        alpha = alpha < 0 || alpha > 1 ? 1 : alpha;
        Color temp = t.color;
        temp.a = alpha;
        t.color = temp;
    }
    /// <summary>
    /// 设置图片透明度
    /// </summary>
    /// <param name="t"></param>
    /// <param name="alpha"></param>
    private void SetImageAlpha(Button t, float alpha)
    {
        Image img = t.GetComponent<Image>();
        if(img)
        {
            Color temp = img.color;
            temp.a = alpha;
            img.color = temp;
        }
    }
}
