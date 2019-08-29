using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionItemUI : MonoBehaviour {
    private Toggle itemToggle;//Item按钮
    public Text txtNumber;//编号
    public Text txtPerson;//负责人

    //  public PersonnelMobileInspection info;//操作票信息

    public InspectionTrack InspectionTrackInfo;
    public Text NumText;//序号
    public ChangeTextColor changeTextColor;
    // Use this for initialization
    void Start()
    {
        itemToggle = GetComponent<Toggle>();
        if (itemToggle != null)
        {
            MobileInspectionUI_N.Instance.ToggleGroupAdd(itemToggle);
            itemToggle.onValueChanged.AddListener(ItemToggle_OnValueChanged);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="numberStr"></param>
    /// <param name="personStr"></param>
    public void Init(InspectionTrack infoT)
    {
        InspectionTrackInfo = infoT;
        UpdateData(InspectionTrackInfo.Code, InspectionTrackInfo.Name );
        NumText.text = MobileInspectionUI_N.Instance.mobileInspectionNum.ToString();
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="numberStr"></param>
    /// <param name="personStr"></param>
    public void UpdateData(string numberStr, string personStr)
    {
        txtNumber.text = numberStr;
        txtPerson.text = personStr;
    }

    /// <summary>
    /// Item按钮触发
    /// </summary>
    public void ItemToggle_OnValueChanged(bool ison)
    {
        if (ison)
        {
            //print("ItemBtn_OnClick!");
            GetInspectionDetail(InspectionTrackInfo,infoDetail=> 
            {
                InspectionTrackInfo = infoDetail;
                MobileInspectionDetailsUI.Instance.Show(InspectionTrackInfo);
                MobileInspectionInfoFollow.Instance.DateUpdate(InspectionTrackInfo);
                ToggleGroup toggleGroup = MobileInspectionUI_N.Instance.toggleGroup;
                FunctionSwitchBarManage.Instance.SetTransparentToggle(true);
                changeTextColor.ClickTextColor();
                MobileInspectionInfoManage.Instance.CloseWindow();//关闭巡检点详情窗口
                MobileInspectionHistoryDetailsUI.Instance.CloseBtn_OnClick();//关闭巡检项窗口
            });           
        }
        else
        {
           	changeTextColor.NormalTextColor();

			MobileInspectionInfoFollow.Instance.Hide();
			FunctionSwitchBarManage.Instance.SetTransparentToggle(false);
			MobileInspectionDetailsUI.Instance.SetWindowActive(false);
            MobileInspectionInfoManage.Instance.CloseWindow();//关闭巡检点详情窗口
            MobileInspectionHistoryDetailsUI.Instance.CloseBtn_OnClick();//关闭巡检项窗口
        }
    }

    private void GetInspectionDetail(InspectionTrack oldInfo, Action<InspectionTrack> onDataRecieve=null)
    {
        if(oldInfo.Route!=null)
        {
            onDataRecieve(oldInfo);
        }
        else
        {
            CommunicationObject service = CommunicationObject.Instance;
            if (service)
            {
                InspectionTrack trackNew = null;
                ThreadManager.Run(() =>
                {
                    trackNew = service.GetInspectionTrackById(oldInfo);
                }, () =>
                {
                    if (onDataRecieve != null)
                    {
                        onDataRecieve(trackNew);
                    }
                }, "");
            }
            else
            {
                if (onDataRecieve != null) onDataRecieve(null);
            }
        }        
    }
}
