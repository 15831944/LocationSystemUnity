
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace UIWidgets
{
    /// <summary>
    /// Tree node toggle.
    /// </summary>
    public class RoleTreeNodeToggle : UIBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        public Sprite click_Image;
        public Sprite Up_Image;
        public bool IsOn=false ;

        
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
                      
            RolePermissionsTreeView.Instance.IsSure = true  ;
            var topoTemp = this.transform.GetComponentInParent<EditRoleItem>().ScreenTopoTemp;
          
            if (!IsOn)
            {
                this.GetComponent<Image>().sprite = click_Image;
                if(!RolePermissionsTreeView.Instance.RolePermissionList.Contains(topoTemp.Id))
                {
                    RolePermissionsTreeView.Instance.RolePermissionList.Add(topoTemp.Id);
                }
            }
           else
            {
                this.GetComponent<Image>().sprite = Up_Image ;
                if (RolePermissionsTreeView.Instance.RolePermissionList.Contains(topoTemp.Id))
                {
                    RolePermissionsTreeView.Instance.RolePermissionList.Remove(topoTemp.Id);
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
