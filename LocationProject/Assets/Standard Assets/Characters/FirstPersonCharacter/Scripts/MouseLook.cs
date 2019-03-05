using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;

        public bool onlyLockCursor;//远程（不锁鼠标，只隐藏鼠标）

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;
        private Vector3 mousePositionOri;//用于远程，处理Axis为0的情况
        private float mouseSensitity = 0.1f;//与projectSetting->Input-Mouse X/Y 的Sensitity相同
        private float moveOffset=5;//靠近屏幕边缘，往回移动一定距离
        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }
        /// <summary>
        /// 设置鼠标初始位置
        /// </summary>
        /// <param name="inputPos"></param>
        public void SetMouseOrginalPos(Vector3 inputPos)
        {
            mousePositionOri = inputPos;
        }
        /// <summary>
        /// 获取鼠标移动的距离，Input.GetAxis("Mouse X")在远程桌面和TeamViewer中都是0，不能移动
        /// </summary>
        /// <returns></returns>
        private Vector2 GetMouseOffset()
        {
            Vector3 inputPos = Input.mousePosition;
            Vector3 offset = inputPos - mousePositionOri;
            //if(mousePos.x <= 0 || mousePos.y < 0 || mousePos.x > Screen.width || mousePos.y > Screen.height)
            if (inputPos.x <= 0) inputPos.x += moveOffset;
            else if (inputPos.x >= Screen.width) inputPos.x -= moveOffset;
            if (inputPos.y <= 0) inputPos.y += moveOffset;
            else if (inputPos.y >= Screen.height) inputPos.y -= moveOffset;
            mousePositionOri = inputPos;
            
            Vector2 rot = new Vector2(offset.x, offset.y) * mouseSensitity;
            //Debug.Log(string.Format("InputMosuePos:{0} ScreenSize:({1},{2})",mousePositionOri,Screen.width,Screen.height));
            return rot;
        }

        public void LookRotation(Transform character, Transform camera)
        {
            float yRot;
            float xRot;
            if(onlyLockCursor)
            {
                Vector2 rot = GetMouseOffset();
                yRot = rot.x * XSensitivity;
                xRot = rot.y * YSensitivity;
            }
            else
            {
                yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
                xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
            }           

            m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

            if(smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                if (!onlyLockCursor) Cursor.lockState = CursorLockMode.Locked;
                else Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                //if(!onlyLockCursor) Cursor.lockState = CursorLockMode.None;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
