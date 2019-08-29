using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelDropdown : MonoBehaviour {

   
    public Dropdown PerDropdown;

    public List<string> devTyprList;
    Dropdown.OptionData tempData;
    void Start()
    {

        PerDropdown = GetComponent<Dropdown>();
        devTyprList = new List<string>();
        AddName();
        SetDropdownata(devTyprList);
        PerDropdown.onValueChanged.AddListener(DataPaging.Instance.ScreenPresonnelCardRole);
    }
    private void SetDropdownata(List<string> data)
    {
        PerDropdown.options.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = data[i];
            PerDropdown.options.Add(tempData);

        }
        PerDropdown.captionText.text = data[0];
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void AddName()
    {
        string n1 = "全部";
        string n2 = "可定位";
        string n3 = "不可定位";
        
        devTyprList.Add(n1);
        devTyprList.Add(n2);
        devTyprList.Add(n3);
        
    }
}
