using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartOutManage : MonoBehaviour {
    public static StartOutManage Instance;
    /// <summary>
    /// 界面
    /// </summary>
    public GameObject Window;
    /// <summary>
    /// 退出程序按钮
    /// </summary>
    public Button ExitButton;
    /// <summary>
    /// 返回首页按钮
    /// </summary>
    public Button MainPageButton;
    /// <summary>
    /// 上一层按钮
    /// </summary>
    public Button UpperStoryButton;
    /// <summary>
    /// 返回按钮
    /// </summary>
    public Button BackButton;
    /// <summary>
    /// 返回回调
    /// </summary>
    public Action BackButtonCall;

    public Button ExitDevEditButton;//退出设备编辑模式


    // Use this for initialization
    void Start () {
        Instance = this;
        ExitButton.onClick.AddListener(OnExitButtonClick);
        MainPageButton.onClick.AddListener(OnMainPageButtonClick);
        UpperStoryButton.onClick.AddListener(OnUpperStoryButtonClick);
        BackButton.onClick.AddListener(OnBackButtonClick);
        ExitDevEditButton.onClick.AddListener(HideDevEditButton);
        SceneEvents.DepNodeChanged+=OnDepNodeChanged;
        SceneEvents.FullViewStateChange += OnFullVeiwStateChange;

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.LogError("Escape click...");
            EscToBack();
        }
    }
    private void OnFullVeiwStateChange(bool isFullView)
    {
        //不显示首页情况下，隐藏返回首页按钮
        if(!isFullView&&!SystemSettingHelper.systemSetting.IsShowHomePage)
        {
            MainPageButton.gameObject.SetActive(false);
        }
    }
    private void EscToBack()
    {
        if(CameraSceneManager.Instance.alignCamera.IsAligning)
        {
            Debug.Log("AlignCamera.IsAligning,wait for next input...");
            return;
        }
        if(ExitDevEditButton.gameObject.activeInHierarchy&&ObjectAddListManage.IsEditMode)
        {
            HideDevEditButton();
        }
        else if(BackButtonCall!=null&&BackButton.gameObject.activeInHierarchy)
        {
            OnBackButtonClick();
        }else if(UpperStoryButton.gameObject.activeInHierarchy)
        {
            OnUpperStoryButtonClick();
        }
    }
    /// <summary>
    /// 区域切换事件
    /// </summary>
    private void OnDepNodeChanged(DepNode last,DepNode current)
    {
        if (current as FactoryDepManager)
        {
            //BackButton.gameObject.SetActive(false);
            SetUpperStoryButtonActive(false);
        }
        else
        {
            //BackButton.gameObject.SetActive(true);
            if (LocationManager.Instance.currentLocationFocusObj != null) return;
            SetUpperStoryButtonActive(true);
        }
    }
    /// <summary>
    /// 显示退出和首页按钮
    /// </summary>
    public void Show()
    {
        Window.SetActive(true);
    }
    /// <summary>
    /// 关闭退出和首页按钮
    /// </summary>
    public void Hide()
    {
        Window.SetActive(false);
    }
    /// <summary>
    /// 退出程序
    /// </summary>
    private  void OnExitButtonClick()
    {
        UGUIMessageBox.Show("是否确定退出软件？", () =>
         {
             Log.Info("StartoutManage.Application.Quit->退出程序");
             Application.Quit();
         }, () =>
         {

         });
        //Application.Quit();
    }
    /// <summary>
    /// 返回首页
    /// </summary>
    private void OnMainPageButtonClick()
    {
        FullViewController.Instance.SwitchToFullView();
        if (ConfigButton.instance) ConfigButton.instance.ChoseConfigView();//关闭打开的配置界面
    }
    /// <summary>
    /// 返回上一层按钮
    /// </summary>
    private void OnUpperStoryButtonClick()
    {
        DepNode parentNode = FactoryDepManager.currentDep.ParentNode;
        if (parentNode as DepController)
        {
            parentNode = parentNode.ParentNode;
        }
        if (parentNode != null)
        {
            RoomFactory.Instance.FocusNode(parentNode);
        }
    }

    /// <summary>
    /// 设置UpperStoryButton
    /// </summary>
    public void SetUpperStoryButtonActive(bool isActive)
    {
        UpperStoryButton.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    private void OnBackButtonClick()
    {
        if (BackButtonCall != null)
        {
            BackButtonCall();
            BackButtonCall = null;
        }
    }


    /// <summary>
    /// 显示BackButton
    /// </summary>
    public void ShowBackButton(Action callT)
    {
        BackButtonCall = callT;
        BackButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void HideBackButton()
    {
        BackButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示退出设备编辑按钮
    /// </summary>
    public void ShowDevEditButton()
    {
        ExitDevEditButton.gameObject.SetActive(true);
    }
    /// <summary>
    /// 退出设备编辑模式
    /// </summary>
    public void HideDevEditButton()
    {
        ExitDevEditButton.gameObject.SetActive(false);
        DevSubsystemManage.Instance.DevEditorToggle.isOn = false;
        ActionBarManage.Instance.Show();
    }
    /// <summary>
    /// 设置首页和返回按钮状态
    /// </summary>
    /// <param name="isOn"></param>
    public void SetMainPageAndBackState(bool isOn)
    {
        ExitButton.gameObject.SetActive(isOn);
        bool isMainPageButonOn = SystemSettingHelper.systemSetting.IsShowHomePage && isOn ? true : false;
        MainPageButton.gameObject.SetActive(isMainPageButonOn);
    }
}
