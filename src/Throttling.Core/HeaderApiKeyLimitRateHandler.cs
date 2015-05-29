using Microsoft.Framework.Internal;

namespace Throttling
{
    public sealed class HeaderApiKeyLimitRateHandler : ClientLimitRateHandler<HeaderApiKeyLimitRateRequirement>
    {
        public HeaderApiKeyLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base (store, apiKeyProvider)
        {
        }
    }
}