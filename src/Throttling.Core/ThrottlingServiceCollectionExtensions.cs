using System;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;
using Throttling;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions for enabling Throttling support.
    /// </summary>
    public static class ThrottlingServiceCollectionExtensions
    {
        /// <summary>
        /// Can be used to configure services in the <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection which needs to be configured.</param>
        /// <param name="configure">A delegate which is run to configure the services.</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureThrottling(
            [NotNull] this IServiceCollection serviceCollection,
            [NotNull] Action<ThrottlingOptions> configure)
        {
            return serviceCollection.Configure(configure);
        }

        /// <summary>
        /// Add services needed to support throttling to the given <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection to which Throttling services are added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddThrottling(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions();
            serviceCollection.AddTransient<IThrottlingPolicyProvider, DefaultThrottlingPolicyProvider>();
            serviceCollection.AddTransient<IRateStore, CacheRateStore>();
            serviceCollection.AddTransient<ISystemClock, SystemClock>();
            serviceCollection.AddTransient<IConfigureOptions<ThrottlingOptions>, ThrottlingOptionsSetup>();
            serviceCollection.AddTransient<IThrottlingService, ThrottlingService>();
            serviceCollection.AddTransient<IThrottlingRouter, ThrottlingRouteCollection>();

            return serviceCollection;
        }
    }
}