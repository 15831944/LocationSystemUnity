using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddDepartmentEditing : MonoBehaviour {
    public static AddDepartmentEditing Instance;
    public Text IDText;
    public InputField DepartmentName;
    public DpeartmentEditList departmentManagement;
    public GameObject DepartEditingWindow;
    public Button CloseBut;
    public Button EnsureBut;
    public Button CancelBut;

    Department CreatDepartment;
    
    int id;
    void Start()
    {
        CreatDepartment = new Department();
        
        Instance = this;
        CloseBut.onClick.AddListener(()=> 
        {
            CloseDepartmentEditingUI();
            AddDepartmentList.Instance.ShowDepartmentListUI();
        });
        EnsureBut.onClick.AddListener(GetModifyDepartmentData);
    
    }

    public void GetDepartEditInfo(Department dep, List<Department> depList)
    {
        CreatDepartment = dep;
        departmentManagement.DepartManagement(depList);
        DepartmentName.text = dep.Name;

        IDText.text = "<color=#60D4E4FF>Id：</color>" + dep.Id.ToString();
        if (string.IsNullOrEmpty(dep.ParentId.ToString()))
        {
            departmentManagement.DepartmentDropdownItem.captionText.text = "——";
        }
        else
        {
            id = (int)dep.ParentId;
            for (int i = 0; i < depList.Count; i++)
            {
                if (id == depList[i].Id)
                {
                    departmentManagement.DepartmentDropdownItem.captionText.text = depList[i].Name.ToString();
                    departmentManagement.DepartmentDropdownItem.transform.GetComponent<Dropdown>().value = i;
                }

            }
        }

    }
    public void ShowDepartmentEditingUI()
    {
        DepartEditingWindow.SetActive(true);
    }
    public void CloseDepartmentEditingUI()
    {
        DepartEditingWindow.SetActive(false);
    }
    public void GetModifyDepartmentData()
    {
        CreatDepartment.Name = DepartmentName.text;
        CreatDepartment.ParentId = id;
        SaveDepartmentData(CreatDepartment);
        CloseDepartmentEditingUI();
    }
    public void SaveDepartmentData(Department info)
    {
        CommunicationObject.Instance.AddDepartment(info);
    }
}
