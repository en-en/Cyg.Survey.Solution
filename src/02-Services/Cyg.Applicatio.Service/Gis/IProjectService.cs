using Cyg.Applicatio.Survey.Dto;
using Cyg.Applicatio.Survey.Dto.Request.Project.Gis;
using Cyg.Applicatio.Survey.Dto.Response.Project;
using System;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service
{
    public partial interface IProjectService : IScopeDependency
    {
        /// <summary>
        /// 上报勘测数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
         Task ReportSurveyGisDataAsync(Survey.Dto.ReportSurveyGisDataRequest request);

        /// <summary>
        /// 获取工程明细
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<ProjectDetailGisResponse> GetGisDetailAsync(string projectId);

        /// <summary>
        /// 获取设计拆除数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<DesignDetailGisResponse> GetGisDesignDetailAsync(string projectId);

        /// <summary>
        /// 添加多媒体数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task AddMediasInfoAsync(AddMediasInfoRequest request);
    }
}
