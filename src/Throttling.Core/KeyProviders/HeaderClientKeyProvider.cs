using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class HeaderClientKeyProvider
    {
        private readonly string _parameter;

        public HeaderClientKeyProvider([NotNull] string parameter)
        {
            _parameter = parameter;
        }

        public string GetKey([NotNull] HttpContext context)
        {
            string[] apiKeys;
            if (context.Request.Headers.TryGetValue(_parameter, out apiKeys) && apiKeys.Length > 0)
            {
                return apiKeys[0];
            }

            return null;
        }
    }
}