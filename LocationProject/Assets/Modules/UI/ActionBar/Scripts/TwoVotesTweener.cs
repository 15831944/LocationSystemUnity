using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoVotesTweener : MonoBehaviour {

    public GameObject TwoVotes;
    public Tween TwoVotesAppearTween;
    public Tween TwoVotesMoveTween;
    public CanvasGroup TwoVotesGroup;
    void Start () {
        TwoVotesGroup = TwoVotes.transform.GetComponent<CanvasGroup>();
    }

    public void TwoVotesTween()
    {
        TwoVotesMoveTween = TwoVotes.transform.GetComponent<RectTransform>().DOMoveY(-5, 0.5f);
        TwoVotesAppearTween = DOTween.To(() => TwoVotesGroup.alpha, x => TwoVotesGroup.alpha = x, 1, 0.5f);
    }
    void Update () {
		
	}
}
