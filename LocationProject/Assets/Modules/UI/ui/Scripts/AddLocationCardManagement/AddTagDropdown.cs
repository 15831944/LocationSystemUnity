using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddTagDropdown : MonoBehaviour {

    public List<CardRole> TagsList;
    public Dropdown AddTagDropdownItem;
    public List<string> CardRoleName;
     void Start()
    {
        AddTagDropdownItem = GetComponent<Dropdown>();
        AddTagDropdownItem.onValueChanged.AddListener(AddEditCardRoleInfo.Instance.GetLocationCardRoleType);
      


    }
    public void GetTagData()
    {

        TagsList = new List<CardRole>();
        CardRoleName = new List<string>();
        CardRoleName.Add("全部人员");
        TagsList = CommunicationObject.Instance.GetCardRoleList();
        foreach (var item in TagsList)
        {
            CardRoleName.Add(item.Name);
        }
        SetDropdownData(CardRoleName);
       

    }
    private void SetDropdownData(List<string> showItem)
    {
        AddTagDropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showItem.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showItem[i].ToString();
            AddTagDropdownItem.options.Add(tempData);
        }
        AddTagDropdownItem.captionText.text  = showItem[0] .ToString();
    }
}
