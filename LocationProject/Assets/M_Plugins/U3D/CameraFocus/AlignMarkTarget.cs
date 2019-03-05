using Mogoson.CameraExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignMarkTarget : MonoBehaviour {

    #region Field and Property
    /// <summary>
    /// Target of camera align.
    /// </summary>
    public AlignTarget alignTarget;
    #endregion

    #region Protected Method
    protected virtual void Reset()
    {
        //Reset align target.
        alignTarget = new AlignTarget(transform, new Vector2(30, 0), 5, new Range(-90, 90), new Range(1, 10));
    }
    #endregion
}
