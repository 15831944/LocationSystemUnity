using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionHistoryDetailInfoItem : MonoBehaviour {

    public Text NumText;//序号
    public Text TxtPersonnelNum;//巡检人工号
    public Text TxtPerson;//巡检人
    public Text devText;//巡检设备名称

    public Text Title;
    public Button ItemsBut;



    // Use this for initialization
    void Start()
    {

    }

    public void ShowInspectionTrackHistory(PatrolPointHistory info,int currentIndex)
    {
        NumText.text = currentIndex.ToString();
        TxtPersonnelNum.text = string.IsNullOrEmpty(info.StaffCode)?"--":info.StaffCode;
        TxtPerson.text = string.IsNullOrEmpty(info.StaffName) ? "--" : info.StaffName;
        string devName = string.IsNullOrEmpty(info.DevName) ? "--" : info.DevName;
        devText.text = string.IsNullOrEmpty(devName) ? "--" : info.DeviceCode;
        ItemsBut.onClick.AddListener(() =>
        {
            ItemTog_OnClick(info);
        });

    }
    public void ItemTog_OnClick(PatrolPointHistory item)
    {
        MobileInspectionHistoryRouteDetails.Instance.Show(item);
        //if (item.Checks != null)
        //{
        //    MobileInspectionHistoryDetailInfo.Instance.CloseMobileInspectionHistoyItemWindow();
        //}
    }
}
