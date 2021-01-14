using Cyg.Resource.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 地物
    /// </summary>
	public class GisSurveyMarkRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DisplayName("主键")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Id { get; set; }

        

        /// <summary>
        /// 地物名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DefineEnum(typeof(MarkType))]
        public MarkType Type { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// 方位角
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        /// 道路等级
        /// </summary>
        [DefineEnum(typeof(RoadLevel))]
        public RoadLevel RoadLevel { get; set; }

        /// <summary>s
        /// 电力线电压等级
        /// </summary>
        [DefineEnum(typeof(LineKvLevel))]
        public LineKvLevel LineKvLevel { get; set; }

        /// <summary>
        /// 楼层数（m）
        /// </summary>
        public int Floors { get; set; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [DisplayName("项目编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }

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
        /// 
        /// </summary>
        public string LiveCross { get; set; }

        /// <summary>
        /// 符号Id
        /// </summary>
        public short SymbolId { get; set; }

        /// <summary>
        /// 几何列，类型为点
        /// </summary>
        public string Geom { get; set; }
    }
}
