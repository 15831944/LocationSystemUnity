using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;
using UIWidgetsSamples;

public class ArcList : MonoBehaviour
{

    public GameObject[] Arcs;

    public GameObject CurrentArc;

    public ListView ListView;

    public Transform[] Transforms;

	// Use this for initialization
	void Start ()
	{
        ListView = gameObject.GetComponent<ListView>();
	    if (ListView)
	    {
            ListView.Clear();
            foreach (GameObject arc in Arcs)
            {
                //ListViewStringComponent item = new ListViewStringComponent();
                //item.Text.text = arc.name;
                ListView.Add(arc.name);
            }
            ListView.OnSelect.AddListener((index, item) =>
            {
                string arcName=ListView.DataSource[index];
                if (CurrentArc)
                {
                    GameObject.DestroyImmediate(CurrentArc);
                }
                
                GameObject arcObj = GetArc(arcName);
                if (arcObj == null) return;
                CurrentArc =GameObject.Instantiate(arcObj);

                ArcReactor_Arc arc = CurrentArc.GetComponent<ArcReactor_Arc>();
                arc.shapeTransforms = Transforms;
                arc.playbackType=ArcReactor_Arc.ArcsPlaybackType.loop;
            });
        }
    }

    private GameObject GetArc(string arcName)
    {
        foreach (GameObject arc in Arcs)
        {
            if (arc.name == arcName) return arc;
        }
        return null;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
