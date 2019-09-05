using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlashesRedAndAudio : MonoBehaviour {

    public static ScreenFlashesRedAndAudio Instance;
    public GameObject ScreenFlashesRed;
    public AudioSource Audio;
    public Tweener FlashesRedTweener;
    Color FlashesRed = new Color(255 / 255f, 0f, 0f, 100 / 255f);
  
    private void Awake()
    {
        Instance = this;
    } 
 
    public void CreateFlashesRedTweener()
    {
        FlashesRedTweener = ScreenFlashesRed.GetComponent<Image>().DOColor(FlashesRed,0.5f);
        FlashesRedTweener.SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        FlashesRedTweener.Pause();        
        FlashesRedTweener.SetAutoKill(false);
    }
    public void FlashesRedTweenerPlay()
    {
        if (ScreenFlashesRed == null || Audio.gameObject == null) return;
        StopFlashesRedTweenerAndAudio();
        StartCoroutine(StopTime());
        FlashesRedTweener.Play ();
        Audio.Play();
    }
   
    public void FlashesRedTweenerStop_Click()
    {
        if (ScreenFlashesRed==null  || Audio.gameObject==null ) return;
        StopFlashesRedTweenerAndAudio();
    }
    public void StopFlashesRedTweenerAndAudio()
    {     
        if (FlashesRedTweener.IsPlaying() || Audio.isPlaying)
        {
            FlashesRedTweener.Pause();
            FlashesRedTweener.Rewind();
            Audio.Stop();
            StopAllCoroutines();
        }
    }
    void Start () {
        CreateFlashesRedTweener();

    }
	IEnumerator StopTime()
    {
        yield return new WaitForSeconds(10.0f);
        FlashesRedTweenerStop_Click();
    
        Debug.LogError("StopTime");
    }
	// Update is called once per frame
	void Update () {
        
       
    }
}
