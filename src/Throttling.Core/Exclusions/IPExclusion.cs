using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;
using Microsoft.Framework.Internal;
using Throttling.IPRanges;

namespace Throttling
{
    public class IPExclusionHandler : ExclusionHandler<IPExclusion>
    {
        public override Task HandleAsync([NotNull]ThrottlingContext throttlingContext, IPExclusion exclusion)
        {
            IHttpConnectionFeature connection = throttlingContext.HttpContext.GetFeature<IHttpConnectionFeature>();
            if (exclusion.Whitelist.Contains(connection.RemoteIpAddress))
            {
                throttlingContext.Abort(exclusion);
            }

            return Constants.CompletedTask;
        }
    }

    public class IPExclusion : IThrottlingExclusion
    {
        public IPExclusion([NotNull] params string[] ranges)
        {
            Whitelist = new IPWhitelist(ranges);
        }

        public IPWhitelist Whitelist
        {
            get;
        }
    }
}
