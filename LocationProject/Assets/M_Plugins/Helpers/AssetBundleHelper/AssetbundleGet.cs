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

    private static void InitAssetPath()
    {
        AssetBundleHelper.SetUnityType(UnityTypeHelper.GetUnityType());
    }

    void OnGUI()
    {

    }

    void Update()
    {

    }
    

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    /// <param name="loadName"></param>
    /// <param name="suffixalName"></param>
    /// <param name="action"></param>
    public void GetObj(string loadName, string suffixalName, Action<UnityEngine.Object> action)
    {
        StartCoroutine(AssetBundleHelper.LoadAssetObjectWithNoDep(Application.dataPath, "StreamingAssets", loadName, suffixalName, action));
    }

    /// <summary>
    /// 读取一个资源,不需要加载依赖文件的使用该方法
    /// </summary>
    /// <param name="loadName"></param>
    /// <param name="suffixalName"></param>
    /// <param name="action"></param>
    public void GetObjWithParams(string loadName, string suffixalName, Action<AssetbundleParams> action)
    {
        StartCoroutine(AssetBundleHelper.LoadAssetObjectWithNoDep(Application.dataPath, "StreamingAssets", loadName, suffixalName, (obj)=>
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
