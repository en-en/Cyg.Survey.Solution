using Cyg.Applicatio.Entity;
using Cyg.Extensions;
using Cyg.Extensions.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Cyg.Resource.Dto.Request;
using Cyg.Resource.Dto.Response;
using Cyg.Applicatio.Dto.Response.SymbolId;
using Cyg.Resource.Enums;
using Cyg.Applicatio.Dto.Request.SymbolId;
using Newtonsoft.Json;
using Cyg.Applicatio.Dto;
using System.Data.Common;
using Cyg.Applicatio.Service.Impl.Internal;
using Survey.Dto.Response.DbDto;
using GisModel;
using Cyg.Applicatio.Survey.Dto.Request.Project.Gis;
using Cyg.Applicatio.Survey.Dto.Response.Project;
using Cyg.Applicatio.Survey.Dto.Response.Project.Design;

namespace Cyg.Applicatio.Service.Impl
{
    partial class ProjectService : BaseService, IProjectService
    {
     

        /// <summary>
        /// 上报勘测数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task ReportSurveyGisDataAsync(Survey.Dto.ReportSurveyGisDataRequest request)
        {
          
            using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectLockKey, request.ProjectId), 5))
            {
                //获取项目信息，项目状态，工程信息
                var (project, projectState, engineer) = await CheckSurveyPermissionAsync(request.ProjectId);
              
                CheckProjectState(projectState, Dto.ProjectStatus.未勘察, Dto.ProjectStatus.勘察中);

                //上传数据为空，不允许上传
                if (!request.SurveyData.Towers.Any() &&
                    !request.SurveyData.Cables.Any() &&
                    !request.SurveyData.CableChannels.Any() &&
                    !request.SurveyData.CableEquipments.Any() &&
                    !request.SurveyData.OverheadEquipments.Any() &&
                    !request.SurveyData.Lines.Any() &&
                    !request.SurveyData.Marks.Any())
                {
                    throw new BusinessException(ResponseErrorCode.BusinessError, "上传勘察数据不允许为空");
                }
                await SyncPostgresqlData(request, engineer);
                await SyncMysqlData(request);
                //修改app上传时间
                await DbHelper.UpdateAsync<ProjectState>(it => it.Id == request.ProjectId, new
                {
                    AppUploadTime = DateTime.Now,
                });
                //更新状态
                if (projectState.Status != ProjectStatus.勘察中)
                {
                    await ChangeProjectStateAsync(project.Id, ProjectStatusType.设计状态, ProjectStatus.勘察中.ToInt(), "上报勘测数据");
                }
                //发送消息通知webgis刷新界面
                Task.Run(() => { _socketService.SendMessage(project.Id); });

            }
        }


        /// <summary>
        /// 获取工程明细
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<Survey.Dto.ProjectDetailGisResponse> GetGisDetailAsync(string projectId)
        {
            var result = new Survey.Dto.ProjectDetailGisResponse();
            try
            {
                await CheckSurveyPermissionAsync(projectId);
                result = await _repository.GetGisDetailAsync(projectId);
                //获取勘察，设计数据
                using (var work = FSql.CreateUnitOfWork())
                {
                    try
                    {
                        result.SurveyData.Towers = FSql.Queryable<Survey_tower>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanTowerGisDto>();
                        result.SurveyData.CableChannels = FSql.Queryable<Survey_cable_channel>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanCableChannelDto>();
                        result.SurveyData.CableEquipments = FSql.Queryable<Survey_cable_equipment>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanCableEquipmentDto>();
                        result.SurveyData.Cables = FSql.Queryable<Survey_cable>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanCableDto>();
                        result.SurveyData.Lines = FSql.Queryable<Survey_line>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanLineDto>();
                        result.SurveyData.Transformers = FSql.Queryable<Survey_transformer>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanTransformerDto>();
                        result.SurveyData.Marks = FSql.Queryable<Survey_mark>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanMarkDto>();
                        result.SurveyData.OverheadEquipments = FSql.Queryable<Survey_over_head_device>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanOverHeadDeviceDto>();
                        result.SurveyData.Medias = DbHelper.FindAll<SurveyMedia>(x=>x.ProjectId == projectId).ToList().MapToList<Survey.Dto.MediaRequest>();
                      

                        result.PlanData.Towers = FSql.Queryable<Plan_tower>().Where(x => x.Project_id == projectId).ToList().MapToList <PlanTowerGisDto>();
                        result.PlanData.CableChannels = FSql.Queryable<Plan_cable_channel>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanCableChannelDto>();
                        result.PlanData.CableEquipments = FSql.Queryable<Plan_cable_equipment>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanCableEquipmentDto>();
                        result.PlanData.Cables = FSql.Queryable<Plan_cable>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanCableDto>();
                        result.PlanData.Lines = FSql.Queryable<Plan_line>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanLineDto>();
                        result.PlanData.Transformers = FSql.Queryable<Plan_transformer>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanTransformerDto>();
                        result.PlanData.Marks = FSql.Queryable<Plan_mark>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanMarkDto>();
                        result.PlanData.OverheadEquipments = FSql.Queryable<Plan_over_head_device>().Where(x => x.Project_id == projectId).ToList().MapToList<PlanOverHeadDeviceDto>();
                        result.PlanData.Medias = DbHelper.FindAll<PlanMedia>(x => x.ProjectId == projectId).ToList().MapToList<Survey.Dto.MediaRequest>();

                        result.DisclosureData = await _repository.GetDisclosureDataResponseAsync(projectId);


                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                    return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取设计拆除数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<DesignDetailGisResponse> GetGisDesignDetailAsync(string projectId)
        {
            var result = new DesignDetailGisResponse();
            var rtn= await _repository.GetGisDetailAsync(projectId);
            result.Project = rtn.Project;
            try
            {
                using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectLockKey, projectId), 5))
                {

                    await CheckSurveyPermissionAsync(projectId);
                    var (project, projectState, _) = await CheckSurveyPermissionAsync(projectId);
                     CheckProjectState(projectState, ProjectStatus.设计完成);
                    //获取勘察，设计数据
                    using (var work = FSql.CreateUnitOfWork())
                    {
                        var tran = work.GetOrBeginTransaction();
                        try
                        {
                            result.DesignData = new DesignCollectionGisDataResponse();
                            result.DesignData.Towers = FSql.Queryable<Design_tower>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignTowerGisDto>();
                            result.DesignData.Cables = FSql.Queryable<Design_cable>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignCableDto>();
                            result.DesignData.CableChannels = FSql.Select<Design_cable_channel>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignCableChannelDto>(); ;
                            result.DesignData.CableChannelProfiles = FSql.Select<Design_cable_channel_profile>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignCableChannelProfileDto>();
                            result.DesignData.Cableheads = FSql.Select<Design_cable_head>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignCableHeadDto>();
                            result.DesignData.CableEquipments = FSql.Select<Design_cable_equipment>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignCableEquipmentDto>();
                            result.DesignData.OverheadEquipments = FSql.Select<Design_over_head_device>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignOverHeadDeviceDto>();
                            result.DesignData.Lines = FSql.Select<Design_line>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignLineDto>();
                            result.DesignData.Marks = FSql.Select<Design_mark>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignMarkDto>();
                            result.DesignData.Meters = FSql.Select<Design_electric_meter>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignElectricMeterDto>();
                            result.DesignData.Materials = FSql.Select<Design_material_info>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignMaterialInfoDto>();
                            result.DesignData.UserLines = FSql.Select<Design_user_line>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignUserLineDto>();
                            result.DesignData.CrossArms = FSql.Select<Design_cross_arm>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignCrossArmDto>();
                            result.DesignData.PullLineRequests = FSql.Select<Design_pull_line>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DesignPullLineDto>();
                            // result.DesignData.Braces = FSql.Select<Design_brace>().Where(x => x.ProjectId == projectId).WithTransaction(tran)..ToList().MapToList<DesignBraceDto>>();
                            result.DesignData.Medias = DbHelper.FindAll<DesignMedia>(x => x.ProjectId == projectId).ToList().MapToList<Survey.Dto.MediaRequest>();

                            result.DismantleData = new DismantleCollectionGisDataResponse();
                            result.DismantleData.TowerList = FSql.Queryable<Dismantle_tower>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleTowerDto>();
                            result.DismantleData.CableList = FSql.Queryable<Dismantle_cable>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleCableDto>();
                            result.DismantleData.CableChannelList = FSql.Select<Dismantle_cable_channel>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleCableChannelDto>(); ;
                            result.DismantleData.CableChannelProfileList = FSql.Select<Dismantle_cable_channel_profile>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleCableChannelProfileDto>();
                            result.DismantleData.CableHeadList = FSql.Select<Dismantle_cable_head>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleCableHeadDto>();
                            result.DismantleData.CableEquipmentList = FSql.Select<Dismantle_cable_equipment>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleCableEquipmentDto>();
                            result.DismantleData.OverheadDeviceList = FSql.Select<Dismantle_over_head_device>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleOverHeadDeviceDto>();
                            result.DismantleData.LineList = FSql.Select<Dismantle_line>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleLineDto>();
                            result.DismantleData.MarkList = FSql.Select<Dismantle_mark>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleMarkDto>();
                            result.DismantleData.ElectricMeterList = FSql.Select<Dismantle_electric_meter>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleElectricMeterDto>();
                            result.DismantleData.MaterialInfoList = FSql.Select<Dismantle_material_info>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleMaterialInfoDto>();
                            result.DismantleData.UserLineList = FSql.Select<Dismantle_user_line>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleUserLineDto>();
                            result.DismantleData.CrossArmList = FSql.Select<Dismantle_cross_arm>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantleCrossArmDto>();
                            result.DismantleData.PullLineList = FSql.Select<Dismantle_pull_line>().Where(x => x.Project_id == projectId).WithTransaction(tran).ToList().MapToList<DismantlePullLineDto>();
                            // result.DismantleData.BraceList = FSql.Select<Dismantle_brace>().Where(x => x.ProjectId == projectId).WithTransaction(tran).MapTo<List<DismantleBraceDto>>();
                            result.DesignData.Medias = DbHelper.FindAll<DismantleMedia>(x => x.ProjectId == projectId).ToList().MapToList<Survey.Dto.MediaRequest>();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        ///     同步mysql数据
        /// </summary>
        private async Task SyncMysqlData(Survey.Dto.ReportSurveyGisDataRequest request)
        {
            try
            {
                //清除多媒体数据
                await DbHelper.DeleteAsync<SurveyMedia>(it => it.ProjectId == request.ProjectId);
                await DbHelper.DeleteAsync<PlanMedia>(it => it.ProjectId == request.ProjectId);

                //写入多媒体数据
                var _surveyMedias = request.SurveyData.Medias?.MapToList<SurveyMedia>();
                if (_surveyMedias.HasVal())
                {
                    _surveyMedias.ForEach(m => m.CreatedBy = CurrentUser.Id);
                    await DbHelper.BulkInsertAsync(_surveyMedias);
                }
                var _PlanMedias = request.PlanData.Medias?.MapToList<PlanMedia>();
                if (_PlanMedias.HasVal())
                {
                    _PlanMedias.ForEach(m => m.CreatedBy = CurrentUser.Id);
                    await DbHelper.BulkInsertAsync(_PlanMedias);
                }

                //写入轨迹数据
                await DbHelper.DeleteAsync<ProjectTrackRecord>(it => it.ProjectId == request.ProjectId);
                if (request.SurveyData.TrackRecords.HasVal())
                {
                    var tracks = request.SurveyData.TrackRecords.MapToList<ProjectTrackRecord>();
                    tracks.ForEach(it =>
                    {
                        it.ProjectId = request.ProjectId;
                        it.RecordType = ProjectTraceRecordType.勘察;
                    });
                    await DbHelper.BulkInsertAsync(tracks);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        ///     同步postgresql数据
        /// </summary>
        private async Task SyncPostgresqlData(Survey.Dto.ReportSurveyGisDataRequest request, Engineer engineer)
        {
            try
            {
                #region 资源库信息获取
                #region 导线选型

                //勘察数据导线选型
                request.SurveyData.Lines.ForEach(l => l.ModeId = l.Mode);

                //获取线材视图模型
                GetBatchLinesRequest getBatchLinesRequest = new GetBatchLinesRequest();
                getBatchLinesRequest.BatchLines = new List<BatchLinesRequest>();
                foreach (var group in request.SurveyData.Lines.GroupBy(l => l.ModeId))
                {
                    foreach (var line in group)
                    {
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            MaterialID = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                    }
                }
                //预设数据导线选型
                request.PlanData.Lines.ForEach(l => l.ModeId = l.Mode);
                foreach (var group in request.PlanData.Lines.GroupBy(l => l.ModeId))
                {
                    foreach (var line in group)
                    {
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            MaterialID = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                    }
                }
                #endregion

                //获取资源库线材信息
                var lineViews = await _apiClient.ExecuteAsync<List<BatchLineViewResponse>>(HttpMethod.Post, specifiedLinesUrl, getBatchLinesRequest);
                foreach (var line in request.PlanData.Lines)
                {
                    var lineView = lineViews.FirstOrDefault(y => y.LineID == line.Id);
                    if (lineView == null) continue;
                    line.ModeId = lineView.MaterialID;
                    line.Mode = lineView.Spec;
                    line.Type = lineView.MaterialName;
                }
                foreach (var line in request.SurveyData.Lines)
                {
                    var lineView = lineViews.FirstOrDefault(y => y.LineID == line.Id);
                    if (lineView == null) continue;
                    line.ModeId = lineView.MaterialID;
                    line.Mode = lineView.Spec;
                    line.Type = lineView.MaterialName;
                }

                //获取杆型方案
                var towers = new List<PlanTowerGisDto>();
                towers.AddRange(request.SurveyData?.Towers);
                towers.AddRange(request.PlanData?.Towers);
                var towerModules = await TowerModules(towers, engineer.LibId);

                //获取电缆井数据 
                var cables = new List<PlanCableDto>();
                cables.AddRange(request.SurveyData.Cables);
                cables.AddRange(request.PlanData.Cables);
                var cableModules = await CableWellResponses(cables, engineer.LibId);

                //获取电缆通道数据
                var cableChannels = new List<PlanCableChannelDto>();
                cableChannels.AddRange(request.SurveyData.CableChannels);
                cableChannels.AddRange(request.PlanData.CableChannels);
                var cableChannelModules = await CableChannelResponses(cableChannels, engineer.LibId);
                //获取电气设备数据
                var cableEquipments = new List<PlanCableEquipmentDto>();
                cableEquipments.AddRange(request.SurveyData.CableEquipments);
                cableEquipments.AddRange(request.PlanData.CableEquipments);
                var cableEquipmentModules = await CableEquipmentResponses(cableEquipments, engineer.LibId);

                var symbolIdsRequest = new List<BatchTowerViewResponse>();
                symbolIdsRequest.AddRange(towerModules);
                //获取型号id
                SymbolIdResponse getSymbolIds = await GetSymbolIds(request, symbolIdsRequest);
                //获取杆上设备型号
                var overheadEquipmentRequests = new List<PlanTransformerDto>();
                overheadEquipmentRequests.AddRange(request.SurveyData?.Transformers);
                overheadEquipmentRequests.AddRange(request.PlanData?.Transformers);
                var transformers = await GetTransformer(overheadEquipmentRequests, towers, engineer.LibId);
                #endregion


                    var work = FSql.CreateUnitOfWork();
                    var tran = work.GetOrBeginTransaction();
                    try
                    {
                       await ClearProjectInfo(request.ProjectId, tran);
                       await SyncData(
                       tran, request,
                       towerModules,
                       request.ProjectId,
                       cableChannelModules,
                       cableModules,
                       cableEquipmentModules,
                       getSymbolIds,
                       transformers);
                        //写入拉线数据
                        await InsertPullLineData(request, towerModules, engineer.LibId,tran);
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  清除项目相关信息
        /// </summary>
        /// <param name="project"></param>
        /// <param name="tran"></param>
        private async Task ClearProjectInfo(string project,DbTransaction tran)
        {
            try
            {
                await FSql.Delete<Survey_tower>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_track>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_cable>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_cable_channel>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_cable_equipment>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_line>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_mark>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_pull_line>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_over_head_device>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Survey_transformer>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();

                await FSql.Delete<Plan_tower>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Plan_cable>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Plan_cable_channel>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Plan_cable_equipment>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Plan_line>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Plan_mark>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Plan_pull_line>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Plan_over_head_device>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
                await FSql.Delete<Plan_transformer>().Where(x => x.Project_id == project).WithTransaction(tran).ExecuteAffrowsAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     同步数据
        /// </summary>
        private async Task  SyncData(
            DbTransaction tran,
            Survey.Dto.ReportSurveyGisDataRequest data,
            List<BatchTowerViewResponse> towerModules,
            string projectId,
            List<CableChannelResponse> cableChannelModules,
            List<CableWellResponse> cableModules,
            List<ComponentResponse> componentResponses,
             SymbolIdResponse symbolIds,
             List<TransformerResponse> transformerResponses
            )
        {
            try
            {
                #region survey
                var request = data.SurveyData;
                #region Tower
                if (request.Towers.HasVal())
                {
                    foreach (var item in request.Towers)
                    {
                        try
                        {
                            #region tower
                            var tower = item.MapTo<Survey_tower>();
                            tower.Surveyor = CurrentUser.Id;
                            if (tower.Kv_level == (int)KVLevel.V380 || tower.Kv_level == (int)KVLevel.V220)
                            {
                                tower.Sort = (int)Arrangement.水平;
                            }
                            //获取资源库杆塔方案
                            var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                            if (module == null) module = new BatchTowerViewResponse();
                            var symbolId = symbolIds.TowerResponse.FirstOrDefault(x => x.Id == tower.Id)?.SymbolId;
                            //设置杆塔材质，杆塔方案id
                            tower.Material = module?.Material;
                            tower.Mode = module?.ModuleName;
                            tower.Type = module?.Type;
                            tower.Pole_type_code = item.PoleTypeCode;
                            tower.Mode_id = module?.ModuleID;
                            tower.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                            tower.Depth = module.Depth;
                            tower.Loop_name = module.LoopNumber;
                            tower.Loop_num = "0";
                            tower.Loop_level = "0";
                            tower.Is_tension = (short)(module.IsTension);
                            await FSql.Insert(tower).WithTransaction(tran).ExecuteAffrowsAsync();
                            #endregion
                        }
                        catch (Exception)
                        {
                            throw new BusinessException($"杆塔:{item.Code}必填项为空，请检查数据!!");
                        }
                     
                    }
                   
                }
                #endregion

                #region Cable
                if (request.Cables.HasVal())
                {

                    foreach (var cable in request.Cables)
                    {

                        try
                        {
                            var symbolId = symbolIds.CableSymbolIdResponse.FirstOrDefault(x => x.Id == cable.Id)?.SymbolId;
                            cable.Surveyor = CurrentUser.Id;
                            //获取资源库电缆井
                            var module = cableModules.FirstOrDefault(x =>
                            x.CableWellID == cable.ModeId);
                            if (module == null) module = new CableWellResponse();
                            var _cable = cable.MapTo<Survey_cable>();
                            _cable.Mode = module.CableWellName;
                            _cable.Symbol_id = symbolId == null ? (short)0 : (short)symbolId;
                            await FSql.Insert(_cable).WithTransaction(tran).ExecuteAffrowsAsync();
                        }
                        catch (Exception)
                        {
                            throw new BusinessException($"电缆井:{cable.Code}必填项为空，请检查数据!!");
                        }
                       
                    }
                }
                #endregion

                #region CableChannel
             
                if (request.CableChannels!=null&& request.CableChannels.HasVal())
                {
                    foreach (var cableChannel in request.CableChannels)
                    {
                        var module = cableChannelModules.FirstOrDefault(x =>
                       x.ChannelID == cableChannel.ModeId);
                        if (module == null) module = new CableChannelResponse();
                        var cableChannelInfo = cableChannel.MapTo<Survey_cable_channel>();
                        cableChannelInfo.Surveyor = CurrentUser.Id;
                        cableChannelInfo.Mode = module.ChannelName;
                        cableChannelInfo.Duct_id = module.DuctMaterialID;
                        cableChannelInfo.Duct_spec = module.DuctSpec;
                        await FSql.Insert(cableChannelInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }
                #endregion

                #region CableEquipment

                if (request.CableEquipments != null && request.CableEquipments.HasVal())
                {
                    foreach (var cableEquipment in request.CableEquipments)
                    {
                        var symbolId = symbolIds.CableEquipmentSymbolIdResponse.FirstOrDefault(x => x.Id == cableEquipment.Id)?.SymbolId;
                       
                        var cableEquipmentInfo = cableEquipment.MapTo<Survey_cable_equipment>();
                        cableEquipmentInfo.Surveyor = CurrentUser.Id;
                        cableEquipmentInfo.Equip_model_id = cableEquipmentInfo.Equip_model;
                        cableEquipmentInfo.Equip_model = componentResponses.FirstOrDefault(x => x.ComponentID == cableEquipment.EquipModel)?.ComponentSpec;
                        cableEquipmentInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                        await FSql.Insert(cableEquipmentInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }
                #endregion

                #region OverheadEquipment

                if (request.OverheadEquipments != null && request.OverheadEquipments.HasVal())
                {
                    foreach (var overheadEquipment in request.OverheadEquipments)
                    {
                        var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == overheadEquipment.Id)?.SymbolId;

                        var overheadEquipmentInfo = overheadEquipment.MapTo<Survey_over_head_device>();

                        if (overheadEquipment.Type == OverheadEquipmentType.变压器)
                        {
                            overheadEquipment.Capacity = overheadEquipment.Capacity;
                        }
                        else
                        {
                            overheadEquipment.Capacity = TransformerCapacity.None;
                        }
                        var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == overheadEquipment.Id);
                        overheadEquipmentInfo.Surveyor = CurrentUser.Id;
                        overheadEquipmentInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                        overheadEquipmentInfo.Mode = transformerResponse?.ComponentSpec;
                        overheadEquipmentInfo.Mode_id = transformerResponse?.ComponentID;
                        await FSql.Insert(overheadEquipmentInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }
                #endregion

                #region Line
                if (request.Lines != null && request.Lines.HasVal())
                {
                    foreach (var line in request.Lines)
                    {
                        var symbolId = symbolIds.LineResponse.FirstOrDefault(x => x.Id == line.Id)?.SymbolId;

                        var lineInfo = line.MapTo<Survey_line>();
                        lineInfo.Surveyor = CurrentUser.Id;
                        lineInfo.Index = (short)line.LoopSerial;
                        lineInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                        await FSql.Insert(lineInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }
                #endregion

                #region Mark
                if (request.Marks != null && request.Marks.HasVal())
                {
                    foreach (var mark in request.Marks)
                    {
                        var markInfo = mark.MapTo<Survey_mark>();
                        markInfo.Surveyor = CurrentUser.Id;
                        await FSql.Insert(markInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }
                #endregion

                #region transformer
                if (request.Transformers != null && request.Transformers.HasVal())
                {
                    foreach (var transformerDto in request.Transformers)
                    {
                        var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == transformerDto.Id)?.SymbolId;

                        var overheadEquipmentInfo = transformerDto.MapTo<Survey_transformer>();

                        
                        var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == transformerDto.Id);
                        overheadEquipmentInfo.Surveyor = CurrentUser.Id;
                        overheadEquipmentInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                        overheadEquipmentInfo.Mode = transformerResponse?.ComponentSpec;
                        overheadEquipmentInfo.Mode_id = transformerResponse?.ComponentID;
                        await FSql.Insert(overheadEquipmentInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }

                #endregion

                #region
                if (request.TrackRecords != null && request.TrackRecords.HasVal())
                {
                    foreach (var trackRecord in request.TrackRecords)
                    {
                        var trackInfo = trackRecord.MapTo<Survey_track>();
                        trackInfo.Id = Guid.NewGuid().ToString();
                        trackInfo.Recorder = CurrentUser.Id;
                        trackInfo.Geom  = $"POINT({(double)trackRecord.Lon_WGS84} {(double)trackRecord.Lat_WGS84})";
                        trackInfo.Type = 1;
                        trackInfo.Company = CurrentUser.CompanyId;
                        trackInfo.Project_id = projectId;
                         await FSql.Insert(trackInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }
                #endregion

                #endregion
                #region Plan
                request = data.PlanData;
                #region Tower
                if (request.Towers.HasVal())
                {
                    foreach (var item in request.Towers)
                    {
                        try
                        {
                            #region towerInfo
                            var towerInfo = item.MapTo<Plan_tower>();
                            towerInfo.Surveyor = CurrentUser.Id;
                            if (towerInfo.Kv_level == (int)KVLevel.V380 || towerInfo.Kv_level == (int)KVLevel.V220)
                            {
                                towerInfo.Sort = (int)Arrangement.水平;
                            }
                            //获取资源库杆塔方案
                            var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                            if (module == null) module = new BatchTowerViewResponse();
                            var symbolId = symbolIds.TowerResponse.FirstOrDefault(x => x.Id == towerInfo.Id)?.SymbolId;
                            //设置杆塔材质，杆塔方案id
                            towerInfo.Material = module?.Material;
                            towerInfo.Mode = module?.ModuleName;
                            towerInfo.Type = module?.Type;
                            towerInfo.Pole_type_code = item.PoleTypeCode;
                            towerInfo.Mode_id = module?.ModuleID;
                            towerInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                            towerInfo.Depth = module.Depth;
                            towerInfo.Loop_name = module.LoopNumber;
                            towerInfo.Loop_num = "0";
                            towerInfo.Loop_level = "0";
                            towerInfo.Is_tension = (short)(module.IsTension);
                            await FSql.Insert(towerInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                            #endregion
                        }
                        catch (Exception)
                        {

                            throw new BusinessException($"杆塔:{item.Code}必填项为空，请检查数据!!");
                        }
                    }
                }
                #endregion

                #region Cable
                if (request.Cables.HasVal())
                {

                    foreach (var cable in request.Cables)
                    {
                        try
                        {
                            var symbolId = symbolIds.CableSymbolIdResponse.FirstOrDefault(x => x.Id == cable.Id)?.SymbolId;
                            cable.Surveyor = CurrentUser.Id;
                            //获取资源库电缆井
                            var module = cableModules.FirstOrDefault(x =>
                            x.CableWellID == cable.ModeId);
                            if (module == null) module = new CableWellResponse();
                            var _cable = cable.MapTo<Plan_cable>();
                            _cable.Mode = module.CableWellName;
                            _cable.Pre_node_type = (int)cable.PreNodeType;
                            _cable.Symbol_id = symbolId == null ? (short)0 : (short)symbolId;
                            await FSql.Insert(_cable).WithTransaction(tran).ExecuteAffrowsAsync();
                        }
                        catch (Exception)
                        {
                            throw new BusinessException($"电缆井:{cable.Code}必填项为空，请检查数据!!");
                        }
                    }
                }
                #endregion

                #region CableChannel

                if (request.CableChannels != null && request.CableChannels.HasVal())
                {
                    foreach (var cableChannel in request.CableChannels)
                    {
                        var module = cableChannelModules.FirstOrDefault(x =>
                       x.ChannelID == cableChannel.ModeId);
                        if (module == null) module = new CableChannelResponse();
                        var cableChannelInfo = cableChannel.MapTo<Plan_cable_channel>();
                        cableChannelInfo.Surveyor = CurrentUser.Id;
                        cableChannelInfo.Mode = module.ChannelName;
                        cableChannelInfo.Duct_id = module.DuctMaterialID;
                        cableChannelInfo.Duct_spec = module.DuctSpec;
                        await FSql.Insert(cableChannelInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                      
                    }
                }
                #endregion

                #region CableEquipment

                if (request.CableEquipments != null && request.CableEquipments.HasVal())
                {
                    foreach (var cableEquipment in request.CableEquipments)
                    {
                        var symbolId = symbolIds.CableEquipmentSymbolIdResponse.FirstOrDefault(x => x.Id == cableEquipment.Id)?.SymbolId;

                        var cableEquipmentInfo = cableEquipment.MapTo<Plan_cable_equipment>();
                        cableEquipmentInfo.Surveyor = CurrentUser.Id;
                        cableEquipmentInfo.Equip_model_id = cableEquipmentInfo.Equip_model;
                        cableEquipmentInfo.Equip_model = componentResponses.FirstOrDefault(x => x.ComponentID == cableEquipment.EquipModel)?.ComponentSpec;
                        cableEquipmentInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                        await FSql.Insert(cableEquipmentInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                        
                    }
                }
                #endregion

                #region OverheadEquipment

                if (request.OverheadEquipments != null && request.OverheadEquipments.HasVal())
                {
                    foreach (var overheadEquipment in request.OverheadEquipments)
                    {
                        var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == overheadEquipment.Id)?.SymbolId;

                        var overheadEquipmentInfo = overheadEquipment.MapTo<Plan_over_head_device>();

                        if (overheadEquipment.Type == OverheadEquipmentType.变压器)
                        {
                            overheadEquipment.Capacity = overheadEquipment.Capacity;
                        }
                        else
                        {
                            overheadEquipment.Capacity = TransformerCapacity.None;
                        }
                        var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == overheadEquipment.Id);
                        overheadEquipmentInfo.Surveyor = CurrentUser.Id;
                        overheadEquipmentInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                        overheadEquipmentInfo.Mode = transformerResponse?.ComponentSpec;
                        overheadEquipmentInfo.Mode_id = transformerResponse?.ComponentID;
                        await FSql.Insert(overheadEquipmentInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                      
                    }
                }
                #endregion

                #region Line
                if (request.Lines != null && request.Lines.HasVal())
                {
                    foreach (var line in request.Lines)
                    {
                        var symbolId = symbolIds.LineResponse.FirstOrDefault(x => x.Id == line.Id)?.SymbolId;

                        var lineInfo = line.MapTo<Plan_line>();
                        lineInfo.Surveyor = CurrentUser.Id;
                        lineInfo.Index = (short)line.LoopSerial;
                        lineInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                        await FSql.Insert(lineInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }
                #endregion

                #region Mark
                if (request.Marks != null && request.Marks.HasVal())
                {
                    foreach (var mark in request.Marks)
                    {
                        var markInfo = mark.MapTo<Plan_mark>();
                        markInfo.Surveyor = CurrentUser.Id;
                        await FSql.Insert(markInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }
                #endregion

                #region transformer
                if (request.Transformers != null && request.Transformers.HasVal())
                {
                    foreach (var transformerDto in request.Transformers)
                    {
                        var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == transformerDto.Id)?.SymbolId;

                        var overheadEquipmentInfo = transformerDto.MapTo<Plan_transformer>();
                        var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == transformerDto.Id);
                        overheadEquipmentInfo.Surveyor = CurrentUser.Id;
                        overheadEquipmentInfo.Symbol_id = symbolId == null ? (short)0 : (short)(symbolId);
                        overheadEquipmentInfo.Mode = transformerResponse?.ComponentSpec;
                        overheadEquipmentInfo.Mode_id = transformerResponse?.ComponentID;
                        await FSql.Insert(overheadEquipmentInfo).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
                }

                #endregion

                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 添加多媒体数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task AddMediasInfoAsync(AddMediasInfoRequest request)
        {
            try
            {
                //写入多媒体数据
                var _surveyMedias = request.SurveyMedias?.MapToList<SurveyMedia>();
                if (_surveyMedias!=null&&_surveyMedias.HasVal())
                {
                    _surveyMedias.ForEach(m => m.CreatedBy = CurrentUser.Id);
                    await DbHelper.BulkInsertAsync(_surveyMedias);
                }
                var _PlanMedias = request.PlanMedias?.MapToList<PlanMedia>();
                if (_PlanMedias!=null&&_PlanMedias.HasVal())
                {
                    _PlanMedias.ForEach(m => m.CreatedBy = CurrentUser.Id);
                    await DbHelper.BulkInsertAsync(_PlanMedias);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("添加多媒体数据异常");
            }
        }




        #region 资源api获取

        /// <summary>
        ///     获取杆塔方案
        /// </summary>
        /// <param name="towers"></param>
        /// <param name="resourceLibID"></param>
        /// <returns></returns>
        private async Task<List<BatchTowerViewResponse>> TowerModules(List<PlanTowerGisDto> towers, string resourceLibID)
        {

            var rtn = new List<BatchTowerViewResponse>();
            if (towers == null || towers.Count == 0) return rtn;
            try
            {
                if (towers.HasVal())
                {
                    var request = new GetBatchModuleQueryRequest();
                    request.BatchModuleQueries = new List<BatchModuleQueryRequest>();
                    foreach (var item in towers)
                    {

                        try
                        {
                            request.BatchModuleQueries.Add(
                                                       new BatchModuleQueryRequest
                                                       {
                                                           KVLevel = item.KvLevel,
                                                           RodDiameter = Convert.ToDouble(item.Rod),
                                                           SegmentMode = ((int)item.Segment) == 0 ? "" : item.Segment.ToString(),
                                                           Height = Convert.ToDouble(item.Height),
                                                           Arrangement = ((int)item.Sort) == 0 ? "" : item.Sort.ToString(),
                                                           ResourceLibID = resourceLibID,
                                                           TowerID = item.Id,
                                                           PoleTypeCodes = new List<string> { item.PoleTypeCode }
                                                       });
                        }
                        catch (Exception)
                        {
                            throw new BusinessException($"杆塔:{item.Code}必填项为空，请检查数据!!");
                        }
                        //杆梢径
                    }
                    //获取资源库杆塔方案
                    rtn = await _apiClient.ExecuteAsync<List<BatchTowerViewResponse>>(HttpMethod.Post, towerModulesUrl, request);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
            return rtn;
        }

        /// <summary>
        ///     获取电缆井信息
        /// </summary>
        /// <param name="cables"></param>
        /// <param name="resourceLibID"></param>
        /// <returns></returns>
        private async Task<List<CableWellResponse>> CableWellResponses(List<PlanCableDto> cables, string resourceLibID)
        {
            var rtn = new List<CableWellResponse>();
            if (cables == null || cables.Count == 0) return rtn;
            try
            {
                if (cables.HasVal())
                {

                    // 获取资源库杆塔方案
                    CableWellListRequest cableWellListRequest = new CableWellListRequest();
                    cableWellListRequest.CableWellIDs = new List<string>();
                    cableWellListRequest.ResourceLibID = resourceLibID;

                    foreach (var item in cables)
                    {
                        cableWellListRequest.CableWellIDs.Add(item.ModeId);
                    }
                    rtn = await _apiClient.ExecuteAsync<List<CableWellResponse>>(HttpMethod.Post, cableWellUrl, cableWellListRequest);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
            return rtn;
        }

        /// <summary>
        ///     获取电缆通道信息
        /// </summary>
        /// <param name="cables"></param>
        /// <param name="resourceLibID"></param>
        /// <returns></returns>
        private async Task<List<CableChannelResponse>> CableChannelResponses(List<PlanCableChannelDto> cables, string resourceLibID)
        {
            var rtn = new List<CableChannelResponse>();
            if (cables == null || cables.Count == 0) return rtn;
            try
            {
                if (cables.HasVal())
                {

                    ChannelListRequest channelListRequest = new ChannelListRequest();
                    channelListRequest.ResourceLibID = resourceLibID;
                    channelListRequest.ChannelIDs = new List<string>();
                    foreach (var item in cables)
                    {
                        channelListRequest.ChannelIDs.Add(item.ModeId);
                    }
                    //获取资源库杆塔方案
                    rtn = (await _apiClient.ExecuteAsync<List<CableChannelResponse>>(HttpMethod.Post, cableChannelUrl, channelListRequest));
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
            return rtn;
        }

        /// <summary>
        ///     获取电气设备信息
        /// </summary>
        /// <param name="cables"></param>
        /// <param name="resourceLibID"></param>
        /// <returns></returns>
        private async Task<List<ComponentResponse>> CableEquipmentResponses(List<PlanCableEquipmentDto> cableEquipmentRequests, string resourceLibID)
        {
            var rtn = new List<ComponentResponse>();
            if (cableEquipmentRequests == null || cableEquipmentRequests.Count == 0) return rtn;
            try
            {
                if (cableEquipmentRequests.HasVal())
                {
                    var requestInfo = new ComponentListRequest { ResourceLibID = resourceLibID };
                    var ids = new List<string>();
                    foreach (var item in cableEquipmentRequests)
                    {
                        ids.Add(item.EquipModel);
                    }
                    requestInfo.ComponentIDs = ids.Distinct();
                    rtn = (await _apiClient.ExecuteAsync<List<ComponentResponse>>(HttpMethod.Post, cableEquipmentUrl, requestInfo));
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
            return rtn;
        }
        #endregion

        #region  SymbolId 计算

        /// <summary>
        ///     计算勘察id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="towerModules"></param>
        /// <returns></returns>
        private async Task<SymbolIdResponse> GetSymbolIds(Survey.Dto.ReportSurveyGisDataRequest request, List<BatchTowerViewResponse> towerModules)
        {
            var rtnData = new SymbolIdResponse();
            try
            {
                if (request == null) return rtnData;
                Dto.Request.SymbolId.GetSymbolIdRequest symbolIdRequest = new GetSymbolIdRequest();
                symbolIdRequest.TowerRequest = new List<GetTowerSymbolIdRequest>();
                symbolIdRequest.CableSymbolIdRequest = new List<GetSurveyCableSymbolIdRequest>();
                symbolIdRequest.CableEquipmentSymbolIdRequest = new List<GetSurveyCableEquipmentSymbolIdRequest>();
                symbolIdRequest.OverheadEquipmentRequest = new List<GeOverheadEquipmentSymbolIdRequest>();
                symbolIdRequest.LineRequest = new List<GetLineSymbolIdRequest>();
                if (request.SurveyData != null)
                {
                    if (request.SurveyData.Towers != null)
                    {
                        foreach (var item in request.SurveyData.Towers)
                        {
                            var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                            if (module != null) symbolIdRequest.TowerRequest.Add(
                               new GetTowerSymbolIdRequest
                               {
                                   Id = item.Id,
                                   State = ((int)item.State).ToEnum<SurveyState>(),
                                   Material = module.Material.ToEnum<PoleMaterial>()
                               });
                        }
                    }
                    if (request.SurveyData.Cables != null)
                    {
                        symbolIdRequest.CableSymbolIdRequest.AddRange((from item in request.SurveyData.Cables
                                                                       select new GetSurveyCableSymbolIdRequest
                                                                       {
                                                                           Id = item.Id,
                                                                           State = ((int)item.State).ToEnum<SurveyState>(),
                                                                           Type = ((int)item.Type).ToEnum<CableType>()
                                                                       }).ToList());
                    }
                    if (request.SurveyData.CableEquipments != null)
                    {
                        symbolIdRequest.CableEquipmentSymbolIdRequest.AddRange((from item in request.SurveyData.CableEquipments
                                                                                select new GetSurveyCableEquipmentSymbolIdRequest
                                                                                {
                                                                                    Id = item.Id,
                                                                                    State = ((int)item.State).ToEnum<SurveyState>(),
                                                                                    Type = ((int)item.Type).ToEnum<CableEquipmentType>()
                                                                                }).ToList());
                    }
                    if (request.SurveyData.OverheadEquipments != null)
                    {
                        symbolIdRequest.OverheadEquipmentRequest.AddRange((from item in request.SurveyData.OverheadEquipments
                                                                           select new GeOverheadEquipmentSymbolIdRequest
                                                                           {
                                                                               Id = item.Id,
                                                                               Type = ((int)item.Type).ToEnum<OverheadEquipmentType>(),
                                                                               State = ((int)item.State).ToEnum<SurveyState>(),
                                                                           }).ToList());
                      
                    }
                    if (request.SurveyData.Transformers != null)
                    {
                        symbolIdRequest.OverheadEquipmentRequest.AddRange((from item in request.SurveyData.Transformers
                                                                           select new GeOverheadEquipmentSymbolIdRequest
                                                                           {
                                                                               Id = item.Id,
                                                                               Type = OverheadEquipmentType.变压器,
                                                                               State = ((int)item.State).ToEnum<SurveyState>(),
                                                                           }).ToList());

                    }
                    if (request.SurveyData.Lines != null)
                    {
                        symbolIdRequest.LineRequest.AddRange((from item in request.SurveyData.Lines
                                                              select new GetLineSymbolIdRequest
                                                              {
                                                                  Id = item.Id,
                                                                  IsCable = item.IsCable==null?false:(bool)item.IsCable,
                                                                  KVLevel = ((int)item.KvLevel).ToEnum<KVLevel>(),
                                                                  State = ((int)item.State).ToEnum<SurveyState>(),
                                                              }).ToList());
                    }
                }
                if (request.PlanData != null)
                {
                    if (request.PlanData.Towers != null)
                    {
                        foreach (var item in request.PlanData.Towers)
                        {
                            var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                            if (module != null) symbolIdRequest.TowerRequest.Add(
                               new GetTowerSymbolIdRequest
                               {
                                   Id = item.Id,
                                   State = ((int)item.State).ToEnum<SurveyState>(),
                                   Material = module.Material.ToEnum<PoleMaterial>()
                               });
                        }
                    }
                    if (request.PlanData.Cables != null)
                    {
                        symbolIdRequest.CableSymbolIdRequest.AddRange((from item in request.PlanData.Cables
                                                                       select new GetSurveyCableSymbolIdRequest
                                                                       {
                                                                           Id = item.Id,
                                                                           State = ((int)item.State).ToEnum<SurveyState>(),
                                                                           Type = ((int)item.Type).ToEnum<CableType>()
                                                                       }).ToList());
                    }
                    if (request.PlanData.CableEquipments != null)
                    {
                        symbolIdRequest.CableEquipmentSymbolIdRequest.AddRange((from item in request.PlanData.CableEquipments
                                                                                select new GetSurveyCableEquipmentSymbolIdRequest
                                                                                {
                                                                                    Id = item.Id,
                                                                                    State = ((int)item.State).ToEnum<SurveyState>(),
                                                                                    Type = ((int)item.Type).ToEnum<CableEquipmentType>()
                                                                                }).ToList());
                    }
                    if (request.PlanData.OverheadEquipments != null)
                    {
                        symbolIdRequest.OverheadEquipmentRequest.AddRange((from item in request.PlanData.OverheadEquipments
                                                                           select new GeOverheadEquipmentSymbolIdRequest
                                                                           {
                                                                               Id = item.Id,
                                                                               Type = ((int)item.Type).ToEnum<OverheadEquipmentType>(),
                                                                               State = ((int)item.State).ToEnum<SurveyState>()
                                                                           }).ToList());
                    }
                    if (request.PlanData.Transformers != null)
                    {
                        symbolIdRequest.OverheadEquipmentRequest.AddRange((from item in request.PlanData.Transformers
                                                                           select new GeOverheadEquipmentSymbolIdRequest
                                                                           {
                                                                               Id = item.Id,
                                                                               Type = OverheadEquipmentType.变压器,
                                                                               State = ((int)item.State).ToEnum<SurveyState>(),
                                                                           }).ToList());

                    }
                    if (request.PlanData.Lines != null)
                    {
                        symbolIdRequest.LineRequest.AddRange((from item in request.PlanData.Lines
                                                              select new GetLineSymbolIdRequest
                                                              {
                                                                  Id = item.Id,
                                                                  IsCable = item.IsCable==null?false:(bool)item.IsCable,
                                                                  KVLevel = ((int)item.KvLevel).ToEnum<KVLevel>(),
                                                                  State = ((int)item.State).ToEnum<SurveyState>()
                                                              }).ToList());
                    }
                }
                rtnData = await _apiClient.ExecuteAsync<SymbolIdResponse>(HttpMethod.Post, getSymbolIdsUrl, symbolIdRequest);
            }
            catch (Exception ex)
            {

                throw new BusinessException($"勘察服务计算型号id异常{ex.Message}");
            }
            return rtnData;
        }


        #endregion

        /// <summary>
        ///     添加拉线数据
        /// </summary>
        /// <param name="towers"></param>
        private async Task InsertPullLineData(Survey.Dto.ReportSurveyGisDataRequest requestInfo, List<BatchTowerViewResponse> sourceInfo, string libId, DbTransaction tran)
        {
            try
            {
                if (sourceInfo == null || sourceInfo.Count == 0) return;
                //增加拉线数据
                var addSurveyPullInfo = new List<Survey_pull_line>();
                var addDesingnPullInfo = new List<Plan_pull_line>();

                //获取杆塔拉线数据
                GetBatchTowerComponentsRequest towerComponent = new GetBatchTowerComponentsRequest();
                towerComponent.BatchModuleComponents = new List<BatchTowerComponentsRequest>();

                foreach (var tower in sourceInfo)
                {
                    BatchTowerComponentsRequest batchTowerComponentsRequest = new BatchTowerComponentsRequest();
                    batchTowerComponentsRequest.ComponentCategories = new List<string> { "拉线" };
                    batchTowerComponentsRequest.TowerID = tower.TowerID;
                    batchTowerComponentsRequest.ObjectID = tower.ModuleID;
                    batchTowerComponentsRequest.ResourceLibID = libId;
                    towerComponent.BatchModuleComponents.Add(batchTowerComponentsRequest);
                }
                var rtn = await _apiClient.ExecuteAsync<List<BatchTowerComponentResponse>>(HttpMethod.Post, getPullLineUrl, towerComponent);
                foreach (var item in rtn)
                {
                    var curTower = requestInfo.SurveyData.Towers?.FirstOrDefault(x => x.Id == item.TowerID);
                    if (curTower != null)
                    {
                        for (int i = 0; i < item.ItemNumber; i++)
                        {
                            addSurveyPullInfo.Add(new Survey_pull_line
                            {
                                Id= Guid.NewGuid().ToString(),
                                Mode = item.ComponentSpec,
                                Mode_id = item.ComponentID,
                                Main_id = curTower.Id,
                                State = (int)curTower.State,
                                Project_id = curTower.ProjectId,
                                Lat = (float)curTower.Lat,
                                Lon = (float)curTower.Lon,
                                Geom = $"POINT({(double)curTower.Lon} {(double)curTower.Lat})",
                                Azimuth = 0
                            });
                            //addDismantlePullInfo.Add(new Dismantle_pull_line
                            //{
                            //    Id = Guid.NewGuid().ToString(),
                            //    Mode = item.ComponentSpec,
                            //    Mode_id = item.ComponentID,
                            //    Main_id = curTower.Id,
                            //    State = (int)curTower.State,
                            //    Project_id = curTower.ProjectId,
                            //    Lat = (float)curTower.Lat,
                            //    Lon = (float)curTower.Lon,
                            //    Geom = $"POINT({(double)curTower.Lon} {(double)curTower.Lat})",
                            //    Azimuth = 0
                            //});
                        }
                    }

                    curTower = requestInfo.PlanData.Towers?.FirstOrDefault(x => x.Id == item.TowerID);
                    if (curTower == null) continue;
                    for (int i = 0; i < item.ItemNumber; i++)
                    {

                        addDesingnPullInfo.Add(new Plan_pull_line
                        {
                            Id = Guid.NewGuid().ToString(),
                            Mode = item.ComponentSpec,
                            Mode_id = item.ComponentID,
                            Main_id = curTower.Id,
                            State = (int)curTower.State,
                            Project_id = curTower.ProjectId,
                            Lat = (float)curTower.Lat,
                            Lon = (float)curTower.Lon,
                            Geom= $"POINT({(double)curTower.Lon} {(double)curTower.Lat})",
                            Azimuth = 0
                        });

                    }
                }
                if (addSurveyPullInfo != null && addSurveyPullInfo.Count > 0)
                    foreach(var item in addSurveyPullInfo)
                        {
                         await FSql.Insert(item).WithTransaction(tran).ExecuteAffrowsAsync();
                        }
                //if (addDismantlePullInfo != null && addDismantlePullInfo.Count > 0)
                //    foreach (var item in addDismantlePullInfo)
                //    {
                //        await FSql.Insert(item).WithTransaction(tran).ExecuteAffrowsAsync();
                //    }
              
                if (addDesingnPullInfo!=null&&addDesingnPullInfo.Count >0)
                    foreach (var item in addDesingnPullInfo)
                    {
                        await FSql.Insert(item).WithTransaction(tran).ExecuteAffrowsAsync();
                    }
            }
            catch (Exception ex)
            {
                throw new BusinessException($"勘察端添加拉线数据异常，参数{JsonConvert.SerializeObject(sourceInfo)},错误信息{ex.Message}");
            }
        }

        #region 变压器
        /// <summary>
        ///     获取变压器组件信息
        /// </summary>
        /// <returns></returns>
        private async Task<List<TransformerResponse>> GetTransformerList(List<PlanOverHeadDeviceDto> requests, List<PlanTowerGisDto> towerRequests, string resourceLibId)
        {
            var rtn = new List<TransformerResponse>();
            if (requests == null || requests.Count == 0) return rtn;
            try
            {
                if (requests == null || requests.Count == 0) return rtn;
                //获取主杆高度
                var getInfoRequest = new GetTransformerRequest();
                getInfoRequest.ResourceLibID = resourceLibId;
                getInfoRequest.TransformerProperties = new List<TransformerPropertyRequest>();
                foreach (var item in requests)
                {
                    var curTower = towerRequests.FirstOrDefault(x => x.Id == item.MainId);
                    if (item.Capacity == null || item.Capacity== TransformerCapacity.None) continue;
                    //if (!Enum.IsDefined(typeof(TransformerCapacity), item.Capacity)) continue;
                    getInfoRequest.TransformerProperties.Add(new TransformerPropertyRequest { Capacity = item.Capacity, Height =string.IsNullOrEmpty(curTower.Height)?0:Convert.ToDouble(curTower.Height), TransformerID = item.Id });
                }
                rtn = await _apiClient.ExecuteAsync<List<TransformerResponse>>(HttpMethod.Post, getTransformerListUrl, getInfoRequest);
            }
            catch (Exception ex)
            {

                throw new BusinessException($"【勘察端】获取变压器组件信息异常,{ex.Message}");
            }
            return rtn;
        }
        #endregion


          #region 变压器
        /// <summary>
        ///     获取变压器组件信息
        /// </summary>
        /// <returns></returns>
        private async Task<List<TransformerResponse>> GetTransformer(List<PlanTransformerDto> requests, List<PlanTowerGisDto> towerRequests, string resourceLibId)
        {
            var rtn = new List<TransformerResponse>();
            if (requests == null || requests.Count == 0) return rtn;
            try
            {
                if (requests == null || requests.Count == 0) return rtn;
                //获取主杆高度
                var getInfoRequest = new GetTransformerRequest();
                getInfoRequest.ResourceLibID = resourceLibId;
                getInfoRequest.TransformerProperties = new List<TransformerPropertyRequest>();
                foreach (var item in requests)
                {
                    var curTower = towerRequests.FirstOrDefault(x => x.Id == item.MainId);
                    if (curTower == null) continue;
                    getInfoRequest.TransformerProperties.Add(new TransformerPropertyRequest { Capacity=item.Capacity,Height=Convert.ToDouble( curTower.Height),TransformerID=item.Id });
                }
                
                rtn = await _apiClient.ExecuteAsync<List<TransformerResponse>>(HttpMethod.Post, getTransformerListUrl, getInfoRequest);
            }
            catch (Exception ex)
            {

                throw new BusinessException($"【勘察端】获取变压器组件信息异常,{ex.Message}");
            }
            return rtn;
        }
        #endregion

    }
}
