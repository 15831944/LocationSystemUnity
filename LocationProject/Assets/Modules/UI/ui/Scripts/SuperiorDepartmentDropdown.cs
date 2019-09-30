using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperiorDepartmentDropdown : MonoBehaviour
{
    public Dropdown SuperiorDepDropdown;
    List<Department> ChildrenDepList;
    void Start()
    {

    }
    public void SetDropdownData(Department dep, List<Department> showItem)
    {
        ChildrenDepList = new List<Department>();
        showItem.Remove(dep);
        foreach (var item in showItem)
        {
            if (item .ParentId ==dep .Id)
            {
                ChildrenDepList.Add(item);
            }
        }
        if (ChildrenDepList != null)
        {
            foreach (var item in ChildrenDepList)
            {
                Department ChildrenDep = showItem.Find(i => i.Id == item.Id);
                if (ChildrenDep != null)
                {
                    showItem.Remove(ChildrenDep);
                }
            }
        }Department firstDep = new Department();
        firstDep.Name = "--";
        showItem.Insert(0, firstDep);//将当前的部门排到第一个
        SuperiorDepDropdown.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showItem.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showItem[i].Name.ToString();
            SuperiorDepDropdown.options.Add(tempData);
        }

    }
    void Update()
    {

    }
}
