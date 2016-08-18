using System;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public class IPBandwidthHandler : BandwidthHandler<IPBandwidthRequirement>
    {
        public override string GetKey(HttpContext httpContext, IPBandwidthRequirement requirement)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            return requirement.GetKey(httpContext);
        }
    }
}