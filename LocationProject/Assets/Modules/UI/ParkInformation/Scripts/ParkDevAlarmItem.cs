using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkDevAlarmItem : MonoBehaviour {

	Color HighAlarm= new Color(250 / 255f, 57 / 255f, 114 / 255f, 255 / 255f);
    Color MiddleAlarm = new Color(250 / 255f, 146 / 255f, 55 / 255f, 255 / 255f);
    Color LowAlarm= new Color(249 / 255f, 250 / 255f, 55 / 255f, 255 / 255f);
    Color Uncertain=new Color(55 / 255f,161 / 255f,255 / 255f, 255 / 255f);
    public Image  DevLevel;
    public Text TypeNameText;
    public Text Name;
    public Text Num;
    public Text Title;
    public Text AlarmTime;
    public Text Message;
    public Text Normal;
    public Button DevPos;

    void Start () {
		
	}
	public void GetDevAlarmData(DeviceAlarm item)
    {
       if (item .Level == Abutment_DevAlarmLevel.高)
        {
            DevLevel.color = HighAlarm;
        }
       else if (item .Level == Abutment_DevAlarmLevel.中)
        {
            DevLevel.color = MiddleAlarm;
        }
       else if (item .Level == Abutment_DevAlarmLevel.低)
        {
            DevLevel.color = LowAlarm;
        }
        else if (item.Level == Abutment_DevAlarmLevel.未定)
        {
            DevLevel.color = Uncertain;
        }
        TypeNameText.text  = item.DevTypeName;
        
        Num.text = item.Id.ToString();
        Name.text = item.DevName.ToString();
        Title.text = item.Title.ToString();
        string time = item.CreateTime.ToString();
        DateTime NewTime = Convert.ToDateTime(time);
        AlarmTime.text  = NewTime.ToString("yyyy年MM月dd日 HH:mm:ss");
        Message.text = item.Message.ToString();
        Normal.text = "<color=#C66BABFF>未消除</color>";
        int dep = (int)item.AreaId;
        DevPos.onClick.AddListener(() =>
        {
            DevBut_Click(item, dep,item.Message);
        });
    }
    /// <summary>
    /// 点击定位设备
    /// </summary>
    /// <param name="devId"></param>
    public void DevBut_Click(DeviceAlarm devAlarm, int DepID,string msg)
    {
        RoomFactory.Instance.FocusDev(devAlarm.DevId.ToString(), DepID,result=> 
        {
            if (!result)
            {
                string msgTitle = DeviceAlarmHelper.TryGetDeviceAlarmInfo(devAlarm);
                UGUIMessageBox.Show(msgTitle);
            }
        });
        ParkDevAlarmInfo.Instance.CloseDevAlarmWindow();
    }
}
