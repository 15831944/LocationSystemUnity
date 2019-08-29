using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTagCardRoleDropdown : MonoBehaviour {
    public List<CardRole> CardRoleList;
    public List<string> CardRoleName;
    public Dropdown CardRoleDropdownItem;
    CardRole firstCardRole;
    void Start()
    {
        CardRoleDropdownItem = GetComponent<Dropdown>();
        CardRoleDropdownItem.onValueChanged.AddListener(EditTagInfo.Instance.GetLocationCardRoleType);
    }

    public void GetCardRoleData()
    {
        CardRoleList = new List<CardRole>();

        CardRoleName = new List<string>();
        CardRoleName.Add("全部人员");

         CardRoleList =  CommunicationObject.Instance.GetCardRoleList();
        foreach (var item in CardRoleList)
        {
            CardRoleName.Add(item.Name);
        }
        SetDropdownData(CardRoleName);
    }
    private void SetDropdownData(List<string> showItem)
    {
        CardRoleDropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showItem.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showItem[i].ToString();
            CardRoleDropdownItem.options.Add(tempData);
        }
        CardRoleDropdownItem.captionText.text = showItem[0];
    }

}
