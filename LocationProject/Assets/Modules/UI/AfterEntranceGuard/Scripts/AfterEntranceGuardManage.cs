using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AfterEntranceGuardManage : MonoBehaviour
{
    public static AfterEntranceGuardManage Instance;
    public List<EntranceGuardActionInfo> EntranceGuardList;
    public List<EntranceGuardActionInfo> NewpagingData;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button CloseButton;
    /// <summary>
    /// 经过门禁（24小时内）
    /// </summary>
    public GameObject Window;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    /// <summary>
    /// 单行的背景图片
    /// </summary>
    public Sprite SingleLine;
    /// <summary>
    /// 双行的背景图片
    /// </summary>
    public Sprite DoubleLine;
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
    // Use this for initialization
    void Start()
    {
        Instance = this;
        EntranceGuardList = new List<EntranceGuardActionInfo>();
        CloseButton.onClick.AddListener(CloseWindow);
        AddPageBut.onClick.AddListener(AddEntranceGuardPage);
        MinusPageBut.onClick.AddListener(MinusEntranceGuardPage);
        pegeNumText.onValueChanged.AddListener(InputEntranceGuardPage);
    }

    // Update is called once per frame
    void Update()
    {
     

    }
    /// <summary>
    /// 有几页数据
    /// </summary>
    /// <param name="data"></param>
    public void TotaiLine(List<EntranceGuardActionInfo> data)
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
    ///获取第几页数据
    /// </summary>
    public void GetPageData(List<EntranceGuardActionInfo> data)
    {

        if (StartPageNum * pageSize < data.Count)
        {
            var QueryData = data.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var per in QueryData)
            {
                NewpagingData.Add(per);
            }
            if (grid .transform .childCount != 0)
            {
                SaveSelection();
            }
           
            SetEntranceGuardInfo(NewpagingData);

        }

        NewpagingData.Clear();
    }
    /// <summary>
    /// 获取门禁数据
    /// </summary>
    /// <param name="id"></param>
    public void GetEntranceGuardData(int id)
    {
        var EntranceGuardData = CommunicationObject.Instance.GetEntranceActionInfoByPerson24Hours(id);
        if (EntranceGuardData !=null)
        {
            EntranceGuardList = new List<EntranceGuardActionInfo>(EntranceGuardData);
            TotaiLine(EntranceGuardList);
            GetPageData(EntranceGuardList);
        }
      else
        {
            pegeTotalText.text = "1";
          
        }
        pegeNumText.text = "1";

    }
    public void SetEntranceGuardInfo(List<EntranceGuardActionInfo> data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            GameObject Obj = InstantiateLine();
            if (i % 2 == 0)
            {
                Obj.transform.GetComponent<Image>().sprite = DoubleLine;
            }
            else
            {
                Obj.transform.GetComponent<Image>().sprite = SingleLine;
            }
            EntranceGuardInfo item = Obj.GetComponent<EntranceGuardInfo>();
            item.ShowEntranceGuardInfo(data[i]);
           
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
        return Obj;
    }
    /// <summary>
    /// 下一页信息
    /// </summary>
    public void AddEntranceGuardPage()
    {

        StartPageNum += 1;
        if (StartPageNum <= EntranceGuardList.Count / pageSize)
        {
            PageNum += 1;
            pegeNumText.text = PageNum.ToString();

            GetPageData(EntranceGuardList);

        }
        else
        {
            StartPageNum -= 1;
        }
    }
    /// <summary>
    /// 上一页信息
    /// </summary>
    public void MinusEntranceGuardPage()
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


            GetPageData(EntranceGuardList);

        }
    }
    /// <summary>
    /// 输入跳转的页数
    /// </summary>
    /// <param name="value"></param>
    public void InputEntranceGuardPage(string value)
    {

        int currentPage = int.Parse(pegeNumText.text);
        int maxPage = (int)Math.Ceiling((double)(EntranceGuardList.Count) / (double)pageSize);
        if (currentPage >= maxPage)
        {
            if (maxPage != 0)
            {
                currentPage = maxPage;
               
            }
           else
            {
                currentPage = 1;
            }
            pegeNumText.text = currentPage.ToString();
        }
        if (currentPage <= 0)
        {
          
            pegeNumText.text = currentPage.ToString();
        }
        StartPageNum = currentPage - 1;
        PageNum = currentPage;
        GetPageData(EntranceGuardList);
    }
    /// <summary>
    /// 删除生成的预设
    /// </summary>
    public void SaveSelection()
    {
        for (int j = grid.transform.childCount - 1; j >=0; j--)
        {
            DestroyImmediate(grid.transform.GetChild(j).gameObject);
        }
       
    }
    public void ShowWindow()
    {
        Window.SetActive(true);
    }
    public void CloseWindow()
    {
        SaveSelection();
        Window.SetActive(false);
    }
}
