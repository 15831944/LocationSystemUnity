using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationCardItem : MonoBehaviour
{

    public Text NumText;
    public Text NameText;
    public Text RoleText;
    public Text electricityText;
    public Text State;
    public Text PerName;
    public Button detailBut;
    public Button DeleteBut;

    void Start()
    {

    }
    int CardId;
    [System.NonSerialized]
    Personnel per;
    public void ShowCardRole(Tag info, List<CardRole> CardRoleList, List<Personnel> peraonnelData, List<Tag> TagInfo)
    {
        NumText.text = info.Code.ToString();
        if (string.IsNullOrEmpty(info.Name))
        {
            NameText.text = "";
        }
        else
        {
            NameText.text = info.Name;
        }


         per = peraonnelData.Find((item) => item.Id == info.PersonId);
        if (per != null)
        {
            PerName.text = per.Name;
        }
        else
        {
            PerName.text = "--";
        }
        if (info.CardRoleId == 0)
        {
            RoleText.text = "--";
        }
        else
        {
            CardRole perRole = CardRoleList.Find((item) => item.Id == info.CardRoleId);
            if (perRole != null)
            {
                CardId = info.CardRoleId;
                RoleText.text = perRole.Name;
            }
        }
        electricityText.text = info.Power.ToString();
        State.text = info.PowerState.ToString();
        detailBut.onClick.AddListener(() =>
       {
           ShowEditCardRoleInfo(info, TagInfo);
       });
        DeleteBut.onClick.AddListener(() =>
      {
          if (per != null)
          {
              UGUIMessageBox.Show("删除该定位卡，会使与之绑定的人员不再持有定位卡且无法定位。仍要删除吗？", "继续删除", "取消操作",
                  () =>
                  {
                      DeleteButLocationCard(info.Id,()=> 
                      {
                          if (IsDelet)
                          {
                              UGUIMessageBox.Show("删除定位卡成功", "确定", "",  null, null,null );
                              LocationCardManagement.Instance.GetLocationCardManagementData();
                              LocationCardManagement.Instance.ShowLocationCardManagementWindow();
                          }
                          else
                          {
                              UGUIMessageBox.Show("删除定位卡信息失败！", "确定", "",null,null ,null);
                          }
                      });
                  },null,null);
          } 
          else
          {
              DeleteButLocationCard(info.Id,()=> {
                  if (IsDelet)
                  {
                      UGUIMessageBox.Show("删除定位卡成功", "确定", "", null, null, null);
                      LocationCardManagement.Instance.GetLocationCardManagementData();
                      LocationCardManagement.Instance.ShowLocationCardManagementWindow();
                  }
                 else
                  {
                      UGUIMessageBox.Show("删除定位卡信息失败！", "确定", "", null, null, null);
                  }

              });
          }   
      });
    }
    /// <summary>
    /// 打开编辑角色定位卡
    /// </summary>
    /// <param name="info"></param>
    public void ShowEditCardRoleInfo(Tag info, List<Tag> TagInfo)
    {
        PosCardEditingInfo.Instance.ShowEditCardInfo(info, TagInfo);
        PosCardEditingInfo.Instance.ShowPosCardEditWindow();
        LocationCardManagement.Instance.CloseLocationCardWindow();
    }
     bool IsDelet;
    public void DeleteButLocationCard(int id, Action action = null)
    {
       IsDelet = CommunicationObject.Instance.DeleteTag(id);
        if (action != null) action();

    }
    void Update()
    {

    }
}
