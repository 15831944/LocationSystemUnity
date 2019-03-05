using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManage : MonoBehaviour
{

    ///// <summary>
    ///// 和主摄像机保持一致的虚拟摄像机
    ///// </summary>
    //public CinemachineVirtualCamera mainVirtualCamera;

    /// <summary>
    /// 主摄像机
    /// </summary>
    public Camera maincamera;

    /// <summary>
    /// 跟随虚拟摄像机
    /// </summary>
    public CinemachineVirtualCamera followVirtualCamera;

    /// <summary>
    /// 观察组虚拟摄像机
    /// </summary>
    public CinemachineVirtualCamera groupVirtualCamera;

    /// <summary>
    /// 观察组
    /// </summary>
    public CinemachineTargetGroup cinemachineTargetGroup;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 关闭所有虚拟摄像机
    /// </summary>
    public void HideAll()
    {
        //mainVirtualCamera.gameObject.SetActive(false);
        followVirtualCamera.gameObject.SetActive(false);
        groupVirtualCamera.gameObject.SetActive(false);
    }

    ///// <summary>
    ///// 开启主虚拟摄像机
    ///// </summary>
    //public void Enable_MainVirtualCamera()
    //{
    //    //mainVirtualCamera.gameObject.SetActive(true);
    //    followVirtualCamera.gameObject.SetActive(false);
    //    groupVirtualCamera.gameObject.SetActive(false);
    //}

    /// <summary>
    /// 开启跟随虚拟摄像机
    /// </summary>
    public void Enable_FollowVirtualCamera()
    {
        //mainVirtualCamera.gameObject.SetActive(false);
        followVirtualCamera.gameObject.SetActive(true);
        groupVirtualCamera.gameObject.SetActive(false);
    }


    /// <summary>
    /// 开启组虚拟摄像机
    /// </summary>
    public void Enable_GroupVirtualCamera()
    {
        //mainVirtualCamera.gameObject.SetActive(false);
        followVirtualCamera.gameObject.SetActive(false);
        groupVirtualCamera.gameObject.SetActive(true);
    }

    public void SetFollow1()
    {
        SetFollow("CapsulePoints");
    }

    public void SetFollow2()
    {
        SetFollow("标签10002");
    }

    public void SetFollowHistoryPath0002()
    {
        SetFollow("HistoryPath0002");
    }

    public void SetFollowHistoryPath0003()
    {
        SetFollow("HistoryPath0003");
    }

    public void SetFollowHistoryPathObj()
    {
        SetFollow("HistoryPathObj");
    }

    public void SetGroupLookTest01()
    {
        List<GameObject> groups = new List<GameObject>();
        groups.Add(GameObject.Find("CapsulePoints"));
        groups.Add(GameObject.Find("标签10002"));
        //groups.Add(GameObject.Find("CapsulePoints"));
        SetGroupLook(groups);
    }


    /// <summary>
    /// 设置摄像机跟随物体
    /// </summary>
    public void SetFollow(string name)
    {
        // Create a Cinemachine brain on the main camera
        var brain = GameObject.Find("Main Camera").AddMissingComponent<CinemachineBrain>();
        brain.m_ShowDebugText = true;
        brain.m_DefaultBlend.m_Time = 1;

        // Create a virtual camera that looks at object "Cube", and set some settings
        //GameObject o = GameObject.Find("VirtualCamera");
        //if (o == null)
        //{
        //followVirtualCamera = new GameObject("VirtualCamera").AddMissingComponent<CinemachineVirtualCamera>();
        //}
        followVirtualCamera.m_Follow = GameObject.Find(name).transform.Find("Head");
        followVirtualCamera.m_LookAt = GameObject.Find(name).transform.Find("Head");
        followVirtualCamera.m_Priority = 10;
        followVirtualCamera.gameObject.transform.position = new Vector3(0, 1, 0);

        SetfollowCameraOffset();

        //var composer = vcam.AddCinemachineComponent<CinemachineSameAsFollowObject>();
        //composer.off

        //// Install a composer.  You can install whatever CinemachineComponents you need,
        //// including your own custom-authored Cinemachine components.
        var composer = followVirtualCamera.GetCinemachineComponent<CinemachineComposer>();
        if (composer == null)
        {
            composer = followVirtualCamera.AddCinemachineComponent<CinemachineComposer>();
            composer.m_ScreenX = 0.5f;
            composer.m_ScreenY = 0.5f;
        }

        Enable_FollowVirtualCamera();

        //// Create a FreeLook vcam on object "Cylinder"
        //freelook = new GameObject("FreeLook").AddComponent<CinemachineFreeLook>();
        //freelook.m_LookAt = GameObject.Find("Cylinder/Sphere").transform;
        //freelook.m_Follow = GameObject.Find("Cylinder").transform;
        //freelook.m_Priority = 11;

        //// You can access the individual rigs in the freeLook if you want.
        //// FreeLook rigs come with Composers pre-installed.
        //// Note: Body MUST be Orbital Transposer.  Don't change it.
        //CinemachineVirtualCamera topRig = freelook.GetRig(0);
        //CinemachineVirtualCamera middleRig = freelook.GetRig(1);
        //CinemachineVirtualCamera bottomRig = freelook.GetRig(2);
        //topRig.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.35f;
        //middleRig.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.25f;
        //bottomRig.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = 0.15f;
    }

    /// <summary>
    /// 设置跟随虚拟相机的偏移量
    /// </summary>
    private void SetfollowCameraOffset()
    {
        var transposer = followVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer = followVirtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
            transposer.m_FollowOffset = new Vector3(SystemSettingHelper.cinemachineSetting.CMvcamFollow_X, SystemSettingHelper.cinemachineSetting.CMvcamFollow_Y, SystemSettingHelper.cinemachineSetting.CMvcamFollow_Z);
        }
    }

    /// <summary>
    /// 保存虚拟相机的偏移量
    /// </summary>
    [ContextMenu("SavefollowCameraOffset")]
    public void SavefollowCameraOffset()
    {
        var transposer = followVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer = followVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            SystemSettingHelper.GetSystemSetting();
            SystemSettingHelper.cinemachineSetting.CMvcamFollow_X = transposer.m_FollowOffset.x;
            SystemSettingHelper.cinemachineSetting.CMvcamFollow_Y = transposer.m_FollowOffset.y;
            SystemSettingHelper.cinemachineSetting.CMvcamFollow_Z = transposer.m_FollowOffset.z;
            SystemSettingHelper.SaveSystemSetting();
        }
    }

    /// <summary>
    /// 设置观察组
    /// </summary>
    public void SetGroupLook(List<GameObject> groups)
    {
        groupVirtualCamera.m_LookAt = GameObject.Find("TargetGroup").transform;
        Enable_GroupVirtualCamera();
        SetTargetGroup(groups);
    }

    /// <summary>
    /// 组物体对象集合
    /// </summary>
    /// <param name="groups"></param>
    public void SetTargetGroup(List<GameObject> groups)
    {
        List<CinemachineTargetGroup.Target> targets = new List<CinemachineTargetGroup.Target>();
        foreach (GameObject o in groups)
        {
            CinemachineTargetGroup.Target t = new CinemachineTargetGroup.Target();
            t.target = o.transform;
            t.weight = 1;
            t.radius = 1;
            targets.Add(t);
        }

        cinemachineTargetGroup.m_Targets = targets.ToArray();
    }
}
