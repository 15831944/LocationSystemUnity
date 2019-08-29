using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxPer : MonoBehaviour {
    public static MessageBoxPer Instance;
    public Text infoText;
    public Button ShowBut;
    public Button CloseBut;
    public Button DeltBut;
    public GameObject MessageWindow;

    void Start () {
        Instance = this;
        ShowBut.onClick.AddListener(()=>
        {
            CloseMessageWindow(null);
        } );
        CloseBut.onClick.AddListener(() =>
        {
            CloseMessageWindow(null);
        });
        DeltBut.onClick.AddListener(() =>
        {
            CloseMessageWindow(null);
        });
    }
	
	public void ShowMessageWindow(Action action = null)
    {
        MessageWindow.SetActive(true);
        if (action != null) action();
    }
    public void CloseMessageWindow(Action action = null)
    {
        MessageWindow.SetActive(false );
        if (action != null) action();
    }

    void Update () {
		
	}
}
