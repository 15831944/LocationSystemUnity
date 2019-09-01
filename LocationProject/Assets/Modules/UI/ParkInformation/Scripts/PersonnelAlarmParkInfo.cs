using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PersonnelAlarmParkInfo : MonoBehaviour
{
    public static PersonnelAlarmParkInfo Instance;
    public GameObject PersonnelAlarmParkWindow;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    public int pageLine = 10;
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

    public InputField pegeNumText;//输入选中页
    public Text promptText;
    public Button AddPageBut;
    public Button MinusPageBut;
    public InputField InputPerAlarm;//输入筛选内容

    [System.NonSerialized]
    /// <summary>
    /// 10条数据存放的列表
    /// </summary>
    public List<LocationAlarm> newPerAlarmList;

    public Sprite DoubleImage;
    public Sprite OddImage;
    [System.NonSerialized]
    List<LocationAlarm> PersonnelAlarm;//全部的人员告警
    [System.NonSerialized]
    List<LocationAlarm> SeachPerItems;//筛选后告警list
    public Button SearchBut;
    public Button CloseBut;
    public Text title;
    bool isPage;
  
    public ParkPersonnelAlarmType parkPersonnelAlarmType;
    /// <summary>
    // Use this for initialization
    void Start()
    {
        Instance = this;
        SeachPerItems = new List<LocationAlarm>();
        PersonnelAlarm = new List<LocationAlarm>();
        SearchBut.onClick.AddListener(PerAlarmSearchBut_Click);
        pegeNumText.onValueChanged.AddListener(InputPersonnelPage);
        AddPageBut.onClick.AddListener(AddPerAlarmPage);
        MinusPageBut.onClick.AddListener(MinPerAlarmPage);
        InputPerAlarm.onValueChanged.AddListener(InputPerAlarmSearch);
        CloseBut.onClick.AddListener(() =>
        {
            ShowPersonnelAlarmParkWindow(false);
            });
    }
    /// <summary>
    /// 得到园区统计人员告警的全部数据
    /// </summary>
    /// <param name="date"></param>
    public void GetPerAlarmList(List<LocationAlarm> date,string name)
    {
        title.text = "定位告警—"+name ;
        if (date.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
           
        }else
        {
            PersonnelAlarm.AddRange(date);
            SeachPerItems.AddRange(date);
            StartPageNum = 0;
            PageNum = 0;
            GetPersonnelAlarmPage(PersonnelAlarm);
            pegeNumText.text = "1";
            InputPerAlarm.text = "";
            TotaiLine(PersonnelAlarm);
             isPage = false;
        }
        
    }
    /// <summary>
    /// 生成多少页
    /// </summary>
    public void TotaiLine(List<LocationAlarm> date)
    {
        if (date.Count != 0)
        {
            if (date.Count % pageLine == 0)
            {
                pegeTotalText.text = (date.Count / pageLine).ToString();
            }
            else
            {
                pegeTotalText.text = Convert.ToString(Math.Ceiling((double)date.Count / (double)pageLine));
            }
        }
        else
        {
            pegeTotalText.text = "1";

        }

    }
    public void SetPersonnelAlarmInfo(List<LocationAlarm> data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            GameObject obj = InstantiateLine();
            PersonneiAlarmInfo item = obj.GetComponent<PersonneiAlarmInfo>();
            item.SetEachItemInfo(data[i]);
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
    /// 每一行的预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject InstantiateLine()
    {
        GameObject Obj = Instantiate(TemplateInformation);
        Obj.SetActive(true);
        Obj.transform.parent = grid.transform;
        Obj.transform.localScale = Vector3.one;
        Obj.transform.localPosition = new Vector3(Obj.transform.localPosition.x, Obj.transform.localPosition.y, 0);
        return Obj;
    }
    public void ShowPersonnelAlarmParkWindow(bool b)
    {
        if (b==true )
        {
            PersonnelAlarmParkWindow.SetActive(true );
            ParkInformationManage.Instance.PersonToggle.isOn = true;
           
        }
        else
        {
            ParkInformationManage.Instance.IsGetPerData = false;
            PersonnelAlarmParkWindow.SetActive(false );
            ParkInformationManage.Instance.PersonToggle.isOn = false;
            PersonnelAlarm.Clear();
            SaveSelection();
        }
    }
    
    /// <summary>
    /// 生成的页数
    /// </summary>
    public void GetPersonnelAlarmPage(List<LocationAlarm> data)
    {

        newPerAlarmList = new List<LocationAlarm>();
        if (StartPageNum * pageLine < data.Count)
        {
            var QueryData = data.Skip(pageLine * StartPageNum).Take(pageLine);
            foreach (var devAlarm in QueryData)
            {

                newPerAlarmList.Add(devAlarm);


            }
            TotaiLine(data);
            SetPersonnelAlarmInfo(newPerAlarmList);
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
   
   
    public void InputPerAlarmSearch(string value)
    {
        SaveSelection();
        SeachPerItems.Clear();
        pegeNumText.text = "1";
        value = InputPerAlarm.text.ToString();
        string key = value.ToLower();
        for (int i = 0; i < PersonnelAlarm.Count; i++)
        {
            if (string.IsNullOrEmpty(key))
            {        
                    SeachPerItems.Add(PersonnelAlarm[i]);
            }

            else
            {
                if (PersonnelAlarm[i].Id.ToString().ToLower().Contains(key) || PersonnelAlarm[i].Personnel .Name .ToLower().Contains(key))
                {
                   
                        SeachPerItems.Add(PersonnelAlarm[i]);
                   
                    
                }

            }
        }
        if (SeachPerItems.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            TotaiLine(SeachPerItems);
            GetPersonnelAlarmPage(SeachPerItems);
        }
    }
    /// <summary>
    /// 搜索人员
    /// </summary>
    public void PerAlarmSearchBut_Click()
    {
        SaveSelection();
        SeachPerItems.Clear();
        pegeNumText.text = "1";
        string key = InputPerAlarm.text.ToString().ToLower();
        for (int i = 0; i < PersonnelAlarm.Count; i++)
        {
            if (string .IsNullOrEmpty (key))
            {
                
                    SeachPerItems.Add(PersonnelAlarm[i]);
               
            }
            
            else 
            {
                if (PersonnelAlarm[i].Id.ToString().ToLower().Contains(key) || PersonnelAlarm[i].Personnel.Name.ToLower().Contains(key))
                {
                    
                        SeachPerItems.Add(PersonnelAlarm[i]);
                    

                }

            }
        }
        if (SeachPerItems.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            TotaiLine(SeachPerItems);
            GetPersonnelAlarmPage(SeachPerItems);
        }
    }
    public void GetScreenPersonnelAlarmType(int level)
    {
        SaveSelection();
        SeachPerItems.Clear();
        pegeNumText.text = "1";
        string key = InputPerAlarm.text.ToString().ToLower();
        for (int i = 0; i < PersonnelAlarm.Count; i++)
        {
            if (key == "" )
            {
                SeachPerItems.Add(PersonnelAlarm[i]);
            }
           else  if (PersonnelAlarm[i].Id.ToString().ToLower().Contains(key) || PersonnelAlarm[i].Personnel.Name.ToLower().Contains(key))
            {
                
                    SeachPerItems.Add(PersonnelAlarm[i]);
                

            }
        }
        if (SeachPerItems.Count == 0)
        {
            pegeNumText.text = "1";
            pegeTotalText.text = "1";
        }
        else
        {
            TotaiLine(SeachPerItems);
            GetPersonnelAlarmPage(SeachPerItems);
        }
    }
    public void AddPerAlarmPage()
    {
     
        double a = Math.Ceiling((double)SeachPerItems.Count / (double)pageLine);
        int m = (int)a;
        if (StartPageNum == 0)
        {
            StartPageNum += 1;
            PageNum += 1;
        }
      else  if (StartPageNum < m&&StartPageNum !=0)
        {
            StartPageNum += 1;
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            SaveSelection();
            GetPersonnelAlarmPage(SeachPerItems);
        }
    }
    public void MinPerAlarmPage()
    {
      if (StartPageNum == 1)
        {
            StartPageNum--;
            PageNum -= 1;
        }
       else  if (StartPageNum > 0)
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
            SaveSelection();
            GetPersonnelAlarmPage(SeachPerItems);
        }
    }
    
    public void InputPersonnelPage(string value)
    {
        if (isPage == true) return;
        isPage = true;
        int currentPage = 0;
        currentPage = int.Parse(pegeNumText.text);
        int MaxPage = (int)Math.Ceiling((double)SeachPerItems.Count /(double)pageLine);
        if (MaxPage == 0)
        {
            pegeNumText.text = "1";
        }else
        {
            if (string.IsNullOrEmpty(pegeNumText.text))
            {
                currentPage = 1;
            }
            else if (currentPage >= MaxPage)
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
            SaveSelection();
            GetPersonnelAlarmPage(SeachPerItems);
            isPage = false;
        }
        
    }
    void Update()
    {

    }
}
