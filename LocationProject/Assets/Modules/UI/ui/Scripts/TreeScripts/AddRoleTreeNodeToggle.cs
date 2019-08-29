using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIWidgets
{
    /// <summary>
    /// Tree node toggle.
    /// </summary>
    public class AddRoleTreeNodeToggle : UIBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        public Sprite click_Image;
        public Sprite Up_Image;
        public  bool IsOn = false;


        /// <summary>
        /// OnClick event.
        /// </summary>
        public UnityEvent OnClick = new UnityEvent();

        #region IPointerUpHandler implementation
        /// <summary>
        /// Raises the pointer up event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnPointerUp(PointerEventData eventData)
        {

        }
        #endregion

        #region IPointerDownHandler implementation
        /// <summary>
        /// Raises the pointer down event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            AddRolePermissionsTreeView.Instance.IsAddSure  = true;
            var topoTemp = this.transform.GetComponentInParent<AddEditRoleItem>().ScreenTopoTemp;

            if (!IsOn)
            {
                this.GetComponent<Image>().sprite = click_Image;
                if (!AddRolePermissionsTreeView.Instance.RolePermissionList.Contains(topoTemp.Id))
                {
                    AddRolePermissionsTreeView.Instance.RolePermissionList.Add(topoTemp.Id);
                }               
            }
            else
            {
                this.GetComponent<Image>().sprite = Up_Image;
                if (AddRolePermissionsTreeView.Instance.RolePermissionList.Contains(topoTemp.Id))
                {
                    AddRolePermissionsTreeView.Instance.RolePermissionList.Remove(topoTemp.Id);
                }               
            }
            IsOn = !IsOn;
        }
        #endregion

        /// <summary>
        /// Raises the pointer click event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {

                return;
            }

            OnClick.Invoke();
        }
    }
}


