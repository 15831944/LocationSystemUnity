using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationUIManage : MonoBehaviour {

    public static LocationUIManage Instance;
    /// <summary>
    /// 窗体
    /// </summary>
    public GameObject window;
    //public Image photo;//头像
    public Sprite boyPhoto;//男头像
    public Sprite girlPhoto;//女头像

    // Use this for initialization
    void Start () {
        Instance = this;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show()
    {
        if (window != null)
        {
            window.SetActive(true);
        }
    }

    public void Hide()
    {
        if (window != null)
        {
            window.SetActive(false);
        }
    }

    /// <summary>
    /// 设置头像
    /// </summary>
    /// <param name="sexStr"></param>
    public void SetPhoto(Image photo, string sexStr)
    {
        if (sexStr.Contains("男"))
        {
            photo.sprite = boyPhoto;
        }
        else if (sexStr.Contains("女"))
        {
            photo.sprite = girlPhoto;
        }
    }
}
