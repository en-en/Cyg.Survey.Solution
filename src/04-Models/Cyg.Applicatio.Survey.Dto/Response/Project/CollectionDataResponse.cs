﻿using System.Collections.Generic;

namespace Cyg.Applicatio.Survey.Dto
{
    public class CollectionDataResponse
    {
        /// <summary>
        /// 杆塔
        /// </summary>
        public IEnumerable<TowerResponse> Towers { get; set; }

        /// <summary>
        /// 电缆井
        /// </summary>
        public IEnumerable<CableResponse> Cables { get; set; }

        /// <summary>
        /// 电缆通道
        /// </summary>
        public IEnumerable<CableChannelResponse> CableChannels { get; set; }

        /// <summary>
        /// 电缆杆上设备
        /// </summary>
        public IEnumerable<CableEquipmentResponse> CableEquipments { get; set; }

        /// <summary>
        /// 架空杆上设备
        /// </summary>
        public IEnumerable<OverheadEquipmentResponse> OverheadEquipments { get; set; }

        /// <summary>
        /// 线路
        /// </summary>
        public IEnumerable<LineResponse> Lines { get; set; }

        /// <summary>
        /// 地物
        /// </summary>
        public IEnumerable<MarkResponse> Marks { get; set; }

        /// <summary>
        /// 多媒体
        /// </summary>
        public IEnumerable<MediaResponse> Medias { get; set; }

        /// <summary>
        /// 轨迹记录集合
        /// </summary>
        public IEnumerable<ProjectTrackRecordResponse> TrackRecords { get; set; }
    }
}
