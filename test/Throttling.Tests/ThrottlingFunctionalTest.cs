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
using SimpleThrottling;
using Moq;

namespace Throttling.Tests
{
    public class ThrottlingFunctionalTest
    {
        private const string SiteName = nameof(MvcThrottling);
        private readonly Action<IApplicationBuilder> _app = new Startup().Configure;
        private readonly Action<IServiceCollection> _configureServices = new Startup().ConfigureServices;

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public async Task ResourceWithSimplePolicy_BellowLimits_Returns200(int tries)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/apikey/test/action/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(11)]
        public async Task ResourceWithSimplePolicy_BeyondLimits_Returns429(int tries)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder = server
                    .CreateRequest("http://localhost/apikey/test/action/" + i);

                // Act
                response = await requestBuilder.SendAsync("GET");
            }

            // Assert
            // TODO : assertions for http headers
            Assert.Equal((HttpStatusCode)429, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public async Task TwoResourcesWithSamePolicy_BellowLimits_Returns200(int tries)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder1 = server
                    .CreateRequest("http://localhost/apikey/test/action/" + i);
                var requestBuilder2 = server
                    .CreateRequest("http://localhost/apikey/test/action2/" + i);

                // Act
                response = await requestBuilder1.SendAsync("GET");
                response = await requestBuilder2.SendAsync("GET");
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseHeaders = response.Headers;
        }

        [Theory]
        [InlineData(21)]
        public async Task TwoResourcesWithSamePolicy_BeyondLimits_Returns429(int tries)
        {
            // Arrange
            var server = TestHelper.CreateServer(_app, SiteName, _configureServices);
            var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                var requestBuilder1 = server
                    .CreateRequest("http://localhost/apikey/test/action/" + i);
                var requestBuilder2 = server
                    .CreateRequest("http://localhost/apikey/test/action2/" + i);

                // Act
                response = await requestBuilder1.SendAsync("GET");
                response = await requestBuilder2.SendAsync("GET");
            }

            // Assert
            Assert.Equal((HttpStatusCode)429, response.StatusCode);
            var responseHeaders = response.Headers;
        }
    }
}
