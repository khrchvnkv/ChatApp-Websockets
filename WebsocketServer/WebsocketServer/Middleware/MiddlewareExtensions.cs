namespace WebsocketServer.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseWebsocketServer(this IApplicationBuilder applicationBuilder) => 
            applicationBuilder.UseMiddleware<WebsocketServerMiddleware>();
    }
}