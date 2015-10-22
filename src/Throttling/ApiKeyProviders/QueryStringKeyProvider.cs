using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class QueryStringApiKeyProvider : IApiKeyProvider
    {
        private readonly string _parameter;

        public QueryStringApiKeyProvider(string parameter) 
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

            var keys = httpContext.Request.Query[_parameter];
            if (keys.Count > 0)
            {
                return keys[0];
            }

            return null;
        }
    }
}