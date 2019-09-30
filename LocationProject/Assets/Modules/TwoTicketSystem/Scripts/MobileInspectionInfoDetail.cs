using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInspectionInfoDetail : MonoBehaviour
{
    public Text NumText;
    public Text PersonnelText;
    public Text PersonnelNumText;
    public Text DevNameText;
    public Text DevKKSCode;
    public Button ButDetail;
    void Start()
    {

    }
    public void Init(PatrolPoint infoT,int currentIndex)
    {

        NumText.text = currentIndex.ToString();
        PersonnelText.text = string.IsNullOrEmpty(infoT.StaffName) ? "--" : infoT.StaffName;
        PersonnelNumText.text = string.IsNullOrEmpty(infoT.StaffCode) ? "--" : infoT.StaffCode;
        string devName = string.IsNullOrEmpty(infoT.DevName) ? infoT.DeviceCode : infoT.DevName;
        DevNameText.text = string.IsNullOrEmpty(devName) ? "--" : devName;
        DevKKSCode.text = string.IsNullOrEmpty(infoT.KksCode)?"--":infoT.KksCode;

        ButDetail.onClick.AddListener(() =>
        {
            ItemTog_OnClick(infoT);
        });

    }
    public void ItemTog_OnClick(PatrolPoint item)
    {
        MobileInspectionHistoryDetailsUI.Instance.Show(item);
    }
}
