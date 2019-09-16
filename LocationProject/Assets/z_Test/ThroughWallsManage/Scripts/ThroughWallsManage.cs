using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThroughWallsManage : MonoBehaviour {

    public static ThroughWallsManage Instance;
    public bool isCharacterControllerThroughWallsTest;//碰撞器检测穿墙测试
    public bool isNavMeshThroughWallsTest;//NavMesh穿墙测试

    public GameObject TestBuilding;//测试的建筑

    public GameObject manPrefabNormal;//一般人物预设
    public GameObject manPrefab_LOD_CharacterController;//碰撞器人物预设
    public GameObject manPrefabNavMesh;//NavMesh人物预设

    public LocationManager locationManager;

    // Use this for initialization
    void Start () {
        Instance = this;

        //if (isCharacterControllerThroughWallsTest)
        //{
        //    if(locationManager!=null)
        //        locationManager.characterPrefab = manPrefab_LOD_CharacterController;
        //}
        //else if (isNavMeshThroughWallsTest)
        //{
        //    if (locationManager != null)
        //        locationManager.characterPrefab = manPrefabNavMesh;
        //}

        meshcolliders = new List<MeshCollider>();
        boxColliders = new List<BoxCollider>();
        if (isCharacterControllerThroughWallsTest)
        {
            //SetAllMeshColliderTrue();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public List<MeshCollider> meshcolliders;
    public List<BoxCollider> boxColliders;

    /// <summary>
    /// 设置是否启用MeshCollider
    /// </summary>
    public void SetAllMeshCollider(bool isEnable)
    {
        if (!Application.isPlaying) return;
        if (TestBuilding == null) return;

        MeshCollider[] meshcollidersT = TestBuilding.GetComponentsInChildren<MeshCollider>(true);

        if (isEnable)
        {
            meshcolliders.Clear();
            foreach (MeshCollider meshcollider in meshcollidersT)
            {
                if (!meshcollider.enabled)
                {
                    meshcollider.enabled = isEnable;
                    meshcolliders.Add(meshcollider);
                }
            }
        }
        else
        {
            foreach (MeshCollider meshcollider in meshcolliders)
            {
                    meshcollider.enabled = isEnable;
            }
        }
    }

    [ContextMenu("SetAllMeshColliderTrue")]
    public void SetAllMeshColliderTrue()
    {
        SetAllMeshCollider(true);
        SetAllBoxColliderFalse();
    }


    [ContextMenu("SetAllMeshColliderFalse")]
    public void SetAllMeshColliderFalse()
    {
        SetAllMeshCollider(false);
        SetAllBoxColliderTrue();
    }

    /// <summary>
    /// 设置是否启用BoxCollider
    /// </summary>
    public void SetAllBoxCollider(bool isEnable)
    {
        if (!Application.isPlaying) return;
        if (TestBuilding == null) return;

        BoxCollider[] boxCollidersT = TestBuilding.GetComponentsInChildren<BoxCollider>(true);

        if (isEnable)
        {
            boxColliders.Clear();
            foreach (BoxCollider boxcollider in boxCollidersT)
            {
                if (!boxcollider.enabled)
                {
                    boxcollider.enabled = isEnable;
                    boxColliders.Add(boxcollider);
                }
            }
        }
        else
        {
            foreach (BoxCollider boxcollider in boxCollidersT)
            {
                boxcollider.enabled = isEnable;
            }
        }
    }

    //[ContextMenu("SetAllBoxColliderTrue")]
    public void SetAllBoxColliderTrue()
    {
        SetAllBoxCollider(true);
    }


    //[ContextMenu("SetAllBoxColliderFalse")]
    public void SetAllBoxColliderFalse()
    {
        SetAllBoxCollider(false);
    }

    ///// <summary>
    ///// 碰撞器检测穿墙测试
    ///// </summary>
    //[ContextMenu("SetColliderThroughWalls")]
    //public void SetColliderThroughWalls()
    //{
    //    isColliderThroughWallsTest = true;
    //    isNavMeshThroughWallsTest = false;
    //}

    ///// <summary>
    ///// NavMesh穿墙测试
    ///// </summary>
    //[ContextMenu("SetNavMeshThroughWallsc")]
    //public void SetNavMeshThroughWalls()
    //{
    //    isColliderThroughWallsTest = false;
    //    isNavMeshThroughWallsTest = true;
    //}

    ///// <summary>
    ///// 不考虑穿墙问题
    ///// </summary>
    //[ContextMenu("SetNormalThroughWalls")]
    //public void SetNormalThroughWalls()
    //{
    //    isColliderThroughWallsTest = false;
    //    isNavMeshThroughWallsTest = false;
    //}

    //private void SetThroughWall()
    //{
    //    if (isColliderThroughWallsTest)
    //    {
    //        locationManager.characterPrefab = manPrefabCollider;
    //    }
    //    else if (isNavMeshThroughWallsTest)
    //    {
    //        locationManager.characterPrefab = manPrefabNavMesh;
    //    }
    //    else
    //    {
    //        locationManager.characterPrefab = manPrefabNormal;
    //    }
    //}
}
