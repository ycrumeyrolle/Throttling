using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.DependencyInjection;
using Throttling;
using Throttling.Tests.Common;

namespace SimpleThrottling
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.AddThrottling();

            services.AddInstance(new RouteApiKeyProvider("{apikey}/{*remaining}", "apikey"));
            services.ConfigureThrottling(options =>
            {
                //System.Threading.Thread.Sleep(10000);
                options.AddPolicy("10 requests per hour, sliding reset", builder =>
                {
                    builder
                        .LimitAuthenticatedUserRate(10, TimeSpan.FromHours(1), true)
                        .LimitIPRate(10, TimeSpan.FromDays(1));
                });
                options.AddPolicy("10 requests per hour, fixed reset", builder =>
                {
                    builder
                        .LimitAuthenticatedUserRate(10, TimeSpan.FromHours(1))
                        .LimitIPRate(10, TimeSpan.FromDays(1));
                });
                options.AddPolicy("10 requests per hour by API key", builder =>
                {
                    builder
                        .LimitClientBandwidthByRoute("{apikey}/{*any}", "apikey", 10, TimeSpan.FromDays(1), true);
                });
                options.AddPolicy("Limited bandwidth", builder =>
                {
                    builder.LimitIPBandwidth(160, TimeSpan.FromHours(1));
                });
                options.Routes.ApplyPolicy("{apikey}/test/action/{id?}", "10 requests per hour, fixed reset");
                options.Routes.ApplyPolicy("{apikey}/test/action2/{id?}", "10 requests per hour, fixed reset");
                options.Routes.ApplyPolicy("{apikey}/test/action3/{id?}", "Limited bandwidth");
                options.Routes.ApplyPolicy("{apikey}/test/action4/{id?}", "10 requests per hour by API key");
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<IPEnforcerMiddleware>();

            app.UseThrottling();

            app.Use(next =>
            {
                return context =>
                {
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{text: \"Hello!\"}");
                };
            });
        }
    }
}
