using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTagInfoItem : MonoBehaviour
{
    public Toggle Select;
    public Text num;
    public Text Name;
    public Text Role;
    public Text State;

    [System.NonSerialized]
    public List<CardRole> CardRoleList;

    [System.NonSerialized]
    private Tag currentTag;
    public Text tagName;
    string CurrentPerName;

    Color SelectColor = new Color(96 / 255f, 212 / 255f, 228 / 255f, 255 / 255f);
    void Start()
    {

    }

    public void PerTagDissociated()
    {
        bool IsEdit;
        Personnel per = DataPaging.Instance.peraonnelData.Find(item => item.TagId == currentTag.Id);
        if (per != null)
        {
            per.TagId = 0;
            IsEdit = CommunicationObject.Instance.EditPerson(per);
        }
        else
        {
            currentTag.IsActive = true;
            IsEdit = CommunicationObject.Instance.EditTag(currentTag);
        }
        if (IsEdit)
        {
            if (per != null)
            {
                int currentTagId = (int)EditPersonnelInformation.Instance.CreatPersonnel.TagId;
                if (currentTagId == currentTag.Id)
                {
                    EditPersonnelInformation.Instance.TagName.text = "-尚未选择-";
                }
            }
        }
        else
        {
            UGUIMessageBox.Show("解除人员定位卡绑定失败！",
   null, null);
        }

    }



    public void GetCardRole(Tag info, List<CardRole> list, List<Personnel> perList, Tag CurrentTag, Personnel currentPer)
    {
        currentTag = info;
        CardRoleList = new List<CardRole>();
        CardRoleList.AddRange(list);
        num.text = info.Code;
        Name.text = info.Name;
        tagName.text = info.Id .ToString();
        CurrentPerName = currentPer.Name;

        Personnel per = perList.Find(i => i.Id == info.PersonId);
        if (per != null)
        {
            if (CurrentTag != null && CurrentTag.Id == info.Id)
            {
                Select.interactable = true;
                Select.isOn = true;
                State.color = SelectColor;
            }
            else
            {
                Select.interactable = false;
                Select.isOn = false;
                State.color = Color.white;
            }
            State.text = per.Name;

        }
        else
        {
            State.text = "--";
        }
        foreach (var role in CardRoleList)
        {
            if (info.CardRoleId == 0)
            {
                Role.text = "--";
            }
            else
            {
                if (info.CardRoleId == role.Id)
                {
                    Role.text = role.Name;
                }
            }
        }
        Select.onValueChanged.AddListener(SelectTag_Click);
        //  Dissociated.onClick.AddListener(PerTagDissociated);
    }
    public void SelectTag_Click(bool b)
    {
        EditTagInfo.Instance.TogGroup.allowSwitchOff = b;
        if (b)
        {
            State.text = CurrentPerName;
            State.color = SelectColor;
        }
        else
        {
            State.color = Color.white;
            State.text = "--";
        }

    }
}


