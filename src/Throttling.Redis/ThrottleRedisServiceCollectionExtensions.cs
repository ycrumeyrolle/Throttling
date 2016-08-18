using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddThrottlingCore();

            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }

            services.AddSingleton<IThrottleCounterStore, RedisThrottleCounterStore>();
            return services;
        }

        //public static IServiceCollection AddThrottlingRedis(this IServiceCollection services, IConfiguration configuration)
        //{
        //    if (services == null)
        //    {
        //        throw new ArgumentNullException(nameof(services));
        //    }
            
        //    if (configuration != null)
        //    {
        //        services.ConfigureThrottlingRedis(configuration);
        //    }

        //    services.AddSingleton<IRateStore, RedisRateStore>();
        //    return services;
        //}

        public static IServiceCollection ConfigureThrottlingRedis(this IServiceCollection services, Action<ThrottleRedisOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.Configure(configureOptions);
        }

        //public static IServiceCollection ConfigureThrottlingRedis(this IServiceCollection services, IConfiguration configuration)
        //{
        //    if (services == null)
        //    {
        //        throw new ArgumentNullException(nameof(services));
        //    }

        //    return services.Configure<ThrottleRedisOptions>(configuration);
        //}
    }
}
