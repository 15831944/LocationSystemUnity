using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcManager : MonoBehaviour {

    public static ArcManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public ArcReactor_Arc ArcLine;

    public ArcReactor_Trail Trail;

    public Vector3 Offset= new Vector3(0, 0.1f, 0);

    // Use this for initialization
    void Start ()
    {
        ArcLine.Offset = Offset;
        Trail.Offset = Offset;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnableTrail(Transform parent)
    {
        if (Trail)
        {
            Trail.transform.parent = parent;
            Trail.transform.localPosition = Vector3.zero;
            Trail.gameObject.SetActive(true);
        }
    }

    public void DisenableTrail()
    {
        if (Trail)
        {
            Trail.transform.parent = null;
            Trail.transform.localPosition = Vector3.zero;
            Trail.gameObject.SetActive(false);
        }
    }
}
