using System.Collections.Generic;

namespace Throttling
{
    public class ThrottlePolicyBuilder
    {
        private readonly string _policyName;

        public ThrottlePolicyBuilder(string policyName)
        {
            _policyName = policyName;
        }

        public IList<IThrottleRequirement> Requirements { get; } = new List<IThrottleRequirement>();

        public IList<IThrottleExclusion> Exclusions { get; } = new List<IThrottleExclusion>();

        /// <summary>
        /// Adds a list of requirements.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public ThrottlePolicyBuilder AddRequirements(params IThrottleRequirement[] requirements)
        {
            foreach (var requirement in requirements)
            {
                Requirements.Add(requirement);
            }

            return this;
        }

        /// <summary>
        /// Adds a list of exclusions.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public ThrottlePolicyBuilder AddExclusions(params IThrottleExclusion[] exclusions)
        {
            foreach (var exclusion in exclusions)
            {
                Exclusions.Add(exclusion);
            }

            return this;
        }

        public ThrottlePolicy Build()
        {
            return new ThrottlePolicy(Requirements, Exclusions, _policyName);
        }
    }
}