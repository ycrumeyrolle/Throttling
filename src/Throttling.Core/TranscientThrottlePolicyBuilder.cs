using Microsoft.Framework.Internal;

namespace Throttling
{
    internal class TranscientThrottlePolicyBuilder : IThrottlePolicyBuilder
    {
        private readonly ThrottlePolicy _policy;
        
        public TranscientThrottlePolicyBuilder(ThrottlePolicy policy)
        {
            _policy = policy;
        }

        public ThrottlePolicy Build([NotNull] ThrottleOptions options)
        {
            return _policy;
        }
    }
}