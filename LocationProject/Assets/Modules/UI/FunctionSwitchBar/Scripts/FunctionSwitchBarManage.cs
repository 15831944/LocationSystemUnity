using MonitorRange;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionSwitchBarManage : MonoBehaviour
{
    public static FunctionSwitchBarManage Instance;

    /// <summary>
    /// 透明
    /// </summary>
    public ToggleButton3 TransparentToggle;
    /// <summary>
    /// 告警区域
    /// </summary>
    public ToggleButton3 AlarmAreaToggle;
    /// <summary>
    /// 摄像机
    /// </summary>
    public ToggleButton3 CameraToggle;
    /// <summary>
    /// 建筑信息
    /// </summary>
    public ToggleButton3 BuildingToggle;
    /// <summary>
    /// 设备信息
    /// </summary>
    public ToggleButton3 DevInfoToggle;
    /// <summary>
    /// 基站设备信息
    /// </summary>
    public ToggleButton3 ArchorInfoToggle;
    /// <summary>
    /// 灯光
    /// </summary>
    public ToggleButton3 lightToggle;
    /// <summary>
    /// CADToggle
    /// </summary>
    public ToggleButton3 CADToggle;

    /// <summary>
    /// 显示人员定位
    /// </summary>
    public GameObject PersonnelPos;
    /// <summary>
    /// 去除建筑遮挡
    /// </summary>
    public GameObject BuildingShielding;
    /// <summary>
    /// 显示物资
    /// </summary>
    public GameObject Goods;
    /// <summary>
    /// 显示门禁
    /// </summary>
    public GameObject EnteranceGuard;
    /// <summary>
    /// 显示摄像头
    /// </summary>
    public GameObject Camera;

    /// <summary>
    /// 点击后按钮的图片
    /// </summary>
    public Sprite checkedImage;
    /// <summary>
    /// 没点击按钮的图片
    /// </summary>
    public Sprite UncheckedImage;
    /// <summary>
    /// 去除遮挡物Text
    /// </summary>
    public Text buildingText;
    public GameObject buildingImage;
    public GameObject Window;
    public void Start()
    {
        Instance = this;
        ImageColor();
        InitToggleMethod();
        SceneEvents.FullViewStateChange += OnMainPageStateChange;

        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            SetCADToggleActive(true);
        }
        else
        {
            SetCADToggleActive(false);
        }
    }
    /// <summary>
    /// 进入/退出首页
    /// </summary>
    /// <param name="isMainPage"></param>
    private void OnMainPageStateChange(bool isMainPage)
    {
        if (isMainPage)
        {
            Window.SetActive(false);
        }
        else
        {
            Window.SetActive(true);
        }
    }
    /// <summary>
    /// 初始化Toggle绑定方法
    /// </summary>
    public void InitToggleMethod()
    {
        PersonnelPos.GetComponent<Toggle>().onValueChanged.AddListener(ShowPersonnelPosition);
        BuildingShielding.GetComponent<Toggle>().onValueChanged.AddListener(ShowBuildingShielding);
        Goods.GetComponent<Toggle>().onValueChanged.AddListener(ShowGoods);
        EnteranceGuard.GetComponent<Toggle>().onValueChanged.AddListener(ShowEnteranceGuard);
        Camera.GetComponent<Toggle>().onValueChanged.AddListener(ShowCamera);

        TransparentToggle.OnValueChanged += TransparentToggle_OnValueChanged;
        CameraToggle.OnValueChanged += CameraToggle_OnValueChanged;
        DevInfoToggle.OnValueChanged += DevInfoToggle_OnValueChanged;
        ArchorInfoToggle.OnValueChanged += ArchorToggle_OnValueChanged;
        BuildingToggle.OnValueChanged += BuildingToggle_OnValueChanged;
        AlarmAreaToggle.OnValueChanged += AlarmAreaToggle_OnValueChanged;
        lightToggle.OnValueChanged += LightToggle_OnValueChanged;
        CADToggle.OnValueChanged += CADToggle_OnValueChanged;
    }

    /// <summary>
    /// 显示人员定位
    /// </summary>
    public void ShowPersonnelPosition(bool isOn)
    {
        if (isOn)
        {
            PersonnelPos.GetComponentInChildren<Image>(true).sprite = checkedImage;

            ShowBuildToggle();

        }
        else
        {
            PersonnelPos.GetComponentInChildren<Image>(true).sprite = UncheckedImage;
            CloseBuildToggle();
        }

    }
    /// <summary>
    /// 当人员定位打开时去除建筑物遮挡可以操作
    /// </summary>
    public void ShowBuildToggle()
    {
        BuildingShielding.GetComponentInChildren<Image>(true).color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 255 / 255f);
        buildingText.GetComponentInChildren<Text>(true).color = new Color(255 / 255, 255 / 25, 255 / 255, 255 / 255f);
        buildingImage.SetActive(false);
    }
    /// <summary>
    /// 关闭人员定位时去除建筑物遮挡不可操作
    /// </summary>
    public void CloseBuildToggle()
    {
        BuildingShielding.GetComponentInChildren<Image>(true).color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 102 / 255f);
        buildingText.GetComponentInChildren<Text>(true).color = new Color(255 / 255, 255 / 25, 255 / 255, 102 / 255f);
        BuildingShielding.GetComponent<Toggle>().isOn = false;
        buildingImage.SetActive(true);
    }
    /// <summary>
    /// 去除建筑遮挡
    /// </summary>
    public void ShowBuildingShielding(bool isOn)
    {
        if (isOn)
        {
            BuildingShielding.GetComponentInChildren<Image>(true).sprite = checkedImage;
            // BuildingShielding.GetComponent<Image>().sprite = checkedImage;

        }
        else
        {
            BuildingShielding.GetComponentInChildren<Image>(true).sprite = UncheckedImage;
        }

    }
    /// <summary>
    /// 显示物资
    /// </summary>
    public void ShowGoods(bool isOn)
    {
        if (isOn)
        {
            Goods.GetComponentInChildren<Image>(true).sprite = checkedImage;

        }
        else
        {
            Goods.GetComponentInChildren<Image>(true).sprite = UncheckedImage;
        }

    }
    /// <summary>
    /// 时门禁
    /// </summary>
    public void ShowEnteranceGuard(bool isOn)
    {
        if (isOn)
        {
            EnteranceGuard.GetComponentInChildren<Image>(true).sprite = checkedImage;

        }
        else
        {
            EnteranceGuard.GetComponentInChildren<Image>(true).sprite = UncheckedImage;
        }

    }
    /// <summary>
    /// 显示摄像头
    /// </summary>
    public void ShowCamera(bool isOn)
    {
        if (isOn)
        {
            Camera.GetComponentInChildren<Image>(true).sprite = checkedImage;

        }
        else
        {
            Camera.GetComponentInChildren<Image>(true).sprite = UncheckedImage;
        }

    }


    /// <summary>
    /// 改变图片颜色
    /// </summary>
    /// <param name="toggle"></param>
    public void ChangeImageColor(GameObject toggle)
    {
        EventTriggerListener color = EventTriggerListener.Get(toggle);

        color.onEnter = CheckImageColor;
        color.onExit = UncheckImageColor;
    }
    /// <summary>
    /// 点击时的颜色
    /// </summary>
    /// <param name="toggle"></param>
    public void CheckImageColor(GameObject toggle)
    {
        toggle.GetComponentInChildren<Image>(true).color = new Color(255 / 255f, 203 / 255f, 82 / 255f, 255 / 255f);
    }
    /// <summary>
    /// 未点击时的颜色
    /// </summary>
    /// <param name="toggle"></param>
    public void UncheckImageColor(GameObject toggle)
    {
        toggle.GetComponentInChildren<Image>(true).color = new Color(109 / 255f, 236 / 255f, 254 / 255f, 255 / 255f);
    }
    public void ImageColor()
    {

        ChangeImageColor(Camera);
        ChangeImageColor(EnteranceGuard);
        ChangeImageColor(Goods);
        ChangeImageColor(PersonnelPos);
        ChangeImageColor(BuildingShielding);
    }

    public void SetWindow(bool b)
    {
        ExitSetTheBarSystem();
        if (b)
        {
            Window.SetActive(true);
        }
        else
        {
            Window.SetActive(false);
        }

    }
    /// <summary>
    /// 退出子系统
    /// </summary>
    public void ExitSetTheBarSystem()
    {
        if (PersonnelPos.GetComponent<Toggle>().isOn == false)
        {
            PersonnelPos.GetComponent<Toggle>().isOn = true;
            ShowPersonnelPosition(true);
        }
        if (BuildingShielding.GetComponent<Toggle>().isOn == true)
        {
            BuildingShielding.GetComponent<Toggle>().isOn = false;
            ShowBuildingShielding(false);
        }
        if (Goods.GetComponent<Toggle>().isOn == true)
        {
            Goods.GetComponent<Toggle>().isOn = false;
            ShowGoods(false);
        }
        if (EnteranceGuard.GetComponent<Toggle>().isOn == false)
        {
            EnteranceGuard.GetComponent<Toggle>().isOn = true;
            ShowEnteranceGuard(true);
        }
        if (Camera.GetComponent<Toggle>().isOn == false)
        {
            Camera.GetComponent<Toggle>().isOn = true;
            ShowCamera(true);
        }

    }

    /// <summary>
    /// 园区透明Toggle改变触发事件
    /// </summary>
    public void TransparentToggle_OnValueChanged(bool ison)
    {
        if (ison)
        {
            LocationManager.Instance.TransparentPark();
        }
        else
        {
            LocationManager.Instance.RecoverParkMaterial();
        }
    }

    /// <summary>
    /// AlarmAreaToggle改变事件,控制告警区域是否显示
    /// </summary>
    public void AlarmAreaToggle_OnValueChanged(bool ison)
    {
        if (ison)
        {
            MonitorRangeManager.Instance.ShowAlarmArea();
        }
        else
        {
            MonitorRangeManager.Instance.HideAlarmArea();
        }
    }

    /// <summary>
    /// 是否显示建筑漂浮UI
    /// </summary>
    /// <param name="isOn"></param>
    public void BuildingToggle_OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            FollowTargetManage.Instance.ShowBuidingUI();
        }
        else
        {
            FollowTargetManage.Instance.HideBuildingUI();
        }
    }
    /// <summary>
    /// 是否显示摄像机UI
    /// </summary>
    /// <param name="isOn"></param>
    public void CameraToggle_OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            FollowTargetManage.Instance.ShowCameraUI();
        }
        else
        {
            FollowTargetManage.Instance.HideCameraUI();
        }
    }
    /// <summary>
    /// 是否显示设备信息UI
    /// </summary>
    /// <param name="isOn"></param>
    public void DevInfoToggle_OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            FollowTargetManage.Instance.ShowDevInfoUI();
        }
        else
        {
            FollowTargetManage.Instance.HideDevInfoUI();
        }
    }
    /// <summary>
    /// 是否基站信息
    /// </summary>
    /// <param name="isOn"></param>
    public void ArchorToggle_OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            FollowTargetManage.Instance.ShowArchorInfoUI();
        }
        else
        {
            FollowTargetManage.Instance.HideArchorInfoUI();
        }
    }
    /// <summary>
    /// 是否开启灯光
    /// </summary>
    /// <param name="isOn"></param>
    public void LightToggle_OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            LightManage.Instance.SetMainlight(true);
        }
        else
        {
            LightManage.Instance.SetMainlight(false);
        }
    }
    /// <summary>
    /// 是否开启灯光
    /// </summary>
    /// <param name="isOn"></param>
    public void CADToggle_OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            FactoryDepManager.Instance.ShowCAD();
        }
        else
        {
            FactoryDepManager.Instance.HideCAD();
        }
    }

    /// <summary>
    /// 设置AlarmAreaToggle的Active
    /// </summary>
    /// <param name="isActive"></param>
    public void SetCADToggleActive(bool isActive)
    {
        if (isActive)
        {

        }
        else
        {
            SetCADToggle(isActive);
        }
        CADToggle.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 设置TransparentToggle
    /// </summary>
    /// <param name="ison"></param>
    public void SetCADToggle(bool ison)
    {
        if (CADToggle.ison != ison)
        {
            CADToggle.ison = ison;
            CADToggle.SetToggle(ison);
        }
    }

    /// <summary>
    /// 设置TransparentToggle
    /// </summary>
    /// <param name="ison"></param>
    public void SetTransparentToggle(bool ison)
    {
        if (TransparentToggle.ison != ison)
        {
            TransparentToggle.ison = ison;
            TransparentToggle.SetToggle(ison);
        }
    }

    /// <summary>
    /// 设置AlarmAreaToggle
    /// </summary>
    /// <param name="ison"></param>
    public void SetAlarmAreaToggle(bool ison)
    {
        if (AlarmAreaToggle.ison != ison)
        {
            AlarmAreaToggle.ison = ison;
            AlarmAreaToggle.SetToggle(ison);
        }
    }

    /// <summary>
    /// 设置AlarmAreaToggle的Active
    /// </summary>
    /// <param name="isActive"></param>
    public void SetAlarmAreaToggleActive(bool isActive)
    {
        AlarmAreaToggle.gameObject.SetActive(isActive);
        if (isActive)
        {
            if (AlarmAreaToggle.ison)
            {
                MonitorRangeManager.Instance.ShowAlarmArea();
            }
        }
        else
        {
            MonitorRangeManager.Instance.HideAlarmArea();
        }
    }

    /// <summary>
    /// 设置TransparentToggle
    /// </summary>
    /// <param name="ison"></param>
    public void SetlightToggle(bool ison)
    {
        if (lightToggle.ison != ison)
        {
            lightToggle.ison = ison;
            lightToggle.SetToggle(ison);
        }
    }
}
