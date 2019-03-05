using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryItem : MonoBehaviour {

    public InspectionTrackHistory info;//信息
    public Text NumText;//巡检序号
    public Text NumberInspectionText;//巡检编号
    public Text RoutingNameText;//路线名称  
    public Text StartTimeText;//巡检开始时间
    public Text EndTimeText;//巡检结束时间
    public Button detailBtn;//详情按钮
    public Text StateText;
    
    // Use this for initialization
    void Start()
    {
        detailBtn.onClick.AddListener(DetailBtn_OnClick);
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    public void Init(InspectionTrackHistory infoT)
    {
        info = infoT;
        NumText.text = info.Id .ToString();
        NumberInspectionText.text = info.Code.ToString();
        RoutingNameText.text = info.Name;
        if (info.dtStartTime != null)
        {
            DateTime timeN = (DateTime)info.dtStartTime ;
            StartTimeText.text = timeN.ToString("yyyy年MM月dd日 HH:mm");
        }
        else
        {
            StartTimeText.text = "";
        }
        if (info.dtEndTime != null)
        {
            DateTime timeN = (DateTime)info.dtEndTime;
            EndTimeText.text = timeN.ToString("yyyy年MM月dd日 HH:mm");        
         
        }
        else
        {
            EndTimeText.text = "";        
        }
        StateText.text = info.State;
     
    }

    /// <summary>
    /// 详情按钮事件触发
    /// </summary>
    public void DetailBtn_OnClick()
    {
        MobileInspectionHistoryDetailInfo.Instance.DateUpdate(info );
        MobileInspectionHistory_N.Instance.CloseBtn_OnClick();
    }
}
