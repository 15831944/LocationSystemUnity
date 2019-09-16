using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeDropdown : MonoBehaviour {

    public Dropdown timeDropdown;
    List<string> tempName;
    void Start () {
        tempName = new List<string>();
        AddName();
     //   timeDropdown.onValueChanged.AddListener(PersonnelBenchmarkMonths.Instance .ScreenPersonnelMonths);
    }
    private void SetDropdownData(List<string> showName)
    {
        timeDropdown.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showName.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showName[i];
            timeDropdown.options.Add(tempData);
        }
        timeDropdown.captionText.text = showName[3];
    }
    public void AddName()
    {
        string n0 = "一个月";
        string n1 = "一个季度";
        string n2 = "半年";
        string n3 = "一年";
        tempName.Add(n0);
        tempName.Add(n1);
        tempName.Add(n2);
        tempName.Add(n3);
        SetDropdownData(tempName);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
