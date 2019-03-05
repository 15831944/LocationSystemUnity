using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewChange : MonoBehaviour {
    public static UIViewChange instance;
    public GameObject TagObj;
    public Transform TagUi;
    public Camera camera;
    public CanvasScaler canvasScaler;
    void Start () {
        ViewChange();
    }
      
    
    public void ViewChange()
    {
        Vector3 pos = camera.WorldToScreenPoint(TagObj.transform.localPosition);
        float yOffset = 0;
        float xOffset = 0;
        Rect rec = camera.rect;
        yOffset = Screen.height * (rec.height / 2 + rec.y - 0.5f);
        xOffset = Screen.width * (rec.width / 2 + rec.x - 0.5f);

        Vector2 uiPos2 = new Vector2(pos.x - (Screen.width / 2) - xOffset, pos.y - (Screen.height / 2) - yOffset);
      
        if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
        {
            uiPos2 = uiPos2 / canvasScaler.scaleFactor;
        }
        else if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            uiPos2 = new Vector2(uiPos2.x * canvasScaler.referenceResolution.x / Screen.width, uiPos2.y * canvasScaler.referenceResolution.y / Screen.height);
        }
       Vector3 p = new Vector3(uiPos2.x, uiPos2.y, 0);
    }
}
