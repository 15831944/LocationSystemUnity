using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModdleToggle : MonoBehaviour {

    public GameObject ModdleTog;
    public Tween ModdleTogAppearTween;
    public Tween ModdleTogMoveTween;
    public CanvasGroup ModdleTogGroup;
    void Start () {
        ModdleTogGroup = ModdleTog.transform.GetComponent<CanvasGroup>();
    }

    public void ModdleToggleTween()
    {
        ModdleTogMoveTween = ModdleTog.transform.GetComponent<RectTransform>().DOMoveY(-5f, 0.5f);
        ModdleTogAppearTween = DOTween.To(() => ModdleTogGroup.alpha, x => ModdleTogGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
