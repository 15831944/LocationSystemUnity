using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTranslatePro : MouseTranslate
{
    /// <summary>
    /// 设置场景移动到该位置
    /// </summary>
    public void SetTranslatePosition(Vector3 pos)
    {
        targetOffset= pos - areaSettings.center.position;
    }
    public void ResetTranslateOffset()
    {
        targetOffset = Vector3.zero;
    }

}
