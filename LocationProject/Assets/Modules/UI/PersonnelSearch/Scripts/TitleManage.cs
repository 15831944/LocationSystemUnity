using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManage : MonoBehaviour {
    public static TitleManage Instance;
    public GameObject   DropdownImage;
    public Sprite  normalImage;
    public Sprite  hovelImage;
    public Sprite  activeImage;
    void Start () {
        Instance = this;
        ClickChange(DropdownImage);

    }
	public void ClickChange(GameObject obj)
    {
        EventTriggerListener objImage = EventTriggerListener.Get(obj);
        objImage.onHover = EnterBut;
        objImage.onExit = ExitBut;
        objImage.onClick = ClickBut;
    }
    public void EnterBut(GameObject obj)
    {
        DropdownImage.GetComponent<Image>().sprite = hovelImage;

    }
    public void ExitBut(GameObject obj )
    {
        DropdownImage.GetComponent<Image>().sprite = normalImage;
    }
    public void ClickBut(GameObject obj)
    {
        DropdownImage.GetComponent<Image>().sprite = activeImage;
    }
void Update () {
		
	}
}
