using Cyg.Applicatio.Dto;
using Cyg.Applicatio.Dto.Enums;
using Cyg.Applicatio.Entity;
using Cyg.Applicatio.Repository;
using Cyg.Applicatio.Survey.Dto;
using Cyg.Extensions;
using Cyg.Extensions.AutoMapper;
using Cyg.Extensions.Redis;
using Cyg.Extensions.Service;
using Cyg.Resource.Enums;
using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cyg.WebApiClient;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Cyg.Resource.Dto.Request;
using Cyg.Resource.Dto.Response;
using CableChannelResponse = Cyg.Resource.Dto.Response.CableChannelResponse;
using Cyg.Applicatio.Dto.Response.SymbolId;
using Cyg.Applicatio.Dto.Request.SymbolId;
using Cyg.Applicatio.Entity.Survey;
using Newtonsoft.Json;
using Cyg.Applicatio.Entity.Dismantle;
using Cyg.Applicatio.Entity.Design;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Logging;
using  Cyg.Extensions;
using System.Security.Cryptography;
using System.Text;
using GisModel;
using Cyg.FreeSql;
using Microsoft.AspNetCore.Authorization;

namespace Cyg.Applicatio.Service.Impl
{
    partial class ProjectService : BaseService, IProjectService
    {
        private readonly IApiClient _apiClient;
        private readonly IProjectRepository _repository;
        private readonly IRedisCache _redisClient;
        private readonly ICapPublisher _capPublisher;
        private readonly IGisTransformService gisTransformService;
        private readonly IConfiguration _configuration;

        private  static int SuccessNum;

        private static int FailNum;

        private  static List<string> FailId;

        private  static int AllNum;

        private ISocketService _socketService;

        #region 资源库信息

        /// <summary>
        ///     批量根据杆型属性条件，从视图中获取符合要求的杆塔方案
        /// </summary>
        private string towerModulesUrl;


        /// <summary>
        ///     根据线材条件，从视图中获取符合要求的线材
        /// </summary>
        private string specifiedLinesUrl;

        /// <summary>
        ///     根据电缆井ID，获取电缆井数据
        /// </summary>
        private string cableWellUrl;

        /// <summary>
        ///     根据电缆通道ID，获取电缆通道数据
        /// </summary>
        private string cableChannelUrl;

        /// <summary>
        ///     根据电缆通道ID，获取电气设备数据
        /// </summary>
        private string cableEquipmentUrl;

        /// <summary>
        ///     获取符号ids
        /// </summary>
        private string getSymbolIdsUrl;

        /// <summary>
        ///     获取拉线数据
        /// </summary>
        private string getPullLineUrl;

        /// <summary>
        ///     通过变压器容量和所属杆高，获取变压器组件信息
        /// </summary>
        private string getTransformerListUrl;


        #endregion


        public ProjectService
        (
            IApiClient apiClient,
            IProjectRepository repository,
            ICapPublisher capPublisher,
            IRedisCache redisClient,
            IGisTransformService gisTransformService,
            ISocketService socketService
        )
        {
            _apiClient = apiClient;
            _repository = repository;
            _capPublisher = capPublisher;
            _redisClient = redisClient;
            this.gisTransformService = gisTransformService;
            _configuration = Ioc.GetService<IConfiguration>();
            _socketService = socketService;
            InitData();
        }

        //初始化信息
        private void InitData()
        {
            towerModulesUrl = $"{_configuration.GetSection("Config")["ResourceApi"]}/api/LibraryDesign/BatchQueryTowerModules";
            specifiedLinesUrl = $"{_configuration.GetSection("Config")["ResourceApi"]}/api/LibraryMaterial/BatchGetSpecifiedLines";
            cableWellUrl = $"{_configuration.GetSection("Config")["ResourceApi"]}/api/LibraryCable/GetCableWellList";
            cableChannelUrl= $"{_configuration.GetSection("Config")["ResourceApi"]}/api/LibraryCable/GetCableChannelList";
            cableEquipmentUrl = $"{_configuration.GetSection("Config")["ResourceApi"]}/api/LibraryComponent/GetComponentList";
            getSymbolIdsUrl = $"{_configuration.GetSection("Config")["ApplicationApi"]}/api/System/GetSymbolIds";
            getPullLineUrl= $"{_configuration.GetSection("Config")["ResourceApi"]}/api/LibraryDesign/BatchGetTowerModuleComponents";
            getTransformerListUrl = $"{_configuration.GetSection("Config")["ResourceApi"]}/api/LibraryComponent/GetTransformerList";
        }


        #region 检查项目状态
        /// <summary>
        /// 检查项目状态
        /// </summary>
        /// <param name="state"></param>
        /// <param name="requiredStatus"></param>
        /// <returns></returns>
        bool CheckProjectState(ProjectState state, params ProjectStatus[] requiredStatus)
        {
            if (!requiredStatus.Contains(state.Status))
            {
                //throw new BusinessException(ResponseErrorCode.ProcessError, $"项目流程状态未处于{string.Join("或", requiredStatus.Select(p => $"[{p.GetText()}]"))},无法进行此操作");
            }
            return true;
        }
        #endregion

        #region 变更项目状态
        /// <summary>
        /// 变更项目状态
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <param name="statusType">状态类型</param>
        /// <param name="status">变更状态</param>
        /// <param name="notes">提示</param>
        /// <returns></returns>
        async Task ChangeProjectStateAsync(string projectId, ProjectStatusType statusType, int status, string notes)
        {
            var project = await DbHelper.FindByIdAsync<Cyg.Applicatio.Entity.Project>(projectId);
            if (!project.HasVal())
            {
                throw new BusinessException(ResponseErrorCode.BusinessError, "项目不存在或已删除");
            }
            var projectState = await DbHelper.FindByIdAsync<ProjectState>(project.Id);
            using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectStatusLockKey, $"{projectId}_{status}"), 5))
            {
                await DbHelper.InsertAsync(new ProjectStateLog()
                {
                    EngineerId = project.EngineerId,
                    ProjectId = project.Id,
                    StatusType = statusType,
                    CreatedBy = CurrentUser.Id,
                    OriginalStatus = projectState.Status.ToInt(),
                    ChangeStatus = status,
                    Notes = notes,
                    CreatedOn = DateTime.Now,
                });

                if (statusType == ProjectStatusType.设计状态)
                {
                    await DbHelper.UpdateAsync<ProjectState>(it => it.Id == projectId, new
                    {
                        Status = status,
                        UpdateTime = DateTime.Now,
                    });
                }
                else if (statusType == ProjectStatusType.外部状态)
                {
                    await DbHelper.UpdateAsync<ProjectState>(it => it.Id == projectId, new
                    {
                        OutsideStatus = status,
                    });
                }
            }
        }
        #endregion

        #region 获取工程列表
        /// <summary>
        /// 获取工程列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<ProjectResponse>> GetListAsync(ProjectRequest request)
        {
            var url = $"{_configuration.GetSection("Config")["ProjectApi"]}/api/Project/GetSurveyList";
            var result = await _apiClient.ExecuteAsync<List<ProjectResponse>>(HttpMethod.Post, url, request);
            return result;
            //return _repository.GetListAsync(request);
        }
        #endregion

        #region 获取工程明细
        /// <summary>
        /// 获取工程明细
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<ProjectDetailResponse> GetDetailAsync(string projectId)
        {
            await CheckSurveyPermissionAsync(projectId);
            return await _repository.GetDetailAsync(projectId);
        }
        #endregion

        #region 上报勘测数据
        /// <summary>
        /// 上报勘测数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [UseTransaction]
       
        public async Task ReportSurveyDataAsync(ReportSurveyDataRequest request)
        {
           // var data= FSql.Select<Design_tower>().ToList(m=>new Design_tower{ Geom=m.Geom.GeometryToString() });
            using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectLockKey, request.ProjectId), 5))
            {
                //获取项目信息，项目状态，工程信息
                var (project, projectState,engineer) = await CheckSurveyPermissionAsync(request.ProjectId);
                CheckProjectState(projectState, ProjectStatus.未勘察,ProjectStatus.勘察中);
                await ClearSurveyAsync(request.ProjectId);
                await ClearDesignAsync(request.ProjectId);
                await ClearDismantleAsync(request.ProjectId);
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
                #region 导线选型

                //勘察数据导线选型
                request.SurveyData.Lines.ForEach(l =>l.L_ModeID=l.L_Mode);

                //获取线材视图模型
                GetBatchLinesRequest getBatchLinesRequest = new GetBatchLinesRequest();
                getBatchLinesRequest.BatchLines = new List<BatchLinesRequest>();
                foreach (var group in request.SurveyData.Lines.GroupBy(l => l.L_ModeID))
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
                request.DesignData.Lines.ForEach(l => l.L_ModeID = l.L_Mode);
                foreach (var group in request.DesignData.Lines.GroupBy(l => l.L_ModeID))
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
                //获取资源库线材信息
                var lineViews = await _apiClient.ExecuteAsync<List<BatchLineViewResponse>>(HttpMethod.Post, specifiedLinesUrl, getBatchLinesRequest);
                foreach (var line in request.DesignData.Lines)
                {
                    var lineView = lineViews.FirstOrDefault(y => y.LineID == line.Id);
                    if (lineView == null) continue;
                    line.L_ModeID = lineView.MaterialID;
                    line.L_Mode = lineView.Spec;
                    line.L_Type= lineView.MaterialName;
                }
                foreach (var line in request.SurveyData.Lines)
                {
                    var lineView = lineViews.FirstOrDefault(y => y.LineID == line.Id);
                    if (lineView == null) continue;
                    line.L_ModeID = lineView.MaterialID;
                    line.L_Mode = lineView.Spec;
                    line.L_Type = lineView.MaterialName;
                }
                #endregion
                //获取杆型方案
                var towers = new List<DesignTowerDto>();
                towers.AddRange(request.SurveyData?.Towers);
                towers.AddRange(request.DesignData?.Towers);
                var towerModules = await TowerModules(towers, engineer.LibId);

                //获取电缆井数据 
                var cables = new List<CableRequest>();
                cables.AddRange(request.SurveyData.Cables);
                cables.AddRange(request.DesignData.Cables);
                var cableModules = await CableWellResponses(cables, engineer.LibId);

                //获取电缆通道数据
                var cableChannels = new List<CableChannelRequest>();
                cableChannels.AddRange(request.SurveyData.CableChannels);
                cableChannels.AddRange(request.DesignData.CableChannels);
                var cableChannelModules = await CableChannelResponses(cableChannels, engineer.LibId);
                //获取电气设备数据
                var cableEquipments = new List<CableEquipmentRequest>();
                cableEquipments.AddRange(request.SurveyData.CableEquipments);
                cableEquipments.AddRange(request.DesignData.CableEquipments);
                var cableEquipmentModules = await CableEquipmentResponses(cableEquipments, engineer.LibId);

                var symbolIdsRequest = new List<BatchTowerViewResponse>();
                symbolIdsRequest.AddRange(towerModules);
                //获取型号id
                SymbolIdResponse getSymbolIds = await GetSymbolIds(request, symbolIdsRequest);
                //获取杆上设备型号
                var overheadEquipmentRequests = new List<OverheadEquipmentRequest>();
                overheadEquipmentRequests.AddRange(request.SurveyData?.OverheadEquipments);
                overheadEquipmentRequests.AddRange(request.DesignData?.OverheadEquipments);
                var transformers =await GetTransformerList(overheadEquipmentRequests, towers, engineer.LibId);

                await WriteSurveyDataAsync
                (
                    request.SurveyData.Towers,
                    request.SurveyData.Cables,
                    request.SurveyData.CableChannels,
                    request.SurveyData.CableEquipments,
                    request.SurveyData.OverheadEquipments,
                    request.SurveyData.Lines,
                    request.SurveyData.Marks,
                    request.SurveyData.Medias,
                    towerModules,
                    request.ProjectId,
                    cableChannelModules,
                    cableModules,
                    cableEquipmentModules,
                    getSymbolIds,
                    transformers
                );
                await WriteDesignDataAsync
                (
                    request.DesignData.Towers,
                    request.DesignData.Cables,
                    request.DesignData.CableChannels,
                    request.DesignData.CableEquipments,
                    request.DesignData.OverheadEquipments,
                    request.DesignData.Lines,
                    request.DesignData.Marks,
                    request.DesignData.Medias,
                    towerModules,
                    request.ProjectId,
                    cableChannelModules,
                    cableModules,
                    cableEquipmentModules,
                    getSymbolIds,
                    transformers
                );
                await WriteDismantleDataAsync
                (
                    request.SurveyData.Towers,
                    request.SurveyData.Cables,
                    request.SurveyData.CableChannels,
                    request.SurveyData.CableEquipments,
                    request.SurveyData.OverheadEquipments,
                    request.SurveyData.Lines,
                    request.SurveyData.Marks,
                    request.SurveyData.Medias,
                    towerModules,
                    request.ProjectId,
                    cableChannelModules,
                    cableModules,
                    cableEquipmentModules,
                    getSymbolIds,
                    transformers
                );
                #region 写入轨迹数据
                await DbHelper.DeleteAsync<ProjectTrackRecord>(it => it.ProjectId == request.ProjectId );
                if (request.SurveyData.TrackRecords.HasVal())
                {
                    //&& it.RecordType == ProjectTraceRecordType.勘察
                    var tracks = request.SurveyData.TrackRecords.MapTo<List<ProjectTrackRecord>>();
                    tracks.ForEach(it =>
                    {
                        it.ProjectId = request.ProjectId;
                        it.RecordType = ProjectTraceRecordType.勘察;
                    });
                    await DbHelper.BulkInsertAsync(tracks);
                }
                #endregion

                #region 写入拉线数据
                await InsertPullLineData(request, towerModules, engineer.LibId);
                #endregion

                //更新状态
                if (projectState.Status != ProjectStatus.勘察中)
                {
                    await ChangeProjectStateAsync(project.Id, ProjectStatusType.设计状态, ProjectStatus.勘察中.ToInt(), "上报勘测数据");
                }
                //修改app上传时间
                await DbHelper.UpdateAsync<ProjectState>(it => it.Id == request.ProjectId, new
                {
                    AppUploadTime = DateTime.Now,
                });
                //同步postgis
                await gisTransformService.GisTransformProjectAsync(new List<string>() { request.ProjectId });
            }
        }

        //public 


        /// <summary>
        /// 上报勘测数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task RepairReportSurveyDataAsync(ReportSurveyDataRequest request)
        {
            using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectLockKey, request.ProjectId), 5))
            {
                    var project = await DbHelper.FindByIdAsync<Project>(request.ProjectId);
                    if (!project.HasVal())
                    {
                        LogHelper.LogWarning($"【勘测端修复项目】未找到当前项目{project.Id}");
                        return;
                    }
                    var engineer = await DbHelper.FindByIdAsync<Engineer>(project.EngineerId);
                    if (!engineer.HasVal())
                    {
                        LogHelper.LogWarning($"【勘测端修复项目】未找到当前项目{project.Id}的工程");
                        return;
                    }
                var projectState = await DbHelper.FindAsync<ProjectState>(it => it.Id == request.ProjectId);
                await RepairClearSurveyAsync(request.ProjectId);
                await RepairClearDesignAsync(request.ProjectId);
                await RepairClearDismantleAsync(request.ProjectId);

                #region 导线选型

                //勘察数据导线选型
                //request.SurveyData.Lines.ForEach(l => l.L_ModeID = l.L_Mode);

                //获取线材视图模型
                GetBatchLinesRequest getBatchLinesRequest = new GetBatchLinesRequest();
                getBatchLinesRequest.BatchLines = new List<BatchLinesRequest>();
                foreach (var group in request.SurveyData.Lines.GroupBy(l => l.L_ModeID))
                {
                    foreach (var line in group)
                    {
                        if (string.IsNullOrEmpty(group.Key)) continue;
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            MaterialID = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            Spec = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                    }
                }
                foreach (var group in request.SurveyData.Lines.GroupBy(l => l.L_Mode))
                {
                    foreach (var line in group)
                    {
                        if (string.IsNullOrEmpty(group.Key)) continue;
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            MaterialID = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            Spec = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                    }
                }
                //预设数据导线选型
                foreach (var group in request.DesignData.Lines.GroupBy(l => l.L_ModeID))
                {
                    foreach (var line in group)
                    {
                        if (string.IsNullOrEmpty(group.Key)) continue;
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            MaterialID = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            Spec = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                    }
                }
                foreach (var group in request.DesignData.Lines.GroupBy(l => l.L_Mode))
                {
                    foreach (var line in group)
                    {
                        if (string.IsNullOrEmpty(group.Key)) continue;
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            MaterialID = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                        getBatchLinesRequest.BatchLines.Add(new BatchLinesRequest
                        {
                            Spec = group.Key,
                            ResourceLibID = engineer.LibId,
                            LineID = line.Id
                        });
                    }
                }
                //获取资源库线材信息
                var lineViews = await _apiClient.ExecuteAsync<List<BatchLineViewResponse>>(HttpMethod.Post, specifiedLinesUrl, getBatchLinesRequest);
                foreach (var line in request.DesignData.Lines)
                {
                    var lineView = lineViews.FirstOrDefault(y => y.LineID == line.Id&&!string.IsNullOrEmpty(y.MaterialID));
                    if (lineView == null) continue;
                    line.L_ModeID = lineView.MaterialID;
                    line.L_Mode = lineView.Spec;
                    line.L_Type = lineView.MaterialName;
                }
                foreach (var line in request.SurveyData.Lines)
                {
                    var lineView = lineViews.FirstOrDefault(y => y.LineID == line.Id && !string.IsNullOrEmpty(y.MaterialID));
                    if (lineView == null) continue;
                    line.L_ModeID = lineView.MaterialID;
                    line.L_Mode = lineView.Spec;
                    line.L_Type = lineView.MaterialName;
                }
                #endregion
                //获取杆型方案
                var towers = new List<DesignTowerDto>();
                towers.AddRange(request.SurveyData?.Towers);
                towers.AddRange(request.DesignData?.Towers);
                var towerModules = await TowerModules(towers, engineer.LibId);

                //获取电缆井数据 
                var cables = new List<CableRequest>();
                cables.AddRange(request.SurveyData.Cables);
                cables.AddRange(request.DesignData.Cables);
                var cableModules = await CableWellResponses(cables, engineer.LibId);

                //获取电缆通道数据
                var cableChannels = new List<CableChannelRequest>();
                cableChannels.AddRange(request.SurveyData.CableChannels);
                cableChannels.AddRange(request.DesignData.CableChannels);
                var cableChannelModules = await CableChannelResponses(cableChannels, engineer.LibId);
                //获取电气设备数据
                var cableEquipments = new List<CableEquipmentRequest>();
                cableEquipments.AddRange(request.SurveyData.CableEquipments);
                cableEquipments.AddRange(request.DesignData.CableEquipments);
                var cableEquipmentModules = await CableEquipmentResponses(cableEquipments, engineer.LibId);

                var symbolIdsRequest = new List<BatchTowerViewResponse>();
                symbolIdsRequest.AddRange(towerModules);
                //获取型号id
                SymbolIdResponse getSymbolIds = await GetSymbolIds(request, symbolIdsRequest);
                //获取杆上设备型号
                var overheadEquipmentRequests = new List<OverheadEquipmentRequest>();
                overheadEquipmentRequests.AddRange(request.SurveyData?.OverheadEquipments);
                overheadEquipmentRequests.AddRange(request.DesignData?.OverheadEquipments);
                var transformers = await GetTransformerList(overheadEquipmentRequests, towers, engineer.LibId);

                await RepairWriteSurveyDataAsync
                (
                    request.SurveyData.Towers,
                    request.SurveyData.Cables,
                    request.SurveyData.CableChannels,
                    request.SurveyData.CableEquipments,
                    request.SurveyData.OverheadEquipments,
                    request.SurveyData.Lines,
                    request.SurveyData.Marks,
                    request.SurveyData.Medias,
                    towerModules,
                    request.ProjectId,
                    cableChannelModules,
                    cableModules,
                    cableEquipmentModules,
                    getSymbolIds,
                    transformers
                );
                await RepairWriteDesignDataAsync
                (
                    request.DesignData.Towers,
                    request.DesignData.Cables,
                    request.DesignData.CableChannels,
                    request.DesignData.CableEquipments,
                    request.DesignData.OverheadEquipments,
                    request.DesignData.Lines,
                    request.DesignData.Marks,
                    request.DesignData.Medias,
                    towerModules,
                    request.ProjectId,
                    cableChannelModules,
                    cableModules,
                    cableEquipmentModules,
                    getSymbolIds,
                    transformers
                );
                await RepairWriteDismantleDataAsync
                (
                    request.SurveyData.Towers,
                    request.SurveyData.Cables,
                    request.SurveyData.CableChannels,
                    request.SurveyData.CableEquipments,
                    request.SurveyData.OverheadEquipments,
                    request.SurveyData.Lines,
                    request.SurveyData.Marks,
                    request.SurveyData.Medias,
                    towerModules,
                    request.ProjectId,
                    cableChannelModules,
                    cableModules,
                    cableEquipmentModules,
                    getSymbolIds,
                    transformers
                );
                #region 写入拉线数据
                await InsertPullLineData(request, towerModules, engineer.LibId);
                #endregion
                //同步postgis
                await gisTransformService.GisTransformProjectAsync(new List<string>() { request.ProjectId });
            }
        }

        /// <summary>
        /// 修复设计数据(从勘测复制)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task RepairReportDesignDataAsync(ReportSurveyDataRequest request)
        {
            using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectLockKey, request.ProjectId), 5))
            {
                var project = await DbHelper.FindByIdAsync<Project>(request.ProjectId);
                if (!project.HasVal())
                {
                    LogHelper.LogWarning($"【勘测端修复项目】未找到当前项目{project.Id}");
                    return;
                }
                var engineer = await DbHelper.FindByIdAsync<Engineer>(project.EngineerId);
                if (!engineer.HasVal())
                {
                    LogHelper.LogWarning($"【勘测端修复项目】未找到当前项目{project.Id}的工程");
                    return;
                }
                var projectState = await DbHelper.FindAsync<ProjectState>(it => it.Id == request.ProjectId);
                await RepairClearDesignAsync(request.ProjectId);

                #region 导线选型

                //获取线材视图模型
                GetBatchLinesRequest getBatchLinesRequest = new GetBatchLinesRequest();
                getBatchLinesRequest.BatchLines = new List<BatchLinesRequest>();
                foreach (var group in request.DesignData.Lines.GroupBy(l => l.L_ModeID))
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

                //获取资源库线材信息
                var lineViews =  await _apiClient.ExecuteAsync<List<BatchLineViewResponse>>(HttpMethod.Post, specifiedLinesUrl, getBatchLinesRequest);
                foreach (var line in request.DesignData.Lines)
                {
                    var lineView = lineViews.FirstOrDefault(y => y.LineID == line.Id);
                    if (lineView == null) continue;
                    line.L_ModeID = lineView.MaterialID;
                    line.L_Mode = lineView.Spec;
                    line.L_Type = lineView.MaterialName;
                }
                #endregion
                //获取杆型方案
                var towers = new List<DesignTowerDto>();
                towers.AddRange(request.DesignData?.Towers);
                var towerModules = await TowerModules(towers, engineer.LibId);

                //获取电缆井数据 
                var cables = new List<CableRequest>();
                cables.AddRange(request.DesignData?.Cables);
                var cableModules =await CableWellResponses(cables, engineer.LibId);

                //获取电缆通道数据
                var cableChannels = new List<CableChannelRequest>();
                cableChannels.AddRange(request.DesignData?.CableChannels);
                var cableChannelModules = await CableChannelResponses(cableChannels, engineer.LibId);
                //获取电气设备数据
                var cableEquipments = new List<CableEquipmentRequest>();
                cableEquipments.AddRange(request.DesignData?.CableEquipments);
                var cableEquipmentModules = await CableEquipmentResponses(cableEquipments, engineer.LibId);

                var symbolIdsRequest = new List<BatchTowerViewResponse>();
                symbolIdsRequest.AddRange(towerModules);
                //获取型号id
                SymbolIdResponse getSymbolIds = await GetSymbolIds(request, symbolIdsRequest);
                //获取杆上设备型号
                var overheadEquipmentRequests = new List<OverheadEquipmentRequest>();
                overheadEquipmentRequests.AddRange(request.DesignData?.OverheadEquipments);
                var transformers = await GetTransformerList(overheadEquipmentRequests, towers, engineer.LibId);

                await RepairWriteDesignDataAsync
                (
                    request.DesignData.Towers,
                    request.DesignData.Cables,
                    request.DesignData.CableChannels,
                    request.DesignData.CableEquipments,
                    request.DesignData.OverheadEquipments,
                    request.DesignData.Lines,
                    request.DesignData.Marks,
                    request.DesignData.Medias,
                    towerModules,
                    request.ProjectId,
                    cableChannelModules,
                    cableModules,
                    cableEquipmentModules,
                    getSymbolIds,
                    transformers
                );
                await InsertPullLineData(request, towerModules, engineer.LibId);
            }
        }
       
        /// <summary>
        ///    修复设计数据(将勘测数据复制到设计数据)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task RepairDesignData(List<string> request)
        {
            System.Threading.Thread.Sleep(4000);
            AllNum =0 ;
            FailNum = 0;
            FailId = new List<string>();
            SuccessNum = 0; 
            if (request == null || request.Count == 0)
            {
                var proData =await DbHelper.FindAllAsync<Project>();
                request = proData.ToList().Select(x => x.Id).ToList();
             
            }
            AllNum = request.Count;
            //获取数据库数据
            foreach (var projectId in request)
            {

                try
                {
                    //获取项目信息
                    var proInfo = DbHelper.FindById<Project>(projectId);
                    var projectState = await DbHelper.FindAsync<ProjectState>(it => it.Id == projectId);
                    if (projectState == null) continue;
                    if (!(projectState.Status == ProjectStatus.勘察中 ||
                        projectState.Status == ProjectStatus.已勘察 ||
                        projectState.Status == ProjectStatus.设计中 ||
                        projectState.Status == ProjectStatus.设计完成)
                    )
                    {
                        FailNum++;
                        FailId.Add(projectId + " " + projectState.Status.GetText());
                        //告知客户端批注消息更新
                        foreach (var item in SocketHandler.userDic)
                        {
                            item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                            SocketHandler.SendMessage(item);
                        }
                        continue;
                    }
                    var engineer = DbHelper.FindById<Engineer>(proInfo.EngineerId);
                    ReportSurveyDataRequest requestInfo = new ReportSurveyDataRequest
                    {
                        DesignData = new CollectionDataRequest(),
                        SurveyData = new CollectionDataRequest(),
                        ProjectId = projectId
                    };
                    #region 获取杆塔数据
                    var surveyTowers = await DbHelper.FindAllAsync<SurveyTower>(x => x.ProjectId == projectId);
                    var surveyTowerExts = await DbHelper.FindAllAsync<SurveyTowerExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.Towers =
                        (from item in surveyTowers
                         select
                                new DesignTowerDto
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    ParentId = item.ParentId,
                                    PreNodeType = item.PreNodeType,
                                    Code = item.Code,
                                    Specification = item.T_Rod + "*" + item.T_Height,
                                    PoleTypeCode = surveyTowerExts.FirstOrDefault(x => x.Id == item.Id)?.PoleTypeCode,
                                    T_Segment = item.T_Segment,
                                    T_Sort = item.T_Sort,
                                    T_Type = item.T_Type,
                                    KVLevel = item.KVLevel,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    ElectrifiedWork = item.ElectrifiedWork,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    #endregion


                    #region 获取电缆井数据
                    var surveyCables = await DbHelper.FindAllAsync<SurveyCable>(x => x.ProjectId == projectId);
                    var surveyCableExts = await DbHelper.FindAllAsync<SurveyCableExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.Cables =
                        (from item in surveyCables
                         select
                                new CableRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    ParentId = item.ParentId,
                                    PreNodeType = item.PreNodeType,
                                    Code = item.Code,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    C_Type = item.C_Type,
                                    C_ModeID = surveyCableExts.FirstOrDefault(x => x.Id == item.Id)?.C_ModeID,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    ElectrifiedWork = item.ElectrifiedWork,
                                    Surveyor = item.Surveyor

                                }).ToList();
                    #endregion

                    #region 获取电缆通道数据
                    var surveyCableChannels = await DbHelper.FindAllAsync<SurveyCableChannel>(x => x.ProjectId == projectId);
                    var surveyCableChannelExts = await DbHelper.FindAllAsync<SurveyCableChannelExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.CableChannels =
                        (from item in surveyCableChannels
                         select
                                new CableChannelRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    Start_Id = item.Start_Id,
                                    StartNodeType = item.StartNodeType,
                                    End_Id = item.End_Id,
                                    EndNodeType = item.EndNodeType,
                                    C_LayMode = item.C_LayMode,
                                    C_ModeId = item.C_ModeId,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    Length = item.Length,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    #endregion

                    #region 获取杆上设备数据
                    var surveyCabeleEquipments = await DbHelper.FindAllAsync<SurveyCableEquipment>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.CableEquipments =
                        (from item in surveyCabeleEquipments
                         select
                                new CableEquipmentRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    Name = item.Name,
                                    Code = item.Code,
                                    Type = item.Type,
                                    ParentId = item.ParentId,
                                    PreNodeType = item.PreNodeType,
                                    EquipModel = item.EquipModel,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    ElectrifiedWork = item.ElectrifiedWork,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    #endregion

                    #region 架空杆上设备
                    var surveyOverheadEquipmentss = await DbHelper.FindAllAsync<SurveyOverheadEquipment>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.OverheadEquipments =
                        (from item in surveyOverheadEquipmentss
                         select
                                new OverheadEquipmentRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    Name = item.Name,
                                    Type = item.Type,
                                    Main_ID = item.Main_ID,
                                    Sub_ID = item.Sub_ID,
                                    Capacity = item.Capacity,
                                    FixMode = item.FixMode,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    Surveyor = item.Surveyor
                                }).ToList();

                    #endregion

                    #region 线路
                    var surveyLines = await DbHelper.FindAllAsync<SurveyLine>(x => x.ProjectId == projectId);
                    var surveyLineExts = await DbHelper.FindAllAsync<SurveyLineExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.Lines =
                        (from item in surveyLines
                         select
                                new LineRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    Name = item.Name,
                                    LoopName = item.LoopName,
                                    Start_ID = item.Start_ID,
                                    StartNodeType = item.StartNodeType,
                                    End_ID = item.End_ID,
                                    EndNodeType = item.EndNodeType,
                                    L_Type = item.L_Type,
                                    L_Mode = item.L_Mode,
                                    L_ModeID = surveyLineExts.FirstOrDefault(x => x.Id == item.Id)?.L_ModeID,
                                    Length = item.Length,
                                    KVLevel = item.KVLevel,
                                    IsCable = item.IsCable,
                                    LoopSerial = item.LoopSerial,
                                    GroupId = item.GroupId,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    Index = item.Index,
                                    Surveyor = item.Surveyor
                                }).ToList();

                    #endregion
                    var surveyMarks = await DbHelper.FindAllAsync<SurveyMark>(x => x.ProjectId == projectId);

                    requestInfo.DesignData.Marks =
                        (
                        from item in surveyMarks
                        select new MarkRequest
                        {
                            Id=item.Id+"1",
                            ProjectId=item.ProjectId,
                            Name=item.Name,
                            Type=item.Type,
                            Width=string.IsNullOrEmpty(item.Width)?"0": item.Width,
                            Height= string.IsNullOrEmpty(item.Height) ? "0" : item.Height,
                            Azimuth= item.Azimuth,
                            Lat_WGS84=item.Lat_WGS84,
                            Lon_WGS84=item.Lon_WGS84,
                            RoadLevel=item.RoadLevel,
                            LineKvLevel=item.LineKvLevel,
                            Floors=item.Floors,
                            Provider=item.Provider,
                            State=item.State,
                            SurveyTime=item.SurveyTime,
                            Remark=item.Remark
                        }
                        ).ToList();
                    var _marks = requestInfo.DesignData.Marks.MapTo<List<DesignMark>>();
                    if (_marks.HasVal())
                    {
                        _marks.ForEach(m => m.Surveyor = CurrentUser.Id);
                        await DbHelper.BulkInsertAsync(_marks);
                    }
                    await RepairReportDesignDataAsync(requestInfo);

                    SuccessNum++;
                }
                catch (Exception e)
                {
                    FailNum++;
                    FailId.Add(projectId+" "+e.Message);
                   //给出异常信息
                  
                }
                //告知客户端批注消息更新
                foreach (var item in SocketHandler.userDic)
                {
                    item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum+"失败详情:"+JsonConvert.SerializeObject(FailId);
                    SocketHandler.SendMessage(item);
                }
                continue;

            }
            
        }

        public async Task RepairDesignDataTwo(List<string> request)
        {
            System.Threading.Thread.Sleep(4000);
            AllNum = 0;
            FailNum = 0;
            FailId = new List<string>();
            SuccessNum = 0;
            if (request == null || request.Count == 0)
            {
                var proData = await DbHelper.FindAllAsync<Project>();
                request = proData.ToList().Select(x => x.Id).ToList();

            }
            AllNum = request.Count;
            //获取数据库数据
            foreach (var projectId in request)
            {

                try
                {
                    //获取项目信息
                    var proInfo = DbHelper.FindById<Project>(projectId);
                    var projectState = await DbHelper.FindAsync<ProjectState>(it => it.Id == projectId);
                    if (projectState == null) continue;
                    if (!(projectState.Status == ProjectStatus.勘察中 ||
                        projectState.Status == ProjectStatus.已勘察 ||
                        projectState.Status == ProjectStatus.设计中 ||
                        projectState.Status == ProjectStatus.设计完成)
                    )
                    {
                        FailNum++;
                        FailId.Add(projectId + " " + projectState.Status.GetText());
                        //告知客户端批注消息更新
                        foreach (var item in SocketHandler.userDic)
                        {
                            item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                            SocketHandler.SendMessage(item);
                        }
                        continue;
                    }
                    var engineer = DbHelper.FindById<Engineer>(proInfo.EngineerId);
                    ReportSurveyDataRequest requestInfo = new ReportSurveyDataRequest
                    {
                        DesignData = new CollectionDataRequest(),
                        SurveyData = new CollectionDataRequest(),
                        ProjectId = projectId
                    };
                    #region 获取杆塔数据
                    var surveyTowers = await DbHelper.FindAllAsync<SurveyTower>(x => x.ProjectId == projectId);
                    var surveyTowerExts = await DbHelper.FindAllAsync<SurveyTowerExt>(x => x.ProjectId == projectId);
                    var designTowers = (from item in surveyTowers select item.MapTo<DesignTower>()).ToList();
                    var designTowerExts = (from item in surveyTowerExts select item.MapTo<DesignTowerExt>()).ToList();
                    await DbHelper.BulkInsertAsync(designTowers);
                    await DbHelper.BulkInsertAsync(designTowerExts);
                    #endregion


                    #region 获取电缆井数据
                    var surveyCables = await DbHelper.FindAllAsync<SurveyCable>(x => x.ProjectId == projectId);
                    var surveyCableExts = await DbHelper.FindAllAsync<SurveyCableExt>(x => x.ProjectId == projectId);
                    var designCables= (from item in surveyTowers select item.MapTo<DesignCable>()).ToList();
                    var designCableExts = (from item in surveyTowerExts select item.MapTo<DesignCableExt>()).ToList();
                    await DbHelper.BulkInsertAsync(designCables);
                    await DbHelper.BulkInsertAsync(designCableExts);
                    #endregion

                    #region 获取电缆通道数据
                    var surveyCableChannels = await DbHelper.FindAllAsync<SurveyCableChannel>(x => x.ProjectId == projectId);
                    var surveyCableChannelExts = await DbHelper.FindAllAsync<SurveyCableChannelExt>(x => x.ProjectId == projectId);
                    var designCableChannels = (from item in surveyCableChannels select item.MapTo<DesignCableChannel>()).ToList();
                    var designCableExtChannels = (from item in surveyCableChannelExts select item.MapTo<DesignCableChannelExt>()).ToList();
                    await DbHelper.BulkInsertAsync(designCableChannels);
                    await DbHelper.BulkInsertAsync(designCableExtChannels);
                    #endregion

                    #region 获取杆上设备数据 无扩展表
                    var surveyCabeleEquipments = await DbHelper.FindAllAsync<SurveyCableEquipment>(x => x.ProjectId == projectId);
                    var designCableEquipments = (from item in surveyCableChannels select item.MapTo<DesignCableEquipment>()).ToList();
                    await DbHelper.BulkInsertAsync(designCableEquipments);
                    #endregion

                    #region 架空杆上设备 无扩展表
                    var surveyOverheadEquipments = await DbHelper.FindAllAsync<SurveyOverheadEquipment>(x => x.ProjectId == projectId);
                    var designOverheadEquipments = (from item in surveyOverheadEquipments select item.MapTo<DesignOverheadEquipment>()).ToList();
                    await DbHelper.BulkInsertAsync(designOverheadEquipments);
                    #endregion

                    #region 线路
                    var surveyLines = await DbHelper.FindAllAsync<SurveyLine>(x => x.ProjectId == projectId);
                    var surveyLineExts = await DbHelper.FindAllAsync<SurveyLineExt>(x => x.ProjectId == projectId);
                    var designLines = (from item in surveyLines select item.MapTo<DesignLine>()).ToList();
                    var designLineExts = (from item in surveyLineExts select item.MapTo<DesignLineExt>()).ToList();
                    await DbHelper.BulkInsertAsync(designLines);
                    await DbHelper.BulkInsertAsync(designLineExts);

                    #endregion  勘测无地物扩展表
                    
                    var surveyMarks = await DbHelper.FindAllAsync<SurveyMark>(x => x.ProjectId == projectId);
                    var designMarks = (from item in surveyOverheadEquipments select item.MapTo<DesignMark>()).ToList();
                    await DbHelper.BulkInsertAsync(designMarks);
                    SuccessNum++;
                }
                catch (Exception e)
                {
                    FailNum++;
                    FailId.Add(projectId + " " + e.Message);
                    //给出异常信息

                }
                //告知客户端批注消息更新
                foreach (var item in SocketHandler.userDic)
                {
                    item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                    SocketHandler.SendMessage(item);
                }
                continue;

            }
        }
            
        /// <summary>
        ///    修复设计数据id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task RepairDesignDataId(List<string> request)
        {
            System.Threading.Thread.Sleep(4000);
            AllNum = 0;
            FailNum = 0;
            FailId = new List<string>();
            SuccessNum = 0;
            if (request == null || request.Count == 0)
            {
                var proData = await DbHelper.FindAllAsync<Project>();
                request = proData.ToList().Select(x => x.Id).ToList();

            }
            AllNum = request.Count;
            //获取数据库数据
            foreach (var projectId in request)
            {

                try
                {
                    //获取项目信息
                    var proInfo = DbHelper.FindById<Project>(projectId);
                    var projectState = await DbHelper.FindAsync<ProjectState>(it => it.Id == projectId);
                    if (projectState == null) continue;
                    if (!(projectState.Status == ProjectStatus.勘察中 ||
                        projectState.Status == ProjectStatus.已勘察 ||
                        projectState.Status == ProjectStatus.设计中 ||
                        projectState.Status == ProjectStatus.设计完成)
                    )
                    {
                        FailNum++;
                        FailId.Add(projectId + " " + projectState.Status.GetText());
                        //告知客户端批注消息更新
                        foreach (var item in SocketHandler.userDic)
                        {
                            item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                            SocketHandler.SendMessage(item);
                        }
                        continue;
                    }
                    var engineer = DbHelper.FindById<Engineer>(proInfo.EngineerId);
                    ReportSurveyDataRequest requestInfo = new ReportSurveyDataRequest
                    {
                        DesignData = new CollectionDataRequest(),
                        SurveyData = new CollectionDataRequest(),
                        ProjectId = projectId
                    };


                    #region 获取杆塔数据    DesignTower: id\parentid;DesignTowerExt:id
                    var designTowers = await DbHelper.FindAllAsync<DesignTower>(x => x.ProjectId == projectId);
                    foreach (var item in designTowers)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(item.Id);
                        var surveyTowerExt = DbHelper.FindById<DesignTowerExt>(id);
                        if (surveyTowerExt.HasVal())
                        {
                            surveyTowerExt.Id = UserMd5(surveyTowerExt.Id);
                            await DbHelper.DeleteByIdAsync<DesignTowerExt>(id);
                            await DbHelper.InsertAsync<DesignTowerExt>(surveyTowerExt);
                        }
                        if (!string.IsNullOrEmpty(item.ParentId))
                        {
                            item.ParentId = UserMd5(item.ParentId);
                        }
                        await DbHelper.DeleteByIdAsync<DesignTower>(id);
                        await DbHelper.InsertAsync<DesignTower>(item);
                    }
                    #endregion


                    #region 获取电缆井数据
                    var designCables = await DbHelper.FindAllAsync<DesignCable>(x => x.ProjectId == projectId);

                    foreach (var item in designCables)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(item.Id);
                        var designCableExt = DbHelper.FindById<DesignCableExt>(id);
                        if (designCableExt.HasVal())
                        {
                            designCableExt.Id = UserMd5(designCableExt.Id);
                            await DbHelper.DeleteByIdAsync<DesignCableExt>(id);
                            await DbHelper.InsertAsync<DesignCableExt>(designCableExt);
                        }
                        if (!string.IsNullOrEmpty(item.ParentId))
                        {
                            item.ParentId = UserMd5(item.ParentId);
                        }
                        await DbHelper.DeleteByIdAsync<DesignCable>(id);
                        await DbHelper.InsertAsync<DesignCable>(item);
                    }
                    #endregion

                    #region 获取电缆通道数据
                    var designCableChannels = await DbHelper.FindAllAsync<DesignCableChannel>(x => x.ProjectId == projectId);
                    foreach (var item in designCableChannels)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(item.Id);
                        var designCableChannelExt = DbHelper.FindById<DesignCableChannelExt>(id);
                        if (designCableChannelExt.HasVal())
                        {
                            designCableChannelExt.Id = UserMd5(designCableChannelExt.Id);
                            await DbHelper.DeleteByIdAsync<DesignCableChannelExt>(id);
                            await DbHelper.InsertAsync<DesignCableChannelExt>(designCableChannelExt);
                        }
                        if (!string.IsNullOrEmpty(item.Start_Id))
                        {
                            item.Start_Id = UserMd5(item.Start_Id);
                        }
                        if (!string.IsNullOrEmpty(item.End_Id))
                        {
                            item.End_Id = UserMd5(item.End_Id);
                        }
                        await DbHelper.DeleteByIdAsync<DesignCableChannel>(id);
                        await DbHelper.InsertAsync<DesignCableChannel>(item);
                    }
                    #endregion

                    #region 获取杆上设备数据
                    var designCabeleEquipments = await DbHelper.FindAllAsync<DesignCableEquipment>(x => x.ProjectId == projectId);
                    foreach (var item in designCabeleEquipments)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(item.Id);
                        var designCableDeviceExt = DbHelper.FindById<DesignCableDeviceExt>(id);
                        if (designCableDeviceExt.HasVal())
                        {
                            designCableDeviceExt.Id = UserMd5(designCableDeviceExt.Id);
                            await DbHelper.DeleteByIdAsync<DesignCableDeviceExt>(id);
                            await DbHelper.InsertAsync(designCableDeviceExt);
                        }

                        if (!string.IsNullOrEmpty(item.ParentId))
                        {
                            item.ParentId = UserMd5(item.ParentId);
                        }
                        await DbHelper.DeleteByIdAsync<DesignCableEquipment>(id);
                        await DbHelper.InsertAsync(item);
                    }

                    #endregion

                    #region 架空杆上设备
                    var designOverheadEquipments = await DbHelper.FindAllAsync<DesignOverheadEquipment>(x => x.ProjectId == projectId);
                    foreach (var item in designOverheadEquipments)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(item.Id);
                        var designOverheadEquipmentExt = DbHelper.FindById<DesignOverheadEquipmentExt>(id);
                        if (designOverheadEquipmentExt.HasVal())
                        {
                            designOverheadEquipmentExt.Id = UserMd5(designOverheadEquipmentExt.Id);
                            await DbHelper.DeleteByIdAsync<DesignOverheadEquipmentExt>(id);
                            await DbHelper.InsertAsync(designOverheadEquipmentExt);
                        }
                        if (!string.IsNullOrEmpty(item.Main_ID))
                        {
                            item.Main_ID = UserMd5(item.Main_ID);
                        }
                        if (!string.IsNullOrEmpty(item.Sub_ID))
                        {
                            item.Sub_ID = UserMd5(item.Sub_ID);
                        }
                        await DbHelper.DeleteByIdAsync<DesignOverheadEquipment>(id);
                        await DbHelper.InsertAsync(item);
                    }
                    #endregion

                    #region 线路
                    var designLines = await DbHelper.FindAllAsync<DesignLine>(x => x.ProjectId == projectId);
                    foreach (var item in designLines)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(item.Id);
                        item.GroupId = UserMd5(item.GroupId);
                        var designLineExt = DbHelper.FindById<DesignLineExt>(id);
                        if (designLineExt.HasVal())
                        {
                            designLineExt.Id = UserMd5(designLineExt.Id);
                            await DbHelper.DeleteByIdAsync<DesignLineExt>(id);
                            await DbHelper.InsertAsync(designLineExt);
                        }
                        if (!string.IsNullOrEmpty(item.Start_ID))
                        {
                            item.Start_ID = UserMd5(item.Start_ID);
                        }
                        if (!string.IsNullOrEmpty(item.End_ID))
                        {
                            item.End_ID = UserMd5(item.End_ID);
                        }
                        await DbHelper.DeleteByIdAsync<DesignLine>(id);
                        await DbHelper.InsertAsync(item);
                    }
                    #endregion

                    #region 地物
                    var designMarks = await DbHelper.FindAllAsync<DesignMark>(x => x.ProjectId == projectId);
                    foreach (var item in designMarks)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(item.Id);
                        var designMarkExt = DbHelper.FindById<DesignMarkExt>(id);
                        if (designMarkExt.HasVal())
                        {
                            designMarkExt.Id = UserMd5(designMarkExt.Id);
                            await DbHelper.DeleteByIdAsync<DesignMarkExt>(id);
                            await DbHelper.InsertAsync(designMarkExt);
                        }
                        await DbHelper.DeleteByIdAsync<DesignMark>(id);
                        await DbHelper.InsertAsync(item);
                    }
                    #endregion

                    #region 拉线
                    var designPullLines = await DbHelper.FindAllAsync<DesignPullLine>(x => x.ProjectId == projectId);
                    foreach (var item in designPullLines)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(item.Id);
                        if (!string.IsNullOrEmpty(item.Main_Id))
                        {
                            item.Main_Id = UserMd5(item.Main_Id);
                        }
                        await DbHelper.DeleteByIdAsync<DesignPullLine>(id);
                        await DbHelper.InsertAsync(item);
                    }
                    #endregion

                    // await RepairReportDesignDataAsync(requestInfo);

                    SuccessNum++;
                }
                catch (Exception e)
                {
                    FailNum++;
                    FailId.Add(projectId + " " + e.Message);
                    //给出异常信息

                }
                //告知客户端批注消息更新
                foreach (var item in SocketHandler.userDic)
                {
                    item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                    SocketHandler.SendMessage(item);
                }
                continue;

            }

        }      

        /// <summary>
        ///    修复拆除数据id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task RepairDismantleDataId(List<string> request)
        {
            System.Threading.Thread.Sleep(4000);
            AllNum = 0;
            FailNum = 0;
            FailId = new List<string>();
            SuccessNum = 0;
            if (request == null || request.Count == 0)
            {
                var proData = await DbHelper.FindAllAsync<Project>();
                request = proData.ToList().Select(x => x.Id).ToList();

            }
            AllNum = request.Count;
            //获取数据库数据
            foreach (var projectId in request)
            {

                try
                {
                    //获取项目信息
                    var proInfo = DbHelper.FindById<Project>(projectId);
                    var projectState = await DbHelper.FindAsync<ProjectState>(it => it.Id == projectId);
                    if (projectState == null) continue;
                    if (!(projectState.Status == ProjectStatus.勘察中 ||
                        projectState.Status == ProjectStatus.已勘察 ||
                        projectState.Status == ProjectStatus.设计中 ||
                        projectState.Status == ProjectStatus.设计完成)
                    )
                    {
                        FailNum++;
                        FailId.Add(projectId + " " + projectState.Status.GetText());
                        //告知客户端批注消息更新
                        foreach (var item in SocketHandler.userDic)
                        {
                            item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                            SocketHandler.SendMessage(item);
                        }
                        continue;
                    }
                    var engineer = DbHelper.FindById<Engineer>(proInfo.EngineerId);

                    #region 获取杆塔数据    DesignTower: id\parentid;DesignTowerExt:id
                    var designTowers = await DbHelper.FindAllAsync<DismantleTower>(x => x.ProjectId == projectId);
                    foreach (var item in designTowers)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(UserMd5(item.Id));
                        var surveyTowerExt = DbHelper.FindById<DismantleTowerExt>(id);
                        if (surveyTowerExt.HasVal())
                        {
                            surveyTowerExt.Id = UserMd5(UserMd5(surveyTowerExt.Id));
                            await DbHelper.DeleteByIdAsync<DismantleTowerExt>(id);
                            await DbHelper.InsertAsync<DismantleTowerExt>(surveyTowerExt);
                        }
                        if (!string.IsNullOrEmpty(item.ParentId))
                        {
                            item.ParentId = UserMd5(UserMd5(item.ParentId));
                        }
                        await DbHelper.DeleteByIdAsync<DismantleTower>(id);
                        await DbHelper.InsertAsync<DismantleTower>(item);
                    }
                    #endregion

                    #region 获取电缆井数据
                    var designCables = await DbHelper.FindAllAsync<DismantleCable>(x => x.ProjectId == projectId);

                    foreach (var item in designCables)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(UserMd5(item.Id));
                        var designCableExt = DbHelper.FindById<DismantleCableExt>(id);
                        if (designCableExt.HasVal())
                        {
                            designCableExt.Id = UserMd5(UserMd5(designCableExt.Id));
                            await DbHelper.DeleteByIdAsync<DismantleCableExt>(id);
                            await DbHelper.InsertAsync<DismantleCableExt>(designCableExt);
                        }
                        if (!string.IsNullOrEmpty(item.ParentId))
                        {
                            item.ParentId = UserMd5(UserMd5(item.ParentId));
                        }
                        await DbHelper.DeleteByIdAsync<DismantleCable>(id);
                        await DbHelper.InsertAsync<DismantleCable>(item);
                    }
                    #endregion

                    #region 获取电缆通道数据
                    var designCableChannels = await DbHelper.FindAllAsync<DismantleCableChannel>(x => x.ProjectId == projectId);
                    foreach (var item in designCableChannels)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(UserMd5(item.Id));
                        var designCableChannelExt = DbHelper.FindById<DismantleCableChannelExt>(id);
                        if (designCableChannelExt.HasVal())
                        {
                            designCableChannelExt.Id = UserMd5(UserMd5(designCableChannelExt.Id));
                            await DbHelper.DeleteByIdAsync<DismantleCableChannelExt>(id);
                            await DbHelper.InsertAsync(designCableChannelExt);
                        }
                        if (!string.IsNullOrEmpty(item.Start_Id))
                        {
                            item.Start_Id = UserMd5(UserMd5(item.Start_Id));
                        }
                        if (!string.IsNullOrEmpty(item.End_Id))
                        {
                            item.End_Id = UserMd5(UserMd5(item.End_Id));
                        }
                        await DbHelper.DeleteByIdAsync<DismantleCableChannel>(id);
                        await DbHelper.InsertAsync<DismantleCableChannel>(item);
                    }
                    #endregion

                    #region 获取杆上设备数据
                    var designCabeleEquipments = await DbHelper.FindAllAsync<DismantleCableEquipment>(x => x.ProjectId == projectId);
                    foreach (var item in designCabeleEquipments)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(UserMd5(item.Id));
                        var designCableDeviceExt = DbHelper.FindById<DismantleCableDeviceExt>(id);
                        if (designCableDeviceExt.HasVal())
                        {
                            designCableDeviceExt.Id = UserMd5(UserMd5(designCableDeviceExt.Id));
                            await DbHelper.DeleteByIdAsync<DismantleCableDeviceExt>(id);
                            await DbHelper.InsertAsync(designCableDeviceExt);
                        }

                        if (!string.IsNullOrEmpty(item.ParentId))
                        {
                            item.ParentId = UserMd5(UserMd5(item.ParentId));
                        }
                        await DbHelper.DeleteByIdAsync<DismantleCableEquipment>(id);
                        await DbHelper.InsertAsync(item);
                    }

                    #endregion

                    #region 架空杆上设备
                    var designOverheadEquipments = await DbHelper.FindAllAsync<DismantleOverheadEquipment>(x => x.ProjectId == projectId);
                    foreach (var item in designOverheadEquipments)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(UserMd5(item.Id));
                        var designOverheadEquipmentExt = DbHelper.FindById<DismantleOverheadEquipmentExt>(id);
                        if (designOverheadEquipmentExt.HasVal())
                        {
                            designOverheadEquipmentExt.Id = UserMd5(UserMd5(designOverheadEquipmentExt.Id));
                            await DbHelper.DeleteByIdAsync<DismantleOverheadEquipmentExt>(id);
                            await DbHelper.InsertAsync(designOverheadEquipmentExt);
                        }
                        if (!string.IsNullOrEmpty(item.Main_ID))
                        {
                            item.Main_ID = UserMd5(UserMd5(item.Main_ID));
                        }
                        if (!string.IsNullOrEmpty(item.Sub_ID))
                        {
                            item.Sub_ID = UserMd5(UserMd5(item.Sub_ID));
                        }
                        await DbHelper.DeleteByIdAsync<DismantleOverheadEquipment>(id);
                        await DbHelper.InsertAsync(item);
                    }
                    #endregion

                    #region 线路
                    var designLines = await DbHelper.FindAllAsync<DismantleLine>(x => x.ProjectId == projectId);
                    foreach (var item in designLines)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(UserMd5(item.Id));
                        item.GroupId = UserMd5(UserMd5(item.GroupId));
                        var designLineExt = DbHelper.FindById<DismantleLineExt>(id);
                        if (designLineExt.HasVal())
                        {
                            designLineExt.Id = UserMd5(UserMd5(designLineExt.Id));
                            await DbHelper.DeleteByIdAsync<DismantleLineExt>(id);
                            await DbHelper.InsertAsync(designLineExt);
                        }
                        if (!string.IsNullOrEmpty(item.Start_ID))
                        {
                            item.Start_ID = UserMd5(UserMd5(item.Start_ID));
                        }
                        if (!string.IsNullOrEmpty(item.End_ID))
                        {
                            item.End_ID = UserMd5(UserMd5(item.End_ID));
                        }
                        await DbHelper.DeleteByIdAsync<DismantleLine>(id);
                        await DbHelper.InsertAsync(item);
                    }
                    #endregion

                    #region 地物
                    var designMarks = await DbHelper.FindAllAsync<DismantleMark>(x => x.ProjectId == projectId);
                    foreach (var item in designMarks)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(UserMd5(item.Id));
                        var designMarkExt = DbHelper.FindById<DismantleMarkExt>(id);
                        if (designMarkExt.HasVal())
                        {
                            designMarkExt.Id = UserMd5(UserMd5(designMarkExt.Id));
                            await DbHelper.DeleteByIdAsync<DismantleMarkExt>(id);
                            await DbHelper.InsertAsync(designMarkExt);
                        }
                        await DbHelper.DeleteByIdAsync<DismantleMark>(id);
                        await DbHelper.InsertAsync(item);
                    }
                    #endregion

                    #region 拉线
                    var designPullLines = await DbHelper.FindAllAsync<DismantlePullLine>(x => x.ProjectId == projectId);
                    foreach (var item in designPullLines)
                    {
                        string id = item.Id;
                        item.Id = UserMd5(UserMd5(item.Id));
                        if (!string.IsNullOrEmpty(item.Main_Id))
                        {
                            item.Main_Id = UserMd5(UserMd5(item.Main_Id));
                        }
                        await DbHelper.DeleteByIdAsync<DismantlePullLine>(id);
                        await DbHelper.InsertAsync(item);
                    }
                    #endregion

                    // await RepairReportDesignDataAsync(requestInfo);

                    SuccessNum++;
                }
                catch (Exception e)
                {
                    FailNum++;
                    FailId.Add(projectId + " " + e.Message);
                    //给出异常信息

                }
                //告知客户端批注消息更新
                foreach (var item in SocketHandler.userDic)
                {
                    item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                    SocketHandler.SendMessage(item);
                }
                continue;

            }

        }

        /// <summary>
        ///    修复数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [UseTransaction]
        public async Task RepairData(List<string> request, bool isCorrect)
        {
            System.Threading.Thread.Sleep(4000);
            AllNum = 0;
            FailNum = 0;
            FailId = new List<string>();
            SuccessNum = 0;
            if (request == null || request.Count == 0)
            {
                var proData = await DbHelper.FindAllAsync<Project>();
                request = proData.ToList().Select(x => x.Id).ToList();

            }
            AllNum = request.Count;
            //获取数据库数据
            foreach (var projectId in request)
            {

                try
                {
                     //获取项目信息
                    var proInfo = DbHelper.FindById<Project>(projectId);
                    var projectState = await DbHelper.FindAsync<ProjectState>(it => it.Id == projectId);
                    if (projectState == null) continue;
                    if (!(projectState.Status == ProjectStatus.勘察中 ||
                        projectState.Status == ProjectStatus.已勘察 ||
                        projectState.Status == ProjectStatus.设计中 ||
                        projectState.Status == ProjectStatus.设计完成)
                    )
                    {
                        FailNum++;
                        FailId.Add(projectId + " " + projectState.Status.GetText());
                        //告知客户端批注消息更新
                        foreach (var item in SocketHandler.userDic)
                        {
                            item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                            SocketHandler.SendMessage(item);
                        }
                        continue;
                    }
                    var engineer = DbHelper.FindById<Engineer>(proInfo.EngineerId);
                    ReportSurveyDataRequest requestInfo = new ReportSurveyDataRequest
                    {
                        DesignData = new CollectionDataRequest(),
                        SurveyData = new CollectionDataRequest(),
                        ProjectId = projectId
                    };
                    #region 获取杆塔数据
                    var designTowers = await DbHelper.FindAllAsync<DesignTower>(x => x.ProjectId == projectId);
                    var surveyTowers = await DbHelper.FindAllAsync<SurveyTower>(x => x.ProjectId == projectId);
                    var designTowerExts = await DbHelper.FindAllAsync<DesignTowerExt>(x => x.ProjectId == projectId);
                    var surveyTowerExts = await DbHelper.FindAllAsync<SurveyTowerExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.Towers =
                    (from item in designTowers
                     select
                     new DesignTowerDto
                     {
                         Id = item.Id,
                         ProjectId = item.ProjectId,
                         ParentId = item.ParentId,
                         PreNodeType = item.PreNodeType,
                         Code = item.Code,
                         Specification = item.T_Rod + "*" + item.T_Height,
                         T_Type = item.T_Type,
                         PoleTypeCode = string.IsNullOrEmpty(designTowerExts.FirstOrDefault(x => x.Id == item.Id)?.PoleTypeCode) ? item.T_Type : designTowerExts.FirstOrDefault(x => x.Id == item.Id)?.PoleTypeCode,
                         T_Segment = item.T_Segment,
                         T_Sort = item.T_Sort,
                         KVLevel = item.KVLevel,
                         Lat_WGS84 = item.Lat_WGS84,
                         Lon_WGS84 = item.Lon_WGS84,
                         State = item.State,
                         SurveyTime = item.SurveyTime,
                         Remark = item.Remark,
                         ElectrifiedWork = item.ElectrifiedWork,
                         Surveyor = item.Surveyor
                     }).ToList();
                    requestInfo.SurveyData.Towers =
                        (from item in surveyTowers
                         select
                                new DesignTowerDto
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    ParentId = item.ParentId,
                                    PreNodeType = item.PreNodeType,
                                    Code = item.Code,
                                    Specification = item.T_Rod + "*" + item.T_Height,
                                    PoleTypeCode = isCorrect? surveyTowerExts.FirstOrDefault(x => x.Id == item.Id)?.PoleTypeCode: item.T_Type,
                                    T_Segment = item.T_Segment,
                                    T_Sort = item.T_Sort,
                                    KVLevel = item.KVLevel,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    ElectrifiedWork = item.ElectrifiedWork,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    #endregion

                    #region 获取杆型方案
                    //获取杆型方案
                    var towers = new List<DesignTowerDto>();
                    towers.AddRange(requestInfo.SurveyData?.Towers);
                    towers.AddRange(requestInfo.DesignData?.Towers);
                    var towerModules = await TowerModules(towers, engineer.LibId);
                    #endregion

                    #region 获取电缆井数据
                    var designCables = await DbHelper.FindAllAsync<DesignCable>(x => x.ProjectId == projectId);
                    var surveyCables = await DbHelper.FindAllAsync<SurveyCable>(x => x.ProjectId == projectId);
                    var designCableExts = await DbHelper.FindAllAsync<DesignCableExt>(x => x.ProjectId == projectId);
                    var surveyCableExts = await DbHelper.FindAllAsync<SurveyCableExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.Cables =
                        (from item in designCables
                         select
                                new CableRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    ParentId = item.ParentId,
                                    PreNodeType = item.PreNodeType,
                                    Code = item.Code,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    C_Type = item.C_Type,
                                    C_ModeID = designCableExts.FirstOrDefault(x => x.Id == item.Id)?.C_ModeID,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    ElectrifiedWork = item.ElectrifiedWork,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    requestInfo.SurveyData.Cables =
                        (from item in surveyCables
                         select
                                new CableRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    ParentId = item.ParentId,
                                    PreNodeType = item.PreNodeType,
                                    Code = item.Code,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    C_Type = item.C_Type,
                                    C_ModeID = surveyCableExts.FirstOrDefault(x => x.Id == item.Id)?.C_ModeID,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    ElectrifiedWork = item.ElectrifiedWork,
                                    Surveyor = item.Surveyor

                                }).ToList();
                    #endregion

                    #region 获取电缆通道数据
                    var designCableChannels = await DbHelper.FindAllAsync<DesignCableChannel>(x => x.ProjectId == projectId);
                    var surveyCableChannels = await DbHelper.FindAllAsync<SurveyCableChannel>(x => x.ProjectId == projectId);
                    var designCableChannelExts = await DbHelper.FindAllAsync<DesignCableChannelExt>(x => x.ProjectId == projectId);
                    var surveyCableChannelExts = await DbHelper.FindAllAsync<SurveyCableChannelExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.CableChannels =
                    (from item in designCableChannels
                     select
                            new CableChannelRequest
                            {
                                Id = item.Id,
                                ProjectId = item.ProjectId,
                                Start_Id = item.Start_Id,
                                StartNodeType = item.StartNodeType,
                                End_Id = item.End_Id,
                                EndNodeType = item.EndNodeType,
                                C_LayMode = item.C_LayMode,
                                C_ModeId = item.C_ModeId,
                                State = item.State,
                                SurveyTime = item.SurveyTime,
                                Remark = item.Remark,
                                Length = item.Length,
                                Surveyor = item.Surveyor
                            }).ToList();
                    requestInfo.SurveyData.CableChannels =
                        (from item in surveyCableChannels
                         select
                                new CableChannelRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    Start_Id = item.Start_Id,
                                    StartNodeType = item.StartNodeType,
                                    End_Id = item.End_Id,
                                    EndNodeType = item.EndNodeType,
                                    C_LayMode = item.C_LayMode,
                                    C_ModeId = item.C_ModeId,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    Length = item.Length,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    #endregion

                    #region 获取杆上设备数据
                    var designCableEquipmentss = await DbHelper.FindAllAsync<DesignCableEquipment>(x => x.ProjectId == projectId);
                    var surveyCabeleEquipments = await DbHelper.FindAllAsync<SurveyCableEquipment>(x => x.ProjectId == projectId);
                    //var designCableEquipmentsExts = await DbHelper.FindAllAsync<DesignCableChannelExt>(x => x.ProjectId == projectId);
                    //var surveyCableChannelExts = await DbHelper.FindAllAsync<SurveyCableChannelExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.CableEquipments =
                    (from item in designCableEquipmentss
                     select
                            new CableEquipmentRequest
                            {
                                Id = item.Id,
                                ProjectId = item.ProjectId,
                                Name = item.Name,
                                Code = item.Code,
                                Type = item.Type,
                                ParentId = item.ParentId,
                                PreNodeType = item.PreNodeType,
                                EquipModel = item.EquipModel,
                                Lat_WGS84 = item.Lat_WGS84,
                                Lon_WGS84 = item.Lon_WGS84,
                                State = item.State,
                                SurveyTime = item.SurveyTime,
                                Remark = item.Remark,
                                ElectrifiedWork = item.ElectrifiedWork,
                                Surveyor = item.Surveyor
                            }).ToList();
                    requestInfo.SurveyData.CableEquipments =
                        (from item in surveyCabeleEquipments
                         select
                                new CableEquipmentRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    Name = item.Name,
                                    Code = item.Code,
                                    Type = item.Type,
                                    ParentId = item.ParentId,
                                    PreNodeType = item.PreNodeType,
                                    EquipModel = item.EquipModel,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    ElectrifiedWork = item.ElectrifiedWork,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    #endregion

                    #region 架空杆上设备
                    var designOverheadEquipments = await DbHelper.FindAllAsync<DesignOverheadEquipment>(x => x.ProjectId == projectId);
                    var surveyOverheadEquipmentss = await DbHelper.FindAllAsync<SurveyOverheadEquipment>(x => x.ProjectId == projectId);
                    //var designCableEquipmentsExts = await DbHelper.FindAllAsync<DesignCableChannelExt>(x => x.ProjectId == projectId);
                    //var surveyCableChannelExts = await DbHelper.FindAllAsync<SurveyCableChannelExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.OverheadEquipments =
                    (from item in designOverheadEquipments
                     select
                            new OverheadEquipmentRequest
                            {
                                Id = item.Id,
                                ProjectId = item.ProjectId,
                                Name = item.Name,
                                Type = item.Type,
                                Main_ID = item.Main_ID,
                                Sub_ID = item.Sub_ID,
                                Capacity = item.Capacity.ToEnum<TransformerCapacity>(),
                                FixMode = item.FixMode,
                                Lat_WGS84 = item.Lat_WGS84,
                                Lon_WGS84 = item.Lon_WGS84,
                                State = item.State,
                                SurveyTime = item.SurveyTime,
                                Remark = item.Remark,
                                Surveyor = item.Surveyor
                            }).ToList();
                    requestInfo.SurveyData.OverheadEquipments =
                        (from item in surveyOverheadEquipmentss
                         select
                                new OverheadEquipmentRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    Name = item.Name,
                                    Type = item.Type,
                                    Main_ID = item.Main_ID,
                                    Sub_ID = item.Sub_ID,
                                    Capacity = item.Capacity,
                                    FixMode = item.FixMode,
                                    Lat_WGS84 = item.Lat_WGS84,
                                    Lon_WGS84 = item.Lon_WGS84,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    #endregion

                    #region 线路
                    var designLines = await DbHelper.FindAllAsync<DesignLine>(x => x.ProjectId == projectId);
                    var surveyLines = await DbHelper.FindAllAsync<SurveyLine>(x => x.ProjectId == projectId);
                    var designLineExts = await DbHelper.FindAllAsync<DesignLineExt>(x => x.ProjectId == projectId);
                    var surveyLineExts = await DbHelper.FindAllAsync<SurveyLineExt>(x => x.ProjectId == projectId);
                    requestInfo.DesignData.Lines =
                    (from item in designLines
                     select
                            new LineRequest
                            {
                                Id = item.Id,
                                ProjectId = item.ProjectId,
                                Name = item.Name,
                                LoopName = item.LoopName,
                                Start_ID = item.Start_ID,
                                StartNodeType = item.StartNodeType,
                                End_ID = item.End_ID,
                                EndNodeType = item.EndNodeType,
                                L_Type = item.L_Type,
                                L_Mode = item.L_Mode,
                                L_ModeID = designLineExts.FirstOrDefault(x => x.Id == item.Id)?.L_ModeID,
                                Length = item.Length,
                                KVLevel = item.KVLevel,
                                IsCable = item.IsCable,
                                LoopSerial = item.LoopSerial,
                                GroupId = item.GroupId,
                                State = item.State,
                                SurveyTime = item.SurveyTime,
                                Remark = item.Remark,
                                Index = item.Index,
                                Surveyor = item.Surveyor
                            }).ToList();
                    requestInfo.SurveyData.Lines =
                        (from item in surveyLines
                         select
                                new LineRequest
                                {
                                    Id = item.Id,
                                    ProjectId = item.ProjectId,
                                    Name = item.Name,
                                    LoopName = item.LoopName,
                                    Start_ID = item.Start_ID,
                                    StartNodeType = item.StartNodeType,
                                    End_ID = item.End_ID,
                                    EndNodeType = item.EndNodeType,
                                    L_Type = item.L_Type,
                                    L_Mode = item.L_Mode,
                                    L_ModeID = designLineExts.FirstOrDefault(x => x.Id == item.Id)?.L_ModeID,
                                    Length = item.Length,
                                    KVLevel = item.KVLevel,
                                    IsCable = item.IsCable,
                                    LoopSerial = item.LoopSerial,
                                    GroupId = item.GroupId,
                                    State = item.State,
                                    SurveyTime = item.SurveyTime,
                                    Remark = item.Remark,
                                    Index = item.Index,
                                    Surveyor = item.Surveyor
                                }).ToList();
                    #endregion
                    
                    await RepairReportSurveyDataAsync(requestInfo);
                    SuccessNum++;
                }
                catch (Exception e)
                {
                    FailNum++;
                    FailId.Add(projectId + " " + e.Message);
                    //给出异常信息

                }
                //告知客户端批注消息更新
                foreach (var item in SocketHandler.userDic)
                {
                    item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                    SocketHandler.SendMessage(item);
                }
                continue;

            }

        }


        /// <summary>
        ///     清除设计数据
        /// </summary>
        /// <returns></returns>
        
        public async Task ClearDesignData()
        {
            try
            {
                var proData = await DbHelper.FindAllAsync<Project>();
                foreach (var project in proData)
                {
                    await ClearDesignAsync(project.Id);
                }
            }
            catch (Exception e)
            {
                ///
            }
        }



        #endregion

        #region 完成勘察
        /// <summary>
        /// 完成勘察
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [UseTransaction]
        public async Task CompleteSurveyAsync(string projectId)
        {
            using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectLockKey, projectId), 5))
            {
                var (project, projectState, _) = await CheckSurveyPermissionAsync(projectId);
                CheckProjectState(projectState, ProjectStatus.未勘察, ProjectStatus.勘察中);
                await ChangeProjectStateAsync(project.Id, ProjectStatusType.设计状态, ProjectStatus.已勘察.ToInt(), "完成勘察");
                //推送模板消息
                await _capPublisher.PublishAsync("PublishTemplateNews", new PublishTemplateNewsEvent
                {
                    ProjectId = projectId,
                    TemplateType = NewsTemplateType.未勘察
                });
            }
        }
        #endregion

        #region 上传交底数据
        /// <summary>
        /// 上传交底数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [UseTransaction]
        public async Task ReportDisclosureDataAsync(ReportDisclosureDataRequest request)
        {
            using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectLockKey, request.ProjectId), 5))
            {
                var (project, projectState, _) = await CheckSurveyPermissionAsync(request.ProjectId);
                CheckProjectState(projectState, ProjectStatus.设计完成);
                //if (project.Stage == ProjectStage.初设 || project.Stage == ProjectStage.可研)
                //{
                //    throw new BusinessException(ResponseErrorCode.ProcessError, $"项目阶段处于[{project.Stage.GetText()}],无法进行此操作");
                //}
                #region 写入交底数据
                if (request.Disclosures.HasVal())
                {
                    var disclosures = new List<ProjectDisclosure>();
                    var disclosureItems = new List<ProjectDisclosureItem>();
                    foreach (var item in request.Disclosures)
                    {
                        var disclosure = item.MapTo<ProjectDisclosure>();
                        disclosure.ProjectId = request.ProjectId;
                        disclosure.CreatedBy = CurrentUser.Id;
                        if (item.Status == ProjectDisclosureStatus.点位有误 && !item.Items.HasVal())
                        {
                            throw new BusinessException(ResponseErrorCode.BusinessError, $"点位有误的[{item.ItemType.GetText()}]-[{item.ItemId}]，交底意见，文字、录音、照片至少添加一种");
                        }
                        if (item.Items.HasVal())
                        {
                            var dItems = item.Items.MapTo<List<ProjectDisclosureItem>>();
                            foreach (var ditem in dItems)
                            {
                                ditem.DisclosureId = disclosure.Id;
                                ditem.ProjectId = request.ProjectId;
                                if (ditem.Category == ProjectDisclosureItemType.文本)
                                {
                                    if (!ditem.Notes.HasVal())
                                    {
                                        throw new BusinessException(ResponseErrorCode.BusinessError, $"点位[{item.ItemType.GetText()}]-[{item.ItemId}]，交底意见文字不能为空");
                                    }
                                }
                                else if (!ditem.FileId.HasVal())
                                {
                                    throw new BusinessException(ResponseErrorCode.BusinessError, $"点位[{item.ItemType.GetText()}]-[{item.ItemId}]，文件编号不能为空");
                                }
                            }
                            disclosureItems.AddRange(dItems);
                        }
                        disclosures.Add(disclosure);
                    }

                    await DbHelper.DeleteAsync<ProjectDisclosure>(p => p.ProjectId == request.ProjectId);

                    await DbHelper.DeleteAsync<ProjectDisclosureItem>(p => p.ProjectId == request.ProjectId);
                    await DbHelper.BulkInsertAsync(disclosures);
                    if (disclosureItems.HasVal())
                    {
                        await DbHelper.BulkInsertAsync(disclosureItems);
                    }
                }
                #endregion

                #region 写入轨迹数据
                var projectTraceRecordType = request.TrackRecords.FirstOrDefault().RecordType;
                await DbHelper.DeleteAsync<ProjectTrackRecord>(it => it.ProjectId == request.ProjectId && it.RecordType == projectTraceRecordType);
                if (request.TrackRecords.HasVal())
                {
                    var tracks = request.TrackRecords.MapTo<List<ProjectTrackRecord>>();
                    tracks.ForEach(it =>
                    {
                        it.ProjectId = request.ProjectId;
                    });
                    await DbHelper.BulkInsertAsync(tracks);
                }
                #endregion

                #region 更改工程状态
                if ((projectState.OutsideStatus & ProjectOutsideStatus.交底中) != ProjectOutsideStatus.交底中)
                {
                    await ChangeProjectStateAsync(project.Id, ProjectStatusType.外部状态, ProjectOutsideStatus.交底中.ToInt(), "上传交底数据");
                }
                #endregion

                //轨迹转存postgis
                await gisTransformService.GisTransformDisclosureTrack(new List<string>() { request.ProjectId });
            }
        }
        #endregion

        #region 完成交底
        /// <summary>
        /// 完成交底
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [UseTransaction]
        public async Task CompleteDisclosureAsync(CompleteDisclosureRequest request)
        {
            using (var lk = _redisClient.Lock(string.Format(CygConstant.ProjectLockKey, request.ProjectId), 5))
            {
                var (project, projectState, _) = await CheckSurveyPermissionAsync(request.ProjectId);
                if ((projectState.OutsideStatus & ProjectOutsideStatus.交底中) != ProjectOutsideStatus.交底中)
                {
                    throw new BusinessException(ResponseErrorCode.ProcessError, $"项目状态未处于{ProjectOutsideStatus.交底中},不允许交底完成");
                }
                var disclosures = await DbHelper.FindAllAsync<ProjectDisclosure>(p => p.ProjectId == request.ProjectId);

                var disclosureTowerCount = disclosures.Count(m => m.ItemType == ProjectDisclosureType.杆塔);
                var towerCount = await DbHelper.CountAsync<DesignTower>(it => it.ProjectId == request.ProjectId);
                if (towerCount != disclosureTowerCount)
                {
                    throw new BusinessException(ResponseErrorCode.BusinessError, "部分杆塔未交底");
                }

                var disclosureCableCount = disclosures.Count(m => m.ItemType == ProjectDisclosureType.电缆井);
                var cableCount = await DbHelper.CountAsync<DesignCable>(it => it.ProjectId == request.ProjectId);
                if (cableCount != disclosureCableCount)
                {
                    throw new BusinessException(ResponseErrorCode.BusinessError, "部分电缆井未交底");
                }

                var disclosureCableDeviceCount = disclosures.Count(m => m.ItemType == ProjectDisclosureType.电气设备);
                var cableDeviceCount = await DbHelper.CountAsync<DesignCableEquipment>(it => it.ProjectId == request.ProjectId);
                if (cableDeviceCount != disclosureCableDeviceCount)
                {
                    throw new BusinessException(ResponseErrorCode.BusinessError, "部分电气设备未交底");
                }

                await ChangeProjectStateAsync(project.Id, ProjectStatusType.外部状态, ProjectOutsideStatus.交底完成.ToInt(), "交底完成");
            }
        }
        #endregion

        #region 检查勘测人员操作权限
        /// <summary>
        /// 检查勘测人员操作权限
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<(Project, ProjectState, Engineer)> CheckSurveyPermissionAsync(string projectId)
        {
            var project = await DbHelper.FindByIdAsync<Project>(projectId);
            if (!project.HasVal())
            {
                throw new BusinessException(ResponseErrorCode.BusinessError, "项目信息不存在或已被删除");
            }
            var projectCollection = await DbHelper.FindByIdAsync<Engineer>(project.EngineerId);
            if (!projectCollection.HasVal())
            {
                throw new BusinessException(ResponseErrorCode.BusinessError, "工程信息不存在");
            }
            var pau = await DbHelper.FindAsync<ProjectAllotUser>(p => p.ProjectId == projectId && p.UserId == CurrentUser.Id && p.ArrangeType == ProjectArrangeType.勘察);
            if (!pau.HasVal())
            {
                throw new BusinessException(ResponseErrorCode.BusinessError, "请求被拒绝，你暂无操作该工程的权限");
            }
            var projectState = await DbHelper.FindAsync<ProjectState>(it => it.Id == projectId);
            return (project, projectState, projectCollection);
        }
        #endregion

        #region 清除勘测数据
        /// <summary>
        /// 清除勘测数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        async Task ClearSurveyAsync(string projectId)
        {
            await DbHelper.DeleteAsync<SurveyTower>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyTowerExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyOverheadEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyOverheadEquipmentExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyMark>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyLine>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyLineExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCableEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCableDeviceExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCable>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCableExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCableChannel>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyMedia>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyPullLine>(it => it.ProjectId == projectId);

        }
        #endregion

        #region 清除设计数据
        /// <summary>
        /// 清除设计数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        async Task ClearDesignAsync(string projectId)
        {
            await DbHelper.DeleteAsync<DesignCable>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableChannel>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableChannelExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableChannelProfile>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCablehead>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableDeviceExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignElectricMeter>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignLine>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignMark>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignMarkExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignMaterialModify>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignOverheadEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignTower>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignLineExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignOverheadEquipmentExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignTowerExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignMedia>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignPullLine>(it => it.ProjectId == projectId);
        }
        #endregion

        #region 清除拆除数据
        /// <summary>
        /// 清除拆除数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        async Task ClearDismantleAsync(string projectId)
        {
            await DbHelper.DeleteAsync<DismantleCable>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableChannel>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableChannelExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableChannelProfile>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCablehead>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableDeviceExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleElectricMeter>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleLine>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleMark>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleMarkExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleMaterialModify>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleOverheadEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleTower>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleLineExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleOverheadEquipmentExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleTowerExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleMedia>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantlePullLine>(it => it.ProjectId == projectId);
        }
        #endregion

        #region 修复数据
        #region 清除勘测数据【修复】
        /// <summary>
        /// 清除勘测数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        async Task RepairClearSurveyAsync(string projectId)
        {
            await DbHelper.DeleteAsync<SurveyTower>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyTowerExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyOverheadEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyOverheadEquipmentExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyLine>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyLineExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCableEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCableDeviceExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCable>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCableExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyCableChannel>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<SurveyPullLine>(it => it.ProjectId == projectId);
        }
        #endregion

        #region 修复清除设计数据
        /// <summary>
        /// 清除设计数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        async Task RepairClearDesignAsync(string projectId)
        {
            await DbHelper.DeleteAsync<DesignCable>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableChannel>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableChannelExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableDeviceExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignLine>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignOverheadEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignTower>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignCableExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignLineExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignOverheadEquipmentExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignTowerExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DesignPullLine>(it => it.ProjectId == projectId);
        }
        #endregion

        #region 修复清除拆除数据
        /// <summary>
        /// 清除拆除数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        async Task RepairClearDismantleAsync(string projectId)
        {
            await DbHelper.DeleteAsync<DismantleCable>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableChannel>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableChannelExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableDeviceExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleLine>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleOverheadEquipment>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleTower>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleCableExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleLineExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleOverheadEquipmentExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantleTowerExt>(it => it.ProjectId == projectId);
            await DbHelper.DeleteAsync<DismantlePullLine>(it => it.ProjectId == projectId);
        }
        #endregion

        #region 写入勘测数据
        /// <summary>
        /// 写入勘测数据
        /// </summary>
        /// <param name="towers"></param>
        /// <param name="cables"></param>
        /// <param name="cableChannels"></param>
        /// <param name="cableEquipments"></param>
        /// <param name="overheadEquipments"></param>
        /// <param name="lines"></param>
        /// <param name="marks"></param>
        /// <param name="medias">多媒体文件</param>
        /// <returns></returns>
        async Task RepairWriteSurveyDataAsync
        (
            List<DesignTowerDto> towers,
            List<CableRequest> cables,
            List<CableChannelRequest> cableChannels,
            List<CableEquipmentRequest> cableEquipments,
            List<OverheadEquipmentRequest> overheadEquipments,
            List<LineRequest> lines,
            List<MarkRequest> marks,
            List<MediaRequest> medias,
            List<BatchTowerViewResponse> towerModules,
            string projectId,
            List<CableChannelResponse> cableChannelModules,
            List<CableWellResponse> cableModules,
            List<ComponentResponse> componentResponses,
            SymbolIdResponse symbolIds,
            List<TransformerResponse> transformerResponses
        )
        {
            #region Tower
            if (towers.HasVal())
            {
                var _towers = new List<SurveyTower>();
                var _towerExts = new List<SurveyTowerExt>();

                foreach (var item in towers)
                {
                    #region tower
                    var tower = item.MapTo<SurveyTower>();
                    //杆梢径
                    if (item.Specification.HasVal() && item.Specification.Contains("*"))
                    {
                        var arry = item.Specification.Split('*');
                        if (arry.Length == 2)
                        {
                            tower.T_Rod = arry[0];
                            tower.T_Height = arry[1];
                        }
                    }
                    if (tower.KVLevel == KVLevel.V380 || tower.KVLevel == KVLevel.V220)
                    {
                        tower.T_Sort = Arrangement.水平;
                    }

                    //获取资源库杆塔方案
                    var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                    if (module == null) module = new BatchTowerViewResponse();
                    //设置杆塔材质，杆塔方案id
                    tower.T_Material = module?.Material;
                    tower.T_Mode = module?.ModuleName;
                    tower.T_Type = module?.Type;
                    _towers.Add(tower);
                    #endregion
                    // LoopName = module.LoopNumber,
                    #region towerExt
                    var symbolId = symbolIds.TowerResponse.FirstOrDefault(x => x.Id == tower.Id)?.SymbolId;
                    var surveyTowerExt = new SurveyTowerExt
                    {
                        Id = tower.Id,
                        ProjectId = projectId,
                        T_ModeId = module.ModuleID,
                        T_Depth = module.Depth,
                        IsTension = Convert.ToBoolean(module.IsTension),
                        PoleTypeCode = item.PoleTypeCode,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                    };
                    _towerExts.Add(surveyTowerExt);
                    #endregion
                }
                await DbHelper.BulkInsertAsync(_towers);
                await DbHelper.BulkInsertAsync(_towerExts);
            }
            #endregion

            #region Cable

            var _cables = cables.MapTo<List<SurveyCable>>();
            var _cableExts = new List<SurveyCableExt>();
            if (_cables.HasVal())
            {

                foreach (var cable in _cables)
                {
                  
                    var symbolId = symbolIds.CableSymbolIdResponse.FirstOrDefault(x => x.Id == cable.Id)?.SymbolId;
                    var cableExt = new SurveyCableExt
                    {
                        Id = cable.Id,
                        ProjectId = cable.ProjectId
                    };
                    cableExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);

                    //获取资源库电缆井
                    var module = cableModules.FirstOrDefault(x =>
                    x.CableWellID == cables.Find(n => n.Id == cableExt.Id)?.C_ModeID);
                    if (module == null) module = new CableWellResponse();
                    cableExt.C_ModeID = cables.Find(n => n.Id == cableExt.Id)?.C_ModeID;
                    cableExt.C_Mode = module.CableWellName;
                    _cableExts.Add(cableExt);
                }
                await DbHelper.BulkInsertAsync(_cables);
                await DbHelper.BulkInsertAsync(_cableExts);
            }
            #endregion

            #region CableChannel
            var _cableChannels = cableChannels.MapTo<List<SurveyCableChannel>>();
            if (_cableChannels.HasVal())
            {
                foreach (var cableChannel in _cableChannels)
                {
                    //获取资源库电缆通道
                    var module = cableChannelModules.FirstOrDefault(x =>
                    x.ChannelID == cableChannel.C_ModeId);
                    if (module == null) module = new CableChannelResponse();
                    cableChannel.C_Mode = module.ChannelName;
                    cableChannel.DuctId = module.DuctMaterialID;
                    cableChannel.DuctSpec = module.DuctSpec;
                }
                await DbHelper.BulkInsertAsync(_cableChannels);
            }
            #endregion

            #region CableEquipment
            var _cableEquipments = cableEquipments.MapTo<List<SurveyCableEquipment>>();
            var _cableEquipmentsExts = new List<SurveyCableDeviceExt>();
            if (_cableEquipments.HasVal())
            {
                foreach (var cableEquipment in _cableEquipments)
                {   
                    var symbolId = symbolIds.CableEquipmentSymbolIdResponse.FirstOrDefault(x => x.Id == cableEquipment.Id)?.SymbolId;

                    _cableEquipmentsExts.Add(new SurveyCableDeviceExt
                    {
                        Id = cableEquipment.Id,
                        ProjectId = cableEquipment.ProjectId,
                        EquipModelID = cableEquipment.EquipModel,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                    });
                    cableEquipment.EquipModel = componentResponses.FirstOrDefault(x => x.ComponentID == cableEquipment.EquipModel)?.ComponentSpec;

                }
                await DbHelper.BulkInsertAsync(_cableEquipments);
                await DbHelper.BulkInsertAsync(_cableEquipmentsExts);
            }
            #endregion

            #region OverheadEquipment
            var _overheadEquipments = overheadEquipments.MapTo<List<SurveyOverheadEquipment>>();
            var _overheadEquipmentsExts = new List<SurveyOverheadEquipmentExt>();
            if (_overheadEquipments.HasVal())
            {
                foreach (var overheadEquipment in _overheadEquipments)
                {
                    if (overheadEquipment.Type == OverheadEquipmentType.变压器)
                    {
                        var curCapacity = (TransformerCapacity)(overheadEquipments
                            .FirstOrDefault((x => x.Id == overheadEquipment.Id)).Capacity);
                        overheadEquipment.Capacity = curCapacity;
                        //获取变压器主杆
                        var mainTower = towers.Find(x => x.Id == overheadEquipment.Main_ID);
                        //获取变压器副杆
                        var subTower = towers.Find(x => x.Id == overheadEquipment.Sub_ID);
                        if (mainTower.HasVal() && subTower.HasVal())
                        {
                            overheadEquipment.Lat_WGS84 = (mainTower.Lat_WGS84 + subTower.Lat_WGS84) / 2;
                            overheadEquipment.Lon_WGS84 = (mainTower.Lon_WGS84 + subTower.Lon_WGS84) / 2;
                        }
                    }
                    else
                    {
                        overheadEquipment.Capacity = TransformerCapacity.None;
                    }

                    var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == overheadEquipment.Id);
                    var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == overheadEquipment.Id)?.SymbolId;
                    _overheadEquipmentsExts.Add(new SurveyOverheadEquipmentExt
                    {
                        Id = overheadEquipment.Id,
                        ProjectId = overheadEquipment.ProjectId,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId),
                        D_Mode = transformerResponse?.ComponentSpec,
                        D_ModeID = transformerResponse?.ComponentID
                    });
                }
                await DbHelper.BulkInsertAsync(_overheadEquipments);
                await DbHelper.BulkInsertAsync(_overheadEquipmentsExts);
            }
            #endregion

            #region Line
            var _lines = new List<SurveyLine>();
            var _lineExts = new List<SurveyLineExt>();
            if (lines.HasVal())
            {
                foreach (var line in lines)
                {
                    var symbolId = symbolIds.LineResponse.FirstOrDefault(x => x.Id == line.Id)?.SymbolId;
                    var curLine = line.MapTo<SurveyLine>();
                    curLine.Name = curLine.LoopName;
                    curLine.Index = (short)line.LoopSerial;
                    _lines.Add(curLine);
                    var curLineExt = line.MapTo<SurveyLineExt>();
                    curLineExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);
                    _lineExts.Add(curLineExt);
                }
                await DbHelper.BulkInsertAsync(_lines);
                await DbHelper.BulkInsertAsync(_lineExts);
            }

            #endregion

        }
        #endregion

        #region 写入预设数据
        /// <summary>
        /// 写入预设数据
        /// </summary>
        /// <param name="towers"></param>
        /// <param name="cables"></param>
        /// <param name="cableChannels"></param>
        /// <param name="cableEquipments"></param>
        /// <param name="overheadEquipments"></param>
        /// <param name="lines"></param>
        /// <param name="marks"></param>
        /// <param name="medias"></param>
        /// <returns></returns>
        async Task RepairWriteDesignDataAsync
        (
            List<DesignTowerDto> towers,
            List<CableRequest> cables,
            List<CableChannelRequest> cableChannels,
            List<CableEquipmentRequest> cableEquipments,
            List<OverheadEquipmentRequest> overheadEquipments,
            List<LineRequest> lines,
            List<MarkRequest> marks,
            List<MediaRequest> medias,
            List<BatchTowerViewResponse> towerModules,
            string projectId,
            List<CableChannelResponse> cableChannelModules,
            List<CableWellResponse> cableModules,
            List<ComponentResponse> componentResponses,
             SymbolIdResponse symbolIds,
             List<TransformerResponse> transformerResponses
        )
        {
            #region Tower
            if (towers.HasVal())
            {

                var _towers = new List<DesignTower>();
                var _towerExts = new List<DesignTowerExt>();
                foreach (var item in towers)
                {
                    #region tower
                    var tower = item.MapTo<DesignTower>();
                    //杆梢径
                    if (item.Specification.HasVal() && item.Specification.Contains("*"))
                    {
                        var arry = item.Specification.Split('*');
                        if (arry.Length == 2)
                        {
                            tower.T_Rod = arry[0];
                            tower.T_Height = arry[1];
                        }
                    }
                    if (tower.KVLevel == KVLevel.V380 || tower.KVLevel == KVLevel.V220)
                    {
                        tower.T_Sort = Arrangement.水平;
                    }

                    //获取资源库杆塔方案
                    var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                    if (module == null) module = new BatchTowerViewResponse();
                    //设置杆塔材质，杆塔方案id
                    tower.T_Material = module?.Material;
                    tower.T_Mode = module?.ModuleName;
                    tower.T_Type = module?.Type;
                    _towers.Add(tower);
                    #endregion
                    #region towerExt
                    var symbolId = symbolIds.TowerResponse.FirstOrDefault(x => x.Id == tower.Id)?.SymbolId;
                    var designTowerExt = new DesignTowerExt
                    {
                        Id = tower.Id,
                        ProjectId = projectId,
                        T_ModeId = module?.ModuleID,
                        T_Depth = module.Depth,
                        LoopName = module.LoopNumber,
                        IsTension = Convert.ToBoolean(module.IsTension),
                        PoleTypeCode = item.PoleTypeCode,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                    };
                    _towerExts.Add(designTowerExt);
                    #endregion
                }
                await DbHelper.BulkInsertAsync(_towers);
                await DbHelper.BulkInsertAsync(_towerExts);

            }
            #endregion

            #region Cable
            var _cables = cables.MapTo<List<DesignCable>>();
            var _cableExts = new List<DesignCableExt>();
            if (_cables.HasVal())
            {

                foreach (var cable in _cables)
                {
                    var symbolId = symbolIds.CableSymbolIdResponse.FirstOrDefault(x => x.Id == cable.Id)?.SymbolId;
                    var cableExt = new DesignCableExt
                    {
                        Id = cable.Id,
                        ProjectId = cable.ProjectId
                    };
                    cableExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);

                    //获取资源库电缆井
                    var module = cableModules.FirstOrDefault(x =>
                    x.CableWellID == cables.Find(n => n.Id == cableExt.Id)?.C_ModeID);
                    if (module == null) module = new CableWellResponse();
                    cableExt.C_ModeID = cables.Find(n => n.Id == cableExt.Id)?.C_ModeID;
                    cableExt.C_Mode = module.CableWellName;
                    _cableExts.Add(cableExt);
                }
                await DbHelper.BulkInsertAsync(_cables);
                await DbHelper.BulkInsertAsync(_cableExts);
            }
            #endregion

            #region CableChannel
            var _cableChannels = cableChannels.MapTo<List<DesignCableChannel>>();
            if (_cableChannels.HasVal())
            {
                foreach (var cableChannel in _cableChannels)
                {
                    //获取资源库电缆通道
                    var module = cableChannelModules.FirstOrDefault(x =>
                    x.ChannelID == cableChannel.C_ModeId);
                    if (module == null) module = new CableChannelResponse();
                    cableChannel.C_Mode = module.ChannelName;
                    cableChannel.DuctId = module.DuctMaterialID;
                    cableChannel.DuctSpec = module.DuctSpec;
                }
                await DbHelper.BulkInsertAsync(_cableChannels);
            }
            #endregion

            #region CableEquipment
            var _cableEquipments = cableEquipments.MapTo<List<DesignCableEquipment>>();
            var _cableEquipmentsExts = new List<DesignCableDeviceExt>();
            if (_cableEquipments.HasVal())
            {
                foreach (var cableEquipment in _cableEquipments)
                {
                    var symbolId = symbolIds.CableEquipmentSymbolIdResponse.FirstOrDefault(x => x.Id == cableEquipment.Id)?.SymbolId;
                    _cableEquipmentsExts.Add(
                        new DesignCableDeviceExt
                        {
                            Id = cableEquipment.Id,
                            ProjectId = cableEquipment.ProjectId,
                            EquipModelID = cableEquipment.EquipModel,
                            SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                        });
                    cableEquipment.EquipModel = componentResponses.FirstOrDefault(x => x.ComponentID == cableEquipment.EquipModel)?.ComponentSpec;
                }
                await DbHelper.BulkInsertAsync(_cableEquipments);
                await DbHelper.BulkInsertAsync(_cableEquipmentsExts);
            }
            #endregion

            #region OverheadEquipment

            var _overheadEquipments = overheadEquipments.MapTo<List<DesignOverheadEquipment>>();
            var _overheadEquipmentsExts = new List<DesignOverheadEquipmentExt>();
            if (_overheadEquipments.HasVal())
            {
                foreach (var overheadEquipment in _overheadEquipments)
                {
                    if (overheadEquipment.Type == OverheadEquipmentType.变压器)
                    {
                        var curCapacity = (TransformerCapacity)(overheadEquipments
                            .FirstOrDefault((x => x.Id == overheadEquipment.Id)).Capacity);
                        overheadEquipment.Capacity = ((int)curCapacity).ToString();
                        //获取变压器主杆
                        var mainTower = towers.Find(x => x.Id == overheadEquipment.Main_ID);
                        //获取变压器副杆
                        var subTower = towers.Find(x => x.Id == overheadEquipment.Sub_ID);
                        if (mainTower.HasVal() && subTower.HasVal())
                        {
                            overheadEquipment.Lat_WGS84 = (mainTower.Lat_WGS84 + subTower.Lat_WGS84) / 2;
                            overheadEquipment.Lon_WGS84 = (mainTower.Lon_WGS84 + subTower.Lon_WGS84) / 2;
                        }
                    }
                    else
                    {
                        overheadEquipment.Capacity = "0";
                    }
                    var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == overheadEquipment.Id);
                    var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == overheadEquipment.Id)?.SymbolId;
                    _overheadEquipmentsExts.Add(new DesignOverheadEquipmentExt
                    {
                        Id = overheadEquipment.Id,
                        ProjectId = overheadEquipment.ProjectId,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId),
                        D_Mode = transformerResponse?.ComponentSpec,
                        D_ModeID = transformerResponse?.ComponentID
                    });
                }
                await DbHelper.BulkInsertAsync(_overheadEquipments);
                await DbHelper.BulkInsertAsync(_overheadEquipmentsExts);
            }
            #endregion

            #region Line
            var _lines = new List<DesignLine>();
            var _lineExts = new List<DesignLineExt>();
            if (lines.HasVal())
            {
                foreach (var line in lines)
                {
                    var symbolId = symbolIds.LineResponse.FirstOrDefault(x => x.Id == line.Id)?.SymbolId;
                    var curLine = line.MapTo<DesignLine>();
                    curLine.Name = curLine.LoopName;
                    curLine.Index = (short)line.LoopSerial;
                    _lines.Add(curLine);
                    var curLineExt = line.MapTo<DesignLineExt>();
                    curLineExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);
                    _lineExts.Add(curLineExt);
                }
                await DbHelper.BulkInsertAsync(_lines);
                await DbHelper.BulkInsertAsync(_lineExts);
            }
            #endregion

        }
        #endregion

        #region 写入拆除数据
        /// <summary>
        /// 写入拆除数据
        /// </summary>
        /// <param name="towers"></param>
        /// <param name="cables"></param>
        /// <param name="cableChannels"></param>
        /// <param name="cableEquipments"></param>
        /// <param name="overheadEquipments"></param>
        /// <param name="lines"></param>
        /// <param name="marks"></param>
        /// <param name="medias"></param>
        /// <returns></returns>
        async Task RepairWriteDismantleDataAsync
        (
            List<DesignTowerDto> towers,
            List<CableRequest> cables,
            List<CableChannelRequest> cableChannels,
            List<CableEquipmentRequest> cableEquipments,
            List<OverheadEquipmentRequest> overheadEquipments,
            List<LineRequest> lines,
            List<MarkRequest> marks,
            List<MediaRequest> medias,
            List<BatchTowerViewResponse> towerModules,
            string projectId,
            List<CableChannelResponse> cableChannelModules,
            List<CableWellResponse> cableModules,
            List<ComponentResponse> componentResponses,
            SymbolIdResponse symbolIds,
            List<TransformerResponse> transformerResponses
        )
        {
            #region Tower
            if (towers.HasVal())
            {
                var _towers = new List<DismantleTower>();
                var _towerExts = new List<DismantleTowerExt>();
                foreach (var item in towers)
                {
                    #region tower
                    var tower = item.MapTo<DismantleTower>();
                    //杆梢径
                    if (item.Specification.HasVal() && item.Specification.Contains("*"))
                    {
                        var arry = item.Specification.Split('*');
                        if (arry.Length == 2)
                        {
                            tower.T_Rod = arry[0];
                            tower.T_Height = arry[1];
                        }
                    }
                    if (tower.KVLevel == KVLevel.V380 || tower.KVLevel == KVLevel.V220)
                    {
                        tower.T_Sort = Arrangement.水平;
                    }
                    //获取资源库杆塔方案
                    var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                    if (module == null) module = new BatchTowerViewResponse();
                    //设置杆塔材质，杆塔方案id
                    tower.T_Material = module?.Material;
                    tower.T_Mode = module?.ModuleName;
                    tower.T_Type = module?.Type;
                    _towers.Add(tower);
                    #endregion
                    #region towerExt
                    var symbolId = symbolIds.TowerResponse.FirstOrDefault(x => x.Id == tower.Id)?.SymbolId;
                    var dismantleTowerExt = new DismantleTowerExt
                    {
                        Id = tower.Id,
                        ProjectId = projectId,
                        T_ModeId = module?.ModuleID,
                        T_Depth = module.Depth,
                        LoopName = module.LoopNumber,
                        IsTension = Convert.ToBoolean(module.IsTension),
                        PoleTypeCode = item.PoleTypeCode,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                    };
                    _towerExts.Add(dismantleTowerExt);
                    #endregion
                }
                await DbHelper.BulkInsertAsync(_towers);
                await DbHelper.BulkInsertAsync(_towerExts);
            }
            #endregion

            #region Cable
            var _cables = cables.MapTo<List<DismantleCable>>();
            var _cableExts = new List<DismantleCableExt>();
            if (_cables.HasVal())
            {

                foreach (var cable in _cables)
                {
                    var symbolId = symbolIds.CableSymbolIdResponse.FirstOrDefault(x => x.Id == cable.Id)?.SymbolId;
                    var cableExt = new DismantleCableExt
                    {
                        Id = cable.Id,
                        ProjectId = cable.ProjectId
                    };
                    cableExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);

                    //获取资源库电缆井
                    var module = cableModules.FirstOrDefault(x =>
                    x.CableWellID == cables.Find(n => n.Id == cableExt.Id)?.C_ModeID);
                    if (module == null) module = new CableWellResponse();
                    cableExt.C_ModeID = cables.Find(n => n.Id == cableExt.Id)?.C_ModeID;
                    cableExt.C_Mode = module.CableWellName;
                    _cableExts.Add(cableExt);
                }
                await DbHelper.BulkInsertAsync(_cables);
                await DbHelper.BulkInsertAsync(_cableExts);
            }
            #endregion

            #region CableChannel
            var _cableChannels = cableChannels.MapTo<List<DismantleCableChannel>>();
            if (_cableChannels.HasVal())
            {
                foreach (var cableChannel in _cableChannels)
                {
                    //获取资源库电缆通道
                    var module = cableChannelModules.FirstOrDefault(x =>
                    x.ChannelID == cableChannel.C_ModeId);
                    if (module == null) module = new CableChannelResponse();
                    cableChannel.C_Mode = module.ChannelName;
                    cableChannel.DuctId = module.DuctMaterialID;
                    cableChannel.DuctSpec = module.DuctSpec;
                }
                await DbHelper.BulkInsertAsync(_cableChannels);
            }
            #endregion

            #region CableEquipment
            var _cableEquipments = cableEquipments.MapTo<List<DismantleCableEquipment>>();
            var _cableEquipmentsExts = new List<DismantleCableDeviceExt>();
            if (_cableEquipments.HasVal())
            {
                foreach (var cableEquipment in _cableEquipments)
                {
                    var symbolId = symbolIds.CableEquipmentSymbolIdResponse.FirstOrDefault(x => x.Id == cableEquipment.Id)?.SymbolId;
                    _cableEquipmentsExts.Add(
                        new DismantleCableDeviceExt
                        {
                            Id = cableEquipment.Id,
                            ProjectId = cableEquipment.ProjectId,
                            EquipModelID = cableEquipment.EquipModel,
                            SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                        });
                    cableEquipment.EquipModel = componentResponses.FirstOrDefault(x => x.ComponentID == cableEquipment.EquipModel)?.ComponentSpec;

                }
                await DbHelper.BulkInsertAsync(_cableEquipments);
                await DbHelper.BulkInsertAsync(_cableEquipmentsExts);
            }
            #endregion

            #region OverheadEquipment

            var _overheadEquipments = overheadEquipments.MapTo<List<DismantleOverheadEquipment>>();
            var _overheadEquipmentsExts = new List<DismantleOverheadEquipmentExt>();
            if (_overheadEquipments.HasVal())
            {
                foreach (var overheadEquipment in _overheadEquipments)
                {
                    if (overheadEquipment.Type == OverheadEquipmentType.变压器)
                    {
                        var curCapacity = (TransformerCapacity)(overheadEquipments
                            .FirstOrDefault((x => x.Id == overheadEquipment.Id)).Capacity);
                        overheadEquipment.Capacity = ((int)curCapacity).ToString();
                        //获取变压器主杆
                        var mainTower = towers.Find(x => x.Id == overheadEquipment.Main_ID);
                        //获取变压器副杆
                        var subTower = towers.Find(x => x.Id == overheadEquipment.Sub_ID);
                        if (mainTower.HasVal() && subTower.HasVal())
                        {
                            overheadEquipment.Lat_WGS84 = (mainTower.Lat_WGS84 + subTower.Lat_WGS84) / 2;
                            overheadEquipment.Lon_WGS84 = (mainTower.Lon_WGS84 + subTower.Lon_WGS84) / 2;
                        }
                    }
                    else
                    {
                        overheadEquipment.Capacity = "0";
                    }
                    var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == overheadEquipment.Id);
                    var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == overheadEquipment.Id)?.SymbolId;
                    _overheadEquipmentsExts.Add(new DismantleOverheadEquipmentExt
                    {
                        Id = overheadEquipment.Id,
                        ProjectId = overheadEquipment.ProjectId,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId),
                        D_Mode = transformerResponse?.ComponentSpec,
                        D_ModeID = transformerResponse?.ComponentID
                    });
                }
                await DbHelper.BulkInsertAsync(_overheadEquipments);
                await DbHelper.BulkInsertAsync(_overheadEquipmentsExts);
            }
            #endregion

            #region Line
            var _lines = new List<DismantleLine>();
            var _lineExts = new List<DismantleLineExt>();
            if (lines.HasVal())
            {
                foreach (var line in lines)
                {
                    var symbolId = symbolIds.LineResponse.FirstOrDefault(x => x.Id == line.Id)?.SymbolId;
                    var curLine = line.MapTo<DismantleLine>();
                    curLine.Name = curLine.LoopName;
                    curLine.Index = (short)line.LoopSerial;
                    _lines.Add(curLine);
                    var curLineExt = line.MapTo<DismantleLineExt>();
                    curLineExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);
                    _lineExts.Add(curLineExt);
                }
                await DbHelper.BulkInsertAsync(_lines);
                await DbHelper.BulkInsertAsync(_lineExts);
            }
            #endregion

        }
        #endregion

        #endregion

        #region 写入勘测数据
        /// <summary>
        /// 写入勘测数据
        /// </summary>
        /// <param name="towers"></param>
        /// <param name="cables"></param>
        /// <param name="cableChannels"></param>
        /// <param name="cableEquipments"></param>
        /// <param name="overheadEquipments"></param>
        /// <param name="lines"></param>
        /// <param name="marks"></param>
        /// <param name="medias">多媒体文件</param>
        /// <returns></returns>
        async Task WriteSurveyDataAsync
        (
            List<DesignTowerDto> towers,
            List<CableRequest> cables,
            List<CableChannelRequest> cableChannels,
            List<CableEquipmentRequest> cableEquipments,
            List<OverheadEquipmentRequest> overheadEquipments,
            List<LineRequest> lines,
            List<MarkRequest> marks,
            List<MediaRequest> medias,
            List<BatchTowerViewResponse> towerModules,
            string projectId,
            List<CableChannelResponse> cableChannelModules,
            List<CableWellResponse> cableModules,
            List<ComponentResponse> componentResponses,
            SymbolIdResponse symbolIds,
            List<TransformerResponse> transformerResponses
        )
        {
            #region Tower
            if (towers.HasVal())
            {
                var _towers = new List<SurveyTower>();
                var _towerExts = new List<SurveyTowerExt>();

                foreach (var item in towers)
                {
                    #region tower
                    var tower = item.MapTo<SurveyTower>();
                    tower.Surveyor = CurrentUser.Id;
                    //杆梢径
                    if (item.Specification.HasVal() && item.Specification.Contains("*"))
                    {
                        var arry = item.Specification.Split('*');
                        if (arry.Length == 2)
                        {
                            tower.T_Rod = arry[0];
                            tower.T_Height = arry[1];
                        }
                    }
                    if (tower.KVLevel == KVLevel.V380 || tower.KVLevel == KVLevel.V220)
                    {
                        tower.T_Sort = Arrangement.水平;
                    }

                    //获取资源库杆塔方案
                    var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                    if (module == null) module = new BatchTowerViewResponse();
                    //设置杆塔材质，杆塔方案id
                    tower.T_Material = module?.Material;
                    tower.T_Mode = module?.ModuleName;
                    tower.T_Type = module?.Type;
                    _towers.Add(tower);
                    #endregion
                    // LoopName = module.LoopNumber,
                    #region towerExt
                    var symbolId = symbolIds.TowerResponse.FirstOrDefault(x => x.Id == tower.Id)?.SymbolId;
                    var surveyTowerExt = new SurveyTowerExt
                    {
                        Id = tower.Id,
                        ProjectId = projectId,
                        T_ModeId = module.ModuleID,
                        T_Depth = module.Depth,
                        IsTension = Convert.ToBoolean(module.IsTension),
                        PoleTypeCode = item.PoleTypeCode,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                    };
                    _towerExts.Add(surveyTowerExt);
                    #endregion
                }
                await DbHelper.BulkInsertAsync(_towers);
                await DbHelper.BulkInsertAsync(_towerExts);
            }
            #endregion

            #region Cable

            var _cables = cables.MapTo<List<SurveyCable>>();
            var _cableExts = new List<SurveyCableExt>();
            if (_cables.HasVal())
            {

                foreach (var cable in _cables)
                {
                    cable.Surveyor = CurrentUser.Id;

                    var symbolId = symbolIds.CableSymbolIdResponse.FirstOrDefault(x => x.Id == cable.Id)?.SymbolId;
                    var cableExt = new SurveyCableExt
                    {
                        Id = cable.Id,
                        ProjectId = cable.ProjectId
                    };
                    cableExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);

                    //获取资源库电缆井
                    var module = cableModules.FirstOrDefault(x =>
                    x.CableWellID == cables.Find(n => n.Id == cableExt.Id)?.C_ModeID);
                    if (module == null) module = new CableWellResponse();
                    cableExt.C_ModeID = cables.Find(n => n.Id == cableExt.Id)?.C_ModeID;
                    cableExt.C_Mode = module.CableWellName;
                    _cableExts.Add(cableExt);
                }
                await DbHelper.BulkInsertAsync(_cables);
                await DbHelper.BulkInsertAsync(_cableExts);
            }
            #endregion

            #region CableChannel
            var _cableChannels = cableChannels.MapTo<List<SurveyCableChannel>>();
            if (_cableChannels.HasVal())
            {
                foreach (var cableChannel in _cableChannels)
                {
                    cableChannel.Surveyor = CurrentUser.Id;
                    //获取资源库电缆通道
                    var module = cableChannelModules.FirstOrDefault(x =>
                    x.ChannelID == cableChannel.C_ModeId);
                    if (module == null) module = new CableChannelResponse();
                    cableChannel.C_Mode = module.ChannelName;
                    cableChannel.DuctId = module.DuctMaterialID;
                    cableChannel.DuctSpec = module.DuctSpec;
                    cableChannel.Surveyor = CurrentUser.Id;
                }
                await DbHelper.BulkInsertAsync(_cableChannels);
            }
            #endregion

            #region CableEquipment
            var _cableEquipments = cableEquipments.MapTo<List<SurveyCableEquipment>>();
            var _cableEquipmentsExts = new List<SurveyCableDeviceExt>();
            if (_cableEquipments.HasVal())
            {
                foreach (var cableEquipment in _cableEquipments)
                {
                    var symbolId = symbolIds.CableEquipmentSymbolIdResponse.FirstOrDefault(x => x.Id == cableEquipment.Id)?.SymbolId;

                    _cableEquipmentsExts.Add(new SurveyCableDeviceExt
                    {
                        Id = cableEquipment.Id,
                        ProjectId = cableEquipment.ProjectId,
                        EquipModelID = cableEquipment.EquipModel,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                    });
                    cableEquipment.EquipModel = componentResponses.FirstOrDefault(x => x.ComponentID == cableEquipment.EquipModel)?.ComponentSpec;
                    cableEquipment.Surveyor = CurrentUser.Id;

                }
                await DbHelper.BulkInsertAsync(_cableEquipments);
                await DbHelper.BulkInsertAsync(_cableEquipmentsExts);
            }
            #endregion

            #region OverheadEquipment
            var _overheadEquipments = overheadEquipments.MapTo<List<SurveyOverheadEquipment>>();
            var _overheadEquipmentsExts = new List<SurveyOverheadEquipmentExt>();
            if (_overheadEquipments.HasVal())
            {
                foreach (var overheadEquipment in _overheadEquipments)
                {
                    if (overheadEquipment.Type == OverheadEquipmentType.变压器)
                    {
                        var curCapacity = (TransformerCapacity)(overheadEquipments
                            .FirstOrDefault((x => x.Id == overheadEquipment.Id)).Capacity);
                        overheadEquipment.Capacity = curCapacity;
                    }
                    else
                    {
                        overheadEquipment.Capacity = TransformerCapacity.None;
                    }
                    var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == overheadEquipment.Id);
                    var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == overheadEquipment.Id)?.SymbolId;
                    _overheadEquipmentsExts.Add(new SurveyOverheadEquipmentExt
                    {
                        Id = overheadEquipment.Id,
                        ProjectId = overheadEquipment.ProjectId,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId),
                        D_Mode = transformerResponse?.ComponentSpec,
                        D_ModeID = transformerResponse?.ComponentID
                    });
                    overheadEquipment.Surveyor = CurrentUser.Id;
                }
                await DbHelper.BulkInsertAsync(_overheadEquipments);
                await DbHelper.BulkInsertAsync(_overheadEquipmentsExts);
            }
            #endregion

            #region Line
            var _lines = new List<SurveyLine>();
            var _lineExts = new List<SurveyLineExt>();
            if (lines.HasVal())
            {
                foreach (var line in lines)
                {
                    var symbolId = symbolIds.LineResponse.FirstOrDefault(x => x.Id == line.Id)?.SymbolId;
                    var curLine = line.MapTo<SurveyLine>();
                    curLine.Surveyor = CurrentUser.Id;
                    curLine.Name = curLine.LoopName;
                    curLine.Index = (short)line.LoopSerial;
                    _lines.Add(curLine);
                    var curLineExt = line.MapTo<SurveyLineExt>();
                    curLineExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);
                    _lineExts.Add(curLineExt);
                }
                await DbHelper.BulkInsertAsync(_lines);
                await DbHelper.BulkInsertAsync(_lineExts);
            }

            #endregion

            #region Mark
            var _marks = marks.MapTo<List<SurveyMark>>();
            if (_marks.HasVal())
            {
                _marks.ForEach(m => m.Surveyor = CurrentUser.Id);
                await DbHelper.BulkInsertAsync(_marks);
            }
            #endregion

            #region media
            var _medias = medias?.MapTo<List<SurveyMedia>>();
            if (_medias.HasVal())
            {
                _medias.ForEach(m => m.CreatedBy = CurrentUser.Id);
                await DbHelper.BulkInsertAsync(_medias);
            }
            #endregion

        }
        #endregion

        #region 写入预设数据
        /// <summary>
        /// 写入预设数据
        /// </summary>
        /// <param name="towers"></param>
        /// <param name="cables"></param>
        /// <param name="cableChannels"></param>
        /// <param name="cableEquipments"></param>
        /// <param name="overheadEquipments"></param>
        /// <param name="lines"></param>
        /// <param name="marks"></param>
        /// <param name="medias"></param>
        /// <returns></returns>
        async Task WriteDesignDataAsync
        (
            List<DesignTowerDto> towers,
            List<CableRequest> cables,
            List<CableChannelRequest> cableChannels,
            List<CableEquipmentRequest> cableEquipments,
            List<OverheadEquipmentRequest> overheadEquipments,
            List<LineRequest> lines,
            List<MarkRequest> marks,
            List<MediaRequest> medias,
            List<BatchTowerViewResponse> towerModules,
            string projectId,
            List<CableChannelResponse> cableChannelModules,
            List<CableWellResponse> cableModules,
            List<ComponentResponse> componentResponses,
             SymbolIdResponse symbolIds,
             List<TransformerResponse> transformerResponses
        )
        {
            #region Tower
            if (towers.HasVal())
            {
               
                var _towers = new List<DesignTower>();
                var _towerExts = new List<DesignTowerExt>();
                foreach (var item in towers)
                {
                    #region tower
                    var tower = item.MapTo<DesignTower>();
                    tower.Surveyor = CurrentUser.Id;
                    //杆梢径
                    if (item.Specification.HasVal() && item.Specification.Contains("*"))
                    {
                        var arry = item.Specification.Split('*');
                        if (arry.Length == 2)
                        {
                            tower.T_Rod = arry[0];
                            tower.T_Height = arry[1];
                        }
                    }
                    if (tower.KVLevel == KVLevel.V380 || tower.KVLevel == KVLevel.V220)
                    {
                        tower.T_Sort = Arrangement.水平;
                    }

                    //获取资源库杆塔方案
                    var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                    if (module == null) module = new BatchTowerViewResponse();
                    //设置杆塔材质，杆塔方案id
                    tower.T_Material = module?.Material;
                    tower.T_Mode = module?.ModuleName;
                    tower.T_Type = module?.Type;
                    _towers.Add(tower);
                    #endregion
                    #region towerExt
                    var symbolId = symbolIds.TowerResponse.FirstOrDefault(x => x.Id == tower.Id)?.SymbolId;
                    var designTowerExt = new DesignTowerExt
                    {
                        Id = tower.Id,
                        ProjectId = projectId,
                        T_ModeId = module?.ModuleID,
                        T_Depth = module.Depth,
                        LoopName = module.LoopNumber,
                        IsTension = Convert.ToBoolean(module.IsTension),
                        PoleTypeCode = item.PoleTypeCode,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                    };
                    _towerExts.Add(designTowerExt);
                    #endregion
                }
                await DbHelper.BulkInsertAsync(_towers);
                await DbHelper.BulkInsertAsync(_towerExts);

            }
            #endregion

            #region Cable
            var _cables = cables.MapTo<List<DesignCable>>();
            var _cableExts = new List<DesignCableExt>();
            if (_cables.HasVal())
            {

                foreach (var cable in _cables)
                {
                    var symbolId = symbolIds.CableSymbolIdResponse.FirstOrDefault(x => x.Id == cable.Id)?.SymbolId;
                    cable.Surveyor = CurrentUser.Id;
                    var cableExt = new DesignCableExt
                    {
                        Id = cable.Id,
                        ProjectId = cable.ProjectId
                    };
                    cableExt.SymbolId =  symbolId == null ? 0 : Convert.ToInt32(symbolId);

                    //获取资源库电缆井
                    var module = cableModules.FirstOrDefault(x =>
                    x.CableWellID == cables.Find(n => n.Id == cableExt.Id)?.C_ModeID);
                    if (module == null) module = new CableWellResponse();
                    cableExt.C_ModeID = cables.Find(n => n.Id == cableExt.Id)?.C_ModeID;
                    cableExt.C_Mode = module.CableWellName;
                    _cableExts.Add(cableExt);
                }
                await DbHelper.BulkInsertAsync(_cables);
                await DbHelper.BulkInsertAsync(_cableExts);
            }
            #endregion

            #region CableChannel
            var _cableChannels = cableChannels.MapTo<List<DesignCableChannel>>();
            if (_cableChannels.HasVal())
            {
                foreach (var cableChannel in _cableChannels)
                {
                    cableChannel.Surveyor = CurrentUser.Id;
                    //获取资源库电缆通道
                    var module = cableChannelModules.FirstOrDefault(x =>
                    x.ChannelID == cableChannel.C_ModeId);
                    if (module == null) module = new CableChannelResponse();
                    cableChannel.C_Mode = module.ChannelName;
                    cableChannel.DuctId = module.DuctMaterialID;
                    cableChannel.DuctSpec = module.DuctSpec;
                    cableChannel.Surveyor = CurrentUser.Id;
                }
                await DbHelper.BulkInsertAsync(_cableChannels);
            }
            #endregion

            #region CableEquipment
            var _cableEquipments = cableEquipments.MapTo<List<DesignCableEquipment>>();
            var _cableEquipmentsExts = new List<DesignCableDeviceExt>();
            if (_cableEquipments.HasVal())
            {
                foreach (var cableEquipment in _cableEquipments)
                {
                    var symbolId = symbolIds.CableEquipmentSymbolIdResponse.FirstOrDefault(x => x.Id == cableEquipment.Id)?.SymbolId;
                    _cableEquipmentsExts.Add(
                        new DesignCableDeviceExt
                        {
                            Id = cableEquipment.Id,
                            ProjectId = cableEquipment.ProjectId,
                            EquipModelID = cableEquipment.EquipModel,
                            SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                        });
                    cableEquipment.EquipModel = componentResponses.FirstOrDefault(x => x.ComponentID == cableEquipment.EquipModel)?.ComponentSpec;
                    cableEquipment.Surveyor = CurrentUser.Id;
                }
                await DbHelper.BulkInsertAsync(_cableEquipments);
                await DbHelper.BulkInsertAsync(_cableEquipmentsExts);
            }
            #endregion

            #region OverheadEquipment

            var _overheadEquipments = overheadEquipments.MapTo<List<DesignOverheadEquipment>>();
            var _overheadEquipmentsExts = new List<DesignOverheadEquipmentExt>();
            if (_overheadEquipments.HasVal())
            {
                foreach (var overheadEquipment in _overheadEquipments)
                {
                    if (overheadEquipment.Type == OverheadEquipmentType.变压器)
                    {
                        var curCapacity = (TransformerCapacity)(overheadEquipments
                            .FirstOrDefault((x => x.Id == overheadEquipment.Id)).Capacity);
                        overheadEquipment.Capacity = ((int)curCapacity).ToString();
                    }
                    else
                    {
                        overheadEquipment.Capacity = "0";
                    }
                    var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == overheadEquipment.Id);
                    var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == overheadEquipment.Id)?.SymbolId;
                    _overheadEquipmentsExts.Add(new DesignOverheadEquipmentExt
                    {
                        Id = overheadEquipment.Id,
                        ProjectId = overheadEquipment.ProjectId,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId),
                        D_Mode = transformerResponse?.ComponentSpec,
                        D_ModeID = transformerResponse?.ComponentID
                    });
                    overheadEquipment.Surveyor = CurrentUser.Id;
                }
                await DbHelper.BulkInsertAsync(_overheadEquipments);
                await DbHelper.BulkInsertAsync(_overheadEquipmentsExts);
            }
            #endregion

            #region Line
            var _lines = new List<DesignLine>();
            var _lineExts = new List<DesignLineExt>();
            if (lines.HasVal())
            {
                foreach (var line in lines)
                {
                    var symbolId = symbolIds.LineResponse.FirstOrDefault(x => x.Id == line.Id)?.SymbolId;
                    var curLine = line.MapTo<DesignLine>();
                    curLine.Surveyor = CurrentUser.Id;
                    curLine.Name = curLine.LoopName;
                    curLine.Index = (short)line.LoopSerial;
                    _lines.Add(curLine);
                    var curLineExt = line.MapTo<DesignLineExt>();
                    curLineExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);
                    _lineExts.Add(curLineExt);
                }
                await DbHelper.BulkInsertAsync(_lines);
                await DbHelper.BulkInsertAsync(_lineExts);
            }
            #endregion

            #region Mark
            var _marks = marks.MapTo<List<DesignMark>>();
            if (_marks.HasVal())
            {
                _marks.ForEach(m => m.Surveyor = CurrentUser.Id);
                await DbHelper.BulkInsertAsync(_marks);
            }
            #endregion

            #region media
            var _medias = medias?.MapTo<List<DesignMedia>>();
            if (_medias.HasVal())
            {
                _medias.ForEach(m => m.CreatedBy = CurrentUser.Id);
                await DbHelper.BulkInsertAsync(_medias);
            }
            #endregion
        }
        #endregion

        #region 写入拆除数据
        /// <summary>
        /// 写入拆除数据
        /// </summary>
        /// <param name="towers"></param>
        /// <param name="cables"></param>
        /// <param name="cableChannels"></param>
        /// <param name="cableEquipments"></param>
        /// <param name="overheadEquipments"></param>
        /// <param name="lines"></param>
        /// <param name="marks"></param>
        /// <param name="medias"></param>
        /// <returns></returns>
        async Task WriteDismantleDataAsync
        (
            List<DesignTowerDto> towers,
            List<CableRequest> cables,
            List<CableChannelRequest> cableChannels,
            List<CableEquipmentRequest> cableEquipments,
            List<OverheadEquipmentRequest> overheadEquipments,
            List<LineRequest> lines,
            List<MarkRequest> marks,
            List<MediaRequest> medias,
            List<BatchTowerViewResponse> towerModules,
            string projectId,
            List<CableChannelResponse> cableChannelModules,
            List<CableWellResponse> cableModules,
            List<ComponentResponse> componentResponses,
            SymbolIdResponse symbolIds,
            List<TransformerResponse> transformerResponses
        )
        {
            #region Tower
            if (towers.HasVal())
            {
                var _towers = new List<DismantleTower>();
                var _towerExts = new List<DismantleTowerExt>();
                foreach (var item in towers)
                {
                    #region tower
                    item.Id = UserMd5(item.Id);
                    item.ParentId = UserMd5(item.ParentId);
                    var tower = item.MapTo<DismantleTower>();
                    tower.Surveyor = CurrentUser.Id;
                    tower.Surveyor = CurrentUser.Id;
                    //杆梢径
                    if (item.Specification.HasVal() && item.Specification.Contains("*"))
                    {
                        var arry = item.Specification.Split('*');
                        if (arry.Length == 2)
                        {
                            tower.T_Rod = arry[0];
                            tower.T_Height = arry[1];
                        }
                    }
                    if (tower.KVLevel == KVLevel.V380 || tower.KVLevel == KVLevel.V220)
                    {
                        tower.T_Sort = Arrangement.水平;
                    }
                    //获取资源库杆塔方案
                    var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                    if (module == null) module = new BatchTowerViewResponse();
                    //设置杆塔材质，杆塔方案id
                    tower.T_Material = module?.Material;
                    tower.T_Mode = module?.ModuleName;
                    tower.T_Type = module?.Type;
                    _towers.Add(tower);
                    #endregion
                    #region towerExt
                    var symbolId = symbolIds.TowerResponse.FirstOrDefault(x => x.Id == tower.Id)?.SymbolId;
                    var dismantleTowerExt = new DismantleTowerExt
                    {
                        Id = tower.Id,
                        ProjectId = projectId,
                        T_ModeId = module?.ModuleID,
                        T_Depth = module.Depth,
                        LoopName = module.LoopNumber,
                        IsTension = Convert.ToBoolean(module.IsTension),
                        PoleTypeCode = item.PoleTypeCode,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                    };
                    _towerExts.Add(dismantleTowerExt);
                    #endregion
                }
                await DbHelper.BulkInsertAsync(_towers);
                await DbHelper.BulkInsertAsync(_towerExts);
            }
            #endregion

            #region Cable
            var _cables = cables.MapTo<List<DismantleCable>>();
            var _cableExts = new List<DismantleCableExt>();
            if (_cables.HasVal())
            {
                foreach (var cable in _cables)
                {
                    cable.Id = UserMd5(cable.Id);
                    if (!string.IsNullOrEmpty(cable.ParentId))
                    {
                        cable.ParentId = UserMd5(cable.ParentId);
                    }
                    var symbolId = symbolIds.CableSymbolIdResponse.FirstOrDefault(x => x.Id == cable.Id)?.SymbolId;
                    cable.Surveyor = CurrentUser.Id;
                    var cableExt = new DismantleCableExt
                    {
                        Id = cable.Id,
                        ProjectId = cable.ProjectId
                    };
                    cableExt.SymbolId =  symbolId == null ? 0 : Convert.ToInt32(symbolId);

                    //获取资源库电缆井
                    var module = cableModules.FirstOrDefault(x =>
                    x.CableWellID == cables.Find(n => n.Id == cableExt.Id)?.C_ModeID);
                    if (module == null) module = new CableWellResponse();
                    cableExt.C_ModeID = cables.Find(n => n.Id == cableExt.Id)?.C_ModeID;
                    cableExt.C_Mode = module.CableWellName;
                    _cableExts.Add(cableExt);
                }
                await DbHelper.BulkInsertAsync(_cables);
                await DbHelper.BulkInsertAsync(_cableExts);
            }
            #endregion

            #region CableChannel
            var _cableChannels = cableChannels.MapTo<List<DismantleCableChannel>>();
            if (_cableChannels.HasVal())
            {
                foreach (var cableChannel in _cableChannels)
                {
                    cableChannel.Id = UserMd5(cableChannel.Id);
                    if (!string.IsNullOrEmpty(cableChannel.Start_Id))
                    {
                        cableChannel.Start_Id = UserMd5(cableChannel.Start_Id);
                    }
                    if (!string.IsNullOrEmpty( cableChannel.End_Id))
                    {
                        cableChannel.End_Id = UserMd5(cableChannel.End_Id);
                    }
                    cableChannel.Surveyor = CurrentUser.Id;
                    //获取资源库电缆通道
                    var module = cableChannelModules.FirstOrDefault(x =>
                    x.ChannelID == cableChannel.C_ModeId);
                    if (module == null) module = new CableChannelResponse();
                    cableChannel.C_Mode = module.ChannelName;
                    cableChannel.DuctId = module.DuctMaterialID;
                    cableChannel.DuctSpec = module.DuctSpec;
                    cableChannel.Surveyor = CurrentUser.Id;
                }
                await DbHelper.BulkInsertAsync(_cableChannels);
            }
            #endregion

            #region CableEquipment
            var _cableEquipments = cableEquipments.MapTo<List<DismantleCableEquipment>>();
            var _cableEquipmentsExts = new List<DismantleCableDeviceExt>();
            if (_cableEquipments.HasVal())
            {
                foreach (var cableEquipment in _cableEquipments)
                {
                    cableEquipment.Id = UserMd5(cableEquipment.Id);
                    if (!string.IsNullOrEmpty(cableEquipment.ParentId))
                    {
                        cableEquipment.ParentId = UserMd5(cableEquipment.ParentId);
                    }
                    var symbolId = symbolIds.CableEquipmentSymbolIdResponse.FirstOrDefault(x => x.Id == cableEquipment.Id)?.SymbolId;
                    _cableEquipmentsExts.Add(
                        new DismantleCableDeviceExt
                        {
                            Id = cableEquipment.Id,
                            ProjectId = cableEquipment.ProjectId,
                            EquipModelID = cableEquipment.EquipModel,
                            SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId)
                });
                    cableEquipment.EquipModel = componentResponses.FirstOrDefault(x => x.ComponentID == cableEquipment.EquipModel)?.ComponentSpec;
                    cableEquipment.Surveyor = CurrentUser.Id;

                }
                await DbHelper.BulkInsertAsync(_cableEquipments);
                await DbHelper.BulkInsertAsync(_cableEquipmentsExts);
            }
            #endregion

            #region OverheadEquipment

            var _overheadEquipments = overheadEquipments.MapTo<List<DismantleOverheadEquipment>>();
            var _overheadEquipmentsExts = new List<DismantleOverheadEquipmentExt>();
            if (_overheadEquipments.HasVal())
            {
                foreach (var overheadEquipment in _overheadEquipments)
                {
                    overheadEquipment.Id = UserMd5(overheadEquipment.Id);
                    if (!string.IsNullOrEmpty(overheadEquipment.Sub_ID))
                    {
                        overheadEquipment.Sub_ID = UserMd5(overheadEquipment.Sub_ID);
                    }
                    if (!string.IsNullOrEmpty(overheadEquipment.Main_ID))
                    {
                        overheadEquipment.Main_ID = UserMd5(overheadEquipment.Main_ID);
                    }
                    if (overheadEquipment.Type == OverheadEquipmentType.变压器)
                    {
                        var curCapacity = (TransformerCapacity)(overheadEquipments
                           .FirstOrDefault((x => x.Id == overheadEquipment.Id)).Capacity);
                        overheadEquipment.Capacity = ((int)curCapacity).ToString();
                    }
                    else
                    {
                        overheadEquipment.Capacity = "0";
                    }
                    var transformerResponse = transformerResponses.FirstOrDefault(x => x.TransformerID == overheadEquipment.Id);
                    var symbolId = symbolIds.OverheadEquipmentResponse.FirstOrDefault(x => x.Id == overheadEquipment.Id)?.SymbolId;
                    _overheadEquipmentsExts.Add(new DismantleOverheadEquipmentExt
                    {
                        Id = overheadEquipment.Id,
                        ProjectId = overheadEquipment.ProjectId,
                        SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId),
                        D_Mode = transformerResponse?.ComponentSpec,
                        D_ModeID = transformerResponse?.ComponentID
                    });
                    overheadEquipment.Surveyor = CurrentUser.Id;
                }
                await DbHelper.BulkInsertAsync(_overheadEquipments);
                await DbHelper.BulkInsertAsync(_overheadEquipmentsExts);
            }
            #endregion

            #region Line
            var _lines = new List<DismantleLine>();
            var _lineExts = new List<DismantleLineExt>();
            if (lines.HasVal())
            {
                foreach (var line in lines)
                {
                    line.Id = UserMd5(line.Id);
                    if (!string.IsNullOrEmpty(line.Start_ID))
                    {
                        line.Start_ID = UserMd5(line.Start_ID);
                    }
                    if (!string.IsNullOrEmpty(line.End_ID))
                    {
                        line.End_ID = UserMd5(line.End_ID);
                    }
                    if (!string.IsNullOrEmpty(line.GroupId))
                    {
                        line.GroupId = UserMd5(line.GroupId);
                    }
                    var symbolId = symbolIds.LineResponse.FirstOrDefault(x => x.Id == line.Id)?.SymbolId;
                    var curLine = line.MapTo<DismantleLine>();
                    curLine.Surveyor = CurrentUser.Id;
                    curLine.Name = curLine.LoopName;
                    curLine.Index = (short)line.LoopSerial;
                    _lines.Add(curLine);
                    var curLineExt = line.MapTo<DismantleLineExt>();
                    curLineExt.SymbolId = symbolId == null ? 0 : Convert.ToInt32(symbolId);
                    _lineExts.Add(curLineExt);
                }
                await DbHelper.BulkInsertAsync(_lines);
                await DbHelper.BulkInsertAsync(_lineExts);
            }
            #endregion

            #region Mark
            var _marks = marks.MapTo<List<DismantleMark>>();
            if (_marks.HasVal())
            {
                _marks.ForEach(m => m.Surveyor = CurrentUser.Id);
                _marks.ForEach(m => m.Id =UserMd5(m.Id));
                await DbHelper.BulkInsertAsync(_marks);
            }
            #endregion

            #region media
            var _medias = medias?.MapTo<List<DismantleMedia>>();
            if (_medias.HasVal())
            {
                _medias.ForEach(m => m.CreatedBy = CurrentUser.Id);
                await DbHelper.BulkInsertAsync(_medias);
            }
            #endregion
        }
        #endregion


        #region 资源api获取

        /// <summary>
        ///     获取杆塔方案
        /// </summary>
        /// <param name="towers"></param>
        /// <param name="resourceLibID"></param>
        /// <returns></returns>
        private async Task<List<BatchTowerViewResponse>> TowerModules(List<DesignTowerDto> towers, string resourceLibID)
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
                        //杆梢径
                        var rod = string.Empty;
                        var height = string.Empty;
                        if (item.Specification.HasVal() && item.Specification.Contains("*"))
                        {
                            var arry = item.Specification.Split('*');
                            if (arry.Length == 2)
                            {
                                rod = arry[0];
                                height = arry[1];
                            }
                        }
                        request.BatchModuleQueries.Add(
                            new BatchModuleQueryRequest
                            {
                                KVLevel = item.KVLevel,
                                RodDiameter = Convert.ToDouble(rod),
                                SegmentMode = ((int)item.T_Segment)==0?"":item.T_Segment.ToString(),
                                Height = Convert.ToDouble(height),
                                Arrangement = ((int)item.T_Sort)==0?"":item.T_Sort.ToString(),
                                ResourceLibID = resourceLibID,
                                TowerID=item.Id,
                                PoleTypeCodes=new List<string> { item.PoleTypeCode }
                            });
                    }
                    //获取资源库杆塔方案
                    rtn = await _apiClient.ExecuteAsync<List<BatchTowerViewResponse>>(HttpMethod.Post, towerModulesUrl, request);

                    //匹配每个杆塔第一个匹配的方案
                    //foreach (var item in towers)
                    //{
                    //    var curTowerInfo = curData.FirstOrDefault(x => x.TowerID == item.Id);
                    //    if (curTowerInfo != null) rtn.Add(curTowerInfo);
                    //}

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
        private async Task<List<CableWellResponse>> CableWellResponses(List<CableRequest> cables, string resourceLibID)
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
                        cableWellListRequest.CableWellIDs.Add(item.C_ModeID);
                       //var requestInfo = new CableWellRequest
                       // {
                       //     CableWellID = item.C_ModeID,
                       //     ResourceLibID = resourceLibID
                       // };

                       // //获取资源库杆塔方案
                       // var module = (await _apiClient.ExecuteAsync<List<CableWellResponse>>(HttpMethod.Post, cableWellUrl, requestInfo)).FirstOrDefault();
                       // if (!string.IsNullOrEmpty(module?.CableWellID))
                       //     rtn.Add(requestInfo, module);
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
        private async Task<List<CableChannelResponse>> CableChannelResponses(List<CableChannelRequest> cables, string resourceLibID)
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
                        channelListRequest.ChannelIDs.Add(item.C_ModeId);
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
        private async Task<List<ComponentResponse>> CableEquipmentResponses(List<CableEquipmentRequest> cableEquipmentRequests, string resourceLibID)
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
        private async Task<SymbolIdResponse> GetSymbolIds(ReportSurveyDataRequest request, List<BatchTowerViewResponse> towerModules)
        {
            var rtnData = new SymbolIdResponse();
            try
            {
                if (request == null) return rtnData;
                GetSymbolIdRequest symbolIdRequest = new GetSymbolIdRequest();
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
                            var tower = item.MapTo<SurveyTower>();
                            if (!tower.Surveyor.HasVal())
                                tower.Surveyor = CurrentUser.Id;
                            //杆梢径
                            if (item.Specification.HasVal() && item.Specification.Contains("*"))
                            {
                                var arry = item.Specification.Split('*');
                                if (arry.Length == 2)
                                {
                                    tower.T_Rod = arry[0];
                                    tower.T_Height = arry[1];
                                }
                            }
                            if (tower.KVLevel == KVLevel.V380 || tower.KVLevel == KVLevel.V220)
                            {
                                tower.T_Sort = Arrangement.水平;
                            }
                            var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                            if (module!=null)   symbolIdRequest.TowerRequest.Add(
                               new GetTowerSymbolIdRequest {
                                Id = tower.Id,
                                State = tower.State,
                                Material= module.Material.ToEnum<PoleMaterial>()
                          });
                          }
                    }
                    if (request.SurveyData.Cables != null)
                    {
                        symbolIdRequest.CableSymbolIdRequest.AddRange((from item in request.SurveyData.Cables
                                                                select new GetSurveyCableSymbolIdRequest
                                                                {
                                                                    Id=item.Id,
                                                                    State=item.State,
                                                                    Type=item.C_Type
                                                                }).ToList());
                    }
                    if (request.SurveyData.CableEquipments != null)
                    {
                        symbolIdRequest.CableEquipmentSymbolIdRequest.AddRange((from item in request.SurveyData.CableEquipments
                                                                         select new GetSurveyCableEquipmentSymbolIdRequest
                                                                         {
                                                                    Id = item.Id,
                                                                    State = item.State,
                                                                    Type=item.Type
                                                                }).ToList());
                    }
                    if (request.SurveyData.OverheadEquipments != null)
                    {
                        symbolIdRequest.OverheadEquipmentRequest.AddRange((from item in request.SurveyData.OverheadEquipments
                                            select new GeOverheadEquipmentSymbolIdRequest
                                            {
                                                                             Id = item.Id,
                                                                             Type = item.Type,
                                                                             State = item.State
                                                                         }).ToList());
                    }
                    if (request.SurveyData.Lines!= null)
                    {
                        symbolIdRequest.LineRequest.AddRange( (from item in request.SurveyData.Lines
                                                       select new GetLineSymbolIdRequest
                                                       {
                                                                        Id = item.Id,
                                                                        IsCable = item.IsCable,
                                                                        KVLevel=item.KVLevel,
                                                                        State=item.State
                                                                    }).ToList());
                    }
                }
                if (request.DesignData != null)
                {
                    if (request.DesignData.Towers != null)
                    {
                        foreach (var item in request.DesignData.Towers)
                        {
                            var tower = item.MapTo<DesignTower>();
                            if(!tower.Surveyor.HasVal())
                            tower.Surveyor = CurrentUser.Id;
                            //杆梢径
                            if (item.Specification.HasVal() && item.Specification.Contains("*"))
                            {
                                var arry = item.Specification.Split('*');
                                if (arry.Length == 2)
                                {
                                    tower.T_Rod = arry[0];
                                    tower.T_Height = arry[1];
                                }
                            }
                            if (tower.KVLevel == KVLevel.V380 || tower.KVLevel == KVLevel.V220)
                            {
                                tower.T_Sort = Arrangement.水平;
                            }
                            var module = towerModules.FirstOrDefault(x => x.TowerID == item.Id);
                            if (module != null) symbolIdRequest.TowerRequest.Add(
                               new GetTowerSymbolIdRequest
                               {
                                   Id = tower.Id,
                                   State = tower.State,
                                   Material = module.Material.ToEnum<PoleMaterial>()
                               });
                        }
                    }
                    if (request.DesignData.Cables != null)
                    {
                        symbolIdRequest.CableSymbolIdRequest.AddRange( (from item in request.DesignData.Cables
                                                                select new GetSurveyCableSymbolIdRequest
                                                                {
                                                                    Id = item.Id,
                                                                    State = item.State,
                                                                    Type = item.C_Type
                                                                }).ToList());
                    }
                    if (request.DesignData.CableEquipments != null)
                    {
                        symbolIdRequest.CableEquipmentSymbolIdRequest.AddRange( (from item in request.DesignData.CableEquipments
                                                                         select new GetSurveyCableEquipmentSymbolIdRequest
                                                                         {
                                                                             Id = item.Id,
                                                                             State = item.State,
                                                                             Type = item.Type
                                                                         }).ToList());
                    }
                    if (request.DesignData.OverheadEquipments != null)
                    {
                        symbolIdRequest.OverheadEquipmentRequest.AddRange( (from item in request.DesignData.OverheadEquipments
                                                                    select new GeOverheadEquipmentSymbolIdRequest
                                                                    {
                                                                        Id = item.Id,
                                                                        Type = item.Type,
                                                                        State=item.State
                                                                    }).ToList());
                    }
                    if (request.DesignData.Lines != null)
                    {
                        symbolIdRequest.LineRequest.AddRange( (from item in request.DesignData.Lines
                                                       select new GetLineSymbolIdRequest
                                                       {
                                                           Id = item.Id,
                                                           IsCable = item.IsCable,
                                                           KVLevel = item.KVLevel,
                                                           State = item.State
                                                       }).ToList());
                    }
                }
                rtnData = await _apiClient.ExecuteAsync<SymbolIdResponse>(HttpMethod.Post, getSymbolIdsUrl, symbolIdRequest);
            }
            catch (Exception ex)
            {

                throw new BusinessException($"勘察服务计算型号id异常{ex.Message}") ;
            }
            return rtnData;
        }


        /// <summary>
        ///     获取SymbolId
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sourceObj"></param>
        /// <returns></returns>
        private int GetSymbolId(object obj,object sourceObj)
        {
            
            //默认是否选型皆为是
            string symbolId = "1";
            try
            {
                if (obj.GetType() == typeof(DesignTowerDto))
                {
                    var curData = (DesignTowerDto)obj;
                    symbolId += (int)curData.State;
                    if (sourceObj == null) return 0;
                    var curEnum= ((TowerViewResponse)sourceObj).Material?.ToEnum<PoleMaterial>();
                    if (curEnum == null) return 0;
                    symbolId += (int)curEnum;
                    return Convert.ToInt32(symbolId);
                }
                if (obj.GetType() == typeof(SurveyCable))
                {
                    var curData = (SurveyCable)obj;
                    symbolId += (int)curData.State;
                    symbolId += (int)curData.C_Type;
                }
                if (obj.GetType() == typeof(SurveyCableEquipment))
                {
                    var curData = (SurveyCableEquipment)obj;
                    symbolId += (int)curData.Type;
                    symbolId += (int)curData.State;
                }
                if (obj.GetType() == typeof(OverheadEquipmentRequest))
                {
                    var curData = (OverheadEquipmentRequest)obj;
                    symbolId += (int)curData.Type;
                }
                if (obj.GetType() == typeof(LineRequest))
                {
                    var curData = (LineRequest)obj;
                    symbolId += curData.IsCable?"1":"0";
                    symbolId += (int)curData.State ;
                    symbolId += (int)curData.KVLevel;
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
            return Convert.ToInt32(symbolId);
        }
        #endregion

        #region  拉线信息
        /// <summary>
        ///     添加拉线数据
        /// </summary>
        /// <param name="towers"></param>
        private async Task InsertPullLineData(ReportSurveyDataRequest requestInfo, List<BatchTowerViewResponse> sourceInfo, string libId)
        {
            try
            {
                if (sourceInfo == null|| sourceInfo.Count==0) return;
                //增加拉线数据
                var addSurveyPullInfo = new List<SurveyPullLine>();
                var addDismantlePullInfo = new List<DismantlePullLine>();
                var addDesingnPullInfo = new List<DesignPullLine>();

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
                            addSurveyPullInfo.Add(new SurveyPullLine
                            {
                                D_Mode = item.ComponentSpec,
                                D_ModeID = item.ComponentID,
                                Main_Id = curTower.Id,
                                State = curTower.State,
                                ProjectId = curTower.ProjectId,
                                Lat_WGS84= curTower.Lat_WGS84,
                                Lon_WGS84= curTower.Lon_WGS84,
                                Azimuth=0
                            });
                            addDismantlePullInfo.Add(new DismantlePullLine
                            {
                                D_Mode = item.ComponentSpec,
                                D_ModeID = item.ComponentID,
                                Main_Id = curTower.Id,
                                State = curTower.State,
                                ProjectId = curTower.ProjectId,
                                Lat_WGS84 = curTower.Lat_WGS84,
                                Lon_WGS84 = curTower.Lon_WGS84,
                                Azimuth = 0
                            });
                        }
                    }
                  
                        curTower = requestInfo.DesignData.Towers?.FirstOrDefault(x => x.Id == item.TowerID);
                        if (curTower == null) continue;
                        for (int i = 0; i < item.ItemNumber; i++)
                        {

                            addDesingnPullInfo.Add(new DesignPullLine
                            {
                                D_Mode = item.ComponentSpec,
                                D_ModeID = item.ComponentID,
                                Main_Id = curTower.Id,
                                State = curTower.State,
                                ProjectId = curTower.ProjectId,
                                Lat_WGS84 = curTower.Lat_WGS84,
                                Lon_WGS84 = curTower.Lon_WGS84,
                                Azimuth = 0
                            });

                        }
                }
                if(addSurveyPullInfo!=null&&addSurveyPullInfo.Count>0) await DbHelper.BulkInsertAsync(addSurveyPullInfo);
                if (addDismantlePullInfo!=null&&addDismantlePullInfo.Count >0) await DbHelper.BulkInsertAsync(addDismantlePullInfo);
               // if (addDesingnPullInfo!=null&&addDesingnPullInfo.Count >0l) await DbHelper.BulkInsertAsync(addDesingnPullInfo);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"勘察端添加拉线数据异常，参数{JsonConvert.SerializeObject(sourceInfo)},错误信息{ex.Message}");
            }
        }

        #endregion

        #region 变压器
        /// <summary>
        ///     获取变压器组件信息
        /// </summary>
        /// <returns></returns>
        private async Task<List<TransformerResponse>> GetTransformerList(List<OverheadEquipmentRequest> requests, List<DesignTowerDto> towerRequests, string resourceLibId)
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
                    var curTower = towerRequests.FirstOrDefault(x => x.Id == item.Main_ID);
                    if (curTower == null||string.IsNullOrEmpty(curTower.Specification)|| !curTower.Specification.Contains("*")) continue;
                    if (item.Capacity == null|| item.Capacity==0) continue;
                    if(!Enum.IsDefined(typeof(TransformerCapacity), item.Capacity.Value))  continue;
                    getInfoRequest.TransformerProperties.Add(new TransformerPropertyRequest { Capacity=(TransformerCapacity)item.Capacity,Height=Convert.ToDouble(curTower.Specification.Split('*')[1]),TransformerID=item.Id });
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

        /// <summary>
        ///     将字符串转化为md5(32位)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string UserMd5(string str)
        {
            return str;
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd = pwd + s[i].ToString("X");

            }
            return pwd;
        }

    }
}
