using UnityEngine;
using System.Collections;

public static class ArcHelper  {
    public static GameObject CreateEmptyObject(Vector3 p1, string n,Transform transform)
    {
        GameObject go1 = new GameObject(n);
        go1.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        go1.transform.position = p1;
        go1.transform.parent = transform;
        return go1;
    }

    public static GameObject CreateArcLine(GameObject arcPrefab, GameObject obj1, GameObject obj2, Camera camera,float size)
    {
        if (arcPrefab == null) return null;
        GameObject arcObj = GameObject.Instantiate(arcPrefab);
        ArcReactor_Arc arc = arcObj.GetComponent<ArcReactor_Arc>();
        arc.lifetime = 2;
        arc.playbackType = ArcReactor_Arc.ArcsPlaybackType.loop;
        arc.shapeTransforms = new Transform[] { obj1.transform, obj2.transform };
        arc.camera = camera;
        arc.sizeMultiplier = size;
        return arcObj;
    }
}
