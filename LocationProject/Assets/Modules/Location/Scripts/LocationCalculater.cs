using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCalculater : MonoBehaviour
{

    /// <summary>
    /// 实际尺寸/三维尺寸
    /// </summary>
    public Vector3 scale;
    /// <summary>
    /// 坐标原点
    /// </summary>
    public Vector3 axisZero;
    /// <summary>
    /// 方向校准
    /// </summary>
    public Vector3 direction = Vector3.one;

    /// <summary>
    /// 坐标原点物体
    /// </summary>
    public Transform axisZeroObj;

    public LocationManager locationManager;

    /// <summary>
    /// 基准点列表
    /// </summary>
    public List<LocationCalculaterItem> list;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 实际尺寸/三维尺寸的计算,以及原点
    /// </summary>
    [ContextMenu("Calculate")]
    public void Calculate()
    {
        direction = new Vector3(direction.x >= 0 ? 1 : -1, direction.y >= 0 ? 1 : -1, direction.z >= 0 ? 1 : -1);

        List<LocationCalculaterItem> listT = new List<LocationCalculaterItem>();
        foreach (LocationCalculaterItem item in list)
        {
            item.Calculate(direction);
            if (item.ISSuccess)
            {
                listT.Add(item);
            }
        }

        if (listT.Count < 2)
        {
            Debug.LogError("有效基准点数量小于2");
            return;
        }

        scale = CalculateScale(listT);
        axisZero = CalculateAxisZero(listT);

        locationManager.LocationOffsetScale = scale;
        locationManager.axisZero = axisZero;
        locationManager.direction = direction;

        axisZeroObj.position = axisZero;
    }


    /// <summary>
    /// 坐标原点的计算
    /// </summary>
    public Vector3 CalculateAxisZero(List<LocationCalculaterItem> listT)
    {
        int n = 0;
        Vector3 vNum = Vector3.zero;
        foreach (LocationCalculaterItem item in listT)
        {
            Vector3 trans = new Vector3(item._B / scale.x, 0, item._A / scale.z);
            Vector3 r = item.targetObj.transform.position - trans;
            vNum += r;
            n++;
            Debug.LogError("点" + (n) + "计算原点：" + r);
        }

        Vector3 result = Vector3.zero;
        if (n > 0)
        {
            result = vNum / n;
        }
        else
        {
            result = Vector3.zero;
        }
        return result;
    }

    /// <summary>
    /// 实际尺寸/三维尺寸的计算
    /// </summary>
    public Vector3 CalculateScale(List<LocationCalculaterItem> listT)
    {

        int n = 0;
        Vector3 scaleNum = Vector3.zero;
        Vector3 result = Vector3.zero;
        //foreach (LocationCalculaterItem item in listT)
        //{
        //    if (item.ISSuccess)
        //    {
        //        n++;
        //        scaleT += item.scale;
        //    }
        //}

        for (int i = 0; i < listT.Count; i++)
        {
            if (i + 1 < listT.Count)
            {
                if (listT[i].A == listT[i + 1].A && listT[i].B == listT[i + 1].B) continue;//相同点不参与计算

                Vector3 r = CalculateTwoItem(listT[i], listT[i + 1]);
                scaleNum += r;
                Debug.LogError("" + listT[i].Name + "->" + listT[i + 1].Name + "计算比例结果:" + r);
                n++;
                continue;
            }

            if (listT.Count >= 2 && i == listT.Count - 1)//最后一个基准点与第一个计算
            {
                if (listT[i].A == listT[0].A && listT[i].B == listT[0].B) continue;//相同点不参与计算

                Vector3 r= CalculateTwoItem(listT[0], listT[i]);
                scaleNum += r;
                Debug.LogError("" + listT[0].Name + "->" + listT[i].Name + "计算比例结果:" + r);
                n++;
                continue;
            }
        }

        if (n > 0)
        {
            result = scaleNum / n;
        }
        else
        {
            result = Vector3.zero;
        }
        return result;
    }

    private Vector3 CalculateTwoItem(LocationCalculaterItem item1, LocationCalculaterItem item2)
    {
        float BDIs = item1._B - item2._B;//对应X
        float CDis = 0;//对应Y
        float ADis = item1._A - item2._A; //对应Z

        bool isC = true;
        if (item1._C != 0 && item2._C != 0)
        {
            CDis = item1._C - item2._C;
            isC = true;
        }
        else
        {
            CDis = 0;
            isC = false;
        }
        BDIs = Math.Abs(BDIs);
        CDis = Math.Abs(CDis);
        ADis = Math.Abs(ADis);


        float XDis_3d = item1.targetObj.transform.position.x - item2.targetObj.transform.position.x;
        float YDIs_3d = 0;
        float ZDis_3d = item1.targetObj.transform.position.z - item2.targetObj.transform.position.z;
        if (isC)
        {
            YDIs_3d = item1.targetObj.transform.position.y - item2.targetObj.transform.position.y;
        }

        XDis_3d = Math.Abs(XDis_3d);
        YDIs_3d = Math.Abs(YDIs_3d);
        ZDis_3d = Math.Abs(ZDis_3d);

        Vector3 result = Vector3.zero;

        float xtemp = BDIs / XDis_3d;
        float ztemp = ADis / ZDis_3d;

        xtemp = xtemp == 0 ? ztemp : xtemp;
        ztemp = ztemp == 0 ? xtemp : ztemp;

        if (isC)
        {
            result = new Vector3(xtemp, CDis / YDIs_3d, ztemp);
        }
        else
        {
            result = new Vector3(xtemp, 0, ztemp);
        }

        return result;
    }
}

[Serializable]
public class LocationCalculaterItem
{
    public string Name;
    public GameObject targetObj;
    /// <summary>
    /// 三维中Z方向，实际的南北方向
    /// </summary>
    public float A = 0;//三维中Z方向，南北方向
    /// <summary>
    /// 三维中X方向，实际的东西方向
    /// </summary>
    public float B = 0;//三维中X方向，东西方向
    /// <summary>
    /// 三维中y方向，高度方向
    /// </summary>
    public float C = 0;//三维中y方向，高度方向

    /// <summary>
    /// 三维中Z方向，实际的南北方向,用于计算
    /// </summary>
    [HideInInspector]
    public float _A = 0;//三维中Z方向，南北方向
    /// <summary>
    /// 三维中X方向，实际的东西方向,用于计算
    /// </summary>
    [HideInInspector]
    public float _B = 0;//三维中X方向，东西方向
    /// <summary>
    /// 三维中y方向，高度方向,用于计算
    /// </summary>
    [HideInInspector]
    public float _C = 0;//三维中y方向，高度方向

    ///// <summary>
    ///// 实际尺寸/三维尺寸
    ///// </summary>
    //public Vector3 scale;
    /// <summary>
    /// 是否可以参与计算
    /// </summary>
    public bool iSSuccess = false;

    public bool ISSuccess
    {
        get
        {
            //Calculate();
            return iSSuccess;
        }

        set
        {
            iSSuccess = value;
        }
    }

    /// <summary>
    /// 实际尺寸/三维尺寸的计算
    /// </summary>
    public void Calculate(Vector3 dir)
    {
        //if (targetObj == null || A == 0 || B == 0)
        if (targetObj == null)
        {
            //Debug.LogError("(targetObj == null || A == 0 || B == 0)");
            Debug.LogError("(targetObj == null)");
            iSSuccess = false;
            return;
        }

        //这里由于现实场景跟三维模型的角度不同
        SetDirection(dir);
        //_A = -A; 
        // _B = -B;
        //_C = C;

        iSSuccess = true;
    }

    /// <summary>
    /// 方向校准,这里由于现实场景跟三维模型的角度不同
    /// </summary>
    /// <param name="v"></param>
    public void SetDirection(Vector3 v)
    {
        _A = v.z * A;
        _B = v.x * B;
        _C = v.y * C;
    }
}
