using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using WebsocketServer.Services;

namespace WebsocketServer.Middleware
{
    public class WebsocketServerMiddleware
    {
        private WebsocketServerConnectionService _websocketService;
        private readonly RequestDelegate _next;

        public WebsocketServerMiddleware(WebsocketServerConnectionService websocketService, RequestDelegate next)
        {
            _websocketService = websocketService;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket connected");

                var connId = _websocketService.AddSocket(webSocket);
                await SendConnIdAsync(webSocket, connId);

                await ReceiveMessage(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine("Message received");
                        var messageData = Encoding.UTF8.GetString(buffer);
                        Console.WriteLine($"Message Data : {messageData}");
                        await RouteJsonMessageAsync(messageData);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        var id = _websocketService.Sockets.FirstOrDefault(s => s.Value == webSocket).Key;
                        if (!string.IsNullOrEmpty(id) && 
                            _websocketService.Sockets.TryRemove(id, out var socket) &&
                            result.CloseStatus is not null)
                        {
                            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                                CancellationToken.None);
                        }
                        Console.WriteLine("Received close messageData");
                    }
                });
            }
            else
            {
                await _next(context);
            }
        }

        private async Task RouteJsonMessageAsync(string messageData)
        {
            var routeObj = JsonConvert.DeserializeObject<dynamic>(messageData);
            if (routeObj is null) return;

            var messageBytes = Encoding.UTF8.GetBytes(routeObj.Message.ToString());
            if (messageBytes is null) return;
            
            if (Guid.TryParse(routeObj.To.ToString(), out Guid guidOutput))
            {
                Console.WriteLine("Targeted");
                if (_websocketService.Sockets.TryGetValue(guidOutput.ToString(), out var socket))
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        await SendMessageToSocket(socket);
                        return;
                    }
                }

                Console.WriteLine("Invalid recipient id");
            }
            else
            {
                Console.WriteLine("Broadcast");
                foreach (var socket in _websocketService.Sockets)
                {
                    if (socket.Value.State == WebSocketState.Open)
                    {
                        await SendMessageToSocket(socket.Value);
                    }
                }
            }

            async Task SendMessageToSocket(WebSocket socket) =>
                await socket.SendAsync(messageBytes, WebSocketMessageType.Text, true,
                    CancellationToken.None);
        }

        private async Task SendConnIdAsync(WebSocket webSocket, string connID)
        {
            var buffer = Encoding.UTF8.GetBytes($"ConnID: {connID}");
            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        
        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None);
                handleMessage?.Invoke(result, buffer);
            }
        }
        
    }
}