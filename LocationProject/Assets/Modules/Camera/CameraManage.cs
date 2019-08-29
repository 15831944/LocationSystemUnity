using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManage : MonoBehaviour {

    public AroundAlignCamera aroundAlignCamera;
    /// <summary>
    /// 场景移动控制
    /// </summary>
    public MouseTranslatePro mouseTranslatePro;
    /// <summary>
    /// 场景中心
    /// </summary>
    public Transform Center;
    /// <summary>
    /// 默认起始角度
    /// </summary>
    public Vector2 angles=new Vector2(30,0);
    /// <summary>
    /// 默认到Center（场景中心）的距离
    /// </summary>
    public float distance = 10;
    ///// <summary>
    ///// 居中抑制系数
    ///// </summary>
    //public float middleDamper = 5;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            mouseTranslatePro.SetTranslatePosition(mouseTranslatePro.areaSettings.GetPos());
            aroundAlignCamera.AlignVeiwToTarget(aroundAlignCamera.GetTarget(), angles, distance);
            //aroundAlignCamera.CurrentAngles = angles;
            //aroundAlignCamera.
            //CurrentOffset = Vector3.Lerp(CurrentOffset, targetOffset, damper * Time.deltaTime);
            //aroundAlignCamera.target.position = Vector3.Lerp(aroundAlignCamera.target.position, Center.position, middleDamper * Time.deltaTime);
            //aroundAlignCamera.target.position = Center.position;
        }
    }
}
