using Microsoft.Framework.Internal;

namespace Throttling
{
    public sealed class FormApiKeyLimitRateHandler : ClientLimitRateHandler<FormApiKeyLimitRateRequirement>
    {
        public FormApiKeyLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base (store, apiKeyProvider)
        {
        }
    }
}