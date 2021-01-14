using Cyg.Applicatio.Dto;
using System;
using System.Collections.Generic;

namespace Cyg.Applicatio.Survey.Dto
{
    public class ProjectDisclosureResponse
    {
        /// <summary>
        /// 交底主键标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 杆塔/电缆井/电气设备 编号
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 交底类型
        /// </summary>
        public ProjectDisclosureType ItemType { get; set; }

        /// <summary>
        /// 交底状态
        /// </summary>
        public ProjectDisclosureStatus Status { get; set; }

        /// <summary>
        /// 交底日期
        /// </summary>
        public DateTime DisclosureDate { get; set; }

        /// <summary>
        /// 交底明细
        /// </summary>
        public List<ProjectDisclosureItemResponse> Items { get; set; }
    }

    public class ProjectDisclosureItemResponse
    {
        /// <summary>
        /// 交底明细主键标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工程编号
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 交底编号
        /// </summary>
        public string DisclosureId { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public ProjectDisclosureItemType Category { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// 文件编号
        /// </summary>
        public string FileId { get; set; }
    }
}
