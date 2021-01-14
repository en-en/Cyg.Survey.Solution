using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;
using System.Reflection;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 线路
    /// </summary>
    public class LineResponse
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
        public NodeType StartNodeType { get; set; }

        /// <summary>
        /// 终节点
        /// </summary>
        public string End_ID { get; set; }

        /// <summary>
        /// 终点节点类型
        /// </summary>
        public NodeType EndNodeType { get; set; }

        /// <summary>
        /// 线路类型
        /// </summary>
        public string L_Type { get; set; }

        /// <summary>
        /// 线路型号
        /// </summary>
        public string L_Mode { get; set; }

        /// <summary>
        /// 线路长度
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// 电压等级
        /// </summary>
        public KVLevel KVLevel { get; set; }

        /// <summary>
        /// 是否电缆
        /// </summary>
        public bool IsCable { get; set; }

        /// <summary>
        /// 回路序列号
        /// </summary>
        public int LoopSerial { get; set; }

        /// <summary>
        /// 线路组编号
        /// </summary>
        public string GroupId { get; set; }

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
        /// 回路标识
        /// </summary>
        public short Index { get; set; }

        /// <summary>
        /// 线路型号iD
        /// </summary>
        public string L_ModeID { get; set; }
    }
}
