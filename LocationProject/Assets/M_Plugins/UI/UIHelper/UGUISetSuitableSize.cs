using UnityEngine;
using System.Collections;

/// <summary>
/// 设置合适的UI界面大小
/// </summary>
public class UGUISetSuitableSize : MonoBehaviour {

    public float standardDistance;//基准位置
    public float CurrentDistance;//用于显示当前world物体距离world摄像机的距离
    private Vector3 localScaleOri;//原来的比例
    public float maxScale = 1.5f;//放大比例限制
    public float minScale = 0.5f;//缩小比例限制
    public GameObject worldObj;//UI跟随的world物体
    public Camera worldCam;//world摄像机
    // Use this for initialization
    void Start()
    {

        Init();

    }

    // Update is called once per frame
    void Update()
    {

        SetSuitableSize();

    }

    public void Init()
    {
        standardDistance = Vector3.Distance(worldObj.transform.position, worldCam.transform.position);
        localScaleOri = transform.localScale;
    }

    public void Init(Camera cam, GameObject obj)
    {
        worldCam = cam;
        worldObj = obj;
        standardDistance = Vector3.Distance(worldObj.transform.position, worldCam.transform.position);
        localScaleOri = transform.localScale;
    }

    public void Init(float distance,Camera cam,GameObject obj)
    {
        standardDistance = distance;
        worldCam = cam;
        worldObj = obj;
        localScaleOri = transform.localScale;
    }

    public void SetSuitableSize()
    {
        float dis = Vector3.Distance(worldObj.transform.position, worldCam.transform.position);
        CurrentDistance = dis;
        if (dis > standardDistance)
        {
            float scaleTemp = 1 - ((dis - standardDistance) / standardDistance);
            if (scaleTemp < minScale)
            {
                scaleTemp = minScale;
            }
            transform.localScale = new Vector3(localScaleOri.x * scaleTemp, localScaleOri.y * scaleTemp, localScaleOri.z);
        }

        if (dis < standardDistance)
        {
            float scaleTemp = 1 + (standardDistance - dis) / standardDistance;
            if (scaleTemp > maxScale)
            {
                scaleTemp = maxScale;
            }
            transform.localScale = new Vector3(localScaleOri.x * scaleTemp, localScaleOri.y * scaleTemp, localScaleOri.z);
        }
    }
}
