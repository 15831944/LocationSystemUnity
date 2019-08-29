using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTextColor : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {

    public Color HoverColor;
    public Color NormalColor;
    public Text ColorChangeText;
	// Use this for initialization
	void Start () {
		
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(ColorChangeText!=null)
        {
            ColorChangeText.color = HoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ColorChangeText != null)
        {
            ColorChangeText.color = NormalColor;
        }
    }
}
