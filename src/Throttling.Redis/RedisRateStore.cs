using System.Threading.Tasks;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;
using StackExchange.Redis;

namespace Throttling.Redis
{
    public class RedisRateStore : IRateStore
    {
        private ConnectionMultiplexer _connection;
        private IDatabase _database;

        private readonly ThrottleRedisOptions _options;
        private readonly string _instance;
        private readonly ISystemClock _clock;

        public RedisRateStore([NotNull] IOptions<ThrottleRedisOptions> optionsAccessor, [NotNull] ISystemClock clock)
        {
            _options = optionsAccessor.Options;
            _clock = clock;
            _instance = _options.InstanceName ?? string.Empty;
        }

        public void Connect()
        {
            if (_connection == null)
            {
                _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                _database = _connection.GetDatabase();
            }
        }

        public async Task<RemainingRate> DecrementRemainingRateAsync([NotNull]string key, [NotNull]ThrottleRequirement requirement, long decrementValue, bool reachLimitAtZero = false)
        {
            Connect();

            // TODO : StringIncrementWithExpiryAsync
            RemainingRate rate = new RemainingRate(reachLimitAtZero);
            var count = await _database.StringIncrementAsync(key, decrementValue);
            bool justCreated = count == decrementValue;
            if (justCreated || requirement.Sliding)
            {
                rate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
                await _database.KeyExpireAsync(key, requirement.RenewalPeriod, CommandFlags.FireAndForget);
            }
            else
            {
                var remainingTime = await _database.KeyTimeToLiveAsync(key);
                rate.Reset = _clock.UtcNow.Add(remainingTime.Value);
            }

            rate.RemainingCalls = requirement.MaxValue - count;

            return rate;
        }

        public async Task<RemainingRate> GetRemainingRateAsync([NotNull]string key, [NotNull]ThrottleRequirement requirement)
        {
            Connect();

            RemainingRate rate = new RemainingRate(true);
            var keyWithExpiry = await _database.StringGetWithExpiryAsync(key);
            long count = (long)keyWithExpiry.Value;
            if (requirement.Sliding)
            {
                rate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
            }
            else
            {
                var remainingTime = keyWithExpiry.Expiry ?? requirement.RenewalPeriod;
                rate.Reset = _clock.UtcNow.Add(remainingTime);
            }

            rate.RemainingCalls = requirement.MaxValue - count;

            return rate;
        }
    }    
}
