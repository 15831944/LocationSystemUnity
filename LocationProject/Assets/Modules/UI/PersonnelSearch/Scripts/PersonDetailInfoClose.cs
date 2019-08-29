using DG.Tweening;
using UnityEngine;

public class PersonDetailInfoClose : MonoBehaviour {
    public PersonDetailInfoClose personDetailInfoClose;
    public GameObject ui;

    private void Start()
    {
        //GameObject ui = EventSystem.current.currentSelectedGameObject;
       
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
