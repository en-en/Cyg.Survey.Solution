
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cyg.Applicatio.Repository
{
    public class SocketHandler
    {

        public static ConcurrentQueue<string> concurrentQueue = new ConcurrentQueue<string>();

       /// public static ConcurrentQueue<WebSoketInfo> userDic = new ConcurrentQueue<WebSoketInfo>();

        public static List<WebSoketInfo> userDic = new List<WebSoketInfo>();

        //private static readonly ICapPublisher _capPublisher=new CapHandler()._capPublisher;

        SocketHandler(WebSocket socket)
        {
            this.socket = socket;
            this.BufferSize = 4 * 1024;
            //capPublisher = ;
        }

        int BufferSize;
        WebSocket socket;

        /// <summary>
        ///     
        /// </summary>
        /// <returns></returns>
        async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);
            while (this.socket.State == WebSocketState.Open)
            {
                Console.WriteLine("收到参数");
              //  WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);

                if (incoming.MessageType == WebSocketMessageType.Close)
                {
                    var curSocketInfo = userDic.FirstOrDefault(x => x.SocketInfo == this.socket);
                    if (curSocketInfo != null)
                    {
                        userDic.Remove(curSocketInfo);
                    }
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    String.Empty, CancellationToken.None);

                }
                else
                {
                    string receviceString = System.Text.Encoding.UTF8.GetString(buffer.Where(b => b != 0).ToArray());
                    if (string.IsNullOrEmpty(receviceString)) { return; }
                    string separator ="/";
                    var msg = receviceString.Split(new[] { separator }, StringSplitOptions.None);
                    string toname = string.Empty;
                    string data = string.Empty;
                    string fromname = string.Empty;

                    if (msg.Length > 2)
                    {
                        toname = msg[0];
                        data = msg[1];
                        fromname = msg[2];
                    }
                    string tomsg = data;
                    List<WebSocket> toSocketList = new List<WebSocket>();
                    if (!string.IsNullOrEmpty(toname))
                    {
                       if (data.Split('|').Length!= 2) continue;
                        var isExist = userDic.FirstOrDefault(x => x.Ip == fromname);

                       if (isExist == null)// !userDic.ContainsKey(fromname))
                        {
                            userDic.Add(new WebSoketInfo
                            {
                                Ip = fromname, ConnectTime = DateTime.Now, ServerName = toname, SocketInfo = this.socket
                            }) ;
                        }
                        if (toname == "Servey")
                        {
                            concurrentQueue.Enqueue(tomsg);
                            var buf = Encoding.UTF8.GetBytes(data.ToCharArray());
                            this.socket.SendAsync(new ArraySegment<byte>(buf, 0, buf.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                            continue;
                        }
                        else if (userDic.FirstOrDefault(x => x.Ip == toname) !=null)
                        {
                            toSocketList.Add(userDic.FirstOrDefault(x => x.Ip == toname).SocketInfo);
                        }
                        else
                        {
                            toSocketList.Add(userDic.FirstOrDefault(x => x.Ip == fromname).SocketInfo);
                            tomsg = "用户不在线";
                        }
                    }
                    else
                    {
                        toSocketList.AddRange(userDic?.Select(x=>x.SocketInfo));
                    }
                    byte[] toBuffer = System.Text.Encoding.UTF8.GetBytes(tomsg);
                    var outgoing = new ArraySegment<byte>(toBuffer);
                    foreach (WebSocket s in toSocketList)
                    {
                        await s.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            await this.socket.CloseAsync(this.socket.CloseStatus.Value, this.socket.CloseStatusDescription, CancellationToken.None);
        }
      
        /// <summary>
        ///     请求管道
        /// </summary>
        /// <param name="app"></param>
        public  static void Map(IApplicationBuilder app)
        {

            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    //var token = context.Request.Query["token"];
                    using (IServiceScope scope = app.ApplicationServices.CreateScope())
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var h = new SocketHandler(webSocket);
                        await h.EchoLoop();
                    }
                }
                else
                {
                    await next();
                }
            });
        }


        public static async Task SendMessage(WebSoketInfo info)
        {

            if (info == null || string.IsNullOrEmpty(info.Ip)) return;
            var curSocket = userDic.FirstOrDefault(x => x.Ip == info.Ip);
            //info.ServerName+"|"+
            var sendStr = info.Content;
            var buf = Encoding.UTF8.GetBytes(sendStr.ToCharArray());
            if (curSocket != null&& curSocket.SocketInfo.State== WebSocketState.Open)
            {
                await curSocket.SocketInfo.SendAsync(new ArraySegment<byte>(buf, 0, buf.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
           
        }
    }

    public class CapHandler
    {
        public readonly ICapPublisher _capPublisher;
        public CapHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }
        public void MessageHandler()
        {
            Task.Run(() => {
                while (true)
                {
                    try
                    {
                        if (SocketHandler.concurrentQueue.Count == 0)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        if (SocketHandler.userDic.Count > 0)
                        {

                            //var removeData = SocketHandler.userDic.Where(x => (DateTime.Now.Subtract(x.ConnectTime)).Seconds > 15000)?.ToList();
                            //removeData.ForEach(x => SocketHandler.userDic.Remove(x));
                        }

                        var str = string.Empty;
                        SocketHandler.concurrentQueue.TryDequeue(out str);
                        if (!string.IsNullOrEmpty(str))
                        {
                             //_capPublisher.Publish("design.services.webGis.publish", "1111");
                            if (str.Split('|').Length != 2) continue;
                            _capPublisher.PublishAsync(str.Split('|')[0], str.Split('|')[1]);
                            //告知客户端批注消息更新
                            //SocketHandler.userDic.Find(x=>x.ip)
                            //foreach (var item in SocketHandler.userDic)
                            //{
                            //    item.Content = "总数量:" + AllNum + ";成功数量:" + SuccessNum + ";失败数量:" + FailNum + "失败详情:" + JsonConvert.SerializeObject(FailId);
                            //    SocketHandler.SendMessage(item);
                            //}

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"勘察端处理客户端消息异常：{ex.Message}");
                    }
                   
                }
                
            });
        }
    }

    public class WebSoketInfo
    {
        public DateTime ConnectTime { get; set; }

        public WebSocket SocketInfo { get; set; }

        public string Ip { get; set; }

        public string ServerName { get; set; }

        public string Content { get; set; }
    }
}