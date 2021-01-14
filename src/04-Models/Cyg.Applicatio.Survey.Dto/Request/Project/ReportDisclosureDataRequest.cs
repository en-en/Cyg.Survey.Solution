using Cyg.Applicatio.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cyg.Applicatio.Survey.Dto
{
    public class ReportDisclosureDataRequest
    {
        /// <summary>
        /// 工程编号
        /// </summary>
        [DisplayName("工程编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ProjectId { get; set; }

        /// <summary>
        /// 交底数据
        /// </summary>
        public List<ReportDisclosure> Disclosures { get; set; }

        /// <summary>
        /// 轨迹记录集合
        /// </summary>
        public List<ReportTrackRecord> TrackRecords { get; set; }
    }

    public class ReportDisclosure
    {
        /// <summary>
        /// 杆塔/电缆井/电气设备 编号
        /// </summary>
        [DisplayName("杆塔/电缆井/电气设备编号")]
        [Required(ErrorMessage = "{0} 不能为空")]
        public string ItemId { get; set; }

        /// <summary>
        /// 交底类型
        /// </summary>
        [DefineEnum(typeof(ProjectDisclosureType))]
        public ProjectDisclosureType ItemType { get; set; }

        /// <summary>
        /// 交底状态
        /// </summary>
        [DefineEnum(typeof(ProjectDisclosureStatus))]
        public ProjectDisclosureStatus Status { get; set; }

        /// <summary>
        /// 交底日期
        /// </summary>
        public DateTime DisclosureDate { get; set; }

        /// <summary>
        /// 交底类型
        /// </summary>
        public ProjectTraceRecordType RecordType { get; set; }

        /// <summary>
        /// 交底明细
        /// </summary>
        public List<ReportDisclosureItem> Items { get; set; }
    }

    public class ReportDisclosureItem
    {
        /// <summary>
        /// 类别
        /// </summary>
        [DefineEnum(typeof(ProjectDisclosureItemType))]
        public ProjectDisclosureItemType Category { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 文件编号
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// 交底类型
        /// </summary>
        public ProjectTraceRecordType RecordType { get; set; }
    }

}
