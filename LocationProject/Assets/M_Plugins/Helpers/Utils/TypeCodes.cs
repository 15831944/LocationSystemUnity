using System;
using System.Reflection;
using Base.Common;
//using DataObjects.Others;

namespace DataObjects.Utils
{
    /// <summary>
    /// 每一项对应数据库NameToType的名字（NameToType中的名字和这里的名字要一样）
    /// </summary>
    public static class TypeCodes
    {
        /// <summary>
        /// 某种端口
        /// </summary>
        public static string OUTLET = "500002";

        /// <summary>
        /// 艾默生空调 78->3000002
        /// </summary>
        public static string AirConditionerEmerson = "3000002";

        /// <summary>
        /// 台达空调 77->3000010
        /// </summary>
        public static string AirConditionerDelta = "3000010";

        /// <summary>
        /// 依米康空调 3000042
        /// </summary>
        public static string AirConditionerYiMiKang = "3000042";
        
        /// <summary>
        /// 微模块空调 3000081
        /// </summary>
        public static string AirConditionerMicroModule = "3000081";

        /// <summary>
        /// 台达UPS 76->3000001
        /// </summary>
        public static string UPSDelta = "3000001";

        /// <summary>
        /// 台达主配电柜 72->3000006
        /// </summary>
        public static string MainPowerDistributionCabinetDelta = "3000006";

        /// <summary>
        /// 单兵系统
        /// </summary>
        public static string SingleMonitorDev = "3000282";

        /// <summary>
        ///APC_UPC 3000014
        /// </summary>
        public static string APC_Smart_UPS = "3000014";

        /// <summary>
        /// 山特UPS 79->3000000
        /// </summary>
        public static string UPSSantak = "3000000";

        /// <summary>
        /// 科隆UPS 3000086
        /// </summary>
        public static string UPSKelong = "3000086";

        /// <summary>
        /// 台达列头柜 71->3000007
        /// </summary>
        public static string ColumnHeadCabinetDelta = "3000007";

        /// <summary>
        /// 警铃 1000110
        /// </summary>
        public static string AlarmBell = "1000110";

        /// <summary>
        /// 摄像头 1000102
        /// </summary>
        public static string Camera = "1000102";

        /// <summary>
        /// 摄像头 53
        /// </summary>
        public static string IPCameraHk = "53";
        /// <summary>
        /// 摄像头 3000610
        /// </summary>
        public static string CameraHk = "3000610";
        /// <summary>
        /// 海康NVR
        /// </summary>
        public static string HkNVR = "53";
        /// <summary> 
        /// 门禁系统控制器　20 
        /// </summary> 
        public static string DoorControl = "20";

        /// <summary> 
        /// 海康KVM 22 
        /// </summary> 
        public static string HK_KVM = "22";

        /// <summary> 
        /// 动环设备　35
        /// </summary> 
        public static string PE_DEV = "35";

        /// <summary>
        /// 大华8路模拟DVR 13
        /// </summary>
        public static string SimulateDVR8 = "13";

        /// <summary>
        /// 海康DVR
        /// </summary>
        public static string HkDVR = "51";

        /// <summary>
        /// 海康DVR2
        /// </summary>
        public static string HkDVR2 = "52";
        /// <summary>
        /// 海康DVR3
        /// </summary>
        public static string HkDVR3 = "3000609";

        /// <summary>
        /// 大华IP摄像头 14
        /// </summary>
        public static string IpCamera = "14";
        /// <summary>
        /// 初灵*微模块1拖4设备 
        /// </summary>
        public static string MicroModule1to4 = "15";
        /// <summary>
        /// 微模块2拖4设备
        /// </summary>
        public static string MicroModule2to4 = "16";
        /// <summary>
        /// 交流电监控 75->3000005
        /// </summary>
        public static string ACMonitor = "3000005";

        /// <summary>
        /// 直流电监控 68->3000015
        /// </summary>
        public static string DCMonitor = "3000015";

        /// <summary>
        /// 初灵门磁传感器 1000104
        /// </summary>
        public static string DoorSensor = "1000104";

        /// <summary>
        /// 水浸传感器
        /// </summary>
        public static string WaterSensor = "1000105";


        /// <summary>
        /// 初灵*漏水检测(线式)
        /// </summary>
        public static string LineLeakageDetection = "3000182";

        /// <summary>
        /// 初灵烟雾探测器
        /// </summary>
        public static string SmokeSensor = "1000106";

        /// <summary>
        /// 红外线传感器
        /// </summary>
        public static string InfraredSensor = "1000108";

        /// <summary>
        /// 门禁
        /// </summary>
        public static string DoorGuard = "1000109";

        //public static string GSM-SM_Modem = "";

        /// <summary>
        /// 初灵温湿度传感器 1000103
        /// </summary>
        public static string TemperatureHumiditySensor = "3000103";

        /// <summary>
        /// 初灵液显温湿度传感器 1000107
        /// </summary>
        public static string TemperatureHumiditySensor2 = "3000107";

        /// <summary>
        /// 初灵液显温湿度传感器 3000283 
        /// </summary>
        public static string TemperatureHumiditySensor3 = "3000283";

        /// <summary>
        /// 初灵液显温湿度传感器 3000283 
        /// </summary>
        public static string TemperatureHumiditySensor4 = "3000237";

        /// <summary>
        /// 0U电源条 1
        /// </summary>
        public static string PowerBar = "1";

        /// <summary>
        /// 带空开16口欧标竖型电源条 63->1000018->3000012
        /// </summary>
        public static string PowerBar_Vertical = "3000012";

        /// <summary>
        /// 竖型电源条
        /// </summary>
        public static string VerticalPowerBar = "1\\3000012\\3000630";
        

        /// <summary>
        /// 核心设备监控
        /// </summary>
        public static string CentralDevice = "3000039";

        /// <summary>
        /// 华三24口3600交换机
        /// </summary>
        public static string SwitchView = "3000039";

        /// <summary>
        /// 民用空调遥控器->3000108
        /// </summary>
        public static string Airconditioner_Controller = "3000108";

        /// <summary>
        /// Windows监控
        /// </summary>
        public static string WinMonitor = "3000137";//3000027//3000025

        /// <summary>
        /// 虚拟机Windows监控
        /// </summary>
        public static string VirtualWinMonitor = "3000612";
        /// <summary>
        /// 虚拟机Linux监控
        /// </summary>
        public static string VirtualLinuxMonitor = "3000611";
        /// <summary>
        /// Linux监控
        /// </summary>
        public static string LinuxMonitor = "3000138";

        /// <summary>
        /// 数据库监控
        /// </summary>
        public static string SqlMonitor = "3000139";//3000027


        /// <summary>
        /// 以太网开关
        /// </summary>
        public static string InternetSwitch = "3000238";

        /// <summary>
        /// BMC设备监控
        /// </summary>
        public static string BMCView = "30000207";
        /// <summary>
        /// BMC设备设备远程开关机
        /// </summary>
        public static string BMCRemoteOpt = "3000208"; 
        /// <summary>
        /// 施耐德电源
        /// </summary>
        public static string PowerDistribution = "3000239";

        /// <summary>
        /// 海洛斯空调
        /// </summary>
        public static string HIROSS = "3000286";

        /// <summary>
        /// IBM BMC监控界
        /// </summary>
        public static string IBMBMCView = "3000209";

        /// <summary>
        /// 施耐德UPS
        /// </summary>
        public static string SEUPS = "3000285";

        /// <summary>
        /// 施耐德*TUAV1822-Cooling
        /// </summary>
        //public static string OpenSECooling = "3000284";
           public static string OpenSECooling = "XXXXXXX";    //因为版本不同原因暂时将其typeCode置空，实际值见上面的注释语句
        /// <summary>
        /// 计通电量仪
        /// </summary>
        public static string JT_EM_VIEW = "3000302";

        /// <summary>
        /// 计通空调
        /// </summary>
        public static string JT_KT_VIEW = "3000303";

        /// <summary>
        /// 计通_伊顿UG
        /// </summary>
        public static string JT_UG_UPS_VIEW = "3000304";

        /// <summary>
        /// 低压配电柜
        /// </summary>
        public static string DistributionBox = "3000009";

        /// <summary>
        /// 基站电源
        /// </summary>
        public static string BaseStationPower = "3000177";

        /// <summary>
        /// 基础电池
        /// </summary>
        public static string Battery = "3000008";

        /// <summary>
        /// 智能电表
        /// </summary>
        public static string SmartElectricMeter = "3000326";

        /// <summary>
        /// 大屏*交换机'   
        /// </summary>
        public static string BigScreenSwitch = "";

        /// <summary>
        /// 大屏*服务器'   
        /// </summary>
        public static string BigScreenServer = "";

        /// <summary>
        /// 大屏*UPS'   
        /// </summary>
        public static string BigScreenUPS = "";

        #region 用于逻辑拓扑
        /// <summary>
        /// 核心路有器  
        /// </summary>
        public static string CoreRouter = "";
        /// <summary>
        /// 核心交换机  
        /// </summary>
        public static string CoreSwitch = "";
        /// <summary>
        /// 网络安全设备
        /// </summary>
        public static string NetSaveDev = "";
        /// <summary>
        /// 存储设备  
        /// </summary>
        public static string StorageDev = "";
        /// <summary>
        /// 接入交换机  
        /// </summary>
        public static string AccessSwitch = "";
        /// <summary>
        /// 服务器  
        /// </summary>
        public static string NormalService = "";
        #endregion
        /// <summary>
        /// 艾默生UPS
        /// </summary>
        public static string AmsUPS = "3000284";

        /// <summary>
        /// 艾默生UPS
        /// </summary>
        public static string CMCC_UPS = "3000399";
        
        /// <summary>
        /// 博尔低压配电
        /// </summary>
        public static string BoerLowValtage = "3000293";

        /// <summary>
        /// 施耐德G5500UPS
        /// </summary>
        public static string APCG5500 = "3000459";

        /// <summary>
        /// 杭州巨远记过分录电源3000
        /// </summary>
        //public static string JY3000 = "3000506";
        public static string JY3000 = "XXXXXXX";//因为版本不同原因暂时将其typeCode置空，实际值见上面的注释语句
        /// <summary>
        /// 浙江三辰电池组
        /// </summary>
        public static string SanChenBattery = "3000461";

        /// <summary>
        /// SN-E-9S4Y-2016液晶多功能电力仪表
        /// </summary>
        public static string SN_E_9S4Y="3000462";

        /// <summary>
        /// SStulz   
        /// </summary>
        public static string Stulz_C7000 = "3000463";

        /// <summary>
        /// 移动对接爱默生UPS   
        /// </summary>
        public static string Thrid_CMCC_Emerson_UPS = "3000284";

        /// <summary>
        /// 移动对接南都蓄电池
        /// </summary>
        public static string Thrid_CMCC_Nandu_UPSBattery = "3000507";

        /// <summary>
        /// 艾默生开关电源
        /// </summary>
        public static string Thrid_CMCC_Emerson_SwitchModePowerSupply = "3000290";

        /// <summary>
        /// 移动对接威尔逊发电机组 
        /// </summary>
        public static string Thrid_CMCC_Wilson_GeneratingSet = "3000508";



        /// <summary>
        /// 辛普森发电组
        /// </summary>
        public static string Simpson_GeneratingSet = "XXXXXXX";

        /// <summary>
        /// 移动对接艾默生专用空调
        /// </summary>
        public static string Thrid_CMCC_Emerson_P1020Cool = "3000285";



        static Type type = typeof(TypeCodes);

        //public static NameToTypeList List;

        //public static void SetValues(NameToTypeList list)
        //{
        //    try
        //    {
        //        NameToTypeList temp = new NameToTypeList();
        //        List = list;
        //        if (list != null)
        //        {
        //            foreach (NameToType item in list)
        //            {
        //                bool r = SetValue(item.Name, item.TypeCode);
        //                if (r == false)
        //                {
        //                    temp.Add(item);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Log.Error("TypeCodes.SetValues", "list == null");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("InitTypeCodes", ex);
        //    }
        //}

        public static bool SetValue(string key, string value)
        {
            try
            {
                BindingFlags flag = BindingFlags.Static | BindingFlags.Public;
                FieldInfo field = type.GetField(key, flag);
                if (field == null)
                {
                    return false;
                }
                //string value = field.GetValue(null) + "";
                //if (value != item.TypeCode)
                //{
                //    //数据库中数据发送了变化
                //    field.SetValue(null,item.TypeCode);
                //}
                field.SetValue(null, value);
            }
            catch (Exception ex)
            {
                Log.Error("SetValue", ex);
                return false;
            }
            return true;
        }
    }
}
