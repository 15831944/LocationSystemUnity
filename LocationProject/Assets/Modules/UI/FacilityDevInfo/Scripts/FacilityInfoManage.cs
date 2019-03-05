using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FacilityInfoManage : MonoBehaviour
{
    public static FacilityInfoManage Instance;
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
    /// 子系统
    /// </summary>
    public FacilitySubSystem SubSystem;
    /// <summary>
    /// 设备信息树
    /// </summary>
    public SubSytemTree DevTree;
    /// <summary>
    /// 当前生产设备信息
    /// </summary>
    private FacilitySystemList CurrentList;
    // Use this for initialization
    void Start ()
	{
	    Instance = this;
	    CloseButton.onClick.AddListener(Hide);
	}
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    int value = Random.Range(10, 15);
        //    Show(value.ToString());
        //}
    }
    /// <summary>
    /// 服务端获取信息
    /// </summary>
    private FacilitySystemList GetDevInfo(string kksCode)
    {
        FacilitySystemList List = new FacilitySystemList();
        List.DevList = new List<FacilitySystem>();
        int randomValue = Random.Range(5,15);
        for (int i = 0; i < randomValue; i++)
        {
            FacilitySystem system = new FacilitySystem();
            List.DevList.Add(system);
            system.DevName = string.Format("#{0} 设备子系统",i);
            system.Status = "";
            system.Value = "";
            system.DevKKS = "";

            system.SubDevs = new List<FacilitySystem>();
            List<FacilitySystem> subDevs = system.SubDevs;
            if (i % 2 == 0)
            {
                for(int j=0;j<7; j++)
                {
                    FacilitySystem subDev = new FacilitySystem();
                    subDev.DevName = string.Format("#{0}{1} 设备子系统,开关柜管理X-10125", i, j + 1);
                    if (j == 1||j==3||j==6) subDev.Status = "告警";
                    else subDev.Status = "/";
                    subDev.Value = "开启";
                    subDev.DevKKS = "WCC ERRCDG";
                    subDev.SubDevs = new List<FacilitySystem>();
                    List<FacilitySystem> sonDevs = subDev.SubDevs;
                    for (int k=0;k<5;k++)
                    {
                        FacilitySystem sonDev = new FacilitySystem(); ;
                        sonDev.DevName = string.Format("#{0}{1}{2} 设备子系统,开关柜管理X-10126", i, j + 1, k + 2);
                        if (k == 1 || k == 3) sonDev.Status = "告警";
                        else sonDev.Status = "/";
                        sonDev.Value = "43";
                        sonDev.DevKKS = "WCC ERRCDG";
                        sonDevs.Add(sonDev);
                    }
                    subDevs.Add(subDev);
                }
             
            }
            else
            {
                FacilitySystem subDev = new FacilitySystem();
                subDev.DevName = string.Format("#{0}{1} 设备子系统", i, i + 1);
                subDev.Status = "/";
                subDev.Value = "";
                subDev.DevKKS = "WCC ERRCDG";
                subDevs.Add(subDev);
            }
        }
        return List;
    }
    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="kksCode"></param>
    public void Show(DevInfo devInfo)
    {
        string kksCode = devInfo.KKSCode;
        if(string.IsNullOrEmpty(kksCode))
        {
            UGUIMessageBox.Show("KKS编码为空，请录入设备KKS编码!");
            return;
        }
        kksCode = "J0GCQ41";
        Dev_Monitor monitorInfo = GetDevMonitor(kksCode);
        if(monitorInfo==null)
        {
            UGUIMessageBox.Show("设备监控数据为空...");
            return;
        }
        Bg.SetActive(true);
        TitleText.text = string.Format("{0}监控信息", devInfo.Name);
        if (SubSystemItem.CurrentSelectItem != null)
        {
            SubSystemItem.CurrentSelectItem.DeselectItem();
        }
        ShowSubSystemInfo(kksCode);
        //ShowSubSystemInfo(kksCode);
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    public void Hide()
    {
        Bg.SetActive(false);
        //if (SubSystemItem.CurrentSelectItem != null)
        //{
        //    SubSystemItem.CurrentSelectItem.DeselectItem();
        //}
    }
    /// <summary>
    /// 获取设备监控信息
    /// </summary>
    /// <param name="kksCode"></param>
    /// <returns></returns>
    private Dev_Monitor GetDevMonitor(string kksCode)
    {
        CommunicationObject service = CommunicationObject.Instance;
        if(service)
        {
            Dev_Monitor monitorInfo = service.GetMonitorInfoByKKS(kksCode,true);
            return monitorInfo;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 显示子系统信息
    /// </summary>
    /// <param name="kksCode"></param>
    private void ShowSubSystemInfo(string kksCode)
    {
        CurrentList = GetDevInfo(kksCode);
        SubSystem.InitSubDevInfo(CurrentList);
    }
}
