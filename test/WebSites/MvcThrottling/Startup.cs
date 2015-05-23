using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Throttling;
using Throttling.Mvc;
using System.Net;
using Microsoft.AspNet.Http.Features;

namespace MvcThrottling
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMvcThrottling();

            services.ConfigureThrottling(options =>
            {
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
            });

            services.ConfigureMvc(options =>
            {
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<IPForcerMiddleware>();

            // loggerFactory.AddConsole((cat, level) => cat.StartsWith("Throttling"));
            app.UseThrottling();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });
        }
    }

    public class IPForcerMiddleware
    {
        private const int DefaultBufferSize = 0x1000;

        private readonly RequestDelegate _next;

        public IPForcerMiddleware(RequestDelegate next)
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
