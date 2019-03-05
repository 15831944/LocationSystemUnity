using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FullViewTweenController : MonoBehaviour {
    /// <summary>
    /// 圆环区域动画
    /// </summary>
    public FullViewTweenCircleController circlePart;
    /// <summary>
    /// 按钮组区域动画
    /// </summary>
    public FVTButtonGroupController buttonGroupPart;
    /// <summary>
    /// 照片区域动画
    /// </summary>
    public FVTPhotoController photoPart;
    /// <summary>
    /// 全景（首页）控制部分
    /// </summary>
    private FullViewController fullViewController;
    /// <summary>
    /// 是否正在做首页动画
    /// </summary>
    public static bool IsTweening;
    /// <summary>
    /// 是否展示首页动画
    /// </summary>
    public static bool displayFullViewTween = true;
    void Start () {
        fullViewController = FullViewController.Instance;
#if UNITY_EDITOR
        //displayFullViewTween = true;
        //StartTweenOnStart();
        displayFullViewTween = false;

        //AsyncLoadScene.Instance.LoadAdd("Park", () =>
        //{
        //    //Invoke("AfterLoadParkEDITOR", 1F);//延迟是为了让Pack场景里的脚本初始化完
        //    //CompleteAllTween();
        //    AfterLoadParkEDITOR();
        //});
        //AfterLoadParkEDITOR();
        AfterLoadParkPack();
#else
        //AsyncLoadScene.Instance.LoadAdd("Park", () =>
        //{
        //    AfterLoadParkPack();
        //});
         AfterLoadParkPack();

        //displayFullViewTween = true;
        //StartTweenOnStart();
#endif
        //CompleteAllTween();
    }

    /// <summary>
    /// 编辑器状态，在加载完园区模型场景之后要进行的操作
    /// </summary>
    public void AfterLoadParkEDITOR()
    {
        CompleteAllTween();
        RoomFactory.Instance.Init();

    }

    /// <summary>
    /// 打包后状态，在加载完园区模型场景之后要进行的操作
    /// </summary>
    public void AfterLoadParkPack()
    {
        displayFullViewTween = true;
        StartTweenOnStart();
        RoomFactory.Instance.Init();
    }

    /// <summary>
    /// 程序开始，自动播放动画
    /// </summary>
    private void StartTweenOnStart()
    {
        StartAllTween();
    }
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (!fullViewController.IsFullView) return;
        //    StartAllTween();
        //}
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (!fullViewController.IsFullView) return;
        //    PauseAllTween();
        //}
        //if(Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    if (!fullViewController.IsFullView) return;
        //    CompleteAllTween();
        //}
    }
    private void StartAllTween()
    {
        IsTweening = true;
        circlePart.StartTween(() =>
        {
            if (IsTweening)
            {
                buttonGroupPart.StartTween();
                photoPart.StartTween(()=> 
                {
                    Show3DArea();
                });
            }
        });
        //buttonGroupPart.StartTween();
        //photoPart.StartTween();
    }
    private void PauseAllTween()
    {
        IsTweening = false;
        circlePart.PauseTween();
        buttonGroupPart.PauseTween();
        photoPart.StopTween();
    }
    private void CompleteAllTween()
    {
        IsTweening = false;
        circlePart.CompleteTween();
        buttonGroupPart.CompleteTween();
        photoPart.CompleteTween();
        Show3DArea();
    }
    /// <summary>
    /// 动画完成后，显示3D区域
    /// </summary>
    private void Show3DArea()
    {
        //MainCamera.gameObject.SetActive(true);
        FullViewController.Instance.SwitchToFullView();
    }
}
