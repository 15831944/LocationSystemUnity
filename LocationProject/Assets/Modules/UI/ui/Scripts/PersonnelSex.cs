using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelSex : MonoBehaviour {
    public Dropdown PerSexDropdownItem;
    public List<string> tempNames;
    
    void Start () {
        PerSexDropdownItem = GetComponent<Dropdown>();
      
      //  AddName();
       

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
        PerSexDropdownItem.captionText.text = tempNames[0];


    }
    public void AddName()
    {
        tempNames = new List<string>();
        //string n1 = "未知";
        //string n2 = "男";
        //string n3= "女";
        //   tempNames.Add("--");
        tempNames.Add("男性");
        tempNames.Add("女性");
        tempNames.Add("机器人");
        tempNames.Add("车辆");
        tempNames.Add("物资");
        SetDropdownData(tempNames);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
