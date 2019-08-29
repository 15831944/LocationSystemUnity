using Location.WCFServiceReferences.LocationServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnUpdate : MonoBehaviour {
    WebApiClient web;
    // Use this for initialization
    void Start () {
        web = gameObject.AddComponent<WebApiClient>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public Text txtName;
    public Text dataPerson;



    /// <summary>
    /// 修改
    /// </summary>
    public void BtnUpdatePerson()
    {
        Debug.Log("修改");
        Personnel per = new Personnel();
        per.Id = 994;
        per.Name = "cs张三up";
        web.UpdatePersonnel(per,callback,OnDisConnect);
        Debug.Log("1111");
    }


    /// <summary>
    /// 连接成功
    /// </summary>
    /// <param name="per"></param>
    private void callback(Personnel per)
    {
        Debug.Log("修改成功");
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
