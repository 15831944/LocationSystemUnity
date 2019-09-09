using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 加载条，在Loading场景和主场景都有一个，方便主场景调试，
/// 因为在开发阶段每次都要通过Loading场景加载主场景，影响工作效率，
/// 虽然存在两个实例，但是不影响正常使用，
/// </summary>
public class ProgressbarLoad : MonoBehaviour {
    public static ProgressbarLoad Instance;
    public GameObject window;//窗口
    public Slider slider;//进度条
    public Text txtValue;//进度条

    public Text progressText;//进度条

    public string initText;

    void Awake()
    {
        Instance = this;
    } 

    // Use this for initialization
    void Start () {

        if (slider == null)
        {
            slider = gameObject.GetComponentInChildren<Slider>(true);
        }

        slider.onValueChanged.AddListener(Slider_OnValueChanged);

        if (progressText != null)
        {
            initText = progressText.text;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 显示进度条
    /// </summary>
    /// <param name="v"></param>
    public void Show(float v,string txt=null)
    {
        //if (v >= 0.1f)
        //{
            slider.value = v;
        //}
        //   else
        //{
        //    slider.value = 0.1f;
        //}
        if (!string.IsNullOrEmpty(txt))
        {
            if (progressText != null)
            {
                progressText.text = txt;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(initText))
            {
                progressText.text = initText;
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
    /// Slider_OnValueChanged
    /// </summary>
    /// <param name="v"></param>
    private void Slider_OnValueChanged(float v)
    {
        //if (v >= 0.1f)
        //{
            v = (float)Math.Round(v, 2);
            txtValue.text = "" + v * 100;
        //}
        
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
