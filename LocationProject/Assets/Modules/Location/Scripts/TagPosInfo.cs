
using Location.WCFServiceReferences.LocationServices;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实时定位相关位置信息
/// </summary>
public class TagPosInfo: PosInfo
{
    public TagPosition TagPos;
   

    public TagPosInfo()
    {

    }

    public TagPosInfo(TagPosition tagPos)
    {
        TagPos = tagPos;
    }
}

public class PosInfo
{
    public Vector3 TargetPos;
    public Vector3 ShowPos;
    public Vector3 CurrentPos;
}