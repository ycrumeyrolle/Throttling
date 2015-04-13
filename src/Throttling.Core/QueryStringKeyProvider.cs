using System;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.WebUtilities;

namespace Throttling
{
    public class QueryStringKeyProvider : IClientKeyProvider
    {
        private readonly string _parameter;

        public QueryStringKeyProvider([NotNull] string parameter)
        {
            _parameter = parameter;
        }

        public string GetKey([NotNull] HttpContext context)
        {
            var keys = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
            string[] apiKeys;
            if (keys != null && keys.TryGetValue(_parameter, out apiKeys))
            {
                if (apiKeys.Length == 0)
                {
                    throw new InvalidOperationException("No API key found in the query string");
                }

                if (apiKeys.Length != 1)
                {
                    throw new InvalidOperationException("Multiples API key found in the query string");
                }

                return apiKeys[0];
            }

            throw new InvalidOperationException("No API key found in the query string");
        }
    }

}