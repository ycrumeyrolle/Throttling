using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class IPBandwidthHandler : BandwidthHandler<IPBandwidthRequirement>
    {
        public IPBandwidthHandler(IRateStore store)
            : base(store)
        {
        }
        
        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] IPBandwidthRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }
}