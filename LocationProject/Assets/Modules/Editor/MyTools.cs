using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class MyTools
{

    /// <summary>
    /// 自动计算所有子对象包围盒
    /// </summary>
    [MenuItem("Tools/Collider/AddBoxCollider")]
    public static void AddBoxCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        ColliderHelper.CreateBoxCollider(parent);
    }

    /// <summary>
    /// 添加所有的MeshCollider
    /// </summary>
    [MenuItem("Tools/Collider/AddAllMeshCollider")]
    public static void AddAllMeshCollider()
    {
        Transform parent = Selection.activeGameObject.transform;
        AddAllMeshCollider(parent);
    }

    public static void AddAllMeshCollider(Transform parent)
    {
        List<MeshFilter> meshFilters = parent.gameObject.FindComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            MeshCollider meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
        //ColliderHelper.CreateBoxCollider(parent);
    }

    //[MenuItem("Tools/AddBoxColliderOld")]
    //public static void AddBoxColliderOld()
    //{
    //    Transform parent = Selection.activeGameObject.transform;
    //    ColliderHelper.CreateBoxColliderOld(parent);
    //}

    //   [MenuItem("Tools/Sockpol")]
    //public static void StartSockpol()
    //{
    //	//System.Diagnostics.Process.Start ("Tools\\SocketPolicyServer\\start.bat");
    //	System.IO.FileInfo file = new System.IO.FileInfo (Application.dataPath+@"\..\Tools\SocketPolicyServer\sockpol.exe");
    //	Debug.Log (file.FullName);
    //	System.Diagnostics.Process.Start(file.FullName, "--all");
    //}
    ///// <summary>
    ///// 添加输入框的聚焦检测
    ///// </summary>
    //[MenuItem("Tools/AddInputFocus")]
    //public static void AddInputFocus()
    //{
    //    Transform parent = Selection.activeGameObject.transform;
    //    InputField[] Inputs = parent.GetComponentsInChildren<InputField>(true);
    //    Debug.LogError("InputFiled Count:"+Inputs.Length);
    //    foreach(var item in Inputs)
    //    {
    //        item.gameObject.AddMissingComponent<InputFocus>();
    //        //Debug.LogError(item.transform.parent.name);
    //    }
    //}

    [MenuItem("MyTools/Windows Build With Postprocess")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { "Assets/Scenes/Loading.unity", "Assets/Scenes/Location1.0.unity" };

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/location.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        SystemSettingHelper.GetSystemSetting();

        GameObject versionObj = GameObject.Find("Version");
        if (versionObj)
        {
            Version v = versionObj.GetComponent<Version>();
            if (v)
            {
                SystemSettingHelper.systemSetting.VersionSetting.VersionNumber = v.VersionStr;
            }
        }
        SystemSettingHelper.SaveSystemSetting();

        string pathstr = path + "/SystemSetting.XML";
        if (File.Exists(pathstr))
        {
            FileUtil.DeleteFileOrDirectory(pathstr);
        }
        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");
        //FileUtil.CopyFileOrDirectory("SystemSetting.XML", path + "/SystemSetting.XML");
        FileUtil.CopyFileOrDirectory("SystemSetting.XML", pathstr);

        // Run the game (Process class from System.Diagnostics).
        //Process proc = new Process();
        //proc.StartInfo.FileName = path + "/BuiltGame.exe";
        //proc.Start();
    }
}
