using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ApiKeyBandwidthHandler : BandwidthHandler<ApiKeyBandwidthRequirement>
    {
        protected ApiKeyBandwidthHandler(IRateStore store)
            : base(store)
        {
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] ApiKeyBandwidthRequirement requirement)
        {
            return requirement.GetApiKey(httpContext);
        }
    }
}