using System;
using System.Collections.Generic;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto
{
    /// <summary>
    /// 新闻资讯响应
    /// </summary>
    public class NewsResponse
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///工程名称
        /// </summary>
        public string EngineerName { get; set; }

        /// <summary>
        /// 线路|杆塔id 模板消息时为空
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// 新闻内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 新闻所属公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        ///     推送人
        /// </summary>
        public string UserName { get; set; }
    }
}
