using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPerCardPermission : MonoBehaviour {
    public static AddPerCardPermission Instance;
    [System.NonSerialized]
    List<CardRole> CardRoleList;
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
    public Button CloseBut;
    public Button CreatPerRole;
    public InputField InputName;
    [System.NonSerialized]
    CardRole AddCardRole;
    public Button SaveBut;
    public Button CancelBut;
    public GameObject AddRoleWindow;
    public Button CloseAddRole;
    public int PreviousID;//上一个选中的ID
    [System.NonSerialized]
    List<Tag> CurrentTagList;
    void Start()
    {
        Instance = this;
     //   CloseBut.onClick.AddListener(CloseCurrentWindows);
        CreatPerRole.onClick.AddListener(()=> {
            AddLocationRoleEdit.Instance.ShowEditRoleWindow();
        });
       // CloseAddRole.onClick.AddListener(CloseAddRoleWindow);
    }
    public void CloseCurrentWindows()
    {
        CloseRoleWindow();
        AddPosCard.Instance.ShowAddPosCardWindow();
    }
    public void GetCardRoleData()
    {
        CardRoleList = new List<CardRole>();
        if (CardRoleList.Count != 0)
        {
            CardRoleList.Clear();
        }
        CardRoleList = CommunicationObject.Instance.GetCardRoleList();
        PreviousID = CardRoleList[0].Id;
        GetCardRoleInfo(CardRoleList, CurrentTagList);
    }
    public void GetCardRoleData(List<Tag> TagInfo)
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
    }
    public void GetCardRoleInfo(List<CardRole> info, List<Tag> TagInfo)
    {
        for (int i = 0; i < info.Count; i++)
        {
            GameObject obj = InstantiateLine();
            AddCardRoleItem item = obj.GetComponent<AddCardRoleItem>();
            item.ShowCardRoleInfo(info[i], TagInfo);
            if (i % 2 == 0)
            {
                item.GetComponent<Image>().sprite = DoubleImage;
            }
            else
            {
                item.GetComponent<Image>().sprite = OddImage;
            }
        }

    }
    public void AddRoleInfo()
    {
        ShowAddRoleWindow();
        AddCardRole = new CardRole();
        AddCardRole.Name = InputName.text;
        SaveBut.onClick.AddListener(() =>
        {
            UGUIMessageBox.Show("保存编辑角色信息！",
          () =>
          {
              SaveAddReloInfo(AddCardRole);
          }, null);

        });
        CancelBut.onClick.AddListener(() =>
        {
            UGUIMessageBox.Show("不保存编辑角色信息！",
       () =>
       {
           Debug.LogError("不保存编辑定位卡信息");
       }, null);
        });
    }
    public void ShowAddRoleWindow()
    {
        PreviousID = CardRoleList[0].Id;
        AddRoleWindow.SetActive(true);
        CloseRoleWindow();

    }
   
    public void SaveAddReloInfo(CardRole info)
    {
    int CardRoleId=   CommunicationObject.Instance.AddCardRole(info);
        info.Id = CardRoleId;
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
        AddRolePermissionsTreeView.Instance.IsAddSure = false;

    }
    public void CloseCurrentWindow()
    {
        CloseRoleWindow();
        AddRoleWindow.SetActive(false);
    }
}
