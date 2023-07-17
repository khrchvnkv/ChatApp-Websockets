namespace WebsocketServer.Middleware
{
    public static class WebsocketServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebsocketServer(this IApplicationBuilder applicationBuilder) => 
            applicationBuilder.UseMiddleware<WebsocketServerMiddleware>();
    }
}