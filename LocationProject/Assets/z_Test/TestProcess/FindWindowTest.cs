using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FindWindowTest : MonoBehaviour {
    public Button FindWindowButton;
	// Use this for initialization
	void Start () {
        FindWindowButton.onClick.AddListener(button1_Click);

    }

    


    private void button1_Click()
    {
        FindWindowByName("视频监控");
        FindWindowByName("HUSSDKDemo");//一个叫HUSSDEDemo的文件夹，可以被找到
        FindWindowByName("Form1");
    }

    public static IntPtr FindWindowByName(string windowName)
    {
        IntPtr window1 = FindWindow(null, (windowName));
        if (window1.ToInt32() != 0)
        {
            int pId = -1;
            GetWindowThreadProcessId(window1,out pId);
           Debug.LogError("FindWindow:" + windowName + " " + window1.ToInt32()+"  "+pId);
        }
        return window1;
    }

    /// <summary>
    /// 根据窗口标题查找窗体
    /// </summary>
    /// <param name="lpClassName"></param>
    /// <param name="lpWindowName"></param>
    /// <returns></returns>
    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    /// <summary>
    /// 根据句柄查找进程ID
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="ID"></param>
    /// <returns></returns>
    [System.Runtime.InteropServices.DllImport("User32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
}
