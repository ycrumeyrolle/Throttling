using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Throttling.Tests
{
    public class MvcTestFixture<TStartup> : IDisposable
    {
        private readonly TestServer _server;

        public MvcTestFixture()
            : this(Path.Combine("test", "WebSites"))
        {
        }

        protected MvcTestFixture(string solutionRelativePath)
        {
            // RequestLocalizationOptions saves the current culture when constructed, potentially changing response
            // localization i.e. RequestLocalizationMiddleware behavior. Ensure the saved culture
            // (DefaultRequestCulture) is consistent regardless of system configuration or personal preferences.
            using (new CultureReplacer())
            {
                var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
                var contentRoot = GetProjectPath(solutionRelativePath, startupAssembly);

                var builder = new WebHostBuilder()
                    .UseContentRoot(contentRoot)
                    .ConfigureServices(InitializeServices)
                    .UseStartup(typeof(TStartup));

                _server = new TestServer(builder);
            }

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
            services.AddSingleton(new InMemoryRateStore(new MemoryCache(Options.Create(new MemoryCacheOptions())), new SystemClock()));
        }

        public static string GetProjectPath(string solutionRelativePath, Assembly assembly)
        {
            var projectName = assembly.GetName().Name;
            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                var solutionFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "Throttling.sln"));
                if (solutionFileInfo.Exists)
                {
                    return Path.GetFullPath(Path.Combine(directoryInfo.FullName, solutionRelativePath, projectName));
                }

                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}.");
        }
    }

    public class MvcSampleFixture<TStartup> : MvcTestFixture<TStartup>
    {
        public MvcSampleFixture()
            : base()
        {
        }
    }

    public abstract class ThrottleFunctionalTest<TStartup> : IClassFixture<MvcSampleFixture<TStartup>>
    {
        protected ThrottleFunctionalTest(HttpClient client)
        {
            Client = client;
        }

        public HttpClient Client { get; }

        [Theory]
        [InlineData(1, "9")]
        [InlineData(10, "0")]
        public async Task ResourceWithSimplePolicy_BellowLimits_Returns200(int tries, string userRemaining)
        {
            // Arrange
            //var server = CreateServer(App, ConfigureServices);
            //var client = server.CreateClient();
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                // Act
                response = await Client.GetAsync("http://localhost/apikey/test/RateLimit10PerHour/" + i);
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
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                // Act
                response = await Client.GetAsync("http://localhost/apikey/test/RateLimit10PerHour/" + i);
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
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                // Act
                await Client.GetAsync("http://localhost/apikey/test/RateLimit10PerHour/" + i);
                response = await Client.GetAsync("http://localhost/apikey/test/RateLimit10PerHour2/" + i);
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
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                // Act
                response = await Client.GetAsync("http://localhost/apikey/test/RateLimit10PerHour/" + i);
                response = await Client.GetAsync("http://localhost/apikey/test/RateLimit10PerHour2/" + i);
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
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                // Act
                response = await Client.GetAsync("http://localhost/apikey/test/Quota160BPerHourByApiKey/" + i);
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
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                // Act
                response = await Client.GetAsync("http://localhost/apikey/test/Quota160BPerHourByIP/" + i);
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
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                // Act
                response = await Client.GetAsync("http://localhost/apikey/test/Quota160BPerHourByApiKey/" + i);
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
            HttpResponseMessage response = null;
            for (int i = 0; i < tries; i++)
            {
                // Act
                response = await Client.GetAsync("http://localhost/apikey/test/Quota160BPerHourByApiKey/" + i);
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

    public class SimpleThrottlingFunctionalTest : ThrottleFunctionalTest<SimpleThrottling.Startup>
    {
        public SimpleThrottlingFunctionalTest(MvcSampleFixture<SimpleThrottling.Startup> fixture) : base(fixture.Client)
        {
        }
    }

    public class MvcThrottlingFunctionalTest : ThrottleFunctionalTest<MvcThrottling.Startup>
    {
        public MvcThrottlingFunctionalTest(MvcSampleFixture<MvcThrottling.Startup> fixture) : base(fixture.Client)
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
        //            .CreateRequest("http://localhost/apikey/test/RateLimit10PerHour/" + i);

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
