using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class IPLimitRateHandler : RateLimitHandler<IPLimitRateRequirement>
    {
        public IPLimitRateHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext context, [NotNull] IPLimitRateRequirement requirement)
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
}