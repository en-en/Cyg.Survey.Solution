using Coldairarrow.DotNettySocket;
using Cyg.Extensions.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service.Impl
{
    public class SocketService : BaseService, ISocketService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private IWebSocketServer _socket = null;
        private readonly ConcurrentDictionary<string, List<string>> _connections = new ConcurrentDictionary<string, List<string>>();
        private static object locker = new object();

        public SocketService(ILogger<SocketService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        #region 启动服务
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public async Task<IWebSocketServer> StartAsync()
        {
            var port = _configuration.GetValue<int>("WebSocketPort");
            _socket = await SocketBuilderFactory.GetWebSocketServerBuilder(port)
                .OnConnectionClose((server, connection) =>
                {
                    _logger.LogInformation($"连接关闭,连接名[{connection.ConnectionName}],当前连接数:{server.GetConnectionCount()}");
                    RemoveConnection(connection.ConnectionId);
                })
                .OnException(ex =>
                {
                    _logger.LogError(ex, $"服务端异常:{ex.Message}");
                })
                .OnNewConnection((server, connection) =>
                {
                    connection.ConnectionName = $"{connection.ClientAddress.ToString()}";
                    _logger.LogInformation($"新的连接:{connection.ConnectionName},当前连接数:{server.GetConnectionCount()}");
                })
                .OnRecieve((server, connection, msg) =>
                {
                    if (msg.HasVal() && msg.StartsWith("InformIdentity_"))
                    {
                        var userId = msg.Replace("InformIdentity_", string.Empty);
                        if (userId.HasVal())
                        {
                            AddConnection(userId, connection.ConnectionId);
                            connection.Send("InformIdentity_ok");
                        }
                    }
                })
                .OnSend((server, connection, msg) =>
                {
                    _logger.LogInformation($"向连接名[{connection.ConnectionName}]发送数据:{msg}");
                })
                .OnServerStarted(server =>
                {
                    _logger.LogInformation($"服务启动");
                }).BuildAsync();
            return _socket;
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="message"></param>
        public void SendMessageByUsers(List<string> userIds, string message)
        {
            if (_socket != null && userIds.HasVal() && message.HasVal())
            {
                foreach (var uid in userIds)
                {
                    var connectionIds = GetConnections(uid);
                    if (connectionIds.HasVal())
                    {
                        foreach (var item in connectionIds)
                        {
                            var connection = _socket.GetConnectionById(item);
                            connection?.Send(message);
                        }
                    }
                }
            }
        }
        #endregion


        #region 群发消息
        public void SendMessage(string message)
        {
            lock (locker)
            {
                foreach (var soketInfo in _connections)
                {
                    foreach (var id in soketInfo.Value)
                    {
                        var connection = _socket.GetConnectionById(id);
                        connection?.Send(message);
                    }
                }
            }
        }
        #endregion

        #region 添加连接
        /// <summary>
        /// 添加连接
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connectionId"></param>
        private void AddConnection(string userId, string connectionId)
        {
            lock (locker)
            {
                if (_connections.ContainsKey(userId))
                {
                    if (_connections.TryGetValue(userId, out List<string> connectionIds))
                    {
                        connectionIds.Add(connectionId);
                        _connections.AddOrUpdate(userId, connectionIds, (k, c) => connectionIds);
                    }
                }
                else
                {
                    _connections.TryAdd(userId, new List<string>() { connectionId });
                }
            }
        }
        #endregion


        #region 移除连接
        /// <summary>
        /// 移除连接
        /// </summary>
        /// <param name="connectionId"></param>
        private void RemoveConnection(string connectionId)
        {
            lock (locker)
            {
                foreach (var item in _connections)
                {
                    if (item.Value.HasVal() && item.Value.Contains(connectionId))
                    {
                        if (_connections.TryGetValue(item.Key, out List<string> connectionIds))
                        {
                            connectionIds.Remove(connectionId);
                            if (connectionIds.Count == 0)
                            {
                                _connections.TryRemove(item.Key, out connectionIds);
                            }
                            else
                            {
                                _connections.AddOrUpdate(item.Key, connectionIds, (k, c) => connectionIds);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 获取连接
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<string> GetConnections(string userId)
        {
            lock (locker)
            {
                List<string> connectionIds = null;
                if (_connections.TryGetValue(userId, out connectionIds))
                {
                    return connectionIds;
                }
                else return null;
            }
        }
        #endregion
    }
}
