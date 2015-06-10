using System;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

namespace Throttling.Redis
{
    public static class RedisThrottleOptionsExtensions
    {
        public static IServiceCollection AddRedisThrottling(this IServiceCollection services, Action<RedisThrottleOptions> configureOptions = null)
        {
            return services.AddRedisThrottling(configuration: null, configureOptions: configureOptions);
        }

        public static IServiceCollection AddRedisThrottling(this IServiceCollection services, IConfiguration configuration, Action<RedisThrottleOptions> configureOptions)
        {
            services.AddSingleton<IRateStore, RedisRateStore>();

            if (configuration != null)
            {
                services.Configure<RedisThrottleOptions>(configuration);
            }

            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            return services;
        }
    }
}
