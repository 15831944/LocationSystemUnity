using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 挂在UI上
/// </summary>

public class UGUIFollowTarget : MonoBehaviour
{
    public Camera Cam;//3d摄像机
    public GameObject Target;//跟随的目标
    private Canvas canvas;
    private RectTransform rectTransform;

    CanvasGroup canvasGroup;//控制显示隐藏
    public bool IsRayCheckCollision = false;//是否开启检测碰撞
    public float DisCamera;//到摄像机的距离
    public bool isup;//凸显当前界面，把当前界面顶到最上层


    private bool isUseCanvasScaler = true;

    public bool IsCheckDistance = false;//是否检测到摄像头的距离
    private float maxDistance;//界面可显示的最远距离
    /// <summary>
    /// 是否忽略Canvas的CanvasScaler组件
    /// </summary>
    public bool IsUseCanvasScaler
    {
        get
        {
            return isUseCanvasScaler;
        }

        set
        {
            isUseCanvasScaler = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (canvas == null)
        {
            canvas = gameObject.GetComponentInParent<Canvas>();
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        Init();
        FollowTarget();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Target == null)
        //{
        //    DestroyImmediate(gameObject);
        //    return;
        //}

        //FollowTarget();
        //GetDisTargetandCam();
        //if (IsRayCheckCollision)
        //{
        //    bool b = RayCheckCollision();
        //    SetShowOrHide(!b);
        //}
    }

    void LateUpdate()
    {
        if (Target == null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        FollowTarget();
        GetDisTargetandCam();
        if (IsRayCheckCollision)
        {
            bool b = RayCheckCollision();
            SetShowOrHide(!b);
        }
        CheckDistance();
    }

    void OnGUI()
    {
        //if (Target == null)
        //{
        //    DestroyImmediate(gameObject);
        //    return;
        //}

        //FollowTarget();
        //GetDisTargetandCam();
        //if (IsRayCheckCollision)
        //{
        //    bool b = RayCheckCollision();
        //    SetShowOrHide(!b);
        //}
    }
    /// <summary>
    /// 设置距离检测
    /// </summary>
    /// <param name="isDistanceCheckOn"></param>
    /// <param name="minDis"></param>
    /// <param name="maxDis"></param>
    public void SetEnableDistace(bool isDistanceCheckOn, float maxDis)
    {
        //IsCheckDistance = isDistanceCheckOn;
        //maxDistance = maxDis;
    }
    private void CheckDistance()
    {
        //if(IsCheckDistance)
        //{
        //    if(DisCamera>maxDistance)
        //    {
        //        SetShowOrHide(false);
        //    }
        //    else
        //    {
        //        SetShowOrHide(true);
        //    }
        //}
    }
    //设置UI的显示隐藏
    private void SetShowOrHide(bool b)
    {
        if (b)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public static UGUIFollowTarget AddUGUIFollowTarget(GameObject uiobj, GameObject target, Camera cam)
    {
        UGUIFollowTarget uguiFollowTarget = uiobj.AddMissingComponent<UGUIFollowTarget>();
        uguiFollowTarget.Target = target;
        uguiFollowTarget.Cam = cam;
        return uguiFollowTarget;
    }

    public static UGUIFollowTarget AddUGUIFollowTarget(GameObject uiobj, GameObject target, Camera cam, bool isUseCanvasScalerT)
    {
        UGUIFollowTarget uguiFollowTarget = uiobj.AddMissingComponent<UGUIFollowTarget>();
        uguiFollowTarget.Target = target;
        uguiFollowTarget.Cam = cam;
        uguiFollowTarget.IsUseCanvasScaler = isUseCanvasScalerT;
        return uguiFollowTarget;
    }


    /// <summary>
    /// UI跟随目标
    /// </summary>
    public void FollowTarget()
    {
        if (Target == null || Cam == null) return;

        Vector3 p = Cam.WorldToScreenPoint(Target.transform.position);

        //如果物体在摄像机的后面，摄像机看不到，就不显示跟随UI
        if (p.z < 0)
        {
            SetShowOrHide(false);
            return;
        }
        else
        {
            SetShowOrHide(true);
        }


        Vector3 p1;
        if (canvas == null)
        {
            p1 = new Vector3(p.x - Screen.width / 2, p.y - Screen.height / 2, 0);
        }
        else
        {
            if (IsUseCanvasScaler)
            {
                CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
                p1 = WorldToUI(Cam, canvasScaler, Target.transform.position);
            }
            else
            {
                p1 = WorldToUIWithIgnoreCanvasScaler(Cam, canvas, Target.transform.position);
            }
        }
        //Vector3 p1 = WorldToUI()
        //transform.localPosition = Vector3.zero;
        rectTransform.localPosition = p1;
    }


    /// <summary>
    /// 创建监控项UI跟随的空物体
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="h">离设备顶部的高度</param>
    public static GameObject CreateTitleTag(GameObject Obj, Vector3 offset)
    {
        Transform titleTag = Obj.transform.Find("TitleTag");
        if (titleTag == null)
        {
            titleTag = new GameObject("TitleTag").transform;

            //Vector3 scale = Obj.transform.lossyScale;
            //offset = new Vector3(offset.x * scale.x, offset.y * scale.y, offset.z * scale.z);
            titleTag.transform.position = new Vector3(offset.x + Obj.transform.position.x,
                (Obj.GetSize().y / 2) + offset.y + Obj.transform.position.y, offset.z + Obj.transform.position.z);
            titleTag.transform.parent = Obj.transform;
        }

        return titleTag.gameObject;
    }

    /// <summary>
    /// 世界物体位置转换为UI位置的方式
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="canvasT"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 WorldToUI(Camera camera, CanvasScaler canvasScalerT, Vector3 pos)
    {
        //if (canvasT == null)
        //{
        //    Debug.LogError("UGUIFollowTarget no Canvas");
        //    return Vector3.zero;
        //}

        //canvasScalerT = canvasT.GetComponent<CanvasScaler>();

        Vector3 screenPos = camera.WorldToScreenPoint(pos);

        return ScreenToUI(camera, canvasScalerT, screenPos);
    }

    /// <summary>
    /// 屏幕转换到UGUI
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="canvasScalerT"></param>
    /// <param name="screenPos"></param>
    /// <returns></returns>
    public static Vector3 ScreenToUI(Camera camera, CanvasScaler canvasScalerT, Vector3 screenPos)
    {
        float yOffset = 0;
        float xOffset = 0;
        Rect rec = camera.rect;
        yOffset = Screen.height * (rec.height / 2 + rec.y - 0.5f);
        xOffset = Screen.width * (rec.width / 2 + rec.x - 0.5f);

        Vector2 uiPos2 = new Vector2(screenPos.x - (Screen.width / 2) - xOffset, screenPos.y - (Screen.height / 2) - yOffset);
        //uiPos2 = uiPos2 / scaler.scaleFactor;

        float scaleFactor = 0;

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

    /// <summary>
    /// 世界物体位置转换为UI位置的方式,此方法不受CanvasScaler约束(就是不计算CanvasScaler对UI的影响)
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="canvasT"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector3 WorldToUIWithIgnoreCanvasScaler(Camera camera, Canvas canvasT, Vector3 pos)
    {
        if (canvasT == null)
        {
            Debug.LogError("UGUIFollowTarget no Canvas");
            return Vector3.zero;
        }
        //CanvasScaler scaler = canvasT.GetComponent<CanvasScaler>();

        Vector3 screenPos = camera.WorldToScreenPoint(pos);

        //float yOffset = 0;
        //float xOffset = 0;
        //Rect rec = camera.rect;
        //yOffset = Screen.height * (rec.height / 2 + rec.y - 0.5f);
        //xOffset = Screen.width * (rec.width / 2 + rec.x - 0.5f);

        //Vector2 uiPos2 = new Vector2(screenPos.x - (Screen.width / 2) - xOffset, screenPos.y - (Screen.height / 2) - yOffset);
        Vector2 uiPos2 = new Vector2(screenPos.x - (Screen.width / 2), screenPos.y - (Screen.height / 2));

        Vector3 p = new Vector3(uiPos2.x, uiPos2.y, 0);
        return p;

        //return p;
    }

    /// <summary>
    /// 射线检测，是否被物体遮挡
    /// </summary>
    private bool RayCheckCollision()
    {
        if (Target == null) return false;
        bool b = false;

        if (Target.transform.name == "TitleTag")
        {
            b = RayCheck(Target.transform.parent.gameObject, Cam);
        }
        else
        {
            b = RayCheck(Target, Cam);
        }

        return b;
    }

    /// <summary>
    /// 射线检测，是否被物体遮挡
    /// </summary>
    public static bool RayCheck(GameObject targetT, Camera cameraT)
    {
        Ray ray = new Ray(targetT.transform.position, cameraT.transform.position - targetT.transform.position);
        RaycastHit hit;
        float dis = Vector3.Distance(targetT.transform.position, cameraT.transform.position);
        if (Physics.Raycast(ray, out hit, dis))
        {
            if (hit.collider.gameObject != targetT)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 获取跟随目标与Camera的距离
    /// </summary>
    public void GetDisTargetandCam()
    {
        DisCamera = Vector3.Distance(Target.transform.position, Cam.transform.position);
    }

    /// <summary>
    /// 是否开启凸显当前界面，把当前界面顶到最上层（在当前组里面）
    /// </summary>
    /// <param name="isActive"></param>
    public void SetIsUp(bool isActive)
    {
        isup = isActive;
    }

    /// <summary>
    /// 设置是否检测射线碰撞
    /// </summary>
    public void SetIsRayCheckCollision(bool b)
    {
        IsRayCheckCollision = b;
    }
}
