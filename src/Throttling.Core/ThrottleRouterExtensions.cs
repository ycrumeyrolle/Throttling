using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public static class ThrottleRouterExtensions
    {
        public static void ApplyPolicy([NotNull]this IThrottleRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] ThrottlePolicy policy)
        {
            router.Add(new UnnamedThrottleRoute(httpMethods, routeTemplate, policy));
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlePolicyBuilder> configurePolicy)
        {
            var policyBuilder = new ThrottlePolicyBuilder(policyName);
            configurePolicy(policyBuilder);
            router.ApplyPolicy(httpMethods, routeTemplate, policyBuilder.Build());
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            ThrottlePolicy policy;
            if (!router.PolicyMap.TryGetValue(policyName, out policy))
            {
                throw new InvalidOperationException("Not policy named '" + policyName + "'");
            }

            router.ApplyPolicy(httpMethods, routeTemplate, policy);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] ThrottlePolicy policy)
        {
            router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policy);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlePolicyBuilder> configurePolicy)
        {
            router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policyName, configurePolicy);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policyName);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouter router, [NotNull] string routeTemplate, [NotNull] ThrottlePolicy policy)
        {
            router.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policy);
        }
        public static void ApplyPolicy([NotNull]this IThrottleRouter router, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            router.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policyName);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouter router, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlePolicyBuilder> configurePolicy)
        {
            router.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policyName, configurePolicy);
        }
    }
}