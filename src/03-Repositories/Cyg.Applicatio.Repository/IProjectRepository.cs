using Cyg.Applicatio.Survey.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Repository
{
    public interface IProjectRepository : IScopeDependency
    {
        /// <summary>
        /// 获取工程列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<ProjectResponse>> GetListAsync(ProjectRequest request);

        /// <summary>
        /// 获取工程明细
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<ProjectDetailResponse> GetDetailAsync(string projectId);

        /// <summary>
        /// 获取交底信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<DisclosureDataResponse> GetDisclosureDataResponseAsync(string projectId);

        /// <summary>
        /// 获取工程明细
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<ProjectDetailGisResponse> GetGisDetailAsync(string projectId);
        
    }
}
