using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;

namespace Cyg.Applicatio.Survey.Dto
{
    public class CableResponse
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 上一节点
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 上一节点类型
        /// </summary>
        public NodeType PreNodeType { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat_WGS84 { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon_WGS84 { get; set; }

        /// <summary>
        /// 电缆井类型
        /// </summary>
        public CableType C_Type { get; set; }

        /// <summary>
        /// 电缆井型号
        /// </summary>
        public string C_ModeID { get; set; }

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
