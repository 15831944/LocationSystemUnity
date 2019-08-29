using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCardRoleItem : MonoBehaviour
{

    public Text Name;
    public Button detailBut;
    public Button CancelBut;
    public Toggle Tog;
    public Button SureBut;

    public Button CloseBut;
    int CurrentId;
    void Start()
    {
        CloseBut. onClick.RemoveAllListeners();
        CloseBut.onClick.AddListener(() =>
        {
            if (AddRolePermissionsTreeView.Instance.IsAddSure)
            {
                UGUIMessageBox.Show("角色权限信息有更改尚未保存，确定放弃保存直接退出吗?", "保存并退出", "直接退出",
                 () =>
                 {
                     SaveAreaPermissionData();
                 }, 
                 () =>
                        {
                            AddPosCard.Instance.ShowAddPosCardWindow();
                            AddPerCardPermission.Instance.CloseRoleWindow();
                        },
                 () =>
                        {
                            AddPosCard.Instance.ShowAddPosCardWindow();
                            AddPerCardPermission.Instance.CloseRoleWindow();
                        });

            }
            else
            {
                AddPosCard.Instance.ShowAddPosCardWindow();
                AddPerCardPermission.Instance.CloseRoleWindow();
            }
        });
        SureBut. onClick.RemoveAllListeners();
        SureBut.onClick.AddListener(() =>
        {
            SaveAreaPermissionData_Click();
        });
    }
    public void SaveAreaPermissionData_Click()
    {
        IsScreen = CommunicationObject.Instance.SetCardRoleAccessAreas(AddPerCardPermission.Instance.PreviousID, AddRolePermissionsTreeView.Instance.RolePermissionList);
        if (IsScreen)
        {
            UGUIMessageBox.Show("角色权限信息已保存！", "确定", "",
        () =>
        {
            Debug.LogError("保存刷新保存人员区域");

        }, null, null);
        }
        else
        {
            UGUIMessageBox.Show("数据保存失败！", "确定", "", null, null, null);
        }
        AddRolePermissionsTreeView.Instance.IsAddSure = false;
        AddPerCardPermission.Instance.PreviousID = CurrentId;
    }
    public void ShowCardRoleInfo(CardRole info, List<Tag> TagInfo)
    {
        CurrentId = info.Id;
        Name.text = info.Name;
        Tog.onValueChanged.AddListener(ShowTreeInfo);
        detailBut.onClick.AddListener(() =>
        {

            AddLocationPerEditInfo.Instance.ShowAddLocationPer();
            AddLocationPerEditInfo.Instance.GetAddLocationPerInfo(info);
        });
        CancelBut.onClick.AddListener(() =>
        {
            Tag perTag = TagInfo.Find((item) => item.CardRoleId == info.Id);
            if (perTag==null)
            {
                DeletRoleData(info.Id);
            }
        });
    }
    
    public void ShowTreeInfo(bool b)
    {
        if (b)
        {
            if (AddRolePermissionsTreeView.Instance.IsAddSure)
            {

                UGUIMessageBox.Show("角色权限信息有更改尚未保存，切换角色前是否保存？", "保存", "不保存",
              () =>
              {
                  SaveAreaPermissionCurrentData();
              }, () =>
              {
                  AddRolePermissionsTreeView.Instance.GetAreaData(CurrentId);
                  AddRolePermissionsTreeView.Instance.GetRolePermissionsTree();                
                  AddPerCardPermission.Instance.PreviousID=CurrentId ;
              }, null);
            }
            else
            {
                AddRolePermissionsTreeView.Instance.GetAreaData(CurrentId);
                AddRolePermissionsTreeView.Instance.GetRolePermissionsTree();     
                AddPerCardPermission.Instance.PreviousID=CurrentId ;
            }
        }

    }
    bool IsScreen;
    public void SaveAreaPermissionCurrentData( Action action = null)
    {

        IsScreen = CommunicationObject.Instance.SetCardRoleAccessAreas(AddPerCardPermission.Instance.PreviousID, AddRolePermissionsTreeView.Instance.RolePermissionList);
        if (IsScreen)
        {
            AddRolePermissionsTreeView.Instance.GetAreaData(CurrentId);
            AddRolePermissionsTreeView.Instance.GetRolePermissionsTree();            
            AddRolePermissionsTreeView.Instance.IsAddSure = false;

        }
        else
        {
            UGUIMessageBox.Show("数据保存失败！", "确定", "", ()=>
            {
                AddRolePermissionsTreeView.Instance.GetAreaData(CurrentId);
                AddRolePermissionsTreeView.Instance.GetRolePermissionsTree();
            }, null, null);
        }
        AddPerCardPermission.Instance.PreviousID = CurrentId;
        if (action != null) action();
    }
    public void SaveAreaPermissionData( Action action = null)
    {

        IsScreen = CommunicationObject.Instance.SetCardRoleAccessAreas(AddPerCardPermission.Instance.PreviousID, AddRolePermissionsTreeView.Instance.RolePermissionList);
        if (IsScreen)
        {
            AddPosCard.Instance.ShowAddPosCardWindow();
            AddPerCardPermission.Instance.CloseRoleWindow();

        }
        else
        {
            UGUIMessageBox.Show("数据保存失败！", "确定", "", () =>
            {
                AddRolePermissionsTreeView.Instance.GetAreaData(CurrentId); ;
                AddRolePermissionsTreeView.Instance.GetRolePermissionsTree();
               
                AddPerCardPermission.Instance.PreviousID = CurrentId;
            }, null, null);

        }
      
        if (action != null) action();
    }
    public void DeletRoleData(int id)
    {
        
        bool IsDelet = CommunicationObject.Instance.DeleteCardRole(id);
        if (IsDelet)
        {
            UGUIMessageBox.Show("删除角色成功！", "确定", "", () =>
            {
                AddPerCardPermission.Instance.SaveSelection();
                AddPerCardPermission.Instance.GetCardRoleData();
                AddPerCardPermission.Instance.grid.gameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            }
, null, null);
        }
    }
}
