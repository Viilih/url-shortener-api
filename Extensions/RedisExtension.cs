using StackExchange.Redis;

namespace UrlShortner.Api.Extensions;

public static class RedisExtension
{
    public static IServiceCollection AddRedisConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisConnectionString");
        var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
        if (!multiplexer.IsConnected)
        {
            throw new Exception("Could not connect to Redis");
        }
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        return services;
    }
}