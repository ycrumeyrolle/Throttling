using System;
using Microsoft.AspNet.Http;

namespace Throttling
{
    public class ApiKeyBandwidthHandler : BandwidthHandler<ApiKeyBandwidthRequirement>
    {
        public ApiKeyBandwidthHandler(IRateStore store)
            : base(store)
        {
        }

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