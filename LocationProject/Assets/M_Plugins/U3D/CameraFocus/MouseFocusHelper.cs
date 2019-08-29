using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mouse click to focus camera to target.
/// </summary>
[RequireComponent(typeof(AroundAlignCamera))]
public class MouseFocusHelper : MonoBehaviour {

    #region Field and Property
    /// <summary>
    /// Layer of ray.
    /// </summary>
    public LayerMask layerMask = 1;

    /// <summary>
    /// Max distance of ray.
    /// </summary>
    public float maxRayDistance = 100;

    /// <summary>
    /// Current focus state.
    /// </summary>
    public bool IsFocus { protected set; get; }

    /// <summary>
    /// Focus enter/exit click count.
    /// </summary>
    public int clickCount = 1;

    /// <summary>
    /// Camera to ray.
    /// </summary>
    protected Camera targetCamera;

    /// <summary>
    /// Around and align component.
    /// </summary>
    protected AroundAlignCamera alignCamera;

    /// <summary>
    ///  Camera default align.
    /// </summary>
    protected AlignTarget defaultAlign;
    #endregion

    #region Protected Method
    protected virtual void Start()
    {
        targetCamera = GetComponent<Camera>();
        alignCamera = GetComponent<AroundAlignCamera>();
    }

    protected virtual void OnGUI()
    {
        if (CheckFocusEnter())
        {
            var ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            var hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, maxRayDistance, layerMask))
            {
                var alignMark = hitInfo.transform.GetComponent<AlignMarkTarget>();
                if (alignMark)
                {
                    if (IsFocus == false)
                    {
                        defaultAlign = new AlignTarget(alignCamera.GetTarget(), alignCamera.CurrentAngles,
                            alignCamera.CurrentDistance, alignCamera.angleRange, alignCamera.distanceRange);
                        IsFocus = true;
                    }
                    alignCamera.AlignVeiwToTarget(alignMark.alignTarget);
                }
            }
        }
        else if (IsFocus && CheckFocusExit())
        {
            alignCamera.AlignVeiwToTarget(defaultAlign);
            IsFocus = false;
        }
    }

    /// <summary>
    /// Check enter focus state.
    /// </summary>
    /// <returns>Is enter focus state.</returns>
    protected virtual bool CheckFocusEnter()
    {
        //Mouse left button double click.
        return Event.current.isMouse && Event.current.button == 0 && Event.current.clickCount == clickCount;
    }

    /// <summary>
    /// Check exit focus state.
    /// </summary>
    /// <returns>Is exit focus state.</returns>
    protected virtual bool CheckFocusExit()
    {
        //Mouse right button double click.
        return Event.current.isMouse && Event.current.button == 1 && Event.current.clickCount == clickCount;
    }
    #endregion
}
