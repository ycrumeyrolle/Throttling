using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using StackExchange.Redis;
using Throttling;
using Throttling.Redis;
using Throttling.Tests.Common;

namespace RedisThrottling
{
    public class Startup
    {
        // Set up application services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddThrottlingCore()
                    .AddThrottlingRedis(options =>
                    {
                        options.Configuration = "localhost:6379";
                        options.InstanceName = GetType().Name;
                    });

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

            ResetRedis(GetType().Name);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<IPEnforcerMiddleware>();

            app.UseThrottling(routes => 
            {
                routes.ApplyPolicy("{apikey}/test/action1/{id?}", "10 requests per hour, fixed reset");
                routes.ApplyPolicy("{apikey}/test/action2/{id?}", "10 requests per hour, fixed reset");
                routes.ApplyPolicy("{apikey}/test/action3/{id?}", "160 bytes per hour by IP");
                routes.ApplyPolicy("{apikey}/test/action4/{id?}", "160 bytes per hour by API key");
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

        private static int RedisPort = 6379; // override default so that do not interfere with anyone else's server

        private static void ResetRedis(string instanceName)
        {
            var connection = ConnectionMultiplexer.Connect("localhost:" + RedisPort + ",allowAdmin=true");
            var database = connection.GetDatabase();
            var server = connection.GetServer("localhost:" + RedisPort);
            server.FlushDatabase();
        }
    }
}
