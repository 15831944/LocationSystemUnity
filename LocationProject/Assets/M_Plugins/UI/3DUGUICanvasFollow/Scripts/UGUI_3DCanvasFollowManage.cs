using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGUI_3DCanvasFollowManage : MonoBehaviour {


    //public static UGUI_3DCanvasFollowManage Instance;

    ///// <summary>
    ///// 所有3D跟随标志的列表，每个parent为一组
    ///// </summary>
    //public Dictionary<string, GameObject> name_uiparent;
    ///// <summary>
    ///// 所有3D跟随标志的列表
    ///// </summary>
    //public Dictionary<string, List<GameObject>> name_uilist;

    ///// <summary>
    ///// 容器
    ///// </summary>
    //public Transform content;

    //// Use this for initialization
    //void Start()
    //{
    //    Instance = this;
    //    name_uiparent = new Dictionary<string, GameObject>();
    //    name_uilist = new Dictionary<string, List<GameObject>>();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    ///// <summary>
    ///// 创建一组跟随标志
    ///// </summary>
    //public void Create3DFollowGroup()
    //{

    //}

    ///// <summary>
    ///// 创建一组
    ///// </summary>
    //public void CreateFlags(GameObject prefabUI, List<GameObject> objs, GameObject camT, Vector3 offset, string parentName)
    //{
    //    foreach (GameObject o in objs)
    //    {
    //        UGUI_3DFollowManage.Instance.CreateItem(prefabUI, o, camT, offset, parentName);
    //    }
    //}

    ///// <summary>
    ///// 创建一项
    ///// </summary>
    //public GameObject CreateItem(GameObject prefabUI, GameObject target, GameObject camT, Vector3 offset, string parentName)
    //{
    //    UGUI_3DFollow ugui_3DFollowT = GetUIbyTarget(parentName, target);
    //    if (ugui_3DFollowT)
    //    {
    //        ugui_3DFollowT.gameObject.SetActive(true);
    //        return ugui_3DFollowT.gameObject;
    //    }
    //    Collider collider = target.GetComponentInChildren<Collider>();
    //    if (collider != null)
    //    {
    //        offset = offset + new Vector3(0, target.GetGlobalSize().y / 2, 0);
    //    }
    //    offset = offset + new Vector3(0, target.GetGlobalSize().y / 2, 0);
    //    GameObject ui = Instantiate(prefabUI);
    //    ui.SetActive(true);
    //    ui.transform.localPosition = target.transform.position;
    //    ui.transform.localScale = Vector3.one;
    //    UGUI_3DFollow ugui_3DFollow = ui.AddMissingComponent<UGUI_3DFollow>();
    //    ugui_3DFollow.Init(target, camT, offset);

    //    if (name_uiparent.ContainsKey(parentName))
    //    {
    //        ui.transform.SetParent(name_uiparent[parentName].transform);
    //    }
    //    else
    //    {
    //        CreateParent(parentName);
    //        ui.transform.SetParent(name_uiparent[parentName].transform);
    //    }

    //    if (name_uilist.ContainsKey(parentName))
    //    {
    //        name_uilist[parentName].Add(ui);
    //    }
    //    else
    //    {
    //        CreateParent(parentName);
    //        name_uilist[parentName].Add(ui);
    //    }
    //    return ui;
    //}

    ///// <summary>
    ///// 创建父物体
    ///// </summary>
    //public void CreateParent(string name)
    //{
    //    GameObject parentT = new GameObject(name);
    //    parentT.transform.SetParent(content);
    //    parentT.transform.localPosition = Vector3.zero;
    //    parentT.transform.localScale = Vector3.one;
    //    name_uilist.Add(name, new List<GameObject>());
    //    name_uiparent.Add(name, parentT);
    //    //return dics[name];
    //}

    ///// <summary>
    ///// 设置跟随UI的显示或隐藏通过组名称
    ///// </summary>
    //public void SetGroupUIbyName(string name, bool b)
    //{
    //    if (name_uiparent.ContainsKey(name))
    //    {
    //        name_uiparent[name].SetActive(b);
    //    }
    //}

    ///// <summary>
    ///// 移除跟随UI的集合，通过名称
    ///// </summary>
    //public void RemoveGroupUIbyName(string name)
    //{
    //    if (name_uiparent.ContainsKey(name))
    //    {
    //        DestroyImmediate(name_uiparent[name].gameObject);
    //        name_uiparent.Remove(name);
    //    }
    //    if (name_uilist.ContainsKey(name))
    //    {
    //        name_uilist.Remove(name);
    //    }
    //}

    ///// <summary>
    ///// 获取UI标志通过目标物体
    ///// </summary>
    //public UGUI_3DFollow GetUIbyTarget(string name, GameObject target)
    //{
    //    if (name_uilist.ContainsKey(name))
    //    {
    //        List<GameObject> uis = name_uilist[name];
    //        foreach (GameObject ui in uis)
    //        {
    //            UGUI_3DFollow follow = ui.GetComponent<UGUI_3DFollow>();
    //            if (follow.target == target)
    //            {
    //                return follow;
    //            }
    //        }
    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 设置UI的名称通过组名称和目标物体
    ///// </summary>
    //public void SetUIbyTarget(string name, GameObject target, bool isActive)
    //{
    //    UGUI_3DFollow ui = GetUIbyTarget(name, target);
    //    ui.gameObject.SetActive(isActive);
    //}


    ///// <summary>
    ///// 是否有某组跟随UI，根据组名称项
    ///// </summary>
    //public bool IsGroupUIbyName(string name)
    //{
    //    if (name_uiparent.ContainsKey(name))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    /////// <summary>
    /////// 是否有跟随UI，根据组名称项和目标物体
    /////// </summary>
    ////public bool IsFollowUIbyName(string name, GameObject target)
    ////{
    ////    if (name_uilist.ContainsKey(name))
    ////    {
    ////        List<GameObject> uis = name_uilist[name];
    ////        foreach (GameObject ui in uis)
    ////        {
    ////            UGUI_3DFollow follow = ui.GetComponent<UGUI_3DFollow>();
    ////            if (follow.target == target)
    ////            {
    ////                return true;
    ////            }
    ////        }
    ////    }
    ////    return false;
    ////}
}
