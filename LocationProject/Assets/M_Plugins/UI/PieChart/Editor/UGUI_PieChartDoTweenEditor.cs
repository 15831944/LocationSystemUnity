using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UGUI_PieChartDoTween), true)]
public class UGUI_PieChartDoTweenEditor : Editor
{
    UGUI_PieChartDoTween pieChart;
    SerializedProperty type;
    SerializedProperty cicle;
    SerializedProperty duration;
    SerializedProperty colors;
    SerializedProperty sprites;
    SerializedProperty values;

    void OnEnable()
    {
        pieChart = (UGUI_PieChartDoTween)target;
    }

    public override void OnInspectorGUI()
    {
        type = serializedObject.FindProperty("type");
        cicle = serializedObject.FindProperty("cicle");
        duration = serializedObject.FindProperty("duration");
        colors = serializedObject.FindProperty("colors");
        sprites = serializedObject.FindProperty("sprites");
        values = serializedObject.FindProperty("values");

        serializedObject.Update();

        EditorGUILayout.PropertyField(cicle);
        EditorGUILayout.PropertyField(duration);

        EditorGUILayout.PropertyField(type);
        //EditorGUILayout.EnumPopup("Type", menu.type);
        UGUI_PieChartDoTween.Type typeValue = (UGUI_PieChartDoTween.Type)type.enumValueIndex;
        //menu.type = typeValue;
        EditorGUI.indentLevel++;//可以使前面空两行
        if (typeValue == UGUI_PieChartDoTween.Type.Color)
        {
            EditorGUILayout.PropertyField(colors, true);
        }
        else if (typeValue == UGUI_PieChartDoTween.Type.Sprite)
        {
            EditorGUILayout.PropertyField(sprites, true);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.PropertyField(values, true);

        serializedObject.ApplyModifiedProperties();

    }

}
