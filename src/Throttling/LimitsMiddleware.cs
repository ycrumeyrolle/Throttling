using System;
using Microsoft.Extensions.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Throttling
{
    public class LimitsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IThrottleCounterStore _store;

        public LimitsMiddleware(RequestDelegate next, IThrottleCounterStore store)
        {
            _next = next;
            _store = store;
        }
    }
}