using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditRoleItem : MonoBehaviour
{

    public Sprite click_Image;
    public Sprite normal_Image;
    [System.NonSerialized] public PhysicalTopology ScreenTopoTemp;
    void Start()
    {

    }
    public void Init(ListNode<TreeViewItem> item)
    {
        PhysicalTopology topoTemp = item.Node.Item.Tag as PhysicalTopology;
        ScreenTopoTemp = topoTemp;
        var RoleItem = this.gameObject.transform.GetComponentInChildren<RoleTreeNodeToggle>();
        RoleItem.gameObject.transform.GetComponent<Image>().sprite = normal_Image;

        foreach (var Info in RolePermissionsTreeView.Instance.RolePermissionList)
        {
            if (Info == topoTemp.Id && RoleItem != null)
            {
                RoleItem.gameObject.transform.GetComponent<Image>().sprite = click_Image;
                RoleItem.IsOn = true;
                return;
            }

        }
        RoleItem.IsOn = false;
        RoleItem.gameObject.transform.GetComponent<Image>().sprite = normal_Image;
    }


    void Update()
    {

    }
}
