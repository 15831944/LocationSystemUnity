using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGUI_3DCanvasFollow : MonoBehaviour {

    ///// <summary>
    ///// 跟随目标物体
    ///// </summary>
    //public GameObject target;
    ///// <summary>
    ///// 3DUI的位置偏移量,标志距离物体顶部的距离
    ///// </summary>
    //public Vector3 offset;
    ///// <summary>
    ///// 摄像机物体
    ///// </summary>
    //public GameObject cam;

    //// Use this for initialization
    //void Start()
    //{
    //    //SetCamera(Camera.main.gameObject);
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //private void LateUpdate()
    //{
    //    //if (target != null)
    //    //{
    //    //    if (!target.activeInHierarchy)
    //    //    {
    //    //        gameObject.SetActive(false);
    //    //    }
    //    //}

    //    SetTransform();
    //}

    ///// <summary>
    ///// 初始化
    ///// </summary>
    //public void Init(GameObject targetT, GameObject camT, Vector3 offsetT)
    //{
    //    target = targetT;
    //    cam = camT;
    //    offset = offsetT;
    //}

    ///// <summary>
    ///// 设置角度
    ///// </summary>
    //private void SetTransform()
    //{
    //    if (cam == null)
    //    {
    //        SetCamera(Camera.main.gameObject);
    //    }
    //    //transform.localPosition = target.transform.position + offset;
    //    transform.position = target.transform.position + offset;
    //    transform.forward = cam.transform.forward;
    //    //Debug.LogError("角度：" + transform.eulerAngles);
    //    float angleX = Mathf.Clamp(transform.eulerAngles.x, 0, 45);

    //    if (transform.eulerAngles.x < 0 || transform.eulerAngles.x > 45)
    //    {
    //        angleX = ClampAngle(transform.eulerAngles.x, 0, 45);
    //    }
    //    transform.rotation = Quaternion.Euler(angleX, transform.eulerAngles.y, transform.eulerAngles.z);
    //}

    //float ClampAngle(float angle, float minAngle, float maxAgnle)
    //{
    //    if (angle < 0)
    //        angle += 360;
    //    if (angle >= 360)
    //        angle -= 360;

    //    float a1 = Mathf.Abs(angle - minAngle);
    //    float a2 = Mathf.Abs(angle - 360 - minAngle);
    //    float a = a1 < a2 ? a1 : a2;
    //    float b1 = Mathf.Abs(angle - maxAgnle);
    //    float b2 = Mathf.Abs(angle - 360 - maxAgnle);
    //    float b = b1 < b2 ? b1 : b2;

    //    if (a <= b)
    //    {
    //        return minAngle;
    //    }
    //    else
    //    {
    //        return maxAgnle;
    //    }
    //}

    ///// <summary>
    ///// 设置摄像机
    ///// </summary>
    //public void SetCamera(GameObject camT)
    //{
    //    cam = camT;
    //}
}
