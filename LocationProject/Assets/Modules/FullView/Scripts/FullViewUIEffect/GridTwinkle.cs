using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GridTwinkle : MonoBehaviour {
    /// <summary>
    /// 所有照片的父物体
    /// </summary>
    public GameObject imageListContent;
    /// <summary>
    /// 所有照片集合
    /// </summary>
    List<Image> imageList = new List<Image>();
    /// <summary>
    /// 是否正在动画
    /// </summary>
    private bool isTwinkle;
    /// <summary>
    /// 是否正在动画
    /// </summary>
    public bool IsTwinkle
    {
        get { return isTwinkle; }
    }
	// Use this for initialization
	void Start () {
        InitImageList();
    }
    /// <summary>
    /// 初始化所有照片
    /// </summary>
    private void InitImageList()
    {
        Image[] imageGroup = imageListContent.GetComponentsInChildren<Image>(true);
        imageList.AddRange(imageGroup);
        foreach (var item in imageList)
        {
            item.gameObject.AddComponent<GridTwinkleItem>();
            Color c = item.color;
            c.a = 0;
            item.color = c;
        }
    }
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.S))
        //{
        //    StartImageTwinkle();
        //}
        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    StopImageTwinkle();
        //}
    }
    public void TwinkleTest()
    {
        StartImageTwinkle();
        Invoke("StopImageTwinkle",1f);
    }
    /// <summary>
    /// 开始图片闪烁
    /// </summary>
    public void StartImageTwinkle()
    {
        isTwinkle = true;
        imageListContent.SetActive(true);
        foreach (var item in imageList)
        {
            GridTwinkleItem twinkleItem=item.gameObject.GetComponent<GridTwinkleItem>();
            twinkleItem.StartTwinkle();
        }
    }
    /// <summary>
    /// 停止闪烁
    /// </summary>
    public void StopImageTwinkle()
    {
        foreach(var item in imageList)
        {
            GridTwinkleItem twinkleItem = item.gameObject.GetComponent<GridTwinkleItem>();
            twinkleItem.StopTwinkle();
        }
        imageListContent.SetActive(false);
        isTwinkle = false;
    }
}
