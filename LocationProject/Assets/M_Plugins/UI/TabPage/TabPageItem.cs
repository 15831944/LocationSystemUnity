using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TabPageItem : MonoBehaviour {

    public GameObject panel;//要控制的panel
    public Toggle tab;

    private CanvasGroup panelCanvasGroup;
    // Use this for initialization
    void Start () {
        if (panel)
        {
            panelCanvasGroup = panel.GetComponent<CanvasGroup>();
        }
        if (tab == null)
        {
            tab = GetComponent<Toggle>();
            tab.onValueChanged.AddListener(On_ToggleChanged);
            if (!tab.isOn)
            {
                if (panelCanvasGroup)
                {
                    panelCanvasGroup.alpha = 0;
                }
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void On_ToggleChanged(bool b)
    {
        Debug.LogError(tab.name + ":" + b);
        if (panel == null) return;
        if (tab.isOn)
        {
            //panel.SetActive(true);
            ShowPanel();
            print(panel.name + ":True");
        }
        else
        {
            //panel.SetActive(false);
            HidePanel();
            print(panel.name + ":False");
        }
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
        if (panelCanvasGroup)
        {
            panelCanvasGroup.DOFade(1, 0.5f);
        }
    }

    public void HidePanel()
    {
        panel.SetActive(false);
        if (panelCanvasGroup)
        {
            panelCanvasGroup.alpha = 0;
        }
    }
}
