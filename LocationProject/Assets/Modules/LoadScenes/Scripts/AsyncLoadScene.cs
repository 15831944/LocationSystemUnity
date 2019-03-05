using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadScene : MonoBehaviour
{

    public static AsyncLoadScene Instance;
    //public Slider loadingSlider;

    //public Text loadingText;

    private float loadingSpeed = 1;

    private float progressBeforeValue;//之前的进度值

    private float targetProgressValue;//目标进度值

    float currentProgressValue;//当前进度值

    private AsyncOperation operation;

    public string nextSceneName;//加载的下个场景

    private Action callAction;//场景加载结束回调

    public bool isLoadStart = false;//是否在Start里面开始加载

    public Image backImage;//背景图

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        Debug.unityLogger.logEnabled = false;
        if (isLoadStart)
        {
            progressBeforeValue = 0;
            currentProgressValue = 0;
            targetProgressValue = 0;
            //loadingSlider.value = 0.0f;
            ProgressbarLoad.Instance.Show(0f);

            if (SceneManager.GetActiveScene().name == "Loading")
            {
                //启动协程
                StartCoroutine(AsyncLoading());
                //StartCoroutine(AsyncLoadingAdd());
            }
        }
    }

    IEnumerator AsyncLoading()
    {
        yield return new WaitForEndOfFrame();//加上这么一句就可以先显示加载画面然后再进行加载
        //operation = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        operation = SceneManager.LoadSceneAsync(nextSceneName);
        //阻止当加载完成自动切换
        operation.allowSceneActivation = false;
        operation.completed += Operation_completed;
        yield return operation;
    }
    IEnumerator AsyncLoadingAdd()
    {
        yield return new WaitForEndOfFrame();//加上这么一句就可以先显示加载画面然后再进行加载
        operation = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        //阻止当加载完成自动切换
        operation.allowSceneActivation = false;
        operation.completed += Operation_completed;
        yield return operation;
    }

    private void Operation_completed(AsyncOperation obj)
    {
        operation = null;
        GC.Collect();
        if (backImage)
        {
            backImage.gameObject.SetActive(false);
        }
        ProgressbarLoad.Instance.Hide();
        Debug.LogError("加载场景：" + nextSceneName + "完成");

        if (callAction != null)
        {
            callAction();
        }
        //throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        if (operation == null) return;
        progressBeforeValue = targetProgressValue;
        targetProgressValue = operation.progress;
        Debug.LogError("operation.progress:" + Math.Round(operation.progress, 3));
        if (Math.Round(operation.progress, 3) >= 0.9)//最大值是0.9f
        {
            targetProgressValue = 1f;
        }

        if (currentProgressValue < targetProgressValue)
        {
            //插值运算
            currentProgressValue = Mathf.Lerp(currentProgressValue, targetProgressValue, Time.deltaTime * loadingSpeed);
            ProgressbarLoad.Instance.Show(currentProgressValue);
            Debug.LogError("currentProgressValue:" + currentProgressValue);
            //if (Mathf.Abs(currentProgressValue - targetProgressValue) < 0.01f)
            //{
            //    ProgressbarLoad.Instance.Show(targetProgressValue);
            //}
        }
        //}
        if (Math.Round(currentProgressValue, 3) == 1f)//unity自己的BUG在加载到最后一直是0.9；
        {
            //允许异步加载完毕后自动切换场景
            operation.allowSceneActivation = true;

            //ProgressbarLoad.Instance.Hide();
            //Debug.LogError("LoadPark!000@");
            //operation = null;
            //if (callAction != null)
            //{
            //    callAction();
            //}
        }
    }


    //// Update is called once per frame
    //void Update()
    //{
    //    if (operation == null) return;
    //    progressBeforeValue = targetProgressValue;
    //    targetProgressValue = operation.progress;

    //    //插值运算
    //    currentProgressValue = Mathf.Lerp(progressBeforeValue, targetProgressValue, Time.deltaTime * loadingSpeed);
    //    ProgressbarLoad.Instance.Show(currentProgressValue);
    //    Debug.LogError("currentProgressValue:" + currentProgressValue);
    //    if (Mathf.Abs(currentProgressValue - targetProgressValue) < 0.01f)
    //    {
    //        ProgressbarLoad.Instance.Show(targetProgressValue);
    //    }
    //    //}
    //    if (currentProgressValue >= 0.9f)//unity自己的BUG在加载到最后一直是0.9；
    //    {
    //        //允许异步加载完毕后自动切换场景
    //        operation.allowSceneActivation = true;
    //        ProgressbarLoad.Instance.Hide();
    //        //operation = null;

    //        Debug.LogError("LoadPark!000@");
    //        operation = null;
    //        if (callAction != null)
    //        {
    //            callAction();
    //        }
    //    }
    //}

    /// <summary>
    /// 加载场景
    /// </summary>
    public void LoadAdd(string scenename, Action callActionT = null)
    {
        nextSceneName = scenename;
        if (string.IsNullOrEmpty(nextSceneName))
        {
            return;
        }
        callAction = callActionT;
        ProgressbarLoad.Instance.Show(0);
        StartCoroutine(AsyncLoadingAdd());

    }

    /// <summary>
    /// 
    /// </summary>
    [ContextMenu("LoadPark")]
    public void LoadPark()
    {
        LoadAdd("Park", () =>
         {
             Debug.LogError("LoadPark!@");
         });
    }
}
