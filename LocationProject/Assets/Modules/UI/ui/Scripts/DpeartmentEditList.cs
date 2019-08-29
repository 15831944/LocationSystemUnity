using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DpeartmentEditList : MonoBehaviour {

    public Dropdown DepartmentDropdownItem;
    void Start()
    {
        DepartmentDropdownItem = GetComponent<Dropdown>();

    }
    public void DepartManagement(List<Department> DepartList)
    {
       
        SetDropdownData(DepartList);
    }
    private void SetDropdownData(List<Department> showItem)
    {
        DepartmentDropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showItem.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showItem[i].Name.ToString();
            DepartmentDropdownItem.options.Add(tempData);
        }
        //     DepartmentDropdownItem.captionText.text  = showItem[0].Name .ToString();
    }
}
