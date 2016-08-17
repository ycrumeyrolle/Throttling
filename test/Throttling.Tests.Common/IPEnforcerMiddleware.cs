using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Throttling.Tests.Common
{
    public class IPEnforcerMiddleware
    {
        private const int DefaultBufferSize = 0x1000;

        private readonly RequestDelegate _next;

        public IPEnforcerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Features.Get<IHttpConnectionFeature>() == null)
            {
                context.Features.Set<IHttpConnectionFeature>(new FallbackHttpConnectionFeature());
            }

            await _next(context);
        }

        private class FallbackHttpConnectionFeature : IHttpConnectionFeature
        {
            public IPAddress RemoteIpAddress { get; set; } = IPAddress.Parse("127.0.0.1");
            public IPAddress LocalIpAddress { get; set; }
            public int RemotePort { get; set; }
            public int LocalPort { get; set; }
            public bool IsLocal { get; set; }

            public string ConnectionId { get; set; }
        }
    }
}
