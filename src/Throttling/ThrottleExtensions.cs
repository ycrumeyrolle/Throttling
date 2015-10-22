using System;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public static class ThrottleExtensions
    {
        /// <summary>
        /// Enable throttling.
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseThrottling(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseThrottling(configureRoutes => { });
        }

        /// <summary>
        /// Enable throttling.
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseThrottling(this IApplicationBuilder app, Action<IThrottleRouteBuilder> configureRoutes)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (configureRoutes == null)
            {
                throw new ArgumentNullException(nameof(configureRoutes));
            }

            var builder = new ThrottleRouteBuilder();

            configureRoutes(builder);
            Action<ThrottleOptions> configure;
            configure = o =>
            {
                o.Routes = builder.Build();
            };

            return app.UseMiddleware<ThrottleMiddleware>(new ConfigureOptions<ThrottleOptions>(configure));
        }
    }
}