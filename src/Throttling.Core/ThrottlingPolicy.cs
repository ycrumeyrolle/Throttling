using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;

namespace Throttling
{
    public sealed class ThrottlingPolicy : IThrottlingPolicy
    {
        private readonly List<IThrottlingPolicy> _policies = new List<IThrottlingPolicy>();

        public string Name { get; set; }

        public string Category { get; set; }

        public void AddPolicy([NotNull] IThrottlingPolicy policy)
        {
            _policies.Add(policy);
            policy.Category = Category + "_" + policy.Category + "_" + _policies.Count;
            policy.Name = Name;
        }

        public void Configure(ThrottlingOptions options)
        {
            for (int i = 0; i < _policies.Count; i++)
            {
                _policies[i].Configure(options);
            }
        }

        public async Task<IEnumerable<ThrottlingResult>> EvaluateAsync([NotNull] HttpContext context, string routeTemplate)
        {
            List<ThrottlingResult> results = new List<ThrottlingResult>();
            for (int i = 0; i < _policies.Count; i++)
            {
                var policy = _policies[i];
                var policyResult = await policy.EvaluateAsync(context, routeTemplate);
                results.AddRange(policyResult);
            }

            return results;
        }
    }
}