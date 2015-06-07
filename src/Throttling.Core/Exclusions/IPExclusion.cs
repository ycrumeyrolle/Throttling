using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;
using Microsoft.Framework.Internal;
using Throttling.IPRanges;

namespace Throttling
{
    public class IPExclusionHandler : ExclusionHandler<IPExclusion>
    {
        public override Task HandleAsync([NotNull]ThrottleContext throttleContext, IPExclusion exclusion)
        {
            IHttpConnectionFeature connection = throttleContext.HttpContext.GetFeature<IHttpConnectionFeature>();
            if (exclusion.Whitelist.Contains(connection.RemoteIpAddress))
            {
                throttleContext.Abort(exclusion);
            }

            return Constants.CompletedTask;
        }
    }

    public class IPExclusion : IThrottleExclusion
    {
        public IPExclusion([NotNull] params string[] ranges)
        {
            Whitelist = new IPWhitelist(ranges);
        }

        public IPWhitelist Whitelist { get; }
    }
}
