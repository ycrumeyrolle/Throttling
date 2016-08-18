using System;

namespace Throttling
{
    public abstract class BandwidthHandler<TRequirement> : OutboundRequirementHandler<TRequirement> where TRequirement : ThrottleRequirement
    {
        public override long GetIncrementValue(ThrottleContext throttleContext, TRequirement requirement)
        {
            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            return throttleContext.ContentLengthTracker.ContentLength;
        }
    }
}