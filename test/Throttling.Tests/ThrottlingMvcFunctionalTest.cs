using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Cors.Core;
using Microsoft.Framework.Logging;
using Microsoft.Framework.DependencyInjection;
using Xunit;
using MvcThrottling;
using Moq;

namespace Throttling.Tests
{
    public class ThrottlingMvcFunctionalTest
    {
        private const string SiteName = nameof(MvcThrottling);
        private readonly Action<IApplicationBuilder> _app = new Startup().Configure;
        private readonly Action<IServiceCollection> _configureServices = new Startup().ConfigureServices;

        [Theory]
        [InlineData(1, "9")]
        [InlineData(10, "0")]
        public async Task ResourceWithSimplePolicy_BellowLimits_Returns200(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/Sliding/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(response.Headers.GetValues("X-RateLimit-UserLimit"), "10");
            Assert.Single(response.Headers.GetValues("X-RateLimit-UserRemaining"), userRemaining);
        }

        [Theory]
        [InlineData(11, "0")]
        public async Task ResourceWithSimplePolicy_BeyondLimits_Returns429(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/Sliding/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            // TODO : assertions for http headers
            Assert.Equal((HttpStatusCode)429, response.StatusCode);
            Assert.Single(response.Headers.GetValues("X-RateLimit-UserLimit"), "10");
            Assert.Single(response.Headers.GetValues("X-RateLimit-UserRemaining"), userRemaining);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").Single());

            Assert.Single(response.Headers.GetValues("Cache-Control"), "no-store, no-cache");
            Assert.Single(response.Headers.GetValues("Pragma"), "no-cache");
            Assert.Single(response.Headers.GetValues("Retry-After"), "86400");
        }

        [Theory]
        [InlineData(1, "9")]
        [InlineData(10, "0")]
        public async Task TwoResourcesWithSamePolicy_BellowLimits_Returns200(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder1 = server
                    .CreateRequest("http://localhost/Sliding/" + i);
                var requestBuilder2 = server
                    .CreateRequest("http://localhost/Sliding2/" + i);

                // Act
                response = await requestBuilder1.SendAsync("GET");
                response = await requestBuilder2.SendAsync("GET");
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(response.Headers.GetValues("X-RateLimit-UserLimit"), "10");
            Assert.Single(response.Headers.GetValues("X-RateLimit-UserRemaining"), userRemaining);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").First());
        }

        [Theory]
        [InlineData(11, "0")]
        public async Task TwoResourcesWithSamePolicy_BeyondLimits_Returns429(int tries, string userRemaining)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder1 = server
                    .CreateRequest("http://localhost/Sliding/" + i);
                var requestBuilder2 = server
                    .CreateRequest("http://localhost/Sliding2/" + i);

                // Act
                response = await requestBuilder1.SendAsync("GET");
                response = await requestBuilder2.SendAsync("GET");
            }

            // Assert
            Assert.Equal((HttpStatusCode)429, response.StatusCode);

            // TODO : Fix the ISystemClock
            // Assert.Equal("1428964312", response.Headers.GetValues("X-RateLimit-UserReset").First());

            Assert.Single(response.Headers.GetValues("Cache-Control"), "no-store, no-cache");
            Assert.Single(response.Headers.GetValues("Pragma"), "no-cache");
            Assert.Single(response.Headers.GetValues("Retry-After"), "86400");
        }
    }
}
