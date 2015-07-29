using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class BandwidthHandler<TRequirement> : OutboundRequirementHandler<TRequirement> where TRequirement : ThrottleRequirement
    {
        public BandwidthHandler(IRateStore store)
            : base(store)
        {
        }

        public override long GetDecrementValue([NotNull] ThrottleContext throttleContext, [NotNull]TRequirement requirement)
        {
            return throttleContext.ContentLengthTracker.ContentLength;
        }
    }
}