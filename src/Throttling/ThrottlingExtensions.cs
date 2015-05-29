using System;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public static class ThrottlingExtensions
    {
        /// <summary>
        /// Enable throttling on the current path
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseThrottling([NotNull] this IApplicationBuilder app, string policyName)
        {
            return app.UseMiddleware<ThrottlingMiddleware>(policyName);
        }

        /// <summary>
        /// Enable directory browsing with the given options
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseThrottling([NotNull] this IApplicationBuilder app)
        {
            return app.UseMiddleware<ThrottlingMiddleware>();
        }
    }
}