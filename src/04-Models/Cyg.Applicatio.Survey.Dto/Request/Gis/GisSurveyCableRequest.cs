using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto
{
    public class GisSurveyCableRequest
    {
        /// <summary>
        /// 主键 Id
        /// </summary>
        [DisplayName("主键")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Id { get; set; }

        /// <summary>
        /// 上一节点
        /// </summary>
        [Required]
        public string ParentId { get; set; }

        /// <summary>
        /// 电缆井编号
        /// </summary>
        [DisplayName("电缆井编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Code { get; set; }

        /// <summary>
        /// 电缆井类型
        /// </summary>
        [DefineEnum(typeof(CableType))]
        public CableType Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 勘测人
        /// </summary>
        public string Surveyor { get; set; }

        /// <summary>
        /// 勘测日期
        /// </summary>
        public DateTime? SurveyTime { get; set; }

        /// <summary>
        /// 敷设方式：无、排管、直埋、电缆沟、隧道
        /// </summary>
        [DefineEnum(typeof(CableLayMode))]
        public CableLayMode LayMode { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [DisplayName("项目编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 上一节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
        public NodeType PreNodeType { get; set; }

        /// <summary>
        /// 带电作业
        /// </summary>
        public bool ElectrifiedWork { get; set; }

        /// <summary>
        /// 逻辑编号
        /// </summary>
        public string LogicCode { get; set; }

        /// <summary>
        /// 电缆井方案
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 电缆井方案ID
        /// </summary>
        public string ModeId { get; set; }

        /// <summary>
        /// 符号Id
        /// </summary>
        public short SymbolId { get; set; }

        /// <summary>
        /// 方位角
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 几何列,类型为点
        /// </summary>
        public string Geom { get; set; }
    }
}
