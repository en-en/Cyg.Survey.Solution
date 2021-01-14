
using Cyg.Applicatio.Survey.Dto;
using Survey.Dto.Response.DbDto;
using System.Collections.Generic;

namespace Cyg.Applicatio
{
    /// <summary>
    /// 
    /// </summary>
    public class CollectionGisDataRequest
    {

        /// <summary>
        /// 杆塔
        /// </summary>
        public List<PlanTowerGisDto> Towers { get; set; }

        /// <summary>
        /// 电缆井
        /// </summary>
        public List<PlanCableDto> Cables { get; set; }

        /// <summary>
        /// 电缆通道
        /// </summary>
        public List<PlanCableChannelDto> CableChannels { get; set; }

        /// <summary>
        /// 电缆杆上设备
        /// </summary>
        public List<PlanCableEquipmentDto> CableEquipments { get; set; }

        /// <summary>
        /// 架空杆上设备
        /// </summary>
        public List<PlanOverHeadDeviceDto> OverheadEquipments { get; set; }

        /// <summary>
        /// 线路
        /// </summary>
        public List<PlanLineDto> Lines { get; set; }

        /// <summary>
        /// 地物
        /// </summary>
        public List<PlanMarkDto> Marks { get; set; }

        /// <summary>
        /// 轨迹记录集合
        /// </summary>
        public List<ReportTrackRecord> TrackRecords { get; set; }

       // public List<SurveyTrackDto> TrackRecords { get; set; }

        /// <summary>
        /// 勘察多媒体文件
        /// </summary>
        public List<MediaRequest> Medias { get; set; }

        /// <summary>
        ///     变压器
        /// </summary>
        public List<PlanTransformerDto> Transformers { get; set; }

    }
}
