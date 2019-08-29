using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGUIHelper : MonoBehaviour {

    //public Canvas canvas;//画布
    //private RectTransform rectTransform;//坐标

    void Start()
    {
        //canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        //rectTransform = GetComponent<RectTransform>();
    }

    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector2 pos;
    //        bool b1 = ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, canvas, out pos);
    //        Debug.LogError("b1:" + b1);
    //        if (b1)
    //        {
    //            Debug.Log(pos);
    //        }
    //    }
    //}

    /// <summary>
    /// 鼠标是否在RectTransform内，是返回true，并设置localPoint，否则返回false
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="screenPoint"></param>
    /// <param name="canvas"></param>
    /// <param name="localPoint"></param>
    /// <returns></returns>
    public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Canvas canvas, out Vector2 localPoint)
    {
        //Vector2 localPoint;
        //判断屏幕点是不是在rect中
        bool isContains = RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, canvas.worldCamera);
        if (isContains)
        {
            bool b1 = RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, canvas.worldCamera, out localPoint);
            //Debug.LogError("isContains:" + isContains);
            ////rectTransform.anchoredPosition = pos;
            //Debug.Log(localPoint);
            return true;
        }
        else
        {
            localPoint = Vector3.zero;
            return false;
        }

    }
}
