using DG.Tweening;
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DataPaging : MonoBehaviour
{
    public static DataPaging Instance;

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
    /// 下一页
    /// </summary>
    public Button AddPageBut;

    /// <summary>
    /// 上一页
    /// </summary>
    public Button MinusPageBut;

    public Text promptText;
    public InputField PerSelected;//搜索人员
    public Button selectedBut;
    private string nameT;
    private string sex;
    private string workNumber;
    private string department;
    private string post;
    private string tagNum;
    private string tagName;
    private string phone;
    string standbyTime;//待机时间
    string area;
    public Sprite womanPhoto;
    public Sprite manPhoto;
    public Button CreatBut;
    /// <summary>
    /// 人员搜索界面
    /// </summary>
    public GameObject personnelSearchUI;
    public Button CloseBut;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    /// <summary>
    /// 存放每一页数据
    /// </summary>
    [System.NonSerialized]
    List<Personnel> NewpagingData;
    /// <summary>
    /// 单行的背景图片
    /// </summary>
    public Sprite SingleLine;
    /// <summary>
    /// 双行的背景图片
    /// </summary>
    public Sprite DoubleLine;
    [System.NonSerialized]
    public Personnel[] personnels;
    [System.NonSerialized]
    public List<Personnel> peraonnelData;
    public PersonnelDropdown personnelDropdown;
    [System.NonSerialized]
    List<LocationObject> listT;
    public Sprite DoubleImage;
    public Sprite OddImage;
    public bool IsGetPersonData;
    void Start()
    {
        Instance = this;
        NewpagingData = new List<Personnel>();

        //personnels = CommunicationObject.Instance.GetPersonnels();

        // StartPerSearchUI();
        AddPageBut.onClick.AddListener(AddPersonnelPage);
        MinusPageBut.onClick.AddListener(MinusPersonnelPage);
        pegeNumText.onValueChanged.AddListener(InputPersonnelPage);
        selectedBut.onClick.AddListener(SetPerFindData_Click);
        CloseBut.onClick.AddListener(ClosepersonnelSearchWindow);
        CreatBut.onClick.AddListener(OPenAddPersonnelUI);
        PerSelected.onValueChanged.AddListener(InputPerFindData);
        ToggleAuthoritySet();

    }

    Toggle SearchToggle;
    /// <summary>
    /// 刚打开人员搜索时界面的显示
    /// </summary>
    public void StartPerSearchUI()
    {
        SearchToggle = PersonSubsystemManage.Instance != null ? PersonSubsystemManage.Instance.SearchToggle : null;
        if (IsGetPersonData || (SearchToggle != null && SearchToggle.isOn == false)) return;
        Loom.StartSingleThread(() =>
        {
            IsGetPersonData = true;
            personnels = CommunicationObject.Instance.GetPersonnels(); ;

            Loom.DispatchToMainThread(() =>
            {
                IsGetPersonData = false;
                if (SearchToggle != null && SearchToggle.isOn == false) return;

                peraonnelData = new List<Personnel>(personnels);
                selectedItem = new List<Personnel>(personnels);
                //PersonnelSearchTweener.Instance.ShowMinWindow(false);

                personnelSearchUI.SetActive(true);
                StartPageNum = 0;
                PageNum = 1;
                TotaiLine(peraonnelData);
                pegeNumText.text = "1";
                PerSelected.text = "";
                GetPageData(peraonnelData);
                promptText.gameObject.SetActive(false);
                EditPersonnelInformation.Instance.GetJobInfo();
            });
        });
        personnelDropdown.PerDropdown.value = 0;
        personnelDropdown.PerDropdown.captionText.text = "全部";
        listT = LocationManager.Instance.GetPersonObjects();
    }
    /// <summary>
    /// 有几页数据
    /// </summary>
    /// <param name="data"></param>
    public void TotaiLine(List<Personnel> data)
    {
        if (data.Count % pageSize == 0)
        {
            pegeTotalText.text = (data.Count / pageSize).ToString();
        }
        else
        {
            pegeTotalText.text = Convert.ToString(Math.Ceiling((double)(data.Count) / (double)pageSize));
        }
    }
    string EditId;
    public void GetPersonnelInfo(List<Personnel> InfoList)
    {
        for (int i = 0; i < InfoList.Count; i++)
        {
            GameObject obj = InstantiateLine();
            PersonnelDetailItem item = obj.GetComponent<PersonnelDetailItem>();
            item.ShowPersonnelDetailInfo(InfoList[i], peraonnelData, listT);
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
    /// 得到人员搜索的数据
    /// </summary>
    public void GetPersonnelData()
    {

        for (int i = 0; i < NewpagingData.Count; i++)
        {
            nameT = NewpagingData[i].Name;
            sex = NewpagingData[i].Sex;
            workNumber = NewpagingData[i].WorkNumber;

            if (NewpagingData[i].Parent != null)
            {
                post = NewpagingData[i].Pst;
                department = NewpagingData[i].Parent.Name;
            }
            else
            {
                Debug.Log(NewpagingData[i]);
                department = "--";
                post = "--";
            }

            if (NewpagingData[i].TagId != null)
            {
                tagNum = NewpagingData[i].TagId.ToString();
            }
            else
            {
                tagNum = "--";
            }

            if (NewpagingData[i].Tag != null)
            {
                tagName = NewpagingData[i].Tag.Name;

                tagNum = NewpagingData[i].Tag.Code;//编号不是Id是code
            }
            else
            {
                tagName = "--";
                tagNum = "--";
            }
            if (NewpagingData[i].AreaName != null)
            {
                area = NewpagingData[i].AreaName.ToString();
            }
            else
            {
                area = "--";
            }

            if (NewpagingData[i].PhoneNumber == null)
            {
                phone = "--";
            }
            else
            {
                phone = NewpagingData[i].PhoneNumber.ToString();
            }

            EditId = NewpagingData[i].Id.ToString();

            SetInstantiateLine(i);
            SetPersonnelData(i, nameT, sex, workNumber, post, department, area, tagNum, tagName, phone, EditId, NewpagingData[i].Tag);
        }
    }
    /// <summary>
    ///获取第几页数据
    /// </summary>
    public void GetPageData(List<Personnel> data)
    {
        NewpagingData.Clear();
        SaveSelection();
        if (StartPageNum * pageSize < data.Count)
        {
            var QueryData = data.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var per in QueryData)
            {
                NewpagingData.Add(per);
            }

            //GetPersonnelData();
            GetPersonnelInfo(NewpagingData);

        }
    }
    /// <summary>
    /// 下一页信息
    /// </summary>
    public void AddPersonnelPage()
    {

        StartPageNum += 1;
        if (StartPageNum <= selectedItem.Count / pageSize)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();

            GetPageData(selectedItem);

        }
        else
        {
            StartPageNum -= 1;
        }
    }
    /// <summary>
    /// 上一页信息
    /// </summary>
    public void MinusPersonnelPage()
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


            GetPageData(selectedItem);

        }
    }
    /// <summary>
    /// 输入跳转的页数
    /// </summary>
    /// <param name="value"></param>
    public void InputPersonnelPage(string value)
    {
        int currentPage;
        if (string.IsNullOrEmpty(pegeNumText.text))
        {
            currentPage = 1;
        }
        else
        {
            currentPage = int.Parse(pegeNumText.text);
        }

        int maxPage = (int)Math.Ceiling((double)(selectedItem.Count) / (double)pageSize);
        if (currentPage > maxPage)
        {
            currentPage = maxPage;
            pegeNumText.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPageData(selectedItem);
    }

    [System.NonSerialized]
    List<Personnel> selectedItem;

    /// <summary>
    /// 搜索人员
    /// </summary>
    public void SetPerFindData_Click()
    {
        StartPageNum = 0;
        PageNum = 1;
        pegeNumText.text = "1";
        selectedItem.Clear();
        SaveSelection();
        ScreenPersonnelInfo(Level, Key);
    }
    public void InputPerFindData(string key)
    {
        StartPageNum = 0;
        PageNum = 1;
        pegeNumText.text = "1";
        selectedItem.Clear();
        SaveSelection();
        string keyInfo = key.ToLower();
        Key = keyInfo;
        ScreenPersonnelInfo(Level, Key);
    }
    int Level = 0;
    string Key ="";
    public void ScreenPresonnelCardRole(int level)
    {
        Level = level;
        StartPageNum = 0;
        PageNum = 1;
        pegeNumText.text = "1";
        selectedItem.Clear();
        SaveSelection();
        ScreenPersonnelInfo(level, Key);
    }
    public void ScreenPersonnelInfo(int level,string key)
    {
        for (int i = 0; i < peraonnelData.Count; i++)
        {
            string personnelName = peraonnelData[i].Name;
            if (personnelName == "马路峰")
            {
                Debug.LogError(peraonnelData[i].Tag);
            }
            string personnelWorkNum = peraonnelData[i].WorkNumber;
            if (string.IsNullOrEmpty(key))
            {
                TagJudge(peraonnelData[i], level);
            }
            else
            {
                if (personnelName.ToLower().Contains(key) || personnelWorkNum.ToLower().Contains(key))
                {
                    TagJudge(peraonnelData[i], level);
                }

            }

        }
        if (selectedItem.Count == 0)
        {
            pegeTotalText.text = "1";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(selectedItem);
            GetPageData(selectedItem);
        }
    }
    public void TagJudge( Personnel peraonnelData,int level)
    {
        if (peraonnelData.Tag == null)
        {
            if (PerCardRole(level, null))
            {
                selectedItem.Add(peraonnelData);
            }
        }
        else
        {
            LocationObject locationObjectT = listT.Find((item) => item.personnel.TagId == peraonnelData.Tag.Id);
            if (PerCardRole(level, locationObjectT))
            {
                selectedItem.Add(peraonnelData);
            }
        }
    }

    public bool PerCardRole(int level, LocationObject locationObjectT)
    {
        if (level == 0) return true;
        if (level == 1)
        {
            if (locationObjectT != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (level == 2)
        {
            if (locationObjectT == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
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
    /// 给每一条预设赋值
    /// </summary>
    /// <param name="i"></param>
    /// <param name="name"></param>
    /// <param name="sex"></param>
    /// <param name="workNumber"></param>
    /// <param name="department"></param>
    /// <param name="tagNum"></param>
    /// <param name="tagName"></param>
    /// <param name="phone"></param>
    public void SetPersonnelData(int i, string name, string sex, string workNumber, string post, string department, string area, string tagNum, string tagName, string phone, string EditId, Tag tag)
    {
        Transform line = grid.transform.GetChild(i);
        line.GetChild(0).GetComponent<Text>().text = name;
        line.GetChild(1).GetComponent<Text>().text = sex;
        line.GetChild(2).GetComponent<Text>().text = workNumber;
        line.GetChild(3).GetComponent<Text>().text = post;
        line.GetChild(4).GetComponent<Text>().text = department;
        line.GetChild(5).GetComponent<Text>().text = area;
        if (string.IsNullOrEmpty(tagNum))
        {
            line.GetChild(6).GetComponent<Text>().text = "";
        }
        else
        {
            line.GetChild(6).GetComponent<Text>().text = tagNum.ToString();
        }
        int tagId = int.Parse(EditId);

        LocationObject locationObjectT = listT.Find((item) => item.personnel.Id == tagId);

        line.GetChild(7).GetComponent<Text>().text = tagName;
        if (locationObjectT == null)
        {
            line.GetChild(8).GetComponent<Text>().text = "——";
        }
        else
        {
            line.GetChild(8).GetComponent<Text>().text = locationObjectT.personInfoUI.infoStandbyTime.text;
        }

        line.GetChild(9).GetComponent<Text>().text = phone;
        Button but = line.GetChild(10).GetChild(0).GetComponent<Button>();
        but.onClick.RemoveAllListeners();

        but.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(tagNum))
            {

            }
            else
            {
                PersonnelBut_Click(tag.Id);
            }

        });

        line.GetChild(10).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {


            EditPersonnelInformation.Instance.GetPersonnelInformation(tagId, peraonnelData);
            EditPersonnelInformation.Instance.ShowAndCloseEditPersonnelInfo(true);
            SaveSelection();
            personnelSearchUI.SetActive(false);
            IsGetPersonData = false;
            //PersonnelDetails(name, sex);
            //PersonnelSearchTweener.Instance.openTweener.PlayForward();
        });

        Transform line1 = grid.transform.GetChild(i).GetChild(10).GetChild(0);
        if (tag == null || locationObjectT == null)
        {

            line1.GetComponent<Button>().interactable = false;
            Color noTag = new Color(109 / 255f, 236 / 255f, 254 / 255f, 52 / 255f);
            line1.GetComponent<Image>().color = noTag;

        }
        else
        {
            if (tag.IsActive == true)
            {
                Color NormalTag = new Color(109 / 255f, 236 / 255f, 254 / 255f, 255 / 255f);
                line1.GetComponent<Image>().color = NormalTag;
                line1.GetComponent<Button>().interactable = true;
            }
            else
            {
                line1.GetComponent<Button>().interactable = false;
                Color noTag = new Color(109 / 255f, 236 / 255f, 254 / 255f, 52 / 255f);
                line1.GetComponent<Image>().color = noTag;
            }

        }
        if (i % 2 == 0)
        {
            line.GetComponent<Image>().sprite = DoubleLine;
        }
        else
        {
            line.GetComponent<Image>().sprite = SingleLine;
        }
    }


    /// <summary>
    /// 设备定位
    /// </summary>
    public void PersonnelBut_Click(int tagId)
    {
        SaveSelection();
        ParkInformationManage.Instance.ShowParkInfoUI(false);
        AlarmPushManage.Instance.CloseAlarmPushWindow(false);
        LocationManager.Instance.FocusPersonAndShowInfo(tagId);
        personnelSearchUI.SetActive(false);
        IsGetPersonData = false;
        PersonSubsystemManage.Instance.SearchToggle.isOn = false;
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
    /// 生成多少预设
    /// </summary>
    /// <param name="num"></param>
    public void SetInstantiateLine(int num)
    {
        if (grid.transform.childCount <= num)
        {
            InstantiateLine();
        }
        else
        {
            for (int j = grid.transform.childCount - 1; j > num; j--)
            {
                DestroyImmediate(grid.transform.GetChild(j).gameObject);
            }
        }
    }
    /// <summary>
    /// 人员搜索界面打开
    /// </summary>
    /// <param name="b"></param>
    public void ShowpersonnelSearchWindow()
    {
        personnelSearchUI.SetActive(true);
    }
    /// <summary>
    /// 人员搜索界面关闭
    /// </summary>
    public void ClosepersonnelSearchWindow()
    {
        SaveSelection();
        personnelSearchUI.SetActive(false);
        IsGetPersonData = false;
        PersonSubsystemManage.Instance.ChangeImage(false, PersonSubsystemManage.Instance.SearchToggle);
        PersonSubsystemManage.Instance.SearchToggle.isOn = false;
    }
    public void ClosepersonnelSearchUI()
    {
        SaveSelection();
        personnelSearchUI.SetActive(false);
        IsGetPersonData = false;
    }
    public void OPenAddPersonnelUI()
    {
        AddPersonnel.Instance.ShowAddPerWindow(peraonnelData);
        AddPersonnel.Instance.JobManagements.GetJobsManagementData();
    }
    public void CloseAllPerWindow()
    {
        EditPersonnelInformation.Instance.CloseEditPersonnelUI(false);
        EditTagInfo.Instance.CloseCurrentWindow();
        DepartmentList.Instance.CloseDepartmentListUI();
        JobList.Instance.CloseJobListWindow();
        AddPersonnel.Instance.CloseAddPerWindow();
        AddEditCardRoleInfo.Instance.CloseCurrentWindow();
        AddDepartmentList.Instance.CloseDepartmentListUI();
        AddDepartment.Instance.CloseAddDepartmentWindow();
        AddJobList.Instance.CloseJobListWindow();
        AddJobs.Instance.CloseJobEditWindow();
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
