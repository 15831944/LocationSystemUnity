#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class ReferenceSceneData{

    public List<AssetSceneDescription> assetList = new List<AssetSceneDescription>();

    public void ClearData()
    {
        assetList.Clear();
    }

    //根据引用信息状态获取状态描述
    public static string GetInfoByState(AssetSceneState state)
    {
        if (state == AssetSceneState.Ignore)
        {
            return "<color=#F0672AFF>Ignore</color>";
        }
        else if (state == AssetSceneState.MISSING)
        {
            return "<color=#FF0000FF>Missing</color>";
        }
        else if (state == AssetSceneState.NODATA)
        {
            return "<color=#FFE300FF>No Data</color>";
        }
        return "Normal";
    }

    public AssetSceneDescription SetAssetSceneDescription(string type,string property,string fullPath,GameObject obj,AssetSceneState state)
    {
        AssetSceneDescription des = new AssetSceneDescription();
        des.type = type;
        des.property = property;
        des.fullPath = fullPath;
        des.sceneObj = obj;
        des.state = state;
        if(assetList!=null)
        {
            assetList.Add(des);
        }
        else
        {
            assetList = new List<AssetSceneDescription>();
            assetList.Add(des);
        }
        return des;
    }

    public class AssetSceneDescription
    {
        public string type = "";
        public string property = "";
        public string fullPath = "";
        public GameObject sceneObj;
        public AssetSceneState state = AssetSceneState.NORMAL;

        /// <summary>
        /// 获取脚本上的属性，在场景中的绝对路径
        /// </summary>
        /// <returns></returns>
        public string GetPropertyPath()
        {
            return string.Format("{0}/{1}/{2}",fullPath,type,property);
        }
    }

    public enum AssetSceneState
    {
        NORMAL,
        Ignore,
        MISSING,
        NODATA,
    }
}
#endif