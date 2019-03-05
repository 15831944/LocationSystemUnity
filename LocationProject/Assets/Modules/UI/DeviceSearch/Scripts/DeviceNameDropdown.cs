using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceNameDropdown : MonoBehaviour {

    public static DeviceNameDropdown Instance;
    public Dropdown devNameDropdown;
    public List<string> devNameList;
    Dropdown.OptionData tempData;
    void Start () {
        Instance = this;
        devNameDropdown = GetComponent<Dropdown>();
        devNameList = new List<string>();
        AddName();
        SetDropdownDate(devNameList);
    }
	
    private void SetDropdownDate(List <string > data)
    {
        devNameDropdown.options.Clear();
        for (int i =0;i <data .Count;i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = data[i];
            devNameDropdown.options.Add(tempData);
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
    public void AddName()
    {
        string n0 = "厂家名称";
        string n1 = "艾默生";
        string n2 = "霍尼韦尔";
        devNameList.Add(n0);
        devNameList.Add(n1);
        devNameList.Add(n2);
    }
}
