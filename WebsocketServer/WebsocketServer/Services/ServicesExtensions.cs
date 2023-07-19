namespace WebsocketServer.Services
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddWebsocketService(this IServiceCollection serviceCollection) => 
            serviceCollection.AddSingleton<WebsocketServerConnectionService>();
    }
}