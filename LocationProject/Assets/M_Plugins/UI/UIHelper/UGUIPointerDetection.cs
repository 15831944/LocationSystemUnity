using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// UGUI鼠标射线检测
/// </summary>
public class UGUIPointerDetection : MonoBehaviour
{
    public static List<RaycastResult> currentResults;

    //public static UGUIPointerDetection instance;

    void Start()
    {
        //instance = this;
    }

    void Update()
    {
        //测试
        //if (IsPointerOverUIObjectA())
        //{
        //    if (currentResults.Count > 0)
        //    {
        //        TPV_Windowsbase t = raycastResults[0].gameObject.GetComponentInParent<TPV_Windowsbase>();
        //        if (t != null)
        //        {
        //            Debug.Log("TPV_Windowsbase");
        //        }
        //        Debug.Log("xxxxxx");
        //    }


        //}
        //else
        //{


        //}
    }

    public static bool IsPointerOverUIObjectS(Canvas canvas, Vector2 screenPosition)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenPosition;

        GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(eventDataCurrentPosition, results);
        //if (results.Count > 0)
        //{
        //    int i = 0;
        //}
        currentResults = results;
        return results.Count > 0;
    }

    public bool IsPointerOverUIObjectA()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        //if (results.Count > 0)
        //{
        //    int i = 0;
        //}
        currentResults = results;
        return results.Count > 0;
    }

    public static List<RaycastResult> IsPointerOverUIObjectB()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        //if (results.Count > 0)
        //{
        //    int i = 0;
        //}
        //currentResults = results;
        return results;
    }


}
