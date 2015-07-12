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
            services.AddThrottling();
            services.AddRedisThrottling(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = GetType().Name;
            });

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
                options.Routes.ApplyPolicy("{apikey}/test/action1/{id?}", "10 requests per hour, fixed reset");
                options.Routes.ApplyPolicy("{apikey}/test/action2/{id?}", "10 requests per hour, fixed reset");
                options.Routes.ApplyPolicy("{apikey}/test/action3/{id?}", "160 bytes per hour by IP");
                options.Routes.ApplyPolicy("{apikey}/test/action4/{id?}", "160 bytes per hour by API key");
            });

            RedisTestConfig.ResetRedis(GetType().Name);
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

    public static class RedisTestConfig
    {
        public static int RedisPort = 6379; // override default so that do not interfere with anyone else's server

        public static void ResetRedis(string instanceName)
        {
            var connection = ConnectionMultiplexer.Connect("localhost:" + RedisPort + ",allowAdmin=true");
            var database = connection.GetDatabase();
            var server = connection.GetServer("localhost:" + RedisPort);
            server.FlushDatabase();
        }
    }
}
