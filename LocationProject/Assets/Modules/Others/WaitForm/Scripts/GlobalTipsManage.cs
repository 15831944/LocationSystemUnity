using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalTipsManage : MonoBehaviour {
    public static GlobalTipsManage Instance;
    public GameObject window;
    public Text tipsText;
    public GameObject ExitButton;//退出按钮
	// Use this for initialization
	void Awake () {
        Instance = this;
        if (ExitButton != null&& ExitButton.GetComponent<Button>()) ExitButton.GetComponent<Button>().onClick.AddListener(OnExitClick);
    }
	
    private void OnExitClick()
    {
        Debug.LogError("GlobalTipsManage.QuitApplication...");
        Application.Quit();
    }

    public void Show(string info,bool showExitButton)
    {
        if(!window.activeInHierarchy)window.SetActive(true);
        if(ExitButton)ExitButton.SetActive(showExitButton);
        tipsText.text = info;
    }
    public void Close()
    {
        if (!window.activeInHierarchy) return;
        window.SetActive(false);
        if (ExitButton) ExitButton.SetActive(false);
        tipsText.text = "";
    }
}
