using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour {

 
    public Sprite normalImage;
    public Sprite hovelImage;
    public Sprite activeImage;
    void Start()
    {
        
        ClickChange(this .gameObject );

    }
    public void ClickChange(GameObject obj)
    {
        EventTriggerListener objImage = EventTriggerListener.Get(obj);
        objImage.onEnter  = EnterBut;
        objImage.onExit = ExitBut;
        objImage.onClick  = ClickBut;
    }
    public void EnterBut(GameObject obj)
    {
        this .gameObject .GetComponent<Image>().sprite = hovelImage;

    }
    public void ExitBut(GameObject obj)
    {
       this .gameObject .GetComponent<Image>().sprite = normalImage;
    }
    public void ClickBut(GameObject obj)
    {
        this .gameObject .GetComponent<Image>().sprite = activeImage;
    }
}
