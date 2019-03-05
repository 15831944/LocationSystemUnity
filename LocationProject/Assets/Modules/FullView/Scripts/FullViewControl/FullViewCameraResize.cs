using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullViewCameraResize : MonoBehaviour {

    /// <summary>
    /// 3D相机所对齐的背景
    /// </summary>
    public GameObject Target;
    /// <summary>
    /// 首页的Canvas
    /// </summary>
    public Canvas MainPageCanvas;
    /// <summary>
    /// 3D相机(非UI相机)
    /// </summary>
    public Camera ThreeDCamera;

    public Camera UICamera;
    private static float scaleFactor = 0;

    private bool isInit;
    // Use this for initialization
    void Start () {
        Invoke("Init",1);
    }
    /// <summary>
    /// 初始化首页3d区域的位置
    /// </summary>
    public void Init()
    {
        if (isInit) return;
        isInit = true;
        CaculateCameraSize();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(FullViewController.Instance&&FullViewController.Instance.IsFullView)
            {
                CaculateCameraSize();
            }            
        }
    }

    /// <summary>
    /// 计算3D摄像机，在首页的参数
    /// </summary>
    [ContextMenu("ResetCameraField")]
    public void CaculateCameraSize()
    {

        Vector3 p = UICamera.WorldToScreenPoint(Target.transform.position);
        Vector3 p1;
        if (MainPageCanvas == null)
        {
            p1 = new Vector3(p.x - Screen.width / 2, p.y - Screen.height / 2, 0);
        }
        else
        {
            CanvasScaler canvasScaler = MainPageCanvas.GetComponent<CanvasScaler>();
            if(canvasScaler==null)
            {
                Debug.LogError("MainPageCanvas.CanvasScaler is null...");
                return;
            }
            p1 = WorldToUI(UICamera, canvasScaler, Target.transform.position);
        }
        //求出左下角的位置（相对屏幕左下角）
        Vector2 size = Target.GetComponent<RectTransform>().rect.size;
        Vector2 bottonPos = new Vector2(p1.x - size.x / 2, p1.y - size.y / 2);
        Vector2 screenSize = new Vector2(Screen.width / scaleFactor, Screen.height / scaleFactor); //MatchWidthOrHeight之后，实际的宽高
        bottonPos += new Vector2(screenSize.x / 2, screenSize.y / 2);

        //根据坐下角的点，算出在屏幕中的比例
        Vector2 cameraXY = new Vector2(bottonPos.x / screenSize.x, bottonPos.y / screenSize.y);
        Vector2 cameraField = new Vector2(size.x / screenSize.x, size.y / screenSize.y);
        ThreeDCamera.rect = new Rect(cameraXY, cameraField);
    }
    private static DateTime recordT;
    private static float minCheckTime = 0.5f;
    public static Vector3 WorldToUI(Camera camera, CanvasScaler canvasScalerT, Vector3 pos)
    {
        recordT = DateTime.Now;
        Vector3 screenPos = camera.WorldToScreenPoint(pos);
        //while(screenPos==Vector3.zero)
        //{            
        //    if ((DateTime.Now - recordT).TotalSeconds > minCheckTime)
        //    {
        //        screenPos = camera.WorldToScreenPoint(pos);
        //        Debug.LogError("ScreenPos in cycle:" + screenPos + " Is Equal:" + (screenPos == Vector3.zero));
        //    }
        //}
        //Debug.LogError("ScreenPos:"+screenPos);
        return ScreenToUI(camera, canvasScalerT, screenPos);
    }
   
    public static Vector3 ScreenToUI(Camera camera, CanvasScaler canvasScalerT, Vector3 screenPos)
    {
        float yOffset = 0;
        float xOffset = 0;
        Rect rec = camera.rect;
        yOffset = Screen.height * (rec.height / 2 + rec.y - 0.5f);
        xOffset = Screen.width * (rec.width / 2 + rec.x - 0.5f);

        Vector2 uiPos2 = new Vector2(screenPos.x - (Screen.width / 2) - xOffset, screenPos.y - (Screen.height / 2) - yOffset);
        //uiPos2 = uiPos2 / scaler.scaleFactor;

        scaleFactor = 0;

        if (canvasScalerT.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
        {
            uiPos2 = uiPos2 / canvasScalerT.scaleFactor;
        }
        else if (canvasScalerT.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            //uiPos2 = new Vector2(uiPos2.x * canvasScalerT.referenceResolution.x / Screen.width, uiPos2.y * canvasScalerT.referenceResolution.y / Screen.height);


            if (canvasScalerT.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
            {
                // We take the log of the relative width and height before taking the average.
                // Then we transform it back in the original space.
                // the reason to transform in and out of logarithmic space is to have better behavior.
                // If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
                // In normal space the average would be (0.5 + 2) / 2 = 1.25
                // In logarithmic space the average is (-1 + 1) / 2 = 0
                float kLogBase = 2;
                float logWidth = Mathf.Log(Screen.width / canvasScalerT.referenceResolution.x, kLogBase);
                float logHeight = Mathf.Log(Screen.height / canvasScalerT.referenceResolution.y, kLogBase);
                float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, canvasScalerT.matchWidthOrHeight);
                scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);

            }
            else if (canvasScalerT.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
            {
                scaleFactor = Mathf.Min(Screen.width / canvasScalerT.referenceResolution.x, Screen.height / canvasScalerT.referenceResolution.y);

            }
            else if (canvasScalerT.screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
            {
                scaleFactor = Mathf.Max(Screen.width / canvasScalerT.referenceResolution.x, Screen.height / canvasScalerT.referenceResolution.y);

            }
        }
        Vector3 p = new Vector3(uiPos2.x, uiPos2.y, 0) / scaleFactor;
        return p;
    }

    private void Test(GameObject model)
    {
        if (!model.name.Contains("Door"))
        {
            if (model.GetComponent<MeshCollider>() == null)
            {
                model.gameObject.AddComponent<MeshCollider>();
            }
        }
    }
}
