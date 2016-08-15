using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.Primitives;

namespace Throttling
{
    public class HeaderApiKeyProvider : IApiKeyProvider
    {
        private readonly string _parameter;

        public HeaderApiKeyProvider(string parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            _parameter = parameter;
        }

        public string GetApiKey(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            StringValues apiKeys;
            if (httpContext.Request.Headers.TryGetValue(_parameter, out apiKeys) && apiKeys.Count > 0)
            {
                return apiKeys[0];
            }

            return null;
        }
    }
}