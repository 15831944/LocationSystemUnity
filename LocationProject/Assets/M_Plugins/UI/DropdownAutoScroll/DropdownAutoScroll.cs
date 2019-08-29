using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownAutoScroll : MonoBehaviour {

    private Dropdown dropdown;
    //private RectTransform rect;

    /// <summary>
    /// 每项的高度
    /// </summary>
    public float itemHeight = 20f;

    float viewHeight = 100;
    private Scrollbar scrollbar;

    public Transform dropdownList;

    private int childcount;//子物体数量，通过这个来判断是否创建了Dropdown List

    // Use this for initialization
    void Start () {
        dropdown = GetComponent<Dropdown>();
        if (dropdown == null)
        {
            this.enabled = false;
            return;
        }
        //rect = GetComponent<RectTransform>();
        //viewHeight = rect.sizeDelta.y;
        childcount = transform.childCount;
    }
	
	// Update is called once per frame
	void Update () {

        if (childcount < transform.childCount)
        {
            childcount = transform.childCount;
            dropdownList = transform.Find("Dropdown List");
            AutoScrollTo();
        }
        else if (childcount > transform.childCount)
        {
            childcount= transform.childCount; 
            dropdownList = null;
            scrollbar = null;
        }
        else
        {

        }
	}

    /// <summary>
    /// 自动滚动到下拉列表选中的位置
    /// </summary>
    public void AutoScrollTo()
    {
        scrollbar = dropdownList.GetComponentInChildren<Scrollbar>();
        if (scrollbar == null) return;

        RectTransform rect = dropdownList.GetComponent<RectTransform>();
        viewHeight = rect.sizeDelta.y;

        int datacount = dropdown.options.Count;
        float allHeight = itemHeight * datacount;
        if (allHeight < viewHeight)
        {
            return;
        }
        else
        {
            float focusV = (dropdown.value + 0.5F) / datacount;
            float focusHeight = focusV * allHeight;

            float minHeight = viewHeight / 2;
            float maxHeight = allHeight - viewHeight / 2;

            float scrollbarvalue = 0;

            if (focusHeight <= minHeight)
            {
                scrollbarvalue = 0;
            }
            else if (focusHeight >= maxHeight)
            {
                scrollbarvalue = 0.999f;
            }
            else
            {
                focusHeight = focusHeight - minHeight;
                scrollbarvalue = focusHeight / (maxHeight - minHeight);
            }

            //if (scrollbar.direction == Scrollbar.Direction.BottomToTop)
            //{
                scrollbar.value = 1 - scrollbarvalue;
            //}
            //else if (scrollbar.direction == Scrollbar.Direction.TopToBottom)
            //{
            //    scrollbar.value = scrollbarvalue;
            //}
        }

    }
}
