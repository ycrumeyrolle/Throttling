using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottlePolicyBuilder
    {
        ThrottlePolicy Build([NotNull] ThrottleOptions options);
    }
}