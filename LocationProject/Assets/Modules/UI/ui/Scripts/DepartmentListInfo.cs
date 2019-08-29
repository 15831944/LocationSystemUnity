using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DepartmentListInfo : MonoBehaviour {

    public List<Department> DepartList;
    IList<Department> departIList;
    public Dropdown DepartmentDropdownItem;
    void Start()
    {
        DepartmentDropdownItem = GetComponent<Dropdown>();

    }
    public void ShowFirstInfo()
    {
        for (int i=0;i < DepartList.Count;i++)
        {
            DepartmentDropdownItem.captionText.text = DepartList[0].Name;
            DepartmentDropdownItem.value = 0;
        }     
    }
    public void DepartManagement()
    {
        DepartList = new List<Department>();
        departIList = CommunicationObject.Instance.GetDepartmentList();
        DepartList = departIList.ToList();
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
