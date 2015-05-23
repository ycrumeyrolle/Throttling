using System;
using System.Threading.Tasks;
using Xunit;
using Throttling;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Logging.Testing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Caching.Memory;
using System.Net;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http.Internal;

namespace Throttling.Tests
{
    public class RateLimitPolicyTest
    {
        private MemoryCache CreateCache()
        {
            return new MemoryCache(new MemoryCacheOptions()
            {
            });
        }

        private ISystemClock CreateClock()
        {
            Mock<ISystemClock> clock = new Mock<ISystemClock>();
            clock.Setup(c => c.UtcNow)
                .Returns(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));

            return clock.Object;
        }
        
        [Theory]
        [InlineData(false, 9, 10)]
        [InlineData(true, 0, 0)]

        public async Task Test(bool exceptedLimitReached, long expectRemainingCalls, long calls)
        {
            // Arrange
            var clock = CreateClock();
            ThrottlingOptionsSetup setup = new ThrottlingOptionsSetup(new CacheRateStore(CreateCache()), clock, new ThrottlingRouteCollection(), new RouteClientKeyProvider("{apiKey}", "apiKey"));
            var optionsAccessor = new OptionsManager<ThrottlingOptions>(new[] { setup });
            optionsAccessor.Configure();
            RateLimitPolicy policy = new RateLimitPolicyStub(calls, TimeSpan.FromDays(1), false);
            HttpContext httpContext = new DefaultHttpContext();
            policy.Configure(optionsAccessor.Options);

            // Act
            var results = await policy.EvaluateAsync(httpContext, "endpoint");

            Assert.NotNull(results);
            Assert.Equal(1, results.Count());
            var result = results.First();

            Assert.Equal(exceptedLimitReached, result.LimitReached);
            Assert.Equal("category", result.Category);
            Assert.Equal("endpoint", result.Endpoint);
            Assert.Equal("key", result.Key);
            Assert.Equal(clock.UtcNow.AddDays(1), result.Reset);
            Assert.Equal(clock.UtcNow.AddDays(1), result.Rate.Reset);
            Assert.Equal(expectRemainingCalls, result.Rate.RemainingCalls);
        }

        private class RateLimitPolicyStub : RateLimitPolicy
        {
            public RateLimitPolicyStub(long calls, TimeSpan renewalPeriod, bool sliding)
                : base(calls, renewalPeriod, sliding)
            {
                Category = "category";
            }

            public override void AddRateLimitHeaders(RemainingRate rate, IDictionary<string, string> rateLimitHeaders)
            {
            }

            public override string GetKey([NotNull] HttpContext context)
            {
                return "key";
            }
        }
    }
}
