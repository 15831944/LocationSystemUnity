using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PosCardEditingInfo : MonoBehaviour
{
    public static PosCardEditingInfo Instance;
    public GameObject PosCardEditWindow;
    public Text IdText;
    public InputField Name;
    public InputField Num;
    public Text Instruction;
    public PosCardEditDropdpwn posCardEditDropdpwn;
    public Button SaveBut;
    public Button CancelBut;
    public Button RoleEdit;
    [System.NonSerialized]
    Tag EditTag;
    public Button CloseBut;
    public Sprite Row_Selected;
    [System.NonSerialized]
    List<Tag> PosCardRole;
    GridLayoutGroup Grid;
    GameObject Obj;
    void Start()
    {
        Instance = this;
        CloseBut.onClick.AddListener(() =>
        {
            CloseCurrentAndShowLastUI();
        });
        ToggleAuthoritySet();
        SaveBut.onClick.AddListener(() =>
        {
            GetModifyCardRole();

        });
        CancelBut.onClick.AddListener(() =>
        {
            ClosePosCardEditWindow();
            LocationCardManagement.Instance.ShowAndCloseLocationCardManagementWindow(true );
        //    LocationCardManagement.Instance.GetLocationCardManagementData();

        });
        RoleEdit.onClick.AddListener(ShowRoleEditInfo);
    }
    public void CloseCurrentAndShowLastUI()
    {
        ClosePosCardEditWindow();
        LocationCardManagement.Instance.ShowAndCloseLocationCardManagementWindow(true );
     //   LocationCardManagement.Instance.GetLocationCardManagementData();
    }
    public void EditPosCardInfo()
    {
        int k = Obj.transform.GetSiblingIndex();
        Transform line = Grid.transform.GetChild(k);
        line.GetChild(0).GetComponent<Text>().text = Num.text;
        line.GetChild(1).GetComponent<Text>().text = Name .text;
        line.GetChild(3).GetComponent<Text>().text = posCardEditDropdpwn.CardRoleDropdownItem.captionText.text;
    }

    public void OpenEditCardAndDataNull()
    {
        EditTag.Name = Name.text;
        foreach (var per in posCardEditDropdpwn.CardRoleList)
        {
            if (per.Name == posCardEditDropdpwn.CardRoleDropdownItem.captionText.text)
            {
                EditTag.CardRoleId = per.Id;
            }
        }
        EditTag.Describe = Instruction.text;
    }
    int RoleId;
    public void ShowEditCardInfo(Tag info, List<Tag> TagInfo, GridLayoutGroup grid,GameObject obj)
    {
        posCardEditDropdpwn.GetCardRoleData();
        Grid = grid;
        Obj = obj;
        PosCardRole = new List<Tag>();
        PosCardRole.AddRange(TagInfo);
        EditTag = new Tag();
        EditTag = info;
        posCardEditDropdpwn.GetCardRoleData();
        IdText.text = "<color=#60D4E4FF>Id：</color>" + info.Id.ToString();

        Name.text = info.Name;
        Num.text = info.Code;
        for (int i = 0; i < posCardEditDropdpwn.CardRoleList.Count; i++)
        {
            if (info.CardRoleId == posCardEditDropdpwn.CardRoleList[i].Id)
            {
                RoleId = info.CardRoleId;
                posCardEditDropdpwn.CardRoleDropdownItem.value = i;
                posCardEditDropdpwn.CardRoleDropdownItem.captionText.text = posCardEditDropdpwn.CardRoleList[i].Name;
                Instruction.text = posCardEditDropdpwn.CardRoleList[i].Description;
            }
        }
    
        

    }
    public void GetModifyCardRole()
    {
        CardRole cardRole = posCardEditDropdpwn.CardRoleList.Find((item) => item.Name == posCardEditDropdpwn.CardRoleDropdownItem.captionText.text);
        if (cardRole != null)
        {
            EditTag.CardRoleId = cardRole.Id;
        }
        EditTag.Describe = Instruction.text;
        if (string.IsNullOrEmpty(Name.text))
        {
            UGUIMessageBox.Show("定位卡必填信息不完整，请补充完整再进行提交！", "确定", null, null, null, null);
        }
        else
        {
            if (string.IsNullOrEmpty(Num.text))
            {
                UGUIMessageBox.Show("定位卡必填信息不完整，请补充完整再进行提交！", "确定", null, null, null, null);
            }
            else
            {
                EditTag.Name = Name.text;
                Tag CardRoleP = PosCardRole.Find(i => i.Code == Num.text);
                if (CardRoleP == null)
                {

                    EditTag.Code = Num.text;
                    SaverCardRoleData(EditTag);
                }
                else
                {
                    if (EditTag.Id == CardRoleP.Id)
                    {
                        EditTag.Code = Num.text;
                        SaverCardRoleData(EditTag);
                    }
                    else
                    {
                        UGUIMessageBox.Show("终端名称已存在,请重新填写！", "确定", null, null, null, null);
                    }

                }
            }
        }


    }
    public void ShowRoleEditInfo()
    {
        PosCardEditWindow.SetActive(false);
        RoleNameList.Instance.ShowRoleWindow();
        RoleNameList.Instance.GetCardRoleData(PosCardRole, () =>
        {
            RoleNameList.Instance.grid.gameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            RolePermissionsTreeView.Instance.GetRolePermissionsTree();
            RolePermissionsTreeView.Instance.GetAreaData(RoleNameList.Instance.PreviousID);
            // RoleNameList.Instance.grid.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;
        });


    }

    public void ShowPosCardEditWindow()
    {
        
        PosCardEditWindow.SetActive(true);
    }
    public void ClosePosCardEditWindow()
    {
        PosCardEditWindow.SetActive(false);      

    }
    public void RolePowerInstruction(int vale)
    {
        CardRole Role = posCardEditDropdpwn.CardRoleList.Find(i => i.Name == posCardEditDropdpwn.CardRoleDropdownItem.captionText.text);
        if (Role != null)
        {
            Instruction.text = Role.Description;
        }

    }
    public void SaverCardRoleData(Tag data, Action action = null)
    {
        bool IsSuccessful = CommunicationObject.Instance.EditTag(data);
        if (IsSuccessful)
        {
            UGUIMessageBox.Show("定位卡信息已保存！", "确定", null,
     () =>
     {
         EditPosCardInfo();
         //LocationCardManagement.Instance.ShowLocationCardManagementWindow();
         //LocationCardManagement.Instance.GetLocationCardManagementData();
         //ClosePosCardEditWindow();
     }
     , null, null);
        }
        else
        {
            UGUIMessageBox.Show("数据保存失败！", "确定", null, null, null, null);
        }

        if (action != null) action();
    }
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            Name.GetComponent<InputField>().interactable = false;
            posCardEditDropdpwn.transform.GetComponent<Dropdown>().interactable = false;
            SaveBut.gameObject.SetActive(false);
            CancelBut.gameObject.SetActive(false);
            RoleEdit.gameObject.SetActive(false);
        }
        else
        {
            Name.GetComponent<InputField>().interactable = true;
            posCardEditDropdpwn.transform.GetComponent<Dropdown>().interactable = true;
            SaveBut.gameObject.SetActive(true);
            CancelBut.gameObject.SetActive(true);
            RoleEdit.gameObject.SetActive(true);
        }
    }
}
