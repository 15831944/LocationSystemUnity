using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : MonoBehaviour {

    public List<LogTagEnable> Tags = new List<LogTagEnable>();

    // Use this for initialization
    void Start()
    {
        SetTagFilter();
    }

    private void SetTagFilter()
    {
        if (Tags.Count > 0)
        {
            Log.TagFilter = new Dictionary<string, bool>();
            foreach (var item in Tags)
            {
                if (Log.TagFilter.ContainsKey(item.tag))
                {
                    Log.TagFilter[item.tag] = item.enabled;
                }
                else
                {
                    Log.TagFilter.Add(item.tag, item.enabled);
                }
            }
        }
        else
        {
            Log.TagFilter = null;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        SetTagFilter();
    }
}

public class LogTagEnable
{
    public string tag;
    public bool enabled;
}
