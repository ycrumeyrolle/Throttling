using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class BandwidthHandler<TRequirement> : OutboundHandler<TRequirement> where TRequirement : ThrottlingRequirement
    {
        public BandwidthHandler(IRateStore store)
            : base(store)
        {
        }

        public override long GetDecrementValue([NotNull] ThrottlingContext throttlingContext, [NotNull]TRequirement requirement)
        {
            return throttlingContext.ContentLengthTracker.ContentLength;
        }
    }
}