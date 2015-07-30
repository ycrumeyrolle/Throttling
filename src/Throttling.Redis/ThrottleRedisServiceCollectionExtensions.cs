using System;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;

namespace Throttling.Redis
{
    public static class ThrottleRedisServiceCollectionExtensions
    {
        public static IServiceCollection AddThrottlingRedis([NotNull]this IServiceCollection services, Action<ThrottleRedisOptions> configureOptions = null, string optionsName = "")
        {
            if (configureOptions != null)
            {
                services.ConfigureThrottlingRedis(configureOptions, optionsName);
            }
            
            services.AddSingleton<IRateStore, RedisRateStore>();
            return services;
        }

        public static IServiceCollection AddThrottlingRedis([NotNull]this IServiceCollection services, [NotNull] IConfiguration configuration, string optionsName = "")
        {
            if (configuration != null)
            {
                services.ConfigureThrottlingRedis(configuration, optionsName);
            }

            services.AddSingleton<IRateStore, RedisRateStore>();
            return services;
        }

        public static IServiceCollection ConfigureThrottlingRedis([NotNull] this IServiceCollection services, [NotNull] Action<ThrottleRedisOptions> configureOptions, string optionsName = "")
        {
            return services.Configure(configureOptions, optionsName);
        }

        public static IServiceCollection ConfigureThrottlingRedis([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration, string optionsName = "")
        {
            return services.Configure<ThrottleRedisOptions>(configuration, optionsName);
        }
    }
}
