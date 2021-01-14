using Cyg.Applicatio.Repository;
using Cyg.Extensions.Service;
using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service.Impl
{
    /// <summary>
    /// 订阅服务
    /// </summary>
    public class SubscribeService: BaseSubscriberService, ISubscriberService
    {
        private ISocketService _socketService;

        private readonly IGisTransformService gisTransformService;
        public SubscribeService(IGisTransformService gisTransformService, ISocketService socketService)
        {
            this.gisTransformService = gisTransformService;
            _socketService = socketService;
        }
        /// <summary>
        /// 转换marialdb勘察数据到Postgis
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        [CapSubscribe("GisTransformSurveyDataByProject")]
        public async Task GisTransformSurveyDataByProjectAysnc(List<string> projectIds)
        {
            if (projectIds.HasVal())
            {
                await gisTransformService.GisTransformProjectAsync(projectIds);
            }
        }
        [CapSubscribe("design.services.webGis.publish")]
        public async Task WebGisPubish(string data)
        {
            //告知客户端批注消息更新
            foreach (var item in SocketHandler.userDic)
            {
                item.Content = data;
                SocketHandler.SendMessage(item);
            }
        }

        /// <summary>
        ///     发送消息告知gis更新
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [CapSubscribe("design.services.webGis.update")]
        public void  WebGisUpdate(string projectId)
        {
            Task.Run(() =>
            {
                _socketService.SendMessage(projectId);
            });
        }
    }
}
