using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;
using Moq;
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
            services.AddTransient<IMemoryCache, MemoryCache>();
            services.AddThrottling();

            services.AddInstance<IRateStore>(RedisTestConfig.CreateStoreInstance(GetType().Name));

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
                    builder.LimitIPBandwidth(160, TimeSpan.FromDays(1));
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

                    context.Response.Body.WriteByte(123);
                    var buffer = System.Text.Encoding.UTF8.GetBytes("test");
                    context.Response.Body.Write(buffer, 1, buffer.Length-1);

                    return context.Response.WriteAsync("{text: \"Hello!\"}");
                };
            });
        }
    }
    
    public static class RedisTestConfig
    {
        public static int RedisPort = 6379; // override default so that do not interfere with anyone else's server

        private static ISystemClock CreateClock()
        {
            // Warning : Take care of the year 3K bug !
            Mock<ISystemClock> clock = new Mock<ISystemClock>();
            clock.Setup(c => c.UtcNow)
                .Returns(new DateTimeOffset(3000, 1, 1, 0, 0, 0, TimeSpan.Zero));

            return clock.Object;
        }

        public static RedisRateStore CreateStoreInstance(string instanceName)
        {
            var connection = ConnectionMultiplexer.Connect("localhost:" + RedisPort + ",allowAdmin=true");

            var database = connection.GetDatabase();
            var server = connection.GetServer("localhost:" + RedisPort);
            server.FlushDatabase();

            return new RedisRateStore(new RedisThrottleOptions()
            {
                Configuration = "localhost:" + RedisPort,
                InstanceName = instanceName,
            }, CreateClock());
        }

    }
}
