using Cyg.Applicatio.Entity;
using Cyg.Applicatio.Survey.Dto;
using Cyg.Extensions.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Repository.Impl
{
    public class NewsRepository : BaseRepository, INewsRepository
    {
        /// <summary>
        ///     获取文件详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NewsResponse> GetNewsDetail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return null;
                var sqlBuider = new SqlBuilder();
                var sql = @"SELECT
                            new.id,
                            new.title,
                            engineer.`Name` as EngineerName,
                            new.ObjectId,
                            content.content,
                            new.createdOn,
                            new.publishTime,
                            new.company,
                            userInfo.UserName
                            FROM
                                pdd_t_news new
                            LEFT JOIN pdd_t_project_news_map newMap ON new.id = newMap.NewsId
                            LEFT JOIN pdd_t_project project on project.Id = newMap.ProjectId
                            LEFT JOIN pdd_t_engineer engineer ON engineer.id = project.EngineerId
                            LEFT JOIN pdd_t_news_content content on content.id = new.id
                            LEFT JOIN pdd_t_users  userInfo ON new.createdBy=userInfo.Id
                            WHERE  new.id=@id";
                sqlBuider.AddSql(sql);
                sqlBuider.AddParam("id", id);
                return DbHelper.QuerySingle<NewsResponse>(sqlBuider);
            }
            catch (Exception ex)
            {
              throw  new BusinessException(ex.Message);
            }
          
        }
        #region 分页获取新闻列表
        /// <summary>
        /// 分页获取新闻列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IPageList<NewsResponse>> GetPageListAsync(QueryNewsRequest request)
        {
            var query = new SqlPageBuilder(request.PageIndex,request.PageSize);
			query.AddHeadSql(@"SELECT t.*");
			query.AddBodySql(@"
FROM
	(
	SELECT
		n.* 
	FROM
		pdd_t_news n
		JOIN pdd_t_news_publish np ON np.NewsId = n.id 
		AND np.NewsPublishType = 1
		JOIN pdd_t_users u ON u.CompanyID = np.ObjectId 
		AND u.ID = @UserId 
	WHERE
		n.`status` = 2 UNION
	SELECT
		n.* 
	FROM
		pdd_t_news n
		JOIN pdd_t_news_publish np ON np.NewsId = n.id 
		AND np.NewsPublishType = 2
		JOIN pdd_t_company_group_user cgu ON cgu.GroupId = np.ObjectId 
		AND cgu.UserId = @UserId 
	WHERE
		n.`status` = 2 UNION
	SELECT
		n.* 
	FROM
		pdd_t_news n
		JOIN pdd_t_news_publish np ON np.NewsId = n.id 
		AND np.NewsPublishType = 3
		JOIN pdd_t_users u ON u.ID = np.ObjectId 
		AND u.ID = @UserId 
	WHERE
	n.`status` = 2 
	) t
where 1=1
");
			if (request.KeyWord.HasVal())
			{
				query.AddBodySql("and t.Title like @Keyword");
				query.AddParam("Keyword", request.KeyWord);
			}
			if (request.Sort.HasVal())
			{
				query.AddOrderBySql($"order by t.{request.Sort.PropertyName} {(request.Sort.IsAsc?"ASC":"DESC")}");
			}
			else
			{
				query.AddOrderBySql($"order by t.publishTime desc");
			}
			query.AddParam("UserId", CurrentUser.Id);
			return await DbHelper.QueryPageListAsync<NewsResponse>(query);
        }
        #endregion
    }
}
