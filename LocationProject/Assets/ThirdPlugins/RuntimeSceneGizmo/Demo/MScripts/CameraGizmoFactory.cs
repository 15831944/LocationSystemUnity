using Mogoson.CameraExtension;
using RuntimeSceneGizmo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraGizmoFactory : MonoBehaviour {

    public static CameraGizmoFactory Instance;

    [SerializeField]
    private float cameraAdjustmentSpeed = 3f;

    [SerializeField]
    private float projectionTransitionSpeed = 2f;

    public Camera mainCamera;

    public Text IntroduceText;//文本介绍 （正交和普通视角）

    public GameObject Container;

    private IEnumerator cameraRotateCoroutine, projectionChangeCoroutine;

    private Vector2 XPositive = new Vector2(0,270);
    private Vector2 XNegative = new Vector2(0,90);
    private Vector2 ZPositive = new Vector2(0,180);
    private Vector2 ZNegative = new Vector2(0,360);
    private Vector2 YPositive = new Vector2(90,360);

    private void Awake()
    {
        Instance = this;
        if (mainCamera==null)mainCamera = Camera.main;
        mainCamera.orthographicSize = 110f;//设置正交距离
        SceneEvents.FullViewStateChange += OnFullViewChange;
    }

    private void OnDisable()
    {
        cameraRotateCoroutine = projectionChangeCoroutine = null;
    }
    private void OnFullViewChange(bool isOn)
    {
        if(isOn)
        {
            Close();
        }
        else
        {
            Show();
        }
    }
    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        Container.SetActive(true);
    }
    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        Container.SetActive(false);
        if(mainCamera.orthographic) SwitchOrthographicMode();
        SetClippingPlaneDis();
    }

    public void ChangeCameraOrthographic()
    {
        OnGizmoComponentClicked(GizmoComponent.Center);
    }

    public void OnGizmoComponentClicked(GizmoComponent component)
    {
        if (component == GizmoComponent.Center)
        {
            SwitchOrthographicMode();
            SetClippingPlaneDis();
        }            
        else
        {
            SetOrthographicSize(component);
            RotateCamera(component);
        }
    }
    /// <summary>
    /// 设置正交默认值
    /// </summary>
    /// <param name="component"></param>
    private void SetOrthographicSize(GizmoComponent component)
    {
        if (!mainCamera.orthographic) return;
        if (component == GizmoComponent.YPositive)
        {
            mainCamera.orthographicSize = 150f;//设置正交距离
        }
        else
        {
            mainCamera.orthographicSize = 60f;//设置正交距离
        }
    }
    private void SetClippingPlaneDis()
    {
        if (mainCamera.orthographic) mainCamera.farClipPlane = 2000;
        else mainCamera.farClipPlane = 1500;
    }

    /// <summary>
    /// 旋转相机
    /// </summary>
    /// <param name="component"></param>
    private void RotateCamera(GizmoComponent component)
    {
        if (component == GizmoComponent.XNegative)
            RotateCameraInAlgin(RegetAngle(XNegative));
        else if (component == GizmoComponent.XPositive)
            RotateCameraInAlgin(RegetAngle(XPositive));
        else if (component == GizmoComponent.YPositive)
            RotateCameraInAlgin(RegetAngle(YPositive));
        else if (component == GizmoComponent.ZNegative)
            RotateCameraInAlgin(RegetAngle(ZNegative));
        else if (component == GizmoComponent.ZPositive)
            RotateCameraInAlgin(RegetAngle(ZPositive));
    }
    /// <summary>
    /// 获取对角角度（正交最低为0）
    /// </summary>
    /// <param name="lastAngle"></param>
    /// <returns></returns>
    private Vector2 RegetAngle(Vector2 lastAngle)
    {
        if(mainCamera.orthographic)
        {
            Vector2 angle = new Vector2(lastAngle.x==5?0:lastAngle.x,lastAngle.y==5?0:lastAngle.y);
            return angle;
        }
        return lastAngle;
    }
    /// <summary>
    /// 旋转摄像机角度
    /// </summary>
    /// <param name="direction"></param>
    public void RotateCameraInAlgin(Vector3 direction)
    {
        CameraSceneManager manager = CameraSceneManager.Instance;
        if(manager)
        {
            AlignTarget target = manager.GetCurrentAlign();
            target.angles = direction;
            manager.ChangeFocusAngle(target);
        }
    }

   



    public void SwitchOrthographicMode(Action onComplete=null)
    {        
        if (projectionChangeCoroutine != null)
            return;

        projectionChangeCoroutine = SwitchProjection(onComplete);
        StartCoroutine(projectionChangeCoroutine);
    }


    private IEnumerator SwitchProjection(Action OnComplete=null)
    {
        bool isOrthographic = mainCamera.orthographic;

        Matrix4x4 dest, src = mainCamera.projectionMatrix;
        if (isOrthographic)
            dest = Matrix4x4.Perspective(mainCamera.fieldOfView, mainCamera.aspect, mainCamera.nearClipPlane, mainCamera.farClipPlane);
        else
        {
            float orthographicSize = mainCamera.orthographicSize;
            float aspect = mainCamera.aspect;
            dest = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, mainCamera.nearClipPlane, mainCamera.farClipPlane);
        }

        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime * projectionTransitionSpeed)
        {
            float lerpModifier = isOrthographic ? t * t : Mathf.Pow(t, 0.2f);
            Matrix4x4 matrix = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                matrix[i] = Mathf.LerpUnclamped(src[i], dest[i], lerpModifier);

            mainCamera.projectionMatrix = matrix;
            yield return null;
        }

        mainCamera.orthographic = !isOrthographic;
        mainCamera.ResetProjectionMatrix();
        SetIntroduceInfo(mainCamera.orthographic);

        projectionChangeCoroutine = null;
        if (OnComplete != null) OnComplete();
    } 

    /// <summary>
    /// 设置介绍文本
    /// </summary>
    /// <param name="isOrthographic"></param>
    private void SetIntroduceInfo(bool isOrthographic)
    {
        //IntroduceText.text = isOrthographic ? "ISO":"Persp";
        IntroduceText.text = isOrthographic ? "平行视角" : "普通视角";
    }
}
