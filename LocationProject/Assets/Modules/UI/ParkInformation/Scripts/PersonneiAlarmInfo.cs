using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonneiAlarmInfo : MonoBehaviour
{

    public Text PerNum;
    public Text PerName;
    public Text perJob;
    public Text Type;
    public Text Content;
    public Text StartTime;
    public Text EndTime;
    public Button But;
    private string PerID;
    void Start()
    {

    }
    public void SetEachItemInfo(LocationAlarm info)
    {

        PerNum.text = info.Personnel.WorkNumber.ToString();
        PerName.text = info.Personnel.Name.ToString();
        perJob.text = info.Personnel.Pst.ToString();
        Content.text = info.Content.ToString();
        Type.text = info.AlarmType.ToString();
        string startTime1 = info.CreateTime.ToString();
        if (startTime1 == "1/1/0001 12:00:00 AM")
        {
            StartTime.text = "";
        }
        else
        {
            DateTime NewTime = Convert.ToDateTime(startTime1);
            StartTime.text = NewTime.ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        string HandleTime1 = info.HandleTime.ToString();

        if (HandleTime1.Contains("2000"))
        {
            EndTime.text = "<color=#C66BABFF>未消除</color>"; ;
        }
        else
        {
            DateTime NewTime = Convert.ToDateTime(HandleTime1);
            EndTime.text = "<color=#FFFFFFFF>已消除</color>" + " " + NewTime.ToString("yyyy年MM月dd日 HH:mm:ss");
        }
        
        PerID = info.TagId.ToString();
        But.onClick.AddListener(() =>
       {
           PersonPosition(PerID,info );
       });

    }
    public void PersonPosition(string PerID, LocationAlarm info)
    {
        int perId = int.Parse(PerID);
        LocationManager.Instance.FocusPersonAndShowInfo(perId);
        PersonnelAlarmParkInfo.Instance.ShowPersonnelAlarmParkWindow(false);
        JudgePerOnLine(perId, info);
    }
    public void JudgePerOnLine(int tagNum, LocationAlarm per)
    {
        List<LocationObject> listT = LocationManager.Instance.GetPersonObjects();
        LocationObject locationObjectT = listT.Find((item) => item.personnel.TagId == tagNum);
        if (per.Tag == null || locationObjectT == null || per.Tag.IsActive == false)
        {
            UGUIMessageBox.Show("当前人员已离线或者不在监控区域！");
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
