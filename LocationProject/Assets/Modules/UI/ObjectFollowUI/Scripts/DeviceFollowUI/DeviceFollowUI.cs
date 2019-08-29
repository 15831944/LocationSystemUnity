using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Location.WCFServiceReferences.LocationServices;

public class DeviceFollowUI : MonoBehaviour {
    /// <summary>
    /// 当前展开弹窗的UI
    /// </summary>
    public static DeviceFollowUI CurrentMonitor;

    /// <summary>
    /// 视频监控按钮
    /// </summary>
    public Button DevInfoButton;
    /// <summary>
    /// 打开界面按钮
    /// </summary>
    public Toggle BgToggle;

    /// <summary>
    /// 弹窗
    /// </summary>
    public GameObject InfoBg;
    /// <summary>
    /// 标题文本
    /// </summary>
    public Text TitleText;
    /// <summary>
    /// 信息文本
    /// </summary>
    public Text InfoText;

    private DevNode devNode;


    /// <summary>
    /// 温度信息模块
    /// </summary>
    public DeviceTemperatureInfo TemperatureInfo;
    private bool isInit=false;
    // Use this for initialization
    void Start()
    {

    }
    private void Update()
    {
        HideUIByRaycast();
    }

    /// <summary>
    /// 通过检测点击，关闭UI 
    /// </summary>
    private void HideUIByRaycast()
    {
        if (gameObject.activeInHierarchy)
        {
            if (IsClickUGUIorNGUI.Instance&&IsClickUGUIorNGUI.Instance.isOverUI) return;
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray,out hit,float.MaxValue))
                {
                    FacilityDevController dev = hit.transform.GetComponent<FacilityDevController>();
                    if(dev==null&&CurrentMonitor!=null)
                    {
                        CurrentMonitor.CloseUI();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    public void ShowUI()
    {
        if (CurrentMonitor != null && CurrentMonitor != this)
        {
            CurrentMonitor.CloseUI();
        }
        CurrentMonitor = this;
        BgToggle.isOn = true;
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 隐藏界面
    /// </summary>
    public void CloseUI()
    {
        BgToggle.isOn = false;
        gameObject.SetActive(false);
        CurrentMonitor = null;
        if (devNode != null) devNode.HighLightOff();
    }
    /// <summary>
    /// 按钮绑定方法
    /// </summary>
    private void InitMethod()
    {
        CloseUI();
        if (isInit) return;
        isInit = true;
        DevInfoButton.onClick.AddListener(ShowMonitor);
        BgToggle.onValueChanged.AddListener(ShowBg);
    }
    /// <summary>
    /// 显示/关闭 窗体
    /// </summary>
    private void ShowBg(bool isOn)
    {
        if (isOn) Show();
        else Hide();
    }
    /// <summary>
    /// 显示背景
    /// </summary>
    private void Show()
    {       
        devNode.HighlightOn();
        InfoBg.SetActive(true);
        var info = devNode.Info;
        if (info == null)
        {
            Log.Error("DeviceFollowUI.Show", "info == null");
            return;
        }
        if (TitleText.text != info.Name) TitleText.text = info.Name;
    }
    /// <summary>
    /// 关闭背景
    /// </summary>
    private void Hide()
    {              
        InfoBg.SetActive(false);
    }
    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="devInfo"></param>
    public void SetInfo(DevNode devNode)
    {
        InitMethod();
        this.devNode = devNode;

        if (devNode == null)
        {
            Log.Error("DeviceFollowUI.SetInfo", "devNode == null");
            return;
        }

        DevInfo devInfo = devNode.Info;
        if (devInfo == null)
        {
            Log.Error("DeviceFollowUI.SetInfo", "devInfo == null");
            return;
        }
        TitleText.text = devInfo.Name;
        string info = "";
        if (devInfo.ParentId != null&&RoomFactory.Instance)
        {
            DepNode node = RoomFactory.Instance.GetDepNodeById((int)devInfo.ParentId);
            //if (node != null) info = node.NodeName+ "/";
            if (node != null) info = node.NodeName;
        }
        //info += devInfo.KKSCode;
        if(string.IsNullOrEmpty(info)) info = "四会热电厂";
        InfoText.text = info;
        TemperatureInfo.Init();
    }
    /// <summary>
    /// 显示监控信息
    /// </summary>
    private void ShowMonitor()
    {
        FacilityDevManage manager = FacilityDevManage.Instance;
        if(manager)
        {
            manager.Show(devNode.Info);
        }
        //FacilityInfoManage manager = FacilityInfoManage.Instance;
        //if (manager)
        //{
        //    manager.Show(devNode.Info);
        //}
    }
}
