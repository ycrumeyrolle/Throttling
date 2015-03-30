using System;
using Microsoft.AspNet.Http;

namespace Throttling
{
    public class ClientKeyProvider : IClientKeyProvider
    {
        public string GetKey(HttpContext context)
        {
            return OnGetKey(context);
        }

        public Func<HttpContext, string> OnGetKey { get; set; } = ctx => "(none)";
    }
}