using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Throttling;
using System.Net;
using Microsoft.Framework.Caching.Memory;
using Microsoft.AspNet.Http.Features;

namespace SimpleThrottling
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.AddThrottling();

            services.ConfigureThrottling(options =>
            {
                //System.Threading.Thread.Sleep(10000);
                options.ClientKeyProvider = new RouteClientKeyProvider("{api}", "api");
                options.AddPolicy("10 requests per hour, sliding reset", builder =>
                {
                    builder
                        .AddUserLimitRatePerHour(10, true)
                        .AddIPLimitRatePerDay(10);
                });
                options.AddPolicy("10 requests per hour, fixed reset", builder =>
                {
                    builder
                        .AddUserLimitRatePerHour(10)
                        .AddIPLimitRatePerDay(10);
                });
                options.Routes.ApplyStrategy("{api}/test/action/{id?}", "10 requests per hour, fixed reset");
                options.Routes.ApplyStrategy("{api}/test/action2/{id?}", "10 requests per hour, fixed reset");
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<IPEnforcerMiddleware>();

            // loggerFactory.AddConsole((cat, level) => cat.StartsWith("Throttling"));
            app.UseThrottling();

            app.Use(next =>
            {
                return async context =>
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{text: \"Hello!\"}");
                };
            });
        }
    }

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
    }

    public class FallbackHttpConnectionFeature : IHttpConnectionFeature
    {
        public IPAddress RemoteIpAddress { get; set; } = IPAddress.Parse("127.0.0.1");
        public IPAddress LocalIpAddress { get; set; }
        public int RemotePort { get; set; }
        public int LocalPort { get; set; }
        public bool IsLocal { get; set; }
    }
}
