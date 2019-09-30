using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSmokeChange : MonoBehaviour {

    public List<GameObject> SmokeList;
	// Use this for initialization
	void Start () {
        //SceneEvents.DepNodeChangeStart += OnDepChangeStart;//PowerGroupStateManage会根据机组实际情况，调整烟雾状态

    }
    private string WaterBuildingName = "供水与水处理建筑物";
    private void OnDepChangeStart(DepNode argOld,DepNode argNew)
    {
        if (SmokeList == null) return;
        if (argNew is FactoryDepManager ||(argNew is DepController&&argNew.NodeName== WaterBuildingName))
        {
            SmokeList.SetActive(true);
        }
        else
        {
            SmokeList.SetActive(false);
        }        
    }
}
