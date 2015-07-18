using System;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.Framework.Internal;
using Throttling;
using Throttling.Mvc;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// The <see cref="IThrottleServiceBuilder"/> extensions for enabling Throttling support on MVC.
    /// </summary>
    public static class ServiceCollectionMvcThrottleExtensions
    {
        public static IThrottleServiceBuilder AddMvcThrottling([NotNull] this IThrottleServiceBuilder builder, Action<ThrottleOptions> configure = null)
        {
            builder.TryAddTransient<IThrottleFilter, ThrottleFilter>();
            builder.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ThrottleApplicationModelProvider>());

            if (configure == null)
            {
                configure = o =>
                {
                };
            }
            builder.Configure(configure);

            return builder;
        }
    }
}