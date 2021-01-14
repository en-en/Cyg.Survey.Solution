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
    public class GisSurveyLineRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DisplayName("主键")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Id { get; set; }

        

        /// <summary>
        /// 线路名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 线路组编号
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 始节点
        /// </summary>
        public string StartId { get; set; }

        /// <summary>
        /// 终节点
        /// </summary>
        public string EndId { get; set; }

        /// <summary>
        /// 线路类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 线路型号
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 线路长度
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// 电压等级
        /// </summary>
        [DefineEnum(typeof(KVLevel))]
        public KVLevel KvLevel { get; set; }

        /// <summary>
        /// 回路序列号
        /// </summary>
        public int LoopSerial { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }


        /// <summary>
        /// 是否电缆
        /// </summary>
        public bool IsCable { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [DisplayName("项目编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 勘测人
        /// </summary>
        public string Surveyor { get; set; }

        /// <summary>
        /// 勘测日期
        /// </summary>
        public DateTime? SurveyTime { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 始节点类型
        /// </summary>
        
        public NodeType StartNodeType { get; set; }

        /// <summary>
        /// 终节点类型
        /// </summary>
        public int EndNodeType { get; set; }

        /// <summary>
        /// 回路名称
        /// </summary>
        public string LoopName { get; set; }

        /// <summary>
        /// 型号ID
        /// </summary>
        public string ModeId { get; set; }

        /// <summary>
        /// 电缆回路数
        /// </summary>
        public int CableNumber { get; set; }

        /// <summary>
        /// 方位角
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 逻辑顺序
        /// </summary>
        public int LoopSeq { get; set; }

        /// <summary>
        /// 回路层级
        /// </summary>
        public int LoopLevel { get; set; }

        /// <summary>
        /// 父级回路名称
        /// </summary>
        public string ParentLoopName { get; set; }

        /// <summary>
        /// 折点
        /// </summary>
        public string TurningPoints { get; set; }

        /// <summary>
        /// 符号Id
        /// </summary>
        public int SymbolId { get; set; }

        /// <summary>
        /// 几何列，类型为线
        /// </summary>
        public string Geom { get; set; }

        /// <summary>
        /// 回路标识
        /// </summary>
        public short Index { get; set; }


    }
}
