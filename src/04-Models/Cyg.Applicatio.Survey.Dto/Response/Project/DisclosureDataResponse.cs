using System.Collections.Generic;

namespace Cyg.Applicatio.Survey.Dto
{
    public class DisclosureDataResponse
    {
        /// <summary>
        /// 交底点位数据
        /// </summary>
        public List<ProjectDisclosureResponse> Disclosures { get; set; }

        /// <summary>
        /// 轨迹记录集合
        /// </summary>
        public IEnumerable<ProjectTrackRecordResponse> TrackRecords { get; set; }
    }
}
