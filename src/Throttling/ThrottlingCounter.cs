using System;

namespace Throttling
{
    public class ThrottleCounter
    {
        public ThrottleCounter(DateTimeOffset reset, long value = 0L, bool limitReached = false)
        {
            Reset = reset;
            Value = value;
            LimitReached = limitReached;
        }

        public bool LimitReached { get; }
        public DateTimeOffset Reset { get; }
        public long Value { get; }

        public long Remaining(ThrottleRequirement requirement)
        {
            return Value > requirement.MaxValue ? 0 : requirement.MaxValue - Value;
        }
    }
}