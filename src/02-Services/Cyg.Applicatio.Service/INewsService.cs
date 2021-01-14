using Cyg.Applicatio.Survey.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service
{
    /// <summary>
    /// 新闻资讯服务
    /// </summary>
    public interface INewsService:IScopeDependency
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IPageList<NewsResponse>> GetPageListAsync(QueryNewsRequest request);


        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<NewsResponse> GetNewsAsync(string id);
    }
}
