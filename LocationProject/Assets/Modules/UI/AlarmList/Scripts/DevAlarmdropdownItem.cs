using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevAlarmdropdownItem : MonoBehaviour
{
    public static DevAlarmdropdownItem instance;
    public Dropdown devAlarmLeveldropdown;

    public List<string> tempNames;
    void Start()
    {
        instance = this;
        devAlarmLeveldropdown = GetComponent<Dropdown>();
        tempNames = new List<string>();
        AddName();
        SetDropdownData(tempNames);
        devAlarmLeveldropdown.onValueChanged.AddListener(DevAlarmListManage.Instance.GetScreenDevAlarmItems);
    }
    Dropdown.OptionData tempData;
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="showName"></param>
	private void SetDropdownData(List<string> showName)
    {
        devAlarmLeveldropdown.options.Clear();

        for (int i = 0; i < showName.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showName[i];
            devAlarmLeveldropdown.options.Add(tempData);
        }
        devAlarmLeveldropdown.captionText.text = showName[0];
    }

    /// <summary>
    /// 添加名字
    /// </summary>
	public void AddName()
    {
        string n1 = "全部告警";
        string n2 = "高级告警";
        string n3 = "中级告警";
        string n4 = "低级告警";

        tempNames.Add(n1);
        tempNames.Add(n2);
        tempNames.Add(n3);
        tempNames.Add(n4);

    }
}
