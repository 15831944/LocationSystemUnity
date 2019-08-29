using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnLocatedAreas : MonoBehaviour {
    public static UnLocatedAreas Instance;
    /// <summary>
    /// 非定位区域
    /// </summary>
    public List<BoxCollider> nonProdctionAreas;

    private List<Bounds> nonProductionBounds = new List<Bounds>();
	// Use this for initialization
	void Awake () {
        Instance = this;
    }
    private void AddBounds()
    {
        foreach(var item in nonProdctionAreas)
        {
            Bounds bound = new Bounds();
            bound.center = item.bounds.center;
            bound.extents = item.bounds.extents;
            Debug.LogErrorFormat("BoundName:{0} center:{1} extents:{2}",item.gameObject.name,bound.center,bound.extents);
        }
    }
    [ContextMenu("TestPos")]
    public void Test()
    {
        Vector3 cadPos = new Vector3(2146.4f,1,1826.6f);
        bool isNoProductionArea = IsInNonProductionArea(cadPos);
        Debug.LogError("TestArea:"+isNoProductionArea);
    }
    /// <summary>
    /// 是否在非生产区域
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsInNonProductionArea(Vector3 cadPos)
    {
        if(nonProdctionAreas==null)
        {
            BoxCollider[] cols = transform.GetComponentsInChildren<BoxCollider>(false);
            if (cols == null || cols.Length == 0) return false;
            else
            {
                nonProdctionAreas = new List<BoxCollider>();
                nonProdctionAreas.AddRange(cols);
            }
        }
        if (nonProdctionAreas == null || nonProdctionAreas.Count == 0) return false;
        Vector3 unityPos = LocationManager.CadToUnityPos(cadPos,false);
        unityPos.y = 1;//高度默认1，主要是计算X和Z
        foreach (var box in nonProdctionAreas)
        {
            if (box.bounds.Contains(unityPos))
            {
                return true;
            }
        }
        return false;
    }

    private bool isContain(Bounds bound,Vector3 pos,bool isTest)
    {
        //if(isTest)
        //{
        //    Debug.LogErrorFormat("Pos:{0} bound.Center:{1} extents:{2}",pos,bound.center,bound.extents);
        //}
        bool isInBoundX = pos.x > bound.center.x - bound.extents.x && pos.x < bound.center.x + bound.extents.x;
        if (!isInBoundX) return false;
        bool isInBoundY= pos.y > bound.center.y - bound.extents.y && pos.y < bound.center.y + bound.extents.y;
        if (!isInBoundY) return false;
        bool isInBoundZ = pos.z > bound.center.z - bound.extents.z && pos.z < bound.center.z + bound.extents.z;
        if (!isInBoundZ) return false;
        return true;
    }
}
