using UnityEngine;
using System.Collections;
//using System;
using UnityEngine.UI;//MonoBehaviour
using System.Collections.Generic;
using System;
using Unity.Common.Consts;
using Unity.Common.Utils;

public class AssetbundleGet : MonoBehaviour
{
    public static AssetbundleGet Instance;

    public bool DeviceFromFile = true;
   // public UnityEngine.Object Obj;

    public void SetInstance()
    {
        Instance = this;
    }



    void Awake()
    {
        Instance = this;

        InitAssetPath();//因为AssetBundleHelper是放到dll中的，dll里面#elif UNITY_EDITOR这些是无效的
    }

    public bool InitCommonModels = true;

    public List<string> commonModels = new List<string>();

    //public List<GameObject> loadModels = new List<GameObject>();

    void Start()
    {
        if (InitCommonModels)
        {
            if (commonModels.Count == 0)
            {
                commonModels.Add("监控区域1_3D");
                commonModels.Add("大华_摄像头2_3D");//todo:写到配置文件中
                commonModels.Add("定位设备2_3D");
                commonModels.Add("定位设备1_3D");
                commonModels.Add("其他_单联单控开关_3D");
            }
            StartCoroutine(LoadCommonModels(commonModels,false));//提前加载模型，提高设备加载效率。
        }
        
    }

    public static IEnumerator LoadCommonModels(List<string> models,bool showProgress)
    {
        for (int i = 0; i < models.Count; i++)
        {
            string model = models[i];
            if (string.IsNullOrEmpty(model)) continue;
            GameObject modelT = ModelIndex.Instance.Get(model);
            if (modelT != null)
            {
                models.RemoveAt(i); i--;//这种删除i--不能忘记
            }
        }

        for (int i = 0; i < models.Count; i++)
        {
            if (showProgress)
            {
                float percent = (i + 1) / models.Count;
                ProgressbarLoad.Instance.Show(percent);//模型进度
            }
            string model = models[i];
            yield return AssetBundleHelper.LoadAssetObject("Devices", model, AssetbundleGetSuffixalName.prefab, obj =>
            {
                if (obj == null)
                {
                    Debug.LogError("获取不到模型:" + model);
                }
                else
                {
                    GameObject g = obj as GameObject;
                    ModelIndex.Instance.Add(g, model); //添加到缓存中
                    Debug.LogError("Info:加载模型:" + model);
                }
            }); //携程方式读取模型文件
        }
    }




    public void InitAssetPath()
    {
        AssetBundleHelper.SetUnityType(UnityTypeHelper.GetUnityType());
        AssetBundleHelper.IsFromHttp = !DeviceFromFile;
    }

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    /// <param name="loadName"></param>
    /// <param name="suffixalName"></param>
    /// <param name="action"></param>
    public void GetObj(string loadName, string suffixalName, Action<UnityEngine.Object> action)
    {
        StartCoroutine(AssetBundleHelper.LoadAssetObject("Devices", loadName, suffixalName, action));
    }

    ///// <summary>
    ///// 读取一个资源,不需要加载依赖文件的使用该方法
    ///// </summary>
    //public IEnumerator GetObj(string loadName, string suffixalName, Action<UnityEngine.Object> action)
    //{
    //    yield return AssetBundleHelper.LoadAssetObject("Devices", loadName, suffixalName, action));
    //}

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    public IEnumerator GetObjFromCatch(string loadName, string suffixalName, Action<UnityEngine.Object> action)
    {
        GameObject modelT = ModelIndex.Instance.Get(loadName);
        if (modelT != null)
        {
            if (action != null)
            {
                action(modelT);
            }
            //yield return null;
        }
        else
        {
            yield return AssetBundleHelper.LoadAssetObject("Devices", loadName, suffixalName, action); //内部也是LoadAssetObject
        }
    }

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    /// <param name="loadName"></param>
    /// <param name="suffixalName"></param>
    /// <param name="action"></param>
    public void GetObjWithParams(string loadName, string suffixalName, Action<AssetbundleParams> action)
    {
        StartCoroutine(AssetBundleHelper.LoadAssetObject("Devices", loadName, suffixalName, (obj)=>
        {
            if (action != null)
            {
                AssetbundleParams args = new AssetbundleParams();
                args.name = loadName;
                args.obj = obj;
                action(args);
            }
        }));
    }


}
