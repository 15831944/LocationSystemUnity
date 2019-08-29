using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddLocationPerEditInfo : MonoBehaviour
{
    public static AddLocationPerEditInfo Instance;
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
        CloseBut.onClick.RemoveAllListeners();
        CloseBut.onClick.AddListener(() =>
        {
            CloseEditRoleWindow();
        });
        SaveBut.onClick.RemoveAllListeners();
        SaveBut.onClick.AddListener(() =>
        {
            SaverCardRoleData();
        });
        CancelBut.onClick.RemoveAllListeners();
        CancelBut.onClick.AddListener(() =>
        {
            CloseEditRoleWindow();
        });
    }
    public void ShowAddLocationPer()
    {
        EditRoleWindow.SetActive(true);

        AddPerCardPermission.Instance.CloseCurrentWindow();
    }
    public void GetAddLocationPerInfo(CardRole info)
    {
        EditCardRole = new CardRole();
        EditCardRole = info;
        InputName.text = info.Name;
        InputDetail.text = info.Description;
    }
    public void SaverCardRoleData(Action action = null)
    {


        if (string.IsNullOrEmpty(InputDetail.text) || string.IsNullOrEmpty(InputName.text))
        {
            UGUIMessageBox.Show("角色缺少必填信息，请补充完整再进行提交！", "确定", "", null, null, null);
        }
        else
        {
            EditCardRole.Name = InputName.text;
            EditCardRole.Description = InputDetail.text;
            bool IsSuccessful = CommunicationObject.Instance.EditCardRole(EditCardRole);
            if (IsSuccessful)
            {
                UGUIMessageBox.Show("新建角色成功！", "确定", "", () =>
                {
                    CloseEditRoleWindow();
                }, null, null);
            }
            else
            {
                UGUIMessageBox.Show("数据保存失败！", "确定", "", null, null, null);
            }
        }

        if (action != null) action();
    }
    public void CloseEditRoleWindow()
    {
        RemoveInfo();
        EditRoleWindow.SetActive(false);
        AddPerCardPermission.Instance.ShowRoleWindow();
        AddPerCardPermission.Instance.GetCardRoleData();

        AddPerCardPermission.Instance.grid.gameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
    }
    public void CloseCurrentWindow()
    {
        RemoveInfo();
        EditRoleWindow.SetActive(false);
    }
    public void RemoveInfo()
    {
       InputName.text="";
         InputDetail.text = "";
    }
    // Update is called once per frame
    void Update()
    {

    }
}
