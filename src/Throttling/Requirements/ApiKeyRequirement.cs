using System;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public abstract class ApiKeyRequirement : ThrottleRequirement, IApiKeyProvider
    {
        private readonly IApiKeyProvider _apiKeyProvider;

        protected ApiKeyRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding, IApiKeyProvider apiKeyProvider)
            : base(maxValue, renewalPeriod, sliding)
        {
            if (apiKeyProvider == null)
            {
                throw new ArgumentNullException(nameof(apiKeyProvider));
            }

            _apiKeyProvider = apiKeyProvider;
        }

        public string GetApiKey(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return _apiKeyProvider.GetApiKey(context);
        }
    }
}