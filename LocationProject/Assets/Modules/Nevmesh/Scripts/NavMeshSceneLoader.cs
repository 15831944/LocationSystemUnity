using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavMeshSceneLoader : MonoBehaviour
{
    public static NavMeshSceneLoader Instance;

    public string currentScene;

    public string SceneWhenCollapse;

    public string SceneWhenExpand;

    void Awake()
    {
        Log.Info("NavMeshSceneLoader.Awake");
        Instance = this;
        //Init();
    }

    void Start()
    {
        Log.Info("NavMeshSceneLoader.Start");
        Init();
    }

    [ContextMenu("Init")]
    public void Init()
    {
        Log.Info("NavMeshSceneLoader.Init");
        //LoadScene(SceneWhenCollapse);

        LoadSceneAsync(SceneWhenCollapse);
    }

    public void LoadScene(string sceneName)
    {
        try
        {
            Log.Info("NavMeshSceneLoader.LoadScene", "start:" + sceneName);
            SceneManager.LoadScene(sceneName);
            Log.Info("NavMeshSceneLoader.LoadScene", "end:" + sceneName);
            currentScene = sceneName;
        }
        catch (System.Exception ex)
        {
            Log.Error("NavMeshSceneLoader.LoadScene",ex.ToString());
        }
        
    }

    public void LoadSceneAsync(string sceneName)
    {
        try
        {
            Log.Info("NavMeshSceneLoader.LoadSceneAsync", "start:" + sceneName);
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (operation == null)
            {
                Log.Error("LoadSceneAsync", "加载场景失败:" + sceneName);
                return;
            }
            operation.completed += op =>
            {
                Log.Info("NavMeshSceneLoader.LoadSceneAsync", "end:" + sceneName);
                NavMeshHelper.RefreshNavMeshInfo();//这个要更新，不然算出来的位置就有问题
                if (LocationManager.Instance != null)
                {
                    LocationManager.Instance.ShowLocation();//开始计算位置信息
                }
            };
            currentScene = sceneName;
        }
        catch (System.Exception ex)
        {
            Log.Error("NavMeshSceneLoader.LoadSceneAsync", ex.ToString());
        }

    }

    public void UnloadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        Debug.Log(scene);
       
        if (scene != null && scene.name == sceneName)
        {
            Debug.Log(scene.name);
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    [ContextMenu("LoadSceneWhenCollapse")]
    public void LoadSceneWhenCollapse()
    {
        UnloadScene(SceneWhenExpand);
        LoadSceneAsync(SceneWhenCollapse);
    }

    [ContextMenu("LoadSceneWhenExpand")]
    public void LoadSceneWhenExpand()
    {
        UnloadScene(SceneWhenCollapse);
        LoadSceneAsync(SceneWhenExpand);
    }

    [ContextMenu("SwitchScene")]
    public void SwitchScene()
    {
        if (string.IsNullOrEmpty(currentScene))
        {
            LoadSceneWhenExpand();
        }
        else if (currentScene == SceneWhenCollapse)
        {
            LoadSceneWhenExpand();
        }
        else
        {
            LoadSceneWhenCollapse();
        }
    }
}
