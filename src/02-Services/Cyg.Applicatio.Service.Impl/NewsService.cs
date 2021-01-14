using Cyg.Applicatio.Dto;
using Cyg.Applicatio.Entity;
using Cyg.Applicatio.Repository;
using Cyg.Applicatio.Survey.Dto;
using Cyg.Extensions.AutoMapper;
using Cyg.Extensions.Service;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service.Impl
{
    /// <summary>
    /// 新闻资讯服务
    /// </summary>
    public class NewsService : BaseService, INewsService
    {
        private readonly INewsRepository repository;
        public NewsService(INewsRepository repository)
        {
            this.repository = repository;
        }
        #region 获取分页列表
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IPageList<NewsResponse>> GetPageListAsync(QueryNewsRequest request)
        {
            return await repository.GetPageListAsync(request);
        }
        #endregion

        #region 根据主键获取实体对象
        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NewsResponse> GetNewsAsync(string id)
        {
            return await repository.GetNewsDetail(id);
        }
        #endregion
    }
}
