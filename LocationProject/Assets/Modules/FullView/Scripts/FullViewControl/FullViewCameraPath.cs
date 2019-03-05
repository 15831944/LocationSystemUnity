using Mogoson.CameraExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FullViewCameraPath : MonoBehaviour
{
    /// <summary>
    /// 主相机
    /// </summary>
    public GameObject MainCamera;
    /// <summary>
    /// 全景界面UI部分
    /// </summary>
    public GameObject FullViewCanvas;
    /// <summary>
    /// 鼠标控制
    /// </summary>
    public MouseTranslatePro MouseTranslate;
    /// <summary>
    /// 相机控制
    /// </summary>
    public AroundAlignCamera CameraControl;
    /// <summary>
    /// 工厂的中心点
    /// </summary>
    public GameObject FactoryPositon;
    [HideInInspector]
    /// <summary>
    /// 默认起始角度
    /// </summary>
    public Vector2 DefaultAngles = new Vector2(45, 0);

    [HideInInspector]
    /// <summary>
    /// 默认到（场景中心）的距离
    /// </summary>
    public float DefaultDistance = 300f;

    /// <summary>
    /// 摄像头对焦的目标
    /// </summary>
    public FullViewCameraTarget CameraTarget;
    ///// <summary>
    ///// 生活区物体
    ///// </summary>
    //public GameObject LivingQuaterObj;
    ///// <summary>
    ///// 主厂区
    ///// </summary>
    //public GameObject MainBuildingObj;

    ///// <summary>
    ///// 锅炉区
    ///// </summary>
    //public GameObject J5;
    ////public GameObject BoilerRoomObj;
    ///// <summary>
    ///// 水处理区
    ///// </summary>
    //public GameObject WaterTreatmentObj;
    ///// <summary>
    ///// 气能源区
    ///// </summary>
    //public GameObject GasEnergyObj;
    ///// <summary>
    ///// 整厂
    ///// </summary>
    //public GameObject FactoryObj;
    // Use this for initialization
    void Awake()
    {
        SceneEvents.FullViewPartChange += OnFullViewPartChange;
        SceneEvents.FullViewStateChange += OnViewChange;
        CameraControl.OnAlignEnd += CloseCameraControl;
    }
    /// <summary>
    /// 全景退出和进入
    /// </summary>
    /// <param name="isFullView"></param>
    private void OnViewChange(bool isFullView)
    {
        if(!isFullView)
        {
            ExitFullView();
        }
        else
        {
            EnterFullView();
        }
    }
    private void OnFullViewPartChange(FullViewPart part)
    {
        switch (part)
        {
            case FullViewPart.FullFactory:
                switchToFullFactory();
                break;
            case FullViewPart.LivingQuarters:
                switchToLiving();
                break;
            case FullViewPart.MainBuilding:
                swtichToMainBuilding();
                break;
            case FullViewPart.BoilerRoom:
                switchToBoilerRoom();
                break;
            case FullViewPart.WaterTreatmentArea:
                switchToWaterTreatment();
                break;
            case FullViewPart.GasEnergyArea:
                switchToGasEnergy();
                break;
            default:
                Debug.LogError("Error:UAVPhotoDisplay.OnFullViewPartChange,part not find." + part);
                break;
        }
    }
    //private void ChangeCameraTarget(GameObject target,Vector3)
    //{
    //    mouseTranslate.SetTranslatePosition(mouseTranslatePro.areaSettings.center.position);
    //    GameObject factory = GameObject.Find("PowerPlantPark");
    //    cameraControl.AlignVeiwToTarget(factory.transform, angles, distance);
    //}
    /// <summary>
    /// 退出全景模式
    /// </summary>
    private void ExitFullView()
    {
        MouseTranslate.enabled = true;
        CameraControl.enabled = true;
        MouseTranslate.areaSettings.center = FactoryPositon.transform;
        MouseTranslate.SetTranslatePosition(FactoryPositon.transform.position);
        CameraControl.AlignVeiwToTarget(MouseTranslate.transform, DefaultAngles, DefaultDistance);
        CameraControl.gameObject.SetActive(false);
        FullViewCanvas.SetActive(false);
        FactoryDepManager dep = FactoryDepManager.Instance;
        if(dep)
        {           
            dep.CreateFactoryDev();
        }
    }
    /// <summary>
    /// 进入电厂
    /// </summary>
    private void EnterFullView()
    {
        if(!FullViewCanvas.activeInHierarchy)
        {
            FullViewCanvas.SetActive(true);
        }
        //MainCamera.gameObject.SetActive(false);
        CameraControl.gameObject.SetActive(true);
    }
    /// <summary>
    /// 全景模式下，关闭相机旋转操作
    /// </summary>
    private void CloseCameraControl()
    {
        if(FullViewController.Instance.IsFullView)
        {
            CameraControl.enabled = false;
        }
    }
    #region 建模
    private Vector2 fullFactoryAngle = new Vector2(45,0);
    private float fullFactoryDis = 300f;

    private Vector2 livingQuaterAngle = new Vector2(50, 40);
    private float livingQuaterDis = 60f;

    private Vector2 mainBuildingAngle = new Vector2(40, -25f);
    private float mainBuildingDis = 100f;

    //private Vector2 bolierAngle = new Vector2(35, 15);
    //private float bolierDis = 60f;

    private Vector2 bolierAngle = new Vector2(45, 55);
    private float bolierDis = 74f;

    private Vector2 waterTreatmentAngle = new Vector2(45, 20);
    private float waterTreatmentDis = 120f;

    private Vector2 gasEnergyAngle = new Vector2(40, 36);
    private float gasEnergyPos = 28f;
    #endregion
    #region 无人机建模
    //private Vector2 livingQuaterAngle = new Vector2(28, 30);
    //private float livingQuaterDis = 2f;

    //private Vector2 mainBuildingAngle = new Vector2(50, 330);
    //private float mainBuildingDis = 1.5f;

    //private Vector2 bolierAngle = new Vector2(40, 40);
    //private float bolierDis = 1.8f;

    //private Vector2 waterTreatmentAngle = new Vector2(50, 355);
    //private float waterTreatmentDis = 2.5f;

    //private Vector2 gasEnergyAngle = new Vector2(40, 50);
    //private float gasEnergyPos = 1.6f;
    #endregion
    #region 初灵大楼
    //private Vector2 livingQuaterAngle = new Vector2(28, 30);
    //private float livingQuaterDis = 3f;

    //private Vector2 mainBuildingAngle = new Vector2(50, 330);
    //private float mainBuildingDis = 4f;

    //private Vector2 bolierAngle = new Vector2(40, 40);
    //private float bolierDis = 2.5f;

    //private Vector2 waterTreatmentAngle = new Vector2(50, 355);
    //private float waterTreatmentDis = 3.5f;

    //private Vector2 gasEnergyAngle = new Vector2(40, 50);
    //private float gasEnergyPos = 4f;
    #endregion

    private void switchToFullFactory()
    {
        MouseTranslate.enabled = false;
        CameraControl.enabled = true;
        CameraControl.AlignVeiwToTarget(CameraTarget.FactoryObj.transform, fullFactoryAngle, fullFactoryDis);
    }
    /// <summary>
    /// 切换至生活区
    /// </summary>
    private void switchToLiving()
    {
        MouseTranslate.enabled = false;
        CameraControl.enabled = true;
        CameraControl.AlignVeiwToTarget(CameraTarget.LivingQuaterObj.transform, livingQuaterAngle, livingQuaterDis);
    }
    /// <summary>
    /// 切换至主厂房
    /// </summary>
    private void swtichToMainBuilding()
    {
        MouseTranslate.enabled = false;
        CameraControl.enabled = true;
        CameraControl.AlignVeiwToTarget(CameraTarget.MainBuildingObj.transform, mainBuildingAngle, mainBuildingDis);
    }
    /// <summary>
    /// 切换至锅炉区
    /// </summary>
    private void switchToBoilerRoom()
    {
        MouseTranslate.enabled = false;
        CameraControl.enabled = true;
        CameraControl.AlignVeiwToTarget(CameraTarget.BoilerRoomObj.transform, bolierAngle, bolierDis);
    }
    /// <summary>
    /// 切换至水处理区
    /// </summary>
    private void switchToWaterTreatment()
    {
        MouseTranslate.enabled = false;
        CameraControl.enabled = true;
        CameraControl.AlignVeiwToTarget(CameraTarget.WaterTreatmentObj.transform, waterTreatmentAngle, waterTreatmentDis);
    }
    /// <summary>
    /// 切换至气能源区
    /// </summary>
    private void switchToGasEnergy()
    {
        MouseTranslate.enabled = false;
        CameraControl.enabled = true;
        CameraControl.AlignVeiwToTarget(CameraTarget.GasEnergyObj.transform, gasEnergyAngle, gasEnergyPos);
    }
    void OnDestroy()
    {
        SceneEvents.FullViewPartChange -= OnFullViewPartChange;
        SceneEvents.FullViewStateChange -= OnViewChange;
    }
}