using System;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Builder;

namespace Throttling
{
    public class LimitsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateStore _store;

        public LimitsMiddleware(RequestDelegate next, IRateStore store)
        {
            _next = next;
            _store = store;
        }
    }
}