using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Throttling.IPRanges;

namespace Throttling
{
    public static class ThrottlingRouterExtensions
    {

        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] IThrottlingPolicy policy, IPWhitelist whitelist = null)
        {
            router.Add(new UnnamedThrottlingRoute(httpMethods, routeTemplate, policy, whitelist));
        }

        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy, IPWhitelist whitelist = null)
        {
            var policyBuilder = new ThrottlingPolicyBuilder(policyName);
            configurePolicy(policyBuilder);
            router.ApplyStrategy(httpMethods, routeTemplate, policyBuilder.Build(), whitelist);
        }

        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName, IPWhitelist whitelist = null)
        {
            IThrottlingPolicy policy;
            if (!router.PolicyMap.TryGetValue(policyName, out policy))
            {
                throw new InvalidOperationException("Not policy named '" + policyName + "'");
            }

            router.ApplyStrategy(httpMethods, routeTemplate, policy, whitelist);
        }

        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] IThrottlingPolicy policy, IPWhitelist whitelist = null)
        {
            router.ApplyStrategy(new[] { httpMethod }, routeTemplate, policy, whitelist);
        }

        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy, IPWhitelist whitelist = null)
        {
            router.ApplyStrategy(new[] { httpMethod }, routeTemplate, policyName, configurePolicy, whitelist);
        }

        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName, IPWhitelist whitelist = null)
        {
            router.ApplyStrategy(new[] { httpMethod }, routeTemplate, policyName, whitelist);
        }

        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, [NotNull] string routeTemplate, [NotNull] IThrottlingPolicy policy, IPWhitelist whitelist = null)
        {
            router.ApplyStrategy((IEnumerable<string>)null, routeTemplate, policy, whitelist);
        }
        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, [NotNull] string routeTemplate, [NotNull] string policyName, IPWhitelist whitelist = null)
        {
            router.ApplyStrategy((IEnumerable<string>)null, routeTemplate, policyName, whitelist);
        }

        public static void ApplyStrategy([NotNull]this IThrottlingRouter router, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy, IPWhitelist whitelist = null)
        {
            router.ApplyStrategy((IEnumerable<string>)null, routeTemplate, policyName, configurePolicy, whitelist);
        }
    }
}