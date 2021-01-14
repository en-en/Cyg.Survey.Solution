using Cyg.Applicatio.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 工程轨迹
    /// </summary>
    public class ReportTrackRecord
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat_WGS84 { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon_WGS84 { get; set; }

        /// <summary>
        /// 记录日期
        /// </summary>
        public DateTime RecordTime { get; set; }

        /// <summary>
        /// 交底类型
        /// </summary>
        public ProjectTraceRecordType RecordType { get; set; }
    }
}
