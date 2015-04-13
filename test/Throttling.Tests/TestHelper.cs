using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.TestHost;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Infrastructure;
using System.Runtime.Versioning;
using Moq;

namespace Throttling.Tests
{
    public static class TestHelper
    {
        // Path from Throttling\\test\\Throttling.FunctionalTests
        private static readonly string WebsitesDirectoryPath = Path.Combine("..", "WebSites");

        public static TestServer CreateServer(Action<IApplicationBuilder> builder, string applicationWebSiteName)
        {
            return CreateServer(builder, applicationWebSiteName, applicationPath: null);
        }

        public static TestServer CreateServer(
            Action<IApplicationBuilder> builder,
            string applicationWebSiteName,
            string applicationPath)
        {
            return CreateServer(
                builder,
                applicationWebSiteName,
                applicationPath,
                configureServices: (Action<IServiceCollection>)null);
        }

        public static TestServer CreateServer(
            Action<IApplicationBuilder> builder,
            string applicationWebSiteName,
            Action<IServiceCollection> configureServices)
        {
            return CreateServer(
                builder,
                applicationWebSiteName,
                applicationPath: null,
                configureServices: configureServices);
        }

        public static TestServer CreateServer(
            Action<IApplicationBuilder> builder,
            string applicationWebSiteName,
            string applicationPath,
            Action<IServiceCollection> configureServices)
        {
            return TestServer.Create(
                builder,
                services => AddTestServices(services, applicationWebSiteName, applicationPath, configureServices));
        }

        public static TestServer CreateServer(
            Action<IApplicationBuilder> builder,
            string applicationWebSiteName,
            Func<IServiceCollection, IServiceProvider> configureServices)
        {
            return CreateServer(
                builder,
                applicationWebSiteName,
                applicationPath: null,
                configureServices: configureServices);
        }

        public static TestServer CreateServer(
            Action<IApplicationBuilder> builder,
            string applicationWebSiteName,
            string applicationPath,
            Func<IServiceCollection, IServiceProvider> configureServices)
        {
            return TestServer.Create(
                CallContextServiceLocator.Locator.ServiceProvider,
                builder,
                services =>
                {
                    AddTestServices(services, applicationWebSiteName, applicationPath, configureServices: null);
                    return (configureServices != null) ? configureServices(services) : services.BuildServiceProvider();
                });
        }

        private static void AddTestServices(
            IServiceCollection services,
            string applicationWebSiteName,
            string applicationPath,
            Action<IServiceCollection> configureServices)
        {
            applicationPath = applicationPath ?? WebsitesDirectoryPath;

            // Get current IApplicationEnvironment; likely added by the host.
            var provider = services.BuildServiceProvider();
            var originalEnvironment = provider.GetRequiredService<IApplicationEnvironment>();

            // When an application executes in a regular context, the application base path points to the root
            // directory where the application is located, for example MvcSample.Web. However, when executing
            // an application as part of a test, the ApplicationBasePath of the IApplicationEnvironment points
            // to the root folder of the test project.
            // To compensate for this, we need to calculate the original path and override the application
            // environment value so that components like the view engine work properly in the context of the
            // test.
            var applicationBasePath = CalculateApplicationBasePath(
                originalEnvironment,
                applicationWebSiteName,
                applicationPath);
            var environment = new TestApplicationEnvironment(
                originalEnvironment,
                applicationBasePath,
                applicationWebSiteName);
            services.AddInstance<IApplicationEnvironment>(environment);
            var hostingEnvironment = new HostingEnvironment();
            hostingEnvironment.Initialize(applicationBasePath, environmentName: null);
            services.AddInstance<IHostingEnvironment>(hostingEnvironment);

            var clock = CreateClock();
            services.AddInstance(clock);

            if (configureServices != null)
            {
                configureServices(services);
            }
        }

        // Calculate the path relative to the application base path.
        private static string CalculateApplicationBasePath(
            IApplicationEnvironment appEnvironment,
            string applicationWebSiteName,
            string websitePath)
        {
            // Mvc/test/WebSites/applicationWebSiteName
            return Path.GetFullPath(
                Path.Combine(appEnvironment.ApplicationBasePath, websitePath, applicationWebSiteName));
        }

        private static ISystemClock CreateClock()
        {
            Mock<ISystemClock> clock = new Mock<ISystemClock>();
            clock.Setup(c => c.UtcNow)
                .Returns(new DateTimeOffset(2000, 01, 01, 00, 00, 00, TimeSpan.Zero));

            return clock.Object;
        }
    }

    // Represents an application environment that overrides the base path of the original
    // application environment in order to make it point to the folder of the original web
    // aplication so that components like ViewEngines can find views as if they were executing
    // in a regular context.
    public class TestApplicationEnvironment : IApplicationEnvironment
    {
        private readonly IApplicationEnvironment _originalAppEnvironment;
        private readonly string _applicationBasePath;
        private readonly string _applicationName;

        public TestApplicationEnvironment(IApplicationEnvironment originalAppEnvironment, string appBasePath, string appName)
        {
            _originalAppEnvironment = originalAppEnvironment;
            _applicationBasePath = appBasePath;
            _applicationName = appName;
        }

        public string ApplicationName
        {
            get { return _applicationName; }
        }

        public string Version
        {
            get { return _originalAppEnvironment.Version; }
        }

        public string ApplicationBasePath
        {
            get { return _applicationBasePath; }
        }

        public string Configuration
        {
            get
            {
                return _originalAppEnvironment.Configuration;
            }
        }

        public FrameworkName RuntimeFramework
        {
            get { return _originalAppEnvironment.RuntimeFramework; }
        }
    }
}