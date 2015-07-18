using System;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;

namespace Throttling.Redis
{
    public static class RedisThrottleOptionsExtensions
    {
        public static IServiceCollection AddRedisThrottling([NotNull]this IServiceCollection services)
        {
            services.AddSingleton<IRateStore, RedisRateStore>();
            return services;
        }

        public static IServiceCollection AddRedisThrottling([NotNull]this IServiceCollection services, [NotNull] Action<RedisThrottleOptions> configureOptions, string optionsName = "")
        {
            if (configureOptions != null)
            {
                services.ConfigureRedisThrottling(configureOptions, optionsName);
            }
            
            services.AddSingleton<IRateStore, RedisRateStore>();
            return services;
        }

        public static IServiceCollection AddRedisThrottling([NotNull]this IServiceCollection services, [NotNull] IConfiguration configuration, string optionsName = "")
        {
            if (configuration != null)
            {
                services.ConfigureRedisThrottling(configuration, optionsName);
            }

            services.AddSingleton<IRateStore, RedisRateStore>();
            return services;
        }

        public static IServiceCollection ConfigureRedisThrottling([NotNull] this IServiceCollection services, [NotNull] Action<RedisThrottleOptions> configureOptions, string optionsName = "")
        {
            return services.Configure(configureOptions, optionsName);
        }

        public static IServiceCollection ConfigureRedisThrottling([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration, string optionsName = "")
        {
            return services.Configure<RedisThrottleOptions>(configuration, optionsName);
        }
    }
}
