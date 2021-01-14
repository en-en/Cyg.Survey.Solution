using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;

namespace Cyg.Applicatio.Survey.Dto
{
    public class CableChannelResponse
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
        /// 始节点
        /// </summary>
        public string Start_Id { get; set; }

        /// <summary>
        /// 始节点类型
        /// </summary>
        public NodeType StartNodeType { get; set; }

        /// <summary>
        /// 终节点
        /// </summary>
        public string End_Id { get; set; }

        /// <summary>
        /// 终点节点类型
        /// </summary>
        public NodeType EndNodeType { get; set; }

        /// <summary>
        /// 敷设方式
        /// </summary>
        public CableLayMode C_LayMode { get; set; }

        /// <summary>
        /// 电缆通道类型ID
        /// </summary>
        public string C_ModeId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SurveyState State { get; set; }

        /// <summary>
        /// 设计人员
        /// </summary>
        public string Surveyor { get; set; }

        /// <summary>
        /// 勘测时间
        /// </summary>
        public DateTime SurveyTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public float Length { get; set; }
    }
}
