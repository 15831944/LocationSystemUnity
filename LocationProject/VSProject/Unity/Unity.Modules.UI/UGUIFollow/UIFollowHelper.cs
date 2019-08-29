using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unity.Modules.UI.UGUIFollow
{
    public static class UIFollowHelper
    {
        /// <summary>
        /// 创建一项
        /// </summary>
        /// isCreateParent:是否创建父物体
        public static GameObject CreateItem(GameObject prefabUI, GameObject target, string groupName, Camera camT = null, bool isCreateParent = false, bool IsRayCheckCollision = false, bool isUseCanvasScalerT = true, int layerint = -1)
        {
            if (camT == null)
            {
                camT = GetCamera();
            }
            else
            {

            }
            UGUIFollowTarget followT = GetUIbyTarget(groupName, target);
            if (followT)
            {
                followT.gameObject.SetActive(true);
                return followT.gameObject;
            }
            GameObject ui = Instantiate(prefabUI);
            ui.SetActive(true);

            UGUIFollowTarget followTarget = UGUIFollowTarget.AddUGUIFollowTarget(ui, target, camT, isUseCanvasScalerT, layerint);
            //followTarget.IsRayCheckCollision = IsRayCheckCollision;
            followTarget.SetIsRayCheckCollision(IsRayCheckCollision);

            if (isCreateParent)
            {
                if (name_uiparent.ContainsKey(groupName))
                {
                    ui.transform.SetParent(name_uiparent[groupName].transform);
                }
                else
                {
                    CreateParent(groupName);
                    ui.transform.SetParent(name_uiparent[groupName].transform);
                }
            }
            else
            {
                ui.transform.SetParent(CommonFollowUIs);
            }

            ui.transform.localScale = Vector3.one;
            ui.transform.localEulerAngles = Vector3.zero;

            if (name_uilist.ContainsKey(groupName))
            {
                name_uilist[groupName].Add(ui);
            }
            else
            {
                name_uilist.Add(groupName, new List<GameObject>());
                name_uilist[groupName].Add(ui);
            }


            return ui;
        }
    }
}
