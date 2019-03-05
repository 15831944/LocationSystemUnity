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
    public Button    AddPageBut;
  
    /// <summary>
    /// 上一页
    /// </summary>
    public Button   MinusPageBut;

    public Text promptText;
    public InputField PerSelected;//搜索人员
    public Button selectedBut;
    private string nameT;
    private string sex;
    private int workNumber;
    private string department;
    private string tagNum;
    private string tagName;
    private string phone;
    string area;
    public Sprite womanPhoto;
    public Sprite manPhoto;
    /// <summary>
    /// 人员详细信息界面
    /// </summary>
    public GameObject PersonnelInfo;
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
    List<Personnel> NewpagingData;
    /// <summary>
    /// 单行的背景图片
    /// </summary>
    public Sprite SingleLine;
    /// <summary>
    /// 双行的背景图片
    /// </summary>
    public Sprite DoubleLine;

    public Personnel[] personnels;
    public List<Personnel> peraonnelData;
    void Start()
    {
        Instance = this;
        NewpagingData = new List<Personnel>();
       
        //personnels = CommunicationObject.Instance.GetPersonnels();

        // StartPerSearchUI();
        AddPageBut. onClick.AddListener(AddPersonnelPage);
        MinusPageBut.onClick.AddListener(MinusPersonnelPage);
        pegeNumText.onValueChanged.AddListener(InputPersonnelPage);
        selectedBut.onClick.AddListener(SetPerFindData_Click);
        CloseBut.onClick.AddListener(ClosepersonnelSearchWindow);

    }
    /// <summary>
    /// 刚打开人员搜索时界面的显示
    /// </summary>
    public void StartPerSearchUI()
    {
        //personnels = CommunicationObject.Instance.GetPersonnels();
        //peraonnelData = new List<Personnel>(personnels);
        //selectedItem = new List<Personnel>(personnels);
        ////PersonnelSearchTweener.Instance.ShowMinWindow(false);
        //personnelSearchUI.SetActive(true);
        //StartPageNum = 0;
        //PageNum = 1;
        //GetPageData(peraonnelData);
        //TotaiLine(peraonnelData);
        //pegeNumText.text = "1";
        //PerSelected.text = "";
        //promptText.gameObject.SetActive(false);

        Loom.StartSingleThread(() =>
        {
            personnels = CommunicationObject.Instance.GetPersonnels(); ;
            Loom.DispatchToMainThread(() =>
            {
                peraonnelData = new List<Personnel>(personnels);
                selectedItem = new List<Personnel>(personnels);
                //PersonnelSearchTweener.Instance.ShowMinWindow(false);
                personnelSearchUI.SetActive(true);
                StartPageNum = 0;
                PageNum = 1;
                GetPageData(peraonnelData);
                TotaiLine(peraonnelData);
                pegeNumText.text = "1";
                PerSelected.text = "";
                promptText.gameObject.SetActive(false);
            });
        });
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
                department = NewpagingData[i].Parent.Name;
            }
            else
            {
                Debug.Log(NewpagingData[i]);
            }

            if (NewpagingData[i].TagId != null)
            {
                tagNum = NewpagingData[i].TagId.ToString();
            }
            else
            {
                Debug.Log(NewpagingData[i].TagId);
            }

            if (NewpagingData[i].Tag != null)
            {
                tagName = NewpagingData[i].Tag.Name.ToString();
            }
            else
            {
                Debug.Log(NewpagingData[i].Tag);
            }
            if (NewpagingData[i].AreaName!=null)
            {
                area = NewpagingData[i].AreaName.ToString();
            }
            else
            {
                area = "";
            }

            if (NewpagingData[i].PhoneNumber == null)
            {
                phone = "";
            }
            else
            {
                phone = NewpagingData[i].PhoneNumber.ToString();
            }

            SetInstantiateLine(i );
            SetPersonnelData(i, nameT, sex, workNumber, department, area, tagNum, tagName, phone);
        }
    }
    /// <summary>
    ///获取第几页数据
    /// </summary>
    public void GetPageData(List<Personnel> data)
    {

        if (StartPageNum * pageSize < data.Count)
        {
            var QueryData = data.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var per in QueryData)
            {
                NewpagingData.Add(per);
            }
           
            GetPersonnelData();
     
        }

        NewpagingData.Clear();
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


        if (StartPageNum >0)
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
        if (currentPage>maxPage)
        {
            currentPage = maxPage;
            pegeNumText.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
            currentPage = 1;
            pegeNumText.text = currentPage.ToString ();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPageData(selectedItem);
    }
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
        string key = PerSelected.text.ToString().ToLower();
       
        for (int i = 0; i < peraonnelData.Count; i++)
        {
            string personnelName = peraonnelData[i].Name;
            string personnelWorkNum = peraonnelData[i].WorkNumber.ToString();
            if (personnelName.ToLower().Contains(key)|| personnelWorkNum.ToLower().Contains(key ))
            {   
                selectedItem.Add(peraonnelData[i]);
            }
        }
        if (selectedItem .Count == 0)
        {
            pegeTotalText.text = "1";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false );
            TotaiLine(selectedItem);
            GetPageData(selectedItem);
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
    public void SetPersonnelData(int i, string name, string sex, int workNumber, string department,string area, string tagNum, string tagName, string phone)
    {
        Transform line = grid.transform.GetChild(i);
        line.GetChild(0).GetComponent<Text>().text = name;
        line.GetChild(1).GetComponent<Text>().text = sex;
        line.GetChild(2).GetComponent<Text>().text = workNumber.ToString();
        line.GetChild(3).GetComponent<Text>().text = department;
        line.GetChild(4).GetComponent<Text>().text = area;
        line.GetChild(5).GetComponent<Text>().text = tagNum.ToString();
        line.GetChild(6).GetComponent<Text>().text = tagName;
        line.GetChild(7).GetComponent<Text>().text = phone;
        Button but = line.GetChild(8).GetChild(0).GetComponent<Button>();
        but .onClick .RemoveAllListeners ();
        but.onClick.AddListener(() =>
        {
            PersonnelBut_Click(tagNum);
        });
        line.GetChild(8).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            PersonnelDetails(name, sex);
            PersonnelSearchTweener.Instance.openTweener.PlayForward();
        });
        if (i % 2 == 0)
        {
            line.GetComponent<Image>().sprite = DoubleLine;
        }
        else
        {
            line.GetComponent<Image>().sprite = SingleLine;
        }
    }
    public void PersonnelDetails(string name, string sex)
    {
        PersonnelInfo.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = name;
        PersonnelInfo.transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>().text = sex;

        //openTweener.PlayForward();
        PersonnelSearchTweener.Instance.openTweener.PlayForward();
        if (sex == "女")
        {
            PersonnelInfo.transform.GetChild(0).GetComponent<Image>().sprite = womanPhoto;

        }
        else
        {
            PersonnelInfo.transform.GetChild(0).GetComponent<Image>().sprite = manPhoto;
        }
    }
    /// <summary>
    /// 设备定位
    /// </summary>
    public void PersonnelBut_Click(string tagNum)
    {
        
        ParkInformationManage.Instance.ShowParkInfoUI(false);
        AlarmPushManage.Instance.ShowAlarmPushWindow(false);
        int tagId = int.Parse(tagNum);
        Debug.LogError("tagNum:" + tagNum);

        //LocationObject obj = LocationManager.Instance.GetLocationObjByPersonalId(tagId);

        LocationManager.Instance.FocusPersonAndShowInfo(tagId);
        //if (obj != null)
        //{
        //    List<DepNode> depNodeListT = FactoryDepManager.currentDep.GetComponentsInChildren<DepNode>().ToList();
        //    if (!depNodeListT.Contains(obj.currentDepNode))
        //    {
        //        RoomFactory.Instance.ChangeDepNodeNoTween();
        //        LocationManager.Instance.RecoverBeforeFocusAlignToOrigin();
        //    }
        //}


        personnelSearchUI.SetActive(false);
        PersonSubsystemManage.Instance.SearchToggle.isOn = false;
    }
    /// <summary>
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public void InstantiateLine()
    {
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
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

        personnelSearchUI.SetActive(false);

        PersonSubsystemManage.Instance.ChangeImage(false, PersonSubsystemManage.Instance.SearchToggle);
        PersonSubsystemManage.Instance.SearchToggle.isOn = false;
    }

   
}
