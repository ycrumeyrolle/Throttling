using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ClientKeyProvider : IClientKeyProvider
    {
        public string GetKey([NotNull] HttpContext context)
        {
            return OnGetKey(context);
        }

        public Func<HttpContext, string> OnGetKey { get; set; } = ctx => "(none)";
    }
}