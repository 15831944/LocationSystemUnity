
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 历史轨迹运动过程中相关位置信息
/// </summary>
public class HisPosInfo: PosInfo
{
    /// <summary>
    /// 上一个历史点的信息
    /// </summary>
    public HisPosInfo PreHisInfo;

    public PositionInfo PrePosInfo;
    public PositionInfo CurrentPosInfo;
    public int currentIndex;

    public RaycastHit hitInfo;


    public Vector3 GetHitPos()
    {
        if (PrePosInfo == null)
        {
            return CurrentPosInfo.Vec;
        }
        if (Physics.Raycast(new Ray(PrePosInfo.Vec, CurrentPosInfo.Vec - PrePosInfo.Vec), out hitInfo))
        {
            return hitInfo.point;
        }
        else
        {
            return CurrentPosInfo.Vec;
        }
    }

    internal void ShowDebugPoints()
    {
        if (PreHisInfo != null)
        {
            PreHisInfo.DestroyPoints();
        }
        GameObject pos=CreateTestPoint(GetHitPos(), Color.blue);
    }
    private GameObject CreateTestPoint(Vector3 p,Color color)
    {
        GameObject pObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pObj.transform.position = p;
        pObj.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
        color.a = 0.5f;
        pObj.SetColor(color);
        Collider collider = pObj.GetComponent<Collider>();
        GameObject.Destroy(collider);

        testPosList.Add(pObj);
        return pObj;
    }

    private void DestroyPoints()
    {
        foreach (GameObject gameObject in testPosList)
        {
            GameObject.DestroyImmediate(gameObject);
        }
    }

    private List<GameObject> testPosList = new List<GameObject>();
}