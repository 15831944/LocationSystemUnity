using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BuildingFloorTweening : MonoBehaviour {
    public List<GameObject> FloorList;
    /// <summary>
    /// 展开的楼层间距
    /// </summary>
    public float OffsetPerFloor=10;
    /// <summary>
    /// 展开楼层，所需要的动画时间
    /// </summary>
    public float TweenTime = 0.8f;
	// Use this for initialization
	void Start () {
        InitSequence();
    }
    /// <summary>
    /// 楼层展开动画
    /// </summary>
    private Sequence FloorSequnce;
	private void InitSequence()
    {
        if(FloorList.Count==0)
        {
            Debug.LogError("Floor is null:"+transform.name);
        }
        else
        {
            FloorSequnce = DOTween.Sequence();
            for(int i=0;i<FloorList.Count;i++)
            {
                Transform trans = FloorList[i].transform;
                float YTemp = trans.localPosition.y + i * OffsetPerFloor;
                Tween t = FloorList[i].transform.DOLocalMoveY(YTemp, TweenTime);
                if (i == 0) FloorSequnce.Append(t);
                else FloorSequnce.Join(t);
            }
            FloorSequnce.SetAutoKill(false);
            FloorSequnce.Pause();
        }
    }
    public void OpenBuilding(bool isImmediately=false,Action onOpenComplete=null)
    {
        FloorSequnce.OnComplete(() =>
        {
            if (onOpenComplete != null) onOpenComplete();
        }).Restart(isImmediately);
    }
    public void CloseBuilding(bool isImmediately=false, Action onCloseComplete=null)
    {
        if(isImmediately)
        {
            FloorSequnce.OnRewind(() =>
            {
                if (onCloseComplete != null) onCloseComplete();
            }).Rewind();
        }
        else
        {
            FloorSequnce.OnRewind(() =>
            {
                if (onCloseComplete != null) onCloseComplete();
            }).PlayBackwards();
        }       
    }
}
