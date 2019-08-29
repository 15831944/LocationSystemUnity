using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryArrowController : MonoBehaviour {

    //[Range(0,1)]
    //public float coefficient;//系数

    public float minScale = 1;//最小比例
    public float maxScale = 5;//最大比例

    public float minDistance = 3;//最小变化距离
    public float maxDistance = 100;//最大变化距离

    public float distance;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SetScale();
    }

    public void SetScale()
    {
        distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        float x = (maxScale - minScale) / (maxDistance - minDistance);
        if(distance >= minDistance && distance <= maxDistance)
        {
            float v = (distance - minDistance) * x + 1f;
            transform.localScale = new Vector3(v, v, v);
        }
    }

}
