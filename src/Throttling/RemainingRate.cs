using System;

namespace Throttling
{
    public class RemainingRate
    {
        private long _remainingCalls;
        private readonly bool _reachLimitAtZero;

        public RemainingRate(bool reachLimitAtZero)
        {
            _reachLimitAtZero = reachLimitAtZero;
        }

        public DateTimeOffset Reset { get; set; }

        public long RemainingCalls
        {
            get
            {
                return _remainingCalls;
            }

            set
            {
                if (value < 0 || (_reachLimitAtZero && value == 0))
                {
                    LimitReached = true;
                    _remainingCalls = 0;
                }
                else
                {
                    _remainingCalls = value;
                }
            }
        }

        public bool LimitReached { get; private set; }
    }
}