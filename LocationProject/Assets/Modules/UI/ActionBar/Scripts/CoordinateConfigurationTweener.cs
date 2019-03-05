using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateConfigurationTweener : MonoBehaviour {

    public GameObject CoordinateConfiguration;
    public Tween CoordinateConfigurationAppearTween;
    public Tween CoordinateConfigurationMoveTween;
    public CanvasGroup CoordinateConfigurationGroup;
    void Start () {
        CoordinateConfigurationGroup = CoordinateConfiguration.transform.GetComponent<CanvasGroup>();
    }

    public void CoordinateConfigurationTween()
    {
        CoordinateConfigurationMoveTween = CoordinateConfiguration.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        CoordinateConfigurationAppearTween = DOTween.To(() => CoordinateConfigurationGroup.alpha, x => CoordinateConfigurationGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
