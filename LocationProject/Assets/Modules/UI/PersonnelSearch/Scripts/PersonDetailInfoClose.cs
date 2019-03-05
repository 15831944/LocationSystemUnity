using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PersonDetailInfoClose : MonoBehaviour {
     PersonDetailInfoClose personDetailInfoClose;
     GameObject ui;

    private void Start()
    {
        GameObject ui = EventSystem.current.currentSelectedGameObject;
       
    } 
    public void Update()
    {//点击除了当前UI界面
        if (Input.GetMouseButtonDown(0))
        {
          
            if (ui == null)
            {
            
                PersonnelSearchTweener.Instance.openTweener.PlayBackwards();

            }
         
            if (personDetailInfoClose == null)
            {
                //   openTweener.PlayBackwards();
                PersonnelSearchTweener.Instance.openTweener.PlayBackwards();
               
            }
        }
    }
}
