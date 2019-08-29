using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevAlarmType : MonoBehaviour {
    public static  DevAlarmType instance;
    public Dropdown DevTypedropdownItem;

    public   List<string> tempNames;
    void Start()
    {
        instance = this;
        DevTypedropdownItem = GetComponent<Dropdown>();
        tempNames = new List<string>();
        AddName();
        SetDropdownData(tempNames);
        DevTypedropdownItem.onValueChanged.AddListener(DevAlarmListManage.Instance.GetScreenAlarmType);

    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="showName"></param>
    private void SetDropdownData(List<string> showName)
    {
        DevTypedropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showName.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showName[i];
            DevTypedropdownItem.options.Add(tempData);
        }
        DevTypedropdownItem.captionText.text = showName[0];
    }
   
    /// <summary>
    /// 添加名字
    /// </summary>
	public void AddName()
    {
        string n0 = "所有设备";
        string n1 = "基站";
        string n2 = "摄像头";
        string n3 = "生产设备";
        
        tempNames.Add(n0);
        tempNames.Add(n1);
        tempNames.Add(n2);
        tempNames.Add(n3);
       

    }
}
