using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour {

    public Vector3 direction = new Vector3(0, 0, 1);

    public bool isActive = false;
    public MeshRenderer[] renderers;
    public List<GameObject> objs = new List<GameObject>();

    // Use this for initialization
    public GameObject obj;
    void Start()
    {
        InitRenders();
        InitSphere();
        ResetPos();
    }

    [ContextMenu("InitSphere")]
    private void InitSphere()
    {
        if (obj != null)
        {
            if (obj.transform.parent != transform)
            {
                obj = null;
            }
        }
        if (obj == null)
        {
            obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //obj.transform.parent = this.transform;
            //obj.transform.localPosition = new Vector3(10 * direction, 0, 0);
            obj.transform.parent = this.transform;
            obj.transform.localPosition = direction + new Vector3(0, -1, 0);
            obj.name = gameObject.name;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.enabled = false;
            }

            //#if !UNITY_EDITOR
            obj.gameObject.SetActive(false);
            //#endif
        }

    }

    [ContextMenu("InitRenders")]
    private void InitRenders()
    {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in renderers)
        {
            if (item.name == "Sphere")
            {
                item.enabled = false;//多出来的小球，不小心拖进去的。
                continue;
            }
            objs.Add(item.gameObject);
        }//放到前面 避免把小球放进去
    }

    [ContextMenu("ResetPos")]
    public void ResetPos()
    {
#if UNITY_EDITOR
        obj.transform.localPosition = direction*0.5f + new Vector3(0, -1, 0);
#endif
    }

    // Update is called once per frame
    void Update () {
        if (isActive)
        {
            Vector3 pos = obj.transform.position;
            Vector3 direction = Camera.main.transform.position - pos;
            RaycastHit[] hits = Physics.RaycastAll(pos, direction);
            bool isHide = false;
            foreach (var hit in hits)
            {
                if (objs.Contains(hit.collider.gameObject))
                {
                    isHide = true;
                    break;
                }
            }
            foreach (var renderer in renderers)
            {
                renderer.enabled = !isHide;
            }
            
        }
	}

    [ContextMenu("ShowHits")]
    public void ShowHits()
    {
        Debug.Log("ShowHits");
        Vector3 pos = obj.transform.position;
        Vector3 direction = Camera.main.transform.position - pos;
        RaycastHit[] hits = Physics.RaycastAll(pos, direction);
        foreach (var hit in hits)
        {
            Debug.Log(hit.collider.gameObject);
        }
    }

    [ContextMenu("ShowRenderer")]
    public void ShowRenderer()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
    }

    [ContextMenu("HideRenderer")]
    public void HideRenderer()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    public void StartHide()
    {
        isActive = true;
        Debug.Log("StartHide:" + this);
    }

    public void StopHide()
    {
        isActive = false;
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
    }
}
