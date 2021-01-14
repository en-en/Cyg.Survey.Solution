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
	public class GisSurveyCableChannelRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DisplayName("主键")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Id { get; set; }

        /// <summary>
        /// 电缆通道编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 始节点
        /// </summary>
        public string StartId { get; set; }

        /// <summary>
        /// 终节点
        /// </summary>
        public string EndId { get; set; }

        /// <summary>
        /// 敷设方式
        /// </summary>
        [DefineEnum(typeof(CableLayMode))]
        public CableLayMode LayMode { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }


        /// <summary>
        /// 电缆管材质编号
        /// </summary>
        public string DuctId { get; set; }

        /// <summary>
        /// 电缆管材质型号
        /// </summary>
        public string DuctSpec { get; set; }

        /// <summary>
        /// 电缆通道型号
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 电缆通道类型ID
        /// </summary>
        public string ModeId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 始节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public NodeType StartNodeType { get; set; }

        /// <summary>
        /// 终点节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public int EndNodeType { get; set; }

        /// <summary>
        /// 勘测人
        /// </summary>
        public string Surveyor { get; set; }

        /// <summary>
        /// 勘测日期
        /// </summary>
        public DateTime? SurveyTime { get; set; }

        /// <summary>
        /// 排列方式
        /// </summary>
        public string Arrangement { get; set; }

        /// <summary>
        /// 符号Id
        /// </summary>
        public short SymbolId { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// 预留宽度(mm)
        /// </summary>
        public double RsvWidth { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [DisplayName("项目编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 电压
        /// </summary>
        public string Voltage { get; set; }

        /// <summary>
        /// 是否交底
        /// </summary>
        public bool IsDisclosure { get; set; }
        /// <summary>
        /// 几何列，类型未线
        /// </summary>
        public string Geom { get; set; }
    }
}
