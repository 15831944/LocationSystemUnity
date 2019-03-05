using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicePositionTweener : MonoBehaviour {

    public GameObject DevPos;
    public Tween DevPosAppearTween;
    public Tween DevPosMoveTween;
    public CanvasGroup DevPosGroup;
    void Start () {
        DevPosGroup = DevPos.transform.GetComponent<CanvasGroup>();
    }

    public void DevicePositionTween()
    {
        DevPosMoveTween = DevPos.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        DevPosAppearTween = DOTween.To(() => DevPosGroup.alpha, x => DevPosGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
