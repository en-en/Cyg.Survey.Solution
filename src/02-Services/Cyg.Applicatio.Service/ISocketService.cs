using Coldairarrow.DotNettySocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cyg.Applicatio.Service
{
    public interface ISocketService : ISingletonDependency
    {
        Task<IWebSocketServer> StartAsync();
        void SendMessageByUsers(List<string> userIds, string message);

        void SendMessage( string message);
        
    }
}
