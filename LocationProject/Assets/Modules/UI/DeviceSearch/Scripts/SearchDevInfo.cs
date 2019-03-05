using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SearchDevInfo : MonoBehaviour {

    public static SearchDevInfo Instance;
    /// <summary>
    /// 输入查询设备的名称
    /// </summary>
    public InputField DevInput;
    public Toggle CameraTog;//摄像头按钮
    public Toggle EntranceGuardTog; // 门禁按钮
    public Toggle ServerTog;//服务器
    public Toggle RefrigerationTog;//制冷设备
    public Toggle SwitchesTog;//交换机
    public Toggle OpticalTog;//光纤配线架
    public Toggle NetworkTog;//网络配线架

    public Toggle HuaWeiTog;//华为
    public Toggle HaiKangTog;//海康
    public Toggle ShiNaiDeTog;//施耐德
    public Toggle AiLiXinTog;//爱立信
    public Toggle ZhongDaTog;//中达电通
    /// <summary>
    /// 搜索时的页数
    /// </summary>
    public InputField SearchTotalText;
    /// <summary>
    /// 筛选后的翻页界面
    /// </summary>
    public GameObject SearchPageing;
    /// <summary>
    /// 总页数
    /// </summary>
    public Text pageTotalText;
    public Button SearchAddBut;
    public Button SearchMinBut;
    /// <summary>
    /// 数据
    /// </summary>
    private int StartPageNum = 0;
    /// <summary>
    /// 页数
    /// </summary>
    private int PageNum = 1;
    /// <summary>
    /// 存放获取的10个数据的列表
    /// </summary>
    public List<DevInfo> NewPagingData;
    private string devType;
    private string devName;
    private string devRoute;
    private string devIP;
    private string devID;
    private int depID;
    public List<DevInfo> SearchDevData;
    public List<DevInfo> devData;
    /// <summary>
    /// 设备查询总数居
    /// </summary>
    public List<DevInfo> devSearch;
    /// <summary>
    /// 设备筛选界面
    /// </summary>
    public GameObject SearchWindow;
    /// <summary>
    /// 设备筛选按钮
    /// </summary>
    public Toggle SearchTog;
    /// <summary>
    /// Text 的模板
    /// </summary>
    public GameObject TextExample;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup SearchGrid;

    void Start () {
        Instance = this;
        devSearch = new List<DevInfo>(CommunicationObject.Instance.GetAllDevInfos());  
        SearchTog.onValueChanged.AddListener(ShowSearchWindow);
       
        SearchTotalText.onValueChanged.AddListener(InputDevPage);
        SearchAddBut.onClick.AddListener(AddDevPage);

        SearchMinBut.onClick.AddListener(MinusDevPage);

    }
   
    
   
   
    /// <summary>
    /// 获取设备数据
    /// </summary>
    public void GetDevData(List<DevInfo> dev)
    {
        for (int i = 0; i < NewPagingData.Count; i++)
        {
            devType = NewPagingData[i].TypeName.ToString();
            devName = NewPagingData[i].Name.ToString();
            devRoute = NewPagingData[i].Path.ToString();
            devIP = NewPagingData[i].IP.ToString();
            devID = NewPagingData[i].DevID.ToString();
            depID = (int)NewPagingData[i].ParentId;
            DeviceDataPaging.Instance.SetInstantiateLine(NewPagingData.Count);
          //  DeviceDataPaging.Instance.SetDevData(i, devType, devName, devRoute, devIP, devID,depID);
        }
    }
    /// <summary>
    /// zongye
    /// </summary>
    /// <param name="DevData"></param>
    public void TotaiLine()
    {
        if (SearchDevData.Count % 10 == 0)
        {
            pageTotalText.text = (SearchDevData.Count / 10).ToString();
        }
        else
        {
            pageTotalText.text = Convert.ToString(Math.Ceiling((double)SearchDevData.Count / (double)10));
        }
        if (SearchDevData.Count == 0)
        {
            SearchTotalText.text = "0";
        }
        else
        {
            SearchTotalText.text = "1";
        }
    }
    /// <summary>
    /// 下一页数据
    /// </summary>
    public void AddDevPage()
    {
        StartPageNum += 1;
        if (StartPageNum <= SearchDevData.Count / 10)
        {
            PageNum += 1;
            SearchTotalText.text = PageNum.ToString();
            GetPageDevData();
        }
        else
        {
            StartPageNum -= 1;
        }
    }
    /// <summary>
    /// 上一页信息
    /// </summary>
    public void MinusDevPage()
    {
        if (StartPageNum > 0)
        {
            StartPageNum--;
            PageNum -= 1;
            SearchTotalText.text = PageNum.ToString();
            GetPageDevData();
        }
    }
    /// <summary>
    /// 选择那一页
    /// </summary>
    /// <param name="value"></param>
    public void InputDevPage(string value)
    {
        int currentPage = int.Parse(SearchTotalText.text);
        StartPageNum = currentPage - 1;
        GetPageDevData();
    }
    /// <summary>
    /// 获取页数
    /// </summary>
    public void GetPageDevData()
    {
      
        if (StartPageNum * 10< SearchDevData.Count)
        {
            var QueryData = SearchDevData.Skip(10 * StartPageNum).Take(10);
            foreach (var dev in QueryData)
            {
                NewPagingData.Add(dev);
            }
           GetDevData(NewPagingData);
          
        }
        NewPagingData.Clear();
    }
    /// <summary>
    /// 摄像头筛选
    /// </summary>
    public void CameraTog_Click()
    {
        for (int i = 0; i < devSearch.Count; i++)
         {
             if (devSearch[i].TypeName == "摄像头")
               {
                    SearchDevData.Add(devSearch[i]);
               }
            }
        
    }
    
    /// <summary>
    /// 门禁筛选
    /// </summary>
    /// <param name="ison"></param>
    public void EntranceGuard_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
          {
              if (devSearch[i].TypeName == "门禁")
                {
                    SearchDevData.Add(devSearch[i]);
                }
            }
    }
    /// <summary>
    /// 服务器筛选
    /// </summary>
    /// <param name="isOn"></param>
    public void ServerTog_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "服务器")
                {
                    SearchDevData.Add(devSearch[i]);
                }
        }
    }
    /// <summary>
    /// 制冷设备筛选
    /// </summary>
    /// <param name="isOn"></param>
    public void RefrigerationTog_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "制冷设备")
                {
                    SearchDevData.Add(devSearch[i]);
                }
        }
    }
    /// <summary>
    /// 交换机筛选
    /// </summary>
    /// <param name="isOn"></param>
    public void SwitchesTog_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "交换机")
                {
                    SearchDevData.Add(devSearch[i]);
                }
            
        }
    }
    /// <summary>
    /// 光纤配架线
    /// </summary>
    /// <param name="isOn"></param>
    public void OpticalTog_Click()
    {
        
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "光线配线架")
                {
                    SearchDevData.Add(devSearch[i]);
                }
            
        }
    }
    /// <summary>
    /// 网络配线架筛选
    /// </summary>
    public void NetworkTog_Click()
    { 
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "网络配线架")
                {
                    SearchDevData.Add(devSearch[i]);
                }
        }
    }
    /// <summary>
    /// 华为筛选
    /// </summary>
    public void HuaWeiTog_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "华为")
                {
                    SearchDevData.Add(devSearch[i]);
                }
        }
    }
    /// <summary>
    /// 海康筛选
    /// </summary>
    public void HaiKangTog_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "海康")
                {
                    SearchDevData.Add(devSearch[i]);
                }
            }
    }
    /// <summary>
    /// 施耐德筛选
    /// </summary>
    public void ShiNaiDeTog_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "施耐德")
                {
                    SearchDevData.Add(devSearch[i]);
                }
        }
    }
    /// <summary>
    /// 爱立信筛选
    /// </summary>
    public void AiLiXinTog_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "爱立信")
                {
                    SearchDevData.Add(devSearch[i]);
                }
        }
    }
    /// <summary>
    /// 中达电通筛选
    /// </summary>
    public void ZhongDaTog_Click()
    {
            for (int i = 0; i < devSearch.Count; i++)
            {
                if (devSearch[i].TypeName == "中达电通")
                {
                    SearchDevData.Add(devSearch[i]);
                }
            }
        }

    /// <summary>
    /// 筛选信息
    /// </summary>
    /// <param name="isOn"></param>
    public void  SetSearchDevInformation()
    {
        if (CameraTog.isOn == true )
        {
            CameraTog_Click();
        }
        if (EntranceGuardTog.isOn == true)
        {
            EntranceGuard_Click();
         }
          if (ServerTog.isOn == true)
          {
            ServerTog_Click();
           }

          if (RefrigerationTog.isOn ==true)
          {
            RefrigerationTog_Click();
             }
           if (SwitchesTog.isOn == true)
           {
            SwitchesTog_Click();
            }
           if (OpticalTog.isOn == true)
           {
            OpticalTog_Click();
           }
           if (NetworkTog.isOn == true)
          {
            NetworkTog_Click();
           }
          if (HuaWeiTog.isOn == true)
          {
            HuaWeiTog_Click();
          }
          if (HaiKangTog.isOn == true)
          {
            HaiKangTog_Click();
           }
          if (ShiNaiDeTog.isOn == true)
          {
            ShiNaiDeTog_Click();
           }
          if (AiLiXinTog.isOn == true)
          {
            AiLiXinTog_Click();
          }
          if (ZhongDaTog.isOn == true)
          {
            ZhongDaTog_Click();
           }
    }
    /// <summary>
    /// 显示筛选项的名称
    /// </summary>
    /// <param name="isOn"></param>
    public void SetSearchItem()
    {
        
            if (CameraTog.isOn == true&& SearchGrid.transform.childCount < 6)
            {     
                InstantiateLine("摄像头");          
            }
            if (EntranceGuardTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("门禁");
            }
            if (ServerTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("服务器");
            }

            if (RefrigerationTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("制冷设备");
            }
            if (SwitchesTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("交换机");
            }
            if (OpticalTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("光纤配线架");
            }
            if (NetworkTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("网络配线架");
            }
            if (HuaWeiTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("华为");
            }
            if (HaiKangTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("海康");
            }
            if (ShiNaiDeTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("施耐德");
            }
            if (AiLiXinTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("爱立信");
            }
            if (ZhongDaTog.isOn == true && SearchGrid.transform.childCount < 6)
            {
                InstantiateLine("中达电通");
            }
        
            if (SearchGrid.transform.childCount == 6)
            {
                InstantiateLine("......");
            }

            SetSearchDevInformation();
        
    }
    /// <summary>
    /// 退出设备搜索界面
    /// </summary>
    public void ExitSearchUI()
    {
        if (CameraTog.isOn == true)
        {
            CameraTog.isOn = false;
        }
        if (EntranceGuardTog.isOn == true)
        {
            EntranceGuardTog.isOn =false ;
        }
        if (ServerTog.isOn == true)
        {
            ServerTog.isOn =false;
        }

        if (RefrigerationTog.isOn == true)
        {
            RefrigerationTog.isOn =false;
        }
        if (SwitchesTog.isOn == true)
        {
            SwitchesTog.isOn =false ;
        }
        if (OpticalTog.isOn == true)
        {
            OpticalTog.isOn =false ;
        }
        if (NetworkTog.isOn == true)
        {
            NetworkTog_Click();
        }
        if (HuaWeiTog.isOn == true)
        {
            HuaWeiTog.isOn =false ;
        }
        if (HaiKangTog.isOn == true)
        {
            HaiKangTog.isOn =false ;
        }
        if (ShiNaiDeTog.isOn == true)
        {
            ShiNaiDeTog.isOn =false ;
        }
        if (AiLiXinTog.isOn == true)
        {
            AiLiXinTog.isOn =false ;
        }
        if (ZhongDaTog.isOn == true)
        {
            ZhongDaTog.isOn =false ;
        }
    }
    /// <summary>
    /// 显示筛选界面
    /// </summary>
    /// <param name="ison"></param>
    public void ShowSearchWindow(bool ison)
    {
        if (ison)
        {
            SearchWindow.SetActive(true);
            SearchDevData.Clear();
            DeviceDataPaging.Instance.SaveSelection();
            SaveSelection();
            ExitSearchUI();
        }
        else
        {
            SetSearchItem();
            GetPageDevData();
           
            SearchWindow.SetActive(false);
            SearchPageing.SetActive(true);
            DeviceDataPaging.Instance.ShowTotalPage(false);
            TotaiLine();
        }
    }
    /// <summary>
    /// 生成Text预设
    /// </summary>
    /// <param name="portList"></param>
    public GameObject  InstantiateLine(string Type)
    {
        GameObject Example = Instantiate(TextExample);
        Example.SetActive(true);
        Example.transform.parent = SearchGrid.transform;
        Example.transform.localScale = Vector3.one;
        Example.transform.localPosition = new Vector3(Example.transform.localPosition.x, Example.transform.localPosition.y, 0);
        Example.transform.GetComponent<Text>().text = Type;
        Example.transform.GetComponent<Text>().color = new Color(108/255f,237/255f,253/255f,153/255f);
        return Example;
    }
    /// <summary>
    /// 保留第一项
    /// </summary>
    public void SaveSelection()
    {
        if(SearchGrid.transform.childCount>1)
            {
            for (int j = SearchGrid.transform.childCount - 1; j > 0; j--)
            {
                DestroyImmediate(SearchGrid.transform.GetChild(j).gameObject);
            }
        }
        
    }
}
