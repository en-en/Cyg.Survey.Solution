using Cyg.Applicatio.Service;
using Cyg.Applicatio.Survey.Dto;
using Cyg.Extensions.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cyg.Application.Api.Controllers
{
    /// <summary>
    /// 新闻资讯
    /// </summary>
    public class NewsController:BaseApiController<INewsService>
    {
        /// <summary>
        /// 分页获取资讯列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetPageList")]
        public Task<IPageList<NewsResponse>> GetPageListAsync(QueryNewsRequest request)
        {
            return Service.GetPageListAsync(request);
        }

        /// <summary>
        /// 获取资讯内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetNews")]
        public  Task<NewsResponse> GetNewsAsync([DisplayName("资讯Id"), Required(ErrorMessage = "{0} 不能为空")]string id)
        {
            return Service.GetNewsAsync(id);
        }
    }
}
