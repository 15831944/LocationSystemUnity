using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonnelPositionTweener : MonoBehaviour {

    public GameObject PerPos;
    public Tween PerPosAppearTween;
    public Tween PerPosMoveTween;
    public CanvasGroup PerPosGroup;

    void Start () {
        PerPosGroup = PerPos.transform.GetComponent<CanvasGroup>();

    }
	public void PersonnelPositionTween()
    {
        PerPosMoveTween = PerPos.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        PerPosAppearTween = DOTween.To(() => PerPosGroup.alpha, x => PerPosGroup.alpha = x, 1, 0.5f);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
