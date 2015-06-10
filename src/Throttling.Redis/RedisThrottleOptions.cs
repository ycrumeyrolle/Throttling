using Microsoft.Framework.OptionsModel;

namespace Throttling.Redis
{
    public class RedisThrottleOptions : IOptions<RedisThrottleOptions>
    {
        public string Configuration { get; set; }

        public string InstanceName { get; set; }

        RedisThrottleOptions IOptions<RedisThrottleOptions>.Options
        {
            get
            {
                return this;
            }
        }

        RedisThrottleOptions IOptions<RedisThrottleOptions>.GetNamedOptions(string name)
        {
            return this;
        }
    }
}
