using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BuildingFloorTweening : MonoBehaviour {
    public List<GameObject> FloorList = new List<GameObject>();
    public List<GameObject> HideFloors = new List<GameObject>();
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

    public bool IsAllNull()
    {
        bool isAllNull = true;
        if (FloorList.Count > 0)
        {
            foreach (var o in FloorList)
            {
                if (o != null)
                {
                    isAllNull = false;
                }
            }
        }
        return isAllNull;
    }

	public void InitSequence()
    {
        if(FloorList.Count==0)
        {
            Debug.LogError("Floor count is 0:"+transform.name);
        }
        else if(IsAllNull())
        {
            Debug.LogError("Floor is All null:" + transform.name);
        }
        else
        {
            Debug.Log("InitSequence");
            FloorSequnce = DOTween.Sequence();
            for(int i=0;i<FloorList.Count;i++)
            {
                var floor = FloorList[i];
                if (floor == null) continue;
                Transform trans = floor.transform;
                float YTemp = trans.localPosition.y + i * OffsetPerFloor;
                Tween t = floor.transform.DOLocalMoveY(YTemp, TweenTime);
                if (i == 0) FloorSequnce.Append(t);
                else FloorSequnce.Join(t);
            }
            FloorSequnce.SetAutoKill(false);
            FloorSequnce.Pause();
        }
    }
    /// <summary>
    /// 获取楼层展开的高度
    /// </summary>
    /// <param name="floor"></param>
    /// <returns></returns>
    public float GetFloorExpandDistance(GameObject floor)
    {
        if (FloorList == null || FloorList.Count == 0) return 0;
        for(int i=0;i<FloorList.Count;i++)
        {
            if(FloorList[i].gameObject==floor)
            {
                return i * OffsetPerFloor;
            }
        }
        return 0;
    }
    /// <summary>
    /// 在Editor下展开
    /// </summary>
    [ContextMenu("Expand")]
    public void ExpandInEditor()
    {
        for (int i = 0; i < FloorList.Count; i++)
        {
            var floor = FloorList[i];
            if (floor == null) continue;
            Transform trans = floor.transform;
            var lp = trans.localPosition;
            float YTemp = lp.y + i * OffsetPerFloor;
            trans.localPosition = new Vector3(lp.x, YTemp, lp.z);
        }
    }

    /// <summary>
    /// 在Editor下展开
    /// </summary>
    [ContextMenu("Collapse")]
    public void CollapseInEditor()
    {
        for (int i = 0; i < FloorList.Count; i++)
        {
            var floor = FloorList[i];
            if (floor == null) continue;
            Transform trans = floor.transform;
            var lp = trans.localPosition;
            float YTemp = lp.y - i * OffsetPerFloor;
            trans.localPosition = new Vector3(lp.x, YTemp, lp.z);
        }
    }

    private LightShadows shadows;

    public void OpenBuilding(bool isImmediately=false,Action onOpenComplete=null)
    {
        Debug.Log("OpenBuilding");
        Light light = GameObject.FindObjectOfType<Light>();
        if (LightShadows.Soft == light.shadows)
        {
            shadows = light.shadows;
            light.shadows = LightShadows.None;
        }

        foreach (var item in HideFloors)
        {
            item.gameObject.SetActive(false);
        }

        FloorSequnce.OnComplete(() =>
        {
            if (onOpenComplete != null) onOpenComplete();
        }).Restart(isImmediately);
    }
    public void CloseBuilding(bool isImmediately=false, Action onCloseComplete=null)
    {
        
        if (isImmediately)
        {
            FloorSequnce.OnRewind(() =>
            {
                Light light = GameObject.FindObjectOfType<Light>();
                light.shadows = shadows;

                foreach (var item in HideFloors)
                {
                    item.gameObject.SetActive(true);
                }

                if (onCloseComplete != null) onCloseComplete();
            }).Rewind();
        }
        else
        {
            FloorSequnce.OnRewind(() =>
            {
                Light light = GameObject.FindObjectOfType<Light>();
                light.shadows = shadows;

                foreach (var item in HideFloors)
                {
                    item.gameObject.SetActive(true);
                }

                if (onCloseComplete != null) onCloseComplete();
            }).PlayBackwards();
        }       
    }
}
