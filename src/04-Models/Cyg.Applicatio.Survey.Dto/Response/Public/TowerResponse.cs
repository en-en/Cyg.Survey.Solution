using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 杆塔
    /// </summary>
    public class TowerResponse
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
        /// 杆型
        /// </summary>
        public string T_Type { get; set; }

        /// <summary>
        /// 杆规格(杆高*杆梢径)
        /// </summary>
        public string Specification
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(T_Height) && !string.IsNullOrWhiteSpace(T_Rod))
                {
                    return $"{T_Rod}*{T_Height}";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 杆高
        /// </summary>
        public string T_Height { get; set; }

        /// <summary>
        /// 杆梢径
        /// </summary>
        public string T_Rod { get; set; }

        /// <summary>
        /// 分段方式
        /// </summary>
        public SegmentMode T_Segment { get; set; }

        /// <summary>
        /// 排列方式
        /// </summary>
        public Arrangement T_Sort { get; set; }

        /// <summary>
        /// 电压等级
        /// </summary>
        public KVLevel KVLevel { get; set; }

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

        /// <summary>
        /// 杆型
        /// </summary>
        public string PoleTypeCode { get; set; }

    }
}
