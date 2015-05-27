using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ThrottlingHandler<TRequirement> : IThrottlingHandler
      where TRequirement : IThrottlingRequirement
    {
        public virtual async Task HandleAsync(ThrottlingContext context)
        {
            foreach (var requirement in context.Requirements.OfType<TRequirement>())
            {
                await HandleAsync(context, requirement);
            }
        }

        public abstract Task HandleAsync(ThrottlingContext context, TRequirement requirement);
    }

    public abstract class ThrottlingHandler<TRequirement, TResource> : IThrottlingHandler
        where TResource : class
        where TRequirement : IThrottlingRequirement
    {
        public virtual async Task HandleAsync(ThrottlingContext context)
        {
            var resource = context.RouteTemplate as TResource;
            // REVIEW: should we allow null resources?
            if (resource != null)
            {
                foreach (var requirement in context.Requirements.OfType<TRequirement>())
                {
                    await HandleAsync(context, requirement, resource);
                }
            }
        }

        public abstract Task HandleAsync(ThrottlingContext context, TRequirement requirement, TResource resource);
    }

    public class IPLimitRateHandler : RateLimitHandler<IPLimitRateRequirement>
    {
        public IPLimitRateHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull]  ThrottlingContext context, [NotNull] IPLimitRateRequirement requirement)
        {
            context.Headers.Set("IpRate", rate.RemainingCalls.ToString());
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] IPLimitRateRequirement requirement)
        {
            // TODO : What if behind reverse proxy? X-Forwarded-For ?
            IHttpConnectionFeature connection = httpContext.GetFeature<IHttpConnectionFeature>();

            return connection.RemoteIpAddress.ToString();
        }
    }

    public sealed class IPLimitRateRequirement : LimitRateRequirement
    {
        public IPLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
        : base(calls, renewalPeriod, sliding)
        {
        }
    }

    public class UserLimitRateHandler : RateLimitHandler<UserLimitRateRequirement>
    {
        private readonly IPLimitRateHandler _unauthenticatedHandler;

        public UserLimitRateHandler([NotNull] IRateStore store)
            : base(store)
        {
            _unauthenticatedHandler = new IPLimitRateHandler(store);
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull]  ThrottlingContext context, [NotNull] UserLimitRateRequirement requirement)
        {
            context.Headers.Set("X-RateLimit-UserLimit", requirement.Calls.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-UserRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-UserReset", rate.Reset.ToEpoch().ToString(CultureInfo.InvariantCulture));
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] UserLimitRateRequirement requirement)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return httpContext.User.Identity.Name;
            }

            return _unauthenticatedHandler?.GetKey(httpContext, requirement.UnauthenticatedRequirement);
        }
    }

    public sealed class UserLimitRateRequirement : LimitRateRequirement
    {
        public UserLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding, [NotNull] IPLimitRateRequirement unauthenticatedUserRequirement)
         : base(calls, renewalPeriod, sliding)
        {
            UnauthenticatedRequirement = unauthenticatedUserRequirement;
        }        

        public IPLimitRateRequirement UnauthenticatedRequirement { get; }
    }

    public abstract class ClientLimitRateHandler<TRequirement> : RateLimitHandler<TRequirement> where TRequirement : LimitRateRequirement
    {
        private readonly IApiKeyProvider _apiKeyProvider;

        protected ClientLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base(store)
        {
            _apiKeyProvider = apiKeyProvider;
        }

        public override void AddRateLimitHeaders([NotNull]RemainingRate rate, [NotNull]ThrottlingContext context, [NotNull]TRequirement requirement)
        {
            context.Headers.Set("X-RateLimit-ClientLimit", requirement.Calls.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-ClientRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
        }
        
        public override string GetKey(HttpContext httpContext, [NotNull] TRequirement requirement)
        {
            return _apiKeyProvider.GetApiKey(httpContext);
        }
    }

    public sealed class RouteApiKeyLimitRateHandler : ClientLimitRateHandler<RouteApiKeyLimitRateRequirement>
    {
        public RouteApiKeyLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base(store, apiKeyProvider)
        {
        }
    }


    public sealed class FormApiKeyLimitRateHandler : ClientLimitRateHandler<FormApiKeyLimitRateRequirement>
    {
        public FormApiKeyLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base(store, apiKeyProvider)
        {
        }
    }


    public sealed class HeaderApiKeyLimitRateHandler : ClientLimitRateHandler<HeaderApiKeyLimitRateRequirement>
    {
        public HeaderApiKeyLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base(store, apiKeyProvider)
        {
        }
    }


    public sealed class QueryStringApiKeyLimitRateHandler : ClientLimitRateHandler<QueryStringApiKeyLimitRateRequirement>
    {
        public QueryStringApiKeyLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base(store, apiKeyProvider)
        {
        }
    }

    public sealed class RouteApiKeyLimitRateRequirement : LimitRateRequirement
    {
        public RouteApiKeyLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
         : base(calls, renewalPeriod, sliding)
        {
        }
    }

    public sealed class FormApiKeyLimitRateRequirement : LimitRateRequirement
    {
        public FormApiKeyLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
          : base(calls, renewalPeriod, sliding)
        {
        }
    }

    public sealed class HeaderApiKeyLimitRateRequirement : LimitRateRequirement
    {
        public HeaderApiKeyLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
           : base(calls, renewalPeriod, sliding)
        {
        }
    }

    public sealed class QueryStringApiKeyLimitRateRequirement : LimitRateRequirement
    {
        public QueryStringApiKeyLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
            :base(calls, renewalPeriod, sliding)
        {
        }
    }

    public abstract class LimitRateRequirement : IThrottlingRequirement
    {
        protected LimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
        {
            Calls = calls;
            RenewalPeriod = renewalPeriod;
            Sliding = sliding;
        }

        public long Calls { get; }

        public TimeSpan RenewalPeriod { get; }

        public bool Sliding { get; }
    }

    public abstract class RateLimitHandler<TRequirement> : ThrottlingHandler<TRequirement> where TRequirement : LimitRateRequirement
    {
        private readonly IRateStore _store;

        public RateLimitHandler([NotNull]IRateStore store)
        {
            _store = store;
        }

        public override async Task HandleAsync([NotNull]ThrottlingContext context, [NotNull] TRequirement requirement)
        {
            var key = GetKey(context.HttpContext, requirement);

            if (key == null)
            {
                throw new InvalidOperationException("The current throttling requirement do not provide a key for the current context.");
            }

            key = typeof(TRequirement) + key;
            var rate = await _store.DecrementRemainingRateAsync(key, requirement, 1);
            if (rate.LimitReached)
            {
                context.TooManyRequest(requirement, rate.Reset);
            }

            AddRateLimitHeaders(rate, context, requirement);
        }

        public abstract void AddRateLimitHeaders([NotNull]RemainingRate rate, [NotNull] ThrottlingContext context, [NotNull] TRequirement requirement);

        public abstract string GetKey([NotNull]HttpContext httpContext, [NotNull] TRequirement requirement);
    }
}