using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundlessssss : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    public string ModelName;
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
        {
           
            Test();
        }
	}
    private void Test()
    {
        string modelName = ModelName;
        Debug.Log("Load Model:" + modelName);
        for(int i=0;i<3;i++)
        {
            AssetbundleGet.Instance.GetObj(modelName, AssetbundleGetSuffixalName.prefab, DragInstantiateFun);
        }       
    }
    private void DragInstantiateFun(Object obj)
    {
        if (obj == null)
        {
            Debug.LogError("拖动获取不到模型");
            return;
        }
        else
        {
            Debug.Log("Load Model success!");
        }

        GameObject g = obj as GameObject;

        GameObject o = Instantiate(g);
        //o.AddMissingComponent<BoxCollider>();
        //o.AddMissingComponent<RoomDevMouseDrag>();
        //o.AddMissingComponent<RoomDevController>();
    }
}
