using Microsoft.AspNet.Builder;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public static class ThrottleExtensions
    {
        /// <summary>
        /// Enable throttling on the current path
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseThrottling([NotNull] this IApplicationBuilder app, string policyName)
        {
            return app.UseMiddleware<ThrottleMiddleware>(policyName);
        }

        /// <summary>
        /// Enable directory browsing with the given options
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseThrottling([NotNull] this IApplicationBuilder app)
        {
            return app.UseMiddleware<ThrottleMiddleware>();
        }
    }
}