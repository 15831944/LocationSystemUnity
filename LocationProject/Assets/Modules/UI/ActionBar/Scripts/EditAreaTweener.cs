using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditAreaTweener : MonoBehaviour {

    public GameObject EditArea;
    public Tween EditAreaAppearTween;
    public Tween EditAreaMoveTween;
    public CanvasGroup EditAreaGroup;
    void Start () {
        EditAreaGroup = EditArea.transform.GetComponent<CanvasGroup>();
    }

    public void EditAreaTween()
    {
        EditAreaMoveTween = EditArea.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        EditAreaAppearTween = DOTween.To(() => EditAreaGroup.alpha, x => EditAreaGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
