using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 线路
    /// </summary>
    public class LineRequest
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        [DisplayName("主键编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        [MaxLength(64, ErrorMessage = "{0} 长度不能超过{1}")]
        public string Id { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        [DisplayName("工程编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 线路名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 回路名称
        /// </summary>
        public string LoopName { get; set; }

        /// <summary>
        /// 始节点
        /// </summary>
        public string Start_ID { get; set; }

        /// <summary>
        /// 始节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public NodeType StartNodeType { get; set; }

        /// <summary>
        /// 终节点
        /// </summary>
        public string End_ID { get; set; }

        /// <summary>
        /// 终点节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public NodeType EndNodeType { get; set; }

        /// <summary>
        /// 线路类型
        /// </summary>
        public string L_Type { get; set; }

        /// <summary>
        /// 线路型号
        /// </summary>
        [DisplayName("线路型号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string L_Mode { get; set; }

        /// <summary>
        /// 线路型号Id
        /// </summary>
        public string L_ModeID { get; set; }

        /// <summary>
        /// 线路长度
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// 电压等级
        /// </summary>
        [DefineEnum(typeof(KVLevel))]
        public KVLevel KVLevel { get; set; }

        /// <summary>
        /// 是否电缆
        /// </summary>
        public bool IsCable { get; set; }

        /// <summary>
        /// 回路序列号
        /// </summary>
        [DisplayName("回路序列号")]
        public int LoopSerial { get; set; }

        /// <summary>
        /// 线路组编号
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }

        /// <summary>
        /// 勘测时间
        /// </summary>
        public DateTime SurveyTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 回路标识
        /// </summary>
        public short Index { get; set; }

        /// <summary>
        ///     上传用户
        /// </summary>
        public string Surveyor { get; set; }
    }
}
