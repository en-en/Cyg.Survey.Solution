using Cyg.Applicatio.Service;
using Cyg.Applicatio.Survey.Dto;
using Cyg.Extensions.WebApi;
using Cyg.Resource.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cyg.Application.Api.Controllers
{
    /// <summary>
    /// 资源库管理
    /// </summary>
    public class ResourceController : BaseApiController<IResourceService>
    {

        #region 获取资源库信息
        /// <summary>
        ///     获取项目基础信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
       [HttpGet("GetSourceSelectionOptions")]
        public  Task<SelectionOptionsResponse> GetSourceSelectionOptions(string projectId)
        {
            try
            {
                return Service.GetSourceSelectionOptions(projectId);
            }
            catch (System.Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        #endregion

        #region 获取杆型和杆规格
        /// <summary>
        /// 获取杆型和杆规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetTowerTypeOptions")]
        [Produces(typeof(List<TowerOptionsResponse>))]
        public Task<ResponseBase<List<TowerOptionsResponse>>> GetTowerTypeOptionsAsync(ResourceOptionsRequest request)
        {
            return Service.GetTowerTypeOptionsAsync(request);
        }
        #endregion

        #region 获取线路型号
        /// <summary>
        /// 获取线路型号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetLineTypeOptions")]
        [Produces(typeof(List<LineOptionsResponse>))]
        public Task<ResponseBase<List<LineOptionsResponse>>> GetLineTypeOptionsAsync(ResourceOptionsRequest request)
        {
            return Service.GetLineTypeOptionsAsync(request);
        }
        #endregion

        #region 获取电缆井类型规格
        /// <summary>
        /// 获取电缆井类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetCableWellTypeOptions")]
        [Produces(typeof(List<CableWellOptionsResponse>))]
        public Task<ResponseBase<List<CableWellOptionsResponse>>> GetCableWellTypeOptionsAsync(BaseResourceRquest request)
        {
            return Service.GetCableWellTypeOptionsAsync(request);
        }
        #endregion

        #region 获取电缆通道类型规格
        /// <summary>
        /// 获取电缆通道类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetChannelTypeOptions")]
        [Produces(typeof(List<ChannelOptionsResponse>))]
        public Task<ResponseBase<List<ChannelOptionsResponse>>> GetChannelTypeOptionsAsync(BaseResourceRquest request)
        {
            return Service.GetChannelTypeOptionsAsync(request);
        }
        #endregion

        #region 获取电气设备类型规格
        /// <summary>
        /// 获取电气设备类型规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetCableEquipmentOptions")]
        [Produces(typeof(List<CableEquipmentOptionsResponse>))]
        public Task<ResponseBase<List<CableEquipmentOptionsResponse>>> GetCableEquipmentOptionsAsync(BaseResourceRquest request)
        {
            return Service.GetCableEquipmentOptionsAsync(request);
        }
        #endregion
    }
}