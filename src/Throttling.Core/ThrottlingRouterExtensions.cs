using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Throttling.IPRanges;

namespace Throttling
{
    public static class ThrottlingRouterExtensions
    {
        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] ThrottlingPolicy policy)
        {
            router.Add(new UnnamedThrottlingRoute(httpMethods, routeTemplate, policy));
        }

        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy)
        {
            var policyBuilder = new ThrottlingPolicyBuilder(policyName);
            configurePolicy(policyBuilder);
            router.ApplyPolicy(httpMethods, routeTemplate, policyBuilder.Build());
        }

        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            ThrottlingPolicy policy;
            if (!router.PolicyMap.TryGetValue(policyName, out policy))
            {
                throw new InvalidOperationException("Not policy named '" + policyName + "'");
            }

            router.ApplyPolicy(httpMethods, routeTemplate, policy);
        }

        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] ThrottlingPolicy policy)
        {
            router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policy);
        }

        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy)
        {
            router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policyName, configurePolicy);
        }

        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policyName);
        }

        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, [NotNull] string routeTemplate, [NotNull] ThrottlingPolicy policy)
        {
            router.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policy);
        }
        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            router.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policyName);
        }

        public static void ApplyPolicy([NotNull]this IThrottlingRouter router, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy)
        {
            router.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policyName, configurePolicy);
        }
    }
}