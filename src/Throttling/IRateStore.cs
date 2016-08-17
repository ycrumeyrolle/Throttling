using System.Threading.Tasks;

namespace Throttling
{
    public interface IRateStore
    {
        Task<RemainingRate> DecrementRemainingRateAsync(string key, ThrottleRequirement requirement, long decrementValue, bool reachLimitAtZero = false);

        Task<RemainingRate> GetRemainingRateAsync(string key, ThrottleRequirement requirement);
    }

    public interface IRemainingRateStore
    {
        Task<RemainingRate> IncrementAsync(RemainingRateKey key, ThrottleRequirement requirement, long incrementValue = 1, bool reachLimitAtMax = false);

        Task<RemainingRate> GetAsync(string key, ThrottleRequirement requirement);
    }
}