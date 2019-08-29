using Location.WCFServiceReferences.LocationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WebApi : MonoBehaviour
{
    string cookie;
    public InputField inputRegisterEmail;
    public InputField inputRegisterPassword;

    public InputField inputLoginEmail;
    public InputField inputLoginPassword;

    public InputField inputGetID;

    public InputField inputPostName;
    public InputField inputPostNumber;

    public InputField inputPutID;
    public InputField inputPutName;
    public InputField inputPutNumber;

    public InputField inputDeleteID;

    // Use this for initialization
    void Start()
    {
        cookie = "";
    }


    public void Register()
    {
        StartCoroutine(IERegister());
    }

    IEnumerator IERegister()
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = "http://localhost:2572/api/account/register";
            www.method = UnityWebRequest.kHttpVerbPOST;

            www.SetRequestHeader("Cookie", cookie);

            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"Email\": \"" + inputRegisterEmail.text + "\",\"password\": \"" + inputRegisterPassword.text + "\",\"ConfirmPassword\": \"" + inputRegisterPassword.text + "\"}");
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }

    public void Login()
    {
        StartCoroutine(IELogin());
    }

    IEnumerator IELogin()
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = "http://localhost:2572/api/account/login";
            www.method = UnityWebRequest.kHttpVerbPOST;

            www.SetRequestHeader("Cookie", cookie);

            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"Email\": \"" + inputLoginEmail.text + "\",\"password\": \"" + inputLoginPassword.text + "\",\"remeberme\": false }");
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
                cookie = www.GetResponseHeader("Set-Cookie");
                Debug.Log(cookie);
                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }

    public void LoginOut()
    {
        StartCoroutine(IELoginOut());
    }

    IEnumerator IELoginOut()
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            //www.url = "http://localhost:2572/api/account/loginOut";
            www.url = "http://localhost:8733/api/tags";
            www.method = UnityWebRequest.kHttpVerbGET;

            www.SetRequestHeader("Cookie", cookie);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                cookie = www.GetResponseHeader("Set-Cookie");
                Debug.Log(cookie);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }

    public void AllAnimes()
    {
        StartCoroutine(IEAllAnimes());
    }

    IEnumerator IEAllAnimes()
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = "http://localhost:2572/api/Animes";
            www.method = UnityWebRequest.kHttpVerbGET;

            www.SetRequestHeader("Cookie", cookie);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }

    public void Anime()
    {
        StartCoroutine(IEAnime());
    }

    IEnumerator IEAnime()
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = "http://localhost:2572/api/Animes/" + inputGetID.text;
            www.method = UnityWebRequest.kHttpVerbGET;

            www.SetRequestHeader("Cookie", cookie);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);


                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }

    public void PostAnime()
    {
        StartCoroutine(IEPostAnime());
    }

    IEnumerator IEPostAnime()
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = "http://localhost:2572/api/animes";
            www.method = UnityWebRequest.kHttpVerbPOST;

            www.SetRequestHeader("Cookie", cookie);

            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"Name\": \"" + inputPostName.text + "\",\"Number\": \"" + inputPostNumber.text + "\",\"DateTime\": \"2011 - 08 - 01T00: 00:00\",\"IndexShow\":false}");
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }

    public void PutAnime()
    {
        StartCoroutine(IEPutAnime());
    }

    IEnumerator IEPutAnime()
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = "http://localhost:2572/api/animes/" + inputPutID.text;
            www.method = UnityWebRequest.kHttpVerbPUT;

            www.SetRequestHeader("Cookie", cookie);

            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"ID\":" + inputPutID.text + ",\"Name\": \"" + inputPutName.text + "\",\"Number\": \"" + inputPutNumber.text + "\",\"DateTime\": \"2011 - 08 - 01T00: 00:00\",\"IndexShow\":false}");
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }


    public void DeleteAnime()
    {
        StartCoroutine(IEDeleteAnime());
    }

    IEnumerator IEDeleteAnime()
    {
        using (UnityWebRequest www = new UnityWebRequest())
        {
            www.url = "http://localhost:2572/api/Animes/" + inputDeleteID.text;
            www.method = UnityWebRequest.kHttpVerbDELETE;

            www.SetRequestHeader("Cookie", cookie);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);


                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
    }
}