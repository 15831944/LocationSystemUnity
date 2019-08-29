using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Click_ButtonImageChange : MonoBehaviour {

    public GameObject Obj;
	void Start () {
        ChangeImageColor(Obj);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
  

         /// <summary>
         /// 改变图片颜色
         /// </summary>
         /// <param name="toggle"></param>
    public void ChangeImageColor(GameObject toggle)
    {
        EventTriggerListener color = EventTriggerListener.Get(toggle);

        color.onEnter = CheckImageColor;
        color.onExit = UncheckImageColor;
    }
    /// <summary>
    /// 点击时的颜色
    /// </summary>
    /// <param name="toggle"></param>
    public void CheckImageColor(GameObject toggle)
    {
        toggle.GetComponent<Text >().color = new Color(108 / 255f, 237 / 255f, 253 / 255f, 255 / 255f);
    }
    /// <summary>
    /// 未点击时的颜色
    /// </summary>
    /// <param name="toggle"></param>
    public void UncheckImageColor(GameObject toggle)
    {
        toggle.GetComponent<Text >().color = new Color(108/ 255f, 237/ 255f,253/ 255f, 100 / 255f);
    }




}
