using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Throttling.Mvc;

namespace MvcThrottling
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
                    builder.AddUserLimitRate(5, System.TimeSpan.FromSeconds(10), true);
                });
                options.AddPolicy("5 requests per 10 seconds, fixed reset", builder =>
                {
                    builder.AddUserLimitRate(5, System.TimeSpan.FromSeconds(10));
                });
            });

            services.ConfigureMvc(options =>
            {
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Verbose);
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });
        }
    }
}
