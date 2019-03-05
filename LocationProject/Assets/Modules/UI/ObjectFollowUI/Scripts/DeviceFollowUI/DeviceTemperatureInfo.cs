using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceTemperatureInfo : MonoBehaviour {

    public List<DeviceTemperatureItem> ItemList;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init()
    {
        //高压主汽门  中压缸排气口  //低压排气支架
        int value = Random.Range(1,4);
        for(int i=0;i< ItemList.Count; i++)
        {
            if(i<value)
            {
                ItemList[i].gameObject.SetActive(true);
                if (i == 0) ItemList[i].InitItem(25.6f,50, "高压主汽门");
                else if (i == 1) ItemList[i].InitItem(32.3f,50, "中压缸排气口");
                else ItemList[i].InitItem(27.5f,50, "低压排气支架");
            }
            else
            {
                ItemList[i].gameObject.SetActive(false);
            }
        }
    }
}
