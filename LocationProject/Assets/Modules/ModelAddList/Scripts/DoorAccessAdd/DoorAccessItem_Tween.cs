using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DoorAccessItem_Tween : MonoBehaviour {
    /// <summary>
    /// 是否单门
    /// </summary>
    public bool IsSingleDoor;
    /// <summary>
    /// 单门
    /// </summary>
    private GameObject singleDoor;
    /// <summary>
    /// 双开门
    /// </summary>
    private GameObject leftDoor;
    private GameObject rightDoor;

    /// <summary>
    /// 单门
    /// </summary>
    private Vector3 _singleDoorAngle = new Vector3(0, -270, 0);
    /// <summary>
    /// 双门旋转角度
    /// </summary>
    private Vector3 _rightDoorAngle = new Vector3(0, -90, 0);
    private Vector3 _leftDoorAngle = new Vector3(0, 90, 0);

    /// <summary>
    /// 动画时间 
    /// </summary>
    private float TweenTime = 1.5f;
    // Use this for initialization
    void Start () {
		
	}
    /// <summary>
    /// 打开门（世界坐标）
    /// </summary>
    /// <param name="firstDoorAngle"></param>
    /// <param name="SecondDoorAngle"></param>
    public void OpenDoorInWorldAngle(Vector3 firstDoorAngle,Vector3 SecondDoorAngle)
    {
        if(IsSingleDoor)
        {
            
        }
    }
    /// <summary>
    /// 设置门动画信息
    /// </summary>
    /// <param name="isSingle"></param>
    /// <param name="firstDoor"></param>
    /// <param name="secondDoor"></param>
    public void SetDoor(bool isSingle,GameObject firstDoor,GameObject secondDoor)
    {
        IsSingleDoor = isSingle;
        if(isSingle)
        {
            singleDoor = firstDoor;
        }
        else
        {
            leftDoor = firstDoor;
            rightDoor = secondDoor;
        }
    }
    /// <summary>
    /// 开门
    /// </summary>
    /// <param name="action"></param>
    public void OpenDoor(Action action)
    {
        if(IsSingleDoor)
        {
            TweenSingleDoor(_singleDoorAngle,action);
        }
        else
        {
            TweenDoubleDoorAngle(_leftDoorAngle,_rightDoorAngle,action);
        }
    }
    /// <summary>
    /// 关门
    /// </summary>
    /// <param name="isImediately"></param>
    /// <param name="onComplete"></param>
    public void CloseDoor(bool isImediately=false,Action onComplete=null)
    {
        if(isImediately)
        {
            if(IsSingleDoor)
            {
                KillTween(singleDoor);
                singleDoor.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                KillTween(leftDoor);
                KillTween(rightDoor);
                leftDoor.transform.localEulerAngles = Vector3.zero;
                rightDoor.transform.localEulerAngles = Vector3.zero;
            }
            if (onComplete != null) onComplete();
        }
        else
        {
            if (IsSingleDoor)
            {
                TweenSingleDoor(Vector3.zero, onComplete);
            }
            else
            {
                TweenDoubleDoorAngle(Vector3.zero, Vector3.zero, onComplete);
            }
        }
    }
    /// <summary>
    /// 删除动画
    /// </summary>
    /// <param name="obj"></param>
    private void KillTween(GameObject obj)
    {
        if (obj != null) obj.transform.DOKill();
    }
    /// <summary>
    /// 单门动画
    /// </summary>
    /// <param name="targetAngle"></param>
    /// <param name="onComplete"></param>
    private void TweenSingleDoor(Vector3 targetAngle,Action onComplete)
    {
        if (singleDoor == null)
        {
            Debug.LogError("Door is null");
            if (onComplete != null) onComplete();
        }
        else
        {
            singleDoor.transform.DOLocalRotate(targetAngle, TweenTime).OnComplete(() =>
            {
                if (onComplete != null) onComplete();
            });
        }
    }
    /// <summary>
    /// 双门动画
    /// </summary>
    /// <param name="leftAngle"></param>
    /// <param name="rightAngle"></param>
    /// <param name="onComplete"></param>
    private void TweenDoubleDoorAngle(Vector3 leftAngle,Vector3 rightAngle,Action onComplete)
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("Door is null");
            if (onComplete != null) onComplete();
        }
        else
        {
            leftDoor.transform.DOLocalRotate(leftAngle, TweenTime);
            rightDoor.transform.DOLocalRotate(rightAngle, TweenTime).OnComplete(() =>
            {
                if (onComplete != null) onComplete();
            });
        }
    }
}
