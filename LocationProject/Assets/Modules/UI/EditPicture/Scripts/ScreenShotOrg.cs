using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotOrg : MonoBehaviour {
    /// <summary>
    /// 截图用正交相机
    /// </summary>
    public Camera screenShotCamera;
    /// <summary>
    /// 截图按钮
    /// </summary>
    public Button ScreenShotButton;
    /// <summary>
    /// 显示截图界面
    /// </summary>
    public Image ScreenShotImage;
    /// <summary>
    /// 图片压缩比例（1920*1080）
    /// </summary>
    private float ScaleSize=0.5f;
    /// <summary>
    /// 截图的宽
    /// </summary>
    private float ImageWidth;
    /// <summary>
    /// 截图的高度
    /// </summary>
    private float ImageHeight;
    /// <summary>
    /// 截图名称
    /// </summary>
    private string PicName = "电厂正交视图";
    /// <summary>
    /// 是否正在截图
    /// </summary>
    private bool IsScreenShoting;
    /// <summary>
    /// 是否正绘制图片
    /// </summary>
    private bool IsDrawImage;
    void Start()
    {
        SetCaptureSize();
        SceneEvents.FullViewStateChange += CaptureImage;
        //ScreenShotButton.onClick.AddListener(TestImage);
    }
    private void SetCaptureSize()
    {
        ImageWidth = Screen.width * ScaleSize;
        ImageHeight = Screen.height * ScaleSize;
    }
    /// <summary>
    /// 保存截图
    /// </summary>
    private void CaptureImage(bool isFullView)
    {
        //OnScreenShotStart();
        //CaptureCamera(screenShotCamera, new Rect(0, 0, Screen.width, Screen.height));
        if(isFullView)
        {
            if (IsInvoking("OnScreenShotStart"))
            {
                CancelInvoke("OnScreenShotStart");
            }
        }      
        else
        {
            InvokeRepeating("OnScreenShotStart", 0, CommunicationObject.Instance.RefreshSetting.ScreenShot);
        }
    }

    /// <summary>
    /// 获取图片
    /// </summary>
    private void LoadImage()
    {
        if (IsDrawImage) return;
        IsDrawImage = true;
        Picture pic = null;                  
        ThreadManager.Run(() =>
        {
            CommunicationObject service = CommunicationObject.Instance;
            if (service)
            {
               pic = service.GetPictureInfo(PicName);
            }
        }, () =>
        {
            if (pic != null)
            {
                Texture2D tex = new Texture2D((int)ImageWidth, (int)ImageHeight);
                tex.LoadImage(pic.Info);
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                ScreenShotImage.overrideSprite = sprite;
            }
            IsDrawImage = false;
        }, "Load Image");
    }
    private DateTime recordTime;
    /// <summary>
    /// 开启截图协程
    /// </summary>
    private void OnScreenShotStart()
    {
        ScreenShotByCamera();
    }
    /// <summary>
    /// 通过相机截图
    /// </summary>
    private void ScreenShotByCamera()
    {
        if (IsScreenShoting) return;
        IsScreenShoting = true;
        if (screenShotCamera == null) return;
        //Debug.LogError("StartShot");
        StartCoroutine(CaptureCamera(screenShotCamera, new Rect(0, 0, ImageWidth, ImageHeight), picBytes => 
        {
            //recordTime = DateTime.Now;
            ThreadManager.Run(() =>
            {
                CommunicationObject service = CommunicationObject.Instance;
                if (service)
                {
                    Picture screenPic = new Picture()
                    {
                        Name = PicName,
                        Info = picBytes
                    };
                    service.EditPictureInfo(screenPic);
                }
            }, () =>
             {
                 //Debug.LogError(string.Format("EditPictureInfo cost time:{0} ms", (DateTime.Now - recordTime).TotalMilliseconds));
                 //recordTime = DateTime.Now;
                 IsScreenShoting = false;
             }, "");
        }));           
    }


    private WaitForEndOfFrame waitTime = new WaitForEndOfFrame();
    //private RenderTexture rt;
    //private Texture2D screenShot;
    /// <summary>  
    /// 对相机截图。   
    /// </summary>  
    /// <returns>The screenshot2.</returns>  
    /// <param name="camera">Camera.要被截屏的相机</param>  
    /// <param name="rect">Rect.截屏的区域</param>  
    IEnumerator CaptureCamera(Camera camera, Rect rect,Action<byte[]>onCaptureComplete=null)
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, -1);
        yield return waitTime;
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();

        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot=null;
        screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();
        yield return waitTime;

        //// 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors  
        DestroyImmediate(rt);

        yield return waitTime;
        // 最后将这些纹理数据，成一个png图片文件 
        //ThreadManager.Run(()=> { byte[] bytes = screenShot.EncodeToPNG(); },()=> { },""); 
        byte[] bytes = screenShot.EncodeToJPG();

        //string filename = Application.dataPath + "/Screenshot.jpg";
        //System.IO.File.WriteAllBytes(filename, bytes);
        //Debug.Log(string.Format("截屏了一张照片: {0}", filename));
        //Debug.Log("Length:" + bytes.Length);

        if (onCaptureComplete != null) onCaptureComplete(bytes);

        //IsScreenShoting = false;

    }

    
}
