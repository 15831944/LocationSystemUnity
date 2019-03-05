/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AroundCamera.cs
 *  Description  :  Camera rotate around target gameobject.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/8/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace Mogoson.CameraExtension
{
    /// <summary>
    /// Camera rotate around target gameobject.
    /// </summary>
    [AddComponentMenu("Mogoson/CameraExtension/AroundCamera")]
    [RequireComponent(typeof(Camera))]
    public class AroundCamera : MonoBehaviour
    {
        #region Field and Property
        /// <summary>
        /// Around center.
        /// </summary>
        public Transform target;

        /// <summary>
        /// Settings of mouse button, pointer and scrollwheel.
        /// </summary>
        public MouseSettings mouseSettings = new MouseSettings(1, 10, 10);

        /// <summary>
        /// Range limit of angle.
        /// </summary>
        public Range angleRange = new Range(-90, 90);

        /// <summary>
        /// Range limit of distance.
        /// </summary>
        public Range distanceRange = new Range(1, 10);

        //正交摄像机范围
        public Range OrthographicRange = new Range(10,155);
        /// <summary>
        /// Damper for move and rotate.
        /// </summary>
        [Range(0, 10)]
        public float damper = 5;

        /// <summary>
        /// Camera current angls.
        /// </summary>
        public Vector2 CurrentAngles { protected set; get; }

        /// <summary>
        /// Current distance from camera to target.
        /// </summary>
        public float CurrentDistance { protected set; get; }

        /// <summary>
        /// Camera target angls.
        /// </summary>
        protected Vector2 targetAngles;

        /// <summary>
        /// Target distance from camera to target.
        /// </summary>
        protected float targetDistance;

        /// <summary>
        /// 是否关闭鼠标旋转
        /// </summary>
        private bool IsDisableMouseInput;

        /// <summary>
        /// 鼠标原始位置（Input.GetAxis("Mouse X")在远程桌面和TeamViewer中都是0，不能移动）
        /// </summary>
        private Vector3 mousePositionOri;

        private Camera mainCamera;
        #endregion

        #region Protected Method
        protected virtual void Start()
        {
            CurrentAngles = targetAngles = transform.eulerAngles;
            CurrentDistance = targetDistance = Vector3.Distance(transform.position, target.position);
            mainCamera = transform.GetComponent<Camera>();
        }

        protected virtual void LateUpdate()
        {
            AroundByMouseInput();
        }
        /// <summary>
        /// 打开/关闭 鼠标旋转功能
        /// </summary>
        /// <param name="isEnableMouse">是否启用鼠标旋转</param>
        public virtual void SetMouseInputState(bool isEnableMouse)
        {
            IsDisableMouseInput = !isEnableMouse;
        }
        /// <summary>
        /// Camera around target by mouse input.
        /// </summary>
        protected void AroundByMouseInput()
        {
            if (IsDisableMouseInput) return;
            if (IsClickUGUIorNGUI.Instance)
            {
                if (IsClickUGUIorNGUI.Instance.isClickedUI) return;
            }
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                mousePositionOri = Input.mousePosition;
            }
            if (Input.GetMouseButton(mouseSettings.mouseButtonID))
            {
                Vector2 mouseOffset = GetMouseOffset();
                targetAngles.y += mouseOffset.x * mouseSettings.pointerSensitivity;
                targetAngles.x -= mouseOffset.y * mouseSettings.pointerSensitivity;

                //Mouse pointer.
                //targetAngles.y += Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
                //targetAngles.x -= Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

                //Range.
                targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
            }

            //Mouse scrollwheel.
            if (mainCamera == null) mainCamera = transform.GetComponent<Camera>();
            if(mainCamera.orthographic)
            {
                if(!Input.GetMouseButton(2))
                {
                    mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * mouseSettings.wheelSensitivity;
                    mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, OrthographicRange.min, OrthographicRange.max);
                }               
            }
            if (!Input.GetMouseButton(2))
            {
                targetDistance -= Input.GetAxis("Mouse ScrollWheel") * mouseSettings.wheelSensitivity;
            }               
            targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);

            //Lerp.
            CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, damper * Time.deltaTime);
            CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, damper * Time.deltaTime);

            //Update transform position and rotation.
            transform.rotation = Quaternion.Euler(CurrentAngles);
            transform.position = target.position - transform.forward * CurrentDistance;
        }

        /// <summary>
        /// 获取鼠标移动的距离，Input.GetAxis("Mouse X")在远程桌面和TeamViewer中都是0，不能移动
        /// </summary>
        /// <returns></returns>
        private Vector2 GetMouseOffset()
        {
            //float x = Input.GetAxis("Mouse X");
            //float y = Input.GetAxis("Mouse Y");
            //Vector2 lastPos = new Vector2(x,y);
            Vector3 mouseOffset = Input.mousePosition - mousePositionOri;
            //Debug.Log("MouseOffset:(" + mouseOffset.x + "," + mouseOffset.y + ")");
            mousePositionOri = Input.mousePosition;
            float x = mouseOffset.x / 30f;
            float y = mouseOffset.y / 30f;   //12
            //if (x != 0 && y != 0)
            //{
            //    Debug.Log(string.Format("Around GetAxis,system:({0},{1}) caculate:({2},{3})", lastPos.x, lastPos.y, x, y));
            //}
            return new Vector2(x, y);
        }
        #endregion
    }
}