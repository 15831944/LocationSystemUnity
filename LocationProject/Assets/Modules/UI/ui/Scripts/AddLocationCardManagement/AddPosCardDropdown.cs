using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPosCardDropdown : MonoBehaviour {

    public List<CardRole> CardRoleList;
    public Dropdown CardRoleDropdownItem;
    void Start()
    {
        CardRoleDropdownItem = GetComponent<Dropdown>();
        CardRoleDropdownItem.onValueChanged.AddListener(AddPosCard.Instance.RolePowerInstruction);
    }

    public void GetCardRoleData()
    {
        CardRoleList = new List<CardRole>();
        CardRoleList = CommunicationObject.Instance.GetCardRoleList();
        SetDropdownData(CardRoleList);
    }
    private void SetDropdownData(List<CardRole> showItem)
    {
        CardRoleDropdownItem.options.Clear();
        Dropdown.OptionData tempData;
        for (int i = 0; i < showItem.Count; i++)
        {
            tempData = new Dropdown.OptionData();
            tempData.text = showItem[i].Name.ToString();
            CardRoleDropdownItem.options.Add(tempData);
        }
        CardRoleDropdownItem.captionText.text = showItem[0].Name.ToString();
    }

}
