using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public static class ThrottleBuilderExtensions
    {
        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] ThrottlePolicy policy)
        {
            builder.ApplyPolicy(httpMethods, routeTemplate, new TranscientThrottlePolicyBuilder(policy));
        }
        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] IThrottlePolicyBuilder policyBuilder)
        {
            builder.Add(new UnnamedThrottleRoute(httpMethods, routeTemplate, policyBuilder));
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlePolicyBuilder> configurePolicy)
        {
            var policyBuilder = new ThrottlePolicyBuilder(policyName);
            configurePolicy(policyBuilder);
            builder.ApplyPolicy(httpMethods, routeTemplate, policyBuilder);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            builder.ApplyPolicy(httpMethods, routeTemplate, new ThrottleNamedPolicyBuilder(policyName));
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] ThrottlePolicy policy)
        {
            builder.ApplyPolicy(new[] { httpMethod }, routeTemplate, policy);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlePolicyBuilder> configurePolicy)
        {
            builder.ApplyPolicy(new[] { httpMethod }, routeTemplate, policyName, configurePolicy);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, [NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            builder.ApplyPolicy(new[] { httpMethod }, routeTemplate, policyName);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, [NotNull] string routeTemplate, [NotNull] ThrottlePolicy policy)
        {
            builder.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policy);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            builder.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policyName);
        }

        public static void ApplyPolicy([NotNull]this IThrottleRouteBuilder builder, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlePolicyBuilder> configurePolicy)
        {
            builder.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policyName, configurePolicy);
        }
    }
}