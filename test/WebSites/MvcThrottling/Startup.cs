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
            services.AddMvc();
            services.AddMvcThrottling();

            services.ConfigureThrottling(options =>
            {
                options.AddPolicy("10 requests per hour, sliding reset", builder =>
                {
                    builder
                        .LimitIPRate(10, TimeSpan.FromHours(1), true);
                });
                options.AddPolicy("10 requests per hour, fixed reset", builder =>
                {
                    builder
                        .LimitIPRate(10, TimeSpan.FromHours(1));
                });
                options.AddPolicy("160 bytes per hour by API key", builder =>
                {
                    builder.LimitClientBandwidthByRoute("{apikey}/{*any}", "apikey", 160, TimeSpan.FromHours(1));
                });
                options.AddPolicy("160 bytes per hour by IP", builder =>
                {
                    builder.LimitIPBandwidth(160, TimeSpan.FromHours(1));
                });
            });

            services.ConfigureMvc(options =>
            {                
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<IPEnforcerMiddleware>();

            app.UseThrottling();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{apikey}/{controller}/{action}/{id?}");
            });
        }
    }
}
