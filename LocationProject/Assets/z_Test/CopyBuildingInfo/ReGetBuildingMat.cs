using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReGetBuildingMat : MonoBehaviour {

    public GameObject MissMaterialBuidling;

    public GameObject NoramlBuilding;

	// Use this for initialization
	void Start () {
		
	}
	[ContextMenu("ReGetMaterial")]
    public void ReGetMaterial()
    {
        MeshRenderer[] rendersMissingMat = MissMaterialBuidling.transform.GetComponentsInChildren<MeshRenderer>(false);
        MeshRenderer[] rendersNormalMat = NoramlBuilding.transform.GetComponentsInChildren<MeshRenderer>(false);
        if(rendersNormalMat==null||rendersMissingMat==null)
        {
            Debug.LogError("Building meshrender is null...");
            return;
        }
        List<MeshRenderer> missingList = rendersMissingMat.ToList();
        foreach(var normal in rendersNormalMat)
        {
             List<MeshRenderer>renderSame = missingList.FindAll(i=>i!=null&&i.transform.name==normal.transform.name);
             foreach(var item in renderSame)
            {
                if(item.transform.parent!=null && item.transform.parent.name==normal.transform.parent.name)
                {
                    item.sharedMaterials = normal.sharedMaterials;
                    break;
                }
                else
                {
                    item.sharedMaterials = normal.sharedMaterials;
                    break;
                }
            }
        }
    }
    

}
