//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNet.Http;
//using Microsoft.AspNet.Routing;
//using Microsoft.AspNet.Routing.Template;
//using Microsoft.Framework.OptionsModel;

//namespace Throttling.Routing
//{
//    public class RoutingThrottlingPolicyProvider : IThrottlingPolicyProvider
//    {
//        private readonly ThrottlingOptions _options;

//        /// <summary>
//        /// Creates a new instance of <see cref="RoutingThrottlingPolicyProvider"/>.
//        /// </summary>
//        /// <param name="options">The options configured for the application.</param>
//        public RoutingThrottlingPolicyProvider(IOptions<ThrottlingOptions> options)
//        {
//            _options = options.Options;
//        }

//        /// <inheritdoc />
//        public virtual Task<IThrottlingPolicy> GetThrottlingPolicyAsync(HttpContext context, string policyName)
//        {
//            var route1 = TemplateParser.Parse("test/action/{int:id}");
//            var matcher1 = new TemplateMatcher(route1, new RouteValueDictionary());

//            var policy = _options.GetPolicy(policyName ?? _options.DefaultPolicyName);
//            if (policy != null && policy.HttpMethods.Contains(context.Request.Method))
//            {
//                var requestPath = context.HttpContext.Request.Path.Value;

//                if (!string.IsNullOrEmpty(requestPath) && requestPath[0] == '/')
//                {
//                    requestPath = requestPath.Substring(1);
//                }

//                var match = matcher1.Match(requestPath);
//                if (match == null)
//                {
//                    return Task.FromResult<IThrottlingPolicy>(null);
//                }
//            }

//            return Task.FromResult(policy);
//        }
//    }
//}