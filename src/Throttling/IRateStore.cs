using System.Threading.Tasks;

namespace Throttling
{
    public interface IRateStore
    {
        Task<RemainingRate> DecrementRemainingRateAsync(string key, ThrottleRequirement requirement, long decrementValue, bool reachLimitAtZero = false);

        Task<RemainingRate> GetRemainingRateAsync(string key, ThrottleRequirement requirement);
    }
}