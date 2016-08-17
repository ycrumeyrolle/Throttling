using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Throttling
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions for enabling core Throttling support.
    /// </summary>
    public static class ThrottleCoreServiceCollectionExtensions
    {
        /// <summary>
        /// Add services needed to support throttling to the given <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection to which Throttling services are added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddThrottlingCore(this IServiceCollection serviceCollection, Action<ThrottleOptions> configureOptions = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddOptions();
            serviceCollection.TryAddTransient<IThrottleStrategyProvider, DefaultThrottleStrategyProvider>();
            serviceCollection.TryAddTransient<ISystemClock, SystemClock>();
            serviceCollection.TryAddTransient<ConfigureOptions<ThrottleOptions>, ThrottleOptionsSetup>();
            serviceCollection.TryAddTransient<IThrottleService, ThrottleService>();

            // TODO : Put this files into Options.ThrottleHandlers & Options.ExclusionHandlers ?
            // Throttling handlers
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IRequirementHandler, AuthenticatedUserRateLimitHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IRequirementHandler, IPRateLimitHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IRequirementHandler, ApiKeyRateLimitHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IRequirementHandler, IPBandwidthHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IRequirementHandler, AuthenticatedUserBandwidthLimitHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IRequirementHandler, ApiKeyBandwidthHandler>());

            // Exclusion handlers
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IExclusionHandler, IPExclusionHandler>());

            if (configureOptions != null)
            {
                serviceCollection.Configure(configureOptions);
            }

            return serviceCollection;
        }
    }
}
