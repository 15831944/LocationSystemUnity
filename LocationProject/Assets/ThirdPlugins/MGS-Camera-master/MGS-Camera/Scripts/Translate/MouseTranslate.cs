/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  MouseTranslate.cs
 *  Description  :  Mouse pointer drag to translate gameobject.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/9/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace Mogoson.CameraExtension
{
    /// <summary>
    /// Mouse pointer drag to translate gameobject.
    /// </summary>
    [AddComponentMenu("Mogoson/CameraExtension/MouseTranslate")]
    public class MouseTranslate : MonoBehaviour
    {
        #region Field and Property
        /// <summary>
        /// Target camera for translate direction.
        /// </summary>
        public Transform targetCamera;

        /// <summary>
        /// Settings of mouse button and pointer.
        /// </summary>
        public MouseSettings mouseSettings = new MouseSettings(0, 1, 0);

        /// <summary>
        /// Settings of move area.
        /// </summary>
        public PlaneArea areaSettings = new PlaneArea(null, 10, 10);

        /// <summary>
        /// Damper for move.
        /// </summary>
        [Range(0, 10)]
        public float damper = 5;

        /// <summary>
        /// Current offset base area center.
        /// </summary>
        public Vector3 CurrentOffset { protected set; get; }

        /// <summary>
        /// Target offset base area center.
        /// </summary>
        protected Vector3 targetOffset;

        /// <summary>
        /// 鼠标原始位置（Input.GetAxis("Mouse X")在远程桌面和TeamViewer中都是0，不能移动）
        /// </summary>
        private Vector3 mousePositionOri;
        #endregion

        #region Protected Method
        protected virtual void Start()
        {
            CurrentOffset = targetOffset = transform.position - areaSettings.center.position;
        }

        protected virtual void Update()
        {
            TranslateByMouseInput();
        }

        /// <summary>
        /// Translate this gameobject by mouse input.
        /// </summary>
        protected void TranslateByMouseInput()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                mousePositionOri = Input.mousePosition;
            }
            if (Input.GetMouseButton(mouseSettings.mouseButtonID))
            {
                Vector2 mouseOffset = GetMouseOffset();
                var mouseX = mouseOffset.x * mouseSettings.pointerSensitivity;
                var mouseY = mouseOffset.y * mouseSettings.pointerSensitivity;

                //Mouse pointer.
                //var mouseX = Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
                //var mouseY = Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

                //Deal with offset base direction of target camera.
                targetOffset -= targetCamera.right * mouseX;
                targetOffset -= Vector3.Cross(targetCamera.right, Vector3.up) * mouseY;

                //Range limit.
                targetOffset.x = Mathf.Clamp(targetOffset.x, -areaSettings.width, areaSettings.width);
                targetOffset.z = Mathf.Clamp(targetOffset.z, -areaSettings.length, areaSettings.length);
            }

            //Lerp and update transform position.
            CurrentOffset = Vector3.Lerp(CurrentOffset, targetOffset, damper * Time.deltaTime);
            transform.position = areaSettings.center.position + CurrentOffset;
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