using System;
using Microsoft.Extensions.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

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