using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpreationEquipTree : MonoBehaviour {


    private Button btn_ReFresh; //刷新树
    private Button btn_ReTract;//收起树
    private Image Im_ReFresh; //刷新树
    TopoTreeManager FindEquipTree; //找到设备树
	void Start () {
        FindEquipTree = TopoTreeManager.Instance;
        btn_ReTract = GameObject.Find("ReTract_Button").GetComponent<Button>();
        btn_ReFresh = GameObject.Find("Refresh_Button").GetComponent<Button>();
        Im_ReFresh = GameObject.Find("Refresh_Button/Image").GetComponent<Image>();
        btn_ReTract.onClick.AddListener(ReTractTree);
        btn_ReFresh.onClick.AddListener(ReFreshTree);
	}
    /// <summary>
    /// 收起树
    /// </summary>
	public void ReTractTree()
    {
        TreeControlViewManager.Instance.ResTractTree(FindEquipTree.Tree);
    }
    //刷新树
    public void ReFreshTree()
    {
        if(FindEquipTree.Tree != null)
        {
            TopoTreeManager.Instance.ResizeTree();
            TreeControlViewManager.Instance.RefreshIconRotate(Im_ReFresh);
        }   
    }
	// Update is called once per frame
	void Update () {
   
    }
}
