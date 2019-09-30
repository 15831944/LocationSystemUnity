using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocationCardManagement : MonoBehaviour
{
    public static LocationCardManagement Instance;
    [System.NonSerialized]
    List<Tag> LocationCardList;
    [System.NonSerialized]
    IList<Tag> CardList;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    //public GameObject DepGrid;
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
    public List<Tag> ScreenList;
    /// <summary>
    /// 总数据
    /// </summary>
    [System.NonSerialized]
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
    public GameObject LocationCardWindow;
    public CardRoleDropdown cardRoleDropdown;
    public InputField LocationRole;
    public Button ScreenBut;
    public Button CloseBut;
    public Button CreatBut;
    public Sprite DoubleImage;
    public Sprite OddImage;
    [System.NonSerialized]
    Personnel[] personnels;
    [System.NonSerialized]
    public List<Personnel> peraonnelData;
    bool IsGetPersonData = true;
    void Start()
    {
        Instance = this;
        AddPageBut.onClick.AddListener(AddLocationCardPage);
        MinusPageBut.onClick.AddListener(MinLocationCardPage);
        pegeNumText.onEndEdit .AddListener(InputLocationCardPage);
        ScreenBut.onClick.AddListener(LocationRoleSearch_Click);
        CloseBut.onClick.AddListener(CloseCurrentWindow);
        LocationRole.onEndEdit .AddListener(InputLocationRoleSearch);
        CreatBut.onClick.AddListener(ShowAddPosCardInfo);
        ToggleAuthoritySet();
    }
    public void CloseCurrentWindow()
    {
        PersonSubsystemManage.Instance.ChangeImage(false, PersonSubsystemManage.Instance.BenchmarkingToggle);
        PersonSubsystemManage.Instance.BenchmarkingToggle.isOn = false;
        if (LocationCardWindow.activeSelf)
        {
            CloseLocationCardWindow();
        }
    }
    public void ShowAddPosCardInfo()
    {
        AddPosCard.Instance.ShowAddPosCardWindow(LocationCardData);
        //    CloseLocationCardWindow();
        ShowAndCloseLocationCardManagementWindow(false);
    }
    private DateTime recordTag;
    public void GetLocationCardManagementData()
    {
        if (!IsGetPersonData) return;
        ThreadManager.Run(() =>
        {
            IsGetPersonData = false;
            LocationCardList = new List<Tag>();
            ShowList = new List<Tag>();
            ScreenList = new List<Tag>();
            LocationCardData = new List<Tag>();
            recordTag = DateTime.Now;
            CardList = CommunicationObject.Instance.GetTags();
            LocationCardList = CardList.ToList();
            LocationCardList.Sort((a, b) =>
            {
                return b.PowerState.CompareTo(a.PowerState);
            });
            LocationCardData.AddRange(LocationCardList);
            ScreenList.AddRange(LocationCardList);
            GetPerInfo();
            IsGetPersonData = true;
        }, () =>
         {
             ShowCardRoleInfo();


         }, "");
    }
    public void ShowCardRoleInfo()
    {
        cardRoleDropdown.GetCardRoleData();
        pegeNumText.text = "1";
        InputLocationCardPage("1");
    }
    public void GetPerInfo()
    {
        peraonnelData = new List<Personnel>();
        personnels = CommunicationObject.Instance.GetPersonnels();
        peraonnelData = new List<Personnel>(personnels);
    }
    public void SetLocationCard(List<Tag> info, List<CardRole> CardRoleList)
    {

        ispage = false;
        for (int i = 0; i < info.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            LocationCardItem item = Obj.GetComponent<LocationCardItem>();
            item.ShowCardRole(info[i], CardRoleList, peraonnelData, info, grid);
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

    /// <summary>
    /// 得到展示数据
    /// </summary>
    /// <param name="info"></param>
    public void GetPageData(List<Tag> info)
    {
        // ispage = false;
        SaveSelection();
        ShowList = new List<Tag>();
        if (StartPageNum * pageSize < info.Count)
        {
            var QueryData = info.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var devAlarm in QueryData)
            {

                ShowList.Add(devAlarm);
            }
        }

        TotaiLine(ScreenList);
        SetLocationCard(ShowList, cardRoleDropdown.CardRoleList);

    }

    public void AddLocationCardPage()
    {
        recordTag = DateTime.Now;
        StartPageNum += 1;
        if (StartPageNum <= ScreenList.Count / pageSize)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            GetPageData(ScreenList);
            Debug.LogError("AddLocationCardPage :" + (DateTime.Now - recordTag).TotalSeconds + " s");
        }
        else
        {
            StartPageNum -= 1;
        }


    }
    public void MinLocationCardPage()
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
    bool ispage = false;
    public void InputLocationCardPage(string vale)
    {
        if (ScreenList.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
            SaveSelection();
            ispage = true;
            return;
        }
        if (ispage == true) return;
        ispage = true;
        int currentPage;
        if (string.IsNullOrEmpty(vale))
        {
            currentPage = 1;
        }
        else
        {
            if (vale.Contains("-") || vale.Contains("—"))
            {
                pegeNumText.text = "1";
                currentPage = 1;
            }
            else
            {
                currentPage = int.Parse(vale);
            }
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
    /// 输入查找项
    /// </summary>
    /// <param name="Key"></param>
    public void InputLocationRoleSearch(string Key)
    {
        pegeNumText.text = "1";
        ScreenList.Clear();
        String key = Key.ToLower();
        string KeyName = cardRoleDropdown.CardRoleDropdownItem.captionText.text;
        for (int i = 0; i < LocationCardData.Count; i++)
        {
            if (key == "")
            {
                if (LocationCardData[i].CardRoleId == 0)
                {
                    if (KeyName == "全部人员")
                    {
                        ScreenList.Add(LocationCardData[i]);
                    }
                }
                else
                {
                    if (KeyName == "全部人员")
                    {
                        ScreenList.Add(LocationCardData[i]);
                    }
                    else
                    {

                        foreach (var role in cardRoleDropdown.CardRoleList)
                        {
                            if (KeyName == role.Name)
                            {
                                if (LocationCardData[i].CardRoleId == role.Id)
                                {
                                    ScreenList.Add(LocationCardData[i]);
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                string PerName = "";
                Personnel per = peraonnelData.Find((item) => item.Id == LocationCardData[i].PersonId);
                if (per !=null)
                {
                    PerName = per.Name;
                }
                if (LocationCardData[i].CardRoleId == 0)
                {
                    if (KeyName == "全部人员")
                    {
                        
                            if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                            {
                                ScreenList.Add(LocationCardData[i]);
                            }                       
                    }
                }
                else
                {
                    if (KeyName == "全部人员")
                    {
                        if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                        {
                            ScreenList.Add(LocationCardData[i]);

                        }
                    }
                    else
                    {
                        foreach (var role in cardRoleDropdown.CardRoleList)
                        {
                            if (KeyName == role.Name)
                            {
                                if (LocationCardData[i].CardRoleId == role.Id )
                                {
                                    if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                                    {
                                        ScreenList.Add(LocationCardData[i]);

                                    }
                                }
                            }
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
            TotaiLine(ScreenList);
            GetPageData(ScreenList);
        }
    }
    /// <summary>
    ///   点击查找But
    /// </summary>
    public void LocationRoleSearch_Click()
    {
        pegeNumText.text = "1";
        ScreenList.Clear();
        String key = LocationRole.text .ToLower();
        string KeyName = cardRoleDropdown.CardRoleDropdownItem.captionText.text;
        for (int i = 0; i < LocationCardData.Count; i++)
        {
            if (key == "")
            {
                if (LocationCardData[i].CardRoleId == 0)
                {
                    if (KeyName == "全部人员")
                    {
                        ScreenList.Add(LocationCardData[i]);
                    }
                }
                else
                {
                    if (KeyName == "全部人员")
                    {
                        ScreenList.Add(LocationCardData[i]);
                    }
                    else
                    {

                        foreach (var role in cardRoleDropdown.CardRoleList)
                        {
                            if (KeyName == role.Name)
                            {
                                if (LocationCardData[i].CardRoleId == role.Id)
                                {
                                    ScreenList.Add(LocationCardData[i]);
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                string PerName = "";
                Personnel per = peraonnelData.Find((item) => item.Id == LocationCardData[i].PersonId);
                if (per != null)
                {
                    PerName = per.Name;
                }
                if (LocationCardData[i].CardRoleId == 0)
                {
                    if (KeyName == "全部人员")
                    {

                        if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                        {
                            ScreenList.Add(LocationCardData[i]);
                        }
                    }
                }
                else
                {
                    if (KeyName == "全部人员")
                    {
                        if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                        {
                            ScreenList.Add(LocationCardData[i]);

                        }
                    }
                    else
                    {
                        foreach (var role in cardRoleDropdown.CardRoleList)
                        {
                            if (KeyName == role.Name)
                            {
                                if (LocationCardData[i].CardRoleId == role.Id)
                                {
                                    if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                                    {
                                        ScreenList.Add(LocationCardData[i]);

                                    }
                                }
                            }
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
            TotaiLine(ScreenList);
            GetPageData(ScreenList);
        }
    }
    /// <summary>
    /// 下拉框筛选
    /// </summary>
    /// <param name="level"></param>
    public void GetLocationCardRoleType(int level)
    {
        pegeNumText.text = "1";
        if (ScreenList != null && ScreenList.Count != 0)
        {
            ScreenList.Clear();
        }

        string key = LocationRole.text.ToString().ToLower();
        string KeyName = cardRoleDropdown.CardRoleDropdownItem.captionText.text;
        for (int i = 0; i < LocationCardData.Count; i++)
        {
            if (key == "")
            {
                if (LocationCardData[i].CardRoleId == 0)
                {
                    if (KeyName == "全部人员")
                    {
                        ScreenList.Add(LocationCardData[i]);
                    }
                }
                else
                {
                    if (KeyName == "全部人员")
                    {
                        ScreenList.Add(LocationCardData[i]);
                    }
                    else
                    {
                        foreach (var role in cardRoleDropdown.CardRoleList)
                        {
                            if (KeyName == role.Name)
                            {
                                if (LocationCardData[i].CardRoleId == role.Id)
                                {
                                    ScreenList.Add(LocationCardData[i]);
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                string PerName = "";
                Personnel per = peraonnelData.Find((item) => item.Id == LocationCardData[i].PersonId);
                if (per !=null)
                {
                    PerName = per.Name;
                }
                if (LocationCardData[i].CardRoleId == 0)
                {
                    if (KeyName == "全部人员")
                    {
                        
                            if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                            {
                                ScreenList.Add(LocationCardData[i]);
                            }                       
                    }
                }
                else
                {
                    if (KeyName == "全部人员")
                    {
                        if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                        {
                            ScreenList.Add(LocationCardData[i]);

                        }
                    }
                    else
                    {
                        foreach (var role in cardRoleDropdown.CardRoleList)
                        {
                            if (KeyName == role.Name)
                            {
                                if (LocationCardData[i].CardRoleId == role.Id )
                                {
                                    if (LocationCardData[i].Code.ToString().ToLower().Contains(key) || LocationCardData[i].Name.ToLower().Contains(key) || PerName.ToLower().Contains(key))
                                    {
                                        ScreenList.Add(LocationCardData[i]);

                                    }
                                }
                            }
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
            TotaiLine(ScreenList);
            GetPageData(ScreenList);
        }
    }
    public bool LocationCardRoleType(string type)
    {
        string role = cardRoleDropdown.CardRoleDropdownItem.captionText.text;
        int level = cardRoleDropdown.CardRoleDropdownItem.value;
        if (level == 0) return true;
        else if (type == GetRoleType())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public string GetRoleType()
    {
        int level = cardRoleDropdown.CardRoleDropdownItem.value;
        for (int i = 1; i < cardRoleDropdown.CardRoleList.Count; i++)
        {
            if (level == i)

                return cardRoleDropdown.CardRoleList[i].Name;
        }
        return null;
    }
    public void ShowLocationCardManagementWindow()
    {
        LocationCardWindow.SetActive(true);
        IsGetPersonData = true;
    }
    public void CloseLocationCardWindow()
    {
        SaveSelection();
        LocationCardWindow.SetActive(false);
        LocationRole.text = "";
        IsGetPersonData = true;
    }

    public void ShowAndCloseLocationCardManagementWindow(bool b)
    {
        LocationCardWindow.SetActive(b);
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
    public void TotaiLine(List<Tag> dinfo)
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
        if (grid.transform.childCount == 0) return;
        for (int j = grid.transform.childCount - 1; j >= 0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
    }
    public void CloseAllCardRoleWindow()
    {
        PosCardEditingInfo.Instance.ClosePosCardEditWindow();
        AddLocationPerEditInfo.Instance.CloseCurrentWindow();
        RoleNameList.Instance.CloseCurrentRoleWindow();
        AddPosCard.Instance.CloseAddPosCardWindow();
        AddPerCardPermission.Instance.CloseCurrentWindow();
        AddLocationRoleEdit.Instance.CloseCurrentWindow();
        EditPersonnelRoleInfo.Instance.CloseEditPersonnelRoleWindow();
    }
    /// <summary>
    /// 不同权限下，按钮的显示
    /// </summary>
    private void ToggleAuthoritySet()
    {
        if (CommunicationObject.Instance.IsGuest())
        {
            CreatBut.gameObject.SetActive(false);
        }
        else
        {
            CreatBut.gameObject.SetActive(true);
        }
    }
}
