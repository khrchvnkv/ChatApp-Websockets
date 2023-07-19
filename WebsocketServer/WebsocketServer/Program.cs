using WebsocketServer.Middleware;
using WebsocketServer.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebsocketService();

var app = builder.Build();

app.UseWebSockets();
app.UseWebsocketServer();

app.Run();