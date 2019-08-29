using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityDevManage : MonoBehaviour {
    public static FacilityDevManage Instance;

    /// <summary>
    /// 界面
    /// </summary>
    public GameObject Bg;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button CloseButton;
    /// <summary>
    /// 标题
    /// </summary>
    public Text TitleText;
    /// <summary>
    /// 设备信息部分
    /// </summary>
    public MainMonitorInfo MainInfo;
    /// <summary>
    /// 子系统部分
    /// </summary>
    public DevSubSystem DevSubSystem;
    /// <summary>
    /// 子系统数据（树结构）
    /// </summary>
    public DevSubSystemTree SubSytemTree;

    private Transform NormalParent;//非漫游状态的父物体

    // Use this for initialization
    void Start () {
        Instance = this;
        NormalParent = transform.parent;
        CloseButton.onClick.AddListener(Hide);
    }
    /// <summary>
    /// 恢复父物体
    /// </summary>
    public void RecoverParent()
    {
        if (NormalParent == null) return;
        gameObject.transform.parent = NormalParent;
    }
    /// <summary>
    /// 设置新的父物体
    /// </summary>
    /// <param name="newParent"></param>
    public void SetNewParent(Transform newParent)
    {
        gameObject.transform.parent = newParent;
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        DevInfo dev = new DevInfo();
    //        dev.KKSCode = "1233";
    //        dev.Name = "生产设备";
    //        Show(dev);
    //    }
    //}

    private DateTime recordTime;
    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="kksCode"></param>
    public void Show(DevInfo devInfo)
    {
        string kksCode = devInfo.KKSCode;
        if (string.IsNullOrEmpty(kksCode))
        {
            UGUIMessageBox.Show("KKS编码为空，请录入设备KKS编码!");
            return;
        }
        //kksCode = "J0GCQ41";
        recordTime = DateTime.Now;
        Dev_Monitor monitorInfo = null;
#if UNITY_EDITOR
        monitorInfo = InitTestMonitor();
        Debug.LogError("Unity.Editor.启用测试数据");
#else
        monitorInfo = CommunicationObject.Instance.GetDevMonitor(kksCode);
#endif
        Debug.LogErrorFormat("GetDevMonitor,costTime:{0} s",(DateTime.Now-recordTime).TotalSeconds);
        recordTime = DateTime.Now;
        if (monitorInfo == null)
        {
            UGUIMessageBox.Show("设备监控数据为空...");
            return;
        }
        Bg.SetActive(true);
        TitleText.text = string.Format("{0}监控信息", devInfo.Name);
        MainInfo.InitMainDevInfo(monitorInfo.MonitorNodeList); //1.设备本身监控信息
        DevSubSystem.InitDevSubSystem(monitorInfo.ChildrenList);
        Debug.LogErrorFormat("InitDevSubSystem,costTime:{0} s", (DateTime.Now - recordTime).TotalSeconds);
        recordTime = DateTime.Now;
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Hide()
    {
        Bg.SetActive(false);
    }
    private Dev_Monitor InitTestMonitor()
    {
        Dev_Monitor monitor = new Dev_Monitor();
        monitor.Name = "#3机#1闭式冷却水泵";


        List<DevMonitorNode> nodeList = new List<DevMonitorNode>();
        DevMonitorNode node1 = new DevMonitorNode();
        node1.Describe = "#3机#1闭式冷却水泵电机定子线圈温度1";
        node1.Value = "25.8";
        node1.Unit = "°C";

        DevMonitorNode node2 = new DevMonitorNode();
        node2.Describe = "#3机#1闭式冷却水泵电机定子线圈温度2";
        node2.Value = "26.2";
        node2.Unit = "°C";

        nodeList.Add(node1);
        nodeList.Add(node2);
        monitor.MonitorNodeList = nodeList.ToArray();
        List<Dev_Monitor> sonList = new List<Dev_Monitor>();
        Dev_Monitor sonfirst = RandomMonitor("#3机#1闭式冷却水泵驱动端轴承1");
        Dev_Monitor sonSeond = RandomMonitor("#3机#1闭式冷却水泵非驱动端轴承2");
        sonList.Add(sonfirst);
        sonList.Add(sonSeond);
        monitor.ChildrenList = sonList.ToArray();

        List<Dev_Monitor> GrandsonList1 = new List<Dev_Monitor>();
        Dev_Monitor grandsonFirst = RandomMonitor("#3机#1闭式冷却水泵驱动端轴承3");
        Dev_Monitor grandsonSeond = RandomMonitor("#3机#1闭式冷却水泵非驱动端轴承4");
        GrandsonList1.Add(grandsonFirst);
        GrandsonList1.Add(grandsonSeond);
        sonfirst.ChildrenList = GrandsonList1.ToArray();

        List<Dev_Monitor> GrandsonList2 = new List<Dev_Monitor>();
        Dev_Monitor grandsonthird = RandomMonitor("#3机#1闭式冷却水泵驱动端轴承5");
        Dev_Monitor grandsonfotrh = RandomMonitor("#3机#1闭式冷却水泵非驱动端轴承6");
        GrandsonList2.Add(grandsonthird);
        GrandsonList2.Add(grandsonfotrh);
        sonSeond.ChildrenList = GrandsonList2.ToArray();

        List<Dev_Monitor> subSystem = new List<Dev_Monitor>();
        Dev_Monitor monitorT = new Dev_Monitor();
        monitorT.Name = "#3机#1闭式冷却水泵进口过滤器差压变送器";
        Dev_Monitor monitorT2 = new Dev_Monitor();
        monitorT2.Name = "#3机#1闭式冷却水泵进口过滤器差压变送器一次门";
        subSystem.Add(monitorT);
        subSystem.Add(monitorT2);
        grandsonFirst.ChildrenList = subSystem.ToArray();
        grandsonthird.ChildrenList = subSystem.ToArray();

        return monitor;
    }

    private Dev_Monitor RandomMonitor(string describe)
    {
        Dev_Monitor monitor = new Dev_Monitor();
        monitor.Name = describe;


        List<DevMonitorNode> nodeList = new List<DevMonitorNode>();
        DevMonitorNode node1 = new DevMonitorNode();
        node1.Describe = describe+"线圈温度1";
        node1.Value = UnityEngine.Random.Range(25.1f,35f).ToString("f2");
        node1.Unit = "°C";

        DevMonitorNode node2 = new DevMonitorNode();
        node2.Describe = describe + "线圈温度2";
        node2.Value = UnityEngine.Random.Range(25.1f, 35f).ToString("f2");
        node2.Unit = "°C";

        nodeList.Add(node1);
        nodeList.Add(node2);
        monitor.MonitorNodeList = nodeList.ToArray();

        return monitor;
    }
}
