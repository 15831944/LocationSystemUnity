using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using DG.Tweening;
using Jacovone.AssetBundleMagic;

public class FPSMode : MonoBehaviour {
    public static FPSMode Instance;
    public GameObject FPSobj;

    public FirstPersonController FPSController;

    public GameObject NoFPSUI;

  //  public GameObject FPSUI;

    public bool IsOn;

    public Camera[] cameras;

    public Collider[] colliders;

    public Collider PlaneCollider;

    public Action AfterExitFPS;
    public GameObject cube;//边界

    [Tooltip("准心图片")]
    public GameObject CenterImage;
   

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        SetFPSControl();
    }
    /// <summary>
    /// 设置漫游控制方式
    /// </summary>
    private void SetFPSControl()
    {
        bool isRemote = SystemSettingHelper.debugSetting.IsRemoteMode;
        FPSController.SetRemoteState(isRemote);       
    }
    public void CloseFPSmode()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(LoadingIndicatorScript.Instance) LoadingIndicatorScript.Instance.IsBuildingAndIsDev();
            SwitchTo(false);
            if (DevSubsystemManage.Instance) DevSubsystemManage.Instance.RoamToggle.isOn = false;
        }
    }

    public void SwitchTo(bool isOn)
    {
        IsOn = isOn;
        if (isOn)
        {
            cameras = GameObject.FindObjectsOfType<Camera>();
            HideCameras(IsOn);
        }
        else
        {
            HideCameras(IsOn);
            if (AfterExitFPS != null)
            {
                AfterExitFPS();
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            RecoverMonitorUI();
        }       
        if (FPSController)
        {
            FPSController.gameObject.SetActive(IsOn);
            if(IsOn)FPSController.ResetCursorState();
        }
        if (NoFPSUI)
        {
            NoFPSUI.SetActive(!IsOn);
        }

        if (SceneAssetManager.Instance)
        {
            SceneAssetManager.Instance.isRoam = isOn;
            if (isOn)
            {
                SceneAssetManager.Instance.subject = FPSController.transform;
            }
            else
            {
                SceneAssetManager.Instance.subject = null;
            }
            //SceneAssetManager.Instance.EnableRoamLoad = isOn;
        }
        SetRoamFollowUI(isOn);
    }
    /// <summary>
    /// 设置漫游漂浮UI
    /// </summary>
    /// <param name="isShow"></param>
    public void SetRoamFollowUI(bool isShow)
    {
        RoamFollowMange followManager = RoamFollowMange.Instance;
        if (followManager == null) return;
        if(isShow)
        {
            followManager.ShowRoamCameraUI();
        }
        else
        {
            followManager.CloseCameraUI();
        }
    }

    /// <summary>
    /// 把普通模式移动过来的UI，还原会原来的Canvas
    /// </summary>
    private void RecoverMonitorUI()
    {
        if(FacilityDevManage.Instance)
        {
            FacilityDevManage.Instance.RecoverParent();
            FacilityDevManage.Instance.Hide();
        }
        if(CameraVideoRtsp.Instance)
        {
            CameraVideoRtsp.Instance.RecoverParent();
            CameraVideoRtsp.Instance.Close();
        }
        if(FollowTargetManage.Instance)
        {
            FollowTargetManage.Instance.CloseDepCameraUI();
        }
        //if(CameraVideoManage.Instance)
        //{
        //    CameraVideoManage.Instance.RecoverParent();
        //    CameraVideoManage.Instance.Close();
        //}
    }

    List<Collider> colliderList = new List<Collider>();
    /// <summary>
    /// 设置Collider状态
    /// </summary>
    /// <param name="colliders"></param>
    /// <param name="isOn"></param>
    public  void SetColliderState(bool ison)
    {
        if (ison)
        {
            colliders = GameObject.FindObjectsOfType<Collider>();
            foreach (Collider item in colliders)
            {
                if (item == null) continue;
                if (item.transform == null) continue;

                if (item.GetComponent<MeshCollider>() || item.GetComponent<DepNode>()) continue;
                if (PlaneCollider == item || item.GetComponent<DoorAccessItem>() || item.GetComponent<SingleDoorTrigger>()||item.GetComponent<RoamBuildingCollider>()
                    ||item.GetComponent<BuildingTopCollider>()||item.GetComponentInParent<BuildingTopCollider>()||item.GetComponent<DevNode>()!=null) continue;
                if (item.enabled == true)
                {
                    colliderList.Add(item);
                    item.enabled = !ison;//隐藏其他碰撞体
                }

            }
        }
        else
        {
            foreach (Collider obj in colliderList)
            {
                if (obj == null) continue;
                if (obj.transform == null) continue;
                obj.enabled = true;
            }

        }
        
     
    }
    public void HideCameras(bool b)
    {
        foreach (Camera item in cameras)
        {
            item.enabled = !b;//隐藏其他摄像头
        }
    }
    /// <summary>
    /// 设置第一人称边界
    /// </summary>
    /// <param name="b"></param>
    public void SetBorder(bool b) 
    {
        if (b)
        {
            cube.SetActive(true);
        }
        else
        {
            cube.SetActive(false);
        }
    }

}
