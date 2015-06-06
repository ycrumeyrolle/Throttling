using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IRateStore
    {
        Task<RemainingRate> DecrementRemainingRateAsync([NotNull] string key, [NotNull] ThrottlingRequirement requirement, long decrementValue, bool reachLimitAtZero = false);

        Task<RemainingRate> GetRemainingRateAsync([NotNull] string key, [NotNull] ThrottlingRequirement requirement);
    }
}