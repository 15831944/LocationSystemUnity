using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRoleItem : MonoBehaviour
{

    public Text Name;
    public Button detailBut;
    public Button CloseBut;
    public Toggle Tog;
    public Button CancelBut;
    public Sprite TransperantBack;
    public Button CloseAddRole;
    public Button SaveWindow;
    // public Button CancelAllWindow;

    void Start()
    {
        CloseAddRole.onClick.AddListener(() =>
        {
            if (RolePermissionsTreeView.Instance.IsSure)
            {
                UGUIMessageBox.Show("角色权限信息有更改尚未保存，确定放弃保存直接退出吗?", "保存并退出", "直接退出",
             () =>
               {
                   SaveCurrentData(CurrentId);
               }, () =>
               {
                   CloseCurrentUIAndOpenPreviousUI();
               }, () =>
             {
                 CloseCurrentUIAndOpenPreviousUI();
             });
            }
            else
            {
                CloseCurrentUIAndOpenPreviousUI();
            }

        });
        SaveWindow. onClick.RemoveAllListeners();
        SaveWindow.onClick.AddListener(() =>
        {
            SaveWindow_Click();
        });
    }
    int CurrentId;//当前选中的ID
    public void SaveWindow_Click()
    {
        IsScreen = CommunicationObject.Instance.SetCardRoleAccessAreas(RoleNameList.Instance.PreviousID, RolePermissionsTreeView.Instance.RolePermissionList);
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
        RolePermissionsTreeView.Instance.IsSure = false;
        RoleNameList.Instance.PreviousID = CurrentId;
    }
    public void ShowCardRoleInfo(CardRole info, List<Tag> TagInfo)
    {
        CurrentId = info.Id;
        Name.text = info.Name;
        Tog.onValueChanged.AddListener(ShowTreeInfo);
        detailBut.onClick.AddListener(() =>
        {
            EditPersonnelRoleInfo.Instance.GetEditPersonnelRoleInfo(info);
            EditPersonnelRoleInfo.Instance.ShowEditPersonnelRoleWindow();
            RoleNameList.Instance.CloseRoleWindow();
        });
        CancelBut.onClick.AddListener(() =>
        {
            Tag perTag = TagInfo.Find((item) => item.CardRoleId == info.Id);
            if (perTag == null)
            {
                DeletRoleData(info.Id);
            }
            else
            {
                UGUIMessageBox.Show("该角色已绑定定位卡，请解绑所有相关定位卡后再删除！", "确定", "", null, null, null);
            }
        });
    }

    public void ShowTreeInfo(bool b)
    {
        if (b)
        {
            if (RolePermissionsTreeView.Instance.IsSure)
            {
                UGUIMessageBox.Show("角色权限信息有更改尚未保存，切换角色前是否保存？", "保存", "不保存",
                () =>
                     {
                        SaveAreaPermissionData();
                      },
                () =>
                     {
                   RolePermissionsTreeView.Instance.GetRolePermissionsTree();
                   RolePermissionsTreeView.Instance.GetAreaData(CurrentId);
                   RoleNameList.Instance.PreviousID = CurrentId;
                     }, null);
            }
            else
            {
                RolePermissionsTreeView.Instance.GetRolePermissionsTree();
                RolePermissionsTreeView.Instance.GetAreaData(CurrentId);
                RoleNameList.Instance.PreviousID = CurrentId;
            }
        }

    }
    /// <summary>
    /// 删除角色名称
    /// </summary>
    /// <param name="id"></param>
    public void DeletRoleData(int id)
    {
     bool IsDelet=   CommunicationObject.Instance.DeleteCardRole(id);
        if (IsDelet)
        {
            UGUIMessageBox.Show("删除角色成功！", "确定", "", () =>
            {
                RoleNameList.Instance.ShowRoleWindow();
                EditPersonnelRoleInfo.Instance.CloseEditPersonnelRoleWindow();
                RoleNameList.Instance.GetCardRoleData(() =>
                {
                    RoleNameList.Instance.grid.gameObject.transform.GetChild(0).GetComponent<Toggle>().isOn = true;

                });
            }
, null, null);
        }
    }
    bool IsScreen;
    public void SaveCurrentData(int id)
    {
        IsScreen = CommunicationObject.Instance.SetCardRoleAccessAreas(RoleNameList.Instance.PreviousID, RolePermissionsTreeView.Instance.RolePermissionList);
        if (IsScreen)
        {
            CloseCurrentUIAndOpenPreviousUI();
       
        }
        else
        {
            UGUIMessageBox.Show("数据保存失败！", "确定", "", () =>
            {
               // RolePermissionsTreeView.Instance.GetAreaData(CurrentId);
                RefreshRolePermissionsTree();
            }
, null, null);

        }
        RoleNameList.Instance . PreviousID = CurrentId;
    }
    public void RefreshRolePermissionsTree(Action action = null)
    {
        RolePermissionsTreeView.Instance.GetAreaData(CurrentId);
        RolePermissionsTreeView.Instance.GetRolePermissionsTree();
     
          RoleNameList.Instance.PreviousID = CurrentId;
        Debug.LogError("关闭刷新保存人员区域");
        if (action != null) action();
    }
    public void CloseCurrentUIAndOpenPreviousUI()
    {
        PosCardEditingInfo.Instance.ShowPosCardEditWindow();
        PosCardEditingInfo.Instance.OpenEditCardAndDataNull();
        RoleNameList.Instance.CloseRoleWindow();
    }
    public void SaveAreaPermissionData()
    {
        IsScreen = CommunicationObject.Instance.SetCardRoleAccessAreas(RoleNameList.Instance.PreviousID, RolePermissionsTreeView.Instance.RolePermissionList);
        if (IsScreen)
        {
            RolePermissionsTreeView.Instance.GetAreaData(CurrentId);
            RolePermissionsTreeView.Instance.GetRolePermissionsTree();          
            RolePermissionsTreeView.Instance.IsSure = false;
            Debug.LogError("切换刷新保存人员区域");
        }
        else
        {
            UGUIMessageBox.Show("数据保存失败！", "确定", "", ()=> {
                RolePermissionsTreeView.Instance.GetAreaData(CurrentId);
                RolePermissionsTreeView.Instance.GetRolePermissionsTree();
            }, null, null);
        }
         RoleNameList.Instance.PreviousID = CurrentId;
    }
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            SaveWindow.gameObject.SetActive(false);
            // CancelAllWindow.gameObject.SetActive(false);
        }
        else
        {
            SaveWindow.gameObject.SetActive(true);
            // CancelAllWindow.gameObject.SetActive(true);
        }
    }
    void Update()
    {

    }
}
