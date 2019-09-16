using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevAlarmLineChartDropdown : MonoBehaviour {

    public Dropdown TypeDropdown;
    void Start()
    {
        
    }
    private void SetDropdownData(List<string> showName)
    {
        TypeDropdown.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showName.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showName[i];
            TypeDropdown.options.Add(tempData);
        }
        TypeDropdown.captionText.text = showName[0];
    }
    public void AddName()
    {       
        SetDropdownData(DevAlarmStatisticalManage.Instance.TypeNameList);
    }

  
	
	// Update is called once per frame
	void Update () {
		
	}
}
