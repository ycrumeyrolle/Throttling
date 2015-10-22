using System;
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

        public RedisRateStore(IOptions<ThrottleRedisOptions> options, ISystemClock clock)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            _options = options.Value;
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

        public async Task<RemainingRate> DecrementRemainingRateAsync(string key, ThrottleRequirement requirement, long decrementValue, bool reachLimitAtZero = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

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

        public async Task<RemainingRate> GetRemainingRateAsync(string key, ThrottleRequirement requirement)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

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
