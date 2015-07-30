using Microsoft.Framework.OptionsModel;

namespace Throttling.Redis
{
    public class ThrottleRedisOptions : IOptions<ThrottleRedisOptions>
    {
        public string Configuration { get; set; }

        public string InstanceName { get; set; }

        ThrottleRedisOptions IOptions<ThrottleRedisOptions>.Options
        {
            get
            {
                return this;
            }
        }

        ThrottleRedisOptions IOptions<ThrottleRedisOptions>.GetNamedOptions(string name)
        {
            return this;
        }
    }
}
