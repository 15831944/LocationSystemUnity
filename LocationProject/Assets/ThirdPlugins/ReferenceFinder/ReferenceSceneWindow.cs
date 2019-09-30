#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using System;
using Base.Common;
using UnityEngine.SceneManagement;

public class ReferenceSceneWindow : EditorWindow
{
    private static ReferenceSceneData data = new ReferenceSceneData();

    private AssetTreeView m_AssetTreeView;

    [SerializeField]
    private TreeViewState m_TreeViewState;

    private bool needUpdateAssetTree = false;

    //选中资源列表
    private List<GameObject> selectedSceneObjs = new List<GameObject>();

    private bool initializedGUIStyle = false;

    //工具栏按钮样式
    private GUIStyle toolbarButtonGUIStyle;
    //工具栏样式
    private GUIStyle toolbarGUIStyle;

    //打开窗口
    [MenuItem("Window/RefrenceInScene Finder", false, 1000)]
    static void OpenWindow()
    {
        ReferenceSceneWindow window = GetWindow<ReferenceSceneWindow>();
        window.wantsMouseMove = false;
        window.titleContent = new GUIContent("Reference in Secne Finder");
        window.Show();
        window.Focus();
    }
    private void OnGUI()
    {
        InitGUIStyleIfNeeded();
        DrawOptionBar();
        UpdateAssetTree();
        if (m_AssetTreeView != null)
        {
            //绘制Treeview
            m_AssetTreeView.OnGUI(new Rect(0, toolbarGUIStyle.fixedHeight, position.width, position.height - toolbarGUIStyle.fixedHeight));
        }
    }

    //绘制上条
    public void DrawOptionBar()
    {
        EditorGUILayout.BeginHorizontal(toolbarGUIStyle);
        if (GUILayout.Button("Refresh Data", toolbarButtonGUIStyle))
        {
            recordTime = DateTime.Now;
            Debug.LogError("Start Refresh Data...");
            CollectDependencyData();
            needUpdateAssetTree = true;
            EditorGUIUtility.ExitGUI();
        }

        GUILayout.FlexibleSpace();

        //扩展
        if (GUILayout.Button("Expand", toolbarButtonGUIStyle))
        {
            if (m_AssetTreeView != null) m_AssetTreeView.ExpandAll();
        }
        //折叠
        if (GUILayout.Button("Collapse", toolbarButtonGUIStyle))
        {
            if (m_AssetTreeView != null) m_AssetTreeView.CollapseAll();
        }
        EditorGUILayout.EndHorizontal();
    }
    /// <summary>
    /// 获取场景中的物体
    /// </summary>
    /// <returns></returns>
    private static GameObject[] GetSceneObjects()
    {
        return Resources.FindObjectsOfTypeAll<GameObject>().Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go)) && go.hideFlags == HideFlags.None).ToArray();
    }
    /// <summary>
    /// 获取场景中所有物体
    /// </summary>
    private void CollectDependencyData()
    {
        var objects = GetSceneObjects();
        selectedSceneObjs = objects.ToList();
        //EditorUtility.DisplayCancelableProgressBar("Refresh", "Generating asset reference info", 3f);
        //EditorUtility.ClearProgressBar();
    }
    //初始化GUIStyle
    void InitGUIStyleIfNeeded()
    {
        if (!initializedGUIStyle)
        {
            toolbarButtonGUIStyle = new GUIStyle("ToolbarButton");
            toolbarGUIStyle = new GUIStyle("Toolbar");
            initializedGUIStyle = true;
        }
    }
    private DateTime recordTime;
    //通过选中资源列表更新TreeView
    private void UpdateAssetTree()
    {
        if (needUpdateAssetTree && selectedSceneObjs.Count != 0)
        {
            LoadIgnoreList();
            var root = SelectedAssetGuidToRootItem(selectedSceneObjs);
            if (m_AssetTreeView == null)
            {
                //初始化TreeView
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();
                var headerState = AssetTreeView.CreateDefaultMultiColumnHeaderState(position.width);
                var multiColumnHeader = new MultiColumnHeader(headerState);
                m_AssetTreeView = new AssetTreeView(m_TreeViewState, multiColumnHeader);
            }
            m_AssetTreeView.assetRoot = root;
            m_AssetTreeView.CollapseAll();
            m_AssetTreeView.Reload();
            m_AssetTreeView.currentWindow = this;
            needUpdateAssetTree = false;
            ShowNotification(new GUIContent(string.Format("搜索完毕，用时：{0}s",(DateTime.Now-recordTime).TotalSeconds.ToString("f2"))));
        }
    }

    private static string FullPath(GameObject go)
    {
        return go.transform.parent == null ? go.name : FullPath(go.transform.parent.gameObject) + "/" + go.name;
    }
    //一些系统自带的组件，这些组件上，引用大都为None.因此，不参与引用为none的检测，只检测missing
    private static string IgnoreCompoment = "Image|Text|MeshFilter|MeshRenderer|Button|MeshCollider|BoxCollider|ParticleSystemRenderer|InputField|ParticleSystem|LayoutElement";
    private static string IgnoreComponent2 = "Scrollbar|Toggle|ScrollRect|Selectable|ImageAdvanced|Dropdown|Slider|RawImage";
    /// <summary>
    /// 是否需要检测的组件
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsNotCheckedComponent(string typeName)
    {
        if (IgnoreCompoment.Contains(typeName) || IgnoreComponent2.Contains(typeName)) return true;
        else return false;
    }

    //通过选择资源列表生成TreeView的根节点
    private AssetViewItem SelectedAssetGuidToRootItem(List<GameObject> selectedSceneObjList)
    {
        int elementCount = 0;
        data.ClearData();
        var root = new AssetViewItem { id = elementCount, depth = -1, displayName = "Root", data = null };
        int depth = 0;
        Scene currentScene= SceneManager.GetActiveScene();
        string sceneName = currentScene == null ? "" : currentScene.name;
        foreach (var item in selectedSceneObjList)
        {          
            Component[] components = item.GetComponents<Component>();
            foreach (var c in components)
            {
                if (!c)
                {
                    Debug.LogError("Missing Component in GO: " + FullPath(item), item);
                    continue;
                }
                SerializedObject so = new SerializedObject(c);
                var sp = so.GetIterator();
                bool isScripts = false;
                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        //目前只处理脚本上，引用丢失或者为空的情况
                        string typeName = c.GetType().Name;
                        string propertyName = ObjectNames.NicifyVariableName(sp.name);
                        if (!isScripts)
                        {
                            isScripts = propertyName == "Script" ? true : false;
                        }
                        if (!isScripts || propertyName == "Script") continue;
                        if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                        {
                            AddChild(sceneName, typeName,propertyName,item,ref elementCount,depth, root, ReferenceSceneData.AssetSceneState.MISSING);
                        }
                        else if(sp.objectReferenceInstanceIDValue == 0)
                        {                            
                            if (IsNotCheckedComponent(typeName)) continue;//脚本才检测无引用的情况，常规Image等组件，不检测
                            AddChild(sceneName,typeName, propertyName, item,ref elementCount, depth, root, ReferenceSceneData.AssetSceneState.NODATA);
                        }
                    }
                }
            }
        }
        Debug.LogError("RootCount:"+root.children.Count);
        SortList(root);
        return root;
    }

    public static string ConfigPath = "\\StreamingAssets\\AssetSceneIgnoreSetting.XML";
    public static AssetSceneIgnoreList ScenePropertyIgnoreList;//所有系统设置
    public void LoadIgnoreList()
    {
        string path = Application.dataPath + ConfigPath;
        if (!File.Exists(path))
        {
            Debug.LogError("ReferenceSceneWindow.CreateXML:"+path);
            CreateSystemSettingXml();
        }
        else
        {
            Debug.LogError("ReferenceSceneWindow.LoadXML:" + path);
            ScenePropertyIgnoreList = SerializeHelper.DeserializeFromFile<AssetSceneIgnoreList>(path);
        }
    }

    public void SaveNewItem(ReferenceSceneData.AssetSceneDescription des)
    {
        if(ScenePropertyIgnoreList!=null)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene == null ? "" : currentScene.name;
            ScenePropertyIgnoreList.SaveNewItem(sceneName,des.fullPath,des.type,des.property);
            SaveSystemSetting();
        }
    }

    public static void CreateSystemSettingXml()
    {
        ScenePropertyIgnoreList = new AssetSceneIgnoreList();
        ScenePropertyIgnoreList.AssetScenes = new List<AssetScene>();
        SaveSystemSetting();
    }
    /// <summary>
    /// 保存系统设置
    /// </summary>
    public static void SaveSystemSetting()
    {
        string path = Application.dataPath + ConfigPath;
        try
        {
            SerializeHelper.Save(ScenePropertyIgnoreList, path);
            Debug.Log("SaveXml success...");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
    /// <summary>
    /// 按状态排序（丢失引用在前，无引用在后）
    /// </summary>
    /// <param name="viewItemList"></param>
    /// <param name="root"></param>
    private void SortList(AssetViewItem root)
    {
        root.children.Sort((a,b)=> 
        {
            AssetViewItem ATemp = (AssetViewItem)a;
            AssetViewItem BTemp = (AssetViewItem)b;
            if (ATemp == null || BTemp == null) return 0;
            return ATemp.data.state.CompareTo(BTemp.data.state);
        });      
    }
    private void AddChild(string sceneName,string typeName,string propertyName,GameObject item,ref int elementCount,int depth,AssetViewItem root, ReferenceSceneData.AssetSceneState state)
    {               
        if(ScenePropertyIgnoreList!=null)
        {
            string propertyPath= string.Format("{0}/{1}/{2}", FullPath(item), typeName, propertyName);//如果存在当前场景的忽略列表，则不处理
            if (ScenePropertyIgnoreList.IsSameScenePropery(sceneName, propertyPath)) return;
        }
        ++elementCount;
        ReferenceSceneData.AssetSceneDescription des = data.SetAssetSceneDescription(typeName, propertyName, FullPath(item), item, state);
        string displayName = string.Format("{0}->{1}", typeName, propertyName);
        root.AddChild(new AssetViewItem { id = elementCount, displayName = displayName, data = des, depth = depth });
    }
}
#endif
