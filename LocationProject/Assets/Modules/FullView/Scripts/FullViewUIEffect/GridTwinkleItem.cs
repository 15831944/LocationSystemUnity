using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GridTwinkleItem : MonoBehaviour {  
    /// <summary>
    /// 效果图片
    /// </summary>
    private Image twinkleImage;
    /// <summary>
    /// 第一次效果延迟时间
    /// </summary>
    private float invokeWaitTime;
    /// <summary>
    /// 方法重复间隔时间
    /// </summary>
    private float repeatTime=0.2f;
    /// <summary>
    /// 最小透明度
    /// </summary>
    private float minFade = 0.016f;
    /// <summary>
    /// 最大透明度
    /// </summary>
    private float maxFade = 0.1f;
    /// <summary>
    /// 单个动画执行时间
    /// </summary>
    private float tweenFadeTime = 0.2f;

    void Awake()
    {
        twinkleImage = transform.GetComponent<Image>();
        
    }
    /// <summary>
    /// 开始图片闪烁
    /// </summary>
    public void StartTwinkle()
    {
        invokeWaitTime = Random.Range(0f, 0.28f);
        InvokeRepeating("DoImageFade", invokeWaitTime, repeatTime);
    }
    /// <summary>
    /// 停止闪烁
    /// </summary>
    public void StopTwinkle()
    {
        if(IsInvoking("DoImageFade"))
        {
            CancelInvoke("DoImageFade");
        }
    }
    /// <summary>
    /// 动画执行部分
    /// </summary>
    private void DoImageFade()
    {
        float endValue = Random.Range(minFade, maxFade);
        endValue = endValue < 0.05f ? Random.Range(endValue, maxFade) : endValue;
        twinkleImage.DOFade(endValue, tweenFadeTime);
    }
}
