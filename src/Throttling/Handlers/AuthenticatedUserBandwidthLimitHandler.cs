using System;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public class AuthenticatedUserBandwidthLimitHandler : BandwidthHandler<AuthenticatedUserBandwidthRequirement>
    {
        public AuthenticatedUserBandwidthLimitHandler(IRateStore store)
            : base(store)
        {
        }
        
        public override string GetKey(HttpContext httpContext, AuthenticatedUserBandwidthRequirement requirement)
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