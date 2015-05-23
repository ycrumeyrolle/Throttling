using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class FormClientKeyProvider : IClientKeyProvider
    {
        private readonly string _parameter;

        public FormClientKeyProvider([NotNull] string parameter)
        {
            _parameter = parameter;
        }

        public string GetKey([NotNull] HttpContext context)
        {
            var apiKeys = context.Request.Form.GetValues(_parameter);

            if (apiKeys != null && apiKeys.Count > 0)
            {
                return apiKeys[0];
            }

            return null;
        }
    }
}