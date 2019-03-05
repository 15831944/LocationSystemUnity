using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTEditor;
public class ModelAddEventListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
        bindGizmosEvent();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void bindGizmosEvent()
    {
        EditorGizmoSystem GizmoSystem = EditorGizmoSystem.Instance;
        if(GizmoSystem==null)
        {
            Debug.LogError("EditorGizmoSystem.Instance is null!");
            return;
        }
        //移动编辑事件
        GizmoSystem.TranslationGizmo.GizmoDragStart += TranslationGizmosStart;
        GizmoSystem.TranslationGizmo.GizmoDragUpdate += TranslationGizmosUpdate;
        GizmoSystem.TranslationGizmo.GizmoDragEnd += TranslationGizmosEnd;
        //旋转编辑事件
        GizmoSystem.RotationGizmo.GizmoDragStart += RotationGizmosStart;
        GizmoSystem.RotationGizmo.GizmoDragUpdate += RotationGizmosUpdate;
        GizmoSystem.RotationGizmo.GizmoDragEnd += RotationGizmosEnd;
        //缩放编辑事件
        GizmoSystem.ScaleGizmo.GizmoDragStart += ScaleGizmosStart;
        GizmoSystem.ScaleGizmo.GizmoDragUpdate += ScaleGizmosUpdate;
        GizmoSystem.ScaleGizmo.GizmoDragEnd += ScaleGizmosEnd;

        EditorObjectSelection.Instance.SelectionChanged += OnEditObjectSelectionChange;


    }
    public void OnEditObjectSelectionChange(ObjectSelectionChangedEventArgs selectionChangedEventArgs)
    {
        List<GameObject> SelectObjList = selectionChangedEventArgs.SelectedObjects;
        foreach(var item in SelectObjList)
        {
            Debug.Log("Select Obj:"+item.name);
        }
        List<GameObject> DeSelectObjList = selectionChangedEventArgs.DeselectedObjects;
        foreach(var item in DeSelectObjList)
        {
            Debug.Log("DeSelect Obj:" + item.name);
        }
    }
    private void TranslationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    private void TranslationGizmosUpdate(Gizmo gizmo)
    {

    }
    private void TranslationGizmosEnd(Gizmo gizmo)
    {
        foreach(var item in gizmo.ControlledObjects)
        {
            Debug.Log(item.name);
        }
    }
    private void RotationGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    private void RotationGizmosUpdate(Gizmo gizmo)
    {

    }
    private void RotationGizmosEnd(Gizmo gizmo)
    {
        foreach (var item in gizmo.ControlledObjects)
        {
            Debug.Log(item.name);
        }
    }
    private void ScaleGizmosStart(Gizmo gizmo)
    {
        Debug.Log(gizmo);
    }
    private void ScaleGizmosUpdate(Gizmo gizmo)
    {

    }
    private void ScaleGizmosEnd(Gizmo gizmo)
    {
        foreach (var item in gizmo.ControlledObjects)
        {
            Debug.Log(item.name);
        }
    }
}
