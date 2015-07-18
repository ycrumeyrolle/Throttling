using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;

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
            if (context.GetFeature<IHttpConnectionFeature>() == null)
            {
                context.SetFeature<IHttpConnectionFeature>(new FallbackHttpConnectionFeature());
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
        }
    }
}
