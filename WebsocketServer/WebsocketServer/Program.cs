using WebsocketServer.Middleware;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseWebSockets();
app.UseWebsocketServer();

app.Run();