using Microsoft.Framework.Internal;

namespace Throttling
{
    public sealed class QueryStringApiKeyLimitRateHandler : ClientLimitRateHandler<QueryStringApiKeyLimitRateRequirement>
    {
        public QueryStringApiKeyLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base(store, apiKeyProvider)
        {
        }
    }
}