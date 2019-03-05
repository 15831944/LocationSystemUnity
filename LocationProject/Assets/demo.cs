using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Vectrosity;
using Mogoson.CameraExtension;
using RTEditor;
using HighlightingSystem;

public class demo : MonoBehaviour {

    //public List<Transform> points;
    //public GameObject target;
    //public AroundAlignCamera aroundAlignCamera;
    private void Awake()
    {
        Debug.LogError("Awake");
    }
    private void OnEnable()
    {
        Debug.LogError("OnEnable");
    }
    // Use this for initialization
    void Start () {
        //EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragStart += TranslationGizmo_GizmoDragStart;
        //EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragUpdate += TranslationGizmo_GizmoDragUpdate;
        //EditorGizmoSystem.Instance.TranslationGizmo.GizmoDragEnd += TranslationGizmo_GizmoDragEnd;
        //EditorGizmoSystem.Instance.ActiveGizmoTypeChanged += Instance_ActiveGizmoTypeChanged;
        //EditorObjectSelection.Instance.SelectionChanged += Instance_SelectionChanged;
        //EditorObjectSelection.Instance.SelectionDuplicated += Instance_SelectionDuplicated;
        //EditorObjectSelection.Instance.SelectionDeleted += Instance_SelectionDeleted;
        //Debug.LogError("Start");
        ////DotweenTest();
        //Canvas canvas = transform.GetComponentInParent<Canvas>();
        //aroundAlignCamera.OnAlignEnd += AroundAlignCamera_OnAlignEnd;
        //EventTriggerListener lis = EventTriggerListener.Get(imageObj);
        //EditorObjectSelection.Instance.ClearSelection()
    }

    private void AroundAlignCamera_OnAlignEnd()
    {
        //throw new System.NotImplementedException();
        Debug.Log("AroundAlignCamera_OnAlignEnd");
    }

    float time = 0;

    // Update is called once per frame
    void Update () {

        //if (time < 10f)
        //{
        //    time += Time.deltaTime;
        //}
        //else
        //{
        //    imageObj.SetActive(false);
        //    time = 0;
        //}
        //Debug.Log(time);
	}

    /// <summary>
    /// 点击测试
    /// </summary>
    public void On_Click()
    {
        print("点击测试!");
        //Sequence E = DOTween.Sequence();
        //E.Complete();
    }

    //public void DotweenTest()
    //{
    //    target.transform.position = points[0].position;
    //    Sequence mySequence = DOTween.Sequence();
    //    mySequence.Append(target.transform.DOMove(points[1].position, 2f));
    //    mySequence.Append(target.transform.DOMove(points[2].position, 4f));
    //    mySequence.Append(target.transform.DOMove(points[3].position, 2f));
    //    mySequence.Append(target.transform.DOMove(points[0].position, 4f));
    //    mySequence.SetLoops(-1);
    //    mySequence.Play();
    //}

    //public GameObject On_CLICK_OBJ;

    //public void On_CLICK()
    //{
    //    On_CLICK_OBJ.transform.position = new Vector3(34.6F, 2.1F, 12.7F);
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public GameObject imageObj;

    public void imageObj_OnHover()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter!" + other.name);
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        print("OnTriggerExit!" + other.name);
    }

    [ContextMenu("FlashingOn")]
    public void Flashing()
    {
        FlashingOn(Color.red);
    }

    public void FlashingOn(Color color)
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.FlashingOn(new Color(color.r, color.g, color.b, 0), new Color(color.r, color.g, color.b, 1));
    }

    [ContextMenu("FlashingOff")]
    public void FlashingOff()
    {
        Highlighter h = gameObject.AddMissingComponent<Highlighter>();
        h.FlashingOff();
    }
}
