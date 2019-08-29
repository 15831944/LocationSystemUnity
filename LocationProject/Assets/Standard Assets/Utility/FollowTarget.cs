using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);

        //public bool RemoveParent = true;

        //void Start()
        //{
        //    transform.parent = null;
        //}

        private void LateUpdate()
        {
            if(target!=null)
                transform.position = target.position + offset;
        }
    }
}
