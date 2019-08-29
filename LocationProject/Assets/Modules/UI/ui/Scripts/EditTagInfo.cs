using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditTagInfo : MonoBehaviour
{
    public static EditTagInfo Instance;
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
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    const int pageSize = 10;
    /// <summary>
    /// 数据
    /// </summary>
    private int StartPageNum = 0;
    /// <summary>
    /// 页数
    /// </summary>
    private int PageNum = 1;
    /// <summary>
    /// 总页数
    /// </summary>
    public Text pegeTotalText;
    /// <summary>
    /// 输入页数
    /// </summary>
    public InputField pegeNumText;
    /// <summary>
    /// 筛选后的数据
    /// </summary>
    [System.NonSerialized]
    List<Tag> ScreenList;
    /// <summary>
    /// 总数据
    /// </summary>
    public List<Tag> LocationCardData;
    /// <summary>
    /// 展示的10条信息
    /// </summary>
    [System.NonSerialized]
    List<Tag> ShowList;
    /// <summary>
    /// 下一页
    /// </summary>
    public Button AddPageBut;

    /// <summary>
    /// 上一页
    /// </summary>
    public Button MinusPageBut;
    [System.NonSerialized]
    public List<Tag> TagsList;
    public EditTagCardRoleDropdown cardRoleDropdown;
    // public InputField LocationRole;
    //public Button ScreenBut;
    public Button CloseBut;
    public Button SaveBut;
    public GameObject EditTagInfoWindow;
    // string tagRoleName;
    [System.NonSerialized]
    public Tag ChooseTag;
    List<Personnel> PersonnelList;
    public ToggleGroup TogGroup;
    public Button RemoveBut;
    void Start()
    {
        Instance = this;
        ShowList = new List<Tag>();
        AddPageBut.onClick.AddListener(AddCardRolePage);
        MinusPageBut.onClick.AddListener(MinCardRolePage);
        pegeNumText.onValueChanged.AddListener(InputLocationCardPage);
        //  ScreenBut.onClick.AddListener(LocationRoleSearch_Click);
        // LocationRole.onValueChanged.AddListener(InputLocationRoleSearch);
        CloseBut.onClick.AddListener(CloseEditTagInfoWindow);

        SaveBut.onClick.AddListener(SaveEditTagInfo);
        RemoveBut.onClick.AddListener(RemoveBinding);
    }
    public void GetTagData(Personnel personData, List<Personnel> personnelList)
    {
        PersonnelList = new List<Personnel>();
        PersonnelList.AddRange(personnelList);
        TagsList = new List<Tag>();
        LocationCardData = new List<Tag>();
        ScreenList = new List<Tag>();
        CommunicationObject.Instance.GetTags((tagsT) =>
        {
            if (tagsT != null)
            {
                TagsList = new List<Tag>(tagsT);
                if (TagsList.Count == 0)
                {
                    pegeNumText.text = "1";
                    pegeTotalText.text = "1";
                }
                else
                {
                    TagDataSorting(TagsList, personData);

                }

            }

        });
    }
    Tag CurrentTag;
    //[System.NonSerialized]
    //private List<Tag> SortTagList;
    Personnel CurrentPer;

    /// <summary>
    /// 排序并展示数据
    /// </summary>
    /// <param name="tagData"></param>
    /// <param name="personData"></param>
    public void TagDataSorting(List<Tag> tagData, Personnel personData)
    {
        CurrentPer = personData;

        List<Tag> tagListTemp = SortTagList(tagData, personData);//对列表进行排序

        ScreenList.AddRange(tagListTemp);
        LocationCardData.AddRange(tagListTemp);

        ShowEditTagInfo();//展示列表数据（分页）

        //设置第一页和总页数
        pegeNumText.text = "1";
        SetTotalPage(tagListTemp);

    }

    public class TagEx:IComparable<TagEx>
    {
        public Tag Tag;
        public Personnel Per;
        public string perName = "";

        public TagEx(Tag tag, Personnel per)
        {
            this.Tag = tag;
            this.Per = per;
            if (per != null)
            {
                perName = per.Name;
            }
        }

        public int CompareTo(TagEx other)
        {
            return this.perName.CompareTo(other.perName);
            //return other.perName.CompareTo(this.perName);
        }
    }

    /// <summary>
    /// 对列表进行排序
    /// </summary>
    /// <param name="tagList"></param>
    /// <param name="personData"></param>
    /// <returns></returns>
    private List<Tag> SortTagList(List<Tag> tagList, Personnel personData)
    {
        if (tagList == null) return null;
        try
        {
            Dictionary<int, Personnel> perDict = GetPersonnelDict();//为了提高性能，将Personnel缓存到Dictionary中

            List<TagEx> tagListEx = new List<TagEx>();
            foreach (var tag in tagList)//循环一次 
            {
                Personnel per = null;
                if (tag.PersonId != null)
                {
                    //per = PersonnelList.Find(i => i.Id == tag.PersonId);//人员数量很多的情况下可能活有性能问题
                    int perId = (int)tag.PersonId;
                    if (perDict.ContainsKey(perId))
                    {
                        per = perDict[perId];
                    }
                }
                TagEx tagEx = new TagEx(tag, per);
                tagListEx.Add(tagEx);
            }
            tagListEx.Sort();//排序，根据是否绑定了人员

            List<Tag> tagListTemp = new List<Tag>();
            foreach (var tagEx in tagListEx)
            {
                tagListTemp.Add(tagEx.Tag);
            }
            //后续代码和原来的一样

            //var tagListTemp = tagList.OrderBy(i => i.IsActive).ToList();//根据IsActive排序

            //将当前的人员排到第一个
            if (personData.Tag != null)
            {
                CurrentTag = tagListTemp.Find(i => i.Id == personData.Tag.Id);
                if (CurrentTag != null)
                {
                    tagListTemp.Remove(CurrentTag);
                    tagListTemp.Insert(0, CurrentTag);//将当前的人员排到第一个
                }
            }
            return tagListTemp;
        }
        catch (Exception ex)
        {
            Log.Error("SortTagList", ex.ToString());
            return tagList;
        }
    }

    private Dictionary<int, Personnel> GetPersonnelDict()
    {
        Dictionary<int, Personnel> perDict = new Dictionary<int, Personnel>();
        foreach (var per in PersonnelList)//循环一次
        {
            perDict.Add(per.Id, per);
        }

        return perDict;
    }

    public void ShowEditTagInfo()
    {

        GetPageData(ScreenList);//ShowEditTagInfo

    }
    public void SetRoleCardInfo(List<Tag> info)
    {
       
        for (int i = 0; i < info.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            EditTagInfoItem item = Obj.GetComponent<EditTagInfoItem>();
            item.GetCardRole(info[i], cardRoleDropdown.CardRoleList, PersonnelList, CurrentTag, CurrentPer);

        }
        //   ToggleAuthoritySet();
    }
    bool ispage = false;
    public void GetPageData(List<Tag> info)
    {
        ispage = false;
        SaveSelection();
        ShowList.Clear();
        if (StartPageNum * pageSize < info.Count)
        {
            var QueryData = info.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var devAlarm in QueryData)
            {

                ShowList.Add(devAlarm);
            }
        }
        SetTotalPage(ScreenList);
        SetRoleCardInfo(ShowList);//设置人员绑定信息
    }
    public void AddCardRolePage()
    {
        StartPageNum += 1;
        double a = Math.Ceiling((double)ScreenList.Count / (double)pageSize);
        int m = (int)a;
        if (StartPageNum <= m)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            GetPageData(ScreenList);
        }
    }
    public void MinCardRolePage()
    {

        if (StartPageNum > 0)
        {
            StartPageNum--;
            PageNum -= 1;
            if (PageNum == 0)
            {
                pegeNumText.text = "1";
            }
            else
            {
                pegeNumText.text = PageNum.ToString();
            }
            GetPageData(ScreenList);
        }

    }
    public void RemoveBinding()
    {
        EditTagInfo.Instance.TogGroup.allowSwitchOff = true ;
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (grid.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Toggle>().isOn == true)
            {
                grid.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Toggle>().isOn = false;
                grid.transform.GetChild(i).GetChild(5).GetComponent<Text>().text = "——";
            }

        }
    }
    public void InputLocationCardPage(string vale)
    {

        if (ispage == true) return;
        ispage = true;
        int currentPage;
        if (string.IsNullOrEmpty(pegeNumText.text))
        {
            currentPage = 1;
        }
        else
        {
            currentPage = int.Parse(pegeNumText.text);
        }

        int MaxPage = (int)Math.Ceiling((double)ScreenList.Count / (double)pageSize);
        if (currentPage > MaxPage)
        {
            currentPage = MaxPage;
            pegeNumText.text = currentPage.ToString();
        }
        else if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPageData(ScreenList);


    }


    /// <summary>
    /// 角色筛选
    /// </summary>
    /// <param name="level"></param>
    public void GetLocationCardRoleType(int level)
    {
        SaveSelection();
        ScreenList.Clear();
        for (int i = 0; i < LocationCardData.Count; i++)
        {
            if (LocationCardData[i].CardRoleId == 0)
            {               
                if (level==0)
                {
                    ScreenList.Add(LocationCardData[i]);
                }
            }
            else
            {
                foreach (var role in cardRoleDropdown.CardRoleList)
                {
                    if (LocationCardData[i].CardRoleId == role.Id)
                    {
                        if (LocationCardRoleType(role.Name))
                        {
                            ScreenList.Add(LocationCardData[i]);
                        }
                    }
                }
            }
        }
        if (ScreenList.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            SaveSelection();

        }
        else
        {
            pegeNumText.text = "1";
            SetTotalPage(ScreenList);
            GetPageData(ScreenList);
        }

    }
    public bool LocationCardRoleType(string type)
    {
        string role = cardRoleDropdown.CardRoleDropdownItem.captionText.text;
        if (role == type)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
   
    public void CloseEditTagInfoWindow()
    {
        EditTagInfoWindow.SetActive(false);
        EditPersonnelInformation.Instance.ShowEditPersonnelWindow();
        // EnsureSaveData();

    }
    string SelectName = "";
    string SelectID = "";
    public void CloseCurrentWindow()
    {
        SelectName = "";
        SelectID = "";
        EditTagInfoWindow.SetActive(false);
    }
    public void ShowEditTagInfoWindow()
    {
     
        EditTagInfoWindow.SetActive(true);
    }
    public void ServePersonnelDate(string id, string name)
    {
        int CurrentTagId;
        bool IsEdit;
        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
        {
            CurrentTagId = int.Parse(id);
            EditPersonnelInformation.Instance.TagName.text = name;
                    
            Tag locationObjectT = LocationCardData.Find((item) => item.Id == CurrentTagId);
            if (locationObjectT != null)
            {
                ChooseTag = locationObjectT;
            }
        }
        else
        {

            
            CurrentPer.TagId = 0;
            IsEdit = CommunicationObject.Instance.EditPerson(CurrentPer);
            EditPersonnelInformation.Instance.TagName.text = "-尚未选择-";
        }
      
    }

    public void EnsureGetData(Action action)
    {
       
        SelectName = "";
        SelectID = "";

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            Toggle tog = grid.transform.GetChild(i).GetChild(1).GetChild(0).gameObject.GetComponent<Toggle>();
            string id = grid.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text;
            string name = grid.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Text>().text;

            //if (tog.isOn)
            //{
            //    EditPersonnelInformation.Instance.TagName.text = name;
            //    int CurrentTagId = int.Parse(id);
            //    Tag locationObjectT = LocationCardData.Find((item) => item.Id == CurrentTagId);
            //    if (locationObjectT != null)
            //    {
            //        ChooseTag = locationObjectT;
            //    }
            //}
            if (tog.isOn)
            {
                SelectName = name;
                SelectID = id;
            }
        }
        ServePersonnelDate(SelectID, SelectName);
        if (action != null)
        {
            action();
        }
    }
    public void EnsureSaveData()
    {
        UGUIMessageBox.Show("保存标签卡信息!", () =>
        {
            EnsureGetData(() =>
            {
                EditTagInfoWindow.SetActive(false);
                EditPersonnelInformation.Instance.ShowEditPersonnelWindow();
            });
        },
        () =>
        {
            EditTagInfoWindow.SetActive(false);

            EditPersonnelInformation.Instance.ShowEditPersonnelWindow();
           
        }

        );

    }
    public void SaveEditTagInfo()
    {
        UGUIMessageBox.Show("保存标签卡信息!", () =>
        {
            EnsureGetData(() =>
            {
                EditTagInfoWindow.SetActive(false);
                EditPersonnelInformation.Instance.ShowEditPersonnelWindow();
            
            });
        },
               null

               );
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
    /// 有几页数据
    /// </summary>
    /// <param name="data"></param>
    private void SetTotalPage(List<Tag> dinfo)
    {
        if (dinfo.Count % pageSize == 0)
        {
            pegeTotalText.text = (dinfo.Count / pageSize).ToString();
        }
        else
        {
            pegeTotalText.text = Convert.ToString(Math.Ceiling((double)(dinfo.Count) / (double)pageSize));
        }
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
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            SaveBut.GetComponent<Button>().interactable = false;
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                grid.transform.GetChild(i).GetComponent<Toggle>().interactable = false;
            }

        }
        else
        {
            SaveBut.GetComponent<Button>().interactable = true;
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                grid.transform.GetChild(i).GetComponent<Toggle>().interactable = true;
            }
        }
    }
    void Update()
    {

    }
}
