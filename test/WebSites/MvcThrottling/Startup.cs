using System;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Throttling;
using Throttling.Tests.Common;

namespace MvcThrottling
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCaching()
                    .AddMvc()
                    .AddThrottling();

            services.ConfigureThrottling(options =>
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
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{apikey}/{controller}/{action}/{id?}");
            });
        }
    }
}
