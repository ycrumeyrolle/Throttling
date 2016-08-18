using System;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public class ApiKeyBandwidthHandler : BandwidthHandler<ApiKeyBandwidthRequirement>
    {
        public override string GetKey(HttpContext httpContext, ApiKeyBandwidthRequirement requirement)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            
            return requirement.GetApiKey(httpContext);
        }
    }
}