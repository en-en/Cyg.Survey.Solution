using Survey.Dto.Response.DbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto.Response.Project.Design
{
    public class DesignCollectionGisDataResponse
    {
        public DesignCollectionGisDataResponse()
        {
            Towers = new List<DesignTowerGisDto>();
            Cables = new List<DesignCableDto>();
            CableChannels = new List<DesignCableChannelDto>();
            CableChannelProfiles = new List<DesignCableChannelProfileDto>();
            Cableheads = new List<DesignCableHeadDto>();
            CableEquipments = new List<DesignCableEquipmentDto>();
            OverheadEquipments = new List<DesignOverHeadDeviceDto>();
            Lines = new List<DesignLineDto>();
            Marks = new List<DesignMarkDto>();
            Meters = new List<DesignElectricMeterDto>();
            Materials = new List<DesignMaterialInfoDto>();
            MaterialStates = new List<DesignMaterialStatusDto>();
            UserLines = new List<DesignUserLineDto>();
            CrossArms = new List<DesignCrossArmDto>();
            PullLineRequests = new List<DesignPullLineDto>();
           // Braces = new List<DesignBraceDto>();
        }

        /// <summary>
        /// 杆塔
        /// </summary>
        public List<DesignTowerGisDto> Towers { get; set; }

        /// <summary>
        /// 电缆井
        /// </summary>
        public List<DesignCableDto> Cables { get; set; }

        /// <summary>
        /// 电缆通道
        /// </summary>
        public List<DesignCableChannelDto> CableChannels { get; set; }

        /// <summary>
        /// 电缆通道剖面
        /// </summary>
        public List<DesignCableChannelProfileDto> CableChannelProfiles { get; set; }

        /// <summary>
        /// 电缆头
        /// </summary>
        public List<DesignCableHeadDto> Cableheads { get; set; }

        /// <summary>
        /// 电缆杆上设备
        /// </summary>
        public List<DesignCableEquipmentDto> CableEquipments { get; set; }

        /// <summary>
        /// 架空杆上设备
        /// </summary>
        public List<DesignOverHeadDeviceDto> OverheadEquipments { get; set; }

        /// <summary>
        /// 线路
        /// </summary>
        public List<DesignLineDto> Lines { get; set; }

        /// <summary>
        /// 地物
        /// </summary>
        public List<DesignMarkDto> Marks { get; set; }

        /// 户表
        /// </summary>
        public List<DesignElectricMeterDto> Meters { get; set; }
        /// <summary>

        /// <summary>
        /// 物料
        /// </summary>
        public List<DesignMaterialInfoDto> Materials { get; set; }

        /// <summary>
        /// 物料状态
        /// </summary>
        public List<DesignMaterialStatusDto> MaterialStates { get; set; }

        /// <summary>
        /// 下户线
        /// </summary>
        public List<DesignUserLineDto> UserLines { get; set; }

        /// <summary>
        /// 横担
        /// </summary>
        public List<DesignCrossArmDto> CrossArms { get; set; }

        /// <summary>
        /// 撑杆
        /// </summary>
      //  public List<DesignBraceDto> Braces { get; set; }

        /// <summary>
        ///     拉线数据
        /// </summary>
        public List<DesignPullLineDto> PullLineRequests
        {
            get; set;
        }

        /// <summary>
        /// 多媒体
        /// </summary>
        public IEnumerable<MediaRequest> Medias { get; set; }
    }
}
