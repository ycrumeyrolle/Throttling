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
                        .LimitAuthenticatedUserRate(10, TimeSpan.FromHours(1), true)
                        .LimitIPRate(10, TimeSpan.FromDays(1));
                });
                options.AddPolicy("10 requests per hour, fixed reset", builder =>
                {
                    builder
                        .LimitAuthenticatedUserRate(10, TimeSpan.FromHours(1))
                        .LimitIPRate(10, TimeSpan.FromDays(1));
                });
                options.AddPolicy("Bandwidth", builder =>
                {
                    builder
                        .LimitIPBandwidth(10, TimeSpan.FromDays(1));
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
                    template: "{controller}/{action}/{id?}");
            });
        }
    }
}
