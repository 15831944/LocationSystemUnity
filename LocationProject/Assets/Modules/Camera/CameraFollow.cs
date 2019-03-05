using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    CinemachineVirtualCamera vcam;
    CinemachineFreeLook freelook;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        GameObject o = GameObject.Find("VirtualCamera");
        if (o == null)
        {
            vcam = new GameObject("VirtualCamera").AddMissingComponent<CinemachineVirtualCamera>();
        }
        vcam.m_Follow = GameObject.Find(name).transform.Find("Head");
        vcam.m_LookAt = GameObject.Find(name).transform.Find("Head");
        vcam.m_Priority = 10;
        vcam.gameObject.transform.position = new Vector3(0, 1, 0);

        var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer == null)
        {
            transposer = vcam.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
            transposer.m_FollowOffset = new Vector3(0, 20f, -10f);
        }

        //var composer = vcam.AddCinemachineComponent<CinemachineSameAsFollowObject>();
        //composer.off

        //// Install a composer.  You can install whatever CinemachineComponents you need,
        //// including your own custom-authored Cinemachine components.
        var composer = vcam.GetCinemachineComponent<CinemachineComposer>();
        if (composer == null)
        {
            composer = vcam.AddCinemachineComponent<CinemachineComposer>();
            composer.m_ScreenX = 0.5f;
            composer.m_ScreenY = 0.5f;
        }

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
}
