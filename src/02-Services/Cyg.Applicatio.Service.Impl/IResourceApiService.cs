using Cyg.Resource.Dto.Request;
using Cyg.Resource.Dto.Response;
using System.Collections.Generic;
using WebApiClient;
using WebApiClient.Attributes;

namespace Cyg.Applicatio.Service
{
    public interface IResourceApiService : IHttpApi
    {
        /// <summary>
        /// 根据电压等级和设计类型，获取杆型和杆规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [JsonReturn, HttpPost("api/LibraryOption/GetTowerTypeOptions")]
        ITask<ResponseBase<List<TowerOptionsResponse>>> GetTowerTypeOptionsAsync([JsonContent]SelectOptionsRequest request);

        /// <summary>
        /// 根据电压等级和设计类型，获取线路型号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [JsonReturn, HttpPost("api/LibraryOption/GetLineTypeOptions")]
        ITask<ResponseBase<List<LineOptionsResponse>>> GetLineTypeOptionsAsync([JsonContent]SelectOptionsRequest request);

        /// <summary>
        /// 获取电缆井类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [JsonReturn, HttpPost("/api/LibraryOption/GetCableWellTypeOptions")]
        ITask<ResponseBase<List<CableWellOptionsResponse>>> GetCableWellTypeOptionsAsync([JsonContent]LibraryTypeRequest request);

        /// <summary>
        /// 获取电缆通道类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [JsonReturn, HttpPost("/api/LibraryOption/GetChannelTypeOptions")]
        ITask<ResponseBase<List<ChannelOptionsResponse>>> GetChannelTypeOptionsAsync([JsonContent]LibraryTypeRequest request);

        /// <summary>
        /// 获取电气设备类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [JsonReturn, HttpPost("/api/LibraryOption/GetCableEquipmentOptions")]
        ITask<ResponseBase<List<CableEquipmentOptionsResponse>>> GetCableEquipmentOptionsAsync([JsonContent]LibraryTypeRequest request);

    }
}
