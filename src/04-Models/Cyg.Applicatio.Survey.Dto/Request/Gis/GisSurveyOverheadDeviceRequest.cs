using Cyg.Resource.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 架空杆上设备
    /// </summary>
	public class GisSurveyOverheadDeviceRequest
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
        /// 类型
        /// </summary>
        [DefineEnum(typeof(OverheadEquipmentType))]
        public OverheadEquipmentType Type { get; set; }

        /// <summary>
        /// 所属杆塔ID（主杆）
        /// </summary>
        public string MainId { get; set; }

        /// <summary>
        /// 所属杆塔ID（副杆）
        /// </summary>
        public string SubId { get; set; }

        /// <summary>
        /// 容量
        /// </summary>
        public string Capacity { get; set; }

        /// <summary>
        /// 安装方法
        /// </summary>
        [DefineEnum(typeof(OverheadEquipmentFixMode))]
        public OverheadEquipmentFixMode FixMode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DefineEnum(typeof(SurveyState))]
        public SurveyState State { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon { get; set; }

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
        /// 型号
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// 型号ID
        /// </summary>
        public string ModeId { get; set; }

        /// <summary>
        /// 方位角
        /// </summary>
        public double Azimuth { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        [DisplayName("项目编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 符号Id
        /// </summary>
        public short SymbolId { get; set; }

        /// <summary>
        /// 集合列,类型为点
        /// </summary>
        public string Geom { get; set; }
    }
}
