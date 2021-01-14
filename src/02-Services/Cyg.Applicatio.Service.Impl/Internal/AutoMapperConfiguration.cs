using AutoMapper;
using AutoMapper.Configuration;
using Cyg.Applicatio.Entity;
using Cyg.Applicatio.Survey.Dto;
using GisModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Survey.Dto.Response.DbDto;
using System;
using System.Collections;
using System.Collections.Generic;
using Cyg.Extensions.AutoMapper;

namespace Cyg.Applicatio.Service.Impl.Internal
{
    [Dependency(ServiceLifetime.Singleton)]
    public partial class AutoMapperConfiguration : IAutoMapperConfiguration
    {
        public void CreateMaps(MapperConfigurationExpression mapper)
        {
            mapper.CreateMap<DesignTowerDto, SurveyTower>();
            mapper.CreateMap<DesignTowerDto, DesignTower>();
            mapper.CreateMap<CableRequest, SurveyCable>();
            mapper.CreateMap<CableRequest, DesignCable>();
            mapper.CreateMap<CableRequest, SurveyCableExt>();
            mapper.CreateMap<CableRequest, DesignCableExt>();
            mapper.CreateMap<CableEquipmentRequest, SurveyCableEquipment>();
            mapper.CreateMap<CableEquipmentRequest, DesignCableEquipment>();
            mapper.CreateMap<OverheadEquipmentRequest, SurveyOverheadEquipment>();
            mapper.CreateMap<OverheadEquipmentRequest, DesignOverheadEquipment>();
            mapper.CreateMap<LineRequest, SurveyLine>();
            mapper.CreateMap<LineRequest, DesignLine>();
            mapper.CreateMap<MarkRequest, SurveyMark>();
            mapper.CreateMap<MarkRequest, DesignMark>();
            mapper.CreateMap<ReportTrackRecord, ProjectTrackRecord>();
            mapper.CreateMap<CableChannelRequest, SurveyCableChannel>();
            mapper.CreateMap<CableChannelRequest, DesignCableChannel>();
            mapper.CreateMap<ReportDisclosure, ProjectDisclosure>();
            mapper.CreateMap<ReportDisclosureItem, ProjectDisclosureItem>();
            mapper.CreateMap<News, NewsResponse>();
            mapper.CreateMap<NewsContent, NewsResponse>();

            mapper.CreateMap<DesignTowerDto, DismantleTower>();
            mapper.CreateMap<CableRequest, DismantleCable>();
            mapper.CreateMap<CableRequest, DismantleCableExt>();
            mapper.CreateMap<CableChannelRequest, DismantleCableChannel>();
            mapper.CreateMap<CableEquipmentRequest, DismantleCableEquipment>();
            mapper.CreateMap<OverheadEquipmentRequest, DismantleOverheadEquipment>();
            mapper.CreateMap<LineRequest, DismantleLine>();
            mapper.CreateMap<MarkRequest, DismantleMark>();
            mapper.CreateMap<LineRequest, SurveyLineExt>();
            mapper.CreateMap<LineRequest, DesignLineExt>();
            mapper.CreateMap<LineRequest, DismantleLineExt>();

            mapper.CreateMap<MediaRequest, SurveyMedia>();
            mapper.CreateMap<MediaRequest, DesignMedia>();
            mapper.CreateMap<MediaRequest, DismantleMedia>();

            #region gis
            mapper.CreateMap<DesignTowerGisDto, Survey_tower>()
                .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
                .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
                .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
                .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
                .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
                .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
                .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
                .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
                .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
                .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
                .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
                .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignTowerGisDto, Dismantle_tower>()
              .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
              .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
              .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
              .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
              .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
              .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
              .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
              .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
              .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
              .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
              .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
              .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
              .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignTowerGisDto, Design_tower>()
             .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
             .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
             .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
             .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
             .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
             .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
             .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
             .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
             .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
             .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
             .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
             .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
             .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
             .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
             .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignCableDto, Survey_cable>()
             .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
             .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
             .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
             .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
             .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
             .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
             .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
             .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
             .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignCableDto, Dismantle_cable>()
             .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
             .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
             .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
             .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
             .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
             .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
             .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
             .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
             .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignCableDto, Design_cable>()
            .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
            .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
            .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
            .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
            .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
            .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignCableChannelDto, Survey_cable_channel>()
            .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
            .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
            .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
            .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
            .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));

            mapper.CreateMap<DesignCableChannelDto, Dismantle_cable_channel>()
          .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
          .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
          .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
          .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
          .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
          .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
          .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
          .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignCableChannelDto, Design_cable_channel>()
          .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
          .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
          .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
          .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
          .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
          .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
          .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
          .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignTowerGisDto, Survey_cable_equipment>()
           .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.Mode))
           .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));

            mapper.CreateMap<DesignTowerGisDto, Dismantle_cable_equipment>()
           .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.Mode))
           .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignTowerGisDto, Design_cable_equipment>()
                     .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.Mode))
                     .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.ModeId))
                     .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
                     .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
                     .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
                     .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                     .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                     .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignOverHeadDeviceDto, Survey_over_head_device>()
               .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
               .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
               .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
               .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));

            mapper.CreateMap<DesignOverHeadDeviceDto, Dismantle_over_head_device>()
               .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
               .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
               .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
               .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignOverHeadDeviceDto, Design_over_head_device>()
              .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
              .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
              .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
              .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignLineDto, Survey_line>()
    .ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
    .ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
    .ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
    .ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
    .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
    .ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
    .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
    .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
    .ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
    .ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
    .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
    .ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
    .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
    .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
    .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
    .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
    .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
    .ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints));

            mapper.CreateMap<DesignLineDto, Dismantle_line>()
.ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
.ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
.ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
.ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
.ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
.ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
.ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
.ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
.ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
.ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
.ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
.ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints));
            mapper.CreateMap<DesignLineDto, Design_line>()
.ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
.ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
.ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
.ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
.ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
.ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
.ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
.ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
.ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
.ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
.ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
.ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints));

            mapper.CreateMap<DesignMarkDto, Survey_mark>()
.ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
.ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));

            mapper.CreateMap<DesignMarkDto, Dismantle_mark>()
.ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
.ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            mapper.CreateMap<DesignMarkDto, Design_mark>()
.ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
.ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));

            mapper.CreateMap<SurveyTrackDto, Survey_track>()
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Record_date, opt => opt.MapFrom(s => s.RecordDate));

            mapper.CreateMap<SurveyTrackDto, Dismantle_track>()
                .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                .ForMember(x => x.Record_date, opt => opt.MapFrom(s => s.RecordDate));
            #endregion
        }
    }

    public static class AutoMapperConfigurationExtend
    {
        public static IServiceProvider ConfigureServices(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
               // cfg.ReplaceMemberName("_", "");
                cfg.AddProfile<AutoMapperConfigurationProfile>();
                cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                //cfg.SourceMemberNamingConvention = new PascalCaseNamingConvention();
                //cfg.con
                //cfg.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            return services.BuildServiceProvider();
        }
    }

    public static class AutoMapperHelper
    {
        private static IServiceProvider ServiceProvider;

        public static void UseStateAutoMapper(this IApplicationBuilder applicationBuilder)
        {
            ServiceProvider = applicationBuilder.ApplicationServices;
        }

        public static TDestination Map<TDestination>(object source)
        {
            var mapper = ServiceProvider.GetRequiredService<AutoMapper.IMapper>();
            return mapper.Map<TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            var mapper = ServiceProvider.GetRequiredService<AutoMapper.IMapper>();

            return mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            var mapper = ServiceProvider.GetRequiredService<AutoMapper.IMapper>();
            return mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TDestination>(this object source)
        {
            var mapper = ServiceProvider.GetRequiredService<AutoMapper.IMapper>();
            var data = mapper.Map<TDestination>(source);
            return data;
        }

        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            var mapper = ServiceProvider.GetRequiredService<AutoMapper.IMapper>();
            return mapper.Map<List<TDestination>>(source);
        }


        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            var mapper = ServiceProvider.GetRequiredService<AutoMapper.IMapper>();
            return mapper.Map<List<TDestination>>(source);
        }

    }

    public class AutoMapperConfigurationProfile : Profile
    {

        public AutoMapperConfigurationProfile()
        {

            CreateMap<DesignTowerDto, SurveyTower>();
            CreateMap<DesignTowerDto, DesignTower>();
            CreateMap<CableRequest, SurveyCable>();
            CreateMap<CableRequest, DesignCable>();
            CreateMap<CableRequest, SurveyCableExt>();
            CreateMap<CableRequest, DesignCableExt>();
            CreateMap<CableEquipmentRequest, SurveyCableEquipment>();
            CreateMap<CableEquipmentRequest, DesignCableEquipment>();
            CreateMap<OverheadEquipmentRequest, SurveyOverheadEquipment>();
            CreateMap<OverheadEquipmentRequest, DesignOverheadEquipment>();
            CreateMap<LineRequest, SurveyLine>();
            CreateMap<LineRequest, DesignLine>();
            CreateMap<MarkRequest, SurveyMark>();
            CreateMap<MarkRequest, DesignMark>();
            CreateMap<ReportTrackRecord, ProjectTrackRecord>();
            CreateMap<CableChannelRequest, SurveyCableChannel>();
            CreateMap<CableChannelRequest, DesignCableChannel>();
            CreateMap<ReportDisclosure, ProjectDisclosure>();
            CreateMap<ReportDisclosureItem, ProjectDisclosureItem>();
            CreateMap<News, NewsResponse>();
            CreateMap<NewsContent, NewsResponse>();

            CreateMap<DesignTowerDto, DismantleTower>();
            CreateMap<CableRequest, DismantleCable>();
            CreateMap<CableRequest, DismantleCableExt>();
            CreateMap<CableChannelRequest, DismantleCableChannel>();
            CreateMap<CableEquipmentRequest, DismantleCableEquipment>();
            CreateMap<OverheadEquipmentRequest, DismantleOverheadEquipment>();
            CreateMap<LineRequest, DismantleLine>();
            CreateMap<MarkRequest, DismantleMark>();
            CreateMap<LineRequest, SurveyLineExt>();
            CreateMap<LineRequest, DesignLineExt>();
            CreateMap<LineRequest, DismantleLineExt>();

            CreateMap<MediaRequest, SurveyMedia>().ReverseMap();
            CreateMap<MediaRequest, DesignMedia>().ReverseMap();
            CreateMap<MediaRequest, DismantleMedia>().ReverseMap();
            CreateMap<MediaRequest, PlanMedia>().ReverseMap();
            CreateMap<Survey_tower, Dismantle_tower>().ReverseMap();
            CreateMap<Survey_cable, Dismantle_cable>().ReverseMap();
            CreateMap<Survey_cable_channel, Dismantle_cable_channel>().ReverseMap();
            CreateMap<Survey_cable_equipment, Dismantle_cable_equipment>().ReverseMap();
            CreateMap<Survey_over_head_device, Dismantle_over_head_device>().ReverseMap();
            CreateMap<Survey_line, Dismantle_line>().ReverseMap();
            CreateMap<Survey_mark, Dismantle_mark>().ReverseMap();
            CreateMap<Survey_track, Dismantle_track>().ReverseMap();
            CreateMap<ReportTrackRecord, ProjectTrackRecord>().ReverseMap();
            CreateMap<SurveyTrackDto, ProjectTrackRecord>().ReverseMap();
            CreateMap<MediaRequest, SurveyMedia>().ReverseMap();
            CreateMap<MediaRequest, DismantleMedia>().ReverseMap();
    
            CreateMap<DesignTowerGisDto, Design_tower>()
                .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
                .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
                .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
                .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
                .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
                .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
                .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
                .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
                .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
                .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
                .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
                .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<DismantleTowerDto, Dismantle_tower>()
              .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
              .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
              .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
              .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
              .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
              .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
              .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
              .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
              .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
              .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
              .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
              .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
              .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<PlanTowerGisDto, Plan_tower>()
            .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
            .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
            .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
            .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
            .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
            .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
            .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
            .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
            .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
            .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
            .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
            .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<PlanTowerGisDto, Survey_tower>()
       .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
       .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
       .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
       .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
       .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
       .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
       .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
       .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
       .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
       .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
       .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
       .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
       .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
       .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
       .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();


            CreateMap<PlanTowerGisDto, Design_tower>()
         .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
         .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
         .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
         .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
         .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
         .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
         .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
         .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
         .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
         .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
         .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
         .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
         .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
         .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
         .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<PlanCableDto, Plan_cable>()
           .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<PlanCableDto, Survey_cable>()
           .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DismantleCableDto, Dismantle_cable>()
           .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyCableDto, Survey_cable>()
           .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyCableChannelDto, Survey_cable_channel>()
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
           .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
           .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
           .ForMember(x => x.Duct_spec, opt => opt.MapFrom(s => s.DuctSpec))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<PlanCableChannelDto, Survey_cable_channel>()
        .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
        .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
        .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
        .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
               .ForMember(x => x.Duct_spec, opt => opt.MapFrom(s => s.DuctSpec))
        .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
        .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
        .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
        .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();


            CreateMap<PlanCableChannelDto, Plan_cable_channel>()
          .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
          .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
          .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
          .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
          .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                 .ForMember(x => x.Duct_spec, opt => opt.MapFrom(s => s.DuctSpec))
          .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
          .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
          .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<DismantleCableChannelDto, Dismantle_cable_channel>()
            .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
            .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
            .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
            .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
            .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                   .ForMember(x => x.Duct_spec, opt => opt.MapFrom(s => s.DuctSpec))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<PlanCableEquipmentDto, Survey_cable_equipment>()
            .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.EquipModel))
            .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.EquipModelId))
            .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
            .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
            .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DesignCableEquipmentDto, Design_cable_equipment>()
          .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.EquipModel))
          .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.EquipModelId))
          .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
          .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
          .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
          .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
          .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
          .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

           CreateMap<DismantleCableEquipmentDto, Dismantle_cable_equipment>()
          .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.EquipModel))
          .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.EquipModelId))
          .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
          .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
          .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
          .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
          .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
          .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<DismantleTowerDto, Dismantle_cable_equipment>()
           .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.Mode))
           .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            CreateMap<PlanCableEquipmentDto, Plan_cable_equipment>()
                      .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.EquipModel))
                      .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.EquipModelId))
                      .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
                      .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
                      .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
                      .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                      .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                      .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<PlanOverHeadDeviceDto, Survey_over_head_device>()
               .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
               .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
               .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
               .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DismantleOverHeadDeviceDto, Dismantle_over_head_device>()
               .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
               .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
               .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
               .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<PlanOverHeadDeviceDto, Plan_over_head_device>()
              .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
              .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
              .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
              .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyLineDto, Survey_line>()
    .ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
    .ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
    .ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
    .ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
    .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
    .ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
    .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
    .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
    .ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
    .ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
    .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
    .ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
    .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
    .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
    .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
    .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
    .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
    .ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints)).ReverseMap();

            CreateMap<DismantleLineDto, Dismantle_line>()
.ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
.ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
.ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
.ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
.ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
.ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
.ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
.ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
.ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
.ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
.ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
.ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints)).ReverseMap();
            CreateMap<PlanLineDto, Plan_line>()
  .ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
  .ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
  .ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
  .ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
  .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
  .ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
  .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
  .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
  .ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
  .ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
  .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
  .ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
  .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
  .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
  .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
  .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
  .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
  .ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints))
  .ForMember(x => x.End_id, opt => opt.MapFrom(s => s.EndId))
  .ReverseMap();
            CreateMap<PlanLineDto, Survey_line>()
.ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
.ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
.ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
.ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
.ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
.ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
.ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
.ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
.ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
.ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
.ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
.ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints))
.ForMember(x => x.End_id, opt => opt.MapFrom(s => s.EndId))
.ReverseMap();

            CreateMap<SurveyMarkDto, Survey_mark>()
     .ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
     .ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
     .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
     .ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
     .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
     .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<PlanMarkDto, Survey_mark>()
     .ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
     .ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
     .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
     .ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
     .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
     .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DismantleMarkDto, Dismantle_mark>()
.ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
.ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<PlanMarkDto, Plan_mark>()
.ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
.ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<SurveyPullLineDto, Survey_pull_line>()
.ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId)).ReverseMap();


            CreateMap<DismantlePullLineDto, Dismantle_pull_line>()
.ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId)).ReverseMap();
            CreateMap<PlanPullLineDto, Plan_pull_line>()
.ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId)).ReverseMap();
            CreateMap<DesignPullLineDto, Design_pull_line>()
.ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId)).ReverseMap();

            CreateMap<SurveyTrackDto, Survey_track>()
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Record_date, opt => opt.MapFrom(s => s.RecordDate)).ReverseMap();

            CreateMap<DismantleTrackDto, Dismantle_track>()
                .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                .ForMember(x => x.Record_date, opt => opt.MapFrom(s => s.RecordDate)).ReverseMap();
            CreateMap<PlanTowerGisDto, Survey_tower>()
                .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
                .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
                .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
                .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
                .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
                .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
                .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
                .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
                .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
                .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
                .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
                .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<SurveyTransformerDto, Survey_transformer>()
                       .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
                       .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
                       .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                       .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
                       .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                       .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                       .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                       .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
                       .ReverseMap();

            CreateMap<DesignTransformerDto, Design_transformer>()
                         .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
                         .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
                         .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                         .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
                         .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                         .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                         .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                         .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
                         .ReverseMap();
            CreateMap<PlanTransformerDto, Plan_transformer>()
                       .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
                       .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
                       .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                       .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
                       .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                       .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                       .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                       .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
                       .ReverseMap();
            CreateMap<PlanTransformerDto, Survey_transformer>()
           .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
           .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
           .ReverseMap();
            CreateMap<DismantleTransformerDto, Dismantle_transformer>()
                     .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
                     .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
                     .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                     .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
                     .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
                     .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                     .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                     .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
                     .ReverseMap();

            CreateMap<ReportTrackRecord, Survey_track>()
                                .ForMember(x => x.Record_date, opt => opt.MapFrom(s => s.RecordTime))
                                .ReverseMap();


            #region 设计拆除
            CreateMap<DesignTowerGisDto, Design_tower>()
            .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
            .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
            .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
            .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
            .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
            .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
            .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
            .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
            .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
            .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
            .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
            .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyTowerDto, Survey_tower>()
              .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
              .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
              .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
              .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
              .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
              .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
              .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
              .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
              .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
              .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
              .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
              .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
              .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<DesignTowerGisDto, Design_tower>()
            .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
            .ForMember(x => x.Is_tension, opt => opt.MapFrom(s => s.IsTension))
            .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
            .ForMember(x => x.Link_num, opt => opt.MapFrom(s => s.LinkNum))
            .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
            .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
            .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
            .ForMember(x => x.Loop_num, opt => opt.MapFrom(s => s.LoopNum))
            .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
            .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
            .ForMember(x => x.Pole_type_code, opt => opt.MapFrom(s => s.PoleTypeCode))
            .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DesignCableDto, Design_cable>()
           .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<DismantleCableDto, Dismantle_cable>()
           .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyCableDto, Survey_cable>()
           .ForMember(x => x.Electrified_work, opt => opt.MapFrom(s => s.ElectrifiedWork))
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyCableChannelDto, Survey_cable_channel>()
           .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
           .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
           .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
           .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
                  .ForMember(x => x.Duct_spec, opt => opt.MapFrom(s => s.DuctSpec))

           .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DesignCableChannelDto, Design_cable_channel>()
          .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
                 .ForMember(x => x.Duct_spec, opt => opt.MapFrom(s => s.DuctSpec))
          .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
          .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
          .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
          .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
          .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
          .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
          .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<DismantleCableChannelDto, Dismantle_cable_channel>()
            .ForMember(x => x.Lay_mode, opt => opt.MapFrom(s => s.LayMode))
                   .ForMember(x => x.Duct_spec, opt => opt.MapFrom(s => s.DuctSpec))
            .ForMember(x => x.Rsv_width, opt => opt.MapFrom(s => s.RsvWidth))
            .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
            .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
            .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyTowerDto, Survey_cable_equipment>()
            .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.Mode))
            .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.ModeId))
            .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
            .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
            .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
            .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
            .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
            .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DismantleTowerDto, Dismantle_cable_equipment>()
           .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.Mode))
           .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.ModeId))
           .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
           .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
           .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
           .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
           .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
           .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId));
            CreateMap<DesignTowerGisDto, Design_cable_equipment>()
                      .ForMember(x => x.Equip_model, opt => opt.MapFrom(s => s.Mode))
                      .ForMember(x => x.Equip_model_id, opt => opt.MapFrom(s => s.ModeId))
                      .ForMember(x => x.Logic_code, opt => opt.MapFrom(s => s.LogicCode))
                      .ForMember(x => x.Parent_id, opt => opt.MapFrom(s => s.ParentId))
                      .ForMember(x => x.Pre_node_type, opt => opt.MapFrom(s => s.PreNodeType))
                      .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                      .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
                      .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyOverHeadDeviceDto, Survey_over_head_device>()
               .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
               .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
               .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
               .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DismantleOverHeadDeviceDto, Dismantle_over_head_device>()
               .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
               .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
               .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
               .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
               .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
               .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<DesignOverHeadDeviceDto, Design_over_head_device>()
              .ForMember(x => x.Fix_mode, opt => opt.MapFrom(s => s.FixMode))
              .ForMember(x => x.Main_id, opt => opt.MapFrom(s => s.MainId))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Sub_id, opt => opt.MapFrom(s => s.SubId))
              .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
              .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
              .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
              .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<SurveyLineDto, Survey_line>()
    .ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
    .ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
    .ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
    .ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
    .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
    .ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
    .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
    .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
    .ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
    .ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
    .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
    .ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
    .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
    .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
    .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
    .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
    .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
    .ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints)).ReverseMap();

            CreateMap<DismantleLineDto, Dismantle_line>()
.ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
.ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
.ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
.ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
.ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
.ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
.ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
.ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
.ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
.ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
.ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
.ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
.ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
.ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints)).ReverseMap();
            CreateMap<DesignLineDto, Design_line>()
  .ForMember(x => x.Cable_number, opt => opt.MapFrom(s => s.CableNumber))
  .ForMember(x => x.End_node_type, opt => opt.MapFrom(s => s.EndNodeType))
  .ForMember(x => x.Group_id, opt => opt.MapFrom(s => s.GroupId))
  .ForMember(x => x.Is_cable, opt => opt.MapFrom(s => s.IsCable))
  .ForMember(x => x.Kv_level, opt => opt.MapFrom(s => s.KvLevel))
  .ForMember(x => x.Loop_direction, opt => opt.MapFrom(s => s.LoopDirection))
  .ForMember(x => x.Loop_level, opt => opt.MapFrom(s => s.LoopLevel))
  .ForMember(x => x.Loop_name, opt => opt.MapFrom(s => s.LoopName))
  .ForMember(x => x.Loop_seq, opt => opt.MapFrom(s => s.LoopSeq))
  .ForMember(x => x.Loop_serial, opt => opt.MapFrom(s => s.LoopSerial))
  .ForMember(x => x.Mode_id, opt => opt.MapFrom(s => s.ModeId))
  .ForMember(x => x.Parent_loop_name, opt => opt.MapFrom(s => s.ParentLoopName))
  .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
  .ForMember(x => x.Start_id, opt => opt.MapFrom(s => s.StartId))
  .ForMember(x => x.Start_node_type, opt => opt.MapFrom(s => s.StartNodeType))
  .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
  .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId))
  .ForMember(x => x.Turning_points, opt => opt.MapFrom(s => s.TurningPoints)).ReverseMap();

            CreateMap<SurveyMarkDto, Survey_mark>()
     .ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
     .ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
     .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
     .ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
     .ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
     .ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<DismantleMarkDto, Dismantle_mark>()
.ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
.ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();
            CreateMap<DesignMarkDto, Design_mark>()
.ForMember(x => x.Line_kv_level, opt => opt.MapFrom(s => s.LineKvLevel))
.ForMember(x => x.Live_cross, opt => opt.MapFrom(s => s.LiveCross))
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Road_level, opt => opt.MapFrom(s => s.RoadLevel))
.ForMember(x => x.Survey_time, opt => opt.MapFrom(s => s.SurveyTime))
.ForMember(x => x.Symbol_id, opt => opt.MapFrom(s => s.SymbolId)).ReverseMap();

            CreateMap<SurveyTrackDto, Survey_track>()
.ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
.ForMember(x => x.Record_date, opt => opt.MapFrom(s => s.RecordDate)).ReverseMap();

            CreateMap<DismantleTrackDto, Dismantle_track>()
                .ForMember(x => x.Project_id, opt => opt.MapFrom(s => s.ProjectId))
                .ForMember(x => x.Record_date, opt => opt.MapFrom(s => s.RecordDate)).ReverseMap();
//            CreateMap<DesignBraceDto, Design_brace>()
//.ForMember(x => x.Lat_wgs84, opt => opt.MapFrom(s => s.LatWgs84))
//.ForMember(x => x.Lon_wgs84, opt => opt.MapFrom(s => s.LonWgs84))
//.ForMember(x => x.Main_Id, opt => opt.MapFrom(s => s.MainId))
//.ForMember(x => x.D_mode, opt => opt.MapFrom(s => s.DMode))
//.ForMember(x => x.D_modeid, opt => opt.MapFrom(s => s.DModeId));
//            CreateMap<DismantleBraceDto, Dismantle_brace>()
//.ForMember(x => x.Lat_wgs84, opt => opt.MapFrom(s => s.LatWgs84))
//.ForMember(x => x.Lon_wgs84, opt => opt.MapFrom(s => s.LonWgs84))
//.ForMember(x => x.Main_Id, opt => opt.MapFrom(s => s.MainId))
//.ForMember(x => x.D_mode, opt => opt.MapFrom(s => s.DMode))
//.ForMember(x => x.D_modeid, opt => opt.MapFrom(s => s.DModeId));
//            CreateMap<PlanBraceDto, Plan_brace>()
//.ForMember(x => x.Lat_wgs84, opt => opt.MapFrom(s => s.LatWgs84))
//.ForMember(x => x.Lon_wgs84, opt => opt.MapFrom(s => s.LonWgs84))
//.ForMember(x => x.Main_Id, opt => opt.MapFrom(s => s.MainId))
//.ForMember(x => x.D_mode, opt => opt.MapFrom(s => s.DMode))
//.ForMember(x => x.D_modeid, opt => opt.MapFrom(s => s.DModeId));
//            CreateMap<SurveyBraceDto, Survey_brace>()
//.ForMember(x => x.Lat_wgs84, opt => opt.MapFrom(s => s.LatWgs84))
//.ForMember(x => x.Lon_wgs84, opt => opt.MapFrom(s => s.LonWgs84))
//.ForMember(x => x.Main_Id, opt => opt.MapFrom(s => s.MainId))
//.ForMember(x => x.D_mode, opt => opt.MapFrom(s => s.DMode))
//.ForMember(x => x.D_modeid, opt => opt.MapFrom(s => s.DModeId));
            #endregion


        }

    }

}
