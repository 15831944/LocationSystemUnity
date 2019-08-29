using Location.WCFServiceReferences.LocationServices;
using Newtonsoft.Json;

using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebApiClient : MonoBehaviour,ICommunicationClient
{
    // Use this for initialization
    void Start()
    {
        //GetTags((txt) =>
        //{
        //    Debug.Log(txt);
        //});
        //GetTag(128, (txt) =>
        // {
        //     Debug.Log(txt);
        // });
        ////LoginOut();
    }

    public bool isGetTagsBusy = false;

    public string host = "localhost";
    public string port = "8733";

    public string GetBaseUrl()
    {
        return string.Format("http://{0}:{1}/api/", host, port);
    }

    IEnumerator GetArray<T>(string url, Action<T[]> callback,Action<string> errorCallback=null)
    {
        //Debug.Log("GetArray:" + url);
        yield return GetString(url, json =>
        {
            try
            {
                T[] array = null;
                if (json == null || json == "")
                {
                    array = null;
                }
                else
                {
                    array = JsonConvert.DeserializeObject<T[]>(json);
                }
                if (callback != null)
                {
                    callback(array);
                }

                //ThreadManager.Run(() => //将json解析过程放到子线程中处理
                //{
                //    T[] array = JsonConvert.DeserializeObject<T[]>(json);
                //    return array;
                //}, (array) =>
                //{
                //    if (callback != null)
                //    {
                //        callback(array);
                //    }
                //}, "DeserializeObject");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }, errorCallback);
    }

    //IEnumerator GetList<T>(string url, Action<List<T>> callback, Action<string> errorCallback)
    //{
    //    //Debug.Log("GetArray:" + url);
    //    yield return GetString(url, json =>
    //    {
    //        try
    //        {
    //            //var jo = JArray.Parse(json);
    //            //T[] array = jo.ToObject<T[]>();//此处Data类和Java中的结构完全一样
    //            List<T> list = JsonConvert.DeserializeObject<List<T>>(json);
    //            if (callback != null)
    //            {
    //                callback(list);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError(ex.ToString());
    //        }
    //    }, errorCallback);
    //}

    IEnumerator GetObject<T>(string url, Action<T> callback, Action<string> errorCallback=null)
    {
        //Debug.Log("GetObject:" + url);
        yield return GetString(url, json =>
        {
            try
            {
                //T obj = JObject.Parse(json).ToObject<T>();//此处Data类和Java中的结构完全一样

                T obj = default(T);
                if (json == null || json == "")
                {
                    obj = default(T);
                }
                else
                {
                    obj = JsonConvert.DeserializeObject<T>(json);
                }

                if (callback != null)
                {
                    callback(obj);
                }

                //ThreadManager.Run(() => //将json解析过程放到子线程中处理
                //{
                //    T obj = JsonConvert.DeserializeObject<T>(json);
                //    return obj;
                //}, (obj) =>
                // {
                //     if (callback != null)
                //     {
                //         callback(obj);
                //     }
                // }, "DeserializeObject");
            }
            catch (Exception ex)
            {
                Debug.LogError("WebApiClient.GetObject:"+url+"\n"+ex.ToString());
            }
        }, errorCallback);
    }

    IEnumerator DeleteString(string url, Action<string> callback, Action<string> errorCallback)
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            www.method = UnityWebRequest.kHttpVerbDELETE;
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error + "|" + url);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
                else
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            }
            else
            {
                string text = www.downloadHandler.text;
                //Debug.Log(text);
                if (callback != null)
                {
                    callback(text);
                }
            }
        }
    }


    IEnumerator DeleteObject<T>(string url, Action<T> callback, Action<string> errorCallback = null)
    {
        yield return DeleteString(url, json =>
        {
            try
            {
                //T obj = JObject.Parse(json).ToObject<T>();//此处Data类和Java中的结构完全一样

                T obj = default(T);
                if (json == null || json == "")
                {
                    obj = default(T);
                }
                else
                {
                    obj = JsonConvert.DeserializeObject<T>(json);
                }

                if (callback != null)
                {
                    callback(obj);
                }

                //ThreadManager.Run(() => //将json解析过程放到子线程中处理
                //{
                //    T obj = JsonConvert.DeserializeObject<T>(json);
                //    return obj;
                //}, (obj) =>
                // {
                //     if (callback != null)
                //     {
                //         callback(obj);
                //     }
                // }, "DeserializeObject");
            }
            catch (Exception ex)
            {
                Debug.LogError("WebApiClient.DeleteObject:" + url + "\n" + ex.ToString());
            }
        }, errorCallback);
    }



    IEnumerator GetString(string url, Action<string> callback,Action<string> errorCallback)
    {
        //Debug.Log("GetString:" + url);
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            www.method = UnityWebRequest.kHttpVerbGET;
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error+"|"+url);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
                else
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            }
            else
            {
                string text = www.downloadHandler.text;
                //Debug.Log(text);
                if (callback != null)
                {
                    callback(text);
                }
            }
        }
    }


    IEnumerator PostString(string url, Action<string> callback, Action<string> errorCallback)
    {
        //Debug.Log("GetString:" + url);
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error + "|" + url);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
                else
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
            }
            else
            {
                string text = www.downloadHandler.text;
                //Debug.Log(text);
                if (callback != null)
                {
                    callback(text);
                }
            }
        }
    }



    public IEnumerator PostObject<T>(string url,T entity,Action<T> callback,Action<string> errorCallback)
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            Debug.Log(url);
            www.method = UnityWebRequest.kHttpVerbPOST;
             string  json = JsonConvert.SerializeObject(entity);
             Debug.Log("获取json:" + json.ToString());
            if (json != null && json != "")
            {
                Debug.Log("转化为byte类型");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                Debug.Log("bodyRaw:byte");
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                Debug.Log(www.uploadHandler.ToString());
                www.SetRequestHeader("Content-Type", "application/json");
            }
            else
            {
                Debug.Log("未生成");
            }

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("错误:www.error =>"+www.error);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
            }
            else
            {
                // Show results as text
                string results = www.downloadHandler.text;
                Debug.Log("返回Json:"+results);
                if (results != null && results != "")
                {
                    try
                    {
                        T t = default(T);
                        t = JsonConvert.DeserializeObject<T>(results);
                        if(t!=null)
                        {
                          //  LoginInfo info= JsonConvert.DeserializeObject<LoginInfo>(results);
                           // Debug.Log("反序列化: "+info.UserName+","+info.Result+","+info.Authority+","+info.IsEncrypted);
                            callback(t);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("WebApiClient.PostObject:"+url+"\n"+ex.ToString());
                    }
                }
                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }


    IEnumerator PutObject<T>(string url, T entity, Action<T> callback, Action<string> errorCallback)
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = url;
            Debug.Log(url);
            www.method = UnityWebRequest.kHttpVerbPUT;
            string json = JsonConvert.SerializeObject(entity);
            Debug.Log("获取json:" + json.ToString());
            if (json != null && json != "")
            {
                Debug.Log("转化为byte类型");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                Debug.Log("bodyRaw:byte");
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                Debug.Log(www.uploadHandler.ToString());
                www.SetRequestHeader("Content-Type", "application/json");
            }
            else
            {
                Debug.Log("未生成");
            }

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("错误:www.error =>" + www.error);
                if (errorCallback != null)
                {
                    errorCallback(www.error);
                }
            }
            else
            {
                // Show results as text
                string results = www.downloadHandler.text;
                Debug.Log(results);
                if (results != null && results != "")
                {
                    try
                    {
                        T t = default(T);
                        t = JsonConvert.DeserializeObject<T>(results);
                        if (t != null)
                        {
                            callback(t);
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("WebApiClient.PostObject:" + url + "\n" + ex.ToString());
                    }
                }
                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }


    

    string getTagsUrl;

    private Tag[] tagsBuffer;

    public void GetTags(Action<Tag[]> callback)
    {
        getTagsUrl = GetBaseUrl() + "tags/detail";
        if (isGetTagsBusy == true && tagsBuffer!=null)//必须有这个，不然和InvokeRepeating结合会导致一直发送没有接收。奇怪，又没有了
        {
            //Log.Alarm("WebApi.GetTags", "isBusy == true");
            if (callback != null)
            {
                callback(tagsBuffer);//有缓存数据把缓存给他。
            }
            return;
        }
        isGetTagsBusy = true;//奇怪，又没有了
        StartCoroutine(GetArray<Tag>(getTagsUrl, (tags) =>
        {
            tagsBuffer = tags;
            if (callback != null)
            {
                callback(tags);
            }
            isGetTagsBusy = false;
        },error=>
        {
            if (callback != null)
            {
                callback(null);//这个不能忘记
            }
            isGetTagsBusy = false;//断线重连的情况
        }));
    }

    public void GetRealPositions(Action<TagPosition> callback)
    {
        string url = GetBaseUrl() + "pos";
        StartCoroutine(GetObject(url, callback));
    }

    public void GetTag(int id, Action<Tag> callback)
    {
        string url = GetBaseUrl() + "tags/" + id;
        StartCoroutine(GetObject(url, callback));
    }
    public void GetPersonTree(Action<AreaNode> callback)
    {
        string url = GetBaseUrl() + "areas/tree?view=2";
        StartCoroutine(GetObject(url, callback));
    }

    public void GetDepartmentTree(Action<Department> callback)
    {
        string url = GetBaseUrl() + "deps/tree?view=2";
        StartCoroutine(GetObject(url, callback));
    }


    public void GetTopoTree(Action<PhysicalTopology> callback)
    {
        string url = GetBaseUrl() + "areas/tree/detail?view=0";
        StartCoroutine(GetObject(url, callback));
    }

    public void GetAreaStatistics(int id, Action<AreaStatistics> callback)
    {
        string url = GetBaseUrl() + "areas/statistics?id=" + id;
        StartCoroutine(GetObject(url, callback));
    }

    public void GetPointsByPid(int areaId, Action<AreaPoints[]> callback)
    {
        string url = GetBaseUrl() + "areas/getPointsByPid?pid=" + areaId;
        StartCoroutine(GetArray(url, callback));
    }

    public void HeartBeat(string info, Action<string> callback, Action<string> errorCallback)
    {
        string url = GetBaseUrl() + "users/HeartBeat/" + info;
        StartCoroutine(GetString(url, callback,errorCallback));
    }
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="info"></param>
    /// <param name="callback"></param>
    /// <param name="errorCallback"></param>
    public void Login(LoginInfo info, Action<LoginInfo> callback,Action<string> errorCallback)
    {
     string url = GetBaseUrl() + "users/LoginPost";
        Debug.Log(url);
        StartCoroutine(PostObject(url,info,callback,errorCallback));
    }

    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="info"></param>
    /// <param name="callback"></param>
    /// <param name="errorCallback"></param>
    public void LoginOut(LoginInfo info, Action<LoginInfo> callback, Action<string> errorCallback)
    {
        string url = GetBaseUrl() + "users/LogoutPost";
        Debug.Log(url);
        StartCoroutine(PostObject(url,info,callback,errorCallback));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <param name="callback"></param>
    /// <param name="errorCallback"></param>
    public void KeepLive(LoginInfo info, Action<LoginInfo> callback, Action<string> errorCallback)
    {
        string url = GetBaseUrl() + "users/KeepLivePost";
        Debug.Log(url);
        StartCoroutine(PostObject(url, info, callback, errorCallback));

    }



    /// <summary>
    /// 获取人员列表
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    /// <param name="errorCallback"></param>
    public void getListPersonls(string name, Action<Personnel[]> callback, Action<string> errorCallback)
    {
        Debug.Log(name);
        string url = GetBaseUrl();
        //if (name != null && name != "")
        //{
        //    url += "persons/list";
        //}
        //else
        //{
        //    url += "persons/search/?name="+name;
        //}
        url += "persons/list";

        Debug.Log(url);
        StartCoroutine(GetArray(url, callback, errorCallback));
    }

    /// <summary>
    /// 添加人员
    /// </summary>
    /// <param name="person"></param>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void AddPersonnel(Personnel person, Action<Personnel> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl()+ "persons";
        Debug.Log(url);
        StartCoroutine(PostObject(url,person,callBack,errorBack));
    }
    /// <summary>
    /// 修改人员
    /// </summary>
    /// <param name="person"></param>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void UpdatePersonnel(Personnel person, Action<Personnel> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl() + "persons";
        Debug.Log(url);
        StartCoroutine(PutObject(url, person, callBack, errorBack));
    }

    /// <summary>
    /// 删除人员
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void DeletePersonnel(string id, Action<Personnel> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl()+ "persons/"+id;
        Debug.Log(url);
        StartCoroutine(DeleteObject(url,callBack,errorBack));
    }

    /// <summary>
    /// 根据ID查询人员信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void GetPersonnelById(string id, Action<Personnel> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl() + "persons/" + id;
        Debug.Log(url);
        StartCoroutine(GetObject(url, callBack, errorBack));
    }


    /// <summary>
    /// 获取设备列表
    /// </summary>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void GetDevList(Action<DevInfo[]> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl() + "devices/list";
        Debug.Log(url);
        StartCoroutine(GetArray(url,callBack,errorBack));
    }

    /// <summary>
    /// 获取设备详细信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void GetDevinfoById(string id, Action<DevInfo> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl() + "devices/"+id;
        Debug.Log(url);
        StartCoroutine(GetObject(url, callBack, errorBack));
    }

    /// <summary>
    /// 添加设备
    /// </summary>
    /// <param name="devinfo"></param>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void AddDevinfo(DevInfo devinfo, Action<DevInfo> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl() + "devices" ;
        Debug.Log(url);
        StartCoroutine(PostObject(url, devinfo,callBack, errorBack));
    }


    /// <summary>
    /// 修改设备
    /// </summary>
    /// <param name="devinfo"></param>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void UpdateDevinfo(DevInfo devinfo, Action<DevInfo> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl() + "devices";
        Debug.Log(url);
        StartCoroutine(PutObject(url, devinfo, callBack, errorBack));
    }
    /// <summary>
    /// 删除设备
    /// </summary>
    /// <param name="id"></param>
    /// <param name="callBack"></param>
    /// <param name="errorBack"></param>
    public void DeleteDevinfo(string id, Action<DevInfo> callBack, Action<string> errorBack)
    {
        string url = GetBaseUrl() + "devices?id="+id;
        Debug.Log(url);
        StartCoroutine(DeleteObject(url, callBack, errorBack));

    }





}