using Cyg.Applicatio.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// Gis勘察轨迹
    /// </summary>
    public class GisDisclosureTrackRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime? RecordDate { get; set; }

        /// <summary>
        /// 记录人
        /// </summary>
        public string Recorder { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 轨迹类型
        /// </summary>
        public ProjectTraceRecordType  Type { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// 几何列，类型为点
        /// </summary>
        public string Geom { get; set; }
    }
}
