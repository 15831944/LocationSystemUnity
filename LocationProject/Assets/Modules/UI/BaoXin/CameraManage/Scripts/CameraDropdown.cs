using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraDropdown : MonoBehaviour {
   
    public Dropdown CameraTypeDropdown;
    private List<string> AlarmTypeList;
    void Start () {
        AlarmTypeList = new List<string>();
        SetCameraAlarmTypeData();
        CameraTypeDropdown.onValueChanged.AddListener(CameraAlarmManage.Instance .ScreenCameraAlarmType);
    }
   
    public void SetCameraAlarmTypeData()
    {
        string n = "告警类型";
        string n1 = "火警";
        string n2 = "未戴安全帽";
        string n3 = "烟雾";
        AlarmTypeList.Add(n);
        AlarmTypeList.Add(n1);
        AlarmTypeList.Add(n2);
        AlarmTypeList.Add(n3);
        SetDropdownData(AlarmTypeList);
    }

    private void SetDropdownData(List<string > showItem)
    {
        CameraTypeDropdown.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showItem.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showItem[i].ToString();
            CameraTypeDropdown.options.Add(tempData);
        }
        CameraTypeDropdown.captionText.text = showItem[0].ToString();
    }
    void Update () {
		
	}
}
