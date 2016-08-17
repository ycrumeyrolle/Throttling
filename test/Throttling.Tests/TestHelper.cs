using System;
using System.IO;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace Throttling.Tests
{
    //public static class TestHelper
    //{
    //    // Path from Throttling\\test\\Throttling.FunctionalTests
    //    private static readonly string WebsitesDirectoryPath = Path.Combine("..", "WebSites");

    //    public static TestServer CreateServer(Action<IApplicationBuilder> builder, string applicationWebSiteName)
    //    {
    //        return CreateServer(builder, applicationWebSiteName, applicationPath: null);
    //    }

    //    public static TestServer CreateServer(
    //        Action<IApplicationBuilder> builder,
    //        string applicationWebSiteName,
    //        string applicationPath)
    //    {
    //        return CreateServer(
    //            builder,
    //            applicationWebSiteName,
    //            applicationPath,
    //            configureServices: (Action<IServiceCollection>)null);
    //    }

    //    public static TestServer CreateServer(
    //        Action<IApplicationBuilder> builder,
    //        string applicationWebSiteName,
    //        Action<IServiceCollection> configureServices)
    //    {
    //        return CreateServer(
    //            builder,
    //            applicationWebSiteName,
    //            applicationPath: null,
    //            configureServices: configureServices);
    //    }

    //    public static TestServer CreateServer(
    //        Action<IApplicationBuilder> builder,
    //        string applicationWebSiteName,
    //        string applicationPath,
    //        Action<IServiceCollection> configureServices)
    //    {
    //        return TestServer.Create(
    //            builder,
    //            services => AddTestServices(services, applicationWebSiteName, applicationPath, configureServices));
    //    }

    //    public static TestServer CreateServer(
    //        Action<IApplicationBuilder> builder,
    //        string applicationWebSiteName,
    //        Func<IServiceCollection, IServiceProvider> configureServices)
    //    {
    //        return CreateServer(
    //            builder,
    //            applicationWebSiteName,
    //            applicationPath: null,
    //            configureServices: configureServices);
    //    }

    //    public static TestServer CreateServer(
    //        Action<IApplicationBuilder> builder,
    //        string applicationWebSiteName,
    //        string applicationPath,
    //        Func<IServiceCollection, IServiceProvider> configureServices)
    //    {
    //        return TestServer.Create(
    //            PlatformServices.Default.Application,
    //            builder,
    //            services =>
    //            {
    //                AddTestServices(services, applicationWebSiteName, applicationPath, configureServices: null);
    //                return (configureServices != null) ? configureServices(services) : services.BuildServiceProvider();
    //            });
    //    }

    //    private static void AddTestServices(
    //        IServiceCollection services,
    //        string applicationWebSiteName,
    //        string applicationPath,
    //        Action<IServiceCollection> configureServices)
    //    {
    //        applicationPath = applicationPath ?? WebsitesDirectoryPath;

    //        // Get current IHostingEnvironment; likely added by the host.
    //        var provider = services.BuildServiceProvider();
    //        var originalEnvironment = provider.GetRequiredService<IHostingEnvironment>();

    //        // When an application executes in a regular context, the application base path points to the root
    //        // directory where the application is located, for example MvcSample.Web. However, when executing
    //        // an application as part of a test, the ApplicationBasePath of the IHostingEnvironment points
    //        // to the root folder of the test project.
    //        // To compensate for this, we need to calculate the original path and override the application
    //        // environment value so that components like the view engine work properly in the context of the
    //        // test.
    //        var applicationBasePath = CalculateApplicationBasePath(
    //            originalEnvironment,
    //            applicationWebSiteName,
    //            applicationPath);
    //        var environment = new TestApplicationEnvironment(
    //            originalEnvironment,
    //            applicationBasePath,
    //            applicationWebSiteName);
    //        services.AddInstance<IHostingEnvironment>(environment);
    //        var hostingEnvironment = new HostingEnvironment();
    //        hostingEnvironment.Initialize(applicationBasePath, environmentName: null);
    //        services.AddInstance<IHostingEnvironment>(hostingEnvironment);


    //        if (configureServices != null)
    //        {
    //            configureServices(services);
    //        }
    //        var clock = CreateClock();
    //        services.AddInstance(clock);
    //    }

    //    // Calculate the path relative to the application base path.
    //    private static string CalculateApplicationBasePath(
    //        IHostingEnvironment appEnvironment,
    //        string applicationWebSiteName,
    //        string websitePath)
    //    {
    //        // Mvc/test/WebSites/applicationWebSiteName
    //        return Path.GetFullPath(
    //            Path.Combine(appEnvironment.ContentRootPath, websitePath, applicationWebSiteName));
    //    }

    //    private static ISystemClock CreateClock()
    //    {
    //        return new TestClock();
    //    }

    //    private class TestClock : ISystemClock
    //    {
    //        public DateTimeOffset UtcNow
    //        {
    //            get
    //            {
    //                return new DateTimeOffset(3000, 01, 01, 00, 00, 00, TimeSpan.Zero);
    //            }
    //        }
    //    }
    //}

    //// Represents an application environment that overrides the base path of the original
    //// application environment in order to make it point to the folder of the original web
    //// aplication so that components like ViewEngines can find views as if they were executing
    //// in a regular context.
    //public class TestApplicationEnvironment : IHostingEnvironment
    //{
    //    private readonly IHostingEnvironment _originalAppEnvironment;
    //    private readonly string _applicationBasePath;
    //    private readonly string _applicationName;

    //    public TestApplicationEnvironment(IHostingEnvironment originalAppEnvironment, string appBasePath, string appName)
    //    {
    //        _originalAppEnvironment = originalAppEnvironment;
    //        _applicationBasePath = appBasePath;
    //        _applicationName = appName;
    //    }

    //    public string ApplicationName
    //    {
    //        get { return _applicationName; }
    //    }

    //    public string ApplicationVersion
    //    {
    //        get { return _originalAppEnvironment.ApplicationVersion; }
    //    }

    //    public string ApplicationBasePath
    //    {
    //        get { return _applicationBasePath; }
    //    }

    //    public string Configuration
    //    {
    //        get
    //        {
    //            return _originalAppEnvironment.Configuration;
    //        }
    //    }

    //    public FrameworkName RuntimeFramework
    //    {
    //        get { return _originalAppEnvironment.RuntimeFramework; }
    //    }

    //    public object GetData(string name)
    //    {
    //        return _originalAppEnvironment.GetData(name);
    //    }

    //    public void SetData(string name, object value)
    //    {
    //        _originalAppEnvironment.SetData(name, value);
    //    }
    //}
}