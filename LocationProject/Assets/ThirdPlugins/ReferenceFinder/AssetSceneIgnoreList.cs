using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlType("AssetSceneIgnoreList")]
public class AssetSceneIgnoreList {

    [XmlArray("AssetScenes")]
    public List<AssetScene> AssetScenes;

    public void SaveNewItem(string sceneName,string path,string scriptName,string propertyName)
    {
        if (AssetScenes == null) AssetScenes = new List<AssetScene>();
        AssetScene sceneTarget = AssetScenes.Find(i => i.SceneName == sceneName);
        if (sceneTarget == null)
        {
            sceneTarget = new AssetScene();
            sceneTarget.SceneName = sceneName;            
            AssetScenes.Add(sceneTarget);
        }
        sceneTarget.AddNewItem(path, scriptName, propertyName);
    }

    /// <summary>
    /// 是否同一场景的同一属性
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsSameScenePropery(string sceneName,string path)
    {
        if (AssetScenes == null || AssetScenes.Count == 0) return false;
        else
        {
            AssetScene sceneTarget = AssetScenes.Find(i=>i.SceneName==sceneName);
            if (sceneTarget == null) return false;
            else
            {
                return sceneTarget.IsSamePropertyPath(path);
            }
        }
    }
}

[XmlType("AssetScene")]
public class AssetScene
{
    /// <summary>
    /// 属性名称
    /// </summary>
    [XmlAttribute]
    public string SceneName;

    [XmlArray]
    public List<AssetSceneIgnoreItem> AssetIgnoreItems;

    /// <summary>
    /// 添加新的忽略项
    /// </summary>
    /// <param name="path"></param>
    /// <param name="scriptName"></param>
    /// <param name="propertyName"></param>
    public void AddNewItem(string path,string scriptName,string propertyName)
    {
        if (AssetIgnoreItems == null) AssetIgnoreItems = new List<AssetSceneIgnoreItem>();
        AssetSceneIgnoreItem item = new AssetSceneIgnoreItem();
        item.Path = path;
        item.PropertyName = propertyName;
        item.ScriptName = scriptName;
        AssetIgnoreItems.Add(item);
    }

    /// <summary>
    /// 是否和忽略列表的属性，路径相同
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsSamePropertyPath(string path)
    {
        if (AssetIgnoreItems == null || AssetIgnoreItems.Count == 0) return false;
        else
        {
            AssetSceneIgnoreItem item = AssetIgnoreItems.Find(i => i != null && i.IsSameProperty(path));
            return item != null;
        }
    }
}

[XmlType("AssetSceneIgnoreItem")]
public class AssetSceneIgnoreItem
{
    /// <summary>
    /// 属性名称
    /// </summary>
    [XmlAttribute]
    public string PropertyName;

    /// <summary>
    /// 脚本名称
    /// </summary>
    [XmlAttribute]
    public string ScriptName;

    /// <summary>
    /// 脚本物体的路径
    /// </summary>
    [XmlAttribute]
    public string Path;

    /// <summary>
    /// 是否同一个路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsSameProperty(string path)
    {
        string propertyPath = string.Format("{0}/{1}/{2}", Path, ScriptName, PropertyName);
        return path == propertyPath;
    }

}
