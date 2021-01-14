using Cyg.Resource.Enums;
using System;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 架空杆上设备
    /// </summary>
    public class OverheadEquipmentResponse
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
        /// 类型
        /// </summary>
        public OverheadEquipmentType Type { get; set; }

        /// <summary>
        /// 所属杆塔ID（主杆）
        /// </summary>
        public string Main_ID { get; set; }

        /// <summary>
        /// 所属杆塔ID（副杆）
        /// </summary>
        public string Sub_ID { get; set; }

        /// <summary>
        /// 容量
        /// </summary>
        public TransformerCapacity? Capacity { get; set; }

        /// <summary>
        /// 安装方法
        /// </summary>
        public OverheadEquipmentFixMode FixMode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SurveyState State { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat_WGS84 { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon_WGS84 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 勘测时间
        /// </summary>
        public DateTime SurveyTime { get; set; }
    }
}
