using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Delete : MonoBehaviour {
    WebApiClient web;
    // Use this for initialization
    void Start () {
        web = gameObject.AddComponent<WebApiClient>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public Text dataPerson;
    /// <summary>
    /// 删除
    /// </summary>
    public void BtnDelete()
    {
        Debug.Log("删除");
        string id = "994";
        web.DeletePersonnel(id,callback,OnDisConnect);
    }

    /// <summary>
    /// 连接成功
    /// </summary>
    /// <param name="per"></param>
    private void callback(Personnel per)
    {
        Debug.Log("删除成功");
        dataPerson.text = "id:" + per.Id.ToString() + ",name:" + per.Name.ToString();
    }

    /// <summary>
    /// 连接失败
    /// </summary>
    /// <param name="errorInfo"></param>
    private void OnDisConnect(string errorInfo)
    {
        Debug.Log(errorInfo.ToString());
    }


}
