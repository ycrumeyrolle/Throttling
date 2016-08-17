using Microsoft.AspNetCore.Http;
using System;

namespace Throttling
{
    public class FormApiKeyProvider : IApiKeyProvider
    {
        private readonly string _parameter;

        public FormApiKeyProvider(string parameter)
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

            var apiKeys = httpContext.Request.Form[_parameter];

            if (apiKeys.Count > 0)
            {
                return apiKeys[0];
            }

            return null;
        }
    }
}