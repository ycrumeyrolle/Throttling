using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class BandwidthHandler<TRequirement> : OutboundHandler<TRequirement> where TRequirement : ThrottlingRequirement
    {
        public BandwidthHandler(IRateStore store)
            : base(store)
        {
        }

        public override long GetDecrementValue([NotNull]HttpContext httpContext, [NotNull]TRequirement requirement)
        {
            return httpContext.Response.Body.Length;
        }
    }
}