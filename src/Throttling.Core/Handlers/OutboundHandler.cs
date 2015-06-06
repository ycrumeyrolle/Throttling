using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class OutboundHandler<TRequirement> : ThrottlingHandler<TRequirement> where TRequirement : ThrottlingRequirement
    {
        private readonly IRateStore _store;

        public OutboundHandler([NotNull] IRateStore store)
        {
            _store = store;
        }

        public override async Task HandleAsync([NotNull] ThrottlingContext throttlingContext, [NotNull] TRequirement requirement)
        {
            var key = GetKey(throttlingContext.HttpContext, requirement);
            if (key == null)
            {
                return;
            }

            key = typeof(TRequirement) + key;
            var rate = await _store.GetRemainingRateAsync(key, requirement);
            if (rate.LimitReached)
            {
                throttlingContext.TooManyRequest(requirement, rate.Reset);
            }
            else
            {
                throttlingContext.Succeed(requirement);
            }

            throttlingContext.HttpContext.Response.TrackContentLength(throttlingContext.ContentLengthTracker);
        }

        public override async Task PostHandleAsync([NotNull] ThrottlingContext throttlingContext, [NotNull]TRequirement requirement)
        {
            var key = GetKey(throttlingContext.HttpContext, requirement);
            if (key == null)
            {
                return;
            }
            
            key = typeof(TRequirement) + key;
            var decrementValue = GetDecrementValue(throttlingContext, requirement);
            await _store.DecrementRemainingRateAsync(key, requirement, decrementValue, reachLimitAtZero: true);
        }

        public abstract string GetKey([NotNull] HttpContext httpContext, [NotNull] TRequirement requirement);

        public abstract long GetDecrementValue([NotNull] ThrottlingContext throttlingContext, [NotNull] TRequirement requirement);
    }
}