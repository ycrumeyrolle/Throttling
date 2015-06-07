using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ApiKeyRequirement : ThrottleRequirement, IApiKeyProvider
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
}