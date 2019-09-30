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
    Tag CurrentTag;
    List<Tag> TagList;
    GridLayoutGroup Grid;
    string InputKey="";
    int Value;
    void Start()
    {
        detailBut.onClick.AddListener(() =>
        {
            ShowEditCardRoleInfo(CurrentTag, TagList);
        });
        DeleteBut.onClick.AddListener(() =>
        {
            DeleteBut_Click();
        });
    }
    int CardId;
    [System.NonSerialized]
    Personnel per;
    public void ShowCardRole(Tag info, List<CardRole> CardRoleList, List<Personnel> peraonnelData, List<Tag> TagInfo, GridLayoutGroup grid)
    {
        Grid = grid;
        CurrentTag = new Tag();
        CurrentTag = info;
        TagList = new List<Tag>();
        TagList.AddRange(TagInfo);
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
        electricityText.text = info.Power==0?"0V":GetPowerValue(info.Power).ToString("f2")+"V";
        State.text = info.PowerState==0&&info.Power!=0?"正常":"<color=#ff3030>电量低</color>";
      
     
    }
    string pagePer;
    public void DeleteBut_Click()
    {
        InputKey = LocationCardManagement.Instance . LocationRole.text;
        Value = LocationCardManagement.Instance.cardRoleDropdown.CardRoleDropdownItem.value;
        pagePer = LocationCardManagement.Instance.pegeNumText.text;
        if (per != null)
        {
            UGUIMessageBox.Show("删除该定位卡，会使与之绑定的人员不再持有定位卡且无法定位。仍要删除吗？", "继续删除", "取消操作",
                () =>
                {
                    DeleteButLocationCard(CurrentTag.Id, () =>
                    {
                        if (IsDelet)
                        {
                            DeleteLocationCardinfo();
                        }
                        else
                        {
                            UGUIMessageBox.Show("删除定位卡信息失败！", "确定", "", null, null, null);
                        }
                    });
                }, null, null);
        }
        else
        {
            DeleteButLocationCard(CurrentTag.Id, () => {
                if (IsDelet)
                {
                    DeleteLocationCardinfo();
                }
                else
                {
                    UGUIMessageBox.Show("删除定位卡信息失败！", "确定", "", null, null, null);
                }

            });
        }
    }
    public void DeleteLocationCardinfo()
    {
        UGUIMessageBox.Show("删除定位卡成功", "确定", "", null, null, null);
        LocationCardManagement.Instance.LocationCardData.RemoveAll(item => item.Id == CurrentTag.Id);
        LocationCardManagement.Instance.ScreenList.RemoveAll(item => item.Id == CurrentTag.Id);
        LocationCardManagement.Instance.ShowCardRoleInfo();
        LocationCardManagement.Instance.ShowLocationCardManagementWindow();
        LocationCardManagement.Instance.cardRoleDropdown.CardRoleDropdownItem.value = Value;
        LocationCardManagement.Instance.cardRoleDropdown.CardRoleDropdownItem.captionText.text = CardRoleDropdown.Instance.CardRoleName[Value];
        double pageNum = Math.Ceiling((double)(LocationCardManagement.Instance.ScreenList.Count) / 10);
        if (int.Parse(pagePer) > pageNum && LocationCardManagement.Instance.ScreenList.Count != 0)
        {
            LocationCardManagement.Instance.pegeNumText.text = pageNum.ToString();
        }
        else if (LocationCardManagement.Instance.ScreenList.Count == 0)
        {
            LocationCardManagement.Instance.pegeNumText.text = "1";
        }
        else
        {
            LocationCardManagement.Instance.pegeNumText.text = "1";
        }
    }
    /// <summary>
    /// 换算单位  404-》4.04V
    /// </summary>
    private float ConversionUnit=100;
    private float GetPowerValue(int num)
    {
        float value = num / ConversionUnit;
        return value;
    }

    /// <summary>
    /// 打开编辑角色定位卡
    /// </summary>
    /// <param name="info"></param>
    public void ShowEditCardRoleInfo(Tag info, List<Tag> TagInfo)
    {
        PosCardEditingInfo.Instance.ShowEditCardInfo(info, TagInfo,Grid,this .gameObject );
        PosCardEditingInfo.Instance.ShowPosCardEditWindow();
        LocationCardManagement.Instance.ShowAndCloseLocationCardManagementWindow(false );
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
