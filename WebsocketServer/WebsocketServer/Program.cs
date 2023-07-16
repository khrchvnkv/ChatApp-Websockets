var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket connected");
    }
    else
    {
        await next();
    }
});

app.Run();