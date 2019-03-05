using Location.WCFServiceReferences.LocationServices;
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
    public Button ButDetail;
    void Start()
    {

    }
    public void Init(PatrolPoint infoT)
    {

        if (infoT.DeviceId == null)
        {
            NumText.text = "";
        }
        else
        {
            NumText.text = infoT.DeviceId.ToString();
        }
        if (infoT .StaffName==null)
        {
            PersonnelText.text = "";
        }
        else
        {
            PersonnelText.text = infoT.StaffName.ToString();
        }
        if (infoT.StaffCode == null)
        {
            PersonnelNumText.text = "";
        }
        else
        {
            PersonnelNumText.text = infoT.StaffCode.ToString();
        }
        if (infoT.DevName == null)
        {
            DevNameText.text = "";
        }
        else
        {
            DevNameText.text = infoT.DevName.ToString();
        }

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
