using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ApiKeyRequirement : ThrottlingRequirement, IApiKeyProvider
    {
        private readonly IApiKeyProvider _apiKeyProvider;

        protected ApiKeyRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding, [NotNull] IApiKeyProvider apiKeyProvider)
            : base(maxValue, renewalPeriod, sliding)
        {
            _apiKeyProvider = apiKeyProvider;
        }

        public string GetApiKey(HttpContext context)
        {
            return _apiKeyProvider.GetApiKey(context);
        }
    }
    public abstract class ApiKeyBandwidthRequirement : ApiKeyRequirement
    {
        protected ApiKeyBandwidthRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding, IApiKeyProvider apiKeyProvider)
            : base(maxValue, renewalPeriod, sliding, apiKeyProvider)
        {
        }
    }
    public abstract class ApiKeyRateLimitRequirement : ApiKeyRequirement
    {
        protected ApiKeyRateLimitRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding, IApiKeyProvider apiKeyProvider)
            : base(maxValue, renewalPeriod, sliding, apiKeyProvider)
        {
        }
    }
}