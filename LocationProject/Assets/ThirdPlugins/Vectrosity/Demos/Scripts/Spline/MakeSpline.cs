using UnityEngine;
using Vectrosity;
using System.Collections.Generic;

public class MakeSpline : MonoBehaviour
{

    public float width = 2.0f;
	public int segments = 250;
	public bool loop = true;
	//public bool usePoints = false;

    public LineType Type = LineType.Continuous;

    private VectorLine Line;

    public GameObject LineObject;

    void Start ()
	{
	    DrawLine();
	}

    [ContextMenu("DrawLine")]
    public void DrawLine()
    {
        var splinePoints = new List<Vector3>();
        var i = 1;
        var obj = GameObject.Find("Sphere" + (i++));
        while (obj != null)
        {
            splinePoints.Add(obj.transform.position);
            obj = GameObject.Find("Sphere" + (i++));
        }

        Line = new VectorLine("Spline", new List<Vector3>(segments + 1), width, Type);
        var set = segments;
        Line.MakeSpline(splinePoints.ToArray(), segments, loop);
        Line.Draw3DAuto();

        LineObject = Line.rectTransform.gameObject;
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        Line.Draw3DAuto();
    }

    [ContextMenu("SetWidth")]
    public void SetWidth()
    {
        Line.SetWidth(width);
    }

    [ContextMenu("StartDraw3DAuto")]
    public void StartDraw3DAuto()
    {
        Line.Draw3DAuto();
    }

    [ContextMenu("StopDrawing3DAuto")]
    public void StopDrawing3DAuto()
    {
        Line.StopDrawing3DAuto();
    }
}