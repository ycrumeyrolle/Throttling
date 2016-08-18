using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Throttling.Redis
{
    public class RedisThrottleCounterStore : IThrottleCounterStore
    {
        private ConnectionMultiplexer _connection;
        private IDatabase _database;

        private readonly ThrottleRedisOptions _options;
        private readonly string _instance;
        private readonly ISystemClock _clock;

        public RedisThrottleCounterStore(IOptions<ThrottleRedisOptions> options, ISystemClock clock)
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

        private void Connect()
        {
            if (_connection == null)
            {
                _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                _database = _connection.GetDatabase();
            }
        }

        public async Task<ThrottleCounter> GetAsync(string key, ThrottleRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            Connect();

            ThrottleCounter counter;
            var entry = await _database.StringGetWithExpiryAsync(key);
            if (entry.Value.IsNull)
            {
                counter = new ThrottleCounter(_clock.UtcNow.Add(requirement.RenewalPeriod));
            }
            else
            {
                long value = (long)entry.Value;
                bool limitReached = value >= requirement.MaxValue;
                counter = new ThrottleCounter(_clock.UtcNow.Add(entry.Expiry.Value), value, limitReached);
            }
                        
            return counter;
        }

        public async Task<ThrottleCounter> IncrementAsync(string key, ThrottleRequirement requirement, long incrementValue = 1, bool reachLimitAtMax = false)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            Connect();

            bool limitReached = false;
            var value = await _database.StringIncrementAsync(key, incrementValue);
            bool justCreated = value == incrementValue;
            DateTimeOffset reset;
            if (justCreated || requirement.Sliding)
            {
                await _database.KeyExpireAsync(key, requirement.RenewalPeriod, CommandFlags.FireAndForget);
                reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
            }
            else
            {
                var remainingTime = await _database.KeyTimeToLiveAsync(key);
                reset =_clock.UtcNow.Add(remainingTime.Value);
            }

            if (value > requirement.MaxValue || (reachLimitAtMax && value == requirement.MaxValue))
            {
                limitReached = true;
            }

            return new ThrottleCounter(reset, value, limitReached);
        }
    }

    //public class RedisRateStore : IRateStore
    //{
    //    private ConnectionMultiplexer _connection;
    //    private IDatabase _database;

    //    private readonly ThrottleRedisOptions _options;
    //    private readonly string _instance;
    //    private readonly ISystemClock _clock;

    //    public RedisRateStore(IOptions<ThrottleRedisOptions> options, ISystemClock clock)
    //    {
    //        if (options == null)
    //        {
    //            throw new ArgumentNullException(nameof(options));
    //        }

    //        if (clock == null)
    //        {
    //            throw new ArgumentNullException(nameof(clock));
    //        }

    //        _options = options.Value;
    //        _clock = clock;
    //        _instance = _options.InstanceName ?? string.Empty;
    //    }

    //    public void Connect()
    //    {
    //        if (_connection == null)
    //        {
    //            _connection = ConnectionMultiplexer.Connect(_options.Configuration);
    //            _database = _connection.GetDatabase();
    //        }
    //    }

    //    public async Task<RemainingRate> DecrementRemainingRateAsync(string key, ThrottleRequirement requirement, long decrementValue, bool reachLimitAtZero = false)
    //    {
    //        if (key == null)
    //        {
    //            throw new ArgumentNullException(nameof(key));
    //        }

    //        if (requirement == null)
    //        {
    //            throw new ArgumentNullException(nameof(requirement));
    //        }

    //        Connect();

    //        var value = await _database.StringIncrementAsync(key, decrementValue);
    //        bool justCreated = value == decrementValue;
    //        if (justCreated || requirement.Sliding)
    //        {
    //            await _database.KeyExpireAsync(key, requirement.RenewalPeriod, CommandFlags.FireAndForget);
    //        }
    //        else
    //        {
    //            var remainingTime = await _database.KeyTimeToLiveAsync(key);
    //            rate.Reset = _clock.UtcNow.Add(remainingTime.Value);
    //        }

    //        rate.RemainingCalls = requirement.MaxValue - value;

    //        return rate;
    //    }

    //    public async Task<RemainingRate> GetRemainingRateAsync(string key, ThrottleRequirement requirement)
    //    {
    //        if (key == null)
    //        {
    //            throw new ArgumentNullException(nameof(key));
    //        }

    //        if (requirement == null)
    //        {
    //            throw new ArgumentNullException(nameof(requirement));
    //        }

    //        Connect();

    //        RemainingRate rate = new RemainingRate(true);
    //        var keyWithExpiry = await _database.StringGetWithExpiryAsync(key);
    //        long count = (long)keyWithExpiry.Value;
    //        if (requirement.Sliding)
    //        {
    //            rate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
    //        }
    //        else
    //        {
    //            var remainingTime = keyWithExpiry.Expiry ?? requirement.RenewalPeriod;  
    //            rate.Reset = _clock.UtcNow.Add(remainingTime);
    //        }

    //        rate.RemainingCalls = requirement.MaxValue - count;

    //        return rate;
    //    }
    //}    
}
