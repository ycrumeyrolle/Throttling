using System;
using Microsoft.AspNet.Http;

namespace Throttling
{
    public class IPBandwidthHandler : BandwidthHandler<IPBandwidthRequirement>
    {
        public IPBandwidthHandler(IRateStore store)
            : base(store)
        {
        }
        
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