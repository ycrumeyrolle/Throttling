using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;

namespace Throttling
{
    public class IPExclusionHandler : ExclusionHandler<IPExclusion>
    {
        public override Task HandleAsync(ThrottleContext throttleContext, IPExclusion exclusion)
        {
            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            if (exclusion == null)
            {
                throw new ArgumentNullException(nameof(exclusion));
            }

            IHttpConnectionFeature connection = throttleContext.HttpContext.Features.Get<IHttpConnectionFeature>();
            if (exclusion.Whitelist.Contains(connection.RemoteIpAddress))
            {
                throttleContext.Abort(exclusion);
            }

            return Constants.CompletedTask;
        }
    }
}