using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FacilitySubSystem : MonoBehaviour {
    /// <summary>
    /// 子系统按钮预设
    /// </summary>
    public GameObject FacilityUIPrefab;
    /// <summary>
    /// 单 按钮背景图
    /// </summary>
    public Sprite singleOddSprite;
    /// <summary>
    /// 双 按钮背景图
    /// </summary>
    public Sprite doubleEvenSprite;
    /// <summary>
    /// 存放物体处
    /// </summary>
    public GameObject SubSystemContent;
    /// <summary>
    /// 子系统集合
    /// </summary>
    private List<SubSystemItem> SubSystemList = new List<SubSystemItem>();
	// Use this for initialization
	void Start () {
		
	}
	

    /// <summary>
    /// 初始化子系统信息
    /// </summary>
    /// <param name="systemList"></param>
    public void InitSubDevInfo(FacilitySystemList systemList)
    {
        HideLastSystem();
        List<FacilitySystem> SubSystem = systemList.DevList;
        int lastSystemIndex = SubSystemList.Count-1;
        List<SubSystemItem> NewItems = new List<SubSystemItem>();
        for(int i=0;i< SubSystem.Count; i++)
        {
            if(i<= lastSystemIndex)
            {
                InitItemInfo(SubSystemList[i],i,SubSystem[i]);
                SubSystemList[i].gameObject.SetActive(true);
            }
            else
            {
                GameObject obj = Instantiate(FacilityUIPrefab);
                obj.SetActive(true);
                obj.transform.parent = SubSystemContent.transform;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                SubSystemItem item = obj.GetComponent<SubSystemItem>();
                InitItemInfo(item, i, SubSystem[i]);
                NewItems.Add(item);
            }
        }
        SubSystemList.AddRange(NewItems);
        SubSystemList[0].SelectItem();
    }
    /// <summary>
    /// 设置按钮背景
    /// </summary>
    /// <param name="item"></param>
    /// <param name="Index"></param>
    private void InitItemInfo(SubSystemItem item,int Index,FacilitySystem systemInfo)
    {
        if(Index%2==0)
        {
            item.Init(systemInfo,singleOddSprite);
        }
        else
        {
            item.Init(systemInfo, doubleEvenSprite);
        }
    }
    /// <summary>
    /// 关闭上一次的信息
    /// </summary>
    private void HideLastSystem()
    {
        if(SubSystemList.Count!=0)
        {
            foreach(var item in SubSystemList)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
