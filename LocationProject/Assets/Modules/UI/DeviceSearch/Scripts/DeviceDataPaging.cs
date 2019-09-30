using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeviceDataPaging : MonoBehaviour
{
    public static DeviceDataPaging Instance;
    /// <summary>
    /// 每页显示的条数
    /// </summary>
    public int pageSize = 10;
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
    public Text pageTotalText;
    /// <summary>
    /// 输入页数
    /// </summary>
    public InputField pageNumText;
    /// <summary>
    /// 下一页
    /// </summary>
    public Button AddPageBut;
    /// <summary>
    /// 上一页
    /// </summary>
    public Button MinusPageBut;
    /// <summary>
    /// 输入设备名称
    /// </summary>
    public InputField InputDev;
    /// <summary>
    /// 输入设备名称后，确定按钮
    /// </summary>
    public Button DevBut;
    /// <summary>
    /// 没有筛选时切页按钮
    /// </summary>
    public GameObject TotalPage;
    /// <summary>
    /// 关闭界面按钮
    /// </summary>
    public Button closeButton;
    /// <summary>
    /// 设备搜素界面
    /// </summary>
    public GameObject devSearchUI;

    public Text promptText;
    private string devType;
    private string devName;
    private string devRoute;
    private string devIP;
    private string devID;
    private int depID;
    private string ManufactorName;
    /// <summary>
    /// 行的模板
    /// </summary>
    public GameObject TemplateInformation;
    /// 存放预设生成的集合
    /// </summary>
    public GridLayoutGroup grid;
    /// <summary>
    /// 分页后数据存放
    /// </summary>
    [System.NonSerialized]
    List<DevInfo> NewPagingData = new List<DevInfo>();
    [System.NonSerialized]
    public List<DevInfo> devSearch;
    [System.NonSerialized]
    public List<DevInfo> SeachDevName;
    public Sprite DoubleImage;
    public Sprite OddImage;
    public DeviceTypeDropdown deviceTypeDropdown;
    public DeviceNameDropdown deviceNameDropdown;
    void Start()
    {
        Instance = this;
        //devSearch = new List<DevInfo>(CommunicationObject.Instance.GetAllDevInfos());
        //   SeachDevName = devSearch;
        //   TotaiLine(devSearch);
        //  GetPageDevData(devSearch);
        AddPageBut.onClick.AddListener(AddDevPage);
        MinusPageBut.onClick.AddListener(MinusDevPage);
        pageNumText.onEndEdit .AddListener(InputDevPage);
        DevBut.onClick.AddListener(InputDevName);
        closeButton.onClick.AddListener(ClosedevSearchWindow);
        deviceNameDropdown.devNameDropdown.onValueChanged.AddListener(ScreenDeviceType);
        deviceTypeDropdown.DevTypeDropdown.onValueChanged.AddListener(ScreenDeviceType);
        InputDev.onEndEdit .AddListener(InputDevName_Click);
        //pageNumText.text = "1";
        //StartPageNum = 0;
    }
    /// <summary>
    /// 刚打开时的界面 
    /// </summary>
    public void StartDevSeachUI()
    {
        //SeachDevName = new List<DevInfo>(CommunicationObject.Instance.GetAllDevInfos());
        //TotaiLine(devSearch);
        //pageNumText.text = "1";
        //PageNum = 1;
        //StartPageNum = 0;
        //InputDev.text = "";
        //GetPageDevData(devSearch);
        //ShowTotalPage(true);
        ////  SearchDevInfo.Instance.SearchPageing.SetActive(false);
        //promptText.gameObject.SetActive(false);

        Loom.StartSingleThread(() =>
        {
            devSearch = new List<DevInfo>(CommunicationObject.Instance.GetAllDevInfos());

            Loom.DispatchToMainThread(() =>
            {
                SeachDevName = new List<DevInfo>();
                SeachDevName.AddRange(devSearch);
                //  SeachDevName = devSearch;
                pageNumText.text = "1";
                PageNum = 1;
                StartPageNum = 0;
                InputDev.text = "";
                GetPageDevData(devSearch);
                TotaiLine(devSearch);
                ShowTotalPage(true);
                //  SearchDevInfo.Instance.SearchPageing.SetActive(false);
                promptText.gameObject.SetActive(false);
            });
        });
    }
    /// <summary>
    /// zongye
    /// </summary>
    /// <param name="DevData"></param>
    public void TotaiLine(List<DevInfo> devNum)
    {
        if (devNum == null || devNum.Count == 0)
        {
            pageTotalText.text = "1";
        }
        else
        {
            if (devNum.Count % pageSize == 0)
            {
                pageTotalText.text = (devNum.Count / pageSize).ToString();
            }
            else
            {
                pageTotalText.text = Convert.ToString(Math.Ceiling((double)devNum.Count / (double)pageSize));
            }
        }


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
            if (NewPagingData[i].Manufactor == null)
            {
                ManufactorName = "";
            }
            else
            {
                ManufactorName = NewPagingData[i].Manufactor;
            }

            devID = NewPagingData[i].DevID.ToString();
            depID = (int)NewPagingData[i].ParentId;
            SetInstantiateLine(NewPagingData.Count);
            SetDevData(i, devType, devName, devRoute, ManufactorName, devIP, devID, depID);
        }
    }
    /// <summary>
    /// 获取每页的数据
    /// </summary>
    public void GetPageDevData(List<DevInfo> DevData)
    {
        NewPagingData.Clear();
        if (StartPageNum * pageSize < DevData.Count)
        {
            var QueryData = DevData.Skip(pageSize * StartPageNum).Take(pageSize);
            foreach (var dev in QueryData)
            {
                NewPagingData.Add(dev);
            }

            GetDevData(NewPagingData);
        }

    }

    /// <summary>
    /// 生成多少行预设
    /// </summary>
    /// <param name="num"></param>
    public void SetInstantiateLine(int num)
    {
        if (grid.transform.childCount < num)
        {
            InstantiateLine();
        }
        else
        {
            for (int j = grid.transform.childCount - 1; j >= num; j--)
            {
                DestroyImmediate(grid.transform.GetChild(j).gameObject);
            }
        }
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
    /// 每条信息赋值
    /// </summary>
    /// <param name="i"></param>

    public void SetDevData(int i, string devType, string devName, string devRoute, string ManufactorName, string devIP, string devID, int depID)
    {
        Transform line = grid.transform.GetChild(i);
        line.GetChild(0).GetComponent<Text>().text = devType;
        line.GetChild(1).GetComponent<Text>().text = devName;
        line.GetChild(2).GetComponent<Text>().text = devRoute;
        line.GetChild(3).GetComponent<Text>().text = ManufactorName;
        line.GetChild(4).GetComponent<Text>().text = devIP;
        Button but = line.GetChild(5).GetChild(0).GetComponent<Button>();
        but.onClick.RemoveAllListeners();
        but.onClick.AddListener(() =>
        {
            DevBut_Click(devID, depID, devName);

        });
        if (i % 2 == 0)
        {
            line.GetComponent<Image>().sprite = DoubleImage;
        }
        else
        {
            line.GetComponent<Image>().sprite = OddImage;
        }
    }
    public void AddDevPage()
    {
        StartPageNum += 1;
        if (StartPageNum <= SeachDevName.Count / pageSize)
        {
            PageNum += 1;
            pageNumText.text = PageNum.ToString();
            GetPageDevData(SeachDevName);
        }
        else
        {
            StartPageNum -= 1;
        }
    }
    /// <summary>
    /// 下一页信息
    /// </summary>
    public void MinusDevPage()
    {
        if (StartPageNum > 0)
        {
            StartPageNum--;
            PageNum -= 1;
            if (PageNum == 0)
            {
                pageNumText.text = "1";
            }
            else
            {
                pageNumText.text = PageNum.ToString();
            }
            GetPageDevData(SeachDevName);
        }
    }
    /// <summary>
    /// 输入页数
    /// </summary>
    /// <param name="value"></param>
    public void InputDevPage(string value)
    {
        int currentPage;
        if (string.IsNullOrEmpty(pageNumText.text))
        {
            currentPage = 1;
        }
        else
        {
            if (value.Contains("-") || value.Contains("—"))
            {
                pageNumText.text = "1";
                currentPage = 1;
            }
            else
            {
                currentPage = int.Parse(value);
            }
        }
        if (SeachDevName.Count == 0)
        {
            pageTotalText.text = "1";
            StartPageNum = currentPage - 1;
            PageNum = currentPage;
            return;
        }
        else
        {
            int maxPage = (int)Math.Ceiling((double)SeachDevName.Count / (double)pageSize);
            if (currentPage > maxPage)
            {
                currentPage = maxPage;
                pageNumText.text = currentPage.ToString();
            }
            if (currentPage <= 0)
            {
                currentPage = 1;
                pageNumText.text = currentPage.ToString();
            }
            StartPageNum = currentPage - 1;
            PageNum = currentPage;
            GetPageDevData(SeachDevName);
        }

    }
    /// <summary>
    /// 输入搜索名字
    /// </summary>
    public void InputDevName()
    {
        StartPageNum = 0;
        PageNum = 1;
        pageNumText.text = "1";
        SeachDevName.Clear();
        SaveSelection();
        string key = InputDev.text.ToString().ToLower();
        for (int i = 0; i < devSearch.Count; i++)
        {
            string devName = devSearch[i].Name.ToLower();
            string devIP = "";
            if (!string.IsNullOrEmpty(devSearch[i].IP))
            {
                devIP = devSearch[i].IP.ToLower();
            }
            if (string.IsNullOrEmpty(key))
            {
                if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                {
                    SeachDevName.Add(devSearch[i]);
                }
            }
            else
            {
                if (devName.ToLower().Contains(key) || devIP.ToLower().Contains(key))
                {
                    if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                    {
                        SeachDevName.Add(devSearch[i]);
                    }
                }
            }

        }
        if (SeachDevName.Count == 0)
        {
            pageNumText.text = "1";
            pageTotalText.text = "1";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(SeachDevName);
            GetPageDevData(SeachDevName);
        }

    }
    /// <summary>
    /// 输入搜索名字
    /// </summary>
    public void InputDevName_Click(string valu)
    {
        StartPageNum = 0;
        PageNum = 1;
        pageNumText.text = "1";
        SeachDevName.Clear();
        SaveSelection();
        string key = valu.ToLower();
        for (int i = 0; i < devSearch.Count; i++)
        {
            string devName = devSearch[i].Name.ToLower();
            string devIP = "";
            if (!string.IsNullOrEmpty(devSearch[i].IP))
            {
                devIP = devSearch[i].IP.ToLower();
            }
            if (string.IsNullOrEmpty(key))
            {
                if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                {
                    SeachDevName.Add(devSearch[i]);
                }
            }
            else
            {
                if (devName.ToLower().Contains(key) || devIP.ToLower().Contains(key))
                {
                    if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                    {
                        SeachDevName.Add(devSearch[i]);
                    }
                }
            }
        }
        if (SeachDevName.Count == 0)
        {
            pageNumText.text = "1";
            pageTotalText.text = "1";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(SeachDevName);
            GetPageDevData(SeachDevName);
        }

    }
    public void DeviceType_Click(int valu)
    {
        StartPageNum = 0;
        PageNum = 1;
        pageNumText.text = "1";
        SeachDevName.Clear();
        SaveSelection();
        string key = InputDev.text.ToString().ToLower();
        for (int i = 0; i < devSearch.Count; i++)
        {
            string devName = devSearch[i].Name.ToLower();
            string devIP = "";
            if (!string .IsNullOrEmpty(devSearch[i].IP))
            {
                devIP = devSearch[i].IP.ToLower();
            }
            if (string.IsNullOrEmpty(key))
            {
                if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                {
                    SeachDevName.Add(devSearch[i]);
                }
            }
            else
            {
                if (devName.ToLower().Contains(key) || devIP.ToLower().Contains(key))
                {
                    if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                    {
                        SeachDevName.Add(devSearch[i]);
                    }
                }
            }
        }
        if (SeachDevName.Count == 0)
        {
            pageNumText.text = "1";
            pageTotalText.text = "1";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(SeachDevName);
            GetPageDevData(SeachDevName);
        }

    }
    public void DeviceNamee_Click(int valu)
    {
        StartPageNum = 0;
        PageNum = 1;
        pageNumText.text = "1";
        SeachDevName.Clear();
        SaveSelection();
        string key = InputDev.text.ToString().ToLower();
        for (int i = 0; i < devSearch.Count; i++)
        {
            string devName = devSearch[i].Name.ToLower();
            string devIP = "";
            if (!string.IsNullOrEmpty(devSearch[i].IP))
            {
                devIP = devSearch[i].IP.ToLower();
            }
            if (string.IsNullOrEmpty(key))
            {
                if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                {
                    SeachDevName.Add(devSearch[i]);
                }
            }
            else
            {
                if (devName.ToLower().Contains(key) || devIP.ToLower().Contains(key))
                {
                    if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                    {
                        SeachDevName.Add(devSearch[i]);
                    }
                }
            }
        }
        if (SeachDevName.Count == 0)
        {
            pageNumText.text = "1";
            pageTotalText.text = "1";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(SeachDevName);
            GetPageDevData(SeachDevName);
        }

    }
    /// <summary>
    /// 点击定位设备
    /// </summary>
    /// <param name="devId"></param>
    public void DevBut_Click(string devId, int DepID, string devName)
    {

        DevSubsystemManage.Instance.QueryToggle.isOn = false;
        RoomFactory.Instance.FocusDev(devId, DepID, result =>
          {
              if (!result)
              {
                  string msgTitle = "找不到对应区域和设备!";
                  if (!string.IsNullOrEmpty(devName)) msgTitle = string.Format("{0} : {1}", devName, msgTitle);
                  UGUIMessageBox.Show(msgTitle);
              }
          });
        //  DeviceSearchTween.instance.ShowMinWindow(true);
        devSearchUI.SetActive(false);
        //  SearchDevInfo.Instance.SaveSelection();
        // SearchDevInfo.Instance.ExitSearchUI();
        deviceNameDropdown.devNameDropdown.captionText.text = deviceNameDropdown.devNameList[0].ToString();
        deviceNameDropdown.devNameDropdown.transform.GetComponent<Dropdown>().value = 0;
        deviceTypeDropdown.DevTypeDropdown.captionText.text = deviceTypeDropdown.devTyprList[0].ToString();
        deviceTypeDropdown.DevTypeDropdown.transform.GetComponent<Dropdown>().value = 0;
        //AlarmPushManage.Instance.IsShow.isOn = false;
        AlarmPushManage.Instance.CloseAlarmPushWindow(false);
        // DevSubsystemManage.Instance.QueryToggle.isOn = false;
        //AlarmPushManage.Instance.IsShow.isOn = false;
    }
    /// <summary>
    /// 没有筛选时的切页列表
    /// </summary>
    /// <param name="b"></param>
    public void ShowTotalPage(bool b)
    {
        if (b)
        {
            TotalPage.SetActive(true);
        }
        else
        {
            TotalPage.SetActive(false);
        }
    }

    /// <summary>
    /// 打开设备搜索界面
    /// </summary>
    public void ShowdevSearchWindow()
    {
        //RectTransform rect = this.GetComponent<RectTransform>();
        //if (rect != null)
        //{
        //    rect.localPosition = Vector3.zero;
        //}

        devSearchUI.SetActive(true);

    }
    /// <summary>
    /// 关闭设备搜索界面
    /// </summary>
    public void ClosedevSearchWindow()
    {
        deviceNameDropdown.devNameDropdown.captionText.text = deviceNameDropdown.devNameList[0].ToString();
        deviceNameDropdown.devNameDropdown.transform.GetComponent<Dropdown>().value = 0;
        deviceTypeDropdown.DevTypeDropdown.captionText.text = deviceTypeDropdown.devTyprList[0].ToString();
        deviceTypeDropdown.DevTypeDropdown.transform.GetComponent<Dropdown>().value = 0;
        devSearchUI.SetActive(false);
        DevSubsystemManage.Instance.ChangeImage(false, DevSubsystemManage.Instance.QueryToggle);
        DevSubsystemManage.Instance.QueryToggle.isOn = false;
        if (SeachDevName != null)
        {
            SeachDevName.Clear();
        }

        // SearchDevInfo.Instance.SaveSelection();
        //  SearchDevInfo.Instance.ExitSearchUI();
        // DeviceSearchTween.instance.ShowMinWindow(false);
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
    /// 设备类型
    /// </summary>
    /// <returns></returns>
    public string GetDevSeachType()
    {
        int level = DeviceTypeDropdown.Instance.DevTypeDropdown.value;
        if (level == 0) return "设备类型";
        else if (level == 1) return "基站";
        else if (level == 2) return "摄像头";
        else if (level == 3) return "门禁";
        else
        {
            return "生产设备";
        }

    }
    /// <summary>
    /// 设备类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool DeviceType(DevInfo type)
    {
        int level = DeviceTypeDropdown.Instance.DevTypeDropdown.value;
        if (level == 0) return true;
        else
        {
            if (type.TypeName == GetDevSeachType())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    /// <summary>
    /// 设备类型
    /// </summary>
    /// <returns></returns>
    public string GetDevSeachName()
    {
        int level = DeviceNameDropdown.Instance.devNameDropdown.value;
        if (level == 0) return "厂家名称";
        if (level == 1) return "艾默生";
        else
        {
            return "霍尼韦尔";
        }

    }
    /// <summary>
    /// 设备类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool DeviceName(DevInfo name)
    {
        pageNumText.text = "1";
        string Name = "";
        if (!string.IsNullOrEmpty(name.Manufactor))
        {
            Name = name.Manufactor;
        }
        int level = DeviceNameDropdown.Instance.devNameDropdown.value;
        if (level == 0) return true;    
        else
        {   
            if (Name == GetDevSeachName())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public void ScreenDeviceType(int level)
    {
        pageNumText.text = "1";
        SaveSelection();
        SeachDevName.Clear();
        for (int i = 0; i < devSearch.Count; i++)
        {
            string key = InputDev.text.ToString().ToLower();
            if (key == "")
            {
                if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                {
                    SeachDevName.Add(devSearch[i]);
                }
            }
            else
            {
                if (devName.ToLower().Contains(key) || devIP.ToLower().Contains(key))
                {
                    if (DeviceType(devSearch[i]) && DeviceName(devSearch[i]))
                    {
                        SeachDevName.Add(devSearch[i]);
                    }
                }
            }
        }
        if (SeachDevName.Count == 0)
        {
            pageNumText.text = "1";
            pageTotalText.text = "1";
            promptText.gameObject.SetActive(true);
        }
        else
        {
            promptText.gameObject.SetActive(false);
            TotaiLine(SeachDevName);
            GetPageDevData(SeachDevName);
        }


    }
}
