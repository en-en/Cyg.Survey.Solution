using System.Collections.Generic;

namespace Cyg.Applicatio.Survey.Dto
{
    public class CollectionDataRequest
    {
        /// <summary>
        /// 杆塔
        /// </summary>
        public List<DesignTowerDto> Towers { get; set; }

        /// <summary>
        /// 电缆井
        /// </summary>
        public List<CableRequest> Cables { get; set; }

        /// <summary>
        /// 电缆通道
        /// </summary>
        public List<CableChannelRequest> CableChannels { get; set; }

        /// <summary>
        /// 电缆杆上设备
        /// </summary>
        public List<CableEquipmentRequest> CableEquipments { get; set; }

        /// <summary>
        /// 架空杆上设备
        /// </summary>
        public List<OverheadEquipmentRequest> OverheadEquipments { get; set; }

        /// <summary>
        /// 线路
        /// </summary>
        public List<LineRequest> Lines { get; set; }

        /// <summary>
        /// 地物
        /// </summary>
        public List<MarkRequest> Marks { get; set; }

        /// <summary>
        /// 轨迹记录集合
        /// </summary>
        public List<ReportTrackRecord> TrackRecords { get; set; }

        /// <summary>
        /// 勘察多媒体文件
        /// </summary>
        public List<MediaRequest> Medias { get; set; }

    }
}
