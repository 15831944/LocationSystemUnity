using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Base.Common;


[XmlType(TypeName = "SystemSetting")]
public class SystemSetting
{
    [XmlElement]
    public bool IsDebug;
    /// <summary>
    /// 是否开启异步通信
    /// </summary>
    [XmlElement]
    public bool IsAsync;
    /// <summary>
    /// 是否关闭通信
    /// </summary>
    [XmlElement]
    public bool IsCloseCommunication;
    [XmlElement]
    public CinemachineSetting CinemachineSetting;
    [XmlElement]
    public CommunicationSetting CommunicationSetting;
    [XmlElement]
    public VersionSetting VersionSetting;
    [XmlElement]
    public RefreshSetting RefreshSetting;

    public SystemSetting()
    {
        CinemachineSetting = new CinemachineSetting();
        CommunicationSetting = new CommunicationSetting();
        VersionSetting = new VersionSetting();
        RefreshSetting = new RefreshSetting();
    }
}

