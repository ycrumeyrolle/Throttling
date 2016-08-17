using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Throttling;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions for enabling Throttling support.
    /// </summary>
    public static class ThrottleServiceCollectionExtensions
    {
        ///// <summary>
        ///// Can be used to configure services in the <paramref name="serviceCollection"/>.
        ///// </summary>
        ///// <param name="serviceCollection">The service collection which needs to be configured.</param>
        ///// <param name="configure">A delegate which is run to configure the services.</param>
        ///// <returns></returns>
        //public static IServiceCollection ConfigureThrottling(this IServiceCollection serviceCollection, Action<ThrottleOptions> configure)
        //{
        //    if (serviceCollection == null)
        //    {
        //        throw new ArgumentNullException(nameof(serviceCollection));
        //    }

        //    return serviceCollection.Configure(configure);
        //}


        public static IServiceCollection AddInMemoryThrottling(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddThrottlingCore();
            serviceCollection.AddMemoryCache();
            serviceCollection.TryAddTransient<IRateStore, InMemoryRateStore>();

            return serviceCollection;
        }

        public static IServiceCollection AddInMemoryThrottling(this IServiceCollection serviceCollection, Action<ThrottleOptions> configureOptions = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddThrottlingCore(configureOptions);
            serviceCollection.AddMemoryCache();
            serviceCollection.TryAddTransient<IRateStore, InMemoryRateStore>();

            return serviceCollection;
        }

        //private class ThrottleBuilder : IThrottleServiceBuilder
        //{
        //    private readonly IServiceCollection _services;

        //    public ThrottleBuilder(IServiceCollection services)
        //    {
        //        _services = services;
        //    }

        //    public ServiceDescriptor this[int index]
        //    {
        //        get
        //        {
        //            return _services[index];
        //        }

        //        set
        //        {
        //            _services[index] = value;
        //        }
        //    }

        //    public int Count
        //    {
        //        get
        //        {
        //            return _services.Count;
        //        }
        //    }

        //    public bool IsReadOnly
        //    {
        //        get
        //        {
        //            return _services.IsReadOnly;
        //        }
        //    }

        //    public void Add(ServiceDescriptor item)
        //    {
        //        _services.Add(item);
        //    }

        //    public void Clear()
        //    {
        //        _services.Clear();
        //    }

        //    public bool Contains(ServiceDescriptor item)
        //    {
        //        return _services.Contains(item);
        //    }

        //    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        //    {
        //        _services.CopyTo(array, arrayIndex);
        //    }

        //    public IEnumerator<ServiceDescriptor> GetEnumerator()
        //    {
        //        return _services.GetEnumerator();
        //    }

        //    public int IndexOf(ServiceDescriptor item)
        //    {
        //        return _services.IndexOf(item);
        //    }

        //    public void Insert(int index, ServiceDescriptor item)
        //    {
        //        _services.Insert(index, item);
        //    }

        //    public bool Remove(ServiceDescriptor item)
        //    {
        //        return _services.Remove(item);
        //    }

        //    public void RemoveAt(int index)
        //    {
        //        _services.RemoveAt(index);
        //    }

        //    IEnumerator IEnumerable.GetEnumerator()
        //    {
        //        return ((IEnumerable)_services).GetEnumerator();
        //    }
        //}
    }


    public class ApiKeyRateLimitHandler : InboundRequirementHandler<ApiKeyRateLimitRequirement>
    {
        public ApiKeyRateLimitHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders(RemainingRate rate, ThrottleContext throttleContext, ApiKeyRateLimitRequirement requirement)
        {
            throttleContext.ResponseHeaders["X-RateLimit-ClientLimit"] = requirement.MaxValue.ToString(CultureInfo.InvariantCulture);
            throttleContext.ResponseHeaders["X-RateLimit-ClientRemaining"] = rate.RemainingCalls.ToString(CultureInfo.InvariantCulture);
        }

        public override string GetKey(HttpContext httpContext, ApiKeyRateLimitRequirement requirement)
        {
            return requirement.GetApiKey(httpContext);
        }
    }

    public class IPRateLimitHandler : InboundRequirementHandler<IPRateLimitRequirement>
    {
        public IPRateLimitHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders(RemainingRate rate, ThrottleContext throttleContext, IPRateLimitRequirement requirement)
        {
            throttleContext.ResponseHeaders["X-RateLimit-IPLimit"] = requirement.MaxValue.ToString();
            throttleContext.ResponseHeaders["X-RateLimit-IPRemaining"] = rate.RemainingCalls.ToString();
        }

        public override string GetKey(HttpContext httpContext, IPRateLimitRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }


    public class AuthenticatedUserRateLimitHandler : InboundRequirementHandler<AuthenticatedUserRateLimitRequirement>
    {
        public AuthenticatedUserRateLimitHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders(RemainingRate rate, ThrottleContext throttleContext, AuthenticatedUserRateLimitRequirement requirement)
        {
            throttleContext.ResponseHeaders["X-RateLimit-UserLimit"] = requirement.MaxValue.ToString(CultureInfo.InvariantCulture);
            throttleContext.ResponseHeaders["X-RateLimit-UserRemaining"] = rate.RemainingCalls.ToString(CultureInfo.InvariantCulture);
            throttleContext.ResponseHeaders["X -RateLimit-UserReset"] = rate.Reset.ToEpoch().ToString(CultureInfo.InvariantCulture);
        }

        public override string GetKey(HttpContext httpContext, AuthenticatedUserRateLimitRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }
}