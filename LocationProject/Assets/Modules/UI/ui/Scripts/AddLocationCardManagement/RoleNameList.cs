
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleNameList : MonoBehaviour {
    public static RoleNameList Instance;
    [System.NonSerialized] List<CardRole> CardRoleList;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    public Sprite DoubleImage;
    public Sprite OddImage;
    public GameObject RoleWindow;
  
    public Button CreatPerRole;
    public int PreviousID;//上一个选中的ID
    [System.NonSerialized]
    List<Tag> CurrentTagList;
    void Start () {
        Instance = this;

        CreatPerRole.onClick.AddListener(()=> {
            AddPerEditInfo.Instance.ShowAddRoleWindow();
        });
    }
   
    public void GetCardRoleData(Action action = null)
    {
        CardRoleList = new List<CardRole>();
        if (CardRoleList.Count != 0)
        {
            CardRoleList.Clear();
        }    
        CardRoleList = CommunicationObject.Instance.GetCardRoleList();
        PreviousID = CardRoleList[0].Id;
        GetCardRoleInfo(CardRoleList, CurrentTagList);
        if (action != null) action();
    }
    public void GetCardRoleData(List<Tag> TagInfo,Action action = null)
    {
        CurrentTagList = new List<Tag>();
        CurrentTagList.AddRange(TagInfo);
        CardRoleList = new List<CardRole>();
        if (CardRoleList.Count != 0)
        {
            CardRoleList.Clear();
        }
        CardRoleList = CommunicationObject.Instance.GetCardRoleList();
        PreviousID = CardRoleList[0].Id;
        GetCardRoleInfo(CardRoleList, TagInfo);
        if (action != null) action();
    }
    public void GetCardRoleInfo(List<CardRole> info, List<Tag> TagInfo)
    {
      for (int i =0;i <info .Count;i++)
        {
            GameObject obj = InstantiateLine();
            CardRoleItem item = obj.GetComponent<CardRoleItem>();
            item.ShowCardRoleInfo(info[i], TagInfo);
            if (i % 2 == 0)
            {
                item.GetComponent<Image>().sprite = DoubleImage;
            }
            else
            {
                item.GetComponent<Image>().sprite = OddImage;
            }
            if (i == 1)
            {
                
            }
        }
     
    }

  
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject InstantiateLine()
    {
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
        return o;
    }
    /// <summary>
    /// 保留选中项
    /// </summary>
    public void SaveSelection()
    {
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
    public void ShowRoleWindow()
    {
        SaveSelection();
        RoleWindow.SetActive(true);
      
    }
    public void CloseRoleWindow()
    {
        SaveSelection();
        RoleWindow.SetActive(false);
        PreviousID = CardRoleList[0].Id;
        RolePermissionsTreeView.Instance.IsSure = false;

    }
    public void CloseCurrentRoleWindow()
    {
        AddPerEditInfo.Instance . AddRoleWindow.SetActive(false);
        RoleWindow.SetActive(false);
      //  PreviousID = CardRoleList[0].Id;
    }
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            CreatPerRole.GetComponent<Button>().interactable = false;
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                grid.transform.GetChild(i).GetChild(3).GetComponent<Toggle>().interactable = false;
            }

        }
        else
        {
            CreatPerRole.GetComponent<Button>().interactable = true;
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                grid.transform.GetChild(i).GetChild(3).GetComponent<Toggle>().interactable = true;
            }
        }
    }
    void Update () {
		
	}
}
