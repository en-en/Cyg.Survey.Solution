using System;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 轨迹记录集合
    /// </summary>
    public class ProjectTrackRecordResponse
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
    }
}
