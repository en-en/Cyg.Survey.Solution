using Cyg.Applicatio.Dto;
using Cyg.Resource.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    public class CableRequest
    {
        /// <summary>
        /// 主键编号
        /// </summary>
        [DisplayName("主键编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Id { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        [DisplayName("工程编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 上一节点
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 上一节点类型
        /// </summary>
        [DefineEnum(typeof(NodeType))]
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
        [DefineEnum(typeof(CableType))]
        public CableType C_Type { get; set; }

        /// <summary>
        /// 电缆井型号
        /// </summary>
        public string C_ModeID { get; set; }

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
        ///  带电作业
        /// </summary>
        public bool ElectrifiedWork { get; set; }

        /// <summary>
        ///     上传用户
        /// </summary>
        public string Surveyor { get; set; }
    }
}
