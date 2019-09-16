using System.Collections;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EditPersonnelInformation : MonoBehaviour
{
    public static EditPersonnelInformation Instance;
    [System.NonSerialized] Personnel PerInfo;
    public Text SerialNum;
    public InputField Name;
    public Text BornTime;
    public CalendarChange StartcalendarDay;
    public InputField WorkNum;
    public Text TagName;
    public Button TagBut;
    public InputField Phone;
    public Dropdown job;
    public Toggle UseTog;
    /// <summary>
    /// 人员编辑界面窗口
    /// </summary>
    public GameObject EditPerWindow;
    public PersonnelSex personnelSex;
    public Button CloseBut;
    /// <summary>
    /// 点击部门列表
    /// </summary>
    public Button DepartBut;
    /// <summary>
    /// 点击岗位列表
    /// </summary>
    public Button JobBut;
    public Text departmentText;
    public JobsManagement JobManagements;
    public Button EnsureBut;
    public Button CancelBut;
    [System.NonSerialized] public Personnel CreatPersonnel;
    public int CurrentId;
    [System.NonSerialized] List<Personnel> PersonnelList;
    string InputKey;
    int Value = 0;
    void Start()
    {
        Instance = this;
        CloseBut.onClick.AddListener(() =>
        {
            CloseCurrentWindow();
        });
        DepartBut.onClick.AddListener(ShowDepartmentInfo);
        JobBut.onClick.AddListener(ShowJobInfo);
        EnsureBut.onClick.AddListener(() =>
        {
            GetModifyPersonnelData();
        });
        CancelBut.onClick.AddListener(() =>
       {
           CloseCurrentWindow();
       });
        TagBut.onClick.AddListener(() =>
        {
            ShowTagCardWindow();
        });
        ToggleAuthoritySet();
    }
    /// <summary>
    /// 编辑Tag
    /// </summary>
    public void ShowTagCardWindow()
    {
        EditTagInfo.Instance.GetTagData(CreatPersonnel, PersonnelList);
        EditTagInfo.Instance.ShowEditTagInfoWindow();
        EditTagInfo.Instance.cardRoleDropdown.GetCardRoleData();
    }

    /// <summary>
    /// 得到人员的详细信息
    /// </summary>
    /// <param name="id"></param>
    public void GetPersonnelInformation(int id, List<Personnel> PerList,string inputKey,int Level)
    {
        InputKey = inputKey;
        Value = Level;
        PersonnelList = new List<Personnel>();
        PersonnelList.AddRange(PerList);
        PerInfo = CommunicationObject.Instance.GetPerson(id);
        personnelSex.AddName();
        ShowPersonnelInfo(PerInfo);
    }
    /// <summary>
    /// 获取部门列表信息
    /// </summary>
    public void GetJobInfo()
    {

        JobManagements.GetJobsManagementData();

    }
    /// <summary>
    /// 展示人员信息
    /// </summary>
    /// <param name="perInfo"></param>
    public void ShowPersonnelInfo(Personnel perInfo)
    {
        CreatPersonnel = perInfo;

        SerialNum.text = "<color=#60D4E4FF>Id：</color>" + PerInfo.Id.ToString();
        Name.text = perInfo.Name.ToString();
        if (perInfo.Tag == null)
        {
            TagName.text = "-尚未选择-";
        }
        else
        {
            TagName.text = perInfo.Tag.Name;
        }
        WorkNum.text = perInfo.WorkNumber.ToString();
        if (string.IsNullOrEmpty(perInfo.PhoneNumber))
        {
            Phone.text = "";
        }
        else
        {
            Phone.text = perInfo.PhoneNumber.ToString();
        }

        UseTog.isOn = perInfo.Enabled;
        personnelSex.PerSexDropdownItem.value = perInfo.TargetType;
        personnelSex.PerSexDropdownItem.captionText.text = personnelSex.tempNames[perInfo.TargetType];
        if (perInfo.Parent != null)
        {
            departmentText.text = perInfo.Parent.Name.ToString();
            CurrentId = (int)perInfo.ParentId;

        }

        for (int i = 0; i < JobManagements.JobsList.Count; i++)
        {
            if (JobManagements.JobsList[i].Name == perInfo.Pst)
            {
                JobManagements.JobsDropdownItem.captionText.text = perInfo.Pst;
                JobManagements.JobsDropdownItem.transform.GetComponent<Dropdown>().value = i;
            }
        }
        DateTime BirthTime = Convert.ToDateTime(perInfo.BirthDay);
        string newBirthTime = BirthTime.ToString("yyyy年MM月dd日");
        BornTime.text = newBirthTime;
    }
    public void ShowAndCloseEditPersonnelInfo(bool b)
    {

        EditPerWindow.SetActive(b);
        if (b == false)
        {
            EditTagInfo.Instance.ChooseTag = null;
            DataPaging.Instance.ShowpersonnelSearchWindow();
        }
    }
    /// <summary>
    /// 打开编辑人员详情窗口
    /// </summary>
    public void ShowEditPersonnelWindow()
    {
        EditPerWindow.SetActive(true);
    }
    /// <summary>
    /// 关闭人员详情信息窗口
    /// </summary>
    public void CloseEditPersonnelWindow()
    {
        EditPerWindow.SetActive(false);
    }
    /// <summary>
    /// 关闭当前编辑人员详情窗口，并且打开上一层窗口
    /// </summary>
    public void CloseCurrentWindow()
    {
        //   ShowAndCloseEditPersonnelInfo(false);
        CloseEditPersonnelWindow();
        DataPaging.Instance.IsGetPersonData = false;
        DataPaging.Instance.ShowpersonnelSearchWindow();
       
        DataPaging.Instance.StartPerSearchUI();
        DataPaging.Instance.PerSelected.text = InputKey;
        DataPaging.Instance.Level = Value;
        DataPaging.Instance.personnelDropdown.PerDropdown.value = Value;
        DataPaging.Instance.personnelDropdown.PerDropdown.captionText.text = DataPaging.Instance.personnelDropdown.devTyprList[Value];
      //  DataPaging.Instance.
        if (EditDepartmentTreeViewManger.Instance.DepBut.isOn)
        {
            EditDepartmentTreeViewManger.Instance.DepBut.isOn = false;
        }
    }
    public void CloseEditPersonnelUI(bool b)
    {
        EditPerWindow.SetActive(b);
        if (b == false)
        {
            EditTagInfo.Instance.ChooseTag = null;
        }

    }
    /// <summary>
    /// 打开编辑部门信息列表
    /// </summary>
    public void ShowDepartmentInfo()
    {
        AddDepartment.Instance.IsAdd = false;

        DepartmentList.Instance.ShowDepartmentListUI();
        DepartmentList.Instance.GetDepartmentListData();

    }
    public void RefreshEditDepartData()
    {
        DepartmentList.Instance.ShowDepartmentListUI();
        DepartmentList.Instance.GetDepartmentListData();
        PersonnelTreeManage.Instance.departmentDivideTree.GetTopoTree();
    }
    public void RefreshEditJobInfo()
    {
        JobManagements.GetJobsManagementData();
        JobManagements.JobsDropdownItem.value = 0;
        if (PersonSubsystemManage.Instance .SearchToggle .isOn ==false )
        {
            Personnel[] personnels;
            List<Personnel> personnelLists = new List<Personnel>();
            personnels = CommunicationObject.Instance.GetPersonnels(); ;
            personnelLists = new List<Personnel>(personnels);
            JobList.Instance.GetJobListData( personnelLists);
           
        }
        else
        {
            JobList.Instance.GetJobListData( PersonnelList);
        }
        JobList.Instance.ShowJobListWindow();
        CloseEditPersonnelWindow();
    }
    /// <summary>
    /// 打开编辑岗位信息界面并且刷新列表
    /// </summary>
    public void ShowJobInfo()
    {
        JobManagements.GetJobsManagementData();
        JobList.Instance.GetJobListData( PersonnelList);
        JobList.Instance.ShowJobListWindow();
        CloseEditPersonnelWindow();
    }
    /// <summary>
    /// 保存当前修改的人员详情
    /// </summary>
    public void GetModifyPersonnelData()
    {
        if (string.IsNullOrEmpty(WorkNum.text) || string.IsNullOrEmpty(Name.text))
        {
            UGUIMessageBox.Show("人员必填信息不完整，请补充完整在进行提交！", null, null);
        }
        else
        {
            CreatPersonnel.Name = Name.text;
            Personnel Per = PersonnelList.Find(i => i.WorkNumber == WorkNum.text);
            if (Per == null)
            {              
                CreatPersonnel.WorkNumber = WorkNum.text;            
            }
            else
            {
                UGUIMessageBox.Show("该工号已存在！", "确定", "", null, null, null);
            }
        }
        CreatPersonnel.Tag = null;
        if (EditDepartmentTreeViewManger.Instance.DepBut.isOn)
        {
            EditDepartmentTreeViewManger.Instance.DepBut.isOn = false;
        }     
        if (!string.IsNullOrEmpty(Name.text))
        {
           // CreatPersonnel.Name = Name.text;
        }
        if (!string.IsNullOrEmpty(Phone.text))
        {
            CreatPersonnel.PhoneNumber = Phone.text;
        }
       
        CreatPersonnel.TargetType = personnelSex.PerSexDropdownItem.value;
        if (!string.IsNullOrEmpty(TagName.text))
        {
            foreach (var tagT in EditTagInfo.Instance.LocationCardData)
            {
                if (tagT.Name == TagName.text)
                {

                    CreatPersonnel.TagId = tagT.Id;
                }
            }
        }else
        {
            TagName.text = "-尚未选择-";
        }
        DateTime BirthTime = Convert.ToDateTime(BornTime.text);
        CreatPersonnel.BirthDay = BirthTime;

      //  CreatPersonnel.WorkNumber = WorkNum.text;
        if (CurrentId == 0)
        {
            CurrentId = 1;
        }
        CreatPersonnel.ParentId = CurrentId;

        CreatPersonnel.Pst = JobManagements.JobsDropdownItem.captionText.text;
        CreatPersonnel.Enabled = UseTog.isOn;
        SavePersonnelData(CreatPersonnel);
        PersonnelTreeManage.Instance.departmentDivideTree.GetTopoTree();
    }
    public void SavePersonnelData(Personnel PerInfo)
    {
        if (string.IsNullOrEmpty(WorkNum.text) || string.IsNullOrEmpty(Name.text))
        {
            UGUIMessageBox.Show("人员必填信息不完整，请补充完整在进行提交！",
          null, null);
        }
        else
        {
            Personnel per = PersonnelList.Find(i => i.WorkNumber == WorkNum.text);
            if (per == null)
            {
                CreatPersonnel.Name = Name.text;
                CreatPersonnel.WorkNumber = WorkNum.text;             
                bool IsEditPer = CommunicationObject.Instance.EditPerson(PerInfo);
                if (IsEditPer)
                {
                    UGUIMessageBox.Show("人员信息已保存！", "确定", "", null,null ,null);
                }
                else
                {
                    UGUIMessageBox.Show("数据保存失败！", "确定", "", null, null, null);
                }
            }
            else
            {
                if (CreatPersonnel.Id !=per .Id)
                {
                    UGUIMessageBox.Show("该工号已存在！", "确定", "", null, null, null);
                }else
                {
                    bool IsEditPer = CommunicationObject.Instance.EditPerson(PerInfo);
                    if (IsEditPer)
                    {
                        UGUIMessageBox.Show("人员信息已保存！", "确定", "", null, null, null);
                    }
                    else
                    {
                        UGUIMessageBox.Show("数据保存失败！", "确定", "", null, null, null);
                    }
                }
               
            }
        }
    }
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            personnelSex.GetComponent<Dropdown>().interactable = false;
            Name.GetComponent<InputField>().interactable = false;
            WorkNum.GetComponent<InputField>().interactable = false;
            Phone.GetComponent<InputField>().interactable = false;
            //departmentManagement.GetComponent<Dropdown>().interactable = false;
            JobManagements.GetComponent<Dropdown>().interactable = false;
            TagBut.gameObject.SetActive(false);
            EnsureBut.gameObject.SetActive(false);
            CancelBut.gameObject.SetActive(false);
            JobBut.gameObject.SetActive(false);
            DepartBut.gameObject.SetActive(false);
        }
        else
        {
            personnelSex.GetComponent<Dropdown>().interactable = true;
            Name.GetComponent<InputField>().interactable = true;
            WorkNum.GetComponent<InputField>().interactable = true;
            Phone.GetComponent<InputField>().interactable = true;
            // departmentManagement.GetComponent<Dropdown>().interactable = true;
            JobManagements.GetComponent<Dropdown>().interactable = true;
            TagBut.gameObject.SetActive(true);
            EnsureBut.gameObject.SetActive(true);
            CancelBut.gameObject.SetActive(true);
            JobBut.gameObject.SetActive(true);
            DepartBut.gameObject.SetActive(true);
        }
    }
    void Update()
    {

    }
}
