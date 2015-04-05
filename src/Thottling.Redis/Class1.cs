//using System;
//using System.Threading.Tasks;
//using Microsoft.Framework.Internal;
//using Microsoft.Framework.OptionsModel;
//using StackExchange.Redis;
//using Throttling;

//namespace Thottling.Redis
//{
//    public class RedisRateStore : IRateStore
//    {
//        private ConnectionMultiplexer _connection;
//        private IDatabase _store;

//        private readonly RedisThrottlingOptions _options;
//        private readonly string _instance;

//        public RedisRateStore([NotNull] IOptions<RedisThrottlingOptions> optionsAccessor)
//        {
//            _options = optionsAccessor.Options;
//            _instance = _options.InstanceName ?? string.Empty;
//        }

//        public void Connect()
//        {
//            if (_connection == null)
//            {
//                _connection = ConnectionMultiplexer.Connect(_options.Configuration);
//                _store = _connection.GetDatabase();
//            }
//        }

//        public async Task<RemainingRate> GetRemainingRateAsync([NotNull]string category, [NotNull]string key)
//        {
//            Connect();

//            var result = await _store.StringGetAsync(key);
//            if(result.HasValue)
//            {
//                return new RemainingRate
//                {
//                    Remaining = (long)result
//                };
//            }
//        }

//        public Task SetRemainingRateAsync([NotNull]string category, [NotNull]string key, [NotNull]RemainingRate remaining)
//        {
//            throw new NotImplementedException();
//        }

//        public Task IncrementAsync([NotNull]string category, [NotNull]string key, int value, DateTimeOffset expires)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class RedisThrottlingOptions
//    {
//        public string Configuration { get; set; }

//        public string InstanceName { get; set; }
//    }
//}
