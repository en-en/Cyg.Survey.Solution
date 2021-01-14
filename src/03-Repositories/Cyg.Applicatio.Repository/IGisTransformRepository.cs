using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Repository
{
    /// <summary>
    /// MarialDb数据转存postgis
    /// </summary>
    public interface IGisTransformRepository:IScopeDependency
    {
        /// <summary>
        /// 按项目MarialDb数据转存postgis
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        Task GisTransformProjectAsync(List<string> projectIds);

        /// <summary>
        /// 按项目转换marialdb交底轨迹数据到postgis
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        Task GisTransformDisclosureTrack(List<string> projectIds);
    }
}
