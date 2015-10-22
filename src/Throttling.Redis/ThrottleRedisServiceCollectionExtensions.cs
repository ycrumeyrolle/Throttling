using System;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;

namespace Throttling.Redis
{
    public static class ThrottleRedisServiceCollectionExtensions
    {
        public static IServiceCollection AddThrottlingRedis(this IServiceCollection services, Action<ThrottleRedisOptions> configureOptions = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions != null)
            {
                services.ConfigureThrottlingRedis(configureOptions);
            }
            
            services.AddSingleton<IRateStore, RedisRateStore>();
            return services;
        }

        public static IServiceCollection AddThrottlingRedis(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            if (configuration != null)
            {
                services.ConfigureThrottlingRedis(configuration);
            }

            services.AddSingleton<IRateStore, RedisRateStore>();
            return services;
        }

        public static IServiceCollection ConfigureThrottlingRedis(this IServiceCollection services, Action<ThrottleRedisOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.Configure(configureOptions);
        }

        public static IServiceCollection ConfigureThrottlingRedis(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.Configure<ThrottleRedisOptions>(configuration);
        }
    }
}
