using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;

public class ObjectListController : MonoBehaviour
{
    public static ObjectListController Instance;
    /// <summary>
    /// 子类型项预设
    /// </summary>
    public GameObject childTypeItem;
    /// <summary>
    /// 内容列表
    /// </summary>
    public Transform content;
    /// <summary>
    /// 当前toggle组
    /// </summary>
    private ToggleGroup currenttoggleGroup;

    /// <summary>
    /// 当前大类信息
    /// </summary>
    public ObjectAddList_Type currentType;
    /// <summary>
    /// 设备大类对应该设备大类的设备列表容器
    /// </summary>
    public Dictionary<string, GameObject> type_List;
    /// <summary>
    /// 当前设备列表的容器
    /// </summary>
    public GameObject currentParent;
    /// <summary>
    /// 滚动条
    /// </summary>
    private Scrollbar VerticalBar;
    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //toggleGroup = GetComponent<ToggleGroup>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    ///// <summary>
    ///// 初始化
    ///// </summary>
    //public void Show(ObjectAddList_Type currentTypeT)
    //{
    //    if (type_List == null)
    //    {
    //        type_List = new Dictionary<string, GameObject>();
    //    }
    //    currentType = currentTypeT;
    //    CreateChildTypeItemParent(currentType.typeName);
    //    RefleshObjectList();
    //}

    /// <summary>
    /// 初始化
    /// </summary>
    public void Show(ObjectAddList_Type currentTypeT)
    {
        if (type_List == null)
        {
            type_List = new Dictionary<string, GameObject>();
        }
        currentType = currentTypeT;
        ResetVerticalBar();
        //ObjectListSearch.Instance.searchBtn_OnClick();
        ObjectListSearch.Instance.RestSearchFiled();
    }
    /// <summary>
    /// 重置滚动条
    /// </summary>
    private void ResetVerticalBar()
    {
        if(VerticalBar==null)
        {
            ScrollRect rect = transform.GetComponentInChildren<ScrollRect>();
            VerticalBar=rect.verticalScrollbar;
            if (VerticalBar) VerticalBar.value = 1;
        }
        else
        {
            VerticalBar.value = 1;
        }        
    }
    /// <summary>
    /// 查询之后的显示设备列表
    /// </summary>
    public void AfterSearchShow(List<ObjectAddList_ChildType> typeModels)
    {
        if (type_List == null)
        {
            type_List = new Dictionary<string, GameObject>();
        }
        //currentType = currentTypeT;
        CreateChildTypeItemParent(currentType.typeName);
        RefleshObjectList(typeModels);
    }

    ///// <summary>
    ///// 刷新物体列表
    ///// </summary>
    //public void RefleshObjectList()
    //{
    //    int childcount = currentParent.transform.childCount;
    //    for (int i = 0; i < currentType.childTypeList.Count; i++)
    //    {
    //        Transform tran = null;
    //        if (i < childcount)
    //        {
    //            tran = currentParent.transform.GetChild(i);
    //        }
    //        else
    //        {
    //            tran = InstantiateItem().transform;
    //        }
    //        if (tran)
    //        {
    //            ChildTypeItem item = tran.GetComponent<ChildTypeItem>();
    //            item.InitData(currentType.childTypeList[i]);
    //            SetToggleGroup(item.topToggle);
    //            if (childcount == 0)
    //            {
    //                item.SetContentActive(false);
    //                if (i == 0)
    //                {
    //                    //item.RefleshItems();
    //                    item.topToggle.isOn = true;
    //                    item.SetContentActive(true);
    //                }
    //            }
    //        }

    //    }

    //    for (int i = childcount - 1; i >= currentType.childTypeList.Count; i--)
    //    {
    //        Transform tran = currentParent.transform.GetChild(i);
    //        DestroyImmediate(tran.gameObject);
    //    }

    //}

    /// <summary>
    /// 刷新物体列表
    /// </summary>
    public void RefleshObjectList(List<ObjectAddList_ChildType> typeModels)
    {
        int childcount = currentParent.transform.childCount;
        for (int i = 0; i < typeModels.Count; i++)
        {
            Transform tran = null;
            if (i < childcount)
            {
                tran = currentParent.transform.GetChild(i);
            }
            else
            {
                tran = InstantiateItem().transform;
            }
            if (tran)
            {
                ChildTypeItem item = tran.GetComponent<ChildTypeItem>();
                item.InitData(typeModels[i]);
                SetToggleGroup(item.topToggle);
                if (!ObjectListSearch.Instance.isSearchShow)
                {
                    //if(childcount==0)
                    if (i == 0)
                    {
                        if(!item.topToggle.isOn)
                        {
                            item.topToggle.isOn = true;
                        }
                        else
                        {
                            StartCoroutine(item.RefleshItems());
                        }
                    }
                }
                else
                {
                    //item.SetContentActive(false);
                    if (i == 0)
                    {
                        item.topToggle.isOn = true;
                        item.topButton_OnValueChanged(true);
                        item.SetContentActive(true);
                    }
                }
            }

        }

        for (int i = childcount - 1; i >= typeModels.Count; i--)
        {
            Transform tran = currentParent.transform.GetChild(i);
            DestroyImmediate(tran.gameObject);
        }
        ObjectListSearch.Instance.SetIsSearchShow(false);
    }

    /// <summary>
    /// 设置ToggleGroup组
    /// </summary>
    public void SetToggleGroup(Toggle tog)
    {
        //currenttoggleGroup.RegisterToggle(tog);
        tog.group = currenttoggleGroup;
    }

    /// <summary>
    /// 实例化预设
    /// </summary>
    /// <returns></returns>
    public GameObject InstantiateItem()
    {
        GameObject o = Instantiate(childTypeItem);
        o.transform.SetParent(currentParent.transform);
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = Vector3.zero;
        o.SetActive(true);
        ChildTypeItem item = o.GetComponent<ChildTypeItem>();
        item.SetContentActive(false);
        return o;
    }

    /// <summary>
    /// 实例化子类型集合父物体
    /// </summary>
    /// <returns></returns>
    public void CreateChildTypeItemParent(string typeName)
    {
        SetParentsAllHide();
        if (!type_List.ContainsKey(typeName))
        {
            GameObject o = new GameObject(typeName);
            RectTransform rect = o.AddMissingComponent<RectTransform>();
            //GameObject o = rect.gameObject;
            o.name = typeName;
            o.transform.SetParent(content);
            o.transform.localPosition = Vector3.zero;
            o.transform.localScale = Vector3.one;
            //RectTransform rect = o.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0, 1);
            o.SetActive(true);
            VerticalLayoutGroup group = o.AddComponent<VerticalLayoutGroup>();
            group.childForceExpandWidth = false;
            group.childForceExpandHeight = false;
            group.spacing =10;
            type_List.Add(typeName, o);
            currentParent = o;
            currenttoggleGroup = o.AddComponent<ToggleGroup>();
            currenttoggleGroup.allowSwitchOff = true;
        }
        else
        {
            currentParent = type_List[typeName];
            currentParent.SetActive(true);
            currenttoggleGroup = currentParent.GetComponent<ToggleGroup>();
        }
    }

    /// <summary>
    /// 设置父物体都隐藏
    /// </summary>
    public void SetParentsAllHide()
    {
        foreach (GameObject o in type_List.Values)
        {
            o.SetActive(false);
        }
    }
}
