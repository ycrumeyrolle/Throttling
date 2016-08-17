using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public interface IThrottleRouter
    {
        ThrottleStrategy GetThrottleStrategy(HttpContext httpContext, ThrottleOptions _options);
    }

    public static class ThrottleRouterExtensions
    {
#if NET451
        private static readonly string[] EmptyHttpMethods = new string[0];
#else
        private static readonly string[] EmptyHttpMethods = Array.Empty<string>();
#endif
        public static void AddRoute(this ThrottleRouteBuilder router, IEnumerable<string> httpMethods, string routeTemplate, ThrottlePolicy policy)
        {
            router.Add(new UnnamedThrottleRoute(httpMethods, routeTemplate, policy));
        }

        //public static void ApplyPolicy(this ThrottleRouteBuilder router, IEnumerable<string> httpMethods, string routeTemplate, string policyName, Action<ThrottlePolicyBuilder> configurePolicy)
        //{
        //    var policyBuilder = new ThrottlePolicyBuilder(policyName);
        //    configurePolicy(policyBuilder);
        //    router.AddRoute(httpMethods, routeTemplate, policyBuilder.Build());
        //}

        public static void AddRoute(this ThrottleRouteBuilder router, IEnumerable<string> httpMethods, string routeTemplate, string policyName)
        {
            router.Add(new NamedThrottleRoute(httpMethods, routeTemplate, policyName));
        }

        //public static void ApplyPolicy(this ThrottleRouteBuilder router,  string httpMethod,  string routeTemplate,  ThrottlePolicy policy)
        //{		
        //    router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policy);		
        //}		

        //public static void ApplyPolicy(this ThrottleRouteBuilder router,  string httpMethod,  string routeTemplate,  string policyName,  Action<ThrottlePolicyBuilder> configurePolicy)
        //{		
        //    router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policyName, configurePolicy);		
        //}		

        //public static void ApplyPolicy(this ThrottleRouteBuilder router,  string httpMethod,  string routeTemplate,  string policyName)
        //{		
        //    router.ApplyPolicy(new[] { httpMethod }, routeTemplate, policyName);		
        //}		

        //public static void ApplyPolicy(this ThrottleRouteBuilder router,  string routeTemplate,  ThrottlePolicy policy)
        //{		
        //    router.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policy);		
        //}		
        public static void ApplyPolicy(this ThrottleRouteBuilder router, string routeTemplate, string policyName)
        {
            router.AddRoute(EmptyHttpMethods, routeTemplate, policyName);
        }

        //public static void ApplyPolicy(this ThrottleRouteBuilder router,  string routeTemplate,  string policyName,  Action<ThrottlePolicyBuilder> configurePolicy)
        //{		
        //    router.ApplyPolicy((IEnumerable<string>)null, routeTemplate, policyName, configurePolicy);		
        //}		
    }
}