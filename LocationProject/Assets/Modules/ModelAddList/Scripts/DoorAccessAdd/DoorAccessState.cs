using System.Runtime.Serialization;
using System;
using Location.WCFServiceReferences.LocationServices;

namespace TModel.Location.Data
{
    [DataContract][Serializable]
    public class DoorAccessState
    {
        /// <summary>
        /// 门禁卡ID
        /// </summary>
        [DataMember]
        public string Abutment_CardId { get; set; }

        /// <summary>
        /// 门禁卡ID (开/关)
        /// </summary>
        [DataMember]
        public string Abutment_CardState { get; set; }

        /// <summary>
        /// 3D场景中，门的Id
        /// </summary>
        [DataMember]
        public string DoorId { get; set; } 

        [DataMember]
        public DevInfo Dev { get; set; }

        /// <summary>
        /// 设置门禁的DevInfo
        /// </summary>
        /// <param name="dev"></param>
        public void SetDev(DevInfo dev)
        {
            Dev = dev;
        }
    }
}
