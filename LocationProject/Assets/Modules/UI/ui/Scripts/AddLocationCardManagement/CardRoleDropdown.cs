using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRoleDropdown : MonoBehaviour {
    public static CardRoleDropdown Instance;
    public List<CardRole> CardRoleList;
    public Dropdown CardRoleDropdownItem;
    public List<string> CardRoleName;
    void Start () {
        Instance = this;
        CardRoleDropdownItem = GetComponent<Dropdown>();
        CardRoleDropdownItem.onValueChanged.AddListener(LocationCardManagement.Instance .GetLocationCardRoleType);
    }
	
	public void GetCardRoleData()
    {
        CardRoleList = new List<CardRole>();
        CardRoleName = new List<string>();
        CardRoleName.Add("全部人员");
        CardRoleList = CommunicationObject.Instance.GetCardRoleList();
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
        CardRoleDropdownItem.captionText.text  = showItem[0] .ToString();
    }

    void Update () {
		
	}
}
