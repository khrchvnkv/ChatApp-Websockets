using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebsocketServer.Services
{
    public class WebsocketServerConnectionService
    {
        private readonly ConcurrentDictionary<string, WebSocket>
            _sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> Sockets => _sockets;

        public string AddSocket(WebSocket socket)
        {
            var connID = Guid.NewGuid().ToString();
            Console.WriteLine(_sockets.TryAdd(connID, socket)
                ? $"Connection added: {connID}"
                : $"Connection adding failed: {connID}");

            return connID;
        }
    }
}