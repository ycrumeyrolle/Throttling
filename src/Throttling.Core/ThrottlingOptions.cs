using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using System.Linq;

namespace Throttling
{
    public class ThrottlingOptions
    {
        private string _defaultPolicyName = "__DefaultThrottlingPolicy";
        private IDictionary<string, IThrottlingPolicy> PolicyMap { get; } = new Dictionary<string, IThrottlingPolicy>();

        public string DefaultPolicyName
        {
            get
            {
                return _defaultPolicyName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _defaultPolicyName = value;
            }
        }

        public RetryAfterMode RetryAfterMode { get; set; }

        public IRateStore RateStore { get; set; }

        public ISystemClock Clock { get; set; }

        public IClientKeyProvider ClientKeyProvider { get; set; }

        internal void ConfigurePolicies()
        {
            foreach (var policy in PolicyMap.Values)
            {
                policy.Configure(this);
            }
        }


        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="policy">The <see cref="IThrottlingPolicy"/> policy to be added.</param>
        public void AddPolicy([NotNull] string name, [NotNull] IThrottlingPolicy policy)
        {
            PolicyMap[name] = policy;
        }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="configurePolicy">A delegate which can use a policy builder to build a policy.</param>
        public void AddPolicy([NotNull] string name, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy)
        {
            var policyBuilder = new ThrottlingPolicyBuilder();
            configurePolicy(policyBuilder);
            PolicyMap[name] = policyBuilder.Build();
        }

        /// <summary>
        /// Gets the policy based on the <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the policy to lookup.</param>
        /// <returns>The <see cref="ThrottlingPolicy"/> if the policy was added.<c>null</c> otherwise.</returns>
        public IThrottlingPolicy GetPolicy([NotNull] string name)
        {
            return PolicyMap.ContainsKey(name) ? PolicyMap[name] : null;
        }

        public IThrottlingRouter Routes { get; set; }

        public void ApplyStrategy([NotNull] string routeTemplate, [NotNull] IThrottlingPolicy policy, IEnumerable<string> httpMethods = null)
        {
            Routes.Add(new ThrottlingRoute(httpMethods, routeTemplate, policy));
        }

        public void ApplyStrategy([NotNull] string routeTemplate, [NotNull] string policyName, IEnumerable<string> httpMethods = null)
        {
            IThrottlingPolicy policy;
            if (!PolicyMap.TryGetValue(policyName, out policy))
            {
                throw new InvalidOperationException("Not policy named '" + policyName + "'");
            }

            ApplyStrategy(routeTemplate, policy);
        }

        // TODO : see how to avoid an optionnal parameter after an Action<>.
        public void ApplyStrategy([NotNull] string routeTemplate, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy, IEnumerable<string> httpMethods = null)
        {
            var policyBuilder = new ThrottlingPolicyBuilder();
            configurePolicy(policyBuilder);
            ApplyStrategy(routeTemplate, policyBuilder.Build());
        }
    }

    public class ThrottlingRoute
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyRouteValues = new RouteValueDictionary();

        private readonly IEnumerable<string> _httpMethods;

        private readonly TemplateMatcher _matcher;

        public ThrottlingRoute(IEnumerable<string> httpMethods, string routeTemplate, IThrottlingPolicy policy)
        {
            _httpMethods = httpMethods;

            var route = TemplateParser.Parse(routeTemplate);
            _matcher = new TemplateMatcher(route, EmptyRouteValues);
            Policy = policy;
        }
        public ThrottlingRoute(string routeTemplate, IThrottlingPolicy policy)
            : this(null, routeTemplate, policy)
        {
        }

        public IThrottlingPolicy Policy { get; set; }

        public bool Match([NotNull] HttpRequest request)
        {
            if (_httpMethods == null || _httpMethods.Contains(request.Method))
            {
                var requestPath = request.Path.Value;

                if (!string.IsNullOrEmpty(requestPath) && requestPath[0] == '/')
                {
                    requestPath = requestPath.Substring(1);
                }

                return _matcher.Match(requestPath) != null;
            }

            return false;
        }
    }

    public class ThrottlingRouteCollection : IThrottlingRouter
    {
        private readonly List<ThrottlingRoute> _routes = new List<ThrottlingRoute>();

        public void Add(ThrottlingRoute route)
        {
            _routes.Add(route);
        }

        public IThrottlingPolicy Route([NotNull] HttpRequest request)
        {
            for (var i = 0; i < _routes.Count; i++)
            {
                var route = _routes[i];

                if (route.Match(request))
                {
                    return route.Policy;
                }
            }

            return null;
        }
    }

    public interface IThrottlingRouter
    {
        void Add(ThrottlingRoute route);

        IThrottlingPolicy Route([NotNull] HttpRequest request);
    }
}