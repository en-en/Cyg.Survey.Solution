using Cyg.Applicatio.Survey.Dto;
using Cyg.Resource.Dto.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Cyg.Resource.Dto.Request;

namespace Cyg.Applicatio.Service
{
    public interface IResourceService : IScopeDependency
    {
        /// <summary>
        /// 获取杆型和杆规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ResponseBase<List<TowerOptionsResponse>>> GetTowerTypeOptionsAsync(ResourceOptionsRequest request);

        /// <summary>
        /// 获取线路型号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ResponseBase<List<LineOptionsResponse>>> GetLineTypeOptionsAsync(ResourceOptionsRequest request);

        /// <summary>
        /// 获取电缆井类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ResponseBase<List<CableWellOptionsResponse>>> GetCableWellTypeOptionsAsync(BaseResourceRquest request);

        /// <summary>
        /// 获取电缆通道类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ResponseBase<List<ChannelOptionsResponse>>> GetChannelTypeOptionsAsync(BaseResourceRquest request);

        /// <summary>
        /// 获取电气设备类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ResponseBase<List<CableEquipmentOptionsResponse>>> GetCableEquipmentOptionsAsync(BaseResourceRquest request);

        /// <summary>
        ///     获取项目基础信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<SelectionOptionsResponse> GetSourceSelectionOptions(string projectId);
    }
}
