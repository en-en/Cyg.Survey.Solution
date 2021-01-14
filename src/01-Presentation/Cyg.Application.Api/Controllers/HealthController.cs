using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cyg.Applicatio.Repository;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace Cyg.Application.Api.Controllers
{
    [Route("/api/health/")]
    public class HealthController : Controller
    {

        /// <summary>
        ///     心跳检查
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{ip}")]
        public IActionResult Heartbeat(string ip)
        {
            var rtnStr = string.Empty;
            var curConnect = SocketHandler.userDic?.FirstOrDefault(x => x.Ip == ip);
            if (curConnect != null)
            {
                curConnect.ConnectTime = DateTime.Now;
                rtnStr = "websoket连接";
            }
            else
            {
                rtnStr = "websoket断开";
            }
            return View(Json(rtnStr));
        }
    }
}