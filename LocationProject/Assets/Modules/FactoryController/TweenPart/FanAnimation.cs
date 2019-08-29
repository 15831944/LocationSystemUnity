using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanAnimation : MonoBehaviour {

    public List<GameObject> FanList;//风扇列表
	// Use this for initialization
	void Start () {

        GetChild();
        Init();

    }

    private void GetChild()
    {
        FanList = new List<GameObject>();
        for(int i=0;i<transform.childCount;i++)
        {
            FanList.Add(transform.GetChild(i).gameObject);
        }
    }

    private Sequence FanSequence;
    private Vector3 roundSize = new Vector3(0,360,0);
    private void Init()
    {
        FanSequence = DOTween.Sequence();

        foreach(var item in FanList)
        {
            Tween mTween = item.transform.DOLocalRotate(roundSize, 1f,RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
            FanSequence.Join(mTween);
        }
        FanSequence.SetLoops(-1,LoopType.Restart);
        FanSequence.SetAutoKill(false);
        FanSequence.Pause();
    }
    private float minRotateDis = 50f;
    private void Update()
    {
        if (Camera.main == null) return;
        if (Vector3.Distance(Camera.main.transform.position,transform.position)< minRotateDis)
        {
            if(FanSequence!=null)
            {
                if(!FanSequence.IsPlaying())
                {
                    FanSequence.Play();
                } 
            }
        }
        else
        {
            if (FanSequence.IsPlaying()) FanSequence.Pause();
        }
    }

    private void OnDisable()
    {
        Debug.Log("FacnSequence disable...");
        if (FanSequence != null) FanSequence.Pause();
    }
}
