using Microsoft.Framework.Internal;

namespace Throttling
{
    public sealed class RouteApiKeyLimitRateHandler : ClientLimitRateHandler<RouteApiKeyLimitRateRequirement>
    {
        public RouteApiKeyLimitRateHandler(IRateStore store,  IApiKeyProvider apiKeyProvider)
            : base (store, apiKeyProvider)
        {
        }
    }
}