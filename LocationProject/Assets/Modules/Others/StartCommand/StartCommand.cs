using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Text;

/// <summary>
///Unity3D-获取命令行启动
///用命令打开exe模式,控制脚本
/// </summary>
public class StartCommand : MonoBehaviour
{

    public static StartCommand Instance;
    /// <summary>
    /// 是否启用：Unity3D-获取命令行启动
    /// </summary>
    public bool isEnableSetLogin;
    private bool isLogined = false;//是否已经登录过了
    private bool isAutoLongin = true;

    public DianChangLogin login;

    public GameObject StartCommandPanelTest;
    public Text CommandLineText;
    public Text CommandLineArgsText;
    public Text IpText;
    public Text PortText;
    public Text UserNameText;
    public Text Passward;

    private string DefaultIp = "127.0.0.1";
    private string DefaultPort = "8733";


    private void Awake()
    {
        Instance = this;
        
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("StartCommand: 001");
        DefaultIp = SystemSettingHelper.systemSetting.CommunicationSetting.Ip1;
        DefaultPort = SystemSettingHelper.systemSetting.CommunicationSetting.Port1;
        if (login == null)
        {
            login = DianChangLogin.Instance;
        }
        if (SystemSettingHelper.systemSetting.IsDebug)
        {
            SetTestPanel(true);
        }
        else
        {
            SetTestPanel(false);
        }
        //初始化通信
        StartCommandRun();

    }

    public void SetTestPanel(bool isactive)
    {
        StartCommandPanelTest.SetActive(isactive);
    }

    /// <summary>
    /// 获取命令行相关参数
    /// </summary>
    public void StartCommandRun()
    {
        string CommandLine = Environment.CommandLine;//获取命令行
        string[] CommandLineArgs = Environment.GetCommandLineArgs();//获取命令相关参数
        string text1 = "CommandLine : " + CommandLine;
        string text2 = CommandLineArgs.Aggregate<string, string>(
            "CommandLineArgs : ",
            (a, b) => a + "|" + b);
        CommandLineText.text = text1;
        CommandLineArgsText.text = text2;
        Debug.Log(text1+","+ text2);
        string[] abs = text2.Split(new string[] { "|", "%7C" }, StringSplitOptions.None);//这里分隔加%7C，是因为谷歌浏览器传过来的数据会把"|"转化为 "%7C"。
        if (abs!=null&&abs.Length>0)
        {
            string value = "";
            foreach (string item in abs)
            {
                value += item + "\n";
                Debug.Log("item:"+item);
            }
            Debug.LogError("Recieve form browser:"+value);
        }
        

        string ipstr = "";
        string portstr = "";
        string userNamestr = "";
        string passwardstr = "";

        if (abs.Length > 2)
        {
            string str = abs[2];
            string s = "locationsystem:";
            if (str.Contains("locationsystem:"))
            {
                str = str.Remove(0, s.Length);
            }

            IpText.text = str;
            ipstr = str;
        }
        else
        {
            //#if !UNITY_EDITOR
            if (isEnableSetLogin)
            {
                //IpText.text = "1";
                IpText.text = "";
                ipstr = DefaultIp;
                //IpText.text = "127.0.0.1%7C8733%7Cadmin%7Cadmin";
            }
            //#endif
            isAutoLongin = false;
        }
        if (abs.Length > 3)
        {
            PortText.text = abs[3];
            portstr = abs[3];
        }
        else
        {

            if (isEnableSetLogin)
            {
                //PortText.text = "2";
                PortText.text = "";
                portstr = DefaultPort;
            }

        }

        if (abs.Length > 4)
        {
            UserNameText.text = abs[4];
            userNamestr = abs[4];
        }
        else
        {

            if (isEnableSetLogin)
            {
                //UserNameText.text = "";
                UserNameText.text = "";
                userNamestr = "Admin";
            }

        }

        if (abs.Length > 5)
        {
            Passward.text = abs[5];
            passwardstr = abs[5];
        }
        else
        {

            if (isEnableSetLogin)
            {
                //Passward.text = "";
                Passward.text = "";
                passwardstr = "Admin@123456";
            }

        }


        if (isEnableSetLogin)
        {
            //login.SetInfo(IpText.text.Trim(), PortText.text.Trim(), UserNameText.text.Trim(), Passward.text.Trim());
            Debug.LogErrorFormat("SetInfo:{0} {1} {2} {3}", ipstr, portstr, userNamestr, passwardstr);
            login.SetInfo(ipstr, portstr, userNamestr, passwardstr);
            
        }

        if (abs.Length>2)
        {
            Debug.Log("进入设置登录");
            login.Login_Click();//设置并登陆
        }

    }


    // Update is called once per frame
    void Update()
    {
#if !UNITY_EDITOR
        if (isEnableSetLogin && !isLogined&&isAutoLongin)
        {
            isLogined = true;
            login.LoginBtn_Trigger();
        }
#endif
    }

}
