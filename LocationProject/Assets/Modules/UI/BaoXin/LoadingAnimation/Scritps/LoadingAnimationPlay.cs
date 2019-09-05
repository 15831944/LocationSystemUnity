using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimationPlay : MonoBehaviour {
    public static LoadingAnimationPlay Instance;
    public Image ImageShow;
    public SpriteRenderer renderSprites;

    public Animator AnimatorController;
    public Text ProgressText;
    public RectTransform RotateImage;
    public Image CompanyLogo;//公司logo
	// Use this for initialization
	void Awake () {
        Instance = this;

    }
    public bool isPlay;
	// Update is called once per frame
	void Update () {
        if(isPlay)
        {
            ImageShow.sprite = renderSprites.sprite;
            RotateImageTransform();
        }      
	}
    /// <summary>
    /// 右下角图标旋转
    /// </summary>
    private void RotateImageTransform()
    {
        Vector3 vec = RotateImage.localEulerAngles;
        if (vec.z < -360f) vec.z = 0;
        vec.z -= 5;
        RotateImage.localEulerAngles = vec;
    }
    Tween LogoTween;
    private void CreateTween()
    {
        LogoTween = CompanyLogo.transform.DOLocalMoveY(10,1.25f).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InOutCubic);
        LogoTween.Pause();
    }
    public void Play()
    {
        AnimatorController.enabled=true;
        isPlay = true;
        ImageShow.gameObject.SetActive(true);
        if (LogoTween == null)
        {
            CreateTween();
        }
        LogoTween.Play();
        SetProgress(0);
    }
    private float v;
    public void SetProgress(float value)
    {
        value = (float)Math.Round(value, 2);
        v = value * 100;
        ProgressText.text = ""+v+"%";
    }

    public void Close()
    {
        AnimatorController.enabled = false;
        isPlay = false;
        ImageShow.gameObject.SetActive(false);
    }
}
