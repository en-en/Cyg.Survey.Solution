using Cyg.Applicatio.Dto.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 多媒体请求
    /// </summary>
    public class MediaRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
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
        [DisplayName("项目编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 多媒体类型
        /// </summary>
        [DisplayName("多媒体类型")]
        [DefineEnum(typeof(MediaType))]
        public MediaType Type { get; set; }

        /// <summary>
        /// 文件大小(字节)
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 存储路径
        /// </summary>
        [DisplayName("存储路径")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string FilePath { get; set; }

        /// <summary>
        /// 所属设备ID
        /// </summary>
        [DisplayName("所属设备ID")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string Main_ID { get; set; }

        /// <summary>
        /// 所属设备类型
        /// </summary>
        [DisplayName("所属设备类型")]
        [DefineEnum(typeof(SurveyDeviceType))]
        public SurveyDeviceType Main_Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
