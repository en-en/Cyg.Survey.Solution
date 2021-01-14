using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 电缆杆上设备
    /// </summary>
    public class CableEquipmentResponse
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public CableEquipmentType Type { get; set; }

        /// <summary>
        /// 上一节点ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 上一节点类型
        /// </summary>
        public NodeType PreNodeType { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string EquipModel { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat_WGS84 { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon_WGS84 { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SurveyState State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 勘测时间
        /// </summary>
        public DateTime SurveyTime { get; set; }

        /// <summary>
        ///  带电作业
        /// </summary>
        public bool ElectrifiedWork { get; set; }
    }
}
