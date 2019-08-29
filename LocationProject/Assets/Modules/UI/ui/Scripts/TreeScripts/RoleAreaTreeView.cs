using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
    /// <summary>
    /// TreeView.
    /// </summary>
    [AddComponentMenu("UI/UIWidgets/TreeView")]
    public class RoleAreaTreeView : TreeView
    {
        public Sprite SelectedImage;
        public Sprite  HighlightImage;
        public Sprite DefaultImage;
       
        /// <summary>
        /// 文本框宽度
        /// </summary>
        private float NormalTextWidth = 450;

        public void ResizeContent()
        {
            base.Resize();
        }
        /// <summary>
        /// Sets component data with specified item.
        /// </summary>
        /// <param name="component">Component.</param>
        /// <param name="item">Item.</param>
        protected override void SetData(TreeViewComponent component, ListNode<TreeViewItem> item)
        {
            component.SetData(item.Node, item.Depth);
            ChangeRoleInfo(component, item);           
        }

        public void ChangeRoleInfo(TreeViewComponent component, ListNode<TreeViewItem> item)
        {
            EditRoleItem RoleItem = component.GetComponent<EditRoleItem>();
            if (item.Node.Item.Tag is PhysicalTopology)
            {
                PhysicalTopology topoTemp = item.Node.Item.Tag as PhysicalTopology;
                if (RoleItem) RoleItem.Init(item);
            }
            float offset = item.Depth * component.PaddingPerLevel;
            LayoutElement element = component.Text.GetComponent<LayoutElement>();
            if (item.Node.Nodes != null && item.Node.Nodes.Count != 0)
            {
                element.preferredWidth = NormalTextWidth - offset;
            }
            else
            {
                float toggleSize = 0;
                if (item.Depth != 0) toggleSize = component.Toggle.GetComponent<LayoutElement>().preferredWidth;
                element.preferredWidth = NormalTextWidth - offset + toggleSize;
            }
        }
        /// <summary>
        /// Set highlights colors of specified component.
        /// </summary>
        /// <param name="component">Component.</param>
        protected override void HighlightColoring(TreeViewComponent component)
        {
            base.HighlightColoring(component);
         
            component.Background.sprite = HighlightImage;
        }

        /// <summary>
        /// Set select colors of specified component.
        /// </summary>
        /// <param name="component">Component.</param>
        protected override void SelectColoring(TreeViewComponent component)
        {
            base.SelectColoring(component);
      
            component.Background.sprite = SelectedImage;
        }

        /// <summary>
        /// Set default colors of specified component.
        /// </summary>
        /// <param name="component">Component.</param>
        protected override void DefaultColoring(TreeViewComponent component)
        {
            if (component == null)
            {
                return;
            }
            base.DefaultColoring(component);
            if (component.Text != null)
            {
                //  component.Text.color = DefaultColor;
                component.Background.sprite = DefaultImage;
            }
        }
    }
}

