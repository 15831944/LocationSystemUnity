using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddLocationRoleEdit : MonoBehaviour {
    public static AddLocationRoleEdit Instance;
    public InputField InputName;
    public InputField InputDetail;
    CardRole EditCardRole;
    public Button SaveBut;
    public Button CancelBut;
    public Text IdText;
    public Button CloseBut;
    public GameObject EditRoleWindow;
    void Start()
    {
        Instance = this;
        CloseBut.onClick.AddListener(() =>
        {
            CloseEditRoleWindow();
        });
        SaveBut.onClick.AddListener(() =>
        {
            SaverCardRoleData();

        });
        CancelBut.onClick.AddListener(() =>
        {
            CloseEditRoleWindow();
        });
    }
   
    public void SaverCardRoleData(Action action = null)
    {
        EditCardRole = new CardRole();
        
        if (string .IsNullOrEmpty(InputDetail.text)||string .IsNullOrEmpty (InputName.text))
        {
            UGUIMessageBox.Show("角色缺少必填信息，请补充完整再进行提交！", "确定", "",null, null, null);

        }
        else
        {
            EditCardRole.Name = InputName.text;
            EditCardRole.Description = InputDetail.text;
            int CardRoleId = CommunicationObject.Instance.AddCardRole(EditCardRole);
            EditCardRole.Id = CardRoleId;
            UGUIMessageBox.Show("新建角色成功！", "确定", "", ()=>
            {
                CloseEditRoleWindow();
            }, null, null);        
        }
    
        if (action != null) action();
    }
    public void DeleteInfo()
    {
        InputName.text = "";
        InputDetail.text = "";
    }
    public void ShowEditRoleWindow()
    {
        DeleteInfo();
        EditRoleWindow.SetActive(true );
        AddPerCardPermission.Instance.CloseRoleWindow();
    }
    public void CloseEditRoleWindow()
    {

        EditRoleWindow.SetActive(false);
        AddPerCardPermission.Instance.ShowRoleWindow();
        AddPerCardPermission.Instance.GetCardRoleData();        
        
        AddPerCardPermission.Instance.grid.gameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
    }
    public void CloseCurrentWindow()
    {
        EditRoleWindow.SetActive(false);
    }
}
