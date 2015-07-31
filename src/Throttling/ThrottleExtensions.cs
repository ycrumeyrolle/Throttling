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
        public static IApplicationBuilder UseThrottling([NotNull] this IApplicationBuilder app)
        {
            return app.UseThrottling(configureRoutes => { });
        }

        /// <summary>
        /// Enable throttling.
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseThrottling([NotNull] this IApplicationBuilder app, [NotNull] Action<IThrottleRouteBuilder> configureRoutes)
        {
            var builder = new ThrottleRouteBuilder();

            configureRoutes(builder);
            Action<ThrottleOptions> configure;
            configure = o =>
            {
                o.Routes = builder.Build();
            };

            return app.UseMiddleware<ThrottleMiddleware>(
                new ConfigureOptions<ThrottleOptions>(configure)
                {
                    Name = string.Empty
                });
        }
    }
}