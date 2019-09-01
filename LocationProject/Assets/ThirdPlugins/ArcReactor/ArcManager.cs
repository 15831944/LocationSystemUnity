using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcManager : MonoBehaviour {

    public static ArcManager Instance;

    public bool Enabled = false;

    void Awake()
    {
        Instance = this;
    }

    public ArcReactor_Arc ArcLine;

    public ArcReactor_Trail Trail;

    public Vector3 Offset= new Vector3(0, 0.1f, 0);

    public Dictionary<string, ArcReactor_Arc> Lines = new Dictionary<string, ArcReactor_Arc>();

    public Dictionary<string, ArcReactor_Trail> Trails = new Dictionary<string, ArcReactor_Trail>();

    public void SetLine(Transform t1,Transform t2)
    {
        if (Enabled == false) return;
        string lname = t1.name + t2.name;
        if (!Lines.ContainsKey(lname) || Lines[lname] == null)
        {
            ArcReactor_Arc newLine = GameObject.Instantiate<ArcReactor_Arc>(ArcLine);
            Lines.Add(lname, newLine);
            Debug.Log(string.Format("ArcManager.AddLine:{0},{1}->{2}", lname,t1.position,t2.position));
        }

        ArcReactor_Arc line = Lines[lname];
        line.SetLine(t1, t2);
    }

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
        if (Enabled == false) return;
        if (Trail)
        {
            //Trail.transform.parent = parent;
            //Trail.transform.localPosition = Vector3.zero;
            //Trail.gameObject.SetActive(true);

            string lname = parent.name;
            if (!Trails.ContainsKey(lname) || Trails[lname]==null)
            {
                ArcReactor_Trail newTrail = GameObject.Instantiate<ArcReactor_Trail>(Trail);
                Trails.Add(lname, newTrail);

                Debug.Log(string.Format("ArcManager.AddTrail:{0},{1}", lname,parent.position));
            }

            ArcReactor_Trail trail = Trails[lname];
            trail.transform.parent = parent;
            trail.transform.localPosition = Vector3.zero;
            trail.gameObject.SetActive(true);
        }
    }

    public void DisenableTrail()
    {
        //if (Trail)
        //{
        //    Trail.transform.parent = null;
        //    Trail.transform.localPosition = Vector3.zero;
        //    Trail.gameObject.SetActive(false);
        //}

        foreach (var item in Trails.Values)
        {
            if (item == null) continue;
            item.transform.parent = null;
            item.transform.localPosition = Vector3.zero;
            item.gameObject.SetActive(false);
        }
    }
}
