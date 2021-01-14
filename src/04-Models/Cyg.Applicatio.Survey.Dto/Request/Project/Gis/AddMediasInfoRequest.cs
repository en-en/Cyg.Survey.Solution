using System;
using System.Collections.Generic;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto.Request.Project.Gis
{
    /// <summary>
    /// 增加图层方案数据
    /// </summary>
    public class AddMediasInfoRequest
    {
        /// <summary>
        /// 勘察多媒体文件
        /// </summary>
        public List<MediaRequest> SurveyMedias { get; set; }

        /// <summary>
        /// 方案多媒体文件
        /// </summary>
        public List<MediaRequest> PlanMedias { get; set; }
    }
}
