using System;

namespace Throttling
{
    internal class TranscientThrottlePolicyBuilder : IThrottlePolicyBuilder
    {
        private readonly ThrottlePolicy _policy;
        
        public TranscientThrottlePolicyBuilder(ThrottlePolicy policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _policy = policy;
        }

        public ThrottlePolicy Build(ThrottleOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return _policy;
        }
    }
}