using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class QueryStringApiKeyProvider : IApiKeyProvider
    {
        private readonly string _parameter;

        public QueryStringApiKeyProvider([NotNull] string parameter) 
        {
            _parameter = parameter;
        }

        public string GetApiKey([NotNull] HttpContext context)
        {
            var keys = context.Request.Query.GetValues(_parameter);
            if (keys != null && keys.Count > 0)
            {
                return keys[0];
            }

            return null;
        }
    }
}