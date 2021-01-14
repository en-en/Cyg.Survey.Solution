using Cyg.Applicatio.Entity;
using Cyg.Applicatio.Survey.Dto;
using Cyg.Extensions;
using Cyg.Extensions.Service;
using Cyg.Resource.Dto.Request;
using Cyg.Resource.Dto.Response;
using Cyg.Resource.Enums;
using Cyg.WebApiClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service.Impl
{
    public class ResourceService : BaseService, IResourceService
    {
        private readonly IResourceApiService _resourceService;
        private readonly IProjectService _projectService;

        private readonly IConfiguration _configuration;

        private readonly IApiClient _apiClient;
        /// <summary>
        ///    获取所有类型规格信息
        /// </summary>
        private string sourceModulesUrl;

        public ResourceService(
            IResourceApiService resourceService, 
            IProjectService projectService,
            IConfiguration configuration,
            IApiClient apiClient)
        {
            _resourceService = resourceService;
            _projectService = projectService;
            _configuration = configuration;
            _apiClient = apiClient;
            Init();
        }

        private void Init()
        {
            sourceModulesUrl = $"{_configuration.GetSection("Config")["ResourceApi"]}/api/LibraryOption/GetSelectionOptions";
        }

        #region 获取资源库信息

        public async Task<SelectionOptionsResponse> GetSourceSelectionOptions(string projectId)
        {
            
            try
            {
                var projectInfo= DbHelper.Find<Project>(x => x.Id == projectId);
                if(!projectInfo.HasVal()) throw new BusinessException("未找到当前项目;" + projectId);
                var engineerInfo = DbHelper.Find<Engineer>(x => x.Id == projectInfo.EngineerId);
                if (!engineerInfo.HasVal()) throw new BusinessException("未找到当前工程;" + projectInfo.EngineerId);
                return await _apiClient.ExecuteAsync<SelectionOptionsResponse>(HttpMethod.Post, sourceModulesUrl, new SelectOptionsRequest { KVLevel=0,ResourceLibID= engineerInfo.LibId});
            }
            catch (System.Exception ex)
            {
                throw new BusinessException(ex.Message+";"+projectId);
            }
           
        }


        #endregion

        #region 获取杆型和杆规格
        /// <summary>
        /// 获取杆型和杆规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase<List<TowerOptionsResponse>>> GetTowerTypeOptionsAsync(ResourceOptionsRequest request)
        {
            var (project, _, projectCollection) = await _projectService.CheckSurveyPermissionAsync(request.ProjectId);
            var result = await _resourceService.GetTowerTypeOptionsAsync(new SelectOptionsRequest()
            {
                ResourceLibID = projectCollection.LibId,
                ForProject = (ProjectType)project.PType.ToInt(),
                ForDesign = request.ForDesign,
                KVLevel = KVLevel.无
            });
            //var result = await _resourceService.GetTowerTypeOptionsAsync(new SelectOptionsRequest()
            //{
            //    ResourceLibID = project.ResourceLibId,
            //    ForProject = projectCollection.ProjectType,
            //    ForDesign = request.ForDesign,
            //    KVLevel = KVLevel.无
            //});
            return result;
        }
        #endregion

        #region 获取线路型号
        /// <summary>
        /// 获取线路型号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase<List<LineOptionsResponse>>> GetLineTypeOptionsAsync(ResourceOptionsRequest request)
        {
            var (project, _, projectCollection) = await _projectService.CheckSurveyPermissionAsync(request.ProjectId);
            return await _resourceService.GetLineTypeOptionsAsync(new SelectOptionsRequest()
            {
                ResourceLibID = projectCollection.LibId,
                ForProject = (ProjectType)project.PType.ToInt(),
                ForDesign = request.ForDesign,
                KVLevel = KVLevel.无
            });
        }
        #endregion

        #region 获取电缆井类型规格
        /// <summary>
        /// 获取电缆井类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase<List<CableWellOptionsResponse>>> GetCableWellTypeOptionsAsync(BaseResourceRquest request)
        {
            var (project, _, engineer) = await _projectService.CheckSurveyPermissionAsync(request.ProjectId);
            return await _resourceService.GetCableWellTypeOptionsAsync(new LibraryTypeRequest()
            {
                ResourceLibID = engineer.LibId
            });
        }
        #endregion

        #region 获取电缆通道类型规格
        /// <summary>
        /// 获取电缆通道类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase<List<ChannelOptionsResponse>>> GetChannelTypeOptionsAsync(BaseResourceRquest request)
        {
            var (project, _, engineer) = await _projectService.CheckSurveyPermissionAsync(request.ProjectId);
            return await _resourceService.GetChannelTypeOptionsAsync(new LibraryTypeRequest()
            {
                ResourceLibID = engineer.LibId
            });
        }
        #endregion

        #region 获取电气设备类型规格
        /// <summary>
        /// 获取电气设备类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase<List<CableEquipmentOptionsResponse>>> GetCableEquipmentOptionsAsync(BaseResourceRquest request)
        {
            var (project, _,engineer) = await _projectService.CheckSurveyPermissionAsync(request.ProjectId);
            return await _resourceService.GetCableEquipmentOptionsAsync(new LibraryTypeRequest()
            {
                ResourceLibID = engineer.LibId
            });
        }
        #endregion
    }
}
