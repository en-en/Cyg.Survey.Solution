using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 电缆杆上设备
    /// </summary>
	public class GisSurveyCableEquipmentRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DisplayName("主键")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Id { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [DefineEnum(typeof(CableEquipmentType))]
        public CableEquipmentType Type { get; set; }

        /// <summary>
        /// 上一节点
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string EquipModel { get; set; }

        /// <summary>
        /// 型号ID
        /// </summary>
        public string EquipModelId { get; set; }

        /// <summary>
        /// 容量
        /// </summary>
        public string Capacity { get; set; }

        /// <summary>
        /// 长
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }

        /// <summary>
        /// 符号Id
        /// </summary>
        public short SymbolId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 上一节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public NodeType PreNodeType { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string Code { get; set; }

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
        /// 方位角
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 逻辑编号
        /// </summary>
        public string LogicCode { get; set; }

        /// <summary>
        /// 几何列,类型为点
        /// </summary>
        public string Geom { get; set; }
    }
}
