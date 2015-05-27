using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        public string GetApiKey([NotNull] HttpContext context)
        {
            return OnGetApiKey(context);
        }

        public Func<HttpContext, string> OnGetApiKey { get; set; } = ctx => "(none)";
    }
}