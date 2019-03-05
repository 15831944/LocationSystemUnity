using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 历史轨迹人员添加界面
/// </summary>
public class HistoryPersonsSearchUI : MonoBehaviour
{
    public static HistoryPersonsSearchUI Instance;
    public GameObject window;//窗口   
    public Button closeBtn;//关闭   

    public Button saveBtn;//保存按钮

    public List<Personnel> personnels;//人员信息列表
    public List<Personnel> currentSelectPersonnels;//当前选中的人员信息列表
    public List<Personnel> currentSelectPersonnelsBeforeShow;//在打开之前选中的人员信息列表

    private bool isEdited;//是否修改过

    // Use this for initialization
    void Start()
    {
        Instance = this;
        closeBtn.onClick.AddListener(CloseBtn_OnClick);
        searchInput.onEndEdit.AddListener(SearchInput_OnEndEdit);
        searchInput.onValueChanged.AddListener(SearchInput_OnValueChanged);
        searchBtn.onClick.AddListener(SearchBtn_OnClick);
        saveBtn.onClick.AddListener(SaveBtn_OnClick);

        previousPageBtn.onClick.AddListener(PreviousPageBtn_OnClick);
        nextPageBtn.onClick.AddListener(NextPageBtn_OnClick);
        InputFieldPage.onEndEdit.AddListener(InputFieldPage_OnEndEdit);

        selectItems = new List<HistoryPersonsSearchUISelectedItem>();
        //currentSelectPersonnels = new List<Personnel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("KeyCode.Return!");
        }
    }

    /// <summary>
    /// 展示
    /// </summary>
    public void Show(List<Personnel> personnelT)
    {
        currentSelectPersonnels = personnelT;
        currentSelectPersonnelsBeforeShow = new List<Personnel>(personnelT);
        personnels = PersonnelTreeManage.Instance.departmentDivideTree.personnels;
        SetWindow(true);
        Search();
        ShowSelectItems();
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        SetWindow(false);
        isEdited = false;
    }

    /// <summary>
    /// 设置窗口的显示隐藏
    /// </summary>
    /// <param name="isActiveT"></param>
    public void SetWindow(bool isActiveT)
    {
        window.SetActive(isActiveT);

    }

    /// <summary>
    /// 关闭按钮触发事件
    /// </summary>
    public void CloseBtn_OnClick()
    {
        CheckIsEdited();
        if (isEdited)
        {
            UGUIMessageBox.Show("是否保存？", "是", "否", () =>
               {
                   SaveBtn_OnClick();
                   Hide();
               }, () =>
             {
                 Hide();
             }, null);
        }
        else
        {
            Hide();
        }
    }


    /// <summary>
    /// 保存按钮触发事件
    /// </summary>
    public void SaveBtn_OnClick()
    {
        Debug.Log("SaveBtn_OnClick!");
        List<Personnel> currentSelectPersonnelsT = new List<Personnel>(currentSelectPersonnels);
        MultHistoryPlayUI.Instance.ShowPersons(currentSelectPersonnelsT);
        currentSelectPersonnelsBeforeShow = new List<Personnel>(currentSelectPersonnels);
        isEdited = false;
    }

    /// <summary>
    /// 检查是否修改过
    /// </summary>
    public void CheckIsEdited()
    {
        if (currentSelectPersonnels.Count != currentSelectPersonnelsBeforeShow.Count)
        {
            isEdited = true;
            return;
        }

        foreach (Personnel p in currentSelectPersonnels)
        {
            if (!currentSelectPersonnelsBeforeShow.Contains(p))
            {
                isEdited = true;
                return;
            }
        }

        isEdited = false;
    }

    #region 选中项的人员列表

    public HorizontalLayoutGroup selectItemsGrid;//列表
    public HistoryPersonsSearchUISelectedItem selectItemPrefab;//选中项预设
    private List<HistoryPersonsSearchUISelectedItem> selectItems;//选中项列表集合

    /// <summary>
    /// 显示选择的项
    /// </summary>
    public void ShowSelectItems()
    {
        ClearSelectItems();
        foreach (Personnel p in currentSelectPersonnels)
        {
            AddSelectItem(p);
        }

    }

    /// <summary>
    /// 添加选中的项
    /// </summary>
    public void AddSelectItem(Personnel personnelT)
    {
        //isEdited = true;
        HistoryPersonsSearchUISelectedItem item = CreateSelectItem(personnelT);
        selectItems.Add(item);
        SetPersonsLimitRelevant();
    }

    /// <summary>
    /// 移除选中的项
    /// </summary>
    public void RemoveSelectItem(Personnel personnelT)
    {
        //isEdited = true;
        HistoryPersonsSearchUISelectedItem item = selectItems.Find((i) => i.personnel.Id == personnelT.Id);
        selectItems.Remove(item);
        DestroyImmediate(item.gameObject);
        SetPersonsLimitRelevant();
    }

    /// <summary>
    /// 创建选中项
    /// </summary>
    public HistoryPersonsSearchUISelectedItem CreateSelectItem(Personnel personnelT)
    {
        HistoryPersonsSearchUISelectedItem item = Instantiate(selectItemPrefab);
        item.transform.SetParent(selectItemsGrid.transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        item.Init(personnelT);

        item.gameObject.SetActive(true);
        return item;
    }

    /// <summary>
    /// 清除已选中的列表
    /// </summary>
    public void ClearSelectItems()
    {
        int n = selectItemsGrid.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
        {
            DestroyImmediate(selectItemsGrid.transform.GetChild(i).gameObject);
        }
        selectItems.Clear();
    }

    #endregion

    #region 人员列表
    public InputField searchInput;//搜索关键字输入框   
    public Button searchBtn;//搜索按钮
    public GridLayoutGroup personsGrid;//人员列表
    public HistoryPersonsSearchUIItem searchItem;//预设
    public InputField InputFieldPage;//输入框，表示当前第几页，或表示要跳到第几页
    public Button previousPageBtn;//上一页
    public Button nextPageBtn;//下一页
    public Text txtPageCount;//总页数Text
    //private List<HistoryPersonsSearchUIItem> selectPersonnelList;//当前显示的列表集合
    private int currentPageNum;//当前所在页,0开始
    private int pageCount;//总页数
    private int showCount = 9;//每页显示人员的个数
    public List<Personnel> searchPersonnels;//当前搜索出来的人员

    /// <summary>
    /// 搜索
    /// </summary>
    public void Search()
    {
        currentPageNum = 0;
        personnels = PersonnelTreeManage.Instance.departmentDivideTree.personnels;
        if (searchInput.text == "")
        {
            searchPersonnels = personnels;
        }
        else
        {
            searchPersonnels = personnels.FindAll((item) => Contains(item));
        }

        if (personnels == null) return;
        pageCount = searchPersonnels.Count / showCount;
        if (searchPersonnels.Count % showCount > 0)
        {
            pageCount += 1;
        }

        ShowPersonsGrid();
    }

    /// <summary>
    /// 展示人员列表
    /// </summary>
    public void ShowPersonsGrid()
    {

        //currentPageNum = 0;
        txtPageCount.text = pageCount.ToString();
        InputFieldPage.text = (currentPageNum + 1).ToString();
        CreatePersonsGrid();
        SetPreviousAndNextPageBtn();
        SetPersonsLimitRelevant();
    }

    /// <summary>
    /// 创建人员列表
    /// </summary>
    public void CreatePersonsGrid()
    {
        ClearPersonItems();
        int startIndex = currentPageNum * showCount;
        int num = showCount;
        if (startIndex + num > searchPersonnels.Count)
        {
            num = searchPersonnels.Count - startIndex;
        }
        if (searchPersonnels.Count == 0) return;
        List<Personnel> personnelsT = searchPersonnels.GetRange(startIndex, num);

        foreach (Personnel p in personnelsT)
        {
            //HistoryPersonsSearchUIItem item = selectPersonnelList.Find((i) => i.personnel == p);
            //if (item != null)
            //{
            //    item.gameObject.SetActive(true);
            //    item.transform.SetAsLastSibling();
            //    continue;
            //}
            HistoryPersonsSearchUIItem item = CreatePersonItem(p);
            Personnel pt = currentSelectPersonnels.Find((it) => it.Id == p.Id);
            if (pt != null)
            {
                item.SetToggle(true);
            }

            item.gameObject.SetActive(true);
            //personItemList.Add(item);

        }
    }

    /// <summary>
    /// 创建人员列表项
    /// </summary>
    public HistoryPersonsSearchUIItem CreatePersonItem(Personnel personnelT)
    {
        HistoryPersonsSearchUIItem item = Instantiate(searchItem);
        item.InitData(personnelT);
        item.transform.SetParent(personsGrid.transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
        //item.gameObject.SetActive(true);
        return item;
    }

    /// <summary>
    /// 清除人员列表项
    /// </summary>
    public void ClearPersonItems()
    {
        int n = personsGrid.transform.childCount;
        for (int i = n - 1; i >= 0; i--)
        {
            GameObject o = personsGrid.transform.GetChild(i).gameObject;
            //HistoryPersonsSearchUIItem itemT = o.GetComponent<HistoryPersonsSearchUIItem>();
            //if (selectPersonnelList.Contains(itemT))
            //{
            //    o.SetActive(false);
            //    continue;
            //}
            DestroyImmediate(o);
        }
    }

    /// <summary>
    /// 搜索框编辑结束触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void SearchInput_OnEndEdit(string txt)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("SearchInput_OnEndEdit!");
            currentPageNum = 0;
            Search();
        }

    }

    /// <summary>
    /// 搜索框改变触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void SearchInput_OnValueChanged(string txt)
    {
        Debug.Log("SearchInput_OnValueChanged!");
    }

    /// <summary>
    /// 搜索按钮触发事件
    /// </summary>
    public void SearchBtn_OnClick()
    {
        Debug.Log("SearchBtn_OnClick!");
        Search();
    }



    /// <summary>
    /// 筛选根据名称
    /// </summary>
    public bool Contains(Personnel p)
    {
        if (ContainsName(p)) return true;
        if (ContainsWorkNum(p)) return true;
        if (ContainsDepartment(p)) return true;
        if (ContainsPost(p)) return true;
        return false;
    }

    /// <summary>
    /// 筛选根据名称
    /// </summary>
    public bool ContainsName(Personnel p)
    {
        if (p.Name.ToLower().Contains(searchInput.text.ToLower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 筛选根据工号
    /// </summary>
    public bool ContainsWorkNum(Personnel p)
    {
        if (p.WorkNumber.ToString().ToLower().Contains(searchInput.text.ToLower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 筛选根据部门
    /// </summary>
    public bool ContainsDepartment(Personnel p)
    {
        if (p.Parent.Name.ToLower().Contains(searchInput.text.ToLower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 筛选根据岗位
    /// </summary>
    public bool ContainsPost(Personnel p)
    {
        if (p.Pst.ToLower().Contains(searchInput.text.ToLower()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 添加选中人员的项
    /// </summary>
    public void AddSelectPersonnelItem(Personnel personnelT)
    {
        //selectPersonnelList.Add(itemT);
        currentSelectPersonnels.Add(personnelT);
        AddSelectItem(personnelT);
    }

    /// <summary>
    /// 移除选中人员的项
    /// </summary>
    public void RemoveSelectPersonnelItem(Personnel personnelT)
    {
        //HistoryPersonsSearchUIItem item = selectPersonnelList.Find((i) => i.personnel == personnelT);
        //selectPersonnelList.Remove(item);
        Personnel p = currentSelectPersonnels.Find((i) => i.Id == personnelT.Id);
        if (p != null)
        {
            currentSelectPersonnels.Remove(p);
        }
        RemoveSelectItem(personnelT);
    }

    /// <summary>
    /// 移除选中人员的项
    /// </summary>
    public void SetSelectPersonnelItemToggle(Personnel personnelT, bool b)
    {
        List<HistoryPersonsSearchUIItem> pList = new List<HistoryPersonsSearchUIItem>(personsGrid.GetComponentsInChildren<HistoryPersonsSearchUIItem>());
        HistoryPersonsSearchUIItem item = pList.Find((i) => i.personnel == personnelT);
        //HistoryPersonsSearchUIItem item = selectPersonnelList.Find((i) => i.personnel == personnelT);
        if (!b)
        {
            if (item != null)
            {
                item.SetToggle(false);
            }
            else
            {
                RemoveSelectPersonnelItem(personnelT);
            }
        }

    }

    /// <summary>
    /// 前一页的按钮触发事件
    /// </summary>
    public void PreviousPageBtn_OnClick()
    {
        Debug.Log("PreviousPageBtn_OnClick!");
        currentPageNum = currentPageNum - 1;
        //int currentPageNumT = currentPageNum;
        //SetPreviousAndNextPageBtn();
        ShowPersonsGrid();
    }

    /// <summary>
    /// 下一页的按钮触发事件
    /// </summary>
    public void NextPageBtn_OnClick()
    {
        Debug.Log("NextPageBtn_OnClick!");
        currentPageNum = currentPageNum + 1;
        //SetPreviousAndNextPageBtn();
        ShowPersonsGrid();
    }

    /// <summary>
    /// 设置前一页和后一页是否开启交互，即interactable
    /// </summary>
    public void SetPreviousAndNextPageBtn()
    {
        if (currentPageNum <= 0)
        {
            currentPageNum = 0;
            previousPageBtn.interactable = false;
        }
        else
        {
            previousPageBtn.interactable = true;
        }

        if (currentPageNum >= pageCount - 1)
        {
            currentPageNum = pageCount - 1;
            nextPageBtn.interactable = false;
        }
        else
        {
            nextPageBtn.interactable = true;
        }
    }

    /// <summary>
    /// 输入页数文本框编辑结束触发事件
    /// </summary>
    /// <param name="txt"></param>
    public void InputFieldPage_OnEndEdit(string txt)
    {
        Debug.Log("InputFieldPage_OnEndEdit!");
        int numP = 0;
        try
        {
            numP = int.Parse(txt);
        }
        catch
        {
            return;
        }

        numP = numP < 1 ? 0 : numP;
        numP = numP > pageCount ? 0 : numP;
        if (numP == 0)
        {
            InputFieldPage.text = (currentPageNum + 1).ToString();
            return;
        }

        currentPageNum = numP - 1;
        //SetPreviousAndNextPageBtn();
        ShowPersonsGrid();
    }

    /// <summary>
    /// 根据人数限制设置相关操作
    /// </summary>
    public void SetPersonsLimitRelevant()
    {
        if (currentSelectPersonnels.Count < MultHistoryPlayUI.Instance.limitPersonNum)
        {
            //SetSearchUIItemInteractable(true);
            SetSearchUIItemLimitAlam(false);
        }
        else
        {
            //SetSearchUIItemInteractable(false);
            SetSearchUIItemLimitAlam(true);
        }
    }


    /// <summary>
    /// 设置SearchUIItem超过人数限制警告
    /// </summary>
    public void SetSearchUIItemLimitAlam(bool b)
    {
        HistoryPersonsSearchUIItem[] items = personsGrid.GetComponentsInChildren<HistoryPersonsSearchUIItem>();
        foreach (HistoryPersonsSearchUIItem item in items)
        {
            if (b)
            {
                if (!item.toggle.isOn)
                {
                    item.AddListener_Alarm();
                }
            }
            else
            {
                item.AddListener();
            }
        }
    }

    /// <summary>
    /// 是否达到人数限制
    /// </summary>
    public bool IsAchisevePersonsLimitNum()
    {
        if (currentSelectPersonnels.Count < MultHistoryPlayUI.Instance.limitPersonNum)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion
}
