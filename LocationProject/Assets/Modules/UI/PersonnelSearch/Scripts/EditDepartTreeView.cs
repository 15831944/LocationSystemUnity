


using UnityEngine;

namespace UIWidgets
{
    /// <summary>
    /// TreeView.
    /// </summary>
    [AddComponentMenu("UI/UIWidgets/TreeView")]
    public class EditDepartTreeView : TreeViewCustom<TreeViewComponent, TreeViewItem>
    {
        public Sprite SelectedImage;
        public Sprite HighlightImage;
        public Sprite DefaultImage;
        /// <summary>
        /// Sets component data with specified item.
        /// </summary>
        /// <param name="component">Component.</param>
        /// <param name="item">Item.</param>
        protected override void SetData(TreeViewComponent component, ListNode<TreeViewItem> item)
        {
            component.SetData(item.Node, item.Depth);
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
                component.Background.sprite = DefaultImage;
            }
        }
    }
}