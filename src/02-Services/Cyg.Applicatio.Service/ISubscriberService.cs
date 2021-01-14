using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service
{
    public interface ISubscriberService: ITransientDependency
    {
        /// <summary>
        /// 转换marialdb勘察数据到Postgis
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        Task GisTransformSurveyDataByProjectAysnc(List<string> projectIds);
    }
}
