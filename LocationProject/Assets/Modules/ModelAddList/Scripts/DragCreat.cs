using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCreat : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    //public string sourcename;
    //public string SubSystemName;
    //public int SubSystemID;
    //Text Name;
    LayerMask mask;
    /// <summary>
    /// AssetBundle加载
    /// </summary>
    public GameObject goPrefab;
    GameObject go;

    private void Start()
    {
        mask = LayerMask.GetMask("Floor");
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //MoseState.ChangeCameraState(MoseState.State.Bussing);
        //AssetBundle bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/SubSystem/" + sourcename + ".assetbundle");
        //go = Instantiate(bundle.LoadAsset(SubSystemName)) as GameObject;
        //bundle.Unload(false);
        go = Instantiate(goPrefab) as GameObject;
        if(!go.activeInHierarchy)
        {
            go.SetActive(true);
        }
        Debug.Log("OnBeginDrag!");
    }
    public bool Ishit;
    public void OnDrag(PointerEventData eventData)
    {
        if (go != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //Vector3 screenSpace;
            if (Physics.Raycast(ray, out hit, float.MaxValue, mask.value))
            {
                float normalY = hit.point.y + go.transform.gameObject.GetSize().y / 2;
                Ishit = true;
                go.transform.position = new Vector3(hit.point.x, normalY, hit.point.z); ;
            }
            else
            {
                Ishit = false;
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                go.transform.position = pos;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //MoseState.ChangeCameraState(MoseState.State.Waite);
        //IfDragging = false;
        //go.AddComponent<DragManage>();
    }
}
