using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Location.WCFServiceReferences.LocationServices;

public class ObjectListSearch : MonoBehaviour
{
    public static ObjectListSearch Instance;
    /// <summary>
    /// 设备列表查询框
    /// </summary>
    public InputField inputField;
    /// <summary>
    /// 查询按钮
    /// </summary>
    public Button searchBtn;

    /// <summary>
    /// 是否是设备列表查询,当inputField为空时，认为不是查询
    /// </summary>
    public bool isSearchShow;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        inputField.onValueChanged.AddListener(inputField_OnValueChanged);
        inputField.onEndEdit.AddListener(inputField_OnEndEdit);
        searchBtn.onClick.AddListener(searchBtn_OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 输入栏是否为空
    /// </summary>
    private bool IsSwitchToEmpty;
    public void inputField_OnValueChanged(string txt)
    {
        if(string.IsNullOrEmpty(txt))
        {
            if(!IsSwitchToEmpty)
            {
                inputField_OnEndEdit(txt);
            }
        }
    }

    /// <summary>
    /// 编辑结束
    /// </summary>
    public void inputField_OnEndEdit(string txt)
    {
        //if (string.IsNullOrEmpty(txt)) return;
        if (!string.IsNullOrEmpty(ObjectListSearch.Instance.inputField.text))
        {
            IsSwitchToEmpty = false;
            //isSearchShow = true;
            SetIsSearchShow(true);
        }
        else
        {
            IsSwitchToEmpty = true;
            //isSearchShow = false;
            SetIsSearchShow(false);
        }
        List<ObjectAddList_ChildType> childTypeList = Searching(txt);
        ObjectListController.Instance.AfterSearchShow(childTypeList);
    }
    /// <summary>
    /// 重置搜索栏
    /// </summary>
    public void RestSearchFiled()
    {
        IsSwitchToEmpty = true;
        inputField.text = "";
        inputField_OnEndEdit("");
    }
    public void searchBtn_OnClick()
    {
        //if (!string.IsNullOrEmpty(inputField.text))
        //{
        inputField_OnEndEdit(inputField.text);
        //}
    }

    /// <summary>
    /// 设置是否是设备列表查询,当inputField为空时，认为不是查询
    /// </summary>
    public void SetIsSearchShow(bool b)
    {
        isSearchShow = b;
    }

    public List<ObjectAddList_ChildType> Searching(string txt)
    {
        //List<TPView_SearchItem> customerInfoT = ObjectAddListManage.Instance.info.Where((searchItem) => Contains(searchItem.name)).ToList();
        List<ObjectAddList_ChildType> typeModels = new List<ObjectAddList_ChildType>();

        foreach (ObjectAddList_ChildType childType in ObjectListController.Instance.currentType.childTypeList)
        {
            List<ObjectAddList_Model> models = childType.modelList.Where((searchItem) => Contains(searchItem.modelName)).ToList();
            if (models.Count > 0)
            {
                ObjectAddList_ChildType childTypeT = new ObjectAddList_ChildType();
                childTypeT.childTypeName = childType.childTypeName;
                childTypeT.modelList = models.ToArray();
                typeModels.Add(childTypeT);
            }
        }

        return typeModels;
    }

    //public List<ObjectAddList_ChildType> Searching(string txt)
    //{
    //    //List<TPView_SearchItem> customerInfoT = ObjectAddListManage.Instance.info.Where((searchItem) => Contains(searchItem.name)).ToList();
    //    List<ObjectAddList_ChildType> typeModels = new List<ObjectAddList_ChildType>();
    //    foreach (ObjectAddList_Type type in ObjectAddListManage.Instance.info)
    //    {
    //        foreach (ObjectAddList_ChildType childType in type.childTypeList)
    //        {
    //            List<ObjectAddList_Model> models = childType.modelList.Where((searchItem) => Contains(searchItem.modelName)).ToList();
    //            if (models.Count > 0)
    //            {
    //                ObjectAddList_ChildType childTypeT = new ObjectAddList_ChildType();
    //                childTypeT.childTypeName = childType.childTypeName;
    //                childTypeT.modelList = models;
    //                typeModels.Add(childTypeT);
    //            }
    //        }
    //    }

    //    return typeModels;
    //}

    /// <summary>
    /// Returns a value indicating whether Input occurs within specified value.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
    public virtual bool Contains(string value)
    {
        return value.ToLower().Contains(inputField.text.ToLower());
    }
}
