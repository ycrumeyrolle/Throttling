using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class AuthenticatedUserBandwidthLimitHandler : BandwidthHandler<AuthenticatedUserBandwidthRequirement>
    {
        public AuthenticatedUserBandwidthLimitHandler([NotNull] IRateStore store)
            : base(store)
        {
        }
        
        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] AuthenticatedUserBandwidthRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }
}