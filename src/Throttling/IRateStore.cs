using System.Threading.Tasks;

namespace Throttling
{
    public interface IThrottleCounterStore
    {
        Task<ThrottleCounter> IncrementAsync(string key, ThrottleRequirement requirement, long incrementValue = 1, bool reachLimitAtMax = false);

        Task<ThrottleCounter> GetAsync(string key, ThrottleRequirement requirement);
    }
}