using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkPersonnelAlarmType : MonoBehaviour {
   
    public Dropdown PerTypedropdownItem;

    public List<string> tempNames;
    // Use this for initialization
    void Start () {
      
        PerTypedropdownItem = GetComponent<Dropdown>();
        tempNames = new List<string>();
        AddName();
        SetDropdownData(tempNames);
        PerTypedropdownItem.onValueChanged.AddListener(PersonnelAlarmParkInfo.Instance.GetScreenPersonnelAlarmType);
    }
    public void StartAlarmType()
    {
        PerTypedropdownItem.captionText.text = "全部告警";
        PerTypedropdownItem.transform.GetComponent<Dropdown>().value = 0;
    }
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="showName"></param>
    private void SetDropdownData(List<string> showName)
    {
        PerTypedropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showName.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showName[i];
            PerTypedropdownItem.options.Add(tempData);
        }
        PerTypedropdownItem.captionText.text = showName[0];
    }
    /// <summary>
    /// 添加名字
    /// </summary>
    public void AddName()
    {
        string n0 = "全部告警";
        string n1 = "区域告警";
        string n2 = "消失告警";
        string n3 = "低电告警";
        string n4 = "传感器告警";
        string n5 = "重启告警";
        string n6 = "非法拆卸";
        string n7 = "其他告警";
        tempNames.Add(n0);
        tempNames.Add(n1);
        tempNames.Add(n2);
        tempNames.Add(n3);
        tempNames.Add(n4);
        tempNames.Add(n5);
        tempNames.Add(n6);
        tempNames.Add(n7);

    }
    // Update is called once per frame
    void Update () {
		
	}
}
