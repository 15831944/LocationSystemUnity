using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceTypeDropdown : MonoBehaviour {

    public static DeviceTypeDropdown Instance;
    public Dropdown DevTypeDropdown;

    public List<string> devTyprList;
    Dropdown.OptionData tempData;
    void Start () {
        Instance = this;
        DevTypeDropdown = GetComponent<Dropdown>();
        devTyprList = new List<string>();
        AddName();
        SetDropdownata(devTyprList);
    }
	private void SetDropdownata(List <string > data)
    {
        DevTypeDropdown.options.Clear();
        for (int i=0;i <data .Count;i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = data[i];
            DevTypeDropdown.options.Add(tempData);

        }
        DevTypeDropdown.captionText.text = data[0];
    }
	// Update is called once per frame
	void Update () {
		
	}
    public void AddName()
    {
        string n1 = "设备类型";
        string n2 = "基站";
        string n3 = "摄像头";
        string n4 = "门禁";
        devTyprList.Add(n1);
        devTyprList.Add(n2);
        devTyprList.Add(n3);
        devTyprList.Add(n4);
    }
}
