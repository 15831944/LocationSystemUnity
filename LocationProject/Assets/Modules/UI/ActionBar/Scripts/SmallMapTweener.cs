using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallMapTweener : MonoBehaviour {
    public static SmallMapTweener Instance;
    /// <summary>
    /// 小地图
    /// </summary>
    public GameObject SmallMap;
    public Tween SmallMapCloseTween;
    public Tween SmallMapOpenTween;

    public CanvasGroup SmallMapGroup;

    public Tween SmallMapCloseDisappear;
    public Tween SmallMapOpenAppear;

    void Start () {
        Instance = this;
        SmallMapGroup = SmallMap.transform.GetComponent<CanvasGroup>();
    }
	
	public void ShowSmallMapCloseTween()
    {
        SmallMapCloseTween = SmallMap.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3 (-367f, 0f ), 1f);
        SmallMapCloseDisappear = DOTween.To(() => SmallMapGroup.alpha, x => SmallMapGroup.alpha = x, 0, 1f);
    }
    public void ShowSmallMapOpenTween()
    {
        SmallMapOpenTween = SmallMap.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 0f), 1f);
        SmallMapOpenAppear = DOTween.To(() => SmallMapGroup.alpha, x => SmallMapGroup.alpha = x, 1, 1f);
    }
}
