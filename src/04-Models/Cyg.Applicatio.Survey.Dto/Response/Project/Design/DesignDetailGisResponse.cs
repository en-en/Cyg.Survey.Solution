using Cyg.Applicatio.Survey.Dto.Response.Project.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cyg.Applicatio.Survey.Dto.Response.Project
{
    public class DesignDetailGisResponse
    {
        /// <summary>
        /// 工程基本信息
        /// </summary>
        public ProjectResponse Project { get; set; }

        /// <summary>
        /// 设计数据
        /// </summary>
        public DesignCollectionGisDataResponse DesignData { get; set; }

        /// <summary>
        /// 拆除数据
        /// </summary>
        public DismantleCollectionGisDataResponse DismantleData { get; set; }
    }
}
