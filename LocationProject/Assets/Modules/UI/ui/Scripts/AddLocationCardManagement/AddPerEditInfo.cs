using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPerEditInfo : MonoBehaviour {
    public static AddPerEditInfo Instance;
    CardRole AddCardRole;
    public GameObject AddRoleWindow;
    public Button CloseAddRole;
    public InputField InputName;
    public InputField InputDetail;
    public Button EnsureBut;
    public Button CancelBut;
    // Use this for initialization
    void Start () {
        Instance = this;
        CloseAddRole.onClick.AddListener(() => 
        {
            CloseAddRoleWindow();
        });
        EnsureBut.onClick.AddListener(() =>
        {
            AddRoleInfo(() =>
            {
                
            });
        });
        CancelBut.onClick.AddListener(() =>
        {
            CloseAddRoleWindow();
        });
    }
    public void AddRoleInfo(Action action = null)
    {
        
        AddCardRole = new CardRole();
        
        if (string .IsNullOrEmpty(InputDetail.text)||string .IsNullOrEmpty (InputName.text))
        {
            UGUIMessageBox.Show("角色缺少必填信息，请补充完整在进行提交！", "确定", "", null, null, null);
        }
        else
        {
            AddCardRole.Name = InputName.text;
            AddCardRole.Description = InputDetail.text;
            SaveAddReloInfo(AddCardRole);
            if (action != null) action();
        }
       
    }
    public void ClearData()
    {
        InputName.text = "";
        InputDetail.text = "";
    }
    public void ShowAddRoleWindow()
    {
       
        AddRoleWindow.SetActive(true);
        RoleNameList.Instance.CloseRoleWindow();
    }
    public void CloseAddRoleWindow()
    {
        ClearData();
        AddRoleWindow.SetActive(false);
        RoleNameList.Instance.ShowRoleWindow();
        RoleNameList.Instance . GetCardRoleData(()=> 
        {
            //RoleNameList.Instance.grid.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = true;
            RoleNameList.Instance.grid.gameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
        });
        
    }

    public void SaveAddReloInfo(CardRole info)
    {
       int CardRoleID= CommunicationObject.Instance.AddCardRole(info);
        info.Id = CardRoleID;
        UGUIMessageBox.Show("角色信息已保存！", "确定", "", ()=> 
        {
            CloseAddRoleWindow();
        }, null, null);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
