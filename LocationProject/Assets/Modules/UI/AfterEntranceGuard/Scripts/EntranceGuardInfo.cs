using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntranceGuardInfo : MonoBehaviour
{

    public Text NameEntranceGuard;
    public Text AreaEntranceGuard;
    public Text CardEntranceGuard;
    public Text TimeEntranceGuard;
    public Text StateEntranceGuard;
    public Button OperationBut;
    int DevID;
    int DepID;
    void Start()
    {
        OperationBut.onClick.AddListener(() =>
        {
            DevBut_Click(DevID, DepID);
        });

    }
    public void ShowEntranceGuardInfo(EntranceGuardActionInfo data)
    {
        NameEntranceGuard.text = data.Name.ToString();
        AreaEntranceGuard.text = data.AreadName.ToString();
        CardEntranceGuard.text = data.Code.ToString();
        TimeEntranceGuard.text = data.OperateTime.ToString();
        StateEntranceGuard.text = data.nInOutState.ToString();
        DevID = data.Id;
        DepID = (int)data.AreadId;


    }
    /// <summary>
    /// 点击定位设备
    /// </summary>
    /// <param name="devId"></param>
    public void DevBut_Click(int devId, int DepID)
    {
        RoomFactory.Instance.FocusDev(devId.ToString(), DepID);
        AfterEntranceGuardManage.Instance.CloseWindow();

    }
    void Update()
    {

    }
}
