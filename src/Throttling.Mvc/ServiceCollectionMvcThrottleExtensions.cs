using System;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.Framework.Internal;
using Microsoft.Framework.DependencyInjection.Extensions;
using Throttling;
using Throttling.Mvc;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// The <see cref="IThrottleServiceBuilder"/> extensions for enabling Throttling support on MVC.
    /// </summary>
    public static class ServiceCollectionMvcThrottleExtensions
    {
        public static IMvcBuilder AddThrottling(this IMvcBuilder builder, Action<ThrottleOptions> configure = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddThrottlingCore();
            builder.Services.TryAddTransient<IThrottleFilter, ThrottleFilter>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ThrottleApplicationModelProvider>());

            if (configure == null)
            {
                configure = o =>
                {
                };
            }
            builder.Services.Configure(configure);

            return builder;
        }
    }
}