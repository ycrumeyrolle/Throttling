using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class OutboundHandler<TRequirement> : ThrottleHandler<TRequirement> where TRequirement : ThrottleRequirement
    {
        private readonly IRateStore _store;

        public OutboundHandler([NotNull] IRateStore store)
        {
            _store = store;
        }

        public override async Task HandleAsync([NotNull] ThrottleContext throttleContext, [NotNull] TRequirement requirement)
        {
            var key = GetKey(throttleContext.HttpContext, requirement);
            if (key == null)
            {
                throttleContext.Skipped(requirement);
                return;
            }

            key = typeof(TRequirement) + key;
            var rate = await _store.GetRemainingRateAsync(key, requirement);
            if (rate.LimitReached)
            {
                throttleContext.TooManyRequest(requirement, rate.Reset);
            }
            else
            {
                throttleContext.Succeed(requirement);
            }

            throttleContext.HttpContext.Response.TrackContentLength(throttleContext);
        }

        public override async Task PostHandleAsync([NotNull] ThrottleContext throttleContext, [NotNull]TRequirement requirement)
        {
            var key = GetKey(throttleContext.HttpContext, requirement);
            if (key == null)
            {
                return;
            }
            
            key = typeof(TRequirement) + key;
            var decrementValue = GetDecrementValue(throttleContext, requirement);
            await _store.DecrementRemainingRateAsync(key, requirement, decrementValue, reachLimitAtZero: true);
        }

        public abstract string GetKey([NotNull] HttpContext httpContext, [NotNull] TRequirement requirement);

        public abstract long GetDecrementValue([NotNull] ThrottleContext throttleContext, [NotNull] TRequirement requirement);
    }
}