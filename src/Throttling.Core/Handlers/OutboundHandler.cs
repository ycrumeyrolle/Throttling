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

        public override async Task HandleAsync([NotNull] ThrottlingContext context, [NotNull] TRequirement requirement)
        {
            var key = GetKey(context.HttpContext, requirement);
            if (key == null)
            {
                return;
            }

            key = typeof(TRequirement) + key;
            var rate = await _store.GetRemainingRateAsync(key, requirement);
            if (rate.LimitReached)
            {
                context.TooManyRequest(requirement, rate.Reset);
            }
            else
            {
                context.Succeed(requirement);
            }

            context.HttpContext.Response.EnableCounting();
        }

        public override async Task PostHandleAsync([NotNull]ThrottlingContext context, [NotNull]TRequirement requirement)
        {
            var key = GetKey(context.HttpContext, requirement);
            if (key == null)
            {
                return;
            }
            
            key = typeof(TRequirement) + key;
            var decrementValue = GetDecrementValue(context.HttpContext, requirement);
            await _store.DecrementRemainingRateAsync(key, requirement, decrementValue, reachLimitAtZero: true);
        }

        public abstract string GetKey([NotNull] HttpContext httpContext, [NotNull] TRequirement requirement);

        public abstract long GetDecrementValue([NotNull] HttpContext httpContext, [NotNull] TRequirement requirement);
    }
}