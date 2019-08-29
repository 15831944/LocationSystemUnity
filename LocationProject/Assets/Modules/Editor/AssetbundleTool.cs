using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class AssetbundleTool : MonoBehaviour {

	static string GetStreamingAssetsDir(){
		string targetPath = Application.dataPath + "/StreamingAssets/";
		if (!Directory.Exists (targetPath)) {
			Directory.CreateDirectory(targetPath);
		}
		return targetPath;
	}

    //[MenuItem("EditorTool/Create AssetBunldes Main")]
    //static void CreateAssetBunldesMain()
    //{
    //    //获取在Project视图中选择的所有游戏对象
    //    Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    //    string streamingAssetDir = GetStreamingAssetsDir();
    //    //遍历所有的游戏对象
    //    foreach (Object obj in SelectedAsset)
    //    {
    //        string sourcePath = AssetDatabase.GetAssetPath(obj);
    //        //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
    //        //StreamingAssets是只读路径，不能写入
    //        //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
    //        Debug.Log("dataPaht:" + Application.dataPath);
    //        string targetPath = streamingAssetDir + obj.name + ".assetbundle";
    //        if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies))
    //        {
    //            Debug.Log(obj.name + "资源打包成功");
    //        }
    //        else
    //        {
    //            Debug.Log(obj.name + "资源打包失败");
    //        }
    //    }
    //    //刷新编辑器
    //    AssetDatabase.Refresh();
    //}

    //[MenuItem("EditorTool/Create AssetBunldes ALL")]
    //static void CreateAssetBunldesALL ()
    //{

    //	Caching.CleanCache ();
    //	string Path = GetStreamingAssetsDir()+"ALL.assetbundle";
    //	Object[] SelectedAsset = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
    //	foreach (Object obj in SelectedAsset) {
    //		Debug.Log ("Create AssetBunldes name :" + obj);
    //	}
    //	//这里注意第二个参数就行
    //	if (BuildPipeline.BuildAssetBundle (null, SelectedAsset, Path, BuildAssetBundleOptions.CollectDependencies)) {
    //		AssetDatabase.Refresh ();
    //	} else {
    //	}
    //}

    [MenuItem("EditorTool/Build AssetBunldes")]//创建AssetBunldeName
    static void CreateAssetBunldesMain()
    {
        Caching.ClearCache();//清除缓存，才能保证每次获取的依赖资源是最新的(方法.GetAllDependencies())
        string path = GetStreamingAssetsDir();
//#if UNITY_5_3
//        BuildPipeline.BuildAssetBundles(path);
//        //#elif UNITY_5_4
//#else
        //BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
//#endif
        AssetDatabase.Refresh();

    }

    /// <summary>
    /// 将所有文件夹内的文件打包到一个AssetBundle中
    /// 针对放模型的“文件夹”设置AssetBunldeName，文件夹可以包括模型、材质、贴图等
    /// </summary>
    [MenuItem("EditorTool/SetDirectoryToAssetBundle")]
    static void SetDirectoryToAssetBundle()
    {
        //获取在Project视图中选择的所有游戏对象
        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0) return;//选中的没有文件夹
        foreach (Object dirObj in dirObjs)
        {
            var dir = GetDirInfo(dirObj);
            SetFilesAssetName(dir);
        }
        CreateAssetBunldesMain();
        //刷新编辑器
        AssetDatabase.Refresh();
    }

    private static List<Object> GetDirObjs()
    {
        Object[] selectedAssets = Selection.GetFiltered(typeof (Object), SelectionMode.Assets);
        //Object[] SelectedAssetSub = Selection.GetFiltered(typeof(Object), SelectionMode.Editable);//不包括不可修改的文件,文件夹属于不可修改的文件
        //List<Object> SelectedAssetList = new List<Object>(SelectedAsset);
        List<Object> dirObjs = new List<Object>();
        foreach (Object obj in selectedAssets)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            if (Directory.Exists(sourcePath)) //判断是否是文件夹
            {
                dirObjs.Add(obj);
            }
        }
        return dirObjs;
    }

    private static void SetFilesAssetName(DirectoryInfo dir)
    {
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories); //搜索文件夹下面的所有文件

        List<Object> subObjects = new List<Object>();
        Regex regex = new Regex(".(meta|cs|xml)$"); //正则表达式，过滤后缀为.meta.cs.xml的文件
        foreach (FileInfo file in files)
        {
            string filePath = file.FullName;
            //int index = filePath.IndexOf(Application.dataPath);
            string relativePath = filePath.Substring(Application.dataPath.Length + 1);
            relativePath = "Assets\\" + relativePath;
            //List<Object> subs = new List<Object>();
            //subs.Add(AssetDatabase.LoadAssetAtPath<Object>(relativePath));//如果路径文件后缀为.meta,则AssetDatabase.LoadAssetAtPath<Object>(relativePath)返回为null；
            if (regex.IsMatch(relativePath))
            {
                continue;
            }
            Object subObj = AssetDatabase.LoadAssetAtPath<Object>(relativePath);
            subObjects.Add(subObj);
        }

        string assetName = dir.Name;
#if UNITY_ANDROID || UNITY_WEBGL
        //assetName = PinYinConverter.Get(assetName);
#endif

        List<string> assets=new List<string>();

        //遍历所有的游戏对象
        foreach (Object subObj in subObjects)
        {
            string sPath = AssetDatabase.GetAssetPath(subObj);
            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            //Debug.Log("dataPaht:" + Application.dataPath);

            AssetImporter importer = AssetImporter.GetAtPath(sPath);
            //importer.assetBundleName = sourcePath;
            importer.assetBundleName = assetName;

            //#if UNITY_ANDROID || UNITY_WEBGL
            //            sPath = PinYinConverter.Get(sPath);
            //#endif
            assets.Add(sPath);
        }
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        AssetBundleBuild bundle = new AssetBundleBuild();
        bundle.assetBundleName = assetName;
        bundle.assetNames = assets.ToArray();
        builds.Add(bundle);
        //string path = GetStreamingAssetsDir();

        //#if UNITY_5_3
        //        BuildPipeline.BuildAssetBundles(path, builds.ToArray());
        //        //BuildPipeline.BuildAssetBundles(path, builds.ToArray(), 0, EditorUserBuildSettings.activeBuildTarget);
        //        //#elif UNITY_5_4
        //#else
        //AssetBundleManifest manifest=BuildPipeline.BuildAssetBundles(path, builds.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        //manifest.GetAllAssetBundles()
        //#endif

    }


    private static DirectoryInfo GetDirInfo(Object dirObj)
    {
        string sourcePath = AssetDatabase.GetAssetPath(dirObj);
        DirectoryInfo dir = new DirectoryInfo(sourcePath); //获取文件夹
        return dir;
    }

    /// <summary>
    /// 将所有文件夹内的文件打包到一个AssetBundle中
    /// SetDirectoryToAssetBundle的扩充，选中一个根目录就能把里面所有的有模型的文件夹单个打包
    /// </summary>
    [MenuItem("EditorTool/SetDirectoryToAssetBundleEx")]
    static void SetDirectoryToAssetBundleEx()
    {
        //获取在Project视图中选择的所有游戏对象
        var dirObjs = GetDirObjs();
        if (dirObjs.Count == 0) return;//选中的没有文件夹
        foreach (Object dirObj in dirObjs)
        {
            var dir = GetDirInfo(dirObj);
            SetDirAssetName(dir);
        }

        CreateAssetBunldesMain();

        //刷新编辑器
        AssetDatabase.Refresh();

    }

    private static void SetDirAssetName(DirectoryInfo dir)
    {
        DirectoryInfo[] subDirs = dir.GetDirectories();
        if (subDirs.Length == 1 && subDirs[0].Name == "Materials")
        {
            SetFilesAssetName(dir);
        }
        else
        {
            foreach (DirectoryInfo subDir in subDirs)
            {
                SetDirAssetName(subDir);
            }
        }
    }

    /// <summary>
    /// 一个“文件”生成一个AssetBunldeName，文件可以包括模型、材质、贴图等
    /// </summary>
    [MenuItem("EditorTool/SetFilesToAssetBundle")]
    static void SetFilesToAssetBundle()
    {
        //获取在Project视图中选择的所有游戏对象，不包括文件夹
        //Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Editable);//不包括不可修改的文件,文件夹属于不可修改的文件

        //获取在Project视图中选择的所有游戏对象
        //Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //Object[] SelectedAssetSub = Selection.GetFiltered(typeof(Object), SelectionMode.Editable);//不包括不可修改的文件,文件夹属于不可修改的文件

        //List<Object> SelectedAssetList = new List<Object>(SelectedAsset);
        //return;
        List<Object> SelectedAssetSub = new List<Object>();//不包括文件夹
        //foreach (Object o in SelectedAssetSub)
        //{
        //    if (SelectedAssetList.Contains(o))
        //    {
        //        SelectedAssetList.Remove(o);
        //    }
        //}

        foreach (Object o in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(o);
            if (File.Exists(sourcePath))//File.Exists()判断是否是文件,过滤文件夹
            {
                SelectedAssetSub.Add(o);
            }

        }

        if (SelectedAssetSub.Count == 0) return;//选中的没有文件

        //遍历所有的游戏对象
        foreach (Object obj in SelectedAssetSub)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            Debug.Log("dataPaht:" + Application.dataPath);
            AssetImporter importer = AssetImporter.GetAtPath(sourcePath);
            importer.assetBundleName = obj.name;

        }
        //刷新编辑器
        AssetDatabase.Refresh();
    }

    [MenuItem("EditorTool/Clear AssetBunldesName")]//清空AssetBunldeName
    static void ClearAssetBunldesName()
    {
        //获取在Project视图中选择的所有游戏对象，不包括文件夹
        //Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Editable);//不包括不可修改的文件,文件夹属于不可修改的文件

        //获取在Project视图中选择的所有游戏对象
        //Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //Object[] SelectedAssetSub = Selection.GetFiltered(typeof(Object), SelectionMode.Editable);//不包括不可修改的文件,文件夹属于不可修改的文件

        //List<Object> SelectedAssetList = new List<Object>(SelectedAsset);
        //return;
        List<Object> SelectedAssetSub = new List<Object>();//不包括文件夹
        //foreach (Object o in SelectedAssetSub)
        //{
        //    if (SelectedAssetList.Contains(o))
        //    {
        //        SelectedAssetList.Remove(o);
        //    }
        //}

        foreach (Object o in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(o);
            if (File.Exists(sourcePath))//File.Exists()判断是否是文件,过滤文件夹
            {
                SelectedAssetSub.Add(o);
            }

        }

        if (SelectedAssetSub.Count == 0) return;//选中的没有文件

        //遍历所有的游戏对象
        foreach (Object obj in SelectedAssetSub)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            Debug.Log("dataPaht:" + Application.dataPath);
            AssetImporter importer = AssetImporter.GetAtPath(sourcePath);
            importer.assetBundleName = ""; 

        }
        //刷新编辑器
        AssetDatabase.Refresh();
    }
}
