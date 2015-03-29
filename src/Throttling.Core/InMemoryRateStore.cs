using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Throttling
{
    public class InMemoryRateStore : IRateStore
    {
        private readonly ConcurrentDictionary<string, RemainingRate> _store = new ConcurrentDictionary<string, RemainingRate>();
        private readonly ISystemClock _clock;
        
        public InMemoryRateStore(ISystemClock clock)
        {
            _clock = clock;
        }

        public Task<RemainingRate> GetRemainingRateAsync(string category, string key)
        {
            RemainingRate value;
            var computedKey = category + ":" + key;
            if (_store.TryGetValue(computedKey, out value))
            {
                if (_clock.UtcNow > value.Reset)
                {
                    _store.TryRemove(computedKey, out value);
                    value = null;
                }
            }
            
            return Task.FromResult(value);
        }

        public Task SetRemainingRateAsync(string category, string key, RemainingRate remaining)
        {
            _store[category + ":" + key] = remaining;
            return Constants.CompletedTask;
        }
    }
}