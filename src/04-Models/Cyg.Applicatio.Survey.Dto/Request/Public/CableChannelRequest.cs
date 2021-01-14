using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 电缆通道
    /// </summary>
    public class CableChannelRequest
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        [DisplayName("主键编号")]
        [MaxLength(32, ErrorMessage = "{0} 长度不能超过{1}")]
        public string Id { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        [DisplayName("工程编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 始节点
        /// </summary>
        public string Start_Id { get; set; }

        /// <summary>
        /// 始节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public NodeType StartNodeType { get; set; }

        /// <summary>
        /// 终节点
        /// </summary>
        public string End_Id { get; set; }

        /// <summary>
        /// 终点节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public NodeType EndNodeType { get; set; }

        /// <summary>
        /// 敷设方式
        /// </summary>
        [DefineEnum(typeof(CableLayMode))]
        public CableLayMode C_LayMode { get; set; }

        /// <summary>
        /// 电缆通道类型ID
        /// </summary>
        public string C_ModeId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }

        /// <summary>
        /// 设计时间
        /// </summary>
        public DateTime SurveyTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 线路长度
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        ///     上传用户
        /// </summary>
        public string Surveyor { get; set; }
    }
}
