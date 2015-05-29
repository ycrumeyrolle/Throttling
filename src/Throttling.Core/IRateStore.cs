using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IRateStore
    {
        Task<RemainingRate> DecrementRemainingRateAsync([NotNull] string key, [NotNull] LimitRateRequirement requirement, long decrementValue);
    }
}