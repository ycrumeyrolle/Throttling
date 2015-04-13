using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Throttling;
using Throttling.Mvc;

namespace ThrottlingSample
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IThrottlingAuthorizationFilter, ThrottlingAuthorizationFilter>();
            services.AddMvc();
            services.AddThrottling();

            services.ConfigureThrottling(options =>
            {
                options.AddPolicy("5 requests per 10 seconds, sliding reset", builder =>
                {
                    builder
                        .AddUserLimitRate(5, TimeSpan.FromSeconds(10), true)
                        .AddIPLimitRatePerDay(10);
                });
                options.AddPolicy("5 requests per 10 seconds, fixed reset", builder =>
                {
                    builder
                        .AddUserLimitRate(5, TimeSpan.FromSeconds(10))
                        .AddIPLimitRatePerDay(10);
                });
                options.ApplyStrategy("test/action/{id?}", "5 requests per 10 seconds, fixed reset");
            });

            services.ConfigureMvc(options =>
            {
            });
        }

        public void Configure(IApplicationBuilder app)
        {
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
}
