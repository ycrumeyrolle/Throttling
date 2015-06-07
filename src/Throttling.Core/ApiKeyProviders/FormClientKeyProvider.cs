using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class FormApiKeyProvider : IApiKeyProvider
    {
        private readonly string _parameter;

        public FormApiKeyProvider([NotNull] string parameter)
        {
            _parameter = parameter;
        }

        public string GetApiKey([NotNull] HttpContext httpContext)
        {
            var apiKeys = httpContext.Request.Form.GetValues(_parameter);

            if (apiKeys != null && apiKeys.Count > 0)
            {
                return apiKeys[0];
            }

            return null;
        }
    }
}