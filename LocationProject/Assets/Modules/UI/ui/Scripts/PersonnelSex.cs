using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelSex : MonoBehaviour {
    public Dropdown PerSexDropdownItem;
    public List<string> tempNames;
    
    void Start () {
        PerSexDropdownItem = GetComponent<Dropdown>();
        tempNames = new List<string>();
        AddName();
        SetDropdownData(tempNames);

    }
	private void SetDropdownData(List <string > showItem)
    {
        PerSexDropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i=0;i < showItem.Count;i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showItem[i];
            PerSexDropdownItem.options.Add(tempData);
        }
        
        
    }
    public void AddName()
    {
        string n1 = "未知";
        string n2 = "男";
        string n3= "女";
       
        tempNames.Add(n1);
        tempNames.Add(n2);
        tempNames.Add(n3);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
