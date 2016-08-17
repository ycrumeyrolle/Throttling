using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Throttling;
using Throttling.Tests.Common;

namespace SimpleThrottling
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInMemoryThrottling(options =>
            {
                options.AddPolicy("10 requests per hour, sliding reset")
                         .LimitIPRate(10, TimeSpan.FromHours(1), true);
                options.AddPolicy("10 requests per hour, fixed reset")
                        .LimitIPRate(10, TimeSpan.FromHours(1));
                options.AddPolicy("160 bytes per hour by API key")
                    .LimitClientBandwidthByRoute("{apikey}/{*any}", "apikey", 160, TimeSpan.FromHours(1));
                options.AddPolicy("160 bytes per hour by IP")
                    .LimitIPBandwidth(160, TimeSpan.FromHours(1));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<IPEnforcerMiddleware>();

            app.UseThrottling(routes => 
            {
                routes.ApplyPolicy("{apikey}/test/RateLimit10PerHour/{id?}", "10 requests per hour, fixed reset");
                routes.ApplyPolicy("{apikey}/test/RateLimit10PerHour2/{id?}", "10 requests per hour, fixed reset");
                routes.ApplyPolicy("{apikey}/test/Quota160BPerHourByIP/{id?}", "160 bytes per hour by IP");
                routes.ApplyPolicy("{apikey}/test/Quota160BPerHourByApiKey/{id?}", "160 bytes per hour by API key");
            });

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
