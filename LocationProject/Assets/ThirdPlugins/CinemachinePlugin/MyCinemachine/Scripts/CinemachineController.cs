using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CinemachineClearShot))]
public class CinemachineController : MonoBehaviour {

    public static CinemachineController Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetFollow(Transform target,Transform lookAt=null)
    {
        Log.Info("CinemachineController.SetFollow");
        this.target = target;
        this.lookAt = lookAt;
        SetTarget();
        if (cinemachine)
        {
            cinemachine.enabled = true;
            cinemachine.gameObject.SetActive(true);
        }
    }

    public void Disable()
    {
        if (cinemachine)
        {
            cinemachine.enabled = false;
            cinemachine.gameObject.SetActive(false);
        }
    }

    public Transform target;

    public Transform lookAt;

    public CinemachineClearShot cinemachine;

    public GameObject vcamPrefab;

    void Start()
    {
        cinemachine = gameObject.GetComponent<CinemachineClearShot>();
    }

    [ContextMenu("SetTarget")]
    public void SetTarget()
    {
        if (lookAt == null)
        {
            lookAt = target;
        }
        cinemachine.Follow = target;
        cinemachine.LookAt = lookAt;
    }

    public int radius = 4;

    public float maxAngle = 360;

    [Serializable]
    public class InitInfo
    {
        public float angle = 30;

        public int count = 5;
    }

    public List<InitInfo> initList = new List<InitInfo>();

    [ContextMenu("InitVcamGroup")]
    public void InitVcamGroup()
    {
        for (int j = 0; j < initList.Count; j++)
        {
            InitInfo info = initList[j];
            var count = info.count;// / Mathf.Pow(2f,j);
            Debug.Log("count:"+count);

            var height = Mathf.Sin(info.angle * 3.14f / 180) * radius;

            var radius2= Mathf.Cos(info.angle * 3.14f / 180) * radius;

            float angle = maxAngle / count;
            for (int i = 0; i < count; i++)
            {
                var ag = angle * i+ info.angle;
                var vcamObj = GameObject.Instantiate<GameObject>(vcamPrefab);
                vcamObj.transform.parent = transform;
                vcamObj.name = "CM vcam_" + (i + 1) + "(" + ag + ")";
                vcamObj.SetActive(true);
                var vcam = vcamObj.GetComponent<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    CinemachineComponentBase body = vcam.GetCinemachineComponent(CinemachineCore.Stage.Body);
                    CinemachineTransposer transposer = body as CinemachineTransposer;
                    if (transposer == null)
                    {
                        Debug.LogError("transposer == null");
                    }
                    else
                    {

                        float x = Mathf.Sin(ag * 3.14f / 180) * radius2;
                        float z = Mathf.Cos(ag * 3.14f / 180) * radius2;
                        transposer.m_FollowOffset = new Vector3(x, height, z);
                        Debug.Log(transposer.m_FollowOffset);
                    }
                }
                else
                {
                    Debug.LogError("vcam==null");
                }
            }
        }
        
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
