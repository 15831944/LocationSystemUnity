using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class FVButtonColor : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler{

    private Text text;
	// Use this for initialization
	void Start () {
        text = transform.GetComponentInChildren<Text>(false);
	}
	
	public void OnPointerEnter(PointerEventData eventData)
    {
        if(text!=null)
        {
            Color c = text.color;
            c.a = 1;
            text.color = c;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (text != null)
        {
            Color c = text.color;
            c.a = 0.5f;
            text.color = c;
        }
    }
    private void OnDisable()
    {
        if (text != null)
        {
            Color c = text.color;
            c.a = 0.5f;
            text.color = c;
        }
    }
}
