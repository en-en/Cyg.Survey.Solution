using Cyg.Applicatio.Survey.Dto;
using Survey.Dto.Response.DbDto;
using System.Collections.Generic;

namespace Cyg.Applicatio
{
    public class CollectionDataGisResponse
    {
        /// <summary>
        /// 杆塔
        /// </summary>
        public IEnumerable<PlanTowerGisDto> Towers { get; set; }

        /// <summary>
        /// 电缆井
        /// </summary>
        public IEnumerable<PlanCableDto> Cables { get; set; }

        /// <summary>
        /// 电缆通道
        /// </summary>
        public IEnumerable<PlanCableChannelDto> CableChannels { get; set; }

        /// <summary>
        /// 电缆杆上设备
        /// </summary>
        public IEnumerable<PlanCableEquipmentDto> CableEquipments { get; set; }

        /// <summary>
        /// 架空杆上设备
        /// </summary>
        public IEnumerable<PlanOverHeadDeviceDto> OverheadEquipments { get; set; }

        /// <summary>
        /// 线路
        /// </summary>
        public IEnumerable<PlanLineDto> Lines { get; set; }

        /// <summary>
        /// 变压器
        /// </summary>
        public IEnumerable<PlanTransformerDto> Transformers { get; set; }

        /// <summary>
        /// 地物
        /// </summary>
        public IEnumerable<PlanMarkDto> Marks { get; set; }

        /// <summary>
        /// 多媒体
        /// </summary>
        public IEnumerable<Survey.Dto.MediaRequest> Medias { get; set; }

        /// <summary>
        /// 轨迹记录集合
        /// </summary>
        public IEnumerable<ProjectTrackRecordResponse> TrackRecords { get; set; }
   
    
    }
}
