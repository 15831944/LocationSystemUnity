using System.Collections;
using System.Collections.Generic;
using Location.WCFServiceReferences.LocationServices;
using MonitorRange;
using UnityEngine;

public class NavMeshAreaTest : MonoBehaviour
{

    public GameObject Parent;

    public GameObject Prefab;

    [ContextMenu("ShowSwitchAreas")]
    public void ShowSwitchAreas()
    {
        if (Parent == null)
        {
            Parent=new GameObject("SwitchAreas");
        }



        CommunicationObject.Instance.GetSwitchAreas(areas =>
        {
            if (areas == null)
            {
                Log.Error("NavMeshAreaTest.ShowSwitchAreas", "areas == null");
            }
            else
            {
                GameObject tmp = MonitorRangeManager.Instance.areaPrefab;
                if (Prefab != null)
                {
                    MonitorRangeManager.Instance.areaPrefab = Prefab;
                }

                foreach (PhysicalTopology area in areas)
                {
                    var obj=MonitorRangeManager.Instance.CreateRange(area, null);
                    obj.transform.parent = Parent.transform;
                }

                MonitorRangeManager.Instance.areaPrefab = tmp;
            }
        });
    }

    
}
