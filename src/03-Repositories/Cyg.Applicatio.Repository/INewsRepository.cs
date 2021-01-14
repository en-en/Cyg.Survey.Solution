using Cyg.Applicatio.Survey.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Repository
{
    public interface INewsRepository:IScopeDependency
    {
        /// <summary>
        /// 分页获取新闻列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IPageList<NewsResponse>> GetPageListAsync(QueryNewsRequest request);

        /// <summary>
        /// 获取文件详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<NewsResponse> GetNewsDetail(string id);
        
    }
}
