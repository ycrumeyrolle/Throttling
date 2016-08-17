using Microsoft.Extensions.Options;

namespace Throttling.Redis
{
    public class ThrottleRedisOptions : ThrottleOptions
    {
        public string Configuration { get; set; }

        public string InstanceName { get; set; }
    }
}
