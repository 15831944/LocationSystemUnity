using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOutTweener : MonoBehaviour {
    public static StartOutTweener Instance;
    public GameObject startOut;

    public Tween startOutCloseTween;
    public Tween startOutOpenTween;

    public CanvasGroup startOutGroup;

    public Tween startOutCloseDisappear;
    public Tween startOutOpenAppear;
    void Start () {
        Instance = this;
        startOutGroup = startOut.transform.GetComponent<CanvasGroup>(); 

    }
	
	public void StartOutCloseTween()
    {
        startOutCloseTween = startOut.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-220f, 0f), 1f);
        startOutCloseDisappear = DOTween.To(() => startOutGroup.alpha, x => startOutGroup.alpha = x, 0, 1f);
    }
    public void StartOutOpernTween()
    {
        startOutOpenTween = startOut.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(2.5f, 0f), 1f);
        startOutOpenAppear = DOTween.To(() => startOutGroup.alpha, x => startOutGroup.alpha = x, 1, 1f);
    }
    void Update () {
		
	}
}
