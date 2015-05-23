using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class QueryStringClientKeyProvider : IClientKeyProvider
    {
        private readonly string _parameter;

        public QueryStringClientKeyProvider([NotNull] string parameter) 
        {
            _parameter = parameter;
        }

        public string GetKey([NotNull] HttpContext context)
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