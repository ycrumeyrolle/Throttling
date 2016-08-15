using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Throttling.Tests
{
    public abstract class ThrottleFunctionalTest
    {
        protected ThrottleFunctionalTest(string siteName, Action<IApplicationBuilder> app, Action<IServiceCollection> configureServices)
        {
            SiteName = siteName;
            App = app;
            ConfigureServices = configureServices;
        }

        public string SiteName { get; set; }

        public Action<IApplicationBuilder> App { get; }

        public Action<IServiceCollection> ConfigureServices { get; }

        [Theory]
        [InlineData(1, "9")]
        [InlineData(10, "0")]
        public async Task ResourceWithSimplePolicy_BellowLimits_Returns200(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/apikey/test/action1/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Single(response.Headers.GetValues("X-RateLimit-IPLimit"), "10");
            Assert.Single(response.Headers.GetValues("X-RateLimit-IPRemaining"), userRemaining);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-IPReset").First());
        }

        [Theory]
        [InlineData(11, "0")]
        public async Task ResourceWithSimplePolicy_BeyondLimits_Returns429(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/apikey/test/action1/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            Assert.Equal((HttpStatusCode)429, response.StatusCode);

            //Assert.Single(response.Headers.GetValues("X-RateLimit-UserLimit"), "10");
            //Assert.Single(response.Headers.GetValues("X-RateLimit-UserRemaining"), userRemaining);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").Single());

            Assert.Single(response.Headers.GetValues("Cache-Control"), "no-store, no-cache");
            Assert.Single(response.Headers.GetValues("Pragma"), "no-cache");
            Assert.Single(response.Headers.GetValues("Retry-After"), "3600");
        }

        [Theory]
        [InlineData(1, "8")]
        [InlineData(5, "0")]
        public async Task TwoResourcesWithSamePolicy_BellowLimits_Returns200(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder1 = server
                    .CreateRequest("http://localhost/apikey/test/action1/" + i);
                var requestBuilder2 = server
                    .CreateRequest("http://localhost/apikey/test/action2/" + i);

                // Act
                await requestBuilder1.SendAsync("GET");
                response = await requestBuilder2.SendAsync("GET");
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Single(response.Headers.GetValues("X-RateLimit-IPLimit"), "10");
            Assert.Single(response.Headers.GetValues("X-RateLimit-IPRemaining"), userRemaining);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").First());
        }

        [Theory]
        [InlineData(21, "0")]
        public async Task TwoResourcesWithSamePolicy_BeyondLimits_Returns429(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder1 = server
                    .CreateRequest("http://localhost/apikey/test/action1/" + i);
                var requestBuilder2 = server
                    .CreateRequest("http://localhost/apikey/test/action2/" + i);

                // Act
                response = await requestBuilder1.SendAsync("GET");
                response = await requestBuilder2.SendAsync("GET");
            }

            // Assert
            Assert.Equal((HttpStatusCode)429, response.StatusCode);
            var responseHeaders = response.Headers;

            //Assert.Single(response.Headers.GetValues("X-RateLimit-UserLimit"), "10");
            //Assert.Single(response.Headers.GetValues("X-RateLimit-UserRemaining"), userRemaining);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").First());

            Assert.Single(response.Headers.GetValues("Cache-Control"), "no-store, no-cache");
            Assert.Single(response.Headers.GetValues("Pragma"), "no-cache");
            Assert.Single(response.Headers.GetValues("Retry-After"), "3600");
        }

        [Theory]
        [InlineData(1, "144")]
        [InlineData(5, "80")]
        [InlineData(10, "0")]
        public async Task BandwidthPolicy_BellowLimits_Returns200(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/apikey/test/action4/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").First());
        }

        [Theory]
        [InlineData(11, "0")]
        public async Task BandwidthPolicy_BeyondLimits_Returns429(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/apikey/test/action3/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            Assert.Equal((HttpStatusCode)429, response.StatusCode);
            var responseHeaders = response.Headers;

            //Assert.Single(response.Headers.GetValues("X-RateLimit-UserLimit"), "10");
            //Assert.Single(response.Headers.GetValues("X-RateLimit-UserRemaining"), userRemaining);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").First());

            Assert.Single(response.Headers.GetValues("Cache-Control"), "no-store, no-cache");
            Assert.Single(response.Headers.GetValues("Pragma"), "no-cache");
            Assert.Single(response.Headers.GetValues("Retry-After"), "3600");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task ApiKeyPolicy_BellowLimits_Returns200(int tries)
        {
            // Arrange
            var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/apikey/test/action4/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").First());
        }

        [Theory]
        [InlineData(11)]
        public async Task ApiKeyPolicy_BeyondLimits_Returns429(int tries)
        {
            // Arrange
            var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/apikey/test/action4/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            Assert.Equal((HttpStatusCode)429, response.StatusCode);
            var responseHeaders = response.Headers;

            //Assert.Single(response.Headers.GetValues("X-RateLimit-UserLimit"), "10");
            //Assert.Single(response.Headers.GetValues("X-RateLimit-UserRemaining"), userRemaining);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").First());

            Assert.Single(response.Headers.GetValues("Cache-Control"), "no-store, no-cache");
            Assert.Single(response.Headers.GetValues("Pragma"), "no-cache");
            Assert.Single(response.Headers.GetValues("Retry-After"), "3600");
        }
    }

    public class SimpleThrottlingFunctionalTest : ThrottleFunctionalTest
    {
        public SimpleThrottlingFunctionalTest()
            : base(nameof(SimpleThrottling), new SimpleThrottling.Startup().Configure, new SimpleThrottling.Startup().ConfigureServices)
        {
        }
    }

    public class MvcThrottlingFunctionalTest : ThrottleFunctionalTest
    {
        public MvcThrottlingFunctionalTest()
            : base(nameof(MvcThrottling), new MvcThrottling.Startup().Configure, new MvcThrottling.Startup().ConfigureServices)
        {
        }
    }
    public class RedisThrottlingFunctionalTest : ThrottleFunctionalTest
    {
        public RedisThrottlingFunctionalTest()
            : base(nameof(RedisThrottling), new RedisThrottling.Startup().Configure, new RedisThrottling.Startup().ConfigureServices)
        {
        }
    }

    public class ThrottlingFunctionalTest2
    {
        private const string SiteName = nameof(SimpleThrottling);
        private readonly Action<IApplicationBuilder> _app = new SimpleThrottling.Startup().Configure;
        private readonly Action<IServiceCollection> _configureServices = new SimpleThrottling.Startup().ConfigureServices;

     //   [Theory]
        //[InlineData(1, "9")]
        //[InlineData(10, "0")]
        //public async Task ResourceWithSimplePolicy_BellowLimits_Returns200(int tries, string userRemaining)
        //{
        //    // Arrange
        //    var server = TestHelper.CreateServer(App, SiteName, ConfigureServices);
        //    var client = server.CreateClient();
        //    HttpResponseMessage response = null;
        //    for (int i = 0; i < tries; i++)
        //    {
        //        var requestBuilder = server
        //            .CreateRequest("http://localhost/apikey/test/action1/" + i);

        //        // Act
        //        response = await requestBuilder.SendAsync("GET");
        //    }

        //    // Assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        //    Assert.Single(response.Headers.GetValues("X-RateLimit-IPLimit"), "10");
        //    Assert.Single(response.Headers.GetValues("X-RateLimit-IPRemaining"), userRemaining);

        //    // TODO : Fix the ISystemClock
        // //   Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-IPReset").First());
        //}
    
    }
}
