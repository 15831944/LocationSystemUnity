using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDropdownItem : MonoBehaviour { 
  
    public  Dropdown dropdownItem;
    
    List<string> tempNames;
	void Start () {
        dropdownItem = GetComponent<Dropdown>();
        tempNames = new List<string>();
        AddName();
        SetDropdownData(tempNames);
	}
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="showName"></param>
	private void SetDropdownData(List <string> showName)
    {
        dropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i=0;i<showName .Count;i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showName[i];
            dropdownItem.options.Add(tempData);
        }
        dropdownItem.captionText.text = showName[0];
    }
    /// <summary>
    /// 添加名字
    /// </summary>
	public void AddName()
    {
        string n1 = "人员搜索";
        string n2 = "门禁搜索";
        string n3 = "摄像头搜索";
        tempNames.Add(n1);
        tempNames.Add(n2);
        tempNames.Add(n3);
    }
   
}
