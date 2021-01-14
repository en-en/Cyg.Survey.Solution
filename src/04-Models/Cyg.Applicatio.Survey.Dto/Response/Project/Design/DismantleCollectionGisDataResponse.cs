using Survey.Dto.Response.DbDto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto.Response.Project.Design
{
    public class DismantleCollectionGisDataResponse
    {
        public DismantleCollectionGisDataResponse()
        {
            CableChannelProfileList = new List<DismantleCableChannelProfileDto>();
            CableChannelList = new List<DismantleCableChannelDto>();
            CableEquipmentList = new List<DismantleCableEquipmentDto>();
            CableHeadList = new List<DismantleCableHeadDto>();
            CableList = new List<DismantleCableDto>();
            ElectricMeterList = new List<DismantleElectricMeterDto>();
            HoleList = new List<DismantleHoleDto>();
            LineList = new List<DismantleLineDto>();
            MarkList = new List<DismantleMarkDto>();
            MaterialInfoList = new List<DismantleMaterialInfoDto>();
            OverheadDeviceList = new List<DismantleOverHeadDeviceDto>();
            PullLineList = new List<DismantlePullLineDto>();
            TowerList = new List<DismantleTowerDto>();
            TransformerList = new List<DismantleTransformerDto>();
            TurningPointList = new List<DismantleTurningPointsDto>();
            MaterialStatusList = new List<DismantleMaterialStatus>();
            UserLineList = new List<DismantleUserLineDto>();
            CrossArmList = new List<DismantleCrossArmDto>();
          //  BraceList = new List<DismantleBraceDto>();
        }
        /// <summary>
        /// 电缆通道剖面列表
        /// </summary>
        [DisplayName("电缆通道剖面列表")]
        //[Required]
        public List<DismantleCableChannelProfileDto> CableChannelProfileList { get; set; }

        /// <summary>
        /// 电缆通道列表
        /// </summary>
        [DisplayName("电缆通道列表")]
        //[Required]
        public List<DismantleCableChannelDto> CableChannelList { get; set; }

        /// <summary>
        /// 电缆杆上设备列表
        /// </summary>
        [DisplayName("电缆杆上设备列表")]
        //[Required]
        public List<DismantleCableEquipmentDto> CableEquipmentList { get; set; }

        /// <summary>
        /// 电缆终端头列表
        /// </summary>
        [DisplayName("电缆终端头列表")]
        //[Required]
        public List<DismantleCableHeadDto> CableHeadList { get; set; }

        /// <summary>
        /// 电缆井列表
        /// </summary>
        [DisplayName("电缆井列表")]
        //[Required]
        public List<DismantleCableDto> CableList { get; set; }

        /// <summary>
        /// 户表列表
        /// </summary>
        [DisplayName("户表列表")]
        //[Required]
        public List<DismantleElectricMeterDto> ElectricMeterList { get; set; }

        /// <summary>
        /// 穿孔列表
        /// </summary>
        [DisplayName("穿孔列表")]
        //[Required]
        public List<DismantleHoleDto> HoleList { get; set; }

        /// <summary>
        /// 线路列表
        /// </summary>
        [DisplayName("线路列表")]
        //[Required]
        public List<DismantleLineDto> LineList { get; set; }

        /// <summary>
        /// 地物列表
        /// </summary>
        [DisplayName("地物列表")]
        //[Required]
        public List<DismantleMarkDto> MarkList { get; set; }

        /// <summary>
        /// 材料信息列表
        /// </summary>
        [DisplayName("材料信息列表")]
        //[Required]
        public List<DismantleMaterialInfoDto> MaterialInfoList { get; set; }

        /// <summary>
        /// 架空杆上设备列表
        /// </summary>
        [DisplayName("架空杆上设备列表")]
        //[Required]
        public List<DismantleOverHeadDeviceDto> OverheadDeviceList { get; set; }

        /// <summary>
        /// 拉线列表
        /// </summary>
        [DisplayName("拉线列表")]
        //[Required]
        public List<DismantlePullLineDto> PullLineList { get; set; }

        /// <summary>
        /// 杆塔列表
        /// </summary>
        [DisplayName("杆塔列表")]
        //[Required]
        public List<DismantleTowerDto> TowerList { get; set; }

        /// <summary>
        /// 变压器列表
        /// </summary>
        [DisplayName("变压器列表")]
        //[Required]
        public List<DismantleTransformerDto> TransformerList { get; set; }

        /// <summary>
        /// 折点列表
        /// </summary>
        [DisplayName("折点列表")]
        //[Required]
        public List<DismantleTurningPointsDto> TurningPointList { get; set; }

        /// <summary>
        /// 物料状态列表
        /// </summary>
        [DisplayName("物料状态列表")]
        //[Required]
        public List<DismantleMaterialStatus> MaterialStatusList { get; set; }

        /// <summary>
        /// 下户线列表
        /// </summary>
        [DisplayName("下户线列表")]
        public List<DismantleUserLineDto> UserLineList { get; set; }

        /// <summary>
        /// 横担列表
        /// </summary>
        [DisplayName("横担列表")]
        public List<DismantleCrossArmDto> CrossArmList { get; set; }
        /// <summary>
        /// 撑杆列表
        /// </summary>
        // [DisplayName("撑杆列表")]
        // public List<DismantleBraceDto> BraceList { get; set; }

        /// <summary>
        /// 多媒体
        /// </summary>
        public IEnumerable<MediaRequest> Medias { get; set; }
    }
}
