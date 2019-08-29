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
        Instance = this;
    }

    void Start()
    {
        Init();
    }

    [ContextMenu("Init")]
    public void Init()
    {
        LoadScene(SceneWhenCollapse);
    }

    public void LoadScene(string sceneName)
    {
        Log.Info("NavMeshSceneLoader.LoadScene", "start:"+ sceneName);
        var operation=SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        operation.completed += op =>
        {
            Log.Info("NavMeshSceneLoader.LoadScene", "end:" + sceneName);
            NavMeshHelper.RefreshNavMeshInfo();//这个要更新，不然算出来的位置就有问题
            if (LocationManager.Instance != null)
            {
                LocationManager.Instance.ShowLocation();//开始计算位置信息
            }
        };
        currentScene = sceneName;
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
        LoadScene(SceneWhenCollapse);
    }

    [ContextMenu("LoadSceneWhenExpand")]
    public void LoadSceneWhenExpand()
    {
        UnloadScene(SceneWhenCollapse);
        LoadScene(SceneWhenExpand);
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
