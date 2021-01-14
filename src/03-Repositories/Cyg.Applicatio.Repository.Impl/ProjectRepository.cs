using Cyg.Applicatio.Dto;
using Cyg.Applicatio.Dto.Enums;
using Cyg.Applicatio.Entity;
using Cyg.Applicatio.Survey.Dto;
using Cyg.Extensions;
using Cyg.Extensions.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Repository.Impl
{
    public class ProjectRepository : BaseRepository, IProjectRepository
    {
        #region 获取项目列表
        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<ProjectResponse>> GetListAsync(ProjectRequest request)
        {
            var query = new SqlBuilder();
            return await DbHelper.QueryAsync<ProjectResponse>(query);
        }
        #endregion

        #region 获取项目明细
        /// <summary>
        /// 获取项目明细
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<ProjectDetailResponse> GetDetailAsync(string projectId)
        {
            var query = new SqlBuilder();
            query.AddSql(@"
select distinct pro.Id
              , pro.Name            ProjectName
              , pro.EngineerId      ProjectCollectionId
              , eng.Name            ProjectCollectionName
              , pro.PType           ProjectType
              , pro.Code            ProjectCode
              , pro.KvLevel         KVLevel
              , pro.ConstructType   Construction
              , pro.Stage           Stage
              , pro.Batch           Batch
              , eng.PlannedYear     BatchYear
              , pro.PileRange       PileRange
              , pro.DisclosureRange DisclosureRange
              , ''                  Province
              , ''                  City
              , ''                  Area
              , pro.Meteorologic    Meteorologic
              , pro.CompanyId       CompanyId
              , pro.CreatedOn       CreatedOn
              , ps.Status           FlowState
              , ps.IsResetSurvey    IsResetSurvey
              , '备注'                Remark
from pdd_t_project pro
         inner join pdd_t_engineer eng on eng.Id = pro.EngineerId
         inner join pdd_t_project_state ps on ps.Id = pro.Id
         inner join pdd_t_project_allot_user pau
                    on pau.ProjectId = pro.Id and pau.UserId = @UserId and pau.ArrangeType = 1 
where pro.Id = @ProjectId;");
            query.AddParam("UserId", CurrentUser.Id);
            query.AddParam("ProjectId",projectId);
            var project = await DbHelper.QuerySingleAsync<ProjectResponse>(query);
            var projectState = await DbHelper.FindByIdAsync<ProjectState>(project.Id);
            ProjectDetailResponse result = null;
            if (project != null)
            {
                result = new ProjectDetailResponse();
                result.Project = project;
                if (project.FlowState == ProjectFlowState.未勘察)
                {
                    return result;
                }
                query.ClearSql();
                query.AddSql(@"
                SELECT tower.*,extTower.PoleTypeCode FROM pdd_t_survey_tower  tower
                LEFT JOIN pdd_t_survey_tower_ext extTower 
                on tower.ID=extTower.ID
                WHERE tower.ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT ptsc.*,ptsce.C_ModeID
                FROM pdd_t_survey_cable as ptsc
				LEFT JOIN pdd_t_survey_cable_ext as ptsce ON ptsc.ID=ptsce.ID
                WHERE ptsc.ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_survey_cablechannel
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_survey_cabledevice
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_survey_overheaddevice
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT line.*,lineExt.L_ModeID FROM pdd_t_survey_line  line
                LEFT JOIN pdd_t_survey_line_ext lineExt on lineExt.Id=line.ID
                WHERE line.ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_survey_mark
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_survey_media
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_trackrecord
                WHERE ProjectID=@ProjectId and RecordType=1;");
                query.AddSql(@"
                SELECT *
                FROM pdd_t_design_tower tower
                LEFT JOIN pdd_t_design_tower_ext extTower 
                on tower.ID=extTower.ID
                WHERE tower.ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT ptdc.*,ptdce.C_ModeID
                FROM pdd_t_design_cable  as ptdc
				LEFT JOIN pdd_t_design_cable_ext as ptdce ON ptdc.ID=ptdce.ID
                WHERE ptdc.ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_design_cablechannel
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_design_cabledevice
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_design_overheaddevice
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT line.*,lineExt.L_ModeID FROM pdd_t_design_line  line
                LEFT JOIN pdd_t_design_line_ext lineExt on lineExt.Id=line.ID
                WHERE line.ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_design_mark
                WHERE ProjectID=@ProjectId;");
                query.AddSql(@"
                SELECT * FROM pdd_t_design_media
                WHERE ProjectID=@ProjectId;");

                if ((projectState.OutsideStatus & ProjectOutsideStatus.交底中) > 0 || (projectState.OutsideStatus & ProjectOutsideStatus.交底完成) > 0)
                {
                    query.AddSql(@"
                    SELECT * FROM pdd_t_trackrecord
                    WHERE ProjectID=@ProjectId and RecordType=2;");
                    query.AddSql(@"
                    SELECT * FROM pdd_t_project_disclosure
                    WHERE ProjectID=@ProjectId;");
                    query.AddSql(@"
                    SELECT * FROM pdd_t_project_disclosureitem
                    WHERE ProjectID=@ProjectId;");
                }
                using (var reader = await DbHelper.QueryMultipleAsync(query))
                {
                    result.SurveyData = new CollectionDataResponse();
                    result.SurveyData.Towers = await reader.ReadAsync<TowerResponse>();
                    result.SurveyData.Cables = await reader.ReadAsync<CableResponse>();
                    result.SurveyData.CableChannels = await reader.ReadAsync<CableChannelResponse>();
                    result.SurveyData.CableEquipments = await reader.ReadAsync<CableEquipmentResponse>();
                    result.SurveyData.OverheadEquipments = await reader.ReadAsync<OverheadEquipmentResponse>();
                    result.SurveyData.Lines = await reader.ReadAsync<LineResponse>();
                    foreach (var surveyDataLine in result.SurveyData.Lines)
                    {
                        surveyDataLine.L_Mode = surveyDataLine.L_ModeID;
                    }
                    result.SurveyData.Marks = await reader.ReadAsync<MarkResponse>();
                    result.SurveyData.Medias = await reader.ReadAsync<MediaResponse>();
                    result.SurveyData.TrackRecords = await reader.ReadAsync<ProjectTrackRecordResponse>();

                    result.DesignData = new CollectionDataResponse();
                    result.DesignData.Towers = await reader.ReadAsync<TowerResponse>();
                    result.DesignData.Cables = await reader.ReadAsync<CableResponse>();
                    result.DesignData.CableChannels = await reader.ReadAsync<CableChannelResponse>();
                    result.DesignData.CableEquipments = await reader.ReadAsync<CableEquipmentResponse>();
                    result.DesignData.OverheadEquipments = await reader.ReadAsync<OverheadEquipmentResponse>();
                    result.DesignData.Lines = await reader.ReadAsync<LineResponse>();
                    foreach (var designDataLine in result.DesignData.Lines)
                    {
                        designDataLine.L_Mode = designDataLine.L_ModeID;
                    }
                    result.DesignData.Marks = await reader.ReadAsync<MarkResponse>();
                    result.DesignData.Medias = await reader.ReadAsync<MediaResponse>();

                    
                    if ((projectState.OutsideStatus&ProjectOutsideStatus.交底中)>0|| (projectState.OutsideStatus & ProjectOutsideStatus.交底完成)>0)
                    {
                        result.DisclosureData = new DisclosureDataResponse();
                        result.DisclosureData.TrackRecords = await reader.ReadAsync<ProjectTrackRecordResponse>();
                        result.DisclosureData.Disclosures = (await reader.ReadAsync<ProjectDisclosureResponse>())?.ToList();
                        var disclosureItems = (await reader.ReadAsync<ProjectDisclosureItemResponse>())?.ToList();
                        if (result.DisclosureData.Disclosures.HasVal() && disclosureItems.HasVal())
                        {
                            foreach (var item in result.DisclosureData.Disclosures)
                            {
                                item.Items = disclosureItems.Where(m => m.DisclosureId == item.Id).ToList();
                                disclosureItems.RemoveAll(m => m.DisclosureId == item.Id);
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取项目明细
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<DisclosureDataResponse> GetDisclosureDataResponseAsync(string projectId)
        {
            var query = new SqlBuilder();
            query.AddSql(@"
            select distinct pro.Id
                          , pro.Name            ProjectName
                          , pro.EngineerId      ProjectCollectionId
                          , eng.Name            ProjectCollectionName
                          , pro.PType           ProjectType
                          , pro.Code            ProjectCode
                          , pro.KvLevel         KVLevel
                          , pro.ConstructType   Construction
                          , pro.Stage           Stage
                          , pro.Batch           Batch
                          , eng.PlannedYear     BatchYear
                          , pro.PileRange       PileRange
                          , pro.DisclosureRange DisclosureRange
                          , ''                  Province
                          , ''                  City
                          , ''                  Area
                          , pro.Meteorologic    Meteorologic
                          , pro.CompanyId       CompanyId
                          , pro.CreatedOn       CreatedOn
                          , ps.Status           FlowState
                          , ps.IsResetSurvey    IsResetSurvey
                          , '备注'                Remark
            from pdd_t_project pro
                     inner join pdd_t_engineer eng on eng.Id = pro.EngineerId
                     inner join pdd_t_project_state ps on ps.Id = pro.Id
                     inner join pdd_t_project_allot_user pau
                                on pau.ProjectId = pro.Id and pau.UserId = @UserId and pau.ArrangeType = 1 
            where pro.Id = @ProjectId;");
            query.AddParam("UserId", CurrentUser.Id);
            query.AddParam("ProjectId", projectId);
            var project = await DbHelper.QuerySingleAsync<ProjectResponse>(query);
            var projectState = await DbHelper.FindByIdAsync<ProjectState>(project.Id);
            DisclosureDataResponse result = null;
            if (project != null)
            {
                result = new DisclosureDataResponse();
                if (project.FlowState == ProjectFlowState.未勘察)
                {
                    return result;
                }
                query.ClearSql();
                if ((projectState.OutsideStatus & ProjectOutsideStatus.交底中) > 0 || (projectState.OutsideStatus & ProjectOutsideStatus.交底完成) > 0)
                {
                    query.AddSql(@"
                    SELECT * FROM pdd_t_trackrecord
                    WHERE ProjectID=@ProjectId and RecordType=2;");
                    query.AddSql(@"
                    SELECT * FROM pdd_t_project_disclosure
                    WHERE ProjectID=@ProjectId;");
                    query.AddSql(@"
                    SELECT * FROM pdd_t_project_disclosureitem
                    WHERE ProjectID=@ProjectId;");
                    using (var reader = await DbHelper.QueryMultipleAsync(query))
                    {
                        if ((projectState.OutsideStatus & ProjectOutsideStatus.交底中) > 0 || (projectState.OutsideStatus & ProjectOutsideStatus.交底完成) > 0)
                        {
                            result.TrackRecords = await reader.ReadAsync<ProjectTrackRecordResponse>();
                            result.Disclosures = (await reader.ReadAsync<ProjectDisclosureResponse>())?.ToList();
                            var disclosureItems = (await reader.ReadAsync<ProjectDisclosureItemResponse>())?.ToList();
                            if (result.Disclosures.HasVal() && disclosureItems.HasVal())
                            {
                                foreach (var item in result.Disclosures)
                                {
                                    item.Items = disclosureItems.Where(m => m.DisclosureId == item.Id).ToList();
                                    disclosureItems.RemoveAll(m => m.DisclosureId == item.Id);
                                }
                            }
                        }
                    }
                }
              
            }
            return result;
        }

        /// <summary>
        /// 获取项目(Gis，项目信息，项目轨迹，交底信息)
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<ProjectDetailGisResponse> GetGisDetailAsync(string projectId)
        {
            var query = new SqlBuilder();
            query.AddSql(@"
              select distinct pro.Id
              , pro.Name            ProjectName
              , pro.EngineerId      ProjectCollectionId
              , eng.Name            ProjectCollectionName
              , pro.PType           ProjectType
              , pro.Code            ProjectCode
              , pro.KvLevel         KVLevel
              , pro.ConstructType   Construction
              , pro.Stage           Stage
              , pro.Batch           Batch
              , eng.PlannedYear     BatchYear
              , pro.PileRange       PileRange
              , pro.DisclosureRange DisclosureRange
              , ''                  Province
              , ''                  City
              , ''                  Area
              , pro.Meteorologic    Meteorologic
              , pro.CompanyId       CompanyId
              , pro.CreatedOn       CreatedOn
              , ps.Status           FlowState
              , ps.IsResetSurvey    IsResetSurvey
              , '备注'                Remark
from pdd_t_project pro
         inner join pdd_t_engineer eng on eng.Id = pro.EngineerId
         inner join pdd_t_project_state ps on ps.Id = pro.Id
         inner join pdd_t_project_allot_user pau
                    on pau.ProjectId = pro.Id and pau.UserId = @UserId and pau.ArrangeType = 1 
where pro.Id = @ProjectId;");
            query.AddParam("UserId", CurrentUser.Id);
            query.AddParam("ProjectId", projectId);
            var project = await DbHelper.QuerySingleAsync<ProjectResponse>(query);
            var projectState = await DbHelper.FindByIdAsync<ProjectState>(project.Id);
            ProjectDetailGisResponse result = null;
            if (project != null)
            {
                result = new ProjectDetailGisResponse
                {
                    Project = project,
                    SurveyData = new CollectionDataGisResponse(),
                    PlanData=new CollectionDataGisResponse()
                };
                if (project.FlowState == ProjectFlowState.未勘察)
                {
                    return result;
                }
               query.ClearSql();

                query.AddSql(@"
                SELECT * FROM pdd_t_trackrecord
                WHERE ProjectID=@ProjectId and RecordType=1;");



                if ((projectState.OutsideStatus & ProjectOutsideStatus.交底中) > 0 || (projectState.OutsideStatus & ProjectOutsideStatus.交底完成) > 0)
                {
                    query.AddSql(@"
                    SELECT * FROM pdd_t_trackrecord
                    WHERE ProjectID=@ProjectId and RecordType=2;");
                    query.AddSql(@"
                    SELECT * FROM pdd_t_project_disclosure
                    WHERE ProjectID=@ProjectId;");
                    query.AddSql(@"
                    SELECT * FROM pdd_t_project_disclosureitem
                    WHERE ProjectID=@ProjectId;");
                }
                using (var reader = await DbHelper.QueryMultipleAsync(query))
                {
                    result.SurveyData.TrackRecords = await reader.ReadAsync<ProjectTrackRecordResponse>();

                    if ((projectState.OutsideStatus & ProjectOutsideStatus.交底中) > 0 || (projectState.OutsideStatus & ProjectOutsideStatus.交底完成) > 0)
                    {
                        result.DisclosureData = new DisclosureDataResponse();
                        result.DisclosureData.TrackRecords = await reader.ReadAsync<ProjectTrackRecordResponse>();
                        result.DisclosureData.Disclosures = (await reader.ReadAsync<ProjectDisclosureResponse>())?.ToList();
                        var disclosureItems = (await reader.ReadAsync<ProjectDisclosureItemResponse>())?.ToList();
                        if (result.DisclosureData.Disclosures.HasVal() && disclosureItems.HasVal())
                        {
                            foreach (var item in result.DisclosureData.Disclosures)
                            {
                                item.Items = disclosureItems.Where(m => m.DisclosureId == item.Id).ToList();
                                disclosureItems.RemoveAll(m => m.DisclosureId == item.Id);
                            }
                        }
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
