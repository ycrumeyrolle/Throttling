using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;
using Throttling;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions for enabling Throttling support.
    /// </summary>
    public static class ThrottleServiceCollectionExtensions
    {
        /// <summary>
        /// Can be used to configure services in the <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection which needs to be configured.</param>
        /// <param name="configure">A delegate which is run to configure the services.</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureThrottling([NotNull] this IServiceCollection serviceCollection, [NotNull] Action<ThrottleOptions> configure)
        {
            return serviceCollection.Configure(configure);
        }

        /// <summary>
        /// Add services needed to support throttling to the given <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection to which Throttling services are added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IThrottleServiceBuilder AddThrottling([NotNull] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions();
            serviceCollection.TryAddTransient<IThrottleStrategyProvider, DefaultThrottleStrategyProvider>();
            serviceCollection.TryAddTransient<ISystemClock, SystemClock>();
            serviceCollection.TryAddTransient<IConfigureOptions<ThrottleOptions>, ThrottleOptionsSetup>();
            serviceCollection.TryAddTransient<IThrottleService, ThrottleService>();
            serviceCollection.AddTransient<IRateStore, InMemoryRateStore>();

            // TODO : Put this files into Options.ThrottleHandlers & Options.ExclusionHandlers
            // Throttling handlers
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IThrottleHandler, AuthenticatedUserRateLimitHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IThrottleHandler, IPRateLimitHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IThrottleHandler, ApiKeyRateLimitHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IThrottleHandler, IPBandwidthHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IThrottleHandler, AuthenticatedUserBandwidthLimitHandler>());
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IThrottleHandler, ApiKeyBandwidthHandler>());
                                              
            // Exclusion handlers
            serviceCollection.TryAddEnumerable(ServiceDescriptor.Transient<IExclusionHandler, IPExclusionHandler>());

            return new ThrottleBuilder(serviceCollection);
        }

        private class ThrottleBuilder : IThrottleServiceBuilder
        {
            private readonly IServiceCollection _services;

            public ThrottleBuilder(IServiceCollection services)
            {
                _services = services;
            }

            public ServiceDescriptor this[int index]
            {
                get
                {
                    return _services[index];
                }

                set
                {
                    _services[index] = value;
                }
            }

            public int Count
            {
                get
                {
                    return _services.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return _services.IsReadOnly;
                }
            }

            public void Add(ServiceDescriptor item)
            {
                _services.Add(item);
            }

            public void Clear()
            {
                _services.Clear();
            }

            public bool Contains(ServiceDescriptor item)
            {
                return _services.Contains(item);
            }

            public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
            {
                _services.CopyTo(array, arrayIndex);
            }

            public IEnumerator<ServiceDescriptor> GetEnumerator()
            {
                return _services.GetEnumerator();
            }

            public int IndexOf(ServiceDescriptor item)
            {
                return _services.IndexOf(item);
            }

            public void Insert(int index, ServiceDescriptor item)
            {
                _services.Insert(index, item);
            }

            public bool Remove(ServiceDescriptor item)
            {
                return _services.Remove(item);
            }

            public void RemoveAt(int index)
            {
                _services.RemoveAt(index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_services).GetEnumerator();
            }
        }
    }
}