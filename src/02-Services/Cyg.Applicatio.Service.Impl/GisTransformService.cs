using Cyg.Applicatio.Entity;
using Cyg.Applicatio.Repository;
using Cyg.Extensions.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service.Impl
{
    /// <summary>
    /// MarialDb数据转存postgis
    /// </summary>
    public class GisTransformService : BaseService, IGisTransformService
    {
        private readonly IGisTransformRepository repository;
        public GisTransformService(IGisTransformRepository repository)
        {
            this.repository = repository;
        }
        /// <summary>
        /// 按项目MarialDb数据转存postgis
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        public async Task GisTransformProjectAsync(List<string> projectIds)
        {
              await repository.GisTransformProjectAsync(projectIds);
        }

        /// <summary>
        /// 按项目转换marialdb交底轨迹数据到postgis
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        public async Task GisTransformDisclosureTrack(List<string> projectIds)
        {
            await repository.GisTransformDisclosureTrack(projectIds);
        }
    }
}
