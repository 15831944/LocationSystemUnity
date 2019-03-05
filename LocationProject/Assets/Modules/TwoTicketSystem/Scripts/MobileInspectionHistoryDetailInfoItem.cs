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

    public void ShowInspectionTrackHistory(PatrolPointHistory info)
    {
     
       
        NumText.text = info.DeviceId.ToString();
        TxtPersonnelNum.text = info.StaffCode.ToString();
        TxtPerson.text = info.StaffName.ToString();
        devText.text = info.DevName.ToString();
        ItemsBut.onClick.AddListener(() =>
        {
            ItemTog_OnClick(info);
        });

    }
    public void ItemTog_OnClick(PatrolPointHistory item)
    {
        MobileInspectionHistoryRouteDetails.Instance.Show(item);
        MobileInspectionHistoryDetailInfo.Instance.CloseMobileInspectionHistoyItemWindow();
    }
}
