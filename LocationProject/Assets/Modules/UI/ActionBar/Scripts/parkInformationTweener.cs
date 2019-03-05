using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parkInformationTweener : MonoBehaviour {
    public static parkInformationTweener Instance;
    /// <summary>
    /// 园区统计信息
    /// </summary>
    public GameObject parkInfo;

    public Tween parkInfoCloseTween;
    public Tween parkInfoOpenTween;

    public CanvasGroup parkInfoGroup;

    public Tween parkInfoCloseDisappear;
    public Tween parkInfoOpenAppear;
    void Start () {
        Instance = this;
        parkInfoGroup = parkInfo.transform.GetComponent<CanvasGroup>();
    }
	public void ParkInformationCloseTween()
    {
        parkInfoCloseTween = parkInfo.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(260f, 294f), 1f);
        parkInfoCloseDisappear = DOTween.To(() => parkInfoGroup.alpha, x => parkInfoGroup.alpha = x, 0, 1f);
    }
    public void ParkInformationOpernTween()
    {
        parkInfoOpenTween = parkInfo.transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(80f, 294f), 1f);
        parkInfoOpenAppear = DOTween.To(() => parkInfoGroup.alpha, x => parkInfoGroup.alpha = x, 1, 1f);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
