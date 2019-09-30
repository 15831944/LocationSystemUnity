using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPosCard : MonoBehaviour
{
    public static AddPosCard Instance;
    public AddPosCardDropdown addPosCardDropdown;
    public GameObject AddPosCardWindow;
    public InputField Name;
    public InputField Num;
    public Text Instruction;
    public Button EnsureBut;
    public Button CancelBut;
    public Button CloseBut;
    public Button CreatBut;

    [System.NonSerialized]
    private Tag AddTag;
    [System.NonSerialized]
    private List<Tag> TagList;

    void Start()
    {
        Instance = this;
        CloseBut.onClick.AddListener(() =>
        {
            CloseCurrentWindow();
        });
        EnsureBut.onClick.AddListener(() =>
        {
            GetAddPosCardData();   
        });
        CancelBut.onClick.AddListener(() =>
        {
            CloseCurrentWindow();
           
        });
        CreatBut.onClick.AddListener(ShowRoleCardEditInfo);
    }
    public void CloseCurrentWindow()
    {
        CloseAddPosCardWindow();
        LocationCardManagement.Instance.ShowCardRoleInfo();
        LocationCardManagement.Instance.ShowAndCloseLocationCardManagementWindow(true );
    }
    public void ShowRoleCardEditInfo()
    {
        Debug.LogError("编辑定位权限");
        CloseAddPosCardWindow();
        AddPerCardPermission.Instance.ShowRoleWindow();
        AddPerCardPermission.Instance.GetCardRoleData(TagList);
        //AddRolePermissionsTreeView.Instance.GetAreaData(AddPerCardPermission.Instance.PreviousID);
        AddPerCardPermission.Instance.grid.gameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
    }
    public void ShowInfoUI()
    {
        addPosCardDropdown.GetCardRoleData();
        Instruction.text = addPosCardDropdown.CardRoleList[0].Description;
        Name.text = "";
        Num.text = "";
    }
    public void RolePowerInstruction(int vale)
    {
        CardRole Role = addPosCardDropdown.CardRoleList.Find(i => i.Name == addPosCardDropdown.CardRoleDropdownItem.captionText.text);
        if (Role != null)
        {
            Instruction.text = Role.Description;
        }
    }
    public void GetAddPosCardData()
    {
        AddTag = new Tag();

        foreach (var per in addPosCardDropdown.CardRoleList)
        {
            if (per.Name == addPosCardDropdown.CardRoleDropdownItem.captionText.text)
            {
                AddTag.CardRoleId = per.Id;
            }
        }
        AddTag.Describe = Instruction.text;
        if (String.IsNullOrEmpty(Name.text) || String.IsNullOrEmpty(Num.text))
        {
            UGUIMessageBox.Show("定位卡必填信息不完整，请补充完整再进行提交！", "确定", null, null, null, null);
        }
        else
        {
            Tag CardRoleName = TagList.Find(i => i.Name  == Name.text);
            Tag CardRoleNum = TagList.Find(i => i.Code == Num.text);
            if (CardRoleName == null&& CardRoleNum==null )
            {
                AddTag.Code = Num.text;
                AddTag.Name = Name.text;
                SaverCardRoleData(AddTag);
             
            }
            else
            {
                if (CardRoleName!=null)
                {
                    UGUIMessageBox.Show("终端名称已存在,请重新填写！", "确定", null, null, null, null);
                }
               else if (CardRoleNum!=null)
                {
                    UGUIMessageBox.Show("终端编号已存在,请重新填写！", "确定", null, null, null, null);
                }
            }
        }
    }
    public void SaverCardRoleData(Tag data)
    {
        int TagID = CommunicationObject.Instance.AddTag(data);
        AddTag.Id = TagID;
        UGUIMessageBox.Show("定位卡信息已保存！", "确定", "", ()=> 
        {
            CloseAddPosCardWindow();
            LocationCardManagement.Instance.LocationCardData.Insert(0, data);
            LocationCardManagement.Instance.ScreenList.Insert(0, data);
            LocationCardManagement.Instance.LocationRole.text = "";
            LocationCardManagement.Instance.ShowCardRoleInfo();
            LocationCardManagement.Instance.ShowAndCloseLocationCardManagementWindow(true);
     


        }, null, null);
    }
    public void ShowAddPosCardWindow(List<Tag> TagT)
    {
        TagList = new List<Tag>();
        TagList.AddRange(TagT);
        ShowInfoUI();
        AddPosCardWindow.SetActive(true);
    }
    public void ShowAddPosCardWindow()
    {
        ShowInfoUI();
        AddPosCardWindow.SetActive(true);
    }
    public void CloseAddPosCardWindow()
    {
        AddPosCardWindow.SetActive(false);

    }


    void Update()
    {

    }
}
