using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using DG.Tweening;
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
   

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
             
    }
    public void CloseFPSmode()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
        }       
        if (FPSController)
        {
            FPSController.gameObject.SetActive(IsOn);
        }
        if (NoFPSUI)
        {
            NoFPSUI.SetActive(!IsOn);
        }
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
                if (item.GetComponent<MeshCollider>() || item.GetComponent<DepNode>()) continue;
                if (PlaneCollider == item || item.GetComponent<DoorAccessItem>() || item.GetComponent<SingleDoorTrigger>()||item.GetComponent<RoamBuildingCollider>()
                    ||item.GetComponent<BuildingTopCollider>()||item.GetComponentInParent<BuildingTopCollider>()) continue;
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
