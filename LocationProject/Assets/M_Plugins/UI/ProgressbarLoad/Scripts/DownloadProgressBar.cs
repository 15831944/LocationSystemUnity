using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadProgressBar : MonoBehaviour {

    public static DownloadProgressBar Instance;
    public GameObject window;//窗口
    public Slider slider;//进度条
    public Text txtValue;//进度条

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        if (slider == null)
        {
            slider = gameObject.GetComponentInChildren<Slider>(true);
        }
    }
    /// <summary>
    /// 显示进度条
    /// </summary>
    /// <param name="v">值</param>
    /// <param name="introduce">对应介绍</param>
    public void Show(float v,string msg)
    {
        slider.value = v;
        v = (float)Math.Round(v, 2);
        txtValue.text = string.Format("下载进度 ：{0}% ",v * 100);
        if(v>=1)
        {
            if (msg == "")
            {
                txtValue.text = "下载完成，准备解压安装...";
            }
            else
            {
                txtValue.text = msg;
            }
        }
        SetWindow(true);
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        SetWindow(false);
    }


    /// <summary>
    /// 设置窗口的显示隐藏
    /// </summary>
    /// <param name="isActive"></param>
    public void SetWindow(bool isActive)
    {
        if (window)
        {
            window.SetActive(isActive);
        }
    }
}
