using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AddDepartmentList : MonoBehaviour {
    public  static  AddDepartmentList Instance;
    [System.NonSerialized] List<Department> DepartList;
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
    [System.NonSerialized] List<Department> ScreenList;
    /// <summary>
    /// 部门总数据
    /// </summary>
    [System.NonSerialized] List<Department> DepartmentData;
    /// <summary>
    /// 展示的10条信息
    /// </summary>
    [System.NonSerialized] List<Department> ShowList;
    /// <summary>
    /// 下一页
    /// </summary>
    public Button AddPageBut;

    /// <summary>
    /// 上一页
    /// </summary>
    public Button MinusPageBut;

    public InputField DepSelected;
    public Button selectedBut;
    public Button AddDep;
    public AddEditDepDropdown addDepDropdown;
    public GameObject DepartmentListWindow;
    public Button CloseDepartmentList;
    public Sprite DoubleImage;
    public Sprite OddImage;

    [System.NonSerialized] IList<Department> departIList;
    void Start()
    {

        Instance = this;
        AddPageBut.onClick.AddListener(AddDepartmentPage);
        MinusPageBut.onClick.AddListener(MinusDepartmentPage);
        pegeNumText.onValueChanged.AddListener(InputDepartmentPage);
        selectedBut.onClick.AddListener(SetDepartment_Click);
       
        CloseDepartmentList.onClick.AddListener(()=>
        {
            UGUIMessageBox.Show("关闭部门信息！",
    () =>
    {
        CloseDepartmentListUI();
        AddPersonnel.Instance.ShowAddPerWindow();
    }, ()=> {
        CloseDepartmentListUI();
        AddPersonnel.Instance.ShowAddPerWindow();
    });
           
        });
        AddDep.onClick.AddListener(() =>
      {
          AddDepartment.Instance.IsAdd = true ;
          addDepDropdown.DepartManagement();
          AddDepartment.Instance.ShowAddDepartmentWindow();
          AddDepartment.Instance.GetDepartmentList(DepartmentData);
          CloseDepartmentListUI();
      });
    }
    public void GetDepartmentListData()
    {
        DepartList = new List<Department>();
        departIList = CommunicationObject.Instance.GetDepartmentList();
        DepartList = departIList.ToList();

        DepSelected.text = "";
        SaveSelection();
        ScreenList = new List<Department>();
        DepartmentData = new List<Department>();
        ShowList = new List<Department>();
        if (ScreenList.Count != 0)
        {
            ScreenList.Clear();
        }
       if (DepartmentData.Count != 0)
        {
            DepartmentData.Clear();
        }
        
       
        ScreenList.AddRange(DepartList);
        DepartmentData.AddRange(DepartList);
        TotaiLine(DepartList);
        pegeNumText.text = "1";
        GetPageData(DepartList);
    }
    public void SetDepartmentData(List<Department> depList)
    {
        for (int i = 0; i < depList.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            AddDepartmentItem item = Obj.GetComponent<AddDepartmentItem>();
            item.ShowDepartmentItemInfo(depList[i], DepartmentData);
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
        GameObject o = Instantiate(TemplateInformation);
        o.SetActive(true);
        o.transform.parent = grid.transform;
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, 0);
        return o;
    }
    /// <summary>
    /// 得到第几页数据
    /// </summary>
    /// <param name="depList"></param>
    /// <param name="perInfo"></param>
    public void GetPageData(List<Department> depList)
    {
        SaveSelection();
        if (StartPageNum * pageSize < depList.Count)
        {
            var QueryData = depList.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var per in QueryData)
            {
                ShowList.Add(per);
            }
            SetDepartmentData(ShowList);
        }
        ShowList.Clear();
    }
    public void AddDepartmentPage()
    {
        StartPageNum += 1;
        if (StartPageNum <= ScreenList.Count / pageSize)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();
            GetPageData(ScreenList);
        }
        else
        {
            StartPageNum -= 1;
        }
    }
    public void MinusDepartmentPage()
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
        }
    }
    /// <summary>
    /// 输入跳转的页数
    /// </summary>
    /// <param name="value"></param>
    public void InputDepartmentPage(string value)
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

        int maxPage = (int)Math.Ceiling((double)(ScreenList.Count) / (double)pageSize);
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
        GetPageData(ScreenList);
    }
    /// <summary>
    /// 筛选部门
    /// </summary>
    public void SetDepartment_Click()
    {
        StartPageNum = 0;
        PageNum = 1;
        pegeNumText.text = "1";
        ScreenList.Clear();
        SaveSelection();
        string key = DepSelected.text.ToString().ToLower();
        for (int i = 0; i < DepartmentData.Count; i++)
        {
            string Name = DepartmentData[i].Name;
            string SuperiorName;
            if (string.IsNullOrEmpty(DepartmentData[i].ParentId.ToString()))
            {
                if (Name.ToLower().Contains(key))
                {
                    ScreenList.Add(DepartmentData[i]);
                }
            }
            else
            {
                int id = (int)DepartmentData[i].ParentId;
                foreach (var per in DepartmentData)
                {
                    if (id == per.Id)
                    {
                        SuperiorName = per.Name.ToString();
                        if (Name.ToLower().Contains(key) || SuperiorName.ToLower().Contains(key))
                        {
                            ScreenList.Add(DepartmentData[i]);
                        }
                    }
                }
            }


        }
        if (ScreenList.Count == 0)
        {
            pegeTotalText.text = "1";
        }
        else
        {
            TotaiLine(ScreenList);
            GetPageData(ScreenList);
        }
    }
    /// <summary>
    /// 有几页数据
    /// </summary>
    /// <param name="data"></param>
    public void TotaiLine(List<Department> depList)
    {
        if (depList.Count % pageSize == 0)
        {
            pegeTotalText.text = (depList.Count / pageSize).ToString();
        }
        else
        {
            pegeTotalText.text = Convert.ToString(Math.Ceiling((double)(depList.Count) / (double)pageSize));
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
    public void ShowDepartmentListUI()
    {
        DepartmentListWindow.SetActive(true);
        AddPersonnel.Instance.CloseAddPerWindow();
    }
    public void CloseDepartmentListUI()
    {
        DepartmentListWindow.SetActive(false);
    }
   
}
