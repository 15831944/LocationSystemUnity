using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;
using DataObjects.Utils;
using Assets.M_Plugins.Helpers.Utils;
/// <summary>
/// 模型添加的子类型项
/// </summary>
public class ChildTypeItem : MonoBehaviour
{
    /// <summary>
    /// 顶部按钮项
    /// </summary>
    public Toggle topToggle;
    /// <summary>
    /// 顶部字体正常颜色
    /// </summary>
    public Color toptextNormalColor;
    /// <summary>
    /// 顶部字体高亮颜色
    /// </summary>
    public Color toptextHighlightColor;
    /// <summary>
    /// 内容列表
    /// </summary>
    public Transform content;
    /// <summary>
    /// 模型单元项，预设
    /// </summary>
    public GameObject unitItem;

    /// <summary>
    /// 顶部
    /// </summary>
    private Text topText;


    /// <summary>
    /// 子类型相关数据
    /// </summary>
    public ObjectAddList_ChildType childType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        topText = topToggle.GetComponentInChildren<Text>(true);
        //topToggle.onValueChanged.RemoveAllListeners();
        topToggle.onValueChanged.RemoveListener(topButton_OnValueChanged);
        topToggle.onValueChanged.AddListener(topButton_OnValueChanged);
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    public void InitData(ObjectAddList_ChildType childTypeT)
    {
        Init();
        childType = childTypeT;
        SetTopName(childType.childTypeName);
    }
    /// <summary>
    /// 是否是通用添加的设备类型，即除了需要在特殊界面添加的几个设备类型
    /// </summary>
    /// <param name="typeCode"></param>
    /// <returns></returns>
    public bool IsCanAdd(string typeCode)
    {
        return typeCode != TypeCodes.HK_KVM
               && typeCode != TypeCodes.PE_DEV
               && typeCode != TypeCodes.DoorControl
               && typeCode != TypeCodes.MicroModule1to4
               && typeCode != TypeCodes.MicroModule2to4
               && typeCode != TypeCodes.DoorGuard
               && typeCode != TypeCodes.SqlMonitor
               && typeCode != TypeCodes.LinuxMonitor
               && typeCode != TypeCodes.WinMonitor;
        ;
    }
    /// <summary>
    /// 刷新单元项列表
    /// </summary>
    public IEnumerator RefleshItems()
    {
        SetContentActive(true);
        List<ObjectAddList_Model> objectAddList_Models = new List<ObjectAddList_Model>();
        foreach (ObjectAddList_Model model in childType.modelList)
        {
            if (IsCanAdd(model.typeCode))
            {
                objectAddList_Models.Add(model);
            }
        }
        int childcount = content.transform.childCount;
        //continue;
        for (int i = 0; i < objectAddList_Models.Count; i++)
        {
            string typecode = objectAddList_Models[i].typeCode;

            Transform tran = null;
            if (i < childcount)
            {
                tran = content.transform.GetChild(i);
            }
            else
            {
                tran = InstantiateItem().transform;
            }
            if (tran)
            {
                Image image = tran.GetChild(0).GetChild(0).GetComponent<Image>();
                string modelname = objectAddList_Models[i].modelName;
                SetImage(image, modelname);
                string typenameT = ObjectListToolbar.Instance.currentType.typeName;
                if (typenameT == "机柜")
                {
                    //ObjectListToolbarItem_Cabinet objectListModelItem = tran.gameObject.AddMissingComponent<ObjectListToolbarItem_Cabinet>();
                    ////ModelConfig modelConfig = ModelSetting.GetModelByModelName(modelname);
                    ////objectListModelItem.SetModelInfo(modelConfig);
                    //objectListModelItem.SetModelName(modelname);
                    //objectListModelItem.SetFocusTipText(modelname);

                    ObjectListModelItem objectListModelItem = tran.gameObject.AddMissingComponent<ObjectListModelItem>();
                    objectListModelItem.SetFocusTipText(modelname);
                    objectListModelItem.SetModelName(modelname, typecode);
                }
                else if (typenameT == "门窗" || typenameT == "装饰")
                {
                    //ObjectListToolbarItem_Decoration objectListModelItem = tran.gameObject.AddMissingComponent<ObjectListToolbarItem_Decoration>();

                    //ModelConfig modelConfig = ModelSetting.GetModelByModelName(modelname);
                    //objectListModelItem.SetModelInfo(modelConfig);
                    //string name = GetRealName(modelname);
                    //objectListModelItem.SetFocusTipText(name);
                    if (TypeCodeHelper.IsDoorAccess(modelname))
                    {
                        ObjectListToolbarItem_DoorAccess doorAccessItem = tran.gameObject.AddMissingComponent<ObjectListToolbarItem_DoorAccess>();
                        doorAccessItem.SetModelName(modelname,typecode);
                        string name = GetRealName(modelname);
                        doorAccessItem.SetFocusTipText(name);

                    }
                    else
                    {
                        ObjectListModelItem objectListModelItem = tran.gameObject.AddMissingComponent<ObjectListModelItem>();
                        objectListModelItem.SetModelName(modelname, typecode);
                        string name = GetRealName(modelname);
                        objectListModelItem.SetFocusTipText(name);
                    }
                }
                else//设备
                {

                    ObjectListModelItem objectListModelItem = tran.gameObject.AddMissingComponent<ObjectListModelItem>();
                    ////string name = GetRealName(modelname);
                    ////string name = modelname;
                    objectListModelItem.SetFocusTipText(modelname);

                    ////IDevType idevtype = DevTypeManager.Instance.GetDevType(typecode);
                    //IDevType idevtype = InitDataManager.GetDevTypeInfo(typecode);

                    //ModelConfig modelConfig = ModelSetting.GetModelByCode(typecode);
                    objectListModelItem.SetModelName(modelname,typecode);
                }
            }

            yield return null;
        }
        for (int j = childcount - 1; j >= childType.modelList.Length; j--)
        {
            if (j>=content.transform.childCount)
            {
                continue;
            }
            Transform tran = content.transform.GetChild(j);
            if (tran != null)
            {
                DestroyImmediate(tran.gameObject);
                yield return null;
            }
        }
        SetContentPos();
    }
    /// <summary>
    /// 设置Content位置
    /// </summary>
    private void SetContentPos()
    {
        ObjectListController listController = ObjectListController.Instance;
        if(listController)
        {
            Transform contentTemp = listController.content;
            RectTransform rect = contentTemp.GetComponent<RectTransform>();
            RectTransform rectParent = contentTemp.parent.GetComponent<RectTransform>();
            if(rect.rect.size.y<=rectParent.rect.size.y)
            {
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,0);
            }
        }
    }
    /// <summary>
    /// 获取设备的真实名称
    /// </summary>
    public string GetRealName(string modelname)
    {
        string[] strs = modelname.Split('_');
        if (strs.Length >= 2)
        {
            return strs[1];
        }
        else
        {
            return modelname;
        }
    }

    /// <summary>
    /// 设置图片
    /// </summary>
    /// <param name="image"></param>
    /// <param name="modelname"></param>
    /// <param name="isLastImage">是否这个类型最后一张图片</param>
    public void SetImage(Image image, string modelname)
    {
        AssetbundleGet.Instance.GetObj(modelname, ".png", (arg) =>
        {
            if (arg == null && !modelname.Contains("监控区域"))
            {
                //缺少图片，一般都由缺少模型导致。目前先隐藏缺少图片的物体
                //Transform item = image.transform.parent.parent;
                //if (item)
                //{
                //    item.gameObject.SetActive(false);
                //}

                //if (isLastImage&&content!=null)
                //{
                //    bool isAllEmpty = true;
                //    for (int i = 0; i < content.childCount; i++)
                //    {
                //        Transform child = content.GetChild(i);
                //        if (child != null && child.gameObject.activeInHierarchy)
                //        {
                //            isAllEmpty = false;
                //            break;
                //        }
                //    }
                //    if (isAllEmpty)
                //    {
                //        gameObject.SetActive(false);
                //        Debug.LogError("Error:" + childType.childTypeName + " is empty...");
                //    }                    
                //}
                Debug.LogWarning(modelname + "缺少图片");
                return;
            }

            Sprite sprite = (arg as Sprite);
            image.sprite = sprite;
            if(sprite!=null)
            {
                Rect rect = sprite.rect;
                AspectRatioFitter fitter = image.GetComponent<AspectRatioFitter>();
                fitter.aspectRatio = rect.width / rect.height;
            }         
        });
    }

    /// <summary>
    /// 实例化模型单元项预设
    /// </summary>
    public GameObject InstantiateItem()
    {
        GameObject item = Instantiate(unitItem);
        item.transform.SetParent(content);
        item.transform.localScale = Vector3.one;
        item.transform.localPosition = Vector3.zero;
        item.SetActive(true);
        return item;
    }

    /// <summary>
    /// 顶部按钮触发事件
    /// </summary>
    public void topButton_OnValueChanged(bool b)
    {
        SetContentActive(b);
        if (b)
        {
            topText.color = toptextHighlightColor;
            StartCoroutine(RefleshItems());
        }
        else
        {
            topText.color = toptextNormalColor;
        }
    }

    public void SetContentActive(bool b)
    {
        content.gameObject.SetActive(b);
    }

    /// <summary>
    /// 设置标题名称
    /// </summary>
    public void SetTopName(string name)
    {
        topText.text = name;
    }
}
