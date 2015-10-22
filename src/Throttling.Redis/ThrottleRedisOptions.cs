using Microsoft.Framework.OptionsModel;

namespace Throttling.Redis
{
    public class ThrottleRedisOptions : IOptions<ThrottleRedisOptions>
    {
        public string Configuration { get; set; }

        public string InstanceName { get; set; }

        public ThrottleRedisOptions Value
        {
            get
            {
                return this;
            }
        }
    }
}
