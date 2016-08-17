using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Throttling;
using Throttling.Tests.Common;

namespace ThrottlingSample
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                    .AddThrottling(options =>
            {
                options.AddPolicy("Empty")
                    .LimitIPRate(10, TimeSpan.FromDays(1));

                options.AddPolicy("5 requests per 10 seconds, sliding reset")
                    .LimitAuthenticatedUserRate(5, TimeSpan.FromSeconds(10), true)
                    .LimitIPRate(10, TimeSpan.FromDays(1));
                options.AddPolicy("5 requests per 10 seconds, fixed reset")
                    .LimitAuthenticatedUserRate(5, TimeSpan.FromSeconds(10))
                    .LimitIPRate(10, TimeSpan.FromDays(1));
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug()
                         .AddConsole();
            app.UseMiddleware<IPEnforcerMiddleware>();
            app.UseThrottling(routes =>
            {
                routes.ApplyPolicy("test/action/{id?}", "5 requests per 10 seconds, fixed reset");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });
        }

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
